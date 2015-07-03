namespace Metabol
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Practices.Unity;

    using Optimization;
    using Optimization.Solver;
    using Optimization.Solver.GLPK;

    using PathwaysLib.ServerObjects;

    public class Program
    {
        /// <summary>
        /// The selected meta.
        /// </summary>
        private static void SelectedMeta()
        {
            //var rand = new Random((int)DateTime.UtcNow.ToBinary());
            //var guid = new Guid[TheAlgorithm.AllReactionCache.Count];
            //TheAlgorithm.AllReactionCache.Keys.CopyTo(guid, 0);
            //Array.Sort(guid);
            //var sguid = new Guid[1000];
            //Array.Copy(guid, 0, sguid, 0, 1000);
            //var lines = sguid.ToList().Select(s => s.ToString() + ";" + (rand.NextDouble() >= 0.5 ? 1 : -1));
            //File.AppendAllLines(TheAlgorithm.SelectedMetaFile, lines);
        }

        #region main

        /// <summary>
        /// The main.
        /// </summary>
        internal static void Main1()
        {
            var p = InitProgram();
            p.Fba.RemoveConstraints = false;
            p.Start();
            do
            {
                var it = p.Step(1).First();
                if (it.Fba != 1)
                {
                    //p.Fba.RemoveConstraints = true;
                    Console.ReadKey();
                }

                //if (it.Id >= 23)
                //{
                //    p.Fba.RemoveConstraints = true;
                //    Console.ReadKey();
                //}


                //var h = gethist(p.hist, p.Sm);
                //using (TextWriter tw = new StreamWriter($"{Util.Dir}hist.csv", false))
                //{
                //    for (var j = 0; j < h.GetLength(1); j++)
                //    {
                //        tw.Write(h[0, j]);
                //        for (var i = 1; i < h.GetLength(0); i++)
                //        {
                //            tw.Write(";{0}", h[i, j]);
                //        }
                //        tw.WriteLine();
                //    }
                //}
                //File.WriteAllLines($"{Util.Dir}react.csv", p.hist.Keys.Select(e => e.ToString()));
            }
            while (true); //count != sm.Edges.Count
        }

        private static double[,] gethist(SortedDictionary<Guid, SortedSet<Tuple<int, double>>> vals, HGraph sm)
        {
            var c = vals.Values.Max(e => e.Max.Item1) + 1;
            var mat = new double[vals.Values.Count, c];
            var j = 0;
            foreach (var key in vals)
            {
                if (!sm.Edges.ContainsKey(key.Key))
                {
                    j++; continue;
                }
                foreach (var tuple in key.Value)
                    mat[j, tuple.Item1] = tuple.Item2;
                j++;
            }
            return mat;
        }

        /// <summary>
        /// The init program.
        /// </summary>
        /// <returns>
        /// The <see cref="Program"/>.
        /// </returns>
        private static TheAlgorithm InitProgram()
        {
            var user = new User(Guid.NewGuid().ToString());
            //var d = DateTime.Now.TimeOfDay.Ticks;
            //user.Worker.file1 = $"C:\\Users\\f\\Desktop\\1bench{d}.txt";
            //user.Worker.file2 = $"C:\\Users\\f\\Desktop\\2bench{d}.txt";
            return user.Worker;
        }

        #endregion

        #region test

        internal static void Main3()
        {
            string[] zn = { "M_h2o_e", "M_HC01441_e", "M_h_g", "M_h_e", "M_udpgal_g", "M_glc_D_e", "M_udp_g", "M_gal_e", "M_glc_D_c", "M_pi_e" };
            var zlist = (from s in zn select ServerSpecies.AllSpeciesByName(s) into spec where spec.Length > 0 select spec[0]).ToList();
            zlist.Sort((s1, s2) => Util.TotalReactions(s1.ID).CompareTo(Util.TotalReactions(s2.ID)));
            //var nodes = zlist.Select(s => HGraph.Node.Create(s.ID, s.SbmlId)).ToList();
            zlist.ForEach(s => Console.WriteLine("{0}: {1}", s.SbmlId, Util.TotalReactions(s.ID)));
            //var node = Util.LonelyMetabolite(nodes);
            //Console.WriteLine(node);
            Console.ReadKey();

        }

        internal static void Main4()
        {
            //var context = SolverContext.GetContext();
            //context.LoadModel(FileFormat.OML, @"A:\model21\13model.txt");
            //var solution = context.Solve(new SimplexDirective());
            //var report = solution.GetReport();
            //Console.WriteLine(report);
            //Console.ReadKey();
            //Console.WriteLine(Util.Dir);
        }

        internal static void Main5()
        {
            var g = new HGraph();
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
                    g.AddOuputNode(pr.ID, pr.SbmlId, sp.ID, sp.SbmlId);

                foreach (var re in sp.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
                    g.AddInputNode(re.ID, re.SbmlId, sp.ID, sp.SbmlId);
            }

            var reactions = "";//g.Edges.Values.Select(TheAlgorithm.ToReaction).ToDictionary(e => e.Id);
            //var fba = new Fba();
            //fba.Solve(reactions, Z, g);
            //Console.ReadKey();
        }

        internal static void Main()
        {
            var graph = new HGraph();
            var m0 = graph.AddNode(Guid.NewGuid(), "m0");
            graph.AddOuputNode(Guid.NewGuid(), "ex0", m0.Id, m0.Label, true);
            graph.AddInputNode(Guid.NewGuid(), "r0", m0.Id, m0.Label);

            var results = new Dictionary<string, double>();
            var prevResults = new Dictionary<string, double>();
            var ri = m0.InputToEdge.First();
            var mi = m0;

            for (var i = 1; i < 100; i++)
            {
                graph.NextStep();
              
                var solver = new GLPKSolver();
                var model = new Model { Name = "FBA" };
                foreach (var edge in graph.Edges)
                    model.AddVariable($"{edge.Value.Label}", 1.0, 1000, VariableType.Continuous);

                var metabolites = new SortedSet<HGraph.Node>();

                foreach (var reaction in graph.Edges.Values)
                    metabolites.UnionWith(reaction.AllNodes());

                var j = 0;
                foreach (var reaction in graph.Edges.Where(reaction => results.ContainsKey(reaction.Value.Label)))
                    model.AddConstraint(new Term(model.GetVariable(reaction.Value.Label)) == results[reaction.Value.Label], $"prev{j++}");

                foreach (var metabolite in metabolites)
                {
                    var sv = Expression.Sum(new[] { 0.0 });
                    foreach (var react in graph.Edges)
                    {
                        var coefficient = react.Value.InputNodes.ContainsKey(metabolite.Id)
                                              ? 1
                                              : react.Value.OuputNodes.ContainsKey(metabolite.Id) ? -1 : 0;

                        if (coefficient == 0) continue;
                        sv = sv + new Term(model.GetVariable(react.Value.Label), coefficient);
                    }

                    model.AddConstraint(sv == 0, metabolite.Label);
                }

                model.AddObjective(new Term(model.GetVariable(m0.InputToEdge.First().Label)), "Fobj", ObjectiveSense.Maximize);

                var solution = solver.Solve(model);
                results.ToList().ForEach(d => prevResults[d.Key] = d.Value);

                if (solution.ModelStatus == ModelStatus.Feasible)
                    solution.VariableValues.ToList().ForEach(d => results[d.Key] = d.Value);
                else
                    model.Variables.ToList().ForEach(d => results[d.Name] = 0);

                Util.SaveAsDgs(mi, graph, results, prevResults);

                var list = results.Select(d => $"{d.Key}:{d.Value}").ToList();
                list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
                File.WriteAllLines($"{Util.Dir}{graph.LastLevel}result.txt", list);

                model.Write(File.Create($"{Util.Dir}{graph.LastLevel}model.txt"), FileType.LP);

                mi = graph.AddNode(Guid.NewGuid(), $"m{i}");

                graph.AddOuputNode(ri.Id, ri.Label, mi.Id, mi.Label);
                graph.AddInputNode(Guid.NewGuid(), $"r{i}", mi.Id, mi.Label);
                ri = mi.InputToEdge.First();

                if (i < 2) continue;

                for (var k = 0; k < i - 1; k++)
                {
                    var rb = graph.Edges.First(e => e.Value.Label == $"r{k}");
                    graph.AddInputNode(rb.Key, rb.Value.Label, mi.Id, mi.Label);
                }
            }
        }
        #endregion
    }
}
