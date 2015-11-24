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
        private static int count = 0;

        public static Graph ConvertFromHypergraph(HyperGraph hyperGraph)
        {
            var graph = new Graph();

            foreach (var edge in hyperGraph.Edges)
            {
                graph.CreateNewReaction(edge.Key, edge.Value.IsReversible);
            }

            foreach (var edge in hyperGraph.Edges)
            {
                graph.AddEdgesToReaction(edge.Value);
            }

            return graph;
        }

        public static Guid CollapseCycle(Graph graph, List<Guid> cycle, HyperGraph hypergraph)
        {
            // debugging lines
            foreach (var v in cycle)
            {
                Console.WriteLine("reaction " + hypergraph.Edges[v].Label + "    isReversible = " + hypergraph.Edges[v].IsReversible);
                Console.Write("reactants:");
                foreach (var reactant in hypergraph.Edges[v].Reactants)
                {
                    Console.Write(reactant.Value.Label + "  ");
                }
                Console.WriteLine();
                Console.Write("products:");
                foreach (var product in hypergraph.Edges[v].Products)
                {
                    Console.Write(product.Value.Label + "  ");
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Guid cycleId = graph.CollapseCycle(cycle);
            

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

                // removing the reactions of the cycle from hypergraph are moved to recordToDatabase
            }

            return cycleId;
        }

//        public static Graph ConvertFromHypergraphTest(HyperGraph hyperGraph)
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
//
//                foreach (var reactant in edge.Value.Reactants)
//                {
//                    foreach (var producer in reactant.Value.Producers)
//                    {
//                        graph[edge.Key].Item1.Add(producer.Id);
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
            Db.Context.Database.ExecuteSqlCommand("INSERT INTO Cycle VALUES (@p0)", cycleId);


            foreach (var reaction in cycle)
            {
                //                bool _isReaction = (Db.Context.Reactions.Find(reaction) != null);
                //                cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycleModel.id, otherId = reaction, isReaction = !hypergraph.Edges[reaction].IsCycle });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleReaction(cycleId, otherId,isReaction ) VALUES (@p0, @p1, @p2)", cycleId, reaction, !hypergraph.Edges[reaction].IsCycle);
            }

            // recording metabolites
            var reversibleMetabolites = hypergraph.Edges[cycleId].Reactants.Intersect(hypergraph.Edges[cycleId].Products).ToDictionary(e => e.Key, e => e.Value);
            hypergraph.Edges[cycleId].Reactants = hypergraph.Edges[cycleId].Reactants.Except(reversibleMetabolites).ToDictionary(e => e.Key, e => e.Value);
            hypergraph.Edges[cycleId].Products = hypergraph.Edges[cycleId].Products.Except(reversibleMetabolites).ToDictionary(e => e.Key, e => e.Value);

            foreach (var reversibleMetabolite in reversibleMetabolites)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reversibleMetabolite.Key, roleId = Db.ReactantId, stoichiometry = reversibleMetabolite.Value.Weights[cycleId], isReversible = true });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, reversibleMetabolite.Key, Db.ReactantId, reversibleMetabolite.Value.Weights[cycleId], true);
            }
            foreach (var reactant in hypergraph.Edges[cycleId].Reactants)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reactant.Key, roleId = Db.ReactantId, stoichiometry = reactant.Value.Weights[cycleId], isReversible = false });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, reactant.Key, Db.ReactantId, reactant.Value.Weights[cycleId], true);
            }
            foreach (var product in hypergraph.Edges[cycleId].Products)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = product.Key, roleId = Db.ProductId, stoichiometry = product.Value.Weights[cycleId], isReversible = false });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, product.Key, Db.ReactantId, product.Value.Weights[cycleId], true);

            }

            // removing reactions from hypergraph
            // this is moved to DFS calling function
