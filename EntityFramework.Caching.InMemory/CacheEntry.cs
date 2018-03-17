using System;

namespace EntityFramework.Caching.InMemory
{
    internal class CacheEntry
    {
        public CacheEntry(byte[] value, TimeSpan? slidingExpiration,DateTimeOffset? absoluteExpiration=null)
        {
            Value = value;
            SlidingExpiration = slidingExpiration;
            AbsoluteExpiration = absoluteExpiration;
            LastAccess = DateTimeOffset.Now;
        }

        public byte[] Value { get; }
        public TimeSpan? SlidingExpiration { get; }
        public DateTimeOffset? AbsoluteExpiration { get; }
        public DateTimeOffset LastAccess { get; set; }
        public bool IsExpired()
        {
            DateTimeOffset now= DateTimeOffset.Now;
            return AbsoluteExpiration < now || (now - LastAccess) > SlidingExpiration;
        }
    }
    internal class IdCacheEntry
    {
        public IdCacheEntry(byte[][] value, TimeSpan? slidingExpiration,DateTimeOffset? absoluteExpiration=null)
        {
            Value = value;
            SlidingExpiration = slidingExpiration;
            AbsoluteExpiration = absoluteExpiration;
            LastAccess = DateTimeOffset.Now;
        }

        public byte[][] Value { get; }
        public TimeSpan? SlidingExpiration { get; }
        public DateTimeOffset? AbsoluteExpiration { get; }
        public DateTimeOffset LastAccess { get; set; }
        public bool IsExpired()
        {
            DateTimeOffset now= DateTimeOffset.Now;
            return AbsoluteExpiration < now || (now - LastAccess) > SlidingExpiration;
        }
    }
}

