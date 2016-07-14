using System;
using System.Collections.Generic;
using System.Linq;

using ILOG.Concert;
using ILOG.CPLEX;
using Metabol.DbModels;

namespace Metabol
{
    using System.IO;

    using Metabol.Util;

    class FVA
    {
        public Dictionary<Guid, Tuple<float, float>> Results;

        public FVA()
        {
            Results = new Dictionary<Guid, Tuple<float, float>>();
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


            var obj = model.AddObjective(ObjectiveSense.Maximize, vars["biomass_reaction"]);
            //var f = Model.Solve();
            //var z = 900;//Model.BestObjValue;
            model.Remove(obj);
            //Model.AddGe(vars["biomass_reaction"],1 * z);
            foreach (var edge in graph.Edges.Values)
            {
                Console.Clear();
                Console.SetCursorPosition(0, y);
                obj = model.AddObjective(ObjectiveSense.Maximize, vars[edge.Label]);
                var isfeas = model.Solve();
                //Model.ExportModel(string.Format("{0}{1}Model.lp", Util.Dir, edge.Label));
                //SaveResult(graph, isfeas, Model, vars, edge);
                var max = (float)model.GetValue(vars[edge.Label]);
                model.Remove(obj);
                Console.SetCursorPosition(0, y);
                obj = model.AddObjective(ObjectiveSense.Minimize, vars[edge.Label]);
                isfeas = model.Solve();
                SaveResult(graph, isfeas, model, vars, edge);
                var min = (float)model.GetValue(vars[edge.Label]);
                model.Remove(obj);

                Results[edge.Id] = Tuple.Create(min, max);

                Console.Write("\r{0} reactions   ", count++);
            }
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

            var list = graph.Edges.ToList().Select(d => string.Format("{0}:{1}", d.Value.Label, d.Value.Flux)).ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines(string.Format("{0}{1}result.txt", Core.Dir, edge.Label), list);
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
