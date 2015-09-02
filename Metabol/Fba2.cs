using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;

using Optimization;
using Optimization.Exporter;
using Optimization.Solver;
using Optimization.Solver.GLPK;

namespace Metabol
{
    using PathwaysLib.ServerObjects;

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

        public ServerSpecies M { get; set; }

        // public HashSet<Guid> IgnoreSet { get; set; }
        public Fba2()
        {
            Label = Util.FbaLabel();
            RemovedConsumerExchange = new Dictionary<Guid, HGraph.Edge>();
            RemovedProducerExchange = new Dictionary<Guid, HGraph.Edge>();
            Results = new Dictionary<string, double>();
            PrevResults = new Dictionary<string, double>();
            UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
            // IgnoreSet = new HashSet<Guid>();
        }

        public bool Solve(ConcurrentDictionary<Guid, int> smz, HGraph sm)
        {
            var solver = new GLPKSolver();
            var model = new Model { Name = "FBA" };

            foreach (var reaction in sm.Edges)
                //if (reaction.Value.Reversible)
                //  model.AddVariable(reaction.Value.Name, -1000, 1000, VariableType.Continuous);
                //else
                model.AddVariable(reaction.Value.Label, 1.0, double.PositiveInfinity, VariableType.Integer);

            AddReactionConstrait(sm, model);

            var fobj = CreateObjective(smz, sm, model);

            model.AddObjective(fobj, "Fobj", ObjectiveSense.Maximize);
            var solution = solver.Solve(model);
            Results.ToList().ForEach(d => PrevResults[d.Key] = d.Value);
            List<string> list;

            if (solution.ModelStatus == ModelStatus.Feasible)
            {
                solution.VariableValues.ToList().ForEach(d => Results[d.Key] = d.Value);
                list = solution.VariableValues.ToList().Select(d => $"{d.Key}:{d.Value}").ToList();
            }
            else
            {
                model.Variables.ToList().ForEach(d => Results[d.Name] = 0);
                list = model.Variables.ToList().Select(d => $"{d.Name}:{d.Value}").ToList();
            }


            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines($"{Util.Dir}{sm.LastLevel}result.txt", list);

            using (var fs = File.Create($"{Util.Dir}{sm.LastLevel}model.txt"))
                model.Write(fs, FileType.LP);

            return solution.ModelStatus == ModelStatus.Feasible;
        }

        private void AddReactionConstrait(HGraph sm, Model model)
        {
            var i = 0;
            foreach (var reaction in sm.Edges)
            {
                if (RemoveConstraints) continue; //|| IgnoreSet.Contains(reaction.Key)

                if (UpdateExchangeConstraint.ContainsKey(reaction.Key))
                {
                    var sv = Expression.Sum(new[] { 0.0 });
                    var isProducer = (reaction.Value.OutputNodes.Count > 0);
                    var isConsumer = (reaction.Value.InputNodes.Count > 0);

                    if (!(isConsumer ^ isProducer))
                    {
                        throw new Exception("pseudo exchange reaction cannot be producer xor consumer");
                    }

                    KeyValuePair<Guid, HGraph.Node> meta;
                    double exchangeStoch;
                    double uc;
                    if (isProducer)
                    {
                        meta = reaction.Value.OutputNodes.First();
                        exchangeStoch = Util.AllStoichiometryCache[meta.Key].Item2;
                        uc =
                            meta.Value.OutputFromEdge.Where(e => !e.IsImaginary)
                                .Sum(e => Math.Abs(Coefficient(e, meta.Value)));

                    }
                    else
                    {
                        meta = reaction.Value.InputNodes.First();
                        exchangeStoch = Util.AllStoichiometryCache[meta.Key].Item1;
                        uc =
                         meta.Value.InputToEdge.Where(e => !e.IsImaginary)
                             .Sum(e => Math.Abs(Coefficient(e, meta.Value)));
                    }

                    var ub = 0.0;
                    foreach (var r in UpdateExchangeConstraint[reaction.Key])
                    {
                        var c = Math.Abs(Coefficient(sm.Edges[r], meta.Value));
                        sv = sv + new Term(model.GetVariable(sm.Edges[r].Label), c);
                        ub += c;
                    }
                    sv = sv + new Term(model.GetVariable(reaction.Value.Label), (exchangeStoch - uc));

                    model.AddConstraint(sv <= (exchangeStoch - uc + ub) * Results[reaction.Value.Label] + 1, $"update1{i}");
                    model.AddConstraint(sv >= (exchangeStoch - uc + ub) * Results[reaction.Value.Label] - 1, $"update2{i++}");

                }
                else if (Results.ContainsKey(reaction.Value.Label)) //&& !reaction.Value.ToServerReaction.Reversible
                {
                    model.AddConstraint(
                        new Term(model.GetVariable(reaction.Value.Label)) == Results[reaction.Value.Label],
                        $"prev{i++}");
                }
            }
        }

