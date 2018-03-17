using EntityFramework.Caching.Abstractions;
using Microsoft.Extensions.Options;

namespace EntityFramework.Caching.Redis.StackExchange
{
    /// <summary>
    /// Configuration options for <see cref="ServiceStackRedisCache"/>.
    /// </summary>
    public class StackExchangeRedisCacheOptions : IOptions<StackExchangeRedisCacheOptions>,ICacheOptions
    {
        /// <summary>
        /// The configuration used to connect to Redis.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// The Redis instance name.
        /// </summary>
        public string InstanceName { get; set; }

        public int ExpirySeconds { get; set; } = 0;
        public bool EnableQueryCache { get; set; } = true;

        StackExchangeRedisCacheOptions IOptions<StackExchangeRedisCacheOptions>.Value => this;
    }
}