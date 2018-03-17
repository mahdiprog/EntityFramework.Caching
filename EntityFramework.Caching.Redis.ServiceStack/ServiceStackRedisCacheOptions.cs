using EntityFramework.Caching.Abstractions;
using Microsoft.Extensions.Options;

namespace EntityFramework.Caching.Redis.ServiceStack
{
    public class ServiceStackRedisCacheOptions : IOptions<ServiceStackRedisCacheOptions>,ICacheOptions
    {
        public string Host { get; set; }

        public int Port { get; set; } = 6379;

        public string Password { get; set; }

        public int ExpirySeconds { get; set; } = 0;
        public bool EnableQueryCache { get; set; } = true;
        ServiceStackRedisCacheOptions IOptions<ServiceStackRedisCacheOptions>.Value
        {
            get { return this; }
        }
    }
}