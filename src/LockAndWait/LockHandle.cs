namespace LockAndWait
{
    public sealed class LockHandle(string key, string token, DateTimeOffset acquiredAt, TimeSpan ttl)
    {
        public string Key { get; } = key ?? throw new ArgumentNullException(nameof(key));
        public string Token { get; } = token ?? throw new ArgumentNullException(nameof(token));
        public DateTimeOffset AcquiredAt { get; } = acquiredAt;
        public TimeSpan Ttl { get; } = ttl;

        public override string ToString() => $"{Key}:{Token} (ttl={Ttl})";
    }
}