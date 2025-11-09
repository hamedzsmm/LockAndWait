using System;
using System.Threading;
using System.Threading.Tasks;

namespace LockAndWait
{
    public interface ILockAndWaitService
    {
        Task<LockHandle?> AcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
        Task<bool> ReleaseAsync(LockHandle handle, CancellationToken cancellationToken = default);
        Task<bool> WaitAsync(string key, TimeSpan? pollInterval = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
    }
}
