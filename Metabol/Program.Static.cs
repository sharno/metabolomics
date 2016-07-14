using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Antlr.Runtime.Tree;
using Ecoli;
using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;
using Metabol.Util.DB2;
using Newtonsoft.Json;
using Optimization;
using Optimization.Solver.Cplex;
using Optimization.Solver.GLPK;
using Db = Metabol.DbModels.Db;
using Model = Optimization.Model;
using ObjectiveSense = ILOG.Concert.ObjectiveSense;

namespace Metabol
{
    using System;
    using System.Linq;
    using Metabol.Util;

    public class Program
    {
        public static void FVA()
        {
            //Core.TimeSeries();
            //Console.ReadKey();

            //var g = Db.LoadGraph();
            //var fva = new FVA();
            //fva.Solve(g);

            //var fixedReactionses = fva.Results.Select(result => new FixedReactions
            //{
            //    id = Guid.NewGuid(),
            //    reactionId = result.Key,
            //    lowerBound = result.Value.Item1,
            //    upperBound = result.Value.Item2
            //});
            //Db.Context.FixedReactions.RemoveRange(Db.Context.FixedReactions);
            //Db.Context.FixedReactions.AddRange(fixedReactionses);
            //Db.Context.SaveChanges();
        }

        public static void Debug(string mfile, IDictionary<string, double> prevals, string dir)
        {
            var model = new Cplex { Name = "FBA" };
            model.SetParam(Cplex.Param.Preprocessing.Presolve, false);
            model.ImportModel(mfile);


            var m = model.GetLPMatrixEnumerator();
            m.MoveNext();
            var mat = (CpxLPMatrix)m.Current;

            var vars = mat.NumVars;
            var ranges = mat.GetRanges();
            var obj = model.GetObjective();

            if (File.Exists($"{dir}debug.txt"))
                File.Delete($"{dir}debug.txt");

            if (File.Exists($"{dir}debug_val.txt"))
                File.Delete($"{dir}debug_val.txt");

            //Model.Solve();
            //foreach (var numVar in vars)
            //{
            //    prevals[Regex.Replace(numVar.Name, "#\\d+", "")] = Model.GetValue(numVar);
            //}

            for (var i = 0; i < ranges.Length; i++)
            {
                var model2 = new Cplex { Name = ranges[i].Name };
                model2.SetParam(Cplex.Param.Preprocessing.Presolve, false);
                var cloner = new SimpleCloneManager(model2);
                foreach (var var in vars)
                {
                    model2.NumVar(var.LB, var.UB, var.Type, var.Name);
                }

                for (var j = 0; j < ranges.Length; j++)
                {
                    if (j == i) continue;

                    model2.AddRange(ranges[j].LB, (CpxQLExpr)ranges[j].Expr.MakeClone(cloner), ranges[j].UB, ranges[j].Name);
                }

                model2.AddObjective(obj.Sense, (CpxQLExpr)obj.Expr.MakeClone(cloner));

                model2.Solve();
                File.AppendAllLines($"{dir}debug.txt", new[] { $"{ranges[i].Name}:{model2.GetStatus()}" });

                model2.EndModel();

                var ex = Regex.Replace(ranges[i].Expr.ToString(), "#\\d+|\\(|\\)|", "");
                var evars = (from str in (from e in ex.Split('+', '-')
                                          select Regex.Replace(e, "\\d+\\*", "").Trim())
                             where !string.IsNullOrEmpty(str)
                             select str).ToList();

                var ex1 = evars.Where(prevals.ContainsKey)
                    .Aggregate(ex + "", (current, var) => current.Replace(var, $"{var}({prevals[var]})"));

                var ex2 = evars.Where(prevals.ContainsKey)
                  .Aggregate(ex + "", (current, var) => current.Replace(var, $"{prevals[var]}"));

                if (Math.Abs(ranges[i].LB - ranges[i].UB) <= double.Epsilon)
                {
                    ex1 = $"{ex1}=={ranges[i].LB}";
                    ex2 = $"{ex2}=={ranges[i].LB}";
                }
                else if (Math.Abs(ranges[i].LB - double.MinValue) < double.Epsilon)
                {
                    ex1 = $"{ex1}<={ranges[i].UB}";
                    ex2 = $"{ex2}<={ranges[i].UB}";

                }
                else if (Math.Abs(ranges[i].UB - double.MaxValue) < double.Epsilon)
                {
                    ex1 = $"{ex1}>={ranges[i].LB}";
                    ex2 = $"{ex2}>={ranges[i].LB}";
                }

                File.AppendAllLines($"{dir}debug_val.txt", new[] { $"{ex2}|{ex1}" });
            }
        }

