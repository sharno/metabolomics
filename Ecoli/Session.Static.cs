using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Metabol.DbModels.Models;

namespace Ecoli
{
    public partial class Session
    {
        private static readonly MemoryCache ResultCache = new MemoryCache("results");
        private static readonly MemoryCache WorkerCache = new MemoryCache("workers");

        public static TheAlgorithm CacheOrGetWorker(string key)
        {
            if (WorkerCache.Contains(key))
                return WorkerCache.Get(key) as TheAlgorithm;

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(3) };
            var worker = new TheAlgorithm();
            WorkerCache.Add(key, worker, policy);
            return worker;
        }

        public static IterationModels CacheOrGetResult(string skey, IterationModels it)
        {
            var key = $"{skey}-{it.Id}";
            if (ResultCache.Contains(key))
                return ResultCache.Get(key) as IterationModels;

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(3) };

            ResultCache.Add(key, it, policy);
            return it;
        }

        public static IterationModels GetResult(string skey, int it)
        {
            var key = $"{skey}-{it}";
            if (ResultCache.Contains(key))
                return ResultCache.Get(key) as IterationModels;
            return IterationModels.Empty;
        }

        public static IEnumerable<IterationModels> Step(string key, int iteration)
        {
            if (ResultCache.Contains($"{key}-{iteration}"))
            {
                yield return ResultCache.Get($"{key}-{iteration}") as IterationModels;
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
            //;
            //foreach (var it in GetUser(userkey).Worker.Step())
            yield return CacheOrGetResult(key, it);
        }

        public static string StartNew()
        {
            var id = Guid.NewGuid().ToString();

            //Task.Run(delegate
            //{
            CacheOrGetWorker(id).Start();
            //});

            return id;
        }

        public static string StartNew(ConcentrationChange[] z)
        {
            var id = Guid.NewGuid().ToString();

            //Task.Run(delegate
            //{
            CacheOrGetWorker(id).Start(z);
            //user.Worker.Start(z);
            //});

            return id;
        }
    }
}
