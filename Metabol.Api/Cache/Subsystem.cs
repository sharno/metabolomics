using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Metabol.Api.Cache
{
    public static class SubsystemCache
    {

        private static readonly MemoryCache cache = new MemoryCache("subsystem");
        private static readonly CacheItemPolicy polity = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(3) };

        public static Guid Cache(Dictionary<string, string[]> data)
        {
            var key = Guid.NewGuid();
            cache.Add(key.ToString(), data, polity);
            return key;
        }

        public static Guid Cache(Dictionary<string, string[]> data, Guid key)
        {
            cache.Add(key.ToString(), data, polity);
            return key;
        }

        public static Dictionary<string, string[]> GetFromCache(Guid key)
        {
            return cache.Get(key.ToString()) as Dictionary<string, string[]>;   
        }
    }
}