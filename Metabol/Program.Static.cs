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
        public static void Main1()
        {
            var gen = new NetworkGenerator();
            gen.Gen1("");
        }

        #region main

        /// <summary>
        /// The main.
        /// </summary>
        public static void Main()
        {
            var p = new TheAlgorithm(); //InitProgram();
            Init(p);
            p.Fba.RemoveConstraints = false;
            p.Start();
            do
            {
                //var it = p.Step(1).First();
                Console.WriteLine("*******ITERATION  " + p.Iteration);
                p.Step();
                if (!p.IsFeasable)
                {
                    //p.Fba.RemoveConstraints = true;
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

            var id = Guid.Parse("{05954e8b-244a-4b59-b650-315f2c8e0f43}");
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
