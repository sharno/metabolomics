namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    public class Fba3N : IDisposable
    {
        public bool RemoveConstraints = false;
        public double LastRuntime { get; set; }
        const double Change = 0.1;

        public bool Solve(ConcurrentDictionary<Guid, int> smz, HyperGraph sm)
        {
            var model = new Cplex { Name = "FBA" };
            model.SetParam(Cplex.Param.RootAlgorithm, Cplex.Algorithm.Concurrent);

            var vars = new Dictionary<string, INumVar>();
            var UpperBound = double.PositiveInfinity;

            foreach (var edge in sm.Edges.Values)
            {
                if (!edge.IsPseudo && edge.ToServerReaction.Reversible)
                {
                    vars[edge.Label] = model.NumVar(Double.NegativeInfinity, Double.PositiveInfinity, NumVarType.Float, edge.Label);
                    continue;
                }

                //foreach (var n in edge.AllNodes())
                //{
                //    // if (RemovedConsumerExchange.ContainsKey(n.Id))
                //    if (n.RemovedConsumerExchange != null)
                //        foreach (var cons in n.Consumers)
                //            vars[sm.Edges[cons.Id].Label] = model.NumVar(0.0, UpperBound, NumVarType.Float, sm.Edges[cons.Id].Label);

                //    //if (RemovedProducerExchange.ContainsKey(n.Id))
                //    if (n.RemovedProducerExchange != null)
                //        foreach (var prod in n.Producers)
                //            vars[sm.Edges[prod.Id].Label] = model.NumVar(0.0, UpperBound, NumVarType.Float, sm.Edges[prod.Id].Label);
                //}

                //if (edge.UpdatePseudo.Count != 0 && !vars.ContainsKey(edge.Label))
                //{
                //    vars[edge.Label] = model.NumVar(0.0, UpperBound, NumVarType.Float, edge.Label);
                //    foreach (var guid in edge.AllNodes().SelectMany(e => e.AllReactions()).Select(e => e.Id))
                //    {
                //        vars[sm.Edges[guid].Label] = model.NumVar(0.0, UpperBound, NumVarType.Float, sm.Edges[guid].Label);
                //    }
                //}
                //else if (edge.PreValue != -1 && !vars.ContainsKey(edge.Label))//Results.ContainsKey(edge.Label)
                //{
                //    vars[edge.Label] = model.NumVar(edge.Flux * (1 - Change), UpperBound, NumVarType.Float, edge.Label);//Results[edge.Label]
                //}
                ////else if (UpdateExchangeConstraint.ContainsKey(edge.Id))
                //else if (!vars.ContainsKey(edge.Label))

                //foreach (var key in smz.Keys)
                {
                    //if (smz[key] > 0 && edge.Products.ContainsKey(key))
                    //    vars[edge.Label] = model.NumVar(1.0, UpperBound, NumVarType.Float, edge.Label);
                    //else if (smz[key] < 0 && edge.Reactants.ContainsKey(key))
                    //    vars[edge.Label] = model.NumVar(1.0, UpperBound, NumVarType.Float, edge.Label);
                    //else
                    vars[edge.Label] = model.NumVar(0.0, UpperBound, NumVarType.Float, edge.Label);
                }
            }


            AddReactionConstrait(sm, model, vars);
            var fobj = AddGlobalConstraint(smz, sm, model, vars);
            Console.WriteLine(fobj.ToString());
            model.AddObjective(ObjectiveSense.Maximize, fobj, "fobj");

            var isfeas = model.Solve();
            model.ExportModel(string.Format("{0}{1}model.lp", Util.Dir, sm.LastLevel));

            sm.Edges.Values.ToList().ForEach(e => e.PreValue = e.Flux);

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
                if (metabolite.IsPseudo)
                {
                    if (metabolite.Consumers.Count == 0)
                        Console.WriteLine("GG");
                    var consumer = metabolite.Consumers.First();
                    var producer = metabolite.Producers.First();

                    var consexp1 = model.LinearNumExpr();
                    var prodexp1 = model.LinearNumExpr();
                    //var unionexp = model.LinearNumExpr();
                    var interexp = model.LinearNumExpr();

                    var inter = new HashSet<Guid>(consumer.Reactions);
                    inter.IntersectWith(producer.Reactions);

                    var union = new HashSet<Guid>(consumer.Reactions);
                    union.UnionWith(producer.Reactions);
                    union.ExceptWith(inter);


                    foreach (var nvar in consumer.Reactions
                        .Select(cid => "sym_" + Util.CachedR(cid).SbmlId)
                        .Select(cname => model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, cname)))
                    {
                        if (!vars.ContainsKey(nvar.Name))
                            vars[nvar.Name] = nvar;
                        consexp1.AddTerm(vars[nvar.Name], 1.0);
                    }
                    model.AddEq(consexp1, vars[consumer.Label], metabolite.Label + "_cons");

                    foreach (var nvar in producer.Reactions
                        .Select(cid => "sym_" + Util.CachedR(cid).SbmlId)
                        .Select(cname => model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, cname)))
                    {
                        if (!vars.ContainsKey(nvar.Name))
                            vars[nvar.Name] = nvar;
                        prodexp1.AddTerm(vars[nvar.Name], 1.0);
                    }
                    model.AddEq(prodexp1, vars[producer.Label], metabolite.Label + "_prod");

                    foreach (var nvar in inter
                        .Select(cid => "sym_" + Util.CachedR(cid).SbmlId)
                        .Select(cname => model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, cname)))
                    {
                        if (!vars.ContainsKey(nvar.Name))
                            vars[nvar.Name] = nvar;
                        interexp.AddTerm(vars[nvar.Name], 1.0);
                    }
                    model.AddGe(interexp, model.Min(vars[producer.Label], vars[consumer.Label]), metabolite.Label + "_min");

                    //foreach (var nvar in union
                    //    .Select(cid => "sym_" + Util.CachedR(cid).SbmlId)
                    //    .Select(cname => model.NumVar(1.0, double.PositiveInfinity, NumVarType.Float, cname)))
                    //{
                    //    if (!vars.ContainsKey(nvar.Name))
                    //        vars[nvar.Name] = nvar;
                    //    unionexp.AddTerm(vars[nvar.Name], 1.0);
                    //}
                    //if (union.Count > 0)
                    //    model.AddGe(unionexp, model.Abs(model.Diff(vars[producer.Label], vars[consumer.Label])), metabolite.Label + "_abs");

                    continue;
                }

                var exp = model.LinearNumExpr();
                var consexp = model.LinearNumExpr();
                var prodexp = model.LinearNumExpr();
                //double consumedCoeff = 0, producedCoeff = 0;
                var consumedCount = 2;
                var producedCount = 2;
                foreach (var reaction in metabolite.AllReactions())
                {
                    var coefficient = Coefficient(reaction, metabolite);
                    exp.AddTerm(vars[reaction.Label], coefficient);

                    if (smz.ContainsKey(metabolite.Id))
                    {
                        fobj.AddTerm(vars[reaction.Label], coefficient * smz[metabolite.Id]);
                    }

                    if (reaction.Reactants.ContainsKey(metabolite.Id)
                        //&& metabolite.RemovedConsumerExchange != null
                        //&& metabolite.RemovedConsumerExchange.Level < reaction.Level
                        )
                    {
                        consexp.AddTerm(vars[reaction.Label], Math.Abs(coefficient));
                        //consumedCoeff += Math.Abs(coefficient);
                        consumedCount++;
                    }

                    if (reaction.Products.ContainsKey(metabolite.Id)
                        //&& metabolite.RemovedProducerExchange != null
                        //&& metabolite.RemovedProducerExchange.Level < reaction.Level
                        )
                    {
                        prodexp.AddTerm(vars[reaction.Label], Math.Abs(coefficient));
                        //producedCoeff += Math.Abs(coefficient);
                        producedCount++;
                    }
                }
                model.AddEq(exp, 0.0, metabolite.Label);

                if (RemoveConstraints)
                {
                    continue;
                }

                if (metabolite.Consumers.Any(e => e.Label.StartsWith("R_EX")) && metabolite.Consumers.Count > 1)
                {
                    var exch = metabolite.Consumers.First(e => e.Label.StartsWith("R_EX"));
                    var exp1 = model.LinearNumExpr();
                    foreach (var consumer in metabolite.Consumers)
                    {
                        var c = 1;//Coefficient(consumer, metabolite);
                        exp1.AddTerm(vars[consumer.Label], Math.Abs(c));
                    }

                    var constr = model.Not(model.Ge(vars[exch.Label], exp1, metabolite.Label + "_exch"));
                    model.Add(constr);
                }

                var skip = false;
                if (metabolite.RemovedConsumerExchange != null && metabolite.RemovedConsumerExchange.Products.Any(n => n.Value.IsPseudo))
                {
                    var consexp1 = model.LinearNumExpr();
                    foreach (var react in metabolite.RemovedConsumerExchange.InitReactions
                        .Select(Util.CachedR))
                    //.Select(cname => model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, cname)))
                    {
                        if (!vars.ContainsKey(react.Name))
                            vars[react.Name] = model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, react.Name);
                        var c = 1;// Coefficient(sm.Edges[react.ID], metabolite);
                        consexp1.AddTerm(vars[react.Name], c);
                    }
                    //if (Math.Abs(metabolite.RemovedConsumerExchange.Flux) > Double.Epsilon)
                    model.AddLe(consexp1, metabolite.RemovedConsumerExchange.Flux * (1 + Change), metabolite.Label + "_cons2U");
                    model.AddGe(consexp1, metabolite.RemovedConsumerExchange.Flux * (1 - Change), metabolite.Label + "_cons2L");

                    skip = true;
                }

                if (metabolite.RemovedProducerExchange != null && metabolite.RemovedProducerExchange.Products.Any(n => n.Value.IsPseudo))
                {
                    var prodexp1 = model.LinearNumExpr();
                    foreach (var react in metabolite.RemovedProducerExchange.InitReactions
                        .Select(Util.CachedR))
                    //.Select(cname => model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, cname)))
                    {
                        if (!vars.ContainsKey(react.Name))
                            vars[react.Name] = model.NumVar(0.0, double.PositiveInfinity, NumVarType.Float, react.Name);
                        var c = 1;// Coefficient(sm.Edges[react.ID], metabolite);
                        prodexp1.AddTerm(vars[react.Name], c);
                    }
                    //if (Math.Abs(metabolite.RemovedProducerExchange.Flux) > Double.Epsilon)
                    model.AddLe(prodexp1, metabolite.RemovedProducerExchange.Flux * (1 + Change), metabolite.Label + "_prod2U");
                    model.AddGe(prodexp1, metabolite.RemovedProducerExchange.Flux * (1 - Change), metabolite.Label + "_prod2L");

                    skip = true;
                }
                if (skip) continue;

                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                if (metabolite.Consumers.Count > 0)
                {
                    model.AddGe(
                        consexp,
                        smz.ContainsKey(metabolite.Id) && smz[metabolite.Id] < 0 ? 1 : 0,//consumedCoeff * Results[RemovedConsumerExchange[metabolite.Id].Label],
                        string.Format("{0}_cons", metabolite.Label));
                    //model.AddRange(
                    //    consumedCoeff * Results[RemovedConsumerExchange[metabolite.Id].Label]
                    //    - 0.1 * Results[RemovedConsumerExchange[metabolite.Id].Label],
                    //    consexp,
                    //    consumedCoeff * Results[RemovedConsumerExchange[metabolite.Id].Label]
                    //    + 0.1 * Results[RemovedConsumerExchange[metabolite.Id].Label],
                    //    $"{metabolite.Label}_cons");
                }

                if (metabolite.Producers.Count > 0)
                {
                    model.AddGe(
                        prodexp,
                        smz.ContainsKey(metabolite.Id) && smz[metabolite.Id] > 0 ? 1 : 0,//producedCoeff * Results[RemovedProducerExchange[metabolite.Id].Label],
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
                if (RemoveConstraints) continue;
                // updated pseudo exchange reaction constriants
                // a1*r1 + a2*r2 + ... + b*exr = (sum(ai) + b)*flux(exr)
                // ai is stoch of ri
                // ri is pulled out from exr 
                // b = (sum of stoch's of reactions exr representing, initExchangeStoch) - sum(ai)
                if (reaction.Value.UpdatePseudo.Count != 0)
                {
                    var sv = model.LinearNumExpr();
                    var isProducer = (reaction.Value.Products.Count(n => !n.Value.IsPseudo) > 0);
                    var isConsumer = (reaction.Value.Reactants.Count(n => !n.Value.IsPseudo) > 0);

                    var meta = new KeyValuePair<Guid, HyperGraph.Node>();
                    double initExchangeStoch = -1;
                    double sum_ai = -1;
                    if (isProducer)// && !reaction.Value.Products.Any(n => n.Value.IsPseudo)
                    {
                        meta = reaction.Value.Products.First(n => !n.Value.IsPseudo);
                        initExchangeStoch = Util.GetStoichiometry(meta.Key).Item2;
                        sum_ai =
                            meta.Value.Producers.Where(e => !e.IsPseudo)
                                .Sum(e => Math.Abs(Coefficient(e, meta.Value)));
                    }
                    else if (isConsumer)// && !reaction.Value.Reactants.Any(n => n.Value.IsPseudo)
                    {
                        meta = reaction.Value.Reactants.First(n => !n.Value.IsPseudo);
                        initExchangeStoch = Util.GetStoichiometry(meta.Key).Item1;
                        sum_ai =
                         meta.Value.Consumers.Where(e => !e.IsPseudo)
                             .Sum(e => Math.Abs(Coefficient(e, meta.Value)));
                    }
                    var sum_ri = 0.0;
                    foreach (var r in reaction.Value.UpdatePseudo)
                    {
                        var c = Math.Abs(Coefficient(sm.Edges[r], meta.Value));
                        sv.AddTerm(vars[sm.Edges[r].Label], c);
                        sum_ri += c;
                    }
                    sv.AddTerm(vars[reaction.Value.Label], (initExchangeStoch - sum_ai));

                    var st = (initExchangeStoch - (sum_ai - sum_ri));
                    model.AddLe(sv, reaction.Value.Flux * (1 + Change) * st, string.Format("update{0}ub", i++));
                    model.AddGe(sv, reaction.Value.Flux * (1 - Change) * st, string.Format("update{0}lb", i));

                }
                else if (reaction.Value.PreValue != -1 && Math.Abs(reaction.Value.Flux) > double.Epsilon)
                {
                    if (!reaction.Value.IsPseudo && reaction.Value.ToServerReaction.Reversible)
                    {
                        continue;
                    }
                    //if (reaction.Flux.Flux != 0)

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
        }
    }
}