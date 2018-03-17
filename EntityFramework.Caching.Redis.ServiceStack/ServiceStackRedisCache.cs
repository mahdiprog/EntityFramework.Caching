using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Caching.Abstractions;
using ServiceStack.Redis;
using Microsoft.Extensions.Options;

namespace EntityFramework.Caching.Redis.ServiceStack
{
    public class ServiceStackRedisCache : ICache
    {
        private readonly IRedisClientsManager _redisManager;
        private readonly ServiceStackRedisCacheOptions _options;
        private const string DelDependantsScript = (@"
                local dependants= redis.call('ZRANGE', KEYS[1], 0,-1)
                local count= redis.call('ZCARD', KEYS[1])
                for i=1,count do
                    redis.call('DEL',dependants[i]) 
                end
                redis.call('DEL',KEYS[1]) 
                return count");
        public ServiceStackRedisCache(IOptions<ServiceStackRedisCacheOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;

            var host = $"{_options.Password}@{_options.Host}:{_options.Port}";
            RedisConfig.VerifyMasterConnections = false;
            _redisManager = new RedisManagerPool(host);
        }

        public ICacheOptions Options => _options;

        public byte[] Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                if (client.Exists(key) == 1)
                {
                    return client.Get(key);
                }
            }
            return null;
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(Get(key));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
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

            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                var expireInSeconds = GetExpireInSeconds(options);
                if (expireInSeconds > 0)
                {
                    client.SetEx(key, expireInSeconds, value);
                    client.SetEx(GetExpirationKey(key), expireInSeconds, Encoding.UTF8.GetBytes(expireInSeconds.ToString()));
                }
                else
                {
                    client.Set(key, value);
                }
            }
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Set(key, value, options), token);
        }

        

        public void Refresh(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                if (client.Exists(key) == 1)
                {
                    var value = client.Get(key);
                    if (value != null)
                    {
                        var expirationValue = client.Get(GetExpirationKey(key));
                        if (expirationValue != null)
                        {
                            client.Expire(key, int.Parse(Encoding.UTF8.GetString(expirationValue)));
                        }
                    }
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

            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                client.Del(key);
            }
        }

        public Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Remove(key), token);
        }

        private int GetExpireInSeconds(DistributedCacheEntryOptions options)
        {
            if (options.SlidingExpiration.HasValue)
            {
                return (int)options.SlidingExpiration.Value.TotalSeconds;
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                return (int)options.AbsoluteExpirationRelativeToNow.Value.TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        private string GetExpirationKey(string key)
        {
            return key + $"-{nameof(DistributedCacheEntryOptions)}";
        }

        public void SetDependency(string setKey, string dependantKey, DistributedCacheEntryOptions options)
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

            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
               client.ZAdd(setKey, 0, Encoding.UTF8.GetBytes(dependantKey));
            }
        }

        public Task SetDependencyAsync(string setKey, string dependantKey, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => SetDependency(setKey, dependantKey, options), token);
        }

        public void ExpireDependendants(string setKey)
        {
            if (setKey == null)
            {
                throw new ArgumentNullException(nameof(setKey));
            }

            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                var removedQueries = client.EvalInt(DelDependantsScript, 1, Encoding.UTF8.GetBytes(setKey));
            }
        }

        public Task ExpireDependendantsAsync(string setKey, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => ExpireDependendants(setKey), token);
        }
        public void SetIdArray(string key, byte[][] value, DistributedCacheEntryOptions options = null)
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
            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                foreach (var item in value)
                {
                    client.SAdd(key, item);
                }
            }
        }

        public async Task SetIdArrayAsync(string key, byte[][] value, DistributedCacheEntryOptions options = null, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => SetIdArray(key, value), token);
        }

        public byte[][] GetIdArray(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            using (var client = _redisManager.GetClient() as IRedisNativeClient)
            {
                return client.SMembers(key);
            }
        }
        public async Task<byte[][]> GetIdArrayAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return await Task.Run(() => GetIdArray(key), token);
        }
       
    }
}