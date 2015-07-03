namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Optimization;
    using Optimization.Exporter;
    using Optimization.Solver;
    using Optimization.Solver.GLPK;

    public class Fba2 : IDisposable
    {
        public bool RemoveConstraints = false;
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedConsumerExchange { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedProducerExchange { get; set; }
        public Dictionary<string, double> Results { get; set; }
        public Dictionary<string, double> PrevResults { get; set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }
        public HashSet<Guid> IgnoreSet { get; set; }

        public Fba2()
        {
            Label = Util.FbaLabel();
            RemovedConsumerExchange = new Dictionary<Guid, HGraph.Edge>();
            RemovedProducerExchange = new Dictionary<Guid, HGraph.Edge>();
            Results = new Dictionary<string, double>();
            PrevResults = new Dictionary<string, double>();
            UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
            IgnoreSet = new HashSet<Guid>();
        }

        public bool Solve(Dictionary<Guid, Reaction> reactions, Dictionary<Guid, int> smz, HGraph sm)
        {
            var solver = new GLPKSolver();
            var model = new Model { Name = "FBA" };

            foreach (var reaction in reactions)
                //if (!reaction.Value.Reversible)
                model.AddVariable(reaction.Value.Name, 1.0, 1000, VariableType.Continuous);
            //else
            //  model.AddVariable(reaction.Value.Name, -1000, 1000, VariableType.Continuous);

            var metabolites = new SortedSet<Metabolite>();

            foreach (var reaction in reactions.Values)
            {
                metabolites.UnionWith(reaction.Products.Select(s => s.Value.Metabolite));
                metabolites.UnionWith(reaction.Reactants.Select(s => s.Value.Metabolite));
            }

            var i = 0;
            foreach (var reaction in reactions)
            {
                if (RemoveConstraints || IgnoreSet.Contains(reaction.Key)) continue;

                var ctx = Results.ContainsKey(reaction.Value.Name) ? Results[reaction.Value.Name] : 0;
                if (Results.ContainsKey(reaction.Value.Name) && !reaction.Value.Reversible)
                    model.AddConstraint(new Term(model.GetVariable(reaction.Value.Name)) == ctx, $"prev{i++}");
            }

            var fobj = CreateObjective(reactions, smz, metabolites, model);

            model.AddObjective(fobj, "Fobj", ObjectiveSense.Maximize);
            var solution = solver.Solve(model);
            Results.ToList().ForEach(d => PrevResults[d.Key] = d.Value);

            if (solution.ModelStatus == ModelStatus.Feasible)
                solution.VariableValues.ToList().ForEach(d => Results[d.Key] = d.Value);
            else
                model.Variables.ToList().ForEach(d => Results[d.Name] = 0);

            var list = Results.Select(d => $"{d.Key}:{d.Value}").ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines($"{Util.Dir}{sm.LastLevel}result.txt", list);

            model.Write(File.Create($"{Util.Dir}{sm.LastLevel}model.txt"), FileType.LP);

            return solution.ModelStatus == ModelStatus.Feasible;
        }

        private Expression CreateObjective(Dictionary<Guid, Reaction> reactions, Dictionary<Guid, int> smz, SortedSet<Metabolite> metabolites, Model model)
        {
            var fobj = Expression.Sum(new[] { 0.0 });
            foreach (var metabolite in metabolites)
            {
                var sv = Expression.Sum(new[] { 0.0 });
                var svin = Expression.Sum(new[] { 0.0 });
                var svout = Expression.Sum(new[] { 0.0 });
                double ind = 0, outd = 0;
                foreach (var react in reactions)
                {
                    var coefficient = Coefficient(react.Value, metabolite);

                    if (Math.Abs(coefficient) < double.Epsilon)
                        continue; // coefficient==0

                    sv = sv + new Term(model.GetVariable(react.Value.Name), coefficient);

                    if (smz.ContainsKey(metabolite.Id))
                        fobj = fobj + new Term(model.GetVariable(react.Value.Name), coefficient * smz[metabolite.Id]);

                    if (react.Value.Reactants.ContainsKey(metabolite.Id) && RemovedConsumerExchange.ContainsKey(metabolite.Id)
                        && RemovedConsumerExchange[metabolite.Id].Level < react.Value.Level)
                    {
                        svin = svin + new Term(model.GetVariable(react.Value.Name), Math.Abs(coefficient));
                        ind += Math.Abs(coefficient);
                    }

                    if (react.Value.Products.ContainsKey(metabolite.Id) && RemovedProducerExchange.ContainsKey(metabolite.Id)
                        && RemovedProducerExchange[metabolite.Id].Level < react.Value.Level)
                    {
                        svout = svout + new Term(model.GetVariable(react.Value.Name), Math.Abs(coefficient));
                        outd += Math.Abs(coefficient);
                    }
                }

                model.AddConstraint(sv == 0, metabolite.Name);

                var con = RemovedConsumerExchange.ContainsKey(metabolite.Id)
                          && ind < Results[RemovedConsumerExchange[metabolite.Id].Label];

                var prod = RemovedProducerExchange.ContainsKey(metabolite.Id)
                           && outd < Results[RemovedProducerExchange[metabolite.Id].Label];

                if (RemoveConstraints || !con || !prod)
                    continue;

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                model.AddConstraint(svin == Results[RemovedConsumerExchange[metabolite.Id].Label], $"{metabolite.Name}_consumer");

                model.AddConstraint(svout == Results[RemovedProducerExchange[metabolite.Id].Label], $"{metabolite.Name}_producer");
            }
            return fobj;
        }

        private static double Coefficient(Reaction reaction, Metabolite metabolite1)
        {
            var coefficient = 0.0;
            if (reaction.Products.ContainsKey(metabolite1.Id))
                coefficient = reaction.Products[metabolite1.Id].Stoichiometry;

            if (reaction.Reactants.ContainsKey(metabolite1.Id))
                coefficient = (-1 * reaction.Reactants[metabolite1.Id].Stoichiometry);
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