# EntityFramework.Caching
Entity Framework does not currently support caching of query results. A sample EF Caching provider is available for Entity Framework version 5 and earlier but due to changes to the provider model this sample provider does not work with Entity Framework 6 and newer. This project is filling the gap by enabling caching of query results for Entity Framework 6.1 applications. 
This created based on [EFCache](https://github.com/moozzyk/EFCache) introduced by **Pawel Kadluczka** but with a main difference.
My main approach here is to cache databse entites as well as queries. thus approach reduce queries sending to DB many many more than caching just query results.

# How to get it

You can get it from NuGet - just install the [EF.Caching.Cache NuGet package](https://www.nuget.org/packages/EF.Caching.Cache/)
If you wanna use **Memory** as cache provider, add package [EF.Caching.InMemory NuGet package](https://www.nuget.org/packages/EF.Caching.InMemory)
If you prefer to use **Redis** as cache provider, use package [EF.Caching.Redis.StackExchange NuGet package](https://www.nuget.org/packages/EF.Caching.Redis.StackExchange) or [EF.Caching.Redis.ServiceStack NuGet package](https://www.nuget.org/packages/EF.Caching.Redis.ServiceStack)

# How to use it

 To make it work, just add `EntityFrameworkCache.Initialize()` method at app startup (before EF is used) and use proper config related to your cache provider:

 **In Memory:**

```C#
EntityFrameworkCache.Initialize(new InMemoryCache());
```
**Redis using ServiceStack library:**

```C#
EntityFrameworkCache.Initialize(new ServiceStackRedisCache(new ServiceStackRedisCacheOptions
{
    Host = "localhost"
}));
```

**Redis using StackExchange library:**

```C#
EntityFrameworkCache.Initialize(new StackExchangeRedisCache(new StackExchangeRedisCacheOptions()
{
    Configuration = "localhost:6379"
}));
```
