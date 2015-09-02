using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using PathwaysLib.ServerObjects;

namespace Metabol
{
    using Common;

    using Microsoft.SolverFoundation.Common;
    using Microsoft.SolverFoundation.Services;

    public class Fba1 : IDisposable
    {
        public bool RemoveConstraints = false;
        public string Label { get; set; }
        public double LastRuntime { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedConsumerExchange { get; set; }
        public Dictionary<Guid, HGraph.Edge> RemovedProducerExchange { get; set; }
        public Dictionary<string, double> Results { get; set; }
        public Dictionary<string, double> PrevResults { get; set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }
        // public HashSet<Guid> IgnoreSet { get; set; }
        public Fba1()
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
            var context = SolverContext.GetContext();

            var model = context.CreateModel();

            var fluxes = AddReactionConstraits(model, sm);

            var fobj = AddMetabolites(model, fluxes, sm, smz);

            model.AddGoal("Fobj", GoalKind.Maximize, fobj.ToTerm());

            var str = new StringBuilder();
            context.SaveModel(FileFormat.OML, new StringWriter(str));
            File.WriteAllText($"{Util.Dir}{sm.LastLevel}model.txt", str.ToString());
            str.Clear();
            //var sim = new SimplexDirective { Arithmetic = Arithmetic.Double, GetSensitivity = false };
            var sim = new InteriorPointMethodDirective();
            var solution = context.Solve(sim);
            var q = solution.Quality;

            Results.ToList().ForEach(d => PrevResults[d.Key] = d.Value);

            if (q == SolverQuality.Feasible || q == SolverQuality.Optimal)
                solution.Decisions.ToList().ForEach(d => Results[d.Name] = d.ToDouble());
            else
                solution.Decisions.ToList().ForEach(d => Results[d.Name] = 0);

            var list = solution.Decisions.Select(d => $"{d.Name}:{Results[d.Name]}").ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines($"{Util.Dir}{sm.LastLevel}result.txt", list);

            var report = solution.GetReport(ReportVerbosity.Decisions);
            Console.WriteLine(report);

            context.ClearModel();
            return q == SolverQuality.Feasible || q == SolverQuality.Optimal;
        }

        private SumTermBuilder AddMetabolites(Model model, Dictionary<Guid, Decision> decisions, HGraph sm, ConcurrentDictionary<Guid, int> smz)
        {
            var fobj = new SumTermBuilder(sm.Edges.Count);
            foreach (var meta in sm.Nodes)
            {
                var sv = new SumTermBuilder(sm.Edges.Count);
                var svin = new SumTermBuilder(sm.Edges.Count);
                var svout = new SumTermBuilder(sm.Edges.Count);
                double ind = 0, outd = 0;

                foreach (var react in sm.Edges)
                {
                    var coefficient = Coefficient(react.Value, meta.Value);

                    if (Math.Abs(coefficient) < double.Epsilon) continue; // coefficient==0

                    sv.Add(coefficient * decisions[react.Key]);
                    if (smz.ContainsKey(meta.Value.Id))
                        fobj.Add(coefficient * smz[meta.Value.Id] * decisions[react.Key]);

                    if (react.Value.InputNodes.ContainsKey(meta.Value.Id)
                        && RemovedConsumerExchange.ContainsKey(meta.Value.Id)
                        && RemovedConsumerExchange[meta.Value.Id].Level < react.Value.Level)
                    {
                        svin.Add(Math.Abs(coefficient) * decisions[react.Key]);
                        ind += Math.Abs(coefficient);
                    }

                    if (react.Value.OutputNodes.ContainsKey(meta.Value.Id)
                        && RemovedProducerExchange.ContainsKey(meta.Value.Id)
                        && RemovedProducerExchange[meta.Value.Id].Level < react.Value.Level)
                    {
                        svout.Add(Math.Abs(coefficient) * decisions[react.Key]);
                        outd += Math.Abs(coefficient);
                    }
                }
                model.AddConstraint(meta.Value.Label, sv.ToTerm() == 0);

                var con = RemovedConsumerExchange.ContainsKey(meta.Value.Id);// && ind < Results[RemovedConsumerExchange[meta.Value.Id].Label];

                var prod = RemovedProducerExchange.ContainsKey(meta.Value.Id);// && outd < Results[RemovedProducerExchange[meta.Value.Id].Label];

                if (RemoveConstraints) continue;

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                if (con)
                    model.AddConstraint($"{meta.Value.Label}_cons",
               svin.ToTerm() == ind * Results[RemovedConsumerExchange[meta.Value.Id].Label]);

                if (prod)
                    model.AddConstraint($"{meta.Value.Label}_prod",
              svout.ToTerm() == outd * Results[RemovedProducerExchange[meta.Value.Id].Label]);
            }
            return fobj;
        }

        private static double Coefficient(HGraph.Edge reaction, HGraph.Node metabolite)
        {
            var coefficient = 0.0;

            if (reaction.OutputNodes.ContainsKey(metabolite.Id))
                coefficient = metabolite.Weight[reaction.Id];//reaction.OutputNodes[metabolite.Id].Stoichiometry;

            if (reaction.InputNodes.ContainsKey(metabolite.Id))
                coefficient = -1 * metabolite.Weight[reaction.Id];//(-1 * reaction.Reactants[metabolite.Id].Stoichiometry);
            return coefficient;
        }

        private Dictionary<Guid, Decision> AddReactionConstraits(Model model, HGraph sm)
        {
            var decisions = new Dictionary<Guid, Decision>(sm.Edges.Count);
            foreach (var reaction in sm.Edges)
                //if (reaction.Value.Reversible)
                //decisions.Add(reaction.Key, new Decision(Domain.RealRange(Rational.NegativeInfinity, Rational.PositiveInfinity), reaction.Value.Label));
                //else
                decisions.Add(reaction.Key, new Decision(Domain.RealRange(1.0, Rational.PositiveInfinity), reaction.Value.Label));

            model.AddDecisions(decisions.Values.ToArray());

            // add constaint for reactions
            var i = 0;
            foreach (var reaction in sm.Edges)
            {
                if (RemoveConstraints) continue; // || IgnoreSet.Contains(reaction.Key)

                if (UpdateExchangeConstraint.ContainsKey(reaction.Key))
                {
                    var sv = new SumTermBuilder(UpdateExchangeConstraint[reaction.Key].Count);
                    var isProducer = (reaction.Value.OutputNodes.Count > 0);
                    var isConsumer = (reaction.Value.InputNodes.Count > 0);

                    if (!(isConsumer ^ isProducer)) throw new Exception("pseudo exchange reaction cannot be producer xor consumer");

                    KeyValuePair<Guid, HGraph.Node> meta;
                    double exchangeStoch;
                    var uc = 0.0;

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
                        sv.Add(c * decisions[r]);
                        ub += c;
                    }
                    sv.Add((exchangeStoch - uc) * decisions[reaction.Key]);

                    model.AddConstraint($"update{i++}", sv.ToTerm() == (exchangeStoch - uc + ub) * Results[reaction.Value.Label]);
                }
                else if (Results.ContainsKey(reaction.Value.Label))
                    model.AddConstraint($"prev{i++}", decisions[reaction.Key] == Results[reaction.Value.Label]);
            }

            return decisions;
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