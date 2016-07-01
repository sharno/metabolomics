using Metabol.DbModels;
using Db = Metabol.DbModels.Db;

namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    using Util;

    public class FbaMoma
    {
        public bool RemoveConstraints = false;
        public int Iteration;
        public double LastRuntime { get; set; }
        private const double Change = 0.01;
        private readonly SortedDictionary<string, double> preValues = new SortedDictionary<string, double>();

        public bool Solve(ConcurrentDictionary<Guid, int> smz, HyperGraph sm)
        {
            var model = new Cplex { Name = "FBA" };
            var vars = new SortedDictionary<string, INumVar>();
            //var bvars = new List<IIntVar>();
            //Model.Add(Model.IfThen(Model.Eq(bvars[1],1), Model.Eq(bvars[2],0)));

            foreach (var edge in sm.Edges.Values)
            {
                if (edge.IsPseudo)
                {
                    vars[edge.Label] = model.NumVar(0.0, 1000, NumVarType.Float, edge.Label);
                    continue;
                }
                var lb = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).lowerBound;
                var ub = Db.Context.ReactionBound.Single(e => e.reactionId == edge.Id).upperBound;

                if (lb == 0 && ub == 0 || lb == -1000 && ub == 0 || edge.IsReversible || edge.Label.StartsWith("r") ||
                    edge.Label.StartsWith("RE") || edge.Label.StartsWith("ID") || edge.Label.StartsWith("THYPX") ||
                    edge.Label.StartsWith("DHE") || edge.Label.StartsWith("STS") || edge.Label.StartsWith("UMP") ||
                    edge.Label.StartsWith("EX") || edge.Label.StartsWith("P") || edge.Label.StartsWith("AK"))
                    vars[edge.Label] = model.NumVar(lb, ub, NumVarType.Float, edge.Label);
                else
                    vars[edge.Label] = model.NumVar(1, ub, NumVarType.Float, edge.Label);
            }

            model.SetParam(Cplex.Param.RootAlgorithm, Cplex.Algorithm.Concurrent);

            AddReactionConstrait(sm, model, vars);
            var fobj = AddGlobalConstraint(smz, sm, model, vars);

            //Console.WriteLine(fobj.ToString());

            model.AddObjective(ObjectiveSense.Minimize, model.Prod(fobj, -1), "fobj");
            var status = false;

            if (Iteration > 0)
            {
                var startVar = preValues.Keys.Where(e => vars.ContainsKey(e)).Select(e => vars[e]).ToArray();
                var startVal = new double[startVar.Length];

                for (var i = 0; i < startVal.Length; i++)
                    startVal[i] = preValues[startVar[i].Name];

                model.SetStart(startVal, null, startVar, null, null, null);
            }

            status = model.Solve();
            //Model.ExportModel($"{Core.Dir}{sm.LastLevel}Model.lp");

            if (status)
                sm.Edges.ToList().ForEach(d => d.Value.Flux = model.GetValue(vars[d.Value.Label]));
            else
                sm.Edges.ToList().ForEach(d => d.Value.Flux = 0);

            if (Iteration > 0 && !status)
            {
                status = MOMA.Calculate(sm, preValues, model.Prod(fobj, -1));
                //var newValues = new SortedDictionary<string, double>();
                //sm.Edges.ToList().ForEach(e => newValues[e.Value.Label] = e.Value.Flux);

                //Console.WriteLine();
            }

            sm.Edges.ToList().ForEach(e => preValues[e.Value.Label] = e.Value.Flux);
            sm.Edges.ToList().ForEach(e => e.Value.PreFlux = e.Value.Flux);

            //var list = sm.Edges.ToList().Select(d => string.Format("{0}:{1}", d.Value.Label, d.Value.Flux)).ToList();
            //list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            //File.WriteAllLines(string.Format("{0}{1}result.txt", Core.Dir, sm.LastLevel), list);

            Console.WriteLine(status);
            return status;
        }

        private ILinearNumExpr AddGlobalConstraint(IDictionary<Guid, int> smz, HyperGraph sm, Cplex model, IDictionary<string, INumVar> vars)
        {
            var fobj = model.LinearNumExpr();
            //var w = 0.25;
            foreach (var metabolite in sm.Nodes.Values)
            {
                var exp = model.LinearNumExpr();
                var consexp = model.LinearNumExpr();
                var prodexp = model.LinearNumExpr();
                foreach (var reaction in metabolite.AllReactions())
                {
                    var coefficient = Coefficient(reaction, metabolite);
                    exp.AddTerm(vars[reaction.Label], coefficient);

                    if (smz.ContainsKey(metabolite.Id))
                        fobj.AddTerm(vars[reaction.Label], coefficient * smz[metabolite.Id]);

                    //if (preValues.Count != 0 && preValues.ContainsKey(reaction.Label) && !reaction.IsPseudo)
                    //    fobj.AddTerm(vars[reaction.Label], (1 - w) * preValues[reaction.Label]);

                    if (reaction.Reactants.ContainsKey(metabolite.Id))
                        consexp.AddTerm(vars[reaction.Label], Math.Abs(coefficient));

                    if (reaction.Products.ContainsKey(metabolite.Id))
                        prodexp.AddTerm(vars[reaction.Label], Math.Abs(coefficient));
                }
                model.AddEq(exp, 0.0, metabolite.Label);
            }

            return fobj;
        }

        private void AddReactionConstrait(HyperGraph sm, Cplex model, IDictionary<string, INumVar> vars)
        {
            if (Iteration == 0) return;
            var i = 0;
            foreach (var reaction in sm.Edges)
            {
                if (Math.Abs(reaction.Value.Flux) < double.Epsilon)
                {
                    //Model.AddEq(vars[reaction.Value.Label], 0, string.Format("prev{0}eq", i++));
                    continue;
                }
                model.AddLe(vars[reaction.Value.Label], reaction.Value.Flux * (1 + Change), string.Format("prev{0}ub", i));
                model.AddGe(vars[reaction.Value.Label], reaction.Value.Flux * (1 - Change), string.Format("prev{0}lb", i++));
            }
        }

        public static double Coefficient(HyperGraph.Edge reaction, HyperGraph.Node metabolite)
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