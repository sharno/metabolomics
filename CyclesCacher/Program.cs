using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol;
using PathwaysLib.ServerObjects;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Threading;
using CyclesCacher.DB;
using CyclesCacher.SimpleCycle;


namespace CyclesCacher
{
    class Program
    {
        public static Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> ConvertFromHypergraph(HyperGraph hyperGraph)
        {
            Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph = new Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>>();

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
                        if (producer.ToServerReaction.Reversible)
                        {
                            graph[edge.Key].Item2.Add(producer.Id);
                        }
                    }
                }
                // if it's a reversible reaction
                if (edge.Value.ToServerReaction.Reversible)
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
                        if (consumer.ToServerReaction.Reversible)
                        {
                            graph[edge.Key].Item1.Add(consumer.Id);
                        }
                    }
                }
                // if it's a reversible reaction
                if (edge.Value.ToServerReaction.Reversible)
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

        public static Guid CollapseCycle(Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph, List<Guid> cycle)
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

            return cycleId;
        }

        public static Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> ConvertFromHypergraphTest(HyperGraph hyperGraph)
        {
            Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph = new Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>>();

            foreach (var edge in hyperGraph.Edges)
            {
                graph[edge.Key] = Tuple.Create(new HashSet<Guid>(), new HashSet<Guid>(), true);

                foreach (var product in edge.Value.Products)
                {
                    foreach (var consumer in product.Value.Consumers)
                    {
                        graph[edge.Key].Item2.Add(consumer.Id);
                    }
                }
            }

            return graph;
        }

        public static void recordToDatabase(Guid cycleId, List<Guid> cycle)
        {
            CycleReactionModel context = new CycleReactionModel();
            var cycleModel = new DB.Cycle();
            cycleModel.id = cycleId;

            foreach (var reaction in cycle)
            {
                bool _isReaction = (context.Reactions.Find(reaction) != null);
                cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycleModel.id, reactionId = reaction, isReaction = _isReaction});
            }
            context.Cycles.Add(cycleModel);
            context.SaveChanges();
        }

        static void Main()
        {
            {
                const int Outliear = 61;
                var strCon = ConfigurationManager.AppSettings["dbConnectString"];
                DBWrapper.Instance = new DBWrapper(strCon);

                var g = new HyperGraph();
                string[] zn =
               {
                 "ADP", "ATP(4-)",
                 "D-Fructose 6-phosphate", "D-Fructose 1,6-bisphosphate", "Dihydroxyacetone phosphate",
                 "Glyceraldehyde 3-phosphate", "L-threonine", "taurochenodeoxycholate",
                 "D-glucose", "3-Phospho-D-glycerate", "D-Glycerate 2-phosphate", "Phosphoenolpyruvate", "pyruvate",
                 "Prothrombin", "pantetheine"
             };

                var zlist =
                    (from s in zn select ServerSpecies.AllSpeciesByNameOnly(s) into spec where spec.Length > 0 select spec[0])
                        .ToList();
                var rand = new Random((int)DateTime.UtcNow.ToBinary());
                var Z = new Dictionary<Guid, int>();
                foreach (var s in zlist)
                    Z[s.ID] = rand.NextDouble() >= 0.5 ? 1 : -1;

                var count = 0;
                foreach (var sp in ServerSpecies.AllSpecies().Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
                {
                    count++;
//                    if (count == 500)
//                    {
//                        break;
//                    }
                    Console.WriteLine("adding metabolite " + count);


                    foreach (var pr in sp.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
                        g.AddProduct(pr.ID, pr.SbmlId, sp.ID, sp.SbmlId);

                    foreach (var re in sp.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
                        g.AddReactant(re.ID, re.SbmlId, sp.ID, sp.SbmlId);
                }

                Console.WriteLine("loaded the whole network");

                // var reactions = "";
                //g.Edges.Values.Select(TheAlgorithm.ToReaction).ToDictionary(e => e.Id);
                //var fba = new Fba();
                //fba.Solve(reactions, Z, g);
                //Console.ReadKey();

                using (var context = new CycleReactionModel())
                {
                    // delete all entries from DB
                    context.CycleReactions.RemoveRange(context.CycleReactions.Where(e => true));
                    context.Cycles.RemoveRange(context.Cycles.Where(e => true));
                    context.SaveChanges();
                    Console.WriteLine("deleted DB entries");
                }


                Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph = ConvertFromHypergraph(g);
                DFS.DetectAndCollapseCycles(graph);
//                while (SimpleCycle.CyclesFinder.Find(graph))
//                {
//                    Console.WriteLine("found cycle");
//                }

                //                List<List<Guid>> cycles = SimpleCycle.CyclesFinder.Find(ConvertFromHypergraph(g));
                //                Console.WriteLine("Number of cycles: " + cycles.Count);

                using (var context = new CycleReactionModel())
                {
                    // delete all entries from DB
                    //                    context.CycleReactions.RemoveRange(context.CycleReactions.Where(e => true));
                    //                    context.Cycles.RemoveRange(context.Cycles.Where(e => true));
                    //                    context.SaveChanges();
                    //
                    //
                    //                    foreach (var cycle in cycles)
                    //                    {
                    //                        var cycleModel = new DB.Cycle();
                    //                        cycleModel.id = Guid.NewGuid();
                    //
                    //                        foreach (var reaction in cycle)
                    //                        {
                    //                            cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycleModel.id, reactionId = reaction.Id, isExchange = false });
                    //                        }
                    //                        context.Cycles.Add(cycleModel);
                    //                    }
                    //                    context.SaveChanges();
                }

                Console.WriteLine("finished saving to DB");
                Console.ReadKey();
            }
        }

        static void Main2()
        {
            var graf = new HyperGraph();

            var rs = new[]
            {
                ServerReaction.Load(Guid.Parse("D4F08E74-2FED-4125-871B-26C5654153B6")),
                ServerReaction.Load(Guid.Parse("ABE9C0CA-CFE2-4981-B862-69765EA1AEAF")),
                ServerReaction.Load(Guid.Parse("1D947A60-F035-443D-86AF-7702E9B5AC98")),
                ServerReaction.Load(Guid.Parse("CAB95D5E-F91D-4806-A6FE-A55B3C5DAFAC")),
                ServerReaction.Load(Guid.Parse("4377C959-2090-4AA8-AED5-C6D777A632C7")),
                ServerReaction.Load(Guid.Parse("9602D7E7-A48B-41F0-B0BC-470D93F7F517")),
                ServerReaction.Load(Guid.Parse("5D4604A6-7AAA-4A26-9582-90C7B668570F")),
                ServerReaction.Load(Guid.Parse("85E356DD-03F6-4774-8962-494E536CCA4A")),
                ServerReaction.Load(Guid.Parse("BE32A9D4-4E83-4820-97FF-67163DF6BA04")),
                ServerReaction.Load(Guid.Parse("CE2296A9-3E7D-4A16-B380-33A211CFC1B3")),
                ServerReaction.Load(Guid.Parse("EEFDAEF0-455F-49EC-915B-1B483DF23D39")),
                ServerReaction.Load(Guid.Parse("6D7FF530-F6BF-45C2-90FC-9409E68C3590"))
            };

            foreach (var reaction in rs)
            {
                foreach (var m in reaction.GetAllProducts())
                {
                    graf.AddProduct(reaction.ID, reaction.SbmlId, m.ID, m.SbmlId);
                }
                foreach (var m in reaction.GetAllReactants())
                {
                    graf.AddReactant(reaction.ID, reaction.SbmlId, m.ID, m.SbmlId);
                }
            }
            Util.SaveAsDgs(graf.Nodes.First().Value, graf, "C:\\Users\\sharno\\Desktop\\");
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
            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);

            var g = new HyperGraph();
            string[] zn =
           {
                 "ADP", "ATP(4-)",
                 "D-Fructose 6-phosphate", "D-Fructose 1,6-bisphosphate", "Dihydroxyacetone phosphate",
                 "Glyceraldehyde 3-phosphate", "L-threonine", "taurochenodeoxycholate",
                 "D-glucose", "3-Phospho-D-glycerate", "D-Glycerate 2-phosphate", "Phosphoenolpyruvate", "pyruvate",
                 "Prothrombin", "pantetheine"
             };

            var zlist =
                (from s in zn select ServerSpecies.AllSpeciesByNameOnly(s) into spec where spec.Length > 0 select spec[0])
                    .ToList();
            var rand = new Random((int)DateTime.UtcNow.ToBinary());
            var Z = new Dictionary<Guid, int>();
            foreach (var s in zlist)
                Z[s.ID] = rand.NextDouble() >= 0.5 ? 1 : -1;

            var count = 0;
            foreach (var sp in ServerSpecies.AllSpecies().Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
            {
                count++;
                //                if (count == 300)
                //                {
                //                    break;
                //                }
                Console.WriteLine("adding metabolite " + count);


                foreach (var pr in sp.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddProduct(pr.ID, pr.SbmlId, sp.ID, sp.SbmlId);

                foreach (var re in sp.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddReactant(re.ID, re.SbmlId, sp.ID, sp.SbmlId);
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



            using (var context = new CycleReactionModel())
            {
                context.CycleReactions.RemoveRange(context.CycleReactions.Where(e => true));
                context.Cycles.RemoveRange(context.Cycles.Where(e => true));
                context.SaveChanges();


                foreach (var cycle in cycles)
                {
                    var cycleModel = new DB.Cycle() { id = cycle.Key };

                    foreach (var reaction in cycle.Value.graph.Edges)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycle.Key, reactionId = reaction.Key });
                    }
                    foreach (var reaction in cycle.Value.inCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycle.Key, reactionId = reaction.Key });
                    }
                    foreach (var reaction in cycle.Value.outOfCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycle.Key, reactionId = reaction.Key });
                    }
                    context.Cycles.Add(cycleModel);
                }
                context.SaveChanges();
            }

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
        }
    }
}