        private Expression CreateObjective(ConcurrentDictionary<Guid, int> smz, HGraph sm, Model model)
        {
            var fobj = Expression.Sum(new[] { 0.0 });
            foreach (var metabolite in sm.Nodes.Values)
            {
                var sv = Expression.Sum(new[] { 0.0 });
                var svin = Expression.Sum(new[] { 0.0 });
                var svout = Expression.Sum(new[] { 0.0 });
                double ind = 0, outd = 0;
                //int v = 1;
                //if (metabolite.Label == M.SbmlId) v = -1;
                //else v = 1;
                foreach (var reaction in metabolite.AllReactions())
                {
                    var coefficient = Coefficient(reaction, metabolite);

                    //if (Math.Abs(coefficient) < double.Epsilon)
                    //    continue; // coefficient==0

                    sv = sv + new Term(model.GetVariable(reaction.Label), coefficient);

                    if (smz.ContainsKey(metabolite.Id))
                        fobj = fobj + new Term(model.GetVariable(reaction.Label), coefficient * smz[metabolite.Id]);

                    if (reaction.InputNodes.ContainsKey(metabolite.Id) && RemovedConsumerExchange.ContainsKey(metabolite.Id)
                        && RemovedConsumerExchange[metabolite.Id].Level < reaction.Level)
                    {
                        svin = svin + new Term(model.GetVariable(reaction.Label), Math.Abs(coefficient));
                        ind += Math.Abs(coefficient);
                    }

                    if (reaction.OutputNodes.ContainsKey(metabolite.Id) && RemovedProducerExchange.ContainsKey(metabolite.Id)
                        && RemovedProducerExchange[metabolite.Id].Level < reaction.Level)
                    {
                        svout = svout + new Term(model.GetVariable(reaction.Label), Math.Abs(coefficient));
                        outd += Math.Abs(coefficient);
                    }
                }

                model.AddConstraint(sv == 0, metabolite.Label); //0.00000000001

                var con = RemovedConsumerExchange.ContainsKey(metabolite.Id);
                //&& ind < Results[RemovedConsumerExchange[metabolite.Id].Label];

                var prod = RemovedProducerExchange.ContainsKey(metabolite.Id);
                //&& outd < Results[RemovedProducerExchange[metabolite.Id].Label];

                if (RemoveConstraints) //|| !con || !prod
                    continue;

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                if (con) model.AddConstraint(svin == ind * Results[RemovedConsumerExchange[metabolite.Id].Label], $"{metabolite.Label}_cons");

                if (prod) model.AddConstraint(svout == outd * Results[RemovedProducerExchange[metabolite.Id].Label], $"{metabolite.Label}_prod");
            }
            //if (sm.LastLevel > 0) return fobj;
            //var sv1 = Expression.Sum(new[] { 0.0 });
            //foreach (var edge in sm.Nodes[M.ID].AllReactions())
            //{
            //    var coefficient = Coefficient(edge, sm.Nodes[M.ID]);

            //    //if (Math.Abs(coefficient) < double.Epsilon)
            //    //    continue; // coefficient==0

            //    sv1 = sv1 + new Term(model.GetVariable(edge.Label), Math.Abs(coefficient));
            //}
            //model.AddConstraint(sv1 >= 1000, sm.Nodes[M.ID].Label+"_2");
            return fobj;
        }

        public double Coefficient(HGraph.Edge reaction, HGraph.Node metabolite)
        {
            var coefficient = 0.0;

            if (reaction.OutputNodes.ContainsKey(metabolite.Id))
            {
                coefficient = metabolite.Weight[reaction.Id];//reaction.OutputNodes[metabolite.Id].Stoichiometry;
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsImaginary)
                {
                    coefficient = 1;
                }
            }

            if (reaction.InputNodes.ContainsKey(metabolite.Id))
            {
                coefficient = -1 * metabolite.Weight[reaction.Id];//(-1 * reaction.Reactants[metabolite.Id].Stoichiometry);
                if (Math.Abs(coefficient) < double.Epsilon && reaction.IsImaginary)
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