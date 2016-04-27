namespace Ecoli
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ILOG.Concert;
    using ILOG.CPLEX;

    using Util;

    public class Fba3
    {
        public static double LastRuntime { get; set; }
        public static List<IConstraint> Constraints = new List<IConstraint>();
        public static Dictionary<Guid, Tuple<double, double>> ReactionsConstraintsDictionary = new Dictionary<Guid, Tuple<double, double>>();
        const double Change = 0.01;

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

            sm.Edges.Values.ToList().ForEach(e => e.PreFlux = e.Flux);

            foreach (var edge in sm.Edges.Values)
            {
                if (edge.IsPseudo)
                {
                    vars[edge.Id] = model.NumVar(pseudoLowerBound, pseudoUpperBound, NumVarType.Float, edge.Label);
                }
                else if (edge is HyperGraph.Cycle)
                {
                    cycleMetabolitesVars[edge.Id] = new Dictionary<Guid, INumVar>();
                    var cycle = edge as HyperGraph.Cycle;

                    // make it irreversible bounds for only products or only reactants but not both
                    cycle.Products.Union(cycle.Reactants).Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                                cycleMetabolitesVars[edge.Id][m.Key] =
                                    model.NumVar(cycleIrreversibleLowerBound, cycleUpperBound, NumVarType.Float,
                                        edge.Label + "_irrev_" + m.Value.Label));

                    // make it reversible bounds for shared products and reactants
                    cycle.Reactants.Intersect(cycle.Products).ToList()
                        .ForEach(
                            m =>
                                cycleMetabolitesVars[edge.Id][m.Key] =
                                    model.NumVar(cycleReversibleLowerBound, cycleUpperBound, NumVarType.Float,
                                        edge.Label + "_rev_" + m.Value.Label));


                    // add atom numbers constraints to link metabolites of the cycle reaction
                    var carbonExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "C");
                                if (formula == null) return;

                                carbonExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                       .ForEach(
                           m =>
                           {
                               var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "C");
                               if (formula == null) return;

                               carbonExpr.AddTerm(- formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                           });
                    Constraints.Add(model.AddEq(carbonExpr, 0.0, edge.Label + "_carbon_atoms_balance"));

                    var hydrogenExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "H");
                                if (formula == null) return;

                                hydrogenExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "H");
                                if (formula == null) return;

                                hydrogenExpr.AddTerm(- formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(hydrogenExpr, 0.0, edge.Label + "_hydrogen_atoms_balance"));

                    var nitrogenExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "N");
                                if (formula == null) return;

                                nitrogenExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "N");
                                if (formula == null) return;

                                nitrogenExpr.AddTerm(- formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(nitrogenExpr, 0.0, edge.Label + "_nitrogen_atoms_balance"));

                    var oxygenExpr = model.LinearNumExpr();
                    cycle.Products.ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "O");
                                if (formula == null) return;

                                oxygenExpr.AddTerm(formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    cycle.Reactants.Except(cycle.Products.Intersect(cycle.Reactants)).ToList()
                        .ForEach(
                            m =>
                            {
                                var formula = Db.Context.Formulae.SingleOrDefault(f => f.speciesId == m.Key && f.atom == "O");
                                if (formula == null) return;

                                oxygenExpr.AddTerm(- formula.numAtoms, cycleMetabolitesVars[edge.Id][m.Key]);
                            });
                    Constraints.Add(model.AddEq(oxygenExpr, 0.0, edge.Label + "_oxygen_atoms_balance"));
                }
                else
                {
                    // TODO get reaction bounds from database with the construction of reactions
                    var reactionBounds = Db.Context.ReactionBounds.Single(rb => rb.reactionId == edge.Id);
                    vars[edge.Id] = model.NumVar(reactionBounds.lowerBound, reactionBounds.upperBound, NumVarType.Float, edge.Label);
                }
            }

            AddReactionsConstraits(sm, model, vars, cycleMetabolitesVars);
            AddMetabolitesStableStateConstraints(sm, model, vars, cycleMetabolitesVars);
            AddObjectiveFunction(sm, model, vars, cycleMetabolitesVars);

            var isfeas = model.Solve();
            model.ExportModel($"{Core.Dir}{sm.Step}model.lp");

            if (isfeas)
            {
                sm.Edges.Where(e => !(e.Value is HyperGraph.Cycle))
                    .ToList()
                    .ForEach(d => d.Value.Flux = model.GetValue(vars[d.Value.Id]));

                cycleMetabolitesVars.Keys.ToList().ForEach(c => cycleMetabolitesVars[c].Keys.ToList().ForEach(m => sm.Cycles[c].Fluxes[m] = model.GetValue(cycleMetabolitesVars[c][m])));
            }
            else
            {
                Debug(sm.Step);
                sm.Edges.ToList().ForEach(d => d.Value.Flux = 0);
            }

            sm.Edges.Values.ToList().ForEach(e => e.RecentlyAdded = false);
            var list = sm.Edges.ToList().Select(d => $"{d.Value.Label}:{d.Value.Flux}").ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines($"{Core.Dir}{sm.Step}result.txt", list);

            var status = model.GetCplexStatus();
            Console.WriteLine(status);

            return isfeas;
        }

        private static void AddObjectiveFunction(HyperGraph hyperGraph, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<Guid, Dictionary<Guid, INumVar>> cycleMetabolitesVars)
        {
            var fobj = model.LinearNumExpr();
            var metabolite = hyperGraph.Nodes[TheAlgorithm.StartingMetabolite];

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

            foreach (var c in metabolite.Consumers.Except(metabolite.Producers))
            {
                if (c is HyperGraph.Cycle)
                {
                    fobj.AddTerm(-1, cycleMetabolitesVars[c.Id][metabolite.Id]);
                }
                else
                {
                    fobj.AddTerm(-1, vars[c.Id]);
                }
            }

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

            model.AddObjective(ObjectiveSense.Maximize, fobj, "fobj");
        }

        private static void AddMetabolitesStableStateConstraints(HyperGraph sm, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<Guid, Dictionary<Guid, INumVar>> cycleMetabolitesVars)
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

        private static void AddReactionsConstraits(HyperGraph sm, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<Guid, Dictionary<Guid, INumVar>> cycleMetabolitesVars)
        {
            foreach (var reaction in sm.Edges.Where(e => ! (e.Value is HyperGraph.Cycle) && ! e.Value.RecentlyAdded))
            {
                if (!ReactionsConstraintsDictionary.ContainsKey(reaction.Key))
                {
                    ReactionsConstraintsDictionary[reaction.Key] = Tuple.Create(reaction.Value.Flux*(1 - Change), reaction.Value.Flux * (1 + Change));
                }
                Constraints.Add(model.AddGe(vars[reaction.Value.Id], ReactionsConstraintsDictionary[reaction.Key].Item1, $"{reaction.Value.Label}_lb"));
                Constraints.Add(model.AddLe(vars[reaction.Value.Id], ReactionsConstraintsDictionary[reaction.Key].Item2, $"{reaction.Value.Label}_ub"));
            }

            foreach (var cycle in sm.Cycles)
            {
                foreach (var m in cycle.Value.Fluxes)
                {
                    Constraints.Add(model.AddGe(cycleMetabolitesVars[cycle.Key][m.Key], m.Value * (1 - Change), $"{cycle.Value.Label}_{sm.Nodes[m.Key].Label}lb"));
                    Constraints.Add(model.AddLe(cycleMetabolitesVars[cycle.Key][m.Key], m.Value * (1 + Change), $"{cycle.Value.Label}_{sm.Nodes[m.Key].Label}_ub"));
                }
            }

            foreach (var constraint in sm.ExchangeConstraints)
            {
                var expr = model.LinearNumExpr();
                expr.AddTerms(constraint.Item1.Select(r => vars[r]).ToArray(), constraint.Item2.ToArray());
                Constraints.Add(model.AddGe(expr, constraint.Item3 * (1 - Change), $"Exchange_{sm.Edges[constraint.Item1[0]].Label}_lb"));
                Constraints.Add(model.AddGe(expr, constraint.Item3 * (1 + Change), $"Exchange_{sm.Edges[constraint.Item1[0]].Label}_ub"));
            }
        }


        public static void Debug(int step)
        {
            var model = new Cplex();
            model.ImportModel($"{Core.Dir}{step}model.lp");
            model.SetParam(Cplex.Param.Preprocessing.Presolve, false);
            var m = model.GetLPMatrixEnumerator();

            m.MoveNext();
            var mat = (CpxLPMatrix)m.Current;

            var vars = mat.NumVars;
            var ranges = mat.GetRanges();
            var obj = model.GetObjective();

            if (File.Exists($"{Core.Dir}debug.txt"))
                File.Delete($"{Core.Dir}debug.txt");

            for (var i = 0; i < ranges.Length; i++)
            {
                var model2 = new Cplex { Name = ranges[i].Name };
                model2.SetParam(Cplex.Param.Preprocessing.Presolve, false);
                var cloner = new SimpleCloneManager(model2);
                foreach (var var in vars)
                {
                    model2.NumVar(var.LB, var.UB, var.Type, var.Name);
                }

                for (var j = 0; j < ranges.Length; j++)
                {
                    if (j == i) continue;

                    model2.AddRange(ranges[j].LB, (CpxQLExpr)ranges[j].Expr.MakeClone(cloner), ranges[j].UB, ranges[j].Name);
                }

                model2.AddObjective(obj.Sense, (CpxQLExpr)obj.Expr.MakeClone(cloner));

                model2.Solve();
                File.AppendAllLines($"{Core.Dir}debug.txt", new[] {$"removing {ranges[i].Name}: {model2.GetStatus()}"});

                model2.EndModel();
            }
        }

    }
}