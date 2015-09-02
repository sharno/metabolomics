namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    public class Fba4 : IDisposable
    {
        public bool RemoveConstraints = false;
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, HyperGraph.Edge> RemovedConsumerExchange { get; set; }
        public Dictionary<Guid, HyperGraph.Edge> RemovedProducerExchange { get; set; }
        public Dictionary<string, double> Results { get; set; }
        public Dictionary<string, double> PrevResults { get; set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }

        public Fba4()
        {
            Label = Util.FbaLabel();
            RemovedConsumerExchange = new Dictionary<Guid, HyperGraph.Edge>();
            RemovedProducerExchange = new Dictionary<Guid, HyperGraph.Edge>();
            Results = new Dictionary<string, double>();
            PrevResults = new Dictionary<string, double>();
            UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public bool Solve(ConcurrentDictionary<Guid, int> smz, HyperGraph sm)
        {
            var model = new Cplex { Name = "FBA" };
            var vars = new Dictionary<string, INumVar>();
            foreach (var edge in sm.Edges.Values)
            {
                //if (edge.IsPseudoReaction && UpdateExchangeConstraint.ContainsKey(edge.Id))
                //    vars[edge.Label] = model.NumVar(0.0, Double.MaxValue, NumVarType.Float, edge.Label);
                //else 
                if (edge.IsPseudo)
                    vars[edge.Label] = model.NumVar(1.0, Double.MaxValue, NumVarType.Float, edge.Label);
                else
                    vars[edge.Label] = model.NumVar(0.0, Double.MaxValue, NumVarType.Float, edge.Label);
            }

            model.SetParam(Cplex.Param.RootAlgorithm, Cplex.Algorithm.Concurrent);

            AddReactionConstrait(sm, model, vars);
            var fobj = AddGlobalConstraint(smz, sm, model, vars);
            Console.WriteLine(fobj.ToString());
            model.Objective(ObjectiveSense.Maximize, fobj, "fobj");

            var isfeas = model.Solve();
            model.ExportModel(string.Format("{0}{1}model.lp",Util.Dir,sm.LastLevel));

            //if (isfeas)
            //model.WriteConflict($"{Util.Dir}{sm.LastLevel}conflict.txt");
            //else
            //model.WriteSolution($"{Util.Dir}{sm.LastLevel}result.txt");

            Results.ToList().ForEach(d => PrevResults[d.Key] = d.Value);

            if (isfeas)
                sm.Edges.ToList().ForEach(d => Results[d.Value.Label] = model.GetValue(vars[d.Value.Label]));
            else
                sm.Edges.ToList().ForEach(d => Results[d.Value.Label] = 0);

            var list = sm.Edges.ToList().Select(d => string.Format("{0}:{1}",d.Value.Label,Results[d.Value.Label])).ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines(string.Format("{0}{1}result.txt",Util.Dir,sm.LastLevel), list);

            var status = model.GetCplexStatus();
            Console.WriteLine(status);

            return isfeas;
        }

        private ILinearNumExpr AddGlobalConstraint(ConcurrentDictionary<Guid, int> smz, HyperGraph sm, Cplex model, Dictionary<string, INumVar> vars)
        {
            var fobj = model.LinearNumExpr();
            foreach (var metabolite in sm.Nodes.Values)
            {
                var exp = model.LinearNumExpr();
                foreach (var reaction in metabolite.AllReactions())
                {
                    var coefficient = Coefficient(reaction, metabolite);
                    exp.AddTerm(vars[reaction.Label], coefficient);

                    if (smz.ContainsKey(metabolite.Id))
                    {
                        fobj.AddTerm(vars[reaction.Label], coefficient * smz[metabolite.Id]);
                    }
                }
                model.AddEq(exp, 0.0, metabolite.Label);
            }

            return fobj;
        }

        private void AddReactionConstrait(HyperGraph sm, Cplex model, Dictionary<string, INumVar> vars)
        {
            const double Change = 0;
            var i = 0;
            foreach (var reaction in sm.Edges.Where(reaction => !this.RemoveConstraints).Where(reaction => this.Results.ContainsKey(reaction.Value.Label) && reaction.Value.IsPseudo))
            {
                //model.AddLe(vars[reaction.Value.Label], this.Results[reaction.Value.Label] + Change * this.Results[reaction.Value.Label], $"prev{i}ub");
                model.AddGe(vars[reaction.Value.Label], this.Results[reaction.Value.Label] - Change * this.Results[reaction.Value.Label], string.Format("prev{0}lb",i++));
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

        public void Dispose()
        {
            RemovedConsumerExchange.Clear();
            RemovedProducerExchange.Clear();
            Results.Clear();
            PrevResults.Clear();
        }
    }
}