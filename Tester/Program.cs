using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Caching.Cache;
using EntityFramework.Caching.InMemory;
using EntityFramework.Caching.Redis.ServiceStack;
using EntityFramework.Caching.Redis.StackExchange;
using ServiceStack.Support;


namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //EntityFrameworkCache.Initialize(new ServiceStackRedisCache(new ServiceStackRedisCacheOptions
            //{
            //    Host = "172.16.6.40",
            //    Password = "SCFLgskjhb9wqdi0weugbfjpsodkf8"
            //}));
            //EntityFrameworkCache.Initialize(new StackExchangeRedisCache(new StackExchangeRedisCacheOptions()
            //{
            //    Configuration="172.16.6.40:6379,serviceName=Sky-Redis01-Cluster01,password=SCFLgskjhb9wqdi0weugbfjpsodkf8"
            //}));
            EntityFrameworkCache.Initialize(new InMemoryCache());

            litlyEntities db = new litlyEntities();
            var urls = db.Urls.Where(u => u.CompanyId == 1).Select(u => new { u.ShortUrl, u.LongUrl }).ToList();

            db.Urls.Add(new Url { ShortUrl = "test", LongUrl = "test.com", CompanyId = 1, UserId = "54731be8-2b39-4bf7-be15-19ecabbb821e", RegDate = DateTime.Now });
            db.SaveChanges();
            var url = db.Urls.First(u => u.ShortUrl == "test");
            url.Hits = 10;
            db.SaveChanges();

            db.Urls.Remove(url);
            db.SaveChanges();

            urls = db.Urls.Where(u => u.CompanyId == 1).Select(u => new { u.ShortUrl, u.LongUrl }).ToList();
        }
    }
}
