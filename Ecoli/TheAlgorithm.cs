using Metabol.DbModels.DB;
using Metabol.DbModels.Models;

namespace Ecoli
{
    using Metabol.DbModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class TheAlgorithm
    {
        // co2_e - no cycle connections
        //Guid.Parse("E0EAF9D2-B3E8-4798-A723-56F75D301702");

        // pep_c
        //Guid.Parse("CB4656B3-F54A-483F-B703-43B8F812740E");

        // q8c
        //  5218A267-A0C3-410D-82AD-21B1C488E0E4

        // start from a reaction instaed of metabolite
        public static readonly Guid StartingMetabolite = Guid.Parse("5218A267-A0C3-410D-82AD-21B1C488E0E4");
        //public static Guid NonZeroReaction = Guid.Empty;

        public readonly LinkedList<string> Pathway = new LinkedList<string>();

        public bool IsFeasable { get; set; }
        public readonly HyperGraph Sm = new HyperGraph();

        public Dictionary<Guid, int> Z = new Dictionary<Guid, int>();
        public int Iteration = 1;
        public int IterationId => Iteration++;

        public void Start(ConcentrationChange[] z)
        {
            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            //var m = Util.CachedS(Z.Keys.OrderBy(Util.GetReactionCountSum).First()); //e => Z[e] > 0
            //TODO use Z
            var m = Db.Context.Species.Find(StartingMetabolite);

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.id, m.sbmlId);

            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            ExtendGraph(m.id, Sm);
        }

        public void Start()
        {
            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            //var m = Util.CachedS(Z.Keys.OrderBy(Util.GetReactionCountSum).First()); //e => Z[e] > 0
            var m = Db.Context.Species.Find(StartingMetabolite);

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.id, m.sbmlId);

            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            ExtendGraph(m.id, Sm);

