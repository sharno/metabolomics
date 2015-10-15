﻿namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    using Exception = System.Exception;

    public class Fba3 : IDisposable
    {
        public bool RemoveConstraints = false;
        //public string Label { get; set; }
        public double LastRuntime { get; set; }
        //public Dictionary<Guid, HyperGraph.Edge> RemovedConsumerExchange { get; set; }
        //public Dictionary<Guid, HyperGraph.Edge> RemovedProducerExchange { get; set; }
        //public Dictionary<string, double> Results { get; set; }
        //public Dictionary<string, double> PrevResults { get; set; }
        //public ConcurrentDictionary<Guid, HashSet<Guid>> UpdateExchangeConstraint { get; set; }
        const double Change = 0.01;
        private Dictionary<string, double> BlockedReactions { get; set; }

        public Fba3()
        {
            //Label = Util.FbaLabel();
            //RemovedConsumerExchange = new Dictionary<Guid, HyperGraph.Edge>();
            //RemovedProducerExchange = new Dictionary<Guid, HyperGraph.Edge>();
            //Results = new Dictionary<string, double>();
            //PrevResults = new Dictionary<string, double>();
            //UpdateExchangeConstraint = new ConcurrentDictionary<Guid, HashSet<Guid>>();
            BlockedReactions = new Dictionary<string, double>();
            //BlockedReactions.Add(Guid.Parse("8af5e2d3-6257-4d9c-ac8f-7f7296d6ab5c"));
            //BlockedReactions.Add(Guid.Parse("35d76d03-46ad-414e-8ca3-cce213a6c71d"));
            foreach (var split in File.ReadAllLines(Util.BlockedReactionsFile).Select(line => line.Split(';')))
            {
                this.BlockedReactions[split[0]] = double.Parse(split[1]);
            }
        }

        public bool Solve(ConcurrentDictionary<Guid, int> smz, HyperGraph sm)
        {
            var model = new Cplex { Name = "FBA" };
            var vars = new Dictionary<string, INumVar>();
            var UpperBound = 1000;
            var LowerBound = -1000;

            foreach (var edge in sm.Edges.Values)
            {
                if (!edge.IsPseudo && edge.ToServerReaction.Reversible)
                {
                    vars[edge.Label] = model.NumVar(LowerBound, UpperBound, NumVarType.Float, edge.Label);
                    continue;
                }
                vars[edge.Label] = model.NumVar(0.0, UpperBound, NumVarType.Float, edge.Label);
            }

            //model.SetParam(Cplex.Param.RootAlgorithm, Cplex.Algorithm.Concurrent);

            AddReactionConstrait(sm, model, vars);
            var fobj = AddGlobalConstraint(smz, sm, model, vars);
            Console.WriteLine(fobj.ToString());
            model.AddObjective(ObjectiveSense.Maximize, fobj, "fobj");

            var isfeas = model.Solve();
            model.ExportModel(string.Format("{0}{1}model.lp", Util.Dir, sm.LastLevel));

            //if (isfeas)
            //model.WriteConflict($"{Util.Dir}{sm.LastLevel}conflict.txt");
            //else
            //model.WriteSolution($"{Util.Dir}{sm.LastLevel}result.txt");
            sm.Edges.Values.ToList().ForEach(e => e.PreFlux = e.Flux);
            //Results.ToList().ForEach(d => PrevResults[d.Key] = d.Flux);

            if (isfeas)
                sm.Edges.ToList().ForEach(d => d.Value.Flux = model.GetValue(vars[d.Value.Label]));//Results[d.Flux.Label]
            else
                sm.Edges.ToList().ForEach(d => d.Value.Flux = 0);

            var list = sm.Edges.ToList().Select(d => string.Format("{0}:{1}", d.Value.Label, d.Value.Flux)).ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines(string.Format("{0}{1}result.txt", Util.Dir, sm.LastLevel), list);

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
                var consexp = model.LinearNumExpr();
                var prodexp = model.LinearNumExpr();
                double consumedCoeff = 0, producedCoeff = 0;
                var consumedCount = 0;
                var producedCount = 0;
                var skip = true;
                foreach (var reaction in metabolite.AllReactions())
                {
                    skip = BlockedReactions.ContainsKey(reaction.Label);
                    var coefficient = Coefficient(reaction, metabolite);
                    exp.AddTerm(vars[reaction.Label], coefficient);

                    if (smz.ContainsKey(metabolite.Id))
                    {
                        fobj.AddTerm(vars[reaction.Label], coefficient * smz[metabolite.Id]);
                    }

                    //if (reaction.Reactants.ContainsKey(metabolite.Id)
                    //    && metabolite.RemovedConsumerExchange != null
                    //    && metabolite.RemovedConsumerExchange.Level < reaction.Level)
                    //&& RemovedConsumerExchange.ContainsKey(metabolite.Id)
                    //&& RemovedConsumerExchange[metabolite.Id].Level < reaction.Level)
                    if (reaction.Reactants.ContainsKey(metabolite.Id)
                        && !BlockedReactions.ContainsKey(reaction.Label))
                    {
                        consexp.AddTerm(vars[reaction.Label], Math.Abs(coefficient));
                        consumedCoeff += Math.Abs(coefficient);
                        consumedCount++;
                    }

                    //if (reaction.Products.ContainsKey(metabolite.Id)
                    //    && metabolite.RemovedProducerExchange != null
                    //    && metabolite.RemovedProducerExchange.Level < reaction.Level)
                    //&& RemovedProducerExchange.ContainsKey(metabolite.Id)
                    //&& RemovedProducerExchange[metabolite.Id].Level < reaction.Level)
                    if (reaction.Products.ContainsKey(metabolite.Id)
                        && !BlockedReactions.ContainsKey(reaction.Label))
                    {
                        prodexp.AddTerm(vars[reaction.Label], Math.Abs(coefficient));
                        producedCoeff += Math.Abs(coefficient);
                        producedCount++;
                    }
                }
                model.AddEq(exp, 0.0, metabolite.Label);

                if (RemoveConstraints)
                {
                    continue;
                }

                if (smz.ContainsKey(metabolite.Id) && !skip)
                {
                    if (smz[metabolite.Id] < 0) model.AddGe(consexp, 1, string.Format("{0}_nonzeroc", metabolite.Label));
                    if (smz[metabolite.Id] > 0) model.AddGe(prodexp, 1, string.Format("{0}_nonzerop", metabolite.Label));
                }

                //var con = RemovedConsumerExchange.ContainsKey(metabolite.Id);
                ////&& ind < Results[RemovedConsumerExchange[metabolite.Id].Label];

                //var prod = RemovedProducerExchange.ContainsKey(metabolite.Id);
                ////&& outd < Results[RemovedProducerExchange[metabolite.Id].Label];

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                if (metabolite.RemovedConsumerExchange != null && consumedCount > 0)
                {
                    model.AddGe(
                        consexp,
                        0,//consumedCoeff * metabolite.RemovedConsumerExchange.Flux,
                        string.Format("{0}_cons", metabolite.Label));
                    //model.AddRange(
                    //    consumedCoeff * Results[RemovedConsumerExchange[metabolite.Id].Label]
                    //    - 0.1 * Results[RemovedConsumerExchange[metabolite.Id].Label],
                    //    consexp,
                    //    consumedCoeff * Results[RemovedConsumerExchange[metabolite.Id].Label]
                    //    + 0.1 * Results[RemovedConsumerExchange[metabolite.Id].Label],
                    //    $"{metabolite.Label}_cons");
                }

                if (metabolite.RemovedProducerExchange != null && producedCount > 0)
                {
                    model.AddGe(
                        prodexp,
                        0,//producedCoeff * metabolite.RemovedProducerExchange.Flux,
                        string.Format("{0}_prod", metabolite.Label));
                    //model.AddRange(
                    //  producedCoeff * Results[RemovedProducerExchange[metabolite.Id].Label]
                    //  - 0.1 * Results[RemovedProducerExchange[metabolite.Id].Label],
                    //  prodexp,
                    //  producedCoeff * Results[RemovedProducerExchange[metabolite.Id].Label]
                    //  + 0.1 * Results[RemovedProducerExchange[metabolite.Id].Label],
                    //   $"{metabolite.Label}_prod");
                }
            }

            return fobj;
        }

        private void AddReactionConstrait(HyperGraph sm, Cplex model, Dictionary<string, INumVar> vars)
        {
            var i = 0;
            foreach (var reaction in sm.Edges)
            {
                if (BlockedReactions.ContainsKey(reaction.Value.Label))
                {
                    model.AddEq(vars[reaction.Value.Label], BlockedReactions[reaction.Value.Label]);
                    continue;
                }

                if (RemoveConstraints) continue;
                //if (UpdateExchangeConstraint.ContainsKey(reaction.Key))
                if (reaction.Value.UpdatePseudo.Count != 0)
                {
                    if (Math.Abs(reaction.Value.Flux) < 0.00001) continue;
                    var sv = model.LinearNumExpr();
                    var isProducer = (reaction.Value.Products.Count > 0);
                    var isConsumer = (reaction.Value.Reactants.Count > 0);

                    //if (!(isConsumer ^ isProducer))
                    //{
                    //    throw new Exception("pseudo exchange reaction cannot be producer xor consumer");
                    //}

                    KeyValuePair<Guid, HyperGraph.Node> meta;
                    double exchangeStoch;
                    double uc;
                    if (isProducer)
                    {
                        meta = reaction.Value.Products.First();
                        exchangeStoch = Util.GetReactionCount(meta.Key).Producers;
                        uc =
                            meta.Value.Producers.Where(e => !e.IsPseudo)
                                .Sum(e => Math.Abs(Coefficient(e, meta.Value)));
                    }
                    else
                    {
                        meta = reaction.Value.Reactants.First();
                        exchangeStoch = Util.GetReactionCount(meta.Key).Consumers;
                        uc =
                         meta.Value.Consumers.Where(e => !e.IsPseudo)
                             .Sum(e => Math.Abs(Coefficient(e, meta.Value)));
                    }

                    var ub = 0.0;
                    foreach (var r in reaction.Value.UpdatePseudo) //UpdateExchangeConstraint[reaction.Key]
                    {
                        var c = Math.Abs(Coefficient(sm.Edges[r], meta.Value));
                        sv.AddTerm(vars[sm.Edges[r].Label], c);
                        ub += c;
                    }
                    sv.AddTerm(vars[reaction.Value.Label], (exchangeStoch - uc));
                    model.AddGe(sv, reaction.Value.Flux * (1 - Change) * (exchangeStoch - uc + ub), string.Format("update{0}lb", i));
                    model.AddLe(sv, reaction.Value.Flux * (1 + Change) * (exchangeStoch - uc + ub), string.Format("update{0}ub", i++));

                }
                //else if (Results.ContainsKey(reaction.Flux.Label) && Math.Abs(this.Results[reaction.Flux.Label]) > double.Epsilon)  //&& !BlockedReactions.Contains(reaction.Key)
                else if (reaction.Value.PreFlux > 0 && reaction.Value.Flux > 0)
                {
                    //if (reaction.Value.PreFlux < 0) continue;
                    //model.AddGe(vars[reaction.Flux.Label], Results[reaction.Flux.Label], $"prev{i++}");
                    //model.AddRange(
                    //    Results[reaction.Flux.Label] - Change * Results[reaction.Flux.Label],
                    //    vars[reaction.Flux.Label],
                    //    Results[reaction.Flux.Label] + Change * Results[reaction.Flux.Label],
                    //    $"prev{i++}");

                    model.AddLe(vars[reaction.Value.Label], reaction.Value.Flux * (1 + Change), string.Format("prev{0}ub", i));
                    model.AddGe(vars[reaction.Value.Label], reaction.Value.Flux * (1 - Change), string.Format("prev{0}lb", i++));
                }
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
            //RemovedConsumerExchange.Clear();
            //RemovedProducerExchange.Clear();
            //Results.Clear();
            //PrevResults.Clear();
        }
    }
}