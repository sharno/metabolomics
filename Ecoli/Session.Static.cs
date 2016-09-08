using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Metabol.DbModels.Models;

namespace Ecoli
{
    public partial class Session
    {
        private static readonly MemoryCache resultCache = new MemoryCache("results");
        private static readonly MemoryCache workerCache = new MemoryCache("workers");
        private static readonly CacheItemPolicy cachePolity = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(3) };

        public static TheAlgorithm CacheOrGetWorker(string key)
        {
            if (workerCache.Contains(key))
                return workerCache.Get(key) as TheAlgorithm;

            var worker = new TheAlgorithm();
            workerCache.Add(key, worker, cachePolity);
            return worker;
        }

        public static IterationModels CacheOrGetResult(string skey, IterationModels it)
        {
            var key = $"{skey}-{it.IterationNumber}";
            if (resultCache.Contains(key))
                return resultCache.Get(key) as IterationModels;

            resultCache.Add(key, it, cachePolity);
            return it;
        }

        public static IterationModels GetResult(string skey, int it)
        {
            var key = $"{skey}-{it}";
            if (resultCache.Contains(key))
                return resultCache.Get(key) as IterationModels;
            return IterationModels.Empty;
        }

        public static IEnumerable<IterationModels> Step(string key, int iteration)
        {
            if (resultCache.Contains($"{key}-{iteration}"))
            {
                yield return resultCache.Get($"{key}-{iteration}") as IterationModels;
                yield break;
            }

            var citer = CacheOrGetWorker(key).Iteration - 1;
            var step = iteration > citer ? iteration - citer : 1;
            var it = CacheOrGetWorker(key).Step();
            CacheOrGetResult(key, it);
            for (var i = 0; i < step - 1; i++)
            {
                it = CacheOrGetWorker(key).Step();
                CacheOrGetResult(key, it);
            }
            yield return CacheOrGetResult(key, it);
        }

        public static string StartNew()
        {
            var id = Guid.NewGuid().ToString();
            CacheOrGetWorker(id).Start();
            return id;
        }

        public static string StartNew(ConcentrationChange[] z)
        {
            var id = Guid.NewGuid().ToString();
            CacheOrGetWorker(id).Start(z);
            return id;
        }
    }
}
