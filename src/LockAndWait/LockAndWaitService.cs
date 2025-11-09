using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LockAndWait
{
    public sealed class LockAndWaitService : ILockAndWaitService
    {
        private readonly IDatabase _db;
        private const string ReleaseScript = @"
            if redis.call('GET', KEYS[1]) == ARGV[1] then
                return redis.call('DEL', KEYS[1])
            else
                return 0
            end";

        public LockAndWaitService(IDatabase database)
        {
            _db = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<LockHandle?> AcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (ttl <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(ttl), "TTL must be positive.");

            var token = Guid.NewGuid().ToString("N");
            var acquired = await _db.StringSetAsync(key, token, ttl, when: When.NotExists).ConfigureAwait(false);
            if (!acquired) return null;

            return new LockHandle(key, token, DateTimeOffset.UtcNow, ttl);
        }

        public async Task<bool> ReleaseAsync(LockHandle handle, CancellationToken cancellationToken = default)
        {
            if (handle is null) throw new ArgumentNullException(nameof(handle));

            var res = await _db.ScriptEvaluateAsync(
                ReleaseScript,
                new RedisKey[] { handle.Key },
                new RedisValue[] { handle.Token }
            ).ConfigureAwait(false);

            if (res.IsNull) return false;
            var releasedCount = (long)res;  // RedisResult -> long
            return releasedCount == 1;
        }

        public async Task<bool> WaitAsync(string key, TimeSpan? pollInterval = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            var delay = pollInterval ?? TimeSpan.FromMilliseconds(100);
            if (delay <= TimeSpan.Zero) delay = TimeSpan.FromMilliseconds(50);

            var sw = Stopwatch.StartNew();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var exists = await _db.KeyExistsAsync(key).ConfigureAwait(false);
                if (!exists) return true;

                if (timeout is not null && sw.Elapsed >= timeout.Value)
                    return false;

                var remaining = timeout is null
                    ? delay
                    : TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds, Math.Max(0, (timeout.Value - sw.Elapsed).TotalMilliseconds)));

                if (remaining <= TimeSpan.Zero) remaining = TimeSpan.FromMilliseconds(10);
                await Task.Delay(remaining, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
