using System;

namespace LockAndWait
{
    public sealed class LockHandle
    {
        public string Key { get; }
        public string Token { get; }
        public DateTimeOffset AcquiredAt { get; }
        public TimeSpan Ttl { get; }

        public LockHandle(string key, string token, DateTimeOffset acquiredAt, TimeSpan ttl)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            AcquiredAt = acquiredAt;
            Ttl = ttl;
        }

        public override string ToString() => $"{Key}:{Token} (ttl={Ttl})";
    }
}