            //foreach (var edge in Sm.Edges)
            //{
            //    if (edge.Value.Label.IndexOf("ex", StringComparison.OrdinalIgnoreCase) < 0)
            //    {
            //        NonZeroReaction = edge.Key;
            //        break;
            //    }
            //}
        }

        public IterationModels Step()
        {
            // steps  5
            var timer = new Stopwatch();
            timer.Start();
            ApplyFba(Sm);
            timer.Stop();

            var it = new IterationModels(IterationId)
            {
                Fba = IsFeasable ? 1 : 0,
                Time = timer.ElapsedMilliseconds * 1.0 / 1000.0,
                Fluxes = Sm.Edges.Values.ToDictionary(r => r.Label, r => r.Flux),
                Constraints = Fba3.ConstraintList,
                Nodes = Sm.JsonNodes(Z),
                Links = Sm.JsonLinks()
                // MetabolicNetwork = sm
            };

            //4. Let a metabolite mb be labeled as a border metabolite 
            // if there is at least one consumer or producer of mb that is not included in the current subnetwork, S(m). 
            var borderm = GetBorderMetabolites(Sm);

            if (borderm.Count == 0)
            {
                #region algo

                Core.SaveAsDgs(Sm.Nodes.First().Value, Sm, Core.Dir);
                Console.WriteLine("NO BORDER METABILTES");

                Console.WriteLine("Expanding the cycles next");
                var cycle = Sm.Cycles.FirstOrDefault();
                while (cycle.Value != null)
                {
                    Sm.NextStep();
                    Console.WriteLine("ITERATION: " + Sm.Step);

                    var cycleDb = Db.Context.Cycles.Find(cycle.Key);
                    foreach (var cycleReaction in cycleDb.CycleReactions)
                    {
                        if (cycleReaction.isReaction)
                        {
                            var reaction = Db.Context.Reactions.Find(cycleReaction.otherId);
                            foreach (var reactionSpecy in reaction.ReactionSpecies)
                            {
                                if (reactionSpecy.roleId == Db.ProductId)
                                {
                                    Sm.AddProduct(reactionSpecy.reactionId, reactionSpecy.Reaction.sbmlId,
                                        reactionSpecy.Reaction.reversible, false, reactionSpecy.speciesId,
                                        reactionSpecy.Species.sbmlId);
                                }
                                else if (reactionSpecy.roleId == Db.ReactantId)
                                {
                                    Sm.AddReactant(reactionSpecy.reactionId, reactionSpecy.Reaction.sbmlId,
                                        reactionSpecy.Reaction.reversible, false, reactionSpecy.speciesId,
                                        reactionSpecy.Species.sbmlId);
                                }
                            }
                        }
                        else
                        {
                            var innerCycle = Db.Context.Cycles.Find(cycleReaction.otherId);
                            foreach (var cycleConnection in innerCycle.CycleConnections)
                            {
                                switch (cycleConnection.roleId)
                                {
                                    case Db.ProductId:
                                        Sm.AddProduct(new HyperGraph.Cycle(innerCycle),
                                            new HyperGraph.Node(cycleConnection.Species, Sm.LastLevel));
                                        break;
                                    case Db.ReactantId:
                                        Sm.AddReactant(new HyperGraph.Cycle(innerCycle),
                                            new HyperGraph.Node(cycleConnection.Species, Sm.LastLevel));
                                        break;
                                    case Db.ReversibleId:
                                        Sm.AddProduct(new HyperGraph.Cycle(innerCycle),
                                            new HyperGraph.Node(cycleConnection.Species, Sm.LastLevel));
                                        Sm.AddReactant(new HyperGraph.Cycle(innerCycle),
                                            new HyperGraph.Node(cycleConnection.Species, Sm.LastLevel));
                                        break;
                                }
                            }
                        }
                    }
                    Sm.RemoveCycle(cycle.Value);

                    // new border metabolites that are a part of an outer cycle that need to be connected as interface metabolites of inner cycles if that outer cycle was deleted
                    var borderMetabolites = GetBorderMetabolites(Sm);
                    foreach (var borderMetabolite in borderMetabolites)
                    {
                        //var parentCycles = Db.ParentCyclesOfMetabolite(borderMetabolite.Id);
                        //foreach (var parentCycle in parentCycles.Where(parentCycle => Sm.Cycles.ContainsKey(parentCycle.Key)))
                        //{
                        //    switch (parentCycle.Value)
                        //    {
                        //        case Db.ProductId:
                        //            Sm.AddProduct(Sm.Cycles[parentCycle.Key], Sm.Nodes[borderMetabolite.Id]);
                        //            break;
                        //        case Db.ReactantId:
                        //            Sm.AddReactant(Sm.Cycles[parentCycle.Key], Sm.Nodes[borderMetabolite.Id]);
                        //            break;
                        //        case Db.ReversibleId:
                        //            Sm.AddProduct(Sm.Cycles[parentCycle.Key], Sm.Nodes[borderMetabolite.Id]);
                        //            Sm.AddReactant(Sm.Cycles[parentCycle.Key], Sm.Nodes[borderMetabolite.Id]);
                        //            break;
                        //    }
                        //}
                        var connectedCycles = Db.Context.CycleConnections.Where(cc => cc.metaboliteId == borderMetabolite.Id);
                        foreach (var cc in connectedCycles)
                        {
                            if (!Sm.Cycles.ContainsKey(cc.cycleId))
                        {
                                continue;
                            }
                            switch (cc.roleId)
                            {
                                case Db.ProductId:
                                    Sm.AddProduct(Sm.Cycles[cc.cycleId], Sm.Nodes[borderMetabolite.Id]);
                                    break;
                                case Db.ReactantId:
                                    Sm.AddReactant(Sm.Cycles[cc.cycleId], Sm.Nodes[borderMetabolite.Id]);
                                    break;
                                case Db.ReversibleId:
                                    Sm.AddProduct(Sm.Cycles[cc.cycleId], Sm.Nodes[borderMetabolite.Id]);
                                    Sm.AddReactant(Sm.Cycles[cc.cycleId], Sm.Nodes[borderMetabolite.Id]);
                                    break;
                            }
                        }
                    }

                    // new border metabolites were introduced to the graph
                    // this includes metabolites with no producers or consumers
                    borderMetabolites = GetBorderMetabolites(Sm);

                    // there shouldn't be border metabolites in the steps of expanding cycles
                    foreach (var borderMetabolite in borderMetabolites)
                    {
                        DefineExReaction(borderMetabolite, Sm);
                        RemoveExchangeReaction(Sm, borderMetabolite);
                    }


                    
                    if (!ApplyFba(Sm))
                    {
                        Core.SaveAsDgs(Sm.Nodes.First().Value, Sm, Core.Dir);
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    Core.SaveAsDgs(Sm.Nodes.First().Value, Sm, Core.Dir);

                    cycle = Sm.Cycles.FirstOrDefault();
                }

                Console.ReadKey();
                //Environment.Exit(0);

                #endregion
            }

            //8. Let m’ be a border metabolite in S(m) involved in the smallest total number of reactions.
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
            return it;
        }

        public void RemoveExchangeReaction(HyperGraph sm, HyperGraph.Node m)
        {
            var flux = 0.0;
            var reactions = new List<Guid>();
            var coefficients = new List<double>();
            if (m.Consumers.Any(c => c.IsPseudo) && !m.IsConsumedBorder)
            {
                var pseudoEx = m.Consumers.Single(c => c.IsPseudo);
                var realConsEx = m.Consumers.Where(c => c.RecentlyAdded && !c.IsReversible).ToList();
                var realProdRevEx = m.Producers.Where(p => p.RecentlyAdded && p.IsReversible).ToList();

                flux -= pseudoEx.Flux;
                realConsEx.ForEach(c =>
                {
                    reactions.Add(c.Id);
                    coefficients.Add(-1);
                });
                realProdRevEx.ForEach(p =>
                {
                    reactions.Add(p.Id);
                    coefficients.Add(1);
                });

                sm.RemoveReaction(m.Consumers.Single(e => e.IsPseudo));
            }

            if (m.Producers.Any(p => p.IsPseudo) && !m.IsProducedBorder)
            {
                var pseudoEx = m.Producers.Single(p => p.IsPseudo);
                var realProdEx = m.Producers.Where(p => p.RecentlyAdded && !p.IsReversible).ToList();
                var realConsRevEx = m.Consumers.Where(c => c.RecentlyAdded && c.IsReversible).ToList();

                flux += pseudoEx.Flux;
                realProdEx.ForEach(p =>
                {
                    reactions.Add(p.Id);
                    coefficients.Add(1);
                });
                realConsRevEx.ForEach(c =>
                {
                    reactions.Add(c.Id);
                    coefficients.Add(-1);
                });

                sm.RemoveReaction(m.Producers.Single(e => e.IsPseudo));
            }

            if (reactions.Any()) sm.ExchangeConstraints.Add(Tuple.Create(reactions, coefficients, flux));
        }

        public bool ApplyFba(HyperGraph sm)
        {
            //5. Apply Flux Balance Analysis on S(m) with the objective function F defined as follows. 
            //For  each non-border metabolite  m  that  is  included  in  both S(m)and  Z,  perform  the following checks:
            IsFeasable = Fba3.Solve(sm);

            //Stats[sm.Step] = Fba3.Stat;
            return IsFeasable;
        }

        private static void DefineExReaction(HyperGraph.Node node, HyperGraph sm)
        {
            if (node.IsProducedBorder && !node.Producers.Any(s => s.IsPseudo))
            {
                sm.AddProduct(Guid.NewGuid(), $"_exr_{node.Label}_prod", false, true, node.Id, node.Label);
            }

            if (node.IsConsumedBorder && !node.Consumers.Any(s => s.IsPseudo))
            {
                sm.AddReactant(Guid.NewGuid(), $"_exr_{node.Label}_cons", false, true, node.Id, node.Label);
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
                var cycle = AddCycleFromReaction(sm, r);
                if (cycle == null)
                {
                    sm.AddProduct(r.id, r.sbmlId, r.reversible, false, metabolite.id, metabolite.sbmlId);
                    AddMetabolites(sm, r);
                }
                else
                {
                    sm.AddProduct(cycle, new HyperGraph.Node(metabolite.id, metabolite.sbmlId, sm.LastLevel));
                }
            }

            foreach (var r in metabolite.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(rs => rs.Reaction))
            {
                var cycle = AddCycleFromReaction(sm, r);
                if (cycle == null)
                {
                    sm.AddReactant(r.id, r.sbmlId, r.reversible, false, metabolite.id, metabolite.sbmlId);
                    AddMetabolites(sm, r);
                }
                else
                {
                    sm.AddReactant(cycle, new HyperGraph.Node(metabolite.id, metabolite.sbmlId, sm.LastLevel));
                }
            }

            // fix direction of reactions with changed boundaries
            foreach (var edge in sm.Edges.Values.Where(edge => edge.LowerBound < 0 && Math.Abs(edge.UpperBound) < double.Epsilon))
            {
                foreach (var product in edge.Products.Values)
                {
                    product.Producers.Remove(edge);
                    product.Consumers.Add(edge);
                    product.Weights[edge.Id] = -product.Weights[edge.Id];
                }
                foreach (var reactant in edge.Reactants.Values)
                {
                    reactant.Consumers.Remove(edge);
                    reactant.Producers.Add(edge);
                    reactant.Weights[edge.Id] = -reactant.Weights[edge.Id];
                }

                var temp = edge.Reactants;
                edge.Reactants = edge.Products;
                edge.Products = temp;

                edge.UpperBound = -edge.LowerBound;
                edge.LowerBound = 0;

                edge.IsReversible = false;
            }


            // Define exchange reactions for all border metabolites in S(m).
            // add exchange reaction to lonely(metabol. with only input or output reactions) metabolites   
            foreach (var m in sm.Nodes.Values)
            {
                DefineExReaction(m, sm);
                RemoveExchangeReaction(sm, m);
            }

            foreach (var node in sm.Nodes[metabolite.id].AllNeighborNodes())
            {
                RemoveExchangeReaction(sm, node);
            }

            sm.Nodes[mid].IsExtended = true;
        }

        private static HyperGraph.Cycle AddCycleFromReaction(HyperGraph hyperGraph, Reaction reaction)
        {
            // check if this reaction is inside a cycle or not
            var cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == reaction.id);
            if (cycle == null) return null;


            // track up to the topmost cycle in heirarchy
            while (Db.Context.CycleReactions.Any(cr => cr.otherId == cycle.cycleId))
            {
                cycle = Db.Context.CycleReactions.First(cr => cr.otherId == cycle.cycleId);
            }
            var cycleReaction = new HyperGraph.Cycle(Db.Context.Cycles.Find(cycle.cycleId));
            
            // if this cycles is already in the graph return it
            if (hyperGraph.Cycles.ContainsKey(cycleReaction.Id))
            {
                return hyperGraph.Cycles[cycleReaction.Id];
            }

            // if not add all its connections
            var products = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ProductId);
            var reactants = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ReactantId);
            var reversibles = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ReversibleId);

            foreach (var product in products)
            {
                var m = new HyperGraph.Node(product.Species, hyperGraph.Step);
                hyperGraph.AddProduct(cycleReaction, m);
            }

            foreach (var reactant in reactants)
            {
                var m = new HyperGraph.Node(reactant.Species, hyperGraph.Step);
                hyperGraph.AddReactant(cycleReaction, m);
            }

            foreach (var reversible in reversibles)
            {
                var m = new HyperGraph.Node(reversible.Species, hyperGraph.Step);
                hyperGraph.AddProduct(cycleReaction, m);
                hyperGraph.AddReactant(cycleReaction, m);
            }

            return cycleReaction;
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
                    sm.AddReactant(reaction.id, reaction.sbmlId, reaction.reversible, false, meta.id, meta.sbmlId);

                foreach (var meta in products
                    /*.Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false)*/)
                    sm.AddProduct(reaction.id, reaction.sbmlId, reaction.reversible, false, meta.id, meta.sbmlId);
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