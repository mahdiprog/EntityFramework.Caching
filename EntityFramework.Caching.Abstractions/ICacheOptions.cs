using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework.Caching.Abstractions
{
    public interface ICacheOptions
    {
        int ExpirySeconds { get; set; }
        bool EnableQueryCache { get; set; }
    }
}
