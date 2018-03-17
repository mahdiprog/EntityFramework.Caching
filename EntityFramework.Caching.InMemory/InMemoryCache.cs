using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Caching.Abstractions;
using Microsoft.Extensions.Options;

namespace EntityFramework.Caching.InMemory
{
    public class InMemoryCache : ICache
    {
        private readonly InMemoryCacheOptions _options;
        private static readonly Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
        private static readonly Dictionary<string, IdCacheEntry> _IdArray = new Dictionary<string, IdCacheEntry>();
        private static readonly Dictionary<string, List<string>> _dependants = new Dictionary<string, List<string>>();

        public InMemoryCache(IOptions<InMemoryCacheOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
        }
        public InMemoryCache()
        {
            _options= new InMemoryCacheOptions();
        }

        #region Implementation of ICache

        public ICacheOptions Options => _options;

        public byte[] Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            lock (_cache)
            {
                CacheEntry entry;
                if (_cache.TryGetValue(key, out entry))
                {
                    if (entry.IsExpired())
                        _cache.Remove(key);
                    else
                    {
                        entry.LastAccess = DateTime.Now;
                        return entry.Value;
                    }
                }
            }

            return null;
        }


        public Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(Get(key));
        }

        public void Set(string key, byte[] value,
            DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                options = new DistributedCacheEntryOptions();
            }
            lock (_cache)
            {
                _cache[key] = new CacheEntry(value, options.SlidingExpiration,options.AbsoluteExpiration);
            }
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Set(key, value, options), token);
        }

        public void SetIdArray(string key, byte[][] value,
            DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (options == null)
            {
                options = new DistributedCacheEntryOptions();
            }
            lock (_cache)
            {
                _IdArray[key] = new IdCacheEntry(value, options.SlidingExpiration,options.AbsoluteExpiration);
            }
        }

        public async Task SetIdArrayAsync(string key, byte[][] value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => SetIdArray(key, value), token);
        }

        public byte[][] GetIdArray(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            lock (_IdArray)
            {
                IdCacheEntry entry;
                if (_IdArray.TryGetValue(key, out entry))
                {
                    if (entry.IsExpired())
                        _IdArray.Remove(key);
                    else
                    {
                        entry.LastAccess = DateTime.Now;
                        return entry.Value;
                    }
                }
            }

            return null;
        }

        public async Task<byte[][]> GetIdArrayAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return await Task.Run(() => GetIdArray(key), token);
        }

        public void Refresh(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            lock (_cache)
            {
                CacheEntry entry;
                if (_cache.TryGetValue(key, out entry))
                {
                    entry.LastAccess = DateTime.Now;
                }
            }
        }

        public Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return Task.Run(() => Refresh(key), token);
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            lock (_cache)
            {
                _cache.Remove(key);
            }
        }

        public Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Remove(key), token);
        }

        public void SetDependency(string setKey, string dependantKey,
            DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions))
        {
            if (setKey == null)
            {
                throw new ArgumentNullException(nameof(setKey));
            }

            if (dependantKey == null)
            {
                throw new ArgumentNullException(nameof(dependantKey));
            }

            if (options == null)
            {
                options = new DistributedCacheEntryOptions();
            }
            lock (_dependants)
            {
                List<string> entry;
                if (_dependants.TryGetValue(setKey, out entry))
                {
                    if(!entry.Contains(dependantKey))
                        entry.Add(dependantKey);
                }
                _dependants[setKey] = new List<string>{dependantKey};
            }
        }

        public Task SetDependencyAsync(string setKey, string dependantKey, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => SetDependency(setKey, dependantKey, options), token);
        }

        public void ExpireDependendants(string setKey)
        {
            if (setKey == null)
            {
                throw new ArgumentNullException(nameof(setKey));
            }
            lock (_dependants)
            {
                List<string> entry;
                if (_dependants.TryGetValue(setKey, out entry))
                {
                    foreach (string dependantKey in entry)
                    {
                        Remove(dependantKey);
                    }
                }
                _dependants.Remove(setKey);
            }
        }

        public Task ExpireDependendantsAsync(string setKey, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => ExpireDependendants(setKey), token);
        }

        #endregion
        
    }
}
