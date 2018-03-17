using System;
using System.Collections.Generic;
using System.Text;
using EntityFramework.Caching.Abstractions;
using Microsoft.Extensions.Options;

namespace EntityFramework.Caching.InMemory
{
    public class InMemoryCacheOptions:IOptions<InMemoryCacheOptions>,ICacheOptions
    {
        #region Implementation of ICacheOptions

        public int ExpirySeconds { get; set; }
        public bool EnableQueryCache { get; set; }

        #endregion

        #region Implementation of IOptions<out InMemoryCacheOptions>

        InMemoryCacheOptions IOptions<InMemoryCacheOptions>.Value => this;

        #endregion
    }
}
