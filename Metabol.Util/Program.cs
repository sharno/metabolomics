namespace Metabol.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Metabol.Util.DB2;

    class Program
    {
        public static Dictionary<Guid, HashSet<Guid>> ConvertFromHypergraph(HyperGraph hyperGraph)
        {
            var graph = new Dictionary<Guid, HashSet<Guid>>();

            foreach (var edge in hyperGraph.Edges)
            {
                graph[edge.Key] = new HashSet<Guid>();

                foreach (var product in edge.Value.Products)
                {
                    foreach (var consumer in product.Value.Consumers)
                    {
                        graph[edge.Key].Add(consumer.Id);
                    }

                    // if one of the producers of the product is a reversible reaction
                    foreach (var producer in product.Value.Producers)
                    {
                        if (producer.ToServerReaction.reversible)
                        {
                            graph[edge.Key].Add(producer.Id);
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
                            graph[edge.Key].Add(consumer.Id);
                        }
                    }
                }
            }

            foreach (var key in graph.Keys)
            {
                Console.WriteLine(hyperGraph.Edges[key].Label);
                Console.WriteLine(graph[key].Count);
            }

            return graph;
        }

        public static Dictionary<Guid, HashSet<Guid>> ConvertFromHypergraphTest(HyperGraph hyperGraph)
        {
            Dictionary<Guid, HashSet<Guid>> graph = new Dictionary<Guid, HashSet<Guid>>();

            foreach (var edge in hyperGraph.Edges)
            {
                graph[edge.Key] = new HashSet<Guid>();

                foreach (var product in edge.Value.Products)
                {
                    foreach (var consumer in product.Value.Consumers)
                    {
                        graph[edge.Key].Add(consumer.Id);
                    }
                }
            }

            return graph;
        }

        public static void recordToDatabase(List<Guid> cycle)
        {
            //CycleReactionModel context = new CycleReactionModel();
            
            var cycleModel = new DB2.Cycle();
            cycleModel.id = Guid.NewGuid();

            foreach (var reaction in cycle)
            {
                cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycleModel.id, reactionId = reaction, isExchange = false });
            }
            Db.Context.Cycles.Add(cycleModel);
            Db.Context.SaveChanges();
        }

        static void Main()
        {
            {
                const int Outliear = 61;
                //var strCon = ConfigurationManager.AppSettings["dbConnectString"];
                //DBWrapper.Instance = new DBWrapper(strCon);
                var g = new HyperGraph();
                var count = 0;
                foreach (var sp in Db.Context.Species.Where(p => Db.GetReactionCountSum(p.id) < Outliear))
                {
                    count++;
                    if (count == 4)
                    {
                        break;
                    }
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

                //                using (var context = new CycleReactionModel())
                //                {
                //                    // delete all entries from DB
                //                    context.CycleReactions.RemoveRange(context.CycleReactions.Where(e => true));
                //                    context.Cycles.RemoveRange(context.Cycles.Where(e => true));
                //                    context.SaveChanges();
                //                    Console.WriteLine("deleted DB entries");
                //                }

                List<List<Guid>> cycles = SimpleCycle.CyclesFinder.Find(ConvertFromHypergraph(g));
                Console.WriteLine("Number of cycles: " + cycles.Count);

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


            List<List<Guid>> l = SimpleCycle.CyclesFinder.Find(ConvertFromHypergraphTest(graph));


            foreach (var cycle in l)
            {
                Console.WriteLine("cycle:");
                foreach (var edge in cycle.ToArray())
                {
                    Console.WriteLine(graph.Edges[edge].Label);
                }
            }
            Console.ReadKey();
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
                        cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycle.Key, reactionId = reaction.Key, isExchange = false });
                    }
                    foreach (var reaction in cycle.Value.inCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycle.Key, reactionId = reaction.Key, isExchange = true });
                    }
                    foreach (var reaction in cycle.Value.outOfCycleReactions)
                    {
                        cycleModel.CycleReactions.Add(new CycleReaction { cycleId = cycle.Key, reactionId = reaction.Key, isExchange = true });
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
