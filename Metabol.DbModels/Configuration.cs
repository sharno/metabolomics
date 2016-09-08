using System.Data.Entity;
using System.Data.Entity.Core.Common;
using EFCache;

namespace Metabol.DbModels
{
    public class Configuration : DbConfiguration
    {
        public static readonly InMemoryCache Cache = new InMemoryCache();
        public Configuration()
        {
            var transactionHandler = new CacheTransactionHandler(Cache);

            AddInterceptor(transactionHandler);

            var cachingPolicy = new CachingPolicy();

            Loaded +=
              (sender, args) => args.ReplaceService<DbProviderServices>(
                (s, _) => new CachingProviderServices(s, transactionHandler,
                  cachingPolicy));
        }
    }
}
