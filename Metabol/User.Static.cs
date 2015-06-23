using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Metabol
{
    public partial class User
    {
        private static readonly MemoryCache UserCache = new MemoryCache("users");
        private static readonly MemoryCache ResultCache = new MemoryCache("results");

        private static Iteration CacheResult(string userKey, Iteration it)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(3) };
            //if (ResultCache.Contains($"{userKey}-{it.Id}"))
            //    return (Iteration)ResultCache.Get($"{userKey}-{it.Id}");

            ResultCache.Add($"{userKey}-{it.Id}", it, policy);
            return it;
        }

        private static User GetUser(string userKey)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(3) };
            if (UserCache.Contains(userKey))
                return (User)UserCache.Get(userKey);

            var user = new User(userKey);
            UserCache.Add(userKey, user, policy);
            return user;
        }

        public static IEnumerable<Iteration> Step(string userkey, int p)
        {
            if (ResultCache.Contains($"{userkey}-{p}"))
            {
                yield return ResultCache.Get($"{userkey}-{p}") as Iteration;
                yield break;
            }

            var citer = GetUser(userkey).Worker.Iteration - 1;
            var step = p > citer ? (p - citer) : 1;

            foreach (var it in GetUser(userkey).Worker.Step(step))
                yield return CacheResult(userkey, it);

        }

        public static object StartNew()
        {
            var id = Guid.NewGuid().ToString();

            Task.Run(delegate
            {
                var user = GetUser(id);
                user.Worker.Start();
            });

            return new { key = id };
        }

    }
}