        public static void XmlToDB()
        {
            //sm.Edges.ToList().ForEach(d => d.Value.Flux = Model.GetValue(vars[d.Value.Label]));
            //Debug("A://1model.lp", new Dictionary<string, double>(), "A://");

            //dynamic ecoli = JsonConvert.DeserializeObject(File.ReadAllText("E:\\Dropbox\\Metabolomics\\Data\\ecoli_core.json"));

            //Console.WriteLine($"{ecoli.reactions.Count}");
            //Console.WriteLine($"{ecoli.genes.Count}");
            //Console.WriteLine($"{ecoli.metabolites.Count}");
            var file = "E:\\Dropbox\\Metabolomics\\Data\\recon2.v02.xml";
            //var file1 = "A:\\pom.xml";

            dynamic recon = DynamicXml.Load(file);

            foreach (var s in recon.model.listOfSpecies)
            {
                string id = $"{Regex.Replace(s.id as string, "^M_|^m_|_\\w$", "").ToLower()}[{s.compartment}]";

                try
                {

                    var species = Db.Context.Species.Single(e => e.sbmlId == id);
                    species.Sbase.notes = s.Contains("notes") ? s.notes.Text : "";
                    species.Sbase.sboTerm = s.sboTerm;
                    species.Sbase.annotation = s.Contains("annotation") ? s.annotation.Text : "";
                    Db.Context.SaveChanges();
                    Console.WriteLine(s.id);
                }
                catch (Exception)
                {
                    // ignored
                }

            }
            foreach (var r in recon.model.listOfReactions)
            {
                string id = $"{Regex.Replace(r.id as string, "^R_|^m_|_\\w$", "").ToLower()}";

                try
                {
                    var re = Db.Context.Reaction.Single(e => e.sbmlId == id);
                    re.Sbase.notes = r.Contains("notes") ? r.notes.Text : "";
                    re.Sbase.sboTerm = r.sboTerm;
                    re.Sbase.annotation = r.Contains("annotation") ? r.annotation.Text : "";
                    Db.Context.SaveChanges();
                    Console.WriteLine(r.id);
                }
                catch (Exception)
                {
                    // ignored
                }

            }

        }

        public static void GeneReg()
        {
            var net = GeneNetwork.LoadGraph("E:\\Dropbox\\Metabolomics\\Data\\ecoli_network_tf_gene.txt");
            Console.WriteLine(net.Genes.Count);
        }

        public static void Main()
        {
            //XmlToDB();
            //GeneReg();
            //Algo();
            GeneRegReport();
            //BooleanExp();
        }

        private static void BooleanExp()
        {
            //var exp = BooleanParser.Parse("(g1 or (g2 and g3)) and (g5 and g6)");
            var p = BooleanParser.Parse("g10 or g2 and g3 or g5 and g6", new ConcurrentDictionary<string, IIntVar>());

            var model = new Cplex();
            var c = model.IfThen(model.Eq(p.RootVar, 0), model.Eq(model.NumVar(0, 100, NumVarType.Float, "r1"), 0));
            Console.WriteLine(p.RootVar);
            Console.ReadKey();
        }

