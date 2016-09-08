using System;
using System.Collections.Generic;
using System.Linq;
using Metabol.DbModels.DB;
using Metabol.DbModels.Models;

namespace Metabol.DbModels
{
    public partial class Db
    {
        public static readonly EcoliCoreModel Context = new EcoliCoreModel();
        public static CacheModel Cache ;
        public static readonly MetabolApiDbContext ApiDbContext = new MetabolApiDbContext();

        public const double PseudoReactionStoichiometry = 1.0;

        public const byte ReactantId = 1;
        public const byte ProductId = 2;
        public const byte ReversibleId = 3;
    }
}
