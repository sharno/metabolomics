using System;
using System.Collections.Generic;
using System.Linq;
using Metabol.DbModels.DB;
using Metabol.DbModels.DB2;
using Metabol.DbModels.Models;

namespace Metabol.DbModels
{
    public partial class Db
    {
        internal static readonly MetabolicNetworkContext Context1 = new MetabolicNetworkContext();
        public static readonly EcoliCoreModel Context = new EcoliCoreModel();
        public static readonly ApplicationDbContext ApiDbContext = new ApplicationDbContext();

        public static readonly Dictionary<Guid, HyperGraph.Node.ReactionCountClass> AllReactionCache = new Dictionary<Guid, HyperGraph.Node.ReactionCountClass>();
        private static readonly Dictionary<Guid, dynamic> AllStoichiometryCache2 = new Dictionary<Guid, dynamic>();

        public const double PseudoReactionStoichiometry = 1.0;

        public const byte ReactantId = 1;
        public const byte ProductId = 2;
        public const byte ReversibleId = 3;
        //public const byte ModifierId = 3;

        internal static HyperGraph LoadGraph()
        {
            var graph = new HyperGraph();

            foreach (var reaction in Context.Reactions.ToList())
            {
                //foreach (var reactant in reaction.ReactionSpecies.Where(e => e.roleId == ReactantId && e.reactionId == reaction.id))
                //    graph.AddReactant(reaction.id, reaction.sbmlId, reactant.Species.id, reactant.Species.sbmlId);

                //foreach (var product in reaction.ReactionSpecies.Where(e => e.roleId == ProductId && e.reactionId == reaction.id))
                //    graph.AddProduct(reaction.id, reaction.sbmlId, product.Species.id, product.Species.sbmlId);
                Console.WriteLine("loaded meta:{0} react:{1}", graph.Nodes.Count, graph.Edges.Count);
            }

            return graph;
        }

        internal static void InitCache()
        {
            Console.WriteLine("Cache building start");
            Context.Reactions.ToList();
            Context.Species.ToList();
            Context.ReactionBounds.ToList();
            Context.ReactionSpecies.ToList();
            Console.WriteLine("Cache building done");
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