        private static void GeneRegReport()
        {
            //var solver = new CplexSolver();
            //var model1 = new Model();
            //model1.AddVariable(new Variable("r1", 0, 100));
            //var g1 = new Variable("g1", 0, 1, VariableType.Integer);
            //model1.AddVariable(g1);
            //model1.AddConstraint(g1 + 1 <= 1);


            var model = new Cplex();
            var v = new Dictionary<string, INumVar>();
            var g = new ConcurrentDictionary<string, IIntVar>();
            //var Gr = new Dictionary<string, string>();
            var fva = new Dictionary<string, Tuple<float, float>>();

            for (var i = 1; i < 11; i++)
            {
                v[$"r{i}"] = model.NumVar(0, 100, NumVarType.Float, $"r{i}");
                g[$"g{i}"] = model.BoolVar($"g{i}");
                //Gr[$"r{i}"] = $"g{i}";
            }
            //v["r10"].LB = 1;

            #region steady state

            var A = model.LinearNumExpr();
            A.AddTerm(v["r1"], 1);
            A.AddTerm(v["r4"], 1);
            A.AddTerm(v["r2"], -1);
            model.Add(model.Eq(A, 0, "A"));

            var B = model.LinearNumExpr();
            B.AddTerm(v["r2"], 1);
            B.AddTerm(v["r3"], -1);
            B.AddTerm(v["r5"], -1);
            model.Add(model.Eq(B, 0, "B"));

            model.Add(model.Eq(v["r3"], v["r4"], "C"));

            model.Add(model.Eq(v["r6"], v["r7"], "E"));
            model.Add(model.Eq(v["r8"], v["r9"], "F"));

            var D = model.LinearNumExpr();
            D.AddTerm(v["r5"], 1);
            D.AddTerm(v["r6"], -1);
            D.AddTerm(v["r8"], -1);
            model.Add(model.Eq(D, 0, "D"));

            var G = model.LinearNumExpr();
            G.AddTerm(v["r7"], 1);
            G.AddTerm(v["r9"], 1);
            G.AddTerm(v["r10"], -1);
            model.Add(model.Eq(G, 0, "G"));

            #endregion

            for (var i = 1; i < 11; i++)
            {
                if (i == 8) continue;
                //if gi==false then ri=0
                model.Add(model.IfThen(model.Eq(g[$"g{i}"], 0), model.Eq(v[$"r{i}"], 0)));
            }

            //model.Add(model.IfThen(model.Eq(g["g3"], 0), model.Eq(v["r3"], 0)));
            //model.Add(model.IfThen(model.Eq(g["g6"], 0), model.Eq(v["r6"], 0)));

            var p = BooleanParser.Parse("g8 or g7", g);
            model.Add(p.Constraints.ToArray());
            model.Add(model.IfThen(model.Eq(p.RootVar, 0), model.Eq(v["r8"], 0), "Gr8"));

            //model.Add(model.Eq(p.RootVar, 0));

            //model.Add(model.Eq(g["g9"], 0));
            //model.Add(model.Eq(g["g8"], 1));

            model.Add(model.IfThen(model.Eq(g["g10"], 1), model.Eq(g["g2"], 0)));
            model.Add(model.IfThen(model.Eq(g["g2"], 1), model.Eq(g["g8"], 0)));
            model.Add(model.IfThen(model.Eq(g["g8"], 1), model.Eq(g["g6"], 0)));
            model.Add(model.IfThen(model.Eq(g["g6"], 1), model.Eq(g["g4"], 0)));

            model.ExportModel("A:\\model.lp");

            foreach (var r in v)
            {
                var obj = model.AddObjective(ObjectiveSense.Maximize, r.Value);
                var isfeas = model.Solve();

                if (!isfeas) Console.ReadKey();

                var max = (float)model.GetValue(r.Value);
                model.Remove(obj);

                obj = model.AddObjective(ObjectiveSense.Minimize, r.Value);
                isfeas = model.Solve();

                if (!isfeas) Console.ReadKey();

                var min = (float)model.GetValue(r.Value);
                model.Remove(obj);

                fva[r.Key] = Tuple.Create(min, max);
            }

            foreach (var tuple in fva)
            {
                Console.WriteLine("{0} [{1} {2}]", tuple.Key, tuple.Value.Item1, tuple.Value.Item2);
            }

            //model.AddObjective(ObjectiveSense.Maximize, v["r10"]);
            //model.ExportModel("A:\\model.lp");
            //var isfeas = model.Solve();
            //if (isfeas)
            //{
            //    foreach (var r in v.Values)
            //        Console.WriteLine($"{r.Name}: {model.GetValue(r)}");

            //    foreach (var r in g.Values)
            //        try
            //        {
            //            Console.WriteLine($"{r.Name}: {model.GetValue(r)}");

            //        }
            //        catch (Exception)
            //        {
            //            // ignored
            //        }
            //}
            //Console.WriteLine(isfeas);
            Console.ReadKey();
        }

        private static void Algo()
        {
            //Db.InitCache();
            var p = new TheAlgorithm();
            p.Fba.RemoveConstraints = true;
            p.Start();
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("\n************** ITERATION {0} *************** ", p.Iteration);
                p.Step();
                //if (p.Iteration >= 8)
                if (!p.IsFeasable)
                {
                    Console.ReadKey();
                }
            } while (true);
        }

    }
}
