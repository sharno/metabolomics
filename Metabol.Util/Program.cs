using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Threading;
using Metabol.Util.SimpleCycle;


namespace Metabol.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Metabol.Util.DB2;

    class Program
    {
        public static Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> ConvertFromHypergraph(HyperGraph hyperGraph)
        {
            var graph = new Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>>();

            foreach (var edge in hyperGraph.Edges)
            {
                // Item1 is coming in edges, Item2 is going out edges
                graph[edge.Key] = Tuple.Create(new HashSet<Guid>(), new HashSet<Guid>(), true);


                // 1. recording going out edges
                foreach (var product in edge.Value.Products)
                {
                    foreach (var consumer in product.Value.Consumers)
                    {
                        graph[edge.Key].Item2.Add(consumer.Id);
                    }
                    // if one of the producers of the product is a reversible reaction
                    foreach (var producer in product.Value.Producers)
                    {
                        if (producer.ToServerReaction.reversible)
                        {
                            graph[edge.Key].Item2.Add(producer.Id);
                        }
                    }
                }
                // if it's a reversible reaction
                if (edge.Value.ToServerReaction.reversible)
                {
                    foreach (var reactant in edge.Value.Reactants)
                    {
                        foreach (var consumer in reactant.Value.Consumers)
                        {
                            graph[edge.Key].Item2.Add(consumer.Id);
                        }
                    }
                }


                // 2. recording coming in edges
                foreach (var reactant in edge.Value.Reactants)
                {
                    foreach (var producer in reactant.Value.Producers)
                    {
                        graph[edge.Key].Item1.Add(producer.Id);
                    }
                    // if one of the consumers of the reactant is a reversible reaction
                    foreach (var consumer in reactant.Value.Consumers)
                    {
                        if (consumer.ToServerReaction.reversible)
                        {
                            graph[edge.Key].Item1.Add(consumer.Id);
                        }
                    }
                }
                // if it's a reversible reaction
                if (edge.Value.ToServerReaction.reversible)
                {
                    foreach (var product in edge.Value.Products)
                    {
                        foreach (var producer in product.Value.Producers)
                        {
                            graph[edge.Key].Item1.Add(producer.Id);
                        }
                    }
                }

                // no self loops
                graph[edge.Key].Item1.Remove(edge.Key);
                graph[edge.Key].Item2.Remove(edge.Key);
            }

            foreach (var key in graph.Keys)
            {
                Console.WriteLine(hyperGraph.Edges[key].Label);
                Console.WriteLine(graph[key].Item2.Count);
            }

            return graph;
        }

        public static Guid CollapseCycle(Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph, List<Guid> cycle, HyperGraph hypergraph)
        {
            Guid cycleId = Guid.NewGuid();
            graph[cycleId] = Tuple.Create(new HashSet<Guid>(), new HashSet<Guid>(), false);
            foreach (var v in cycle)
            {
                foreach (var comingIn in graph[v].Item1)
                {
                    graph[comingIn].Item2.Remove(v);
                    graph[comingIn].Item2.Add(cycleId);
                }

                foreach (var goingOut in graph[v].Item2)
                {
                    graph[goingOut].Item1.Remove(v);
                    graph[goingOut].Item1.Add(cycleId);
                }

                graph[cycleId].Item1.UnionWith(graph[v].Item1.Except(cycle));
                graph[cycleId].Item2.UnionWith(graph[v].Item2.Except(cycle));

                // no self loops
                graph[cycleId].Item1.Remove(cycleId);
                graph[cycleId].Item2.Remove(cycleId);

                graph.Remove(v);
            }

            // modify hypergraph
            foreach (var v in cycle)
            {
//                reactants.UnionWith(
//                    hypergraph.Edges[v].Reactants.Where(
//                        m => m.Value.Producers.Select(p => p.Id).Except(cycle).Count() != 0).Select(m => m.Key));
//                reactants.UnionWith(
//                    hypergraph.Edges[v].Reactants.Where(
//                        m => m.Value.Consumers.Select(c => c.Id).Except(cycle).Count() != 0).Select(m => m.Key));
//
//                products.UnionWith(
//                    hypergraph.Edges[v].Products.Where(
//                        m => m.Value.Consumers.Select(c => c.Id).Except(cycle).Count() != 0).Select(m => m.Key));
//                products.UnionWith(
//                    hypergraph.Edges[v].Products.Where(
//                        m => m.Value.Producers.Select(p => p.Id).Except(cycle).Count() != 0).Select(m => m.Key));

                foreach (var reactant in hypergraph.Edges[v].Reactants)
                {
                    if (reactant.Value.Producers.Select(r => r.Id).Except(cycle).Count() != 0 || reactant.Value.Consumers.Select(r => r.Id).Except(cycle).Count() != 0)
                    {
                        reactant.Value.Weights[cycleId] = reactant.Value.Weights[v];
                        hypergraph.AddReactant(cycleId, cycleId.ToString(), reactant.Key, reactant.Value.Label, true, true);

                        // TODO if reaction is reversible
                        if (hypergraph.Edges[v].IsReversible)
                        {
                            hypergraph.AddProduct(cycleId, cycleId.ToString(), reactant.Key, reactant.Value.Label, true, true);
                        }
                    }
                }
                foreach (var product in hypergraph.Edges[v].Products)
                {
                    if (product.Value.Consumers.Select(r => r.Id).Except(cycle).Count() != 0 || product.Value.Producers.Select(r => r.Id).Except(cycle).Count() != 0)
                    {
                        product.Value.Weights[cycleId] = product.Value.Weights[v];
                        hypergraph.AddProduct(cycleId, cycleId.ToString(), product.Key, product.Value.Label, true, true);

                        // TODO if reaction is reversible
                        if (hypergraph.Edges[v].IsReversible)
                        {
                            hypergraph.AddReactant(cycleId, cycleId.ToString(), product.Key, product.Value.Label, true, true);
                        }
                    }
                }

                // TODO remove this line if all graph is connected
                hypergraph.Edges.GetOrAdd(cycleId, HyperGraph.Edge.Create(cycleId, 1));
            }
            
            return cycleId;
        }

//        public static Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> ConvertFromHypergraphTest(HyperGraph hyperGraph)
//        {
//            Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph = new Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>>();
//
//            foreach (var edge in hyperGraph.Edges)
//            {
//                graph[edge.Key] = Tuple.Create(new HashSet<Guid>(), new HashSet<Guid>(), true);
//
//                foreach (var product in edge.Value.Products)
//                {
//                    foreach (var consumer in product.Value.Consumers)
//                    {
//                        graph[edge.Key].Item2.Add(consumer.Id);
//                    }
//                }
//            }
//
//            return graph;
//        }

        public static void recordToDatabase(Guid cycleId, List<Guid> cycle, Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph, HyperGraph hypergraph)
        {
            var cycleModel = new DB2.Cycle();
            cycleModel.id = cycleId;


            foreach (var reaction in cycle)
            {
//                bool _isReaction = (Db.Context.Reactions.Find(reaction) != null);
                cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycleModel.id, otherId = reaction, isReaction = !hypergraph.Edges[reaction].IsCycle });
            }

            // recording metabolites
            var reversibleMetabolites = hypergraph.Edges[cycleId].Reactants.Intersect(hypergraph.Edges[cycleId].Products).ToDictionary(e => e.Key, e => e.Value);
            hypergraph.Edges[cycleId].Reactants = hypergraph.Edges[cycleId].Reactants.Except(reversibleMetabolites).ToDictionary(e=>e.Key, e=>e.Value);
            hypergraph.Edges[cycleId].Products = hypergraph.Edges[cycleId].Products.Except(reversibleMetabolites).ToDictionary(e => e.Key, e => e.Value);

            foreach (var reversibleMetabolite in reversibleMetabolites)
            {
                Db.Context.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reversibleMetabolite.Key, roleId = Db.ReactantId, stoichiometry = reversibleMetabolite.Value.Weights[cycleId], isReversible = true});
            }
            foreach (var reactant in hypergraph.Edges[cycleId].Reactants)
            {
                Db.Context.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reactant.Key, roleId = Db.ReactantId, stoichiometry = reactant.Value.Weights[cycleId], isReversible = false});
            }
            foreach (var product in hypergraph.Edges[cycleId].Products)
            {
                Db.Context.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = product.Key, roleId = Db.ProductId, stoichiometry = product.Value.Weights[cycleId], isReversible = false});
            }


            Db.Context.Cycles.Add(cycleModel);
            Db.Context.SaveChanges();
        }

        static void Main()
        {
            {
                const int Outliear = 61;
                var g = new HyperGraph();
                var count = 0;
                //.Where(p => Db.GetReactionCountSum(p.id) < Outliear)
                foreach (var sp in Db.Context.Species)
                {
                    count++;
//                    if (count == 500)
//                        break;

                    Console.WriteLine("adding metabolite " + count);

                    foreach (var pr in sp.ReactionSpecies
                        .Where(
                            rs =>
                                rs.speciesId == sp.id && rs.roleId == Db.ProductId &&
                                rs.Reaction.sbmlId != "R_biomass_reaction"))
                    {
                        g.AddProduct(pr.reactionId, pr.Reaction.sbmlId, sp.id, sp.sbmlId);
                        g.Edges[pr.reactionId].IsReversible = pr.Reaction.reversible;
                    }
                    foreach (var pr in sp.ReactionSpecies
                        .Where(
                            rs =>
                                rs.speciesId == sp.id && rs.roleId == Db.ReactantId &&
                                rs.Reaction.sbmlId != "R_biomass_reaction"))
                    {
                        g.AddReactant(pr.reactionId, pr.Reaction.sbmlId, sp.id, sp.sbmlId);
                        g.Edges[pr.reactionId].IsReversible = pr.Reaction.reversible;
                    }
                }

                Console.WriteLine("loaded the whole network");

                // var reactions = "";
                //g.Edges.Values.Select(TheAlgorithm.ToReaction).ToDictionary(e => e.Id);
                //var fba = new Fba();
                //fba.Solve(reactions, Z, g);
                //Console.ReadKey();

                {
                    // delete all entries from DB
                    Db.Context.CycleReactions.RemoveRange(Db.Context.CycleReactions.Where(e => true));
                    Db.Context.CycleConnections.RemoveRange(Db.Context.CycleConnections.Where(e => true));
                    Db.Context.Cycles.RemoveRange(Db.Context.Cycles.Where(e => true));
                    Db.Context.SaveChanges();
                    Console.WriteLine("deleted DB entries");
                }


                Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph = ConvertFromHypergraph(g);
                DFS.DetectAndCollapseCycles(graph, g);


                //                List<List<Guid>> cycles = SimpleCycle.CyclesFinder.Find(ConvertFromHypergraph(g));
                //                Console.WriteLine("Number of cycles: " + cycles.Count);

                //using (var context = new CycleReactionModel())
                {
                    // delete all entries from DB
                    //                    DbCache.MnContext.CycleReactions.RemoveRange(context.CycleReactions.Where(e => true));
                    //                    DbCache.MnContext.Cycles.RemoveRange(context.Cycles.Where(e => true));
                    //                    DbCache.MnContext.SaveChanges();
                    //
                    //
                    //                    foreach (var cycle in cycles)
                    //                    {
                    //                        var cycleModel = new DB2.Cycle();
                    //                        cycleModel.id = Guid.NewGuid();
                    //
                    //                        foreach (var reaction in cycle)
                    //                        {
                    //                            cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycleModel.id, reactionId = reaction.Id, isExchange = false });
                    //                        }
                    //                        DbCache.MnContext.Cycles.Add(cycleModel);
                    //                    }
                    //                    DbCache.MnContext.SaveChanges();
                }

                Console.WriteLine("finished saving to DB");
                Console.ReadKey();
            }
        }

        static void Main2()
        {
            var graf = new HyperGraph();

            //var rs = new[]
            //{
            //    ServerReaction.Load(Guid.Parse("D4F08E74-2FED-4125-871B-26C5654153B6")),
            //    ServerReaction.Load(Guid.Parse("ABE9C0CA-CFE2-4981-B862-69765EA1AEAF")),
            //    ServerReaction.Load(Guid.Parse("1D947A60-F035-443D-86AF-7702E9B5AC98")),
            //    ServerReaction.Load(Guid.Parse("CAB95D5E-F91D-4806-A6FE-A55B3C5DAFAC")),
            //    ServerReaction.Load(Guid.Parse("4377C959-2090-4AA8-AED5-C6D777A632C7")),
            //    ServerReaction.Load(Guid.Parse("9602D7E7-A48B-41F0-B0BC-470D93F7F517")),
            //    ServerReaction.Load(Guid.Parse("5D4604A6-7AAA-4A26-9582-90C7B668570F")),
            //    ServerReaction.Load(Guid.Parse("85E356DD-03F6-4774-8962-494E536CCA4A")),
            //    ServerReaction.Load(Guid.Parse("BE32A9D4-4E83-4820-97FF-67163DF6BA04")),
            //    ServerReaction.Load(Guid.Parse("CE2296A9-3E7D-4A16-B380-33A211CFC1B3")),
            //    ServerReaction.Load(Guid.Parse("EEFDAEF0-455F-49EC-915B-1B483DF23D39")),
            //    ServerReaction.Load(Guid.Parse("6D7FF530-F6BF-45C2-90FC-9409E68C3590"))
            //};

            //foreach (var reaction in rs)
            //{
            //    foreach (var m in reaction.GetAllProducts())
            //    {
            //        graf.AddProduct(reaction.ID, reaction.SbmlId, m.ID, m.SbmlId);
            //    }
            //    foreach (var m in reaction.GetAllReactants())
            //    {
            //        graf.AddReactant(reaction.ID, reaction.SbmlId, m.ID, m.SbmlId);
            //    }
            //}
            //Util.SaveAsDgs(graf.Nodes.First().Value, graf, "C:\\Users\\sharno\\Desktop\\");
        }

        static void Main3()
        {
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var c = Guid.NewGuid();
            var d = Guid.NewGuid();
            var e = Guid.NewGuid();

            var v1 = Guid.NewGuid();
            var v2 = Guid.NewGuid();
            var v3 = Guid.NewGuid();
            var v4 = Guid.NewGuid();
            var v5 = Guid.NewGuid();

            var graph = new HyperGraph();
            graph.AddNode(a, "A");
            graph.AddNode(b, "B");
            graph.AddNode(c, "C");
            graph.AddNode(d, "D");

            graph.AddReactant(v1, "v1", a, "A");
            graph.AddProduct(v1, "v1", b, "B");

            graph.AddReactant(v1, "v1", a, "A");
            graph.AddProduct(v1, "v1", c, "C");

            graph.AddReactant(v2, "v2", b, "B");
            graph.AddProduct(v2, "v2", a, "A");

            graph.AddReactant(v2, "v2", c, "C");
            graph.AddProduct(v2, "v2", a, "A");

            //            graph.AddReactant(v4, "v4", d, "D");
            //            graph.AddProduct(v4, "v4", a, "A");
            //
            //            graph.AddReactant(v5, "v5", a, "A");
            //            graph.AddProduct(v5, "v5", c, "C");


            //            List<List<Guid>> l = SimpleCycle.CyclesFinder.Find(ConvertFromHypergraphTest(graph));


            //            foreach (var cycle in l)
            //            {
            //                Console.WriteLine("cycle:");
            //                foreach (var edge in cycle.ToArray())
            //                {
            //                    Console.WriteLine(graph.Edges[edge].Label);
            //                }
            //            }
            //            Console.ReadKey();
        }


        static void MainOfStronglyConnectedComponents(string[] args)
        {
            const int Outliear = 61;
            //var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            //DBWrapper.Instance = new DBWrapper(strCon);

            var g = new HyperGraph();

            var count = 0;
            //foreach (var sp in ServerSpecies.AllSpecies().Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
            //{
            //    count++;
            //    //                if (count == 300)
            //    //                {
            //    //                    break;
            //    //                }
            //    Console.WriteLine("adding metabolite " + count);


            //    foreach (var pr in sp.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
            //        g.AddProduct(pr.ID, pr.SbmlId, sp.ID, sp.SbmlId);

            //    foreach (var re in sp.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
            //        g.AddReactant(re.ID, re.SbmlId, sp.ID, sp.SbmlId);
            //}
            foreach (var sp in Db.Context.Species.Where(p => Db.GetReactionCountSum(p.id) < Outliear))
            {
                count++;
                //                if (count == 300)
                //                {
                //                    break;
                //                }
                Console.WriteLine("adding metabolite " + count);

                foreach (var pr in sp.ReactionSpecies
                    .Where(rs => rs.speciesId == sp.id && rs.roleId == Db.ProductId && rs.Reaction.sbmlId != "R_biomass_reaction"))
                    g.AddProduct(pr.reactionId, pr.Reaction.sbmlId, sp.id, sp.sbmlId);
                foreach (var pr in sp.ReactionSpecies
                    .Where(rs => rs.speciesId == sp.id && rs.roleId == Db.ReactantId && rs.Reaction.sbmlId != "R_biomass_reaction"))
                    g.AddReactant(pr.reactionId, pr.Reaction.sbmlId, sp.id, sp.sbmlId);
            }

            Console.WriteLine("loaded the whole network");

            // var reactions = "";
            //g.Edges.Values.Select(TheAlgorithm.ToReaction).ToDictionary(e => e.Id);
            //var fba = new Fba();
            //fba.Solve(reactions, Z, g);
            //Console.ReadKey();

            Dictionary<Guid, Cycle> cycles = CyclesFinder.Run(g);

            // removing exchange reactions from non exchange in cycle
            foreach (var cycle in cycles)
            {
                List<KeyValuePair<Guid, HyperGraph.Edge>> toRemove = cycle.Value.inCycleReactions.Where(e => cycle.Value.graph.Edges.ContainsKey(e.Key)).ToList();
                toRemove.AddRange(cycle.Value.outOfCycleReactions.Where(e => cycle.Value.graph.Edges.ContainsKey(e.Key)).ToList());

                foreach (var reaction in toRemove)
                {
                    HyperGraph.Edge _;
                    cycle.Value.graph.Edges.TryRemove(reaction.Key, out _);
                }

                cycle.Value.inCycleReactions =
                    cycle.Value.inCycleReactions.Where(e => !cycle.Value.outOfCycleReactions.ContainsKey(e.Key)).ToDictionary(e => e.Key, e => e.Value);
            }



            //using (var context = new CycleReactionModel())
            {
                Db.Context.CycleReactions.RemoveRange(Db.Context.CycleReactions.Where(e => true));
                Db.Context.Cycles.RemoveRange(Db.Context.Cycles.Where(e => true));
                Db.Context.SaveChanges();


                foreach (var cycle in cycles)
                {
                    var cycleModel = new DB2.Cycle { id = cycle.Key };

                    foreach (var reaction in cycle.Value.graph.Edges)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycle.Key, otherId = reaction.Key, isReaction = false });
                    }
                    foreach (var reaction in cycle.Value.inCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycle.Key, otherId = reaction.Key, isReaction = true });
                    }
                    foreach (var reaction in cycle.Value.outOfCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycle.Key, otherId = reaction.Key, isReaction = true });
                    }
                    Db.Context.Cycles.Add(cycleModel);
                }
                Db.Context.SaveChanges();
            }

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
        }
    }
}
