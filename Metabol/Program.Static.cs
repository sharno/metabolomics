namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    using PathwaysLib.ServerObjects;
    using System.Configuration;

    public class Program
    {
        public static void Main()
        {
            //var graph = new HyperGraph();
            //Console.WriteLine("Loading graph...");
            //var count = 0;
            //foreach (var m in ServerModel.Load(Guid.Parse("682C0D3C-8652-4A26-8CC3-4AFA845919B8")).GetAllSpecies())
            //{
            //    graph.AddNode(m.ID, m.SbmlId);
            //    foreach (var consumer in m.getAllReactions(Util.Reactant))
            //    {
            //        graph.AddReactant(consumer.ID, consumer.SbmlId, m.ID, m.SbmlId);
            //    }

            //    foreach (var producer in m.getAllReactions(Util.Product))
            //    {
            //        graph.AddProduct(producer.ID, producer.SbmlId, m.ID, m.SbmlId);
            //    }

            //    Console.Write("\r{0} metabolites  ", count++);
            //}
            //Console.WriteLine("M:{0}   R:{1}", graph.Nodes.Count, graph.Edges.Count);
            //var deadend = graph.Nodes.Values.Where(n => n.Consumers.Count == 0 || n.Producers.Count == 0).Select(n => n.Label + " " + Util.GetReactionCountSum(n.Id));
            //File.WriteAllLines("C:\\b\\deadend2.txt", deadend);
            //var lone = graph.Nodes.Values.Count(n => n.Consumers.Count == 0 && n.Producers.Count == 0);

            //foreach (var m in graph.Nodes.Values)
            //{
            //    if (graph.Nodes[m.Id].Producers.Count == 0)
            //        graph.AddProduct(Guid.NewGuid(), string.Format("exr_{0}_prod", m.Label), m.Id, m.Label, true);

            //    if (graph.Nodes[m.Id].Consumers.Count == 0)
            //        graph.AddReactant(Guid.NewGuid(), string.Format("exr_{0}_cons", m.Label), m.Id, m.Label, true);
            //}

            //var deadend2 = graph.Nodes.Values.Count(n => n.Consumers.Count == 0 || n.Producers.Count == 0);
            //var pseudo = graph.Edges.Values.Count(e => e.IsPseudo);
            //var reacts = graph.Edges.Values.Count();

            //Util.SaveAsDgs(graph.Nodes.First().Value, graph, "C://b/3before");
            //new FVA().Solve(graph);


            //CyclesFinder cyclesFinder = new CyclesFinder();
            //Dictionary<Guid, Dictionary<Guid, CyclesFinder.Vertex>> stronglyConnectedComponents = cyclesFinder.FindCycles(graph);
            //cyclesFinder.CollapseCycles(graph, stronglyConnectedComponents);
            //Util.SaveAsDgs(graph.Nodes.First().Value, graph, "C://b/3after");

            //Console.WriteLine("deadend:{0}  deadend2:{1}   lone:{2} | pseudo:{3}   reacts:{4}", deadend.Count(), deadend2, lone, pseudo, reacts);


            //Console.WriteLine("\nDone!\n");
            //Console.ReadKey();
            //TheAlgorithm.DefineBlockedReactions(graph);
        }

        public static void Main1()
        {
            var gen = new NetworkGenerator();
            gen.Gen1("");
        }

        #region main

        /// <summary>
        /// The main.
        /// </summary>
        public static void MainOriginal()
        {
            var p = new TheAlgorithm(); //InitProgram();
            //Init(p);
            p.Fba.RemoveConstraints = false;
            p.Start();
            do
            {
                //var it = p.Step(1).First();
                Console.WriteLine("\n************** ITERATION {0} *************** ", p.Iteration);
                p.Step();
                if (!p.IsFeasable)
                {
                    //p.Fba.RemoveConstraints = true;
                    foreach (var str in p.Pathway)
                    {
                        Console.Write(str+" => ");
                    }
                    Console.ReadKey();
                }

                //if (it.Id >= 21)
                //{
                //    p.Fba.RemoveConstraints = true;
                //    Console.ReadKey();
                //}

            }
            while (true); //count != sm.Edges.Count
        }

        private static void Init(TheAlgorithm Worker)
        {
            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);


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
            zlist.Sort((species, serverSpecies) => string.Compare(species.SbmlId, serverSpecies.SbmlId, StringComparison.Ordinal));
            //var rand = new Random((int)DateTime.UtcNow.ToBinary());

            foreach (var s in zlist)
            {
                Worker.Z[s.ID] = (s.ID.GetHashCode() % 2) == 0 ? -1 : 1;//rand.NextDouble() >= 0.5 ? 1 : -1;
                Console.WriteLine("{0}:{1}", s.SbmlId, Worker.Z[s.ID]);
            }

            //var id = Guid.Parse("{05954e8b-244a-4b59-b650-315f2c8e0f43}");
            var id = Guid.Parse("1218e2af-e534-41e6-bc59-e9731c95b182");
            Worker.Z[id] = (id.GetHashCode() % 2) == 0 ? -1 : 1; //rand.NextDouble() >= 0.5 ? 1 : -1;
            Console.WriteLine("{0}:{1}", Util.CachedS(id).SbmlId, Worker.Z[id]);
        }

        #endregion

        #region test

        //public static void Main3()
        //{
        //    string[] zn = { "M_h2o_e", "M_HC01441_e", "M_h_g", "M_h_e", "M_udpgal_g", "M_glc_D_e", "M_udp_g", "M_gal_e", "M_glc_D_c", "M_pi_e" };
        //    var zlist = (from s in zn select ServerSpecies.AllSpeciesByName(s) into spec where spec.Length > 0 select spec[0]).ToList();
        //    zlist.Sort((s1, s2) => Util.TotalReactions(s1.ID).CompareTo(Util.TotalReactions(s2.ID)));
        //    //var nodes = zlist.Select(s => HyperGraph.Node.Create(s.ID, s.SbmlId)).ToList();
        //    zlist.ForEach(s => Console.WriteLine("{0}: {1}", s.SbmlId, Util.TotalReactions(s.ID)));
        //    //var node = Util.LonelyMetabolite(nodes);
        //    //Console.WriteLine(node);
        //    Console.ReadKey();

        //}

        //public static void Main4()
        //{
        //    var context = SolverContext.GetContext();
        //    context.LoadModel(FileFormat.OML, @"A:\model2\1model.txt");
        //    var solution = context.Solve(new SimplexDirective());
        //    var report = solution.GetReport();
        //    Console.WriteLine(report);
        //    Console.ReadKey();
        //    Console.WriteLine(Util.Dir);
        //}

        //public static void Main5()
        //{
        //    var solver = new GLPKSolver();
        //    var model = new Model();
        //    using (var fs = File.OpenRead(@"A:\model2\4model.txt"))
        //        model.Load(fs, FileType.LP);
        //    var solution = solver.Solve(model);
        //    var report = solution.ModelStatus;
        //    Console.WriteLine(report);
        //    Console.ReadKey();
        //}

        public static void Main6()
        {
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
            foreach (var sp in ServerSpecies.AllSpecies())
            {
                foreach (var pr in sp.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddProduct(pr.ID, pr.SbmlId, sp.ID, sp.SbmlId);

                foreach (var re in sp.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddReactant(re.ID, re.SbmlId, sp.ID, sp.SbmlId);
            }

            var reactions = "";//g.Edges.Values.Select(TheAlgorithm.ToReaction).ToDictionary(e => e.Id);
            //var fba = new Fba();
            //fba.Solve(reactions, Z, g);
            //Console.ReadKey();
        }

        public static double Coefficient(HyperGraph.Edge reaction, HyperGraph.Node metabolite)
        {
            var coefficient = 0.0;

            if (reaction.Products.ContainsKey(metabolite.Id))
            {
                coefficient = metabolite.Weights[reaction.Id];
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsPseudo)
                    coefficient = 1;
            }

            if (reaction.Reactants.ContainsKey(metabolite.Id))
            {
                coefficient = -1 * metabolite.Weights[reaction.Id];
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsPseudo)
                    coefficient = -1;
            }
            return coefficient;
        }
        #endregion
    }
}
