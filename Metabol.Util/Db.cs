namespace Metabol.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    using Metabol.Util.DB2;

    public class Db
    {
        public static readonly MetabolicNetworkDBContext Context = new MetabolicNetworkDBContext();

        private static readonly MemoryCache CacheSpecieses = new MemoryCache("species");
        private static readonly MemoryCache CacheReactions = new MemoryCache("reactions");
        private static readonly MemoryCache CacheReactionsSpecies = new MemoryCache("reactionspecies");

        public static readonly Dictionary<Guid, dynamic> AllReactionCache2 = new Dictionary<Guid, dynamic>();
        public static readonly Dictionary<Guid, dynamic> AllStoichiometryCache2 = new Dictionary<Guid, dynamic>();

        public const byte ReactantId = 1;
        public const byte ProductId = 2;
        public const byte ReversibleId = 3;

        //public const byte ModifierId = 3;

        private static dynamic InvolvedReactionCount(Guid id)
        {
            if (AllReactionCache2.ContainsKey(id)) return AllReactionCache2[id];
            var species = Context.Species.Single(s => s.id == id);
            var product = species.ReactionSpecies.Count(rs => rs.roleId == ProductId);
            var reactant = species.ReactionSpecies.Count(rs => rs.roleId == ReactantId);
            //MnContext.MetaboliteReactionCount.Add(new MetaboliteReactionCount
            //{
            //    id = Guid.NewGuid(),
            //    consumerCount = reactant,
            //    producerCount = product,
            //    speciesId = id,
            //});
            //MnContext.SaveChanges();
            AllReactionCache2[id] = new { Consumers = reactant, Producers = product };
            return AllReactionCache2[id];
        }

        private static dynamic InvolvedReactionStoch(Guid id)
        {
            if (AllStoichiometryCache2.ContainsKey(id)) return AllStoichiometryCache2[id];
            var rsum = Context.ReactionSpecies.Where(r => r.speciesId == id && r.roleId == ReactantId).ToList().Sum(e => e.stoichiometry);
            var psum = Context.ReactionSpecies.Where(r => r.speciesId == id && r.roleId == ProductId).ToList().Sum(e => e.stoichiometry);
            //MnContext.MetaboliteReactionStoichiometry.Add(new MetaboliteReactionStoichiometry
            //{
            //    id = Guid.NewGuid(),
            //    consumerStoch = rsum,
            //    producerStoch = psum,
            //    speciesId = id,
            //});
            //MnContext.SaveChanges();

            AllStoichiometryCache2[id] = new { Consumers = rsum, Producers = psum };
            return AllStoichiometryCache2[id];
        }

        public static Reaction CachedR(Guid id)
        {
            if (CacheReactions.Contains(id.ToString()))
                return (Reaction)CacheReactions.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = Context.Reactions.Single(r => r.id == id);
            CacheReactions.Add(id.ToString(), ss, policy);
            return ss;
        }

        public static Species CachedS(Guid id)
        {
            if (CacheSpecieses.Contains(id.ToString()))
                return (Species)CacheSpecieses.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = Context.Species.Single(s => s.id == id);
            CacheSpecieses.Add(id.ToString(), ss, policy);
            return ss;
        }

        public static ReactionSpecy CachedRs(Guid rid, Guid sid)
        {
            var key = rid.ToString() + sid;
            if (CacheReactionsSpecies.Contains(key))
                return (ReactionSpecy)CacheReactionsSpecies.Get(key);

            foreach (var ss in Context.ReactionSpecies.Where(r => r.reactionId == rid))//GetAllReactionsSpeciesForOneReaction(rid)
            {
                var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
                CacheReactionsSpecies.Add(rid.ToString() + ss.speciesId, ss, policy);
            }

            return (ReactionSpecy)CacheReactionsSpecies.Get(key);
        }

        public static int TotalReactions(Guid id)
        {
            InvolvedReactionCount(id);
            var sum = AllReactionCache2[id].Consumers + AllReactionCache2[id].Producers;
            //if sum==0, k is probably Modifier
            return sum == 0 ? Int32.MaxValue : sum;
        }

        public static int GetReactionCountSum(Guid id)
        {
            return InvolvedReactionCount(id).Consumers + InvolvedReactionCount(id).Producers;
        }

        public static dynamic GetReactionCount(Guid id)
        {
            return InvolvedReactionCount(id);
        }

        public static dynamic GetStoichiometry(Guid id)
        {
            dynamic rval;

            try
            {
                rval = InvolvedReactionStoch(id);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return rval;

        }
    }
}
