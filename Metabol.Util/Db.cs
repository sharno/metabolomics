using EFCache;

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

            AllReactionCache2[id] = new { Consumers = reactant, Producers = product };
            return AllReactionCache2[id];
        }

        public static dynamic InvolvedReactionStoch(Guid id)
        {
            if (AllStoichiometryCache2.ContainsKey(id)) return AllStoichiometryCache2[id];
            var rsum = Context.ReactionSpecies.Where(r => r.speciesId == id && r.roleId == ReactantId).ToList().Sum(e => e.stoichiometry);
            var psum = Context.ReactionSpecies.Where(r => r.speciesId == id && r.roleId == ProductId).ToList().Sum(e => e.stoichiometry);

            rsum += Context.CycleConnections.Where(cc => cc.metaboliteId == id && (cc.roleId == ReactantId || cc.roleId == ReversibleId)).ToList().Sum(cc => cc.stoichiometry);
            psum += Context.CycleConnections.Where(cc => cc.metaboliteId == id && (cc.roleId == ProductId || cc.roleId == ReversibleId)).ToList().Sum(cc => cc.stoichiometry);

            AllStoichiometryCache2[id] = new { Consumers = rsum, Producers = psum };
            return AllStoichiometryCache2[id];
        }

        public static Reaction CachedR(Guid id)
        {
            return Context.Reactions.Find(id);
        }

        public static Species CachedS(Guid id)
        {
            return Context.Species.Find(id);
        }

        public static ReactionSpecy CachedRs(Guid rid, Guid sid)
        {
            return Context.ReactionSpecies.Single(rs => rs.reactionId == rid && rs.speciesId == sid);
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
    }
}
