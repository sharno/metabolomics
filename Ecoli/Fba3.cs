using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using Metabol.DbModels;


namespace Ecoli
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    public class Fba3
    {
        public static List<IConstraint> Constraints = new List<IConstraint>();
        public static Dictionary<Guid, Tuple<double, double>> ReactionsConstraintsDictionary = new Dictionary<Guid, Tuple<double, double>>();
        const double Change = 0.01;
        private const double ZeroOutFlux = 0.01;
        //TODO get rid of this
        public static List<string> ConstraintList = new List<string>();

        public static bool Solve(HyperGraph sm)
        {
            // temp name cycles
            //sm.Cycles.Keys.Where(c => ! CyclesNames.ContainsKey(c)).ToList().ForEach(c => CyclesNames[c] = $"c{CycleNum++}");

            var model = new Cplex { Name = "FBA" };
            var vars = new Dictionary<Guid, INumVar>();
            var cycleMetabolitesVars = new Dictionary<Guid, Dictionary<Guid, INumVar>>();

            const double pseudoUpperBound = 1000.0;
            const double pseudoLowerBound = 0.0;
            const double cycleUpperBound = 1000.0;
            const double cycleReversibleLowerBound = -1000.0;
            const double cycleIrreversibleLowerBound = 0.0;

            sm.Edges.Values.ToList().ForEach(e => e.PreFlux = Math.Abs(e.Flux) < 0.00001 ? 0 : e.Flux);
            sm.Cycles.Values.ToList().ForEach(c => c.Fluxes.Keys.ToList().ForEach(m => c.PreFluxes[m] = Math.Abs(c.Fluxes[m]) < 0.00001 ? 0 : c.Fluxes[m]));
            foreach (var cycle in sm.Cycles)
            {
                var f = cycle.Value.PreFluxes;
            }

            foreach (var edge in sm.Edges.Values)
            {
                if (edge.IsPseudo)
                {
                    vars[edge.Id] = model.NumVar(pseudoLowerBound, pseudoUpperBound, NumVarType.Float, edge.Label);
                }
                else if (edge is HyperGraph.Cycle)
                {
                    #region cycle

                    cycleMetabolitesVars[edge.Id] = new Dictionary<Guid, INumVar>();
                    var cycle = edge as HyperGraph.Cycle;

                    // make it irreversible bounds for only products or only reactants but not both
                    cycle.Products.Union(cycle.Reactants).Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                                cycleMetabolitesVars[edge.Id][m.Key] =
                                    model.NumVar(cycleIrreversibleLowerBound, cycleUpperBound, NumVarType.Float,
                                        edge.Label + /*"_irrev"*/ "_" + m.Value.Label));

                    // make it reversible bounds for shared products and reactants
                    cycle.Reactants.Intersect(cycle.Products).ToList()
                        .ForEach(
                            m =>
                                cycleMetabolitesVars[edge.Id][m.Key] =
                                    model.NumVar(cycleReversibleLowerBound, cycleUpperBound, NumVarType.Float,
                                        edge.Label + /*"_rev*/ "_" + m.Value.Label));



                    var ratios =
                        Db.Context.cycleInterfaceMetabolitesRatios.Where(ci => ci.cycleId == cycle.Id).ToList();
                    ratios.ForEach(ra =>
                    {
                        //if (ra.ratio < 1 || ra.ratio > 100) return;

                        var expr1 = model.LinearNumExpr();
                        expr1.AddTerm(1, cycleMetabolitesVars[edge.Id][ra.metabolite1]);

                        //var expr2 = model.LinearNumExpr();
                        //expr2.AddTerm(Math.Abs(ra.ratio), cycleMetabolitesVars[edge.Id][ra.metabolite1]);


                        var expr2Low = model.LinearNumExpr();
                        expr2Low.AddTerm(Math.Abs(ra.ratio) * 0.9, cycleMetabolitesVars[edge.Id][ra.metabolite2]);

                        var expr2High = model.LinearNumExpr();
                        expr2High.AddTerm(Math.Abs(ra.ratio) * 1.1, cycleMetabolitesVars[edge.Id][ra.metabolite2]);

                        Console.WriteLine("ratio: " + ra.Species.sbmlId + " / " + ra.Species1.sbmlId + " = " + ra.ratio);


                        //var equal = model.Eq(model.Abs(expr1), model.Abs(expr2), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio");

                        var or = model.Or();

                        var low1 = model.Ge(model.Abs(expr1), model.Abs(expr2Low),
                            $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_low");
                        var high1 = model.Le(model.Abs(expr1), model.Abs(expr2High),
                            $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_high");
                        var withRatio1 = model.And();
                        withRatio1.Add(low1);
                        withRatio1.Add(high1);
                        or.Add(withRatio1);

                        //var low2 = model.Le(model.Abs(expr1), model.Abs(expr2Low), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_low");
                        //var high2 = model.Ge(model.Abs(expr1), model.Abs(expr2High), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_high");
                        //var withRatio2 = model.And();
                        //withRatio2.Add(low2);
                        //withRatio2.Add(high2);
                        //or.Add(withRatio2);

                        //var upperBound = model.BoolVar();
                        //var lowerBound = model.BoolVar();
                        //var withinBounds = model.BoolVar();
                        //var leftZero = model.BoolVar();
                        //var rightZero = model.BoolVar();

                        //// upperBound
                        //model.Add(model.IfThen(model.Le(model.Abs(expr1), model.Abs(expr2High), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_high"), model.Eq(upperBound, 1)));
                        //model.Add(model.IfThen(model.Not(model.Le(model.Abs(expr1), model.Abs(expr2High), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_high")), model.Eq(upperBound, 0)));

                        //// lowerBound
                        //model.Add(model.IfThen(model.Ge(model.Abs(expr1), model.Abs(expr2Low), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_low"), model.Eq(lowerBound, 1)));
                        //model.Add(model.IfThen(model.Not(model.Ge(model.Abs(expr1), model.Abs(expr2Low), $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_low")), model.Eq(lowerBound, 0)));

                        //// withinBounds (and-ing operation)
                        //model.Add(model.IfThen(model.Eq(model.Sum(upperBound, lowerBound), 2), model.Eq(withinBounds, 1)));
                        //model.Add(model.IfThen(model.Not(model.Eq(model.Sum(upperBound, lowerBound), 2)), model.Eq(withinBounds, 1)));

                        //// leftZero
                        //model.Add(model.IfThen(model.Eq(model.Abs(expr1), 0), model.Eq(leftZero, 1)));
                        //model.Add(model.IfThen(model.Not(model.Eq(model.Abs(expr1), 0)), model.Eq(leftZero, 0)));

                        //// rightZero
                        //model.Add(model.IfThen(model.Eq(model.Abs(expr2High), 0), model.Eq(rightZero, 1)));
                        //model.Add(model.IfThen(model.Not(model.Eq(model.Abs(expr2High), 0)), model.Eq(rightZero, 0)));

                        //// or-ing operation
                        //model.AddGe(model.Sum(withinBounds, leftZero, rightZero), 1);

                        var zeroLeft = model.Le(model.Abs(cycleMetabolitesVars[edge.Id][ra.metabolite1]), ZeroOutFlux);
                        var zeroRight = model.Le(model.Abs(cycleMetabolitesVars[edge.Id][ra.metabolite2]), ZeroOutFlux);

                        or.Add(zeroLeft);
                        or.Add(zeroRight);

                        //model.Add(or);
                        model.Add(low1);
                        model.Add(high1);
                    });

                    // add atom numbers constraints to link metabolites of the cycle reaction
                    var carbonExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "C");
                                if (formula == null) return;

                                carbonExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "C");
                                if (formula == null) return;

                                carbonExpr.AddTerm(-formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(carbonExpr, 0.0, edge.Label + "_carbon_atoms_balance"));

                    var hydrogenExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "H");
                                if (formula == null) return;

                                hydrogenExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "H");
                                if (formula == null) return;

                                hydrogenExpr.AddTerm(-formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(hydrogenExpr, 0.0, edge.Label + "_hydrogen_atoms_balance"));

                    var nitrogenExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "N");
                                if (formula == null) return;

                                nitrogenExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "N");
                                if (formula == null) return;

                                nitrogenExpr.AddTerm(-formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(nitrogenExpr, 0.0, edge.Label + "_nitrogen_atoms_balance"));

                    var oxygenExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "O");
                                if (formula == null) return;

                                oxygenExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula =
                                    Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "O");
                                if (formula == null) return;

                                oxygenExpr.AddTerm(-formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(oxygenExpr, 0.0, edge.Label + "_oxygen_atoms_balance"));

                    #endregion
                }
                else
                {
                    // TODO get reaction bounds from database with the construction of reactions
                    
                    var reactionBounds = Db.Context.ReactionBounds.Single(rb => rb.reactionId == edge.Id);
                    var fixedbounds = Db.Context.ReactionBoundFixes.SingleOrDefault(rbf => rbf.reactionId == edge.Id);

                    // ATPM bounds
                    if (edge.Id == Guid.Parse("0AB996A3-C97A-4A4F-968E-12E9F2AD9180"))
                    {
                        vars[edge.Id] = model.NumVar(0, reactionBounds.upperBound, NumVarType.Float,
                            edge.Label);
                        continue;
                    }

                    if (fixedbounds != null)
                    {
                        vars[edge.Id] = model.NumVar(fixedbounds.lowerbound, fixedbounds.upperbound, NumVarType.Float,
                            edge.Label);
                    }
                    else
                    {
                        vars[edge.Id] = model.NumVar(reactionBounds.lowerBound, reactionBounds.upperBound,
                            NumVarType.Float, edge.Label);
                    }
                }
            }

            AddReactionsConstraits(sm, model, vars, cycleMetabolitesVars);
            AddMetabolitesStableStateConstraints(sm, model, vars, cycleMetabolitesVars);

            //var fva = new FVA();
            //fva.Solve(model, sm, vars.Values);
            //File.AppendAllText("A:\\fva.csv", $"{fva.Stat.Item1},{fva.Stat.Item2}\r\n");

            //GeneNetwork.AddRegulationConstraints(model, sm, vars);

            //var fva = new FVA();
            //fva.Solve(model, sm, vars.Values);
            //File.AppendAllText("A:\\rfva.csv", $"{fva.Stat.Item1},{fva.Stat.Item2}\r\n");

            AddObjectiveFunction(sm, model, vars, cycleMetabolitesVars);



            model.ExportModel($"{Core.Dir}{sm.Step}model.lp");
            //model = new Cplex();
            //model.ImportModel($"{Core.Dir}{sm.Step}model.lp");

            var isfeas = model.Solve();

            if (isfeas)
            {
                sm.Edges.Where(e => !(e.Value is HyperGraph.Cycle))
                    .ToList()
                    .ForEach(d => d.Value.Flux = model.GetValue(vars[d.Value.Id]));

                cycleMetabolitesVars.Keys
                    .ToList()
                    .ForEach(c => cycleMetabolitesVars[c].Keys.ToList().ForEach(m => sm.Cycles[c].Fluxes[m] = model.GetValue(cycleMetabolitesVars[c][m])));
            }
            else
            {
                var reactionsFluxes = ReactionsFluxes(sm);
                Debug($"{Core.Dir}{sm.Step}model.lp", reactionsFluxes);
                sm.Edges.ToList().ForEach(d => d.Value.Flux = 0);
            }

            var list = sm.Edges.ToList().Select(d => $"{d.Value.Label}:{d.Value.Flux}").ToList();
            list.AddRange(cycleMetabolitesVars.Keys.ToList().SelectMany(c => cycleMetabolitesVars[c].Keys.ToList().Select(
                m =>
                {
                    if (isfeas) return $"{sm.Cycles[c].Label}_{sm.Nodes[m].Label}:{model.GetValue(cycleMetabolitesVars[c][m])}";
                    else return $"{sm.Cycles[c].Label}_{sm.Nodes[m].Label}:0";
                })));
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines($"{Core.Dir}{sm.Step}result.txt", list);

            var status = model.GetCplexStatus();
            
            Console.WriteLine(status);

            var tmpfile = Path.GetTempFileName() + ".lp";

            Console.WriteLine(status);
            model.ExportModel(tmpfile);
            ConstraintList.Clear();
            ConstraintList.AddRange(Core.Constraints(tmpfile));

            Core.SaveAsDgs(sm.Nodes.First().Value, sm, Core.Dir);

            sm.Nodes.Values.ToList().ForEach(n => n.RecentlyAdded = false);
            sm.Edges.Values.ToList().ForEach(e => e.RecentlyAdded = false);

            return isfeas;
        }

        public static Dictionary<string, double> ReactionsFluxes(HyperGraph sm)
        {
            var reactionsFluxes = new Dictionary<string, double>();
            sm.Edges.Where(e => !(e.Value is HyperGraph.Cycle) && !e.Value.RecentlyAdded)
                .ToList()
                .ForEach(e => reactionsFluxes[e.Value.Label] = e.Value.PreFlux);
            sm.Cycles
                .ToList()
                .ForEach(c => c.Value.Fluxes.Keys.ToList()
                    .ForEach(m => reactionsFluxes[$"{c.Value.Label}_{sm.Nodes[m].Label}"] = c.Value.PreFluxes[m]));
            reactionsFluxes.Keys.Where(f => f.StartsWith("EX_")).ToList().ForEach(f =>
            {
                reactionsFluxes["_" + f] = reactionsFluxes[f];
                reactionsFluxes.Remove(f);
            });
            return reactionsFluxes;
        }

        public static void AddObjectiveFunction(HyperGraph hyperGraph, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<Guid, Dictionary<Guid, INumVar>> cycleMetabolitesVars)
        {
            var fobj = model.LinearNumExpr();
            var metabolite = hyperGraph.Nodes[TheAlgorithm.StartingMetabolite];

            if (hyperGraph.Step == 0)
            {
                //model.Add(model.Not(model.Eq(vars[hyperGraph.Edges.Keys.ToList()[0]], 0)));
                var t = hyperGraph.Edges.Values.Single(e => e.Label == "_exr_succ_e_cons");
                model.AddGe(vars[t.Id], 10);
            }

            foreach (var p in metabolite.Producers.Except(metabolite.Consumers))
            {
                if (p is HyperGraph.Cycle)
                {
                    fobj.AddTerm(1, cycleMetabolitesVars[p.Id][metabolite.Id]);
                }
                else
                {
                    fobj.AddTerm(1, vars[p.Id]);
                }
            }

            //foreach (var c in metabolite.Consumers.Except(metabolite.Producers))
            //{
            //    if (c is HyperGraph.Cycle)
            //    {
            //        fobj.AddTerm(-1, cycleMetabolitesVars[c.Id][metabolite.Id]);
            //    }
            //    else
            //    {
            //        fobj.AddTerm(-1, vars[c.Id]);
            //    }
            //}

            // if the objective reaction is not in the graph and one of its parent cycles is
            //if (! vars.ContainsKey(TempObjective))
            //{
            //    if (vars.ContainsKey(Objective))
            //        TempObjective = Objective;
            //    else
            //    {
            //        var parentCycleInGraph = Db.ParentCyclesOfReactionOrCycle(Objective).Intersect(vars.Keys).Single();
            //        TempObjective = parentCycleInGraph;
            //    }
            //}
            //fobj.AddTerm(1, vars[TempObjective]);

            // make the objective non zero
            //model.AddGe(vars[TempObjective], 1, $"{TempObjective}_nonzeroc");

            Console.WriteLine(fobj.ToString());
            model.Remove(model.GetObjective());
            model.AddObjective(ObjectiveSense.Maximize, fobj, "fobj");
        }

        public static void AddMetabolitesStableStateConstraints(HyperGraph sm, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<Guid, Dictionary<Guid, INumVar>> cycleMetabolitesVars)
        {
            foreach (var metabolite in sm.Nodes.Values)
            {
                var expr = model.LinearNumExpr();

                foreach (var reaction in metabolite.Producers)
                {
                    if (reaction is HyperGraph.Cycle)
                    {
                        expr.AddTerm(1, cycleMetabolitesVars[reaction.Id][metabolite.Id]);
                    }
                    else if (reaction.IsPseudo)
                    {
                        expr.AddTerm(Db.PseudoReactionStoichiometry, vars[reaction.Id]);
                    }
                    else
                    {
                        expr.AddTerm(metabolite.Weights[reaction.Id], vars[reaction.Id]);
                    }
                }

                foreach (var reaction in metabolite.Consumers)
                {
                    if (reaction is HyperGraph.Cycle)
                    {
                        // we don't want to add the cycle again if it was added in Producers
                        if (!metabolite.Producers.Contains(reaction))
                        {
                            expr.AddTerm(-1, cycleMetabolitesVars[reaction.Id][metabolite.Id]);
                        }
                    }
                    else if (reaction.IsPseudo)
                    {
                        expr.AddTerm(-1 * Db.PseudoReactionStoichiometry, vars[reaction.Id]);
                    }
                    else
                    {
                        expr.AddTerm(metabolite.Weights[reaction.Id], vars[reaction.Id]);
                    }
                }

                Constraints.Add(model.AddEq(expr, 0.0, metabolite.Label));
            }
        }

        public static void AddReactionsConstraits(HyperGraph sm, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<Guid, Dictionary<Guid, INumVar>> cycleMetabolitesVars)
        {
            foreach (var reaction in sm.Edges.Where(e => !(e.Value is HyperGraph.Cycle) && !e.Value.RecentlyAdded))
            {
                // debugging
                //if (reaction.Value.Flux == 0) continue;
                //if (reaction.Value.Label == "EX_pi_e") continue;
                //if (reaction.Value.Label == "_exr_h2o_c_cons") continue;
                //if (reaction.Value.Label == "_exr_h_e_cons") continue;
                //if (reaction.Value.Label == "EX_glc__D_e") continue;


                if (!ReactionsConstraintsDictionary.ContainsKey(reaction.Key))
                {
                    if (Math.Abs(reaction.Value.Flux) > double.Epsilon && Math.Abs(reaction.Value.Flux) < ZeroOutFlux)
                    {
                        var ub = ZeroOutFlux;
                        var lb = - ZeroOutFlux;
                        ReactionsConstraintsDictionary[reaction.Key] = Tuple.Create(lb, ub);

                        //reaction.Value.Flux = Math.Abs(reaction.Value.Flux) < ZeroOutFlux ? 0 : reaction.Value.Flux;
                    }
                    else
                    {

                        var ub = Math.Max(reaction.Value.Flux*(1 - Change), reaction.Value.Flux*(1 + Change));
                        var lb = Math.Min(reaction.Value.Flux*(1 - Change), reaction.Value.Flux*(1 + Change));
                        ReactionsConstraintsDictionary[reaction.Key] = Tuple.Create(lb, ub);
                    }
                }
                Constraints.Add(model.AddGe(vars[reaction.Value.Id], ReactionsConstraintsDictionary[reaction.Key].Item1, $"{reaction.Value.Label}_lb"));
                Constraints.Add(model.AddLe(vars[reaction.Value.Id], ReactionsConstraintsDictionary[reaction.Key].Item2, $"{reaction.Value.Label}_ub"));
            }

            // adding cycles to metabolites constraints
            //foreach (var cycle in sm.Cycles)
            //{
            //    foreach (var m in cycle.Value.Fluxes)
            //    {
            //        var ub = Math.Max(m.Value * (1 - Change), m.Value * (1 + Change));
            //        var lb = Math.Min(m.Value * (1 - Change), m.Value * (1 + Change));
            //        Constraints.Add(model.AddGe(cycleMetabolitesVars[cycle.Key][m.Key], lb, $"{cycle.Value.Label}_{sm.Nodes[m.Key].Label}lb"));
            //        Constraints.Add(model.AddLe(cycleMetabolitesVars[cycle.Key][m.Key], ub, $"{cycle.Value.Label}_{sm.Nodes[m.Key].Label}_ub"));
            //    }
            //}

            foreach (var constraint in sm.ExchangeConstraints)
            {
                // debugging
                //if (sm.Edges[constraint.Item1[0]].Label == "EX_pi_e") continue;
                //if (sm.Edges[constraint.Item1[0]].Label == "_exr_h2o_c_cons") continue;
                //if (sm.Edges[constraint.Item1[0]].Label == "_exr_h_e_cons") continue;
                //if (sm.Edges[constraint.Item1[0]].Label == "EX_glc__D_e") continue;

                var expr = model.LinearNumExpr();
                expr.AddTerms(constraint.Item1.Select(r => vars[r]).ToArray(), constraint.Item2.ToArray());


                var ub = Math.Max(constraint.Item3 * (1 - Change), constraint.Item3 * (1 + Change));
                var lb = Math.Min(constraint.Item3 * (1 - Change), constraint.Item3 * (1 + Change));

                // zero out very small fluxes
                if (Math.Abs(constraint.Item3) > double.Epsilon && Math.Abs(constraint.Item3) < ZeroOutFlux)
                {
                    ub = ZeroOutFlux;
                    lb = -ZeroOutFlux;
                }

                Constraints.Add(model.AddGe(expr, lb, $"Exchange_{sm.Edges[constraint.Item1[0]].Label}_lb"));
                Constraints.Add(model.AddLe(expr, ub, $"Exchange_{sm.Edges[constraint.Item1[0]].Label}_ub"));
            }
        }

        public static dynamic ToDynamic(object value)
        {
            return (ExpandoObject)value.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Aggregate((IDictionary<string, object>)new ExpandoObject(), (obj, info) =>
                {
                    obj[info.Name] = info.GetValue(value);
                    return obj;
                });
        }

        public static void Debug(string file, IDictionary<string, double> prevals)
        {
            var model = new Cplex();
            //model.SetParam(Cplex.Param.Preprocessing.Presolve, false);
            model.ImportModel(file);
            dynamic link = ToDynamic(ToDynamic(ToDynamic(ToDynamic(model)._model)._gc)._link);
            link = ToDynamic(link._prev);
            var cconst = new List<CpxIfThen>();
            while (link._obj != null)
            {
                cconst.Add(link._obj as CpxIfThen);
                link = ToDynamic(link._prev);
            }

            var m = model.GetLPMatrixEnumerator();
            m.MoveNext();
            var mat = (CpxLPMatrix)m.Current;

            var vars = mat.GetNumVars();
            var ranges = mat.GetRanges();
            var obj = model.GetObjective();

            if (File.Exists($"{Core.Dir}debug.txt"))
                File.Delete($"{Core.Dir}debug.txt");

            if (File.Exists($"{Core.Dir}debug_val.txt"))
                File.Delete($"{Core.Dir}debug_val.txt");

            for (var i = 0; i < ranges.Length; i++)
            {
                var model2 = new Cplex { Name = ranges[i].Name };
                //model2.SetParam(Cplex.Param.Preprocessing.Presolve, false);
                foreach (var var in vars)
                    model2.NumVar(var.LB, var.UB, var.Type, var.Name);
                var cloner = new SimpleCloneManager(model2);

                for (var j = 0; j < ranges.Length; j++)
                {
                    if (j == i) continue;
                    model2.AddRange(ranges[j].LB, (CpxQLExpr)ranges[j].Expr.MakeClone(cloner), ranges[j].UB, ranges[j].Name);
                }

                foreach (var ifThen in cconst)
                    model2.Add((CpxIfThen)ifThen.MakeClone(cloner));

                model2.AddObjective(obj.Sense, (CpxQLExpr)obj.Expr.MakeClone(cloner));

                Console.WriteLine(model2.Solve());
                //model2.Add(ranges[i]);
                File.AppendAllLines($"{Core.Dir}debug.txt", new[] { $"{ranges[i].Name}:{model2.GetStatus()}" });

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

                File.AppendAllLines($"{Core.Dir}debug_val.txt", new[] { $"{ex2}|{ex1}" });
            }
        }
    }
}