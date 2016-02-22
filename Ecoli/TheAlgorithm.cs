namespace Ecoli
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Util;

    using Newtonsoft.Json;
    using DB;

    public class TheAlgorithm
    {
        public readonly Fba3 Fba = new Fba3();
        public readonly LinkedList<string> Pathway = new LinkedList<string>();
        //public readonly HashSet<Guid> BlockedReactions = new HashSet<Guid>();
        private const int CommonMetabolite = 61;
        [JsonProperty("isFeasable")]
        public bool IsFeasable { get; set; }

        [JsonProperty("graph")]
        public readonly HyperGraph Sm = new HyperGraph();

        [JsonProperty("iter")]
        public int Iteration = 1;

        [JsonProperty("Z")]
        public Dictionary<Guid, int> Z = new Dictionary<Guid, int>();

        public int IterationId => Iteration++;


        public void Start()
        {
            // co2_e - no cycle connections
            //var id = Guid.Parse("A8B08CEE-9002-4EB9-8993-9BE19DD1C5E6");

            // pi_c - inside a cycle
            var id = Guid.Parse("2AB52B98-E92B-4807-A660-B3986A08ECC8");

            Z[id] = (id.GetHashCode() % 2) == 0 ? -1 : 1;

            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            //var m = Util.CachedS(Z.Keys.OrderBy(Util.GetReactionCountSum).First()); //e => Z[e] > 0
            var m = Db.Context.Species.Find(id);

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.id, m.sbmlId);

            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            ExtendGraph(m.id, Sm);
        }

        public void Step()
        {
            // steps 4, 5
            ApplyFba(Sm, Z, IterationId);

            //8. Let m’ be a border metabolite in S(m) involved in the smallest total number of reactions.
            var borderm = GetBorderMetabolites(Sm);
            if (borderm.Count == 0)
            {
                Core.SaveAsDgs(Sm.Nodes.First().Value, Sm, Core.Dir);
                Console.WriteLine("NO BORDER METABILTES");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var m2 = borderm.Select(m => m.Id).OrderBy(Db.TotalReactions).First();
            Core.SaveAsDgs(Sm.Nodes[m2], Sm, Core.Dir);
            Sm.NextStep();

            //Extend S(m) with m’ and its reactions from M.
            ExtendGraph(m2, Sm);

            //Remove the exchange reaction that was introduced for m’ in step 4.
            //Add a constraint that total net flux of reactions of m’ should
            //be equal to those of the removed flux exchange reaction.
            RemoveExchangeReaction(Sm, Sm.Nodes[m2]);

            //Go to step 4 to add exchange fluxes for the new border metabolites. 
            //If S(m) cannot be extended, then go to step 3.
        }

        public void RemoveExchangeReaction(HyperGraph sm, HyperGraph.Node m2)
        {
            if (m2.IsExtended)
            {
                if (m2.Producers.Count(e => !e.IsPseudo) != 0)
                {
                    foreach (var reaction in m2.Producers.Where(e => e.IsPseudo).ToList())
                    {
                        sm.RemoveReaction(reaction);
                    }
                }

                if (m2.Consumers.Count(e => !e.IsPseudo) != 0)
                {
                    foreach (var reaction in m2.Consumers.Where(e => e.IsPseudo).ToList())
                    {
                        sm.RemoveReaction(reaction);
                    }
                }
            }
        }

        public void UpdateNeighbore(HyperGraph sm, HyperGraph.Node m2)
        {
            var inex = m2.Consumers.Any(e => e.IsPseudo) ? m2.Consumers.First(s => s.IsPseudo) : null;
            var outex = m2.Producers.Any(e => e.IsPseudo) ? m2.Producers.First(s => s.IsPseudo) : null;

            if (inex != null)
            {
                inex.UpdatePseudo.Clear();
                foreach (var s in m2.Consumers.Where(s => !s.IsPseudo && sm.LastLevel <= s.Level)) //
                    inex.UpdatePseudo.Add(s.Id);
            }

            if (outex != null)
            {
                outex.UpdatePseudo.Clear();
                foreach (var s in m2.Producers.Where(s => !s.IsPseudo && sm.LastLevel <= s.Level)) //
                    outex.UpdatePseudo.Add(s.Id);
            }
        }

        public void ApplyFba(HyperGraph sm, Dictionary<Guid, int> z, int iteration)
        {
            //4. Let a metabolite mb be labeled as a border metabolite 
            // if there is at least one consumer or producer of mb that is not included in the current subnetwork, S(m). 
            var borderm = GetBorderMetabolites(sm);

            // Define exchange reactions for all border metabolites in S(m).
            borderm.ToList().ForEach(mbs => DefinePseudo(mbs, sm));
            sm.Nodes.Values.Where(n => n.IsCommon).ToList().ForEach(mbs => DefinePseudo(mbs, sm));

            var nonborder = GetNonBorderMetabolites(sm);

            var dic = Intersect(z, nonborder);

            //5. Apply Flux Balance Analysis on S(m) with the objective function F defined as follows. 
            //For  each non-border metabolite  m  that  is  included  in  both S(m)and  Z,  perform  the following checks:
            var timer = new Stopwatch();
            timer.Start();
            IsFeasable = Fba.Solve(dic, sm);
            Fba.LastRuntime = timer.ElapsedMilliseconds * 1.0 / 1000.0;
            timer.Stop();

            //return it;
        }

        public ConcurrentDictionary<Guid, int> Intersect(Dictionary<Guid, int> z, HashSet<HyperGraph.Node> nonborder)
        {
            var dic = new ConcurrentDictionary<Guid, int>();
            foreach (var guid in z.Select(s => s.Key).Intersect(nonborder.Select(n => n.Id)))
            {
                dic[guid] = z[guid];
            }
            return dic;
        }

        public void DefinePseudo(HyperGraph.Node node, HyperGraph sm)
        {
            DefineExReaction(node, sm);
        }

        private void DefineExReaction(HyperGraph.Node node, HyperGraph sm)
        {
            if ((node.IsProducedBorder || node.ReactionCount.Producers == 0) && !node.Producers.Any(s => s.IsPseudo))
            {
                var producer = Guid.NewGuid();
                sm.AddProduct(producer, string.Format("exr_{0}_prod", node.Label), node.Id, node.Label, true);
                sm.Edges[producer].Reactions.UnionWith(
                    node.ToSpecies.ReactionSpecies.Where(
                        rs =>
                        rs.speciesId == node.Id && rs.roleId == Db.ProductId && !sm.Edges.ContainsKey(rs.reactionId))
                        .Select(e => e.reactionId));
            }

            if ((node.IsConsumedBorder || node.ReactionCount.Consumers == 0) && !node.Consumers.Any(s => s.IsPseudo))
            {
                var consumer = Guid.NewGuid();
                sm.AddReactant(consumer, string.Format("exr_{0}_cons", node.Label), node.Id, node.Label, true);
                sm.Edges[consumer].Reactions.UnionWith(
                    node.ToSpecies.ReactionSpecies.Where(
                        rs =>
                        rs.speciesId == node.Id && rs.roleId == Db.ReactantId
                        && !sm.Edges.ContainsKey(rs.reactionId)).Select(e => e.id));
            }
        }

        public static void DefineExReactionLonely(HyperGraph.Node m, HyperGraph sm)
        {
            if (m.IsExtended) return;

            if (m.Consumers.Count == 0)
            {
                sm.AddReactant(Guid.NewGuid(), string.Format("exr_{0}_cons", m.Label), m.Id, m.Label, true);
            }

            if (m.Producers.Count == 0)
            {
                sm.AddProduct(Guid.NewGuid(), string.Format("exr_{0}_prod", m.Label), m.Id, m.Label, true);
            }

        }

        public static HashSet<HyperGraph.Node> GetBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsCommon && node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public static HashSet<HyperGraph.Node> GetNonBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsCommon && !node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public void ExtendGraph(Guid mid, HyperGraph sm)
        {
            var metabolite = Db.Context.Species.Find(mid);

            foreach (var r in metabolite.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId).Select(rs => rs.Reaction))
            {
                if (!AddCycleFromReaction(sm, r))
                {
                    sm.AddProduct(r.id, r.sbmlId, metabolite.id, metabolite.sbmlId);
                    AddMetabolites(sm, r);
                }
            }

            foreach (var r in metabolite.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(rs => rs.Reaction))
            {
                if (!AddCycleFromReaction(sm, r))
                {
                    sm.AddReactant(r.id, r.sbmlId, metabolite.id, metabolite.sbmlId);
                    AddMetabolites(sm, r);
                }
            }


            // add exchange reaction to lonely(metabol. with only input or output reactions) metabolites   
            foreach (var lon in sm.Nodes.Values.Where(n => n.IsLonely))
            {
                DefineExReactionLonely(lon, sm);
                RemoveExchangeReaction(sm, lon);
            }

            foreach (var node in sm.Nodes[metabolite.id].AllNeighborNodes()) //.Where(node => !node.IsBorder)
            {
                if (node.IsBorder) UpdateNeighbore(sm, node);
                RemoveExchangeReaction(sm, node);
            }

            sm.Nodes[mid].IsExtended = true;
        }

        private static bool AddCycleFromReaction(HyperGraph hyperGraph, Reaction reaction)
        {
            // check if this reaction is inside a cycle or not
            var cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == reaction.id);
            if (cycle == null) return false;


            // track up to the topmost cycle in heirarchy
            while (Db.Context.CycleReactions.Any(cr => cr.otherId == cycle.cycleId))
            {
                cycle = Db.Context.CycleReactions.First(cr => cr.otherId == cycle.cycleId);
            }
            var cycleReaction = new HyperGraph.Cycle(Db.Context.Cycles.Find(cycle.cycleId));


            var products = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ProductId);
            var reactants = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ReactantId);
            var reversibles = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ReversibleId);

            foreach (var product in products)
            {
                var m = new HyperGraph.Node(product.Species, hyperGraph.Step);
                hyperGraph.AddProduct(cycleReaction, m, 1);
            }

            foreach (var reactant in reactants)
            {
                var m = new HyperGraph.Node(reactant.Species, hyperGraph.Step);
                hyperGraph.AddReactant(cycleReaction, m, 1);
            }

            foreach (var reversible in reversibles)
            {
                var m = new HyperGraph.Node(reversible.Species, hyperGraph.Step);
                hyperGraph.AddProduct(cycleReaction, m, 1);
                hyperGraph.AddReactant(cycleReaction, m, 1);
            }

            return true;
        }

        private static void AddCycleFromMetabolite(HyperGraph hyperGraph, Species sp)
        {
            // get cycles connected to this metabolite
            var cycleConnections = Db.Context.CycleConnections.Where(cc => cc.metaboliteId == sp.id);
            if (!cycleConnections.Any()) return;
            foreach (var cycleConnection in cycleConnections)
            {
                var cycle = Db.Context.Cycles.Find(cycleConnection.cycleId);
                foreach (var connection in cycle.CycleConnections)
                {
                    foreach (var cycleReaction in connection.Cycle.CycleReactions)
                    {
                        if (cycleReaction.isReaction)
                        {
                            var reaction = Db.Context.Reactions.Find(cycleReaction.otherId);
                            hyperGraph.AddReaction(reaction);
                        }
                        else
                        {
                            var innerCycleDb = Db.Context.Cycles.Find(cycleReaction.otherId);
                            var innerCycle = new HyperGraph.Cycle(innerCycleDb);
                            foreach (var innerCycleConnection in innerCycleDb.CycleConnections)
                            {
                                // todo change level value to real step number
                                if (innerCycleConnection.roleId == Db.ProductId)
                                {
                                    var m = HyperGraph.Node.Create(innerCycleConnection.metaboliteId, innerCycleConnection.Species.sbmlId, 0);
                                    hyperGraph.AddProduct(innerCycle, m, 1);
                                }
                                else if (innerCycleConnection.roleId == Db.ReactantId)
                                {
                                    var m = HyperGraph.Node.Create(innerCycleConnection.metaboliteId, innerCycleConnection.Species.sbmlId, 0);
                                    hyperGraph.AddReactant(innerCycle, m, 1);
                                }
                                else
                                {
                                    var m = HyperGraph.Node.Create(innerCycleConnection.metaboliteId, innerCycleConnection.Species.sbmlId, 0);
                                    hyperGraph.AddProduct(innerCycle, m, 1);
                                    hyperGraph.AddReactant(innerCycle, m, 1);
                                }
                            }
                        }

                    }
                }
            }
        }

        private static void AddMetabolites(HyperGraph sm, Reaction reaction)
        {
            try
            {
                var products = Db.Context.ReactionSpecies
                    .Where(rs => rs.roleId == Db.ProductId && rs.reactionId == reaction.id)
                    .Select(rs => rs.Species).ToList();

                var reactants = Db.Context.ReactionSpecies
                    .Where(rs => rs.roleId == Db.ReactantId && rs.reactionId == reaction.id)
                    .Select(rs => rs.Species).ToList();

                foreach (var meta in reactants
                    /*.Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false)*/)
                    sm.AddReactant(reaction.id, reaction.sbmlId, meta.id, meta.sbmlId);

                foreach (var meta in products
                    /*.Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false)*/)
                    sm.AddProduct(reaction.id, reaction.sbmlId, meta.id, meta.sbmlId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void DefineBlockedReactions(HyperGraph graph)
        {
            Console.WriteLine("Running FVA...");
            var fva = new FVA();
            //var smodel = ServerModel.Load(Guid.Parse("74737800-92B5-41E3-9DAA-077CCAE71F0F"));
            //var g = new HyperGraph();
            //foreach (var m in smodel.GetAllSpecies())
            //{
            //    g.AddNode(m.ID, m.SbmlId);
            //    foreach (var prod in m.getAllReactions(Util.Product))
            //        g.AddProduct(prod.ID, prod.SbmlId, m.ID, m.SbmlId);

            //    foreach (var react in m.getAllReactions(Util.Reactant))
            //        g.AddReactant(react.ID, react.SbmlId, m.ID, m.SbmlId);

            //    if (g.Nodes[m.ID].Producers.Count == 0)
            //        g.AddProduct(Guid.NewGuid(), string.Format("exr{0}_prod", m.SbmlId), m.ID, m.SbmlId, true);
            //    else if (g.Nodes[m.ID].Consumers.Count == 0)
            //        g.AddReactant(Guid.NewGuid(), string.Format("exr{0}_cons", m.SbmlId), m.ID, m.SbmlId, true);
            //}
            //Util.SaveAsDgs(g.Nodes.Values.First(),g,"B:\\model2\\");
            fva.Solve(graph);
            foreach (var minmax in fva.Results.Where(minmax => Math.Abs(minmax.Value.Item1 - minmax.Value.Item2) < 0.0001))//&& Math.Abs(minmax.Value.Item2) < double.Epsilon
            {
                //File.AppendAllText(Util.BlockedReactionsFile, string.Format("{0};{1}\n", graph.Edges[minmax.Key].Label, minmax.Value.Item1));
            }
            Console.WriteLine("Running FVA Done!");
        }
    }
}