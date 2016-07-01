﻿using System.Data.Entity;
using System.Data.Entity.Core.Common;
using EFCache;

namespace Metabol.DbModels
{
  public class Configuration : DbConfiguration
  {
    public Configuration()
    {
      var transactionHandler = new CacheTransactionHandler(new InMemoryCache());

      AddInterceptor(transactionHandler);

      var cachingPolicy = new CachingPolicy();

      Loaded +=
        (sender, args) => args.ReplaceService<DbProviderServices>(
          (s, _) => new CachingProviderServices(s, transactionHandler,
            cachingPolicy));
    }
  }


}
