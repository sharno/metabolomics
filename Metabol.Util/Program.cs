using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Security.Cryptography;
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
        public static HyperGraph.Cycle CollapseCycle(List<HyperGraph.Entity> cycle, HyperGraph hypergraph)
        {
            // debugging lines
            foreach (var v in cycle)
            {
                Console.WriteLine("Entity " + v.Label + " " + v.Id);
                //                Console.Write("prev:");
                //                foreach (var prev in v.Previous)
                //                {
                //                    Console.Write(prev.Value.Label + "  ");
                //                }
                //                Console.WriteLine();
                //                Console.Write("next:");
                //                foreach (var nxt in v.Next)
                //                {
                //                    Console.Write(nxt.Value.Label + "  ");
                //                }
                //                Console.WriteLine();
                //                Console.WriteLine();
            }
            Console.WriteLine();


            HyperGraph.Cycle cycleReaction = new HyperGraph.Cycle();

            // modify hypergraph
            // TODO add weights if needed
            foreach (var v in cycle)
            {
                if (v is HyperGraph.Node)
                {
                    // if this metabolite have any outside connection it should be added with 2 edges as it's consumed and produced inside the cycle
                    if (v.Next.Values.Union(v.Previous.Values).Any(e => !cycle.Contains(e)))
                    {
                        hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)v, 1/*((HyperGraph.Node)v).Weights[((HyperGraph.Node)v).Producers.First().Id]*/);
                        hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)v, 1/*((HyperGraph.Node)v).Weights[((HyperGraph.Node)v).Consumers.First().Id] */);
                    }
                    else
                    {
                        hypergraph.RemoveNode(v.Id);
                    }
                }
                // cycle should come first because it's a child of edge
                else if (v is HyperGraph.Cycle)
                {
                    // for separate metabolites
                    var outsideReactants = v.Previous.Values.Except(cycle);
                    var outsideProducts = v.Next.Values.Except(cycle);

                    foreach (var outsideProduct in outsideProducts)
                    {
                        hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)outsideProduct, 1);
                    }
                    foreach (var outsideReactant in outsideReactants)
                    {
                        hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)outsideReactant, 1);
                    }


                    // for inside reactions
                    foreach (var reaction in ((HyperGraph.Cycle)v).InterfaceReactions.Values)
                    {
                        if (reaction.Products.Union(reaction.Reactants).Any(m => !cycle.Contains(m.Value)))
                        {
                            foreach (var e in cycle)
                            {
                                reaction.Products.Remove(e.Id);
                                reaction.Reactants.Remove(e.Id);
                            }

                            cycleReaction.InterfaceReactions.Add(reaction.Id, reaction);
                        }
                    }

                    hypergraph.RemoveCycle((HyperGraph.Cycle)v);
                }
                else if (v is HyperGraph.Edge)
                {
                    var outsideReactants = v.Previous.Values.Except(cycle);
                    var outsideProducts = v.Next.Values.Except(cycle);

                    foreach (var outsideProduct in outsideProducts)
                    {
                        hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)outsideProduct, ((HyperGraph.Node)outsideProduct).Weights[v.Id]);
                    }
                    foreach (var outsideReactant in outsideReactants)
                    {
                        hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)outsideReactant, ((HyperGraph.Node)outsideReactant).Weights[v.Id]);
                    }

                    if (((HyperGraph.Edge)v).IsReversible)
                    {
                        foreach (var outsideProduct in outsideProducts)
                        {
                            hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)outsideProduct, ((HyperGraph.Node)outsideProduct).Weights[v.Id]);
                        }
                        foreach (var outsideReactant in outsideReactants)
                        {
                            hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)outsideReactant, ((HyperGraph.Node)outsideReactant).Weights[v.Id]);
                        }
                    }

                    hypergraph.RemoveReaction((HyperGraph.Edge)v);
                }
            }

            return cycleReaction;
        }

        public static void recordToDatabase(HyperGraph.Cycle cycleReaction, List<HyperGraph.Entity> cycle)
        {
            //var cycleModel = new DB2.Cycle();
            //cycleModel.id = cycleId;

            Guid cycleId = cycleReaction.Id;
            Db.Context.Database.ExecuteSqlCommand("INSERT INTO Cycle VALUES (@p0)", cycleId);


            foreach (var reaction in cycle)
            {
                //                bool _isReaction = (Db.Context.Reactions.Find(reaction) != null);
                //                cycleModel.CycleReactions.Add(new CycleReaction() { cycleId = cycleModel.id, otherId = reaction, isReaction = !hypergraph.Edges[reaction].IsCycle });
                if (reaction is HyperGraph.Edge)
                {
                    Db.Context.Database.ExecuteSqlCommand(
                        "INSERT INTO CycleReaction(cycleId, otherId, isReaction) VALUES (@p0, @p1, @p2)", cycleId,
                        reaction.Id, !(reaction is HyperGraph.Cycle));
                }
            }

            // recording metabolites
            var reversibleMetabolites = cycleReaction.Reactants.Intersect(cycleReaction.Products);
            var reactants = cycleReaction.Reactants.Except(reversibleMetabolites);
            var products = cycleReaction.Products.Except(reversibleMetabolites);

            foreach (var reaction in cycleReaction.InterfaceReactions.Values)
            {
                if (reaction.IsReversible)
                {
                    reversibleMetabolites = reversibleMetabolites.Union(reaction.Reactants).Union(reaction.Products);
                }
                else
                {
                    reactants = reactants.Union(reaction.Reactants);
                    products = products.Union(reaction.Products);
                }
            }

            foreach (var reversibleMetabolite in reversibleMetabolites)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reversibleMetabolite.Key, roleId = Db.ReactantId, stoichiometry = reversibleMetabolite.Value.Weights[cycleId], isReversible = true });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, reversibleMetabolite.Key, Db.ReversibleId, reversibleMetabolite.Value.Weights[cycleId], true);
            }
            foreach (var reactant in reactants)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reactant.Key, roleId = Db.ReactantId, stoichiometry = reactant.Value.Weights[cycleId], isReversible = false });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, reactant.Key, Db.ReactantId, reactant.Value.Weights[cycleId], false);
            }
            foreach (var product in products)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = product.Key, roleId = Db.ProductId, stoichiometry = product.Value.Weights[cycleId], isReversible = false });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, product.Key, Db.ProductId, product.Value.Weights[cycleId], false);

            }
        }

        static void EraseAndRecordToDb()
        {
            Console.WriteLine("WARNING: This is going to erase the whole cycle database and record it from scratch, and this is going to take a lot of time");
            Console.WriteLine("Press any key if you are sure you want to continue ...");
            Console.ReadKey();

            const int Outlier = 61;
            var count = 0;

            var g = ConstructHyperGraphFromSpecies(Db.Context.Species.Where(s => s.ReactionSpecies.Count < Outlier));

            Console.WriteLine("loaded the whole network");


            // delete all entries from DB
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleReaction");
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleConnection");
            Db.Context.Database.ExecuteSqlCommand("DELETE FROM Cycle");
            Console.WriteLine("deleted DB entries");


            Dictionary<HyperGraph.Cycle, List<HyperGraph.Entity>> cycles = DFS.DetectAndCollapseCycles(g);

            foreach (var cycle in cycles)
            {
                recordToDatabase(cycle.Key, cycle.Value);
            }

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
        }

        static HyperGraph ConstructHyperGraphFromSpecies(IEnumerable<Species> species)
        {
            var g = new HyperGraph();
            foreach (var s in species)
            {
                g.AddSpecies(s);
            }
            return g;
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
            EraseAndRecordToDb();
        }

        static void TestRealData()
        {
            Console.WriteLine("Testing on real data");

            //            var g = new HyperGraph();
            //            var count = 0;
            //            foreach (var sp in Db.Context.Species.ToList())
            //            {
            //                count++;
            ////                if (count == 500)
            ////                    break;

            //                Console.WriteLine("adding metabolite " + count);

            //                foreach (var pr in sp.ReactionSpecies
            //                    .Where(
            //                        rs =>
            //                            rs.speciesId == sp.id && rs.roleId == Db.ProductId &&
            //                            rs.Reaction.sbmlId != "R_biomass_reaction"))
            //                {
            //                    g.AddProduct(pr.reactionId, pr.Reaction.sbmlId, sp.id, sp.sbmlId);
            //                    g.Edges[pr.reactionId].IsReversible = pr.Reaction.reversible;
            //                }
            //                foreach (var pr in sp.ReactionSpecies
            //                    .Where(
            //                        rs =>
            //                            rs.speciesId == sp.id && rs.roleId == Db.ReactantId &&
            //                            rs.Reaction.sbmlId != "R_biomass_reaction"))
            //                {
            //                    g.AddReactant(pr.reactionId, pr.Reaction.sbmlId, sp.id, sp.sbmlId);
            //                    g.Edges[pr.reactionId].IsReversible = pr.Reaction.reversible;
            //                }
            //            }
            int outlier = 61;
            var g = ConstructHyperGraphFromSpecies(Db.Context.Species.Where(s => s.ReactionSpecies.Count < outlier));

            Console.WriteLine("loaded the whole network");


            var cycles = DFS.DetectAndCollapseCycles(g);
            var ms = cycles.SelectMany(c => c.Key.Products.Values).Union(cycles.SelectMany(c => c.Key.Reactants.Values)).Union(cycles.SelectMany(c => c.Key.InterfaceReactions.Values).SelectMany(r => r.Products.Values)).Union(cycles.SelectMany(c => c.Key.InterfaceReactions.Values).SelectMany(r => r.Reactants.Values));
            var t = ms.ToDictionary(e=> e, e=>ms.Count(a => a.Equals(e)));

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

            DFS.DetectAndCollapseCycles(g);
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

            DFS.DetectAndCollapseCycles(g);
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
            g.AddProduct(B, "B", m3, "m3");
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

            DFS.DetectAndCollapseCycles(g);
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
            g.AddReactant(B, "B", m3, "m3");
            g.Edges[B].IsReversible = true;

            foreach (var node in g.Nodes)
            {
                foreach (var edge in g.Edges)
                {
                    node.Value.Weights[edge.Key] = 1;
                }
            }

            DFS.DetectAndCollapseCycles(g);
            Console.ReadKey();
        }

        static void Test5()
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
            g.AddProduct(B, "B", m2, "m2");
            g.Edges[B].IsReversible = true;

            foreach (var node in g.Nodes)
            {
                foreach (var edge in g.Edges)
                {
                    node.Value.Weights[edge.Key] = 1;
                }
            }

            DFS.DetectAndCollapseCycles(g);
            Console.ReadKey();
        }
    }
}
