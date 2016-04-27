using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecoli.Util;
using Ecoli.Util.DB;

namespace Ecoli.Util
{
    public class Db
    {
        public static readonly EcoliCoreModel Context = new EcoliCoreModel();

        public const byte ReactantId = 1;
        public const byte ProductId = 2;
        public const byte ReversibleId = 3;

        public const double PseudoReactionStoichiometry = 1.0;

        public static readonly Dictionary<Guid, dynamic> AllStoichiometryCache = new Dictionary<Guid, dynamic>();
        public static readonly Dictionary<Guid, HyperGraph.Node.ReactionCountClass> AllReactionCache = new Dictionary<Guid, HyperGraph.Node.ReactionCountClass>();

        public static dynamic InvolvedReactionStoch(Guid id)
        {
            if (AllStoichiometryCache.ContainsKey(id)) return AllStoichiometryCache[id];

            var rsum = Context.ReactionSpecies.Where(r => r.speciesId == id && r.roleId == ReactantId).ToList().Sum(e => e.stoichiometry);
            var psum = Context.ReactionSpecies.Where(r => r.speciesId == id && r.roleId == ProductId).ToList().Sum(e => e.stoichiometry);

            rsum += Context.CycleConnections.Where(cc => cc.metaboliteId == id && (cc.roleId == ReactantId || cc.roleId == ReversibleId)).ToList().Sum(cc => cc.stoichiometry);
            psum += Context.CycleConnections.Where(cc => cc.metaboliteId == id && (cc.roleId == ProductId || cc.roleId == ReversibleId)).ToList().Sum(cc => cc.stoichiometry);

            AllStoichiometryCache[id] = new { Consumers = rsum, Producers = psum };
            return AllStoichiometryCache[id];
        }

        public static HyperGraph.Node.ReactionCountClass InvolvedReactionCount(Guid id)
        {
            if (AllReactionCache.ContainsKey(id)) return AllReactionCache[id];

            var species = Context.Species.Single(s => s.id == id);
            var products = species.ReactionSpecies.Count(rs => rs.roleId == ProductId);
            var reactants = species.ReactionSpecies.Count(rs => rs.roleId == ReactantId);

            AllReactionCache[id] = new HyperGraph.Node.ReactionCountClass(reactants, products);
            return AllReactionCache[id];
        }

        public static int GetReactionCountSum(Guid id)
        {
            return InvolvedReactionCount(id).Consumers + InvolvedReactionCount(id).Producers;
        }

        public static int TotalReactions(Guid id)
        {
            InvolvedReactionCount(id);
            var sum = AllReactionCache[id].Consumers + AllReactionCache[id].Producers;
            return sum == 0 ? Int32.MaxValue : sum;
        }

        public static List<Guid> ParentCyclesOfReactionOrCycle(Guid reactionId)
        {
            var parentCycles = new List<Guid>();
            var cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == reactionId);
            while (cycle != null)
            {
                parentCycles.Add(cycle.cycleId);
                cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == cycle.cycleId);
            }
            return parentCycles;
        }

        public static Dictionary<Guid, byte> ParentCyclesOfMetabolite(Guid metabolite)
        {
            var parentCycles = new Dictionary<Guid, byte>();
            var connectedCycles = Db.Context.CycleConnections.Where(cc => cc.metaboliteId == metabolite);
            foreach (var cycleConnection in connectedCycles)
            {
                parentCycles[cycleConnection.cycleId] = cycleConnection.roleId;
            }
            
            foreach (var connectedCycle in connectedCycles)
            {
                var cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == connectedCycle.cycleId);
                while (cycle != null)
                {
                    // if a cycle was previously found with a different role .. then the connection should be a reversible connection
                    if (parentCycles.ContainsKey(cycle.cycleId) && parentCycles[cycle.cycleId] != connectedCycle.roleId)
                    {
                        parentCycles[cycle.cycleId] = Db.ReversibleId;
                    }
                    else
                    {
                        parentCycles[cycle.cycleId] = connectedCycle.roleId;
                    }
                    cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == cycle.cycleId);
                }
            }
            return parentCycles;
        } 
    }
}