//            foreach (var reaction in cycle)
//            {
//                hypergraph.RemoveReaction(hypergraph.Edges[reaction]);
//            }
        }

        static void Main0()
        {
            Console.WriteLine("WARNING: This is going to erase the whole cycle database and record it from scratch, and this is going to take a lot of time");
            Console.WriteLine("Press any key if you are sure you want to continue ...");
            Console.ReadKey();

            const int Outliear = 61;
            var g = new HyperGraph();
            var count = 0;
            //.Where(p => Db.GetReactionCountSum(p.id) < Outliear)
            foreach (var sp in Db.Context.Species.ToList())
            {
                count++;
                if (count == 30)
                    break;

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
                

            // delete all entries from DB
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleReaction");
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleConnection");
            Db.Context.Database.ExecuteSqlCommand("DELETE FROM Cycle");
            Console.WriteLine("deleted DB entries");


            Graph graph = ConvertFromHypergraph(g);
            DFS.DetectAndCollapseCycles(graph, g);

                

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
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
            Species s1 = Db.Context.Species.Find("");

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

        static void Main()
        {
            Test4();
        }

        static void TestRealData()
        {
            Console.WriteLine("Testing on real data");

            var g = new HyperGraph();
            var count = 0;
            foreach (var sp in Db.Context.Species.ToList())
            {
                count++;
                if (count == 30)
                    break;

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


            Graph graph = ConvertFromHypergraph(g);
            DFS.DetectAndCollapseCycles(graph, g);

            Console.WriteLine("finished all");
            Console.ReadKey();
        }

        static void Test1()
        {
            //var m1 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
            //var m2 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
            //var m3 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
            //var m4 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");

            //var A = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaA");
            //var B = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaB");
            //var C = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaC");
            //var D = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaD");
            //var v5 = Guid.NewGuid();

            //var g = new HyperGraph();
            //g.AddNode(m1, "m1");
            //g.AddNode(m2, "m2");
            //g.AddNode(m3, "m3");
            //g.AddNode(m4, "m4");

            //g.AddReactant(A, "A", m2, "m2");
            //g.AddProduct(A, "A", m1, "m1");
            //g.AddProduct(A, "A", m3, "m3");
            //g.AddProduct(A, "A", m4, "m4");

            //g.AddReactant(B, "B", m1, "m1");
            //g.AddProduct(B, "B", m2, "m2");
            //g.AddProduct(B, "B", m3, "m3");
            //g.AddProduct(B, "B", m4, "m4");

            //g.AddReactant(C, "C", m3, "m3");
            //g.AddProduct(C, "C", m4, "m4");
            //g.AddProduct(C, "C", m1, "m1");
            //g.AddProduct(C, "C", m2, "m2");

            //g.AddReactant(D, "D", m4, "m4");
            //g.AddProduct(D, "D", m1, "m1");
            //g.AddProduct(D, "D", m2, "m2");
            //g.AddProduct(D, "D", m3, "m3");

            var m1 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
            var m2 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
            var m3 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
            var m4 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");
            var m5 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5");
            var m6 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6");
            var m7 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7");
            var m8 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8");
            var m9 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9");
            var m10 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10");

            var A = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaA");
            var B = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaB");
            var C = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaC");
            var D = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaD");
            var E = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaE");
            var F = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaF");
            var v5 = Guid.NewGuid();

            var g = new HyperGraph();
            g.AddNode(m1, "m1");
            g.AddNode(m2, "m2");
            g.AddNode(m3, "m3");
            g.AddNode(m4, "m4");
            g.AddNode(m5, "m5");
            g.AddNode(m6, "m6");
            g.AddNode(m7, "m7");
            g.AddNode(m8, "m8");
            g.AddNode(m9, "m9");
            g.AddNode(m10, "m10");
            
            g.AddReactant(A, "A", m2, "m2");
            g.AddReactant(A, "A", m9, "m9");
            g.AddReactant(A, "A", m7, "m7");
            g.AddProduct(A, "A", m1, "m1");
            g.AddProduct(A, "A", m8, "m8");

            g.AddReactant(B, "B", m1, "m1");
            g.AddReactant(B, "B", m4, "m4");
            g.AddProduct(B, "B", m2, "m2");
            g.AddProduct(B, "B", m3, "m3");

            g.AddReactant(C, "C", m3, "m3");
            g.AddReactant(C, "C", m6, "m6");
            g.AddProduct(C, "C", m4, "m4");
            g.AddProduct(C, "C", m5, "m5");
            g.AddProduct(C, "C", m10, "m10");

            g.AddReactant(D, "D", m5, "m5");
            g.AddReactant(D, "D", m8, "m8");
            g.AddProduct(D, "D", m7, "m7");
            g.AddProduct(D, "D", m6, "m6");

            g.AddProduct(E, "E", m9, "m9");

            g.AddReactant(F, "F", m10, "m10");

            foreach (var node in g.Nodes)
            {
                foreach (var edge in g.Edges)
                {
                    node.Value.Weights[edge.Key] = 1;
                }
            }

            Graph graph = ConvertFromHypergraph(g);
            DFS.DetectAndCollapseCycles(graph, g);
            Console.ReadKey();
        }


        static void Test2()
        {
            var m1 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
            var m2 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

            var A = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaA");
            var B = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaB");

            var g = new HyperGraph();
            g.AddNode(m1, "m1");
            g.AddNode(m2, "m2");

            g.AddReactant(A, "A", m1, "m1");
            g.AddProduct(A, "A", m2, "m2");

            g.AddReactant(B, "B", m1, "m1");
            g.AddReactant(B, "B", m2, "m2");
            g.Edges[B].IsReversible = true;

            foreach (var node in g.Nodes)
            {
                foreach (var edge in g.Edges)
                {
                    node.Value.Weights[edge.Key] = 1;
                }
            }

            Graph graph = ConvertFromHypergraph(g);
            DFS.DetectAndCollapseCycles(graph, g);
            Console.ReadKey();
        }

        static void Test4()
        {
            var m1 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
            var m2 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
            var m3 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
            var m4 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");
            var m5 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5");
            var m6 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6");
            var m7 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7");
            var m8 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8");
            var m9 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9");
            var m10 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10");

            var A = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaA");
            var B = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaB");
            var C = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaC");
            var D = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaD");
            var E = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaE");
            var F = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaF");

            var g = new HyperGraph();
            g.AddNode(m1, "m1");
            g.AddNode(m2, "m2");
            g.AddNode(m3, "m3");

            g.AddReactant(A, "A", m1, "m1");
            g.AddProduct(A, "A", m2, "m2");
            g.Edges[A].IsReversible = true;

            g.AddReactant(B, "B", m1, "m1");
            g.AddReactant(B, "B", m3, "m3");
            g.Edges[B].IsReversible = true;


            g.AddProduct(E, "E", m1, "m1");
            g.Edges[A].IsReversible = true;

            g.AddReactant(C, "C", m1, "m1");
            g.AddReactant(C, "C", m3, "m3");

            g.AddProduct(D, "D", m1, "m1");
            g.AddReactant(D, "D", m3, "m3");

            foreach (var node in g.Nodes)
            {
                foreach (var edge in g.Edges)
                {
                    node.Value.Weights[edge.Key] = 1;
                }
            }

            Graph graph = ConvertFromHypergraph(g);
            DFS.DetectAndCollapseCycles(graph, g);
            Console.ReadKey();
        }

        static void Test3()
        {
            var m1 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
            var m2 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
            var m3 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");

            var A = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaA");
            var B = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaB");

            var g = new HyperGraph();
            g.AddNode(m1, "m1");
            g.AddNode(m2, "m2");

            g.AddReactant(A, "A", m1, "m1");
            g.AddProduct(A, "A", m2, "m2");
            g.Edges[A].IsReversible = true;

            g.AddProduct(B, "B", m1, "m1");
            g.AddProduct(B, "B", m3, "m3");
            g.Edges[B].IsReversible = true;

            foreach (var node in g.Nodes)
            {
                foreach (var edge in g.Edges)
                {
                    node.Value.Weights[edge.Key] = 1;
                }
            }

            Graph graph = ConvertFromHypergraph(g);
            DFS.DetectAndCollapseCycles(graph, g);
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
