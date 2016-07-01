using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;

namespace Ecoli
{
    using System.IO;


    class FVA
    {
        public Dictionary<string, Tuple<double, double>> Results;
        public Tuple<int, int> Stat;

        public FVA()
        {
            Results = new Dictionary<string, Tuple<double, double>>();
        }

        public void Solve(HyperGraph graph)
        {
            var model = new Cplex { Name = "FVA" };

            var vars = new Dictionary<string, INumVar>();
            var UpperBound = 1000;
            var LowerBound = -1000;
            foreach (var edge in graph.Edges.Values)
                if (!edge.IsPseudo && edge.IsReversible)
                    vars[edge.Label] = model.NumVar(LowerBound, UpperBound, NumVarType.Float, edge.Label);
                else
                    vars[edge.Label] = model.NumVar(0, UpperBound, NumVarType.Float, edge.Label);

            AddGlobalConstraint(graph, model, vars);
            var count = 0;
            var y = Console.CursorTop + 2;


            //var obj = model.AddObjective(ObjectiveSense.Maximize, vars["biomass_reaction"]);
            //var f = model.Solve();
            //var z = model.BestObjValue;
            //model.Remove(obj);
            //model.AddGe(vars["biomass_reaction"], 0.0*z);
            foreach (var edge in graph.Edges.Values)
            {
                Console.Clear();
                Console.SetCursorPosition(0, y);
                var obj = model.AddObjective(ObjectiveSense.Maximize, vars[edge.Label]);
                var isfeas = model.Solve();
                Debug.Assert(isfeas);
                //model.ExportModel(string.Format("{0}{1}model.lp", Util.Dir, edge.Label));
                //SaveResult(graph, isfeas, model, vars, edge);
                var max = model.GetValue(vars[edge.Label]);
                model.Remove(obj);
                Console.SetCursorPosition(0, y);
                obj = model.AddObjective(ObjectiveSense.Minimize, vars[edge.Label]);
                isfeas = model.Solve();
                Debug.Assert(isfeas);
                //SaveResult(graph, isfeas, model, vars, edge);
                var min = model.GetValue(vars[edge.Label]);
                min = Math.Abs(min) < 0.0000001 ? 0.0 : min;
                max = Math.Abs(max) < 0.0000001 ? 0.0 : max;

                model.Remove(obj);

                Results[edge.Label] = Tuple.Create(min, max);

                Console.Write("\r{0} reactions   ", count++);
            }
            Stat = new Tuple<int, int>(
                    Results.Values.Count(tuple => Math.Abs(tuple.Item1 - tuple.Item2) > 0.00001),
                    Results.Values.Count(tuple => Math.Abs(tuple.Item1) < 0.00001 &&
                                                  Math.Abs(tuple.Item2) < 0.00001));

        }

        public void Solve(Cplex model2, HyperGraph graph, Dictionary<Guid, INumVar>.ValueCollection vars)
        {
            //var filename = "A:\\tmp.lp";
            //model.ExportModel(filename);
            //var model2 = new Cplex();
            //model2.ImportModel(filename);

            //var m = model2.GetLPMatrixEnumerator();
            //m.MoveNext();
            //var mat = (CpxLPMatrix)m.Current;
            //model2.Remove(model2.GetObjective());
            //var vars = mat.GetNumVars();
            //vars = vars.Where(n => n.Type == NumVarType.Float && !n.Name.StartsWith("x")).ToArray();

            //model2.Remove(model2.GetObjective());

            foreach (var v in vars)
            {
                //var fobj = model2.LinearNumExpr();
                //fobj.AddTerm(v, 1);

                model2.Remove(model2.GetObjective());
                model2.AddObjective(ObjectiveSense.Maximize, v, "maxfobj");
                //model2.ExportModel(filename);

                var isfeas = model2.Solve();
                //if (!isfeas) Fba3.Debug(filename, Fba3.ReactionsFluxes(graph));
                //Debug.Assert(isfeas);
                var max = model2.GetValue(v);
                
                model2.Remove(model2.GetObjective());
                model2.AddObjective(ObjectiveSense.Minimize, v, "minfobj");
                //model2.ExportModel(filename);
                isfeas = model2.Solve();
                //if (!isfeas) Fba3.Debug(filename, Fba3.ReactionsFluxes(graph));
                //Debug.Assert(isfeas);

                var min = model2.GetValue(v);

                min = Math.Abs(min) < 0.00001 ? 0.0 : min;
                max = Math.Abs(max) < 0.00001 ? 0.0 : max;
                Results[v.Name] = Tuple.Create(min, max);
                //Console.Write("\r{0} reactions   ", count++);
            }
            var inacitve = Results.Values.Count(tuple => Math.Abs(tuple.Item1) < 0.00001 &&
                                                         Math.Abs(tuple.Item2) < 0.00001);
            Stat = new Tuple<int, int>(Results.Count - inacitve, inacitve);
            Console.WriteLine(Stat);

        }

        public static void SaveResult(HyperGraph graph, bool isfeas, Cplex model, Dictionary<string, INumVar> vars, HyperGraph.Edge edge)
        {
            if (isfeas)
            {
                graph.Edges.ToList().ForEach(d => d.Value.Flux = model.GetValue(vars[d.Value.Label])); //Results[d.Flux.Label]
            }
            else
            {
                graph.Edges.ToList().ForEach(d => d.Value.Flux = 0);
            }

            var list = graph.Edges.ToList().Select(d => $"{d.Value.Label}:{d.Value.Flux}").ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines($"{Core.Dir}{edge.Label}result.txt", list);
        }

        private void AddGlobalConstraint(HyperGraph sm, Cplex model, Dictionary<string, INumVar> vars)
        {
            foreach (var metabolite in sm.Nodes.Values)
            {
                var exp = model.LinearNumExpr();
                foreach (var reaction in metabolite.AllReactions())
                {
                    var coefficient = Coefficient(reaction, metabolite);
                    exp.AddTerm(vars[reaction.Label], coefficient);
                }
                model.AddEq(exp, 0.0, metabolite.Label);
            }
        }

        public double Coefficient(HyperGraph.Edge reaction, HyperGraph.Node metabolite)
        {
            var coefficient = 0.0;

            if (reaction.Products.ContainsKey(metabolite.Id))
            {
                coefficient = metabolite.Weights[reaction.Id];//reaction.Products[metabolite.Id].Stoichiometry;
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsPseudo)
                {
                    coefficient = 1;
                }
            }

            if (reaction.Reactants.ContainsKey(metabolite.Id))
            {
                coefficient = -1 * metabolite.Weights[reaction.Id];//(-1 * reaction.Reactants[metabolite.Id].Stoichiometry);
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsPseudo)
                {
                    coefficient = -1;
                }
            }
            //coefficient = Math.Round(coefficient, 2);
            return coefficient;
        }
    }
}
