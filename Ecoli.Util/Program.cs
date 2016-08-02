using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Ecoli.Util.DB;
using Ecoli.Util.SimpleCycle;
using Newtonsoft.Json;

using ILOG.Concert;
using ILOG.CPLEX;

namespace Ecoli.Util
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.BufferHeight = Int16.MaxValue-1;
            //EraseAndRecordToDb();
            //RecordFormulaeToDb();
            //ValidateReactionsAtomsBalance();
            //ValidateCyclesAtomsBalance();
            //CheckExchangeReactionsInCycles();
            //LimitReactionsFluxes();
            //RecordCyclesInterfaceMetabolitesRations();
            RecordCyclesRationsFromStoichiometry();

            // cycles with no ratios:
            //419FB6A3-A478-4607-BCF4-32A30C1AFDA7
            //1F958FD4-D2BB-46E1-B418-6DE0C731E50C
            //09814608-806D-4B7F-A8CB-AEAEC590A541
            //AAC30E50-570F-4927-A36D-ED534DD2DABC
            //87F8DBCE-D972-4BD2-AE60-FD05A928707B

            // 35: 419fb6a3-a478-4607-bcf4-32a30c1afda7
            // 36: 09814608-806d-4b7f-a8cb-aeaec590a541

            // 199: 01295cb3-4ada-4054-bafd-4be8696c3516
            // 133: ab5765ed-d596-4d77-ad5f-0d6a5b3d4692
            //CheckCycleRatios(Guid.Parse("01295cb3-4ada-4054-bafd-4be8696c3516"));
        }

        private const double ZeroOutFlux = 0.01;

        public static void RecordCyclesRationsFromStoichiometry()
        {
            // delete database previous records
            Db.Context.cycleInterfaceMetabolitesRatios.RemoveRange(Db.Context.cycleInterfaceMetabolitesRatios.Where(e => true));
            Db.Context.SaveChanges();

            Db.Context.Cycles.ToList();
            Db.Context.CycleConnections.ToList();
            Db.Context.CycleReactions.ToList();
            Db.Context.cycleInterfaceMetabolitesRatios.ToList();
            Db.Context.Species.ToList();

            var cache = new List<cycleInterfaceMetabolitesRatio>();
            var sortedCycles = TopologicalSortCycles(Db.Context.Cycles.ToList());
            foreach (var cycle in sortedCycles) {
                Console.WriteLine($"checking cycle: {cycle.id}");
                var nestedCycles = cycle.CycleReactions.Where(cr => !cr.isReaction).Select(cr => cr.otherId).ToList();
                foreach (CycleConnection c in cycle.CycleConnections)
                {
                    var first = Db.Context.Species.Find(c.metaboliteId);
                    var firstNumberOfConnectionsInsideCycle = cycle.CycleReactions.Count(cr => first.ReactionSpecies.Any(rs => rs.reactionId == cr.otherId));
                    firstNumberOfConnectionsInsideCycle += cycle.CycleReactions.Count(cr => first.CycleConnections.Any(cc => cc.cycleId == cr.otherId));
                    if (firstNumberOfConnectionsInsideCycle != 1) continue;

                    foreach (CycleConnection c2 in cycle.CycleConnections.Where(e => e.metaboliteId != c.metaboliteId))
                    {
                        var second = Db.Context.Species.Find(c2.metaboliteId);
                        var secondNumberOfConnectionsInsideCycle = cycle.CycleReactions.Count(cr => second.ReactionSpecies.Any(rs => rs.reactionId == cr.otherId));
                        secondNumberOfConnectionsInsideCycle += cycle.CycleReactions.Count(cr => second.CycleConnections.Any(cc => cc.cycleId == cr.otherId));
                        if (secondNumberOfConnectionsInsideCycle != 1) continue;

                        var sharedReactions = first.ReactionSpecies.Where(rs => second.ReactionSpecies.Any(rs2 => rs2.reactionId == rs.reactionId) && cycle.CycleReactions.Any(cr => cr.otherId == rs.reactionId)).ToList();
                        var nestedRatios = cache.Where(ra => nestedCycles.Contains(ra.cycleId) && ra.metabolite1 == first.id && ra.metabolite2 == second.id).ToList();

                        if (sharedReactions.Count == 1 && nestedRatios.Count == 0 && ! cache.Any(ra => ra.metabolite1 == second.id && ra.metabolite2 == first.id))
                        {
                            Console.WriteLine($"recording: {first.sbmlId}, {second.sbmlId}");
                            var record = new cycleInterfaceMetabolitesRatio
                            {
                                cycleId = cycle.id,
                                metabolite1 = first.id,
                                metabolite2 = second.id,
                                ratio = Math.Abs(sharedReactions[0].stoichiometry / second.ReactionSpecies.Single(rs => rs.reactionId == sharedReactions[0].reactionId).stoichiometry)
                            };
                            cache.Add(record);
                        }
                        else if (sharedReactions.Count == 0 && nestedRatios.Count == 1 && !cache.Any(ra => ra.metabolite1 == second.id && ra.metabolite2 == first.id))
                        {
                            Console.WriteLine($"recording: {first.sbmlId}, {second.sbmlId}");
                            var record = new cycleInterfaceMetabolitesRatio
                            {
                                cycleId = cycle.id,
                                metabolite1 = first.id,
                                metabolite2 = second.id,
                                ratio = nestedRatios[0].ratio
                            };
                            cache.Add(record);
                        }
                    }
                }
            }

            cache.ForEach(r => Db.Context.cycleInterfaceMetabolitesRatios.Add(r));
            Db.Context.SaveChanges();
        }

        private static void CheckCycleRatios(Guid cycleId)
        {
            var count = 0;
            var cycle = Db.Context.Cycles.Find(cycleId);

            var recordedRatios = new Dictionary<Guid, Dictionary<Guid, Dictionary<int, List<double>>>>();

            var nestedCycles = cycle.CycleReactions.Where(cr => !cr.isReaction).Select(cr => cr.otherId).ToList();

            var ratios =
                nestedCycles.SelectMany(
                    nc => Db.Context.cycleInterfaceMetabolitesRatios.Where(ci => ci.cycleId == nc)).ToList();

            var cReactions = cycle.CycleReactions.Select(cr => cr.otherId).ToList();
            var allSpecies =
                Db.Context.ReactionSpecies.Where(rs => cReactions.Contains(rs.reactionId))
                    .Select(rs => rs.speciesId).Distinct().ToList();
            allSpecies =
                allSpecies.Union(
                    Db.Context.CycleConnections.Where(cc => nestedCycles.Contains(cc.cycleId))
                        .Select(cc => cc.metaboliteId)).Distinct().ToList();

            foreach (var cycleConnection in cycle.CycleConnections)
            {
                var model = new Cplex { Name = "FBA" };
                var pseudoMetsVars = new Dictionary<Guid, INumVar>();
                var vars = new Dictionary<Guid, INumVar>();
                var cycleMets = new Dictionary<Tuple<Guid, Guid>, Guid>();
                var mainPseudoReaction = Guid.Empty;

                cycle.CycleReactions.Where(cr => cr.isReaction).ToList().ForEach(r =>
                {
                    var bound = Db.Context.ReactionBounds.Single(rb => rb.reactionId == r.otherId);
                    vars[r.otherId] = model.NumVar(bound.lowerBound, bound.upperBound, NumVarType.Float,
                        Db.Context.Reactions.Find(r.otherId).sbmlId);
                });

                Db.Context.CycleConnections.Where(
                    cc => allSpecies.Contains(cc.metaboliteId) && nestedCycles.Contains(cc.cycleId))
                    .ToList()
                    .ForEach(cc =>
                    {
                        cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)] = Guid.NewGuid();
                        vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]] =
                            model.NumVar(cc.isReversible ? -1000 : 0, 1000, cc.Species.sbmlId);
                    });

                allSpecies.ForEach(s =>
                {
                    var expr = model.LinearNumExpr();
                    Db.Context.ReactionSpecies.Where(
                        rs => rs.speciesId == s && cReactions.Contains(rs.reactionId))
                        .ToList()
                        .ForEach(rs =>
                        {
                            expr.AddTerm(rs.stoichiometry, vars[rs.reactionId]);
                        });
                    Db.Context.CycleConnections.Where(
                        cc => cc.metaboliteId == s && nestedCycles.Contains(cc.cycleId))
                        .ToList()
                        .ForEach(cc =>
                        {
                            expr.AddTerm(1, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                        });


                    // add atoms balance
                    // add atom numbers constraints to link metabolites of the cycle reaction
                    foreach (Guid nc in nestedCycles)
                    {
                        var carbonExpr = model.LinearNumExpr();
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "C");
                                    if (formula == null) return;

                                    carbonExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "C");
                                    if (formula == null) return;

                                    carbonExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        model.AddEq(carbonExpr, 0.0, nc + "_carbon_atoms_balance");

                        var hydrogenExpr = model.LinearNumExpr();
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "H");
                                    if (formula == null) return;

                                    hydrogenExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "H");
                                    if (formula == null) return;

                                    hydrogenExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        model.AddEq(hydrogenExpr, 0.0, nc + "_hydrogen_atoms_balance");

                        var nitrogenExpr = model.LinearNumExpr();
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "N");
                                    if (formula == null) return;

                                    nitrogenExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "N");
                                    if (formula == null) return;

                                    nitrogenExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        model.AddEq(nitrogenExpr, 0.0, nc + "_nitrogen_atoms_balance");

                        var oxygenExpr = model.LinearNumExpr();
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "O");
                                    if (formula == null) return;

                                    oxygenExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                            .ForEach(
                                cc =>
                                {
                                    var formula =
                                        Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "O");
                                    if (formula == null) return;

                                    oxygenExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                });
                        model.AddEq(oxygenExpr, 0.0, nc + "_oxygen_atoms_balance");
                    }


                    // add reversible pseudo reactions to every interface metabolite
                    if (cycle.CycleConnections.Any(cc => cc.metaboliteId == s))
                    {
                        var pseudo = Guid.NewGuid();
                        vars[pseudo] = model.NumVar(-1000, 1000, NumVarType.Float,
                            "pseudo_" + Db.Context.Species.Find(s).sbmlId);
                        pseudoMetsVars[s] = vars[pseudo];
                        expr.AddTerm(1, vars[pseudo]);

                        if (cycleConnection.metaboliteId == s) mainPseudoReaction = pseudo;
                    }
                    model.AddEq(expr, 0, Db.Context.Species.Find(s).sbmlId);
                });

                ratios.ForEach(ra =>
                {
                    var expr1 = model.LinearNumExpr();
                    expr1.AddTerm(1, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite1)]]);

                    var expr2Low = model.LinearNumExpr();
                    expr2Low.AddTerm(Math.Abs(ra.ratio) * 0.9, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]);

                    var expr2High = model.LinearNumExpr();
                    expr2High.AddTerm(Math.Abs(ra.ratio) * 1.1, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]);

                    var or = model.Or();

                    var low1 = model.Ge(model.Abs(expr1), model.Abs(expr2Low),
                        $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_low");
                    var high1 = model.Le(model.Abs(expr1), model.Abs(expr2High),
                        $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_high");
                    var withRatio1 = model.And();
                    withRatio1.Add(low1);
                    withRatio1.Add(high1);
                    or.Add(withRatio1);

                    var zeroLeft = model.Le(model.Abs(vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite1)]]), ZeroOutFlux);
                    var zeroRight = model.Le(model.Abs(vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]), ZeroOutFlux);

                    or.Add(zeroLeft);
                    or.Add(zeroRight);

                    model.Add(or);
                });


                // solving for producers
                var fluxConstraint = model.AddGe(vars[mainPseudoReaction], 500);

                var solved = model.Solve();
                count++;
                model.ExportModel($"{Core.Dir}{count}model_p.lp");
                if (solved)
                {



                    var list =
                        vars.ToList()
                            .Select(
                                v =>
                                    (Db.Context.Reactions.Find(v.Key) != null
                                        ? Db.Context.Reactions.Find(v.Key).sbmlId
                                        : v.Value.Name) + "    " + model.GetValue(v.Value));
                    File.WriteAllLines($"{Core.Dir}{count}result_p.txt", list);


                    cycle.CycleConnections.ToList().ForEach(cc =>
                    {
                        var first = 0.0;
                        foreach (
                            var rs in
                                Db.Context.ReactionSpecies.Where(
                                    rs => rs.speciesId == cc.metaboliteId && cReactions.Contains(rs.reactionId)))
                        {
                            if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) > ZeroOutFlux)
                            {
                                first += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                            }
                        }
                        first +=
                            nestedCycles.Where(
                                nc =>
                                    cycleMets.ContainsKey(Tuple.Create(nc, cc.metaboliteId)) &&
                                    model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]) > ZeroOutFlux)
                                .Sum(nc => model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]));
                        if (model.GetValue(pseudoMetsVars[cc.metaboliteId]) > ZeroOutFlux) {
                            first += model.GetValue(pseudoMetsVars[cc.metaboliteId]);
                        }


                        cycle.CycleConnections.ToList()
                            .Where(cc2 => cc2.metaboliteId != cc.metaboliteId)
                            .ToList()
                            .ForEach(cc2 =>
                            {
                                var second = 0.0;
                                foreach (
                                    var rs in
                                        Db.Context.ReactionSpecies.Where(
                                            rs =>
                                                rs.speciesId == cc2.metaboliteId &&
                                                cReactions.Contains(rs.reactionId))
                                    )
                                {
                                        // only pick producers
                                        if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) > ZeroOutFlux)
                                    {
                                        second += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                    }
                                }
                                second +=
                                    nestedCycles.Where(
                                        nc =>
                                            cycleMets.ContainsKey(Tuple.Create(nc, cc2.metaboliteId)) &&
                                            model.GetValue(
                                                vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]) > ZeroOutFlux)
                                        .Sum(
                                            nc =>
                                                model.GetValue(
                                                    vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]));
                                if (model.GetValue(pseudoMetsVars[cc2.metaboliteId]) > ZeroOutFlux)
                                {
                                    second += model.GetValue(pseudoMetsVars[cc2.metaboliteId]);
                                }

                                first = Math.Abs(first);
                                second = Math.Abs(second);

                                if (
                                    String.Compare(cc.Species.sbmlId, cc2.Species.sbmlId, StringComparison.Ordinal) <
                                    0)
                                {
                                    if (!recordedRatios.ContainsKey(cc.metaboliteId))
                                        recordedRatios[cc.metaboliteId] = new Dictionary<Guid, Dictionary<int, List<double>>>();
                                    if (!recordedRatios[cc.metaboliteId].ContainsKey(cc2.metaboliteId))
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId] = new Dictionary<int, List<double>>();
                                    if (!recordedRatios[cc.metaboliteId][cc2.metaboliteId].ContainsKey(count))
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId][count] = new List<double>();
                                    recordedRatios[cc.metaboliteId][cc2.metaboliteId][count].Add(first / second);
                                }
                                else
                                {
                                    if (!recordedRatios.ContainsKey(cc2.metaboliteId))
                                        recordedRatios[cc2.metaboliteId] = new Dictionary<Guid, Dictionary<int, List<double>>>();
                                    if (!recordedRatios[cc2.metaboliteId].ContainsKey(cc.metaboliteId))
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId] = new Dictionary<int, List<double>>();
                                    if (!recordedRatios[cc2.metaboliteId][cc.metaboliteId].ContainsKey(count))
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId][count] = new List<double>();
                                    recordedRatios[cc2.metaboliteId][cc.metaboliteId][count].Add(second / first);
                                }
                            });
                    });
                }


                // solving for consumers
                model.Remove(fluxConstraint);
                fluxConstraint = model.AddLe(vars[mainPseudoReaction], -500);

                solved = model.Solve();
                model.ExportModel($"{Core.Dir}{count}model_n.lp");
                if (solved)
                {
                    var list =
                        vars.ToList()
                            .Select(
                                v =>
                                    (Db.Context.Reactions.Find(v.Key) != null
                                        ? Db.Context.Reactions.Find(v.Key).sbmlId
                                        : v.Value.Name) + "    " + model.GetValue(v.Value));
                    File.WriteAllLines($"{Core.Dir}{count}result_n.txt", list);


                    cycle.CycleConnections.ToList().ForEach(cc =>
                    {
                        var first = 0.0;
                        foreach (
                            var rs in
                                Db.Context.ReactionSpecies.Where(
                                    rs => rs.speciesId == cc.metaboliteId && cReactions.Contains(rs.reactionId)))
                        {
                            if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) < -ZeroOutFlux)
                            {
                                first += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                            }
                        }
                        first +=
                            nestedCycles.Where(
                                nc =>
                                    cycleMets.ContainsKey(Tuple.Create(nc, cc.metaboliteId)) &&
                                    model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]) < -ZeroOutFlux)
                                .Sum(nc => model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]));
                        if (model.GetValue(pseudoMetsVars[cc.metaboliteId]) < -ZeroOutFlux)
                        {
                            first += model.GetValue(pseudoMetsVars[cc.metaboliteId]);
                        }


                        cycle.CycleConnections.ToList()
                            .Where(cc2 => cc2.metaboliteId != cc.metaboliteId)
                            .ToList()
                            .ForEach(cc2 =>
                            {
                                var second = 0.0;
                                foreach (
                                    var rs in
                                        Db.Context.ReactionSpecies.Where(
                                            rs =>
                                                rs.speciesId == cc2.metaboliteId &&
                                                cReactions.Contains(rs.reactionId))
                                    )
                                {
                                        // only pick consumers
                                        if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) < -ZeroOutFlux)
                                    {
                                        second += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                    }
                                }
                                second +=
                                    nestedCycles.Where(
                                        nc =>
                                            cycleMets.ContainsKey(Tuple.Create(nc, cc2.metaboliteId)) &&
                                            model.GetValue(
                                                vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]) < -ZeroOutFlux)
                                        .Sum(
                                            nc =>
                                                model.GetValue(
                                                    vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]));
                                if (model.GetValue(pseudoMetsVars[cc2.metaboliteId]) < -ZeroOutFlux)
                                {
                                    second += model.GetValue(pseudoMetsVars[cc2.metaboliteId]);
                                }

                                first = Math.Abs(first);
                                second = Math.Abs(second);

                                if (
                                    String.Compare(cc.Species.sbmlId, cc2.Species.sbmlId, StringComparison.Ordinal) <
                                    0)
                                {
                                    if (!recordedRatios.ContainsKey(cc.metaboliteId))
                                        recordedRatios[cc.metaboliteId] = new Dictionary<Guid, Dictionary<int, List<double>>>();
                                    if (!recordedRatios[cc.metaboliteId].ContainsKey(cc2.metaboliteId))
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId] = new Dictionary<int, List<double>>();
                                    if (!recordedRatios[cc.metaboliteId][cc2.metaboliteId].ContainsKey(count))
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId][count] = new List<double>();
                                    recordedRatios[cc.metaboliteId][cc2.metaboliteId][count].Add(first / second);
                                }
                                else
                                {
                                    if (!recordedRatios.ContainsKey(cc2.metaboliteId))
                                        recordedRatios[cc2.metaboliteId] = new Dictionary<Guid, Dictionary<int, List<double>>>();
                                    if (!recordedRatios[cc2.metaboliteId].ContainsKey(cc.metaboliteId))
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId] = new Dictionary<int, List<double>>();
                                    if (!recordedRatios[cc2.metaboliteId][cc.metaboliteId].ContainsKey(count))
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId][count] = new List<double>();
                                    recordedRatios[cc2.metaboliteId][cc.metaboliteId][count].Add(second / first);
                                }
                            });
                    });
                }
            }

            recordedRatios.ToList().ForEach(f => f.Value.ToList().ForEach(s =>
            {
                var values = s.Value.Values.SelectMany(v => v).Where(v => Math.Abs(v) > 0 && !double.IsInfinity(v)).ToList();
                var lists = s.Value.Values.Where(v => v.Any(e => Math.Abs(e) > 0 && !double.IsInfinity(e))).ToList();
                Console.WriteLine("List of ratios of " + Db.Context.Species.Find(f.Key).sbmlId + " .. " +
                                  Db.Context.Species.Find(s.Key).sbmlId);
                s.Value.ToList().ForEach(i => Console.Write(i.Key + ": " + i.Value[0] + (i.Value.Count > 2 ? " n:" + i.Value[2] : "") + " | "));
                //values.ForEach(v => Console.Write(v + ", "));
                Console.WriteLine();

                if (lists.Count > 1 &&
                    values.All(v => Math.Round(Math.Abs(Math.Abs(v) - Math.Abs(values[0])), 2) < double.Epsilon))
                {
                    Console.WriteLine("111111111111111111111111111   " + Db.Context.Species.Find(f.Key).sbmlId +
                                      " " + Db.Context.Species.Find(s.Key).sbmlId + " " + values[0]);
                }
                else
                {
                    Console.WriteLine("didn't :(");
                }
            }));

            Console.ReadKey();
        }

        private static IEnumerable<Guid> GetAllReactionsOfCycle(Guid cycleId)
        {
            var cycle = Db.Context.Cycles.Find(cycleId);
            if (cycle.CycleReactions.All(cr => cr.isReaction))
            {
                return cycle.CycleReactions.Select(cr => cr.otherId);
            }

            var realReactions = cycle.CycleReactions.Where(cr => cr.isReaction).Select(cr => cr.otherId);
            var nestedReactions =
                cycle.CycleReactions.Where(cr => !cr.isReaction)
                    .Select(cr => cr.otherId)
                    .SelectMany(GetAllReactionsOfCycle);

            return realReactions.Union(nestedReactions);
        }

        private static List<Cycle> TopologicalSortCycles(List<Cycle> cycles)
        {
            var sortedCycles = new List<Cycle>();
            var tempCycles = cycles.Select(c => c.id).ToList();
            while (tempCycles.Any())
            {
                var level = cycles.Where(c => 
                    tempCycles.Contains(c.id) && (
                        c.CycleReactions.All(cr => cr.isReaction) ||
                        c.CycleReactions.Where(cr => !cr.isReaction).All(nc => sortedCycles.Select(sc => sc.id).Contains(nc.otherId))
                    )
                 ).ToList();
                tempCycles.RemoveAll(tc => level.Select(lc => lc.id).Contains(tc));
                sortedCycles.AddRange(level);
            }

            Console.WriteLine("Topologically sorted cycles:");
            foreach (var sortedCycle in sortedCycles)
            {
                Console.WriteLine("  cycle: " + sortedCycle.id);
            }
            return sortedCycles;
        }

        private static void RecordCyclesInterfaceMetabolitesRations()
        {
            Db.Context.cycleInterfaceMetabolitesRatios.RemoveRange(Db.Context.cycleInterfaceMetabolitesRatios.Where(e => true));
            Db.Context.SaveChanges();

            int count = 0;

            var sortedCycles = TopologicalSortCycles(Db.Context.Cycles.ToList());
            foreach (var cycle in sortedCycles)
            {
                var recordedRatios = new Dictionary<Guid, Dictionary<Guid, List<double>>>();

                var nestedCycles = cycle.CycleReactions.Where(cr => !cr.isReaction).Select(cr => cr.otherId).ToList();

                var ratios =
                    nestedCycles.SelectMany(
                        nc => Db.Context.cycleInterfaceMetabolitesRatios.Where(ci => ci.cycleId == nc)).ToList();

                var cReactions = cycle.CycleReactions.Select(cr => cr.otherId).ToList();
                var allSpecies =
                    Db.Context.ReactionSpecies.Where(rs => cReactions.Contains(rs.reactionId))
                        .Select(rs => rs.speciesId).Distinct().ToList();
                allSpecies =
                    allSpecies.Union(
                        Db.Context.CycleConnections.Where(cc => nestedCycles.Contains(cc.cycleId))
                            .Select(cc => cc.metaboliteId)).Distinct().ToList();

                foreach (var cycleConnection in cycle.CycleConnections)
                {
                    var model = new Cplex {Name = "FBA"};
                    var vars = new Dictionary<Guid, INumVar>();
                    var pseudoMetsVars = new Dictionary<Guid, INumVar>();
                    var cycleMets = new Dictionary<Tuple<Guid, Guid>, Guid>();
                    var mainPseudoReaction = Guid.Empty;

                    cycle.CycleReactions.Where(cr => cr.isReaction).ToList().ForEach(r =>
                    {
                        var bound = Db.Context.ReactionBounds.Single(rb => rb.reactionId == r.otherId);
                        vars[r.otherId] = model.NumVar(bound.lowerBound, bound.upperBound, NumVarType.Float,
                            Db.Context.Reactions.Find(r.otherId).sbmlId);
                    });

                    Db.Context.CycleConnections.Where(
                        cc => allSpecies.Contains(cc.metaboliteId) && nestedCycles.Contains(cc.cycleId))
                        .ToList()
                        .ForEach(cc =>
                        {
                            cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)] = Guid.NewGuid();
                            vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]] =
                                model.NumVar(cc.isReversible ? -1000 : 0, 1000, cc.Species.sbmlId);
                        });

                    allSpecies.ForEach(s =>
                    {
                        var expr = model.LinearNumExpr();
                        Db.Context.ReactionSpecies.Where(
                            rs => rs.speciesId == s && cReactions.Contains(rs.reactionId))
                            .ToList()
                            .ForEach(rs =>
                            {
                                expr.AddTerm(rs.stoichiometry, vars[rs.reactionId]);
                            });
                        Db.Context.CycleConnections.Where(
                            cc => cc.metaboliteId == s && nestedCycles.Contains(cc.cycleId))
                            .ToList()
                            .ForEach(cc =>
                            {
                                expr.AddTerm(1, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                            });


                        // add atoms balance
                        // add atom numbers constraints to link metabolites of the cycle reaction
                        foreach (Guid nc in nestedCycles)
                        {
                            var carbonExpr = model.LinearNumExpr();
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "C");
                                        if (formula == null) return;

                                        carbonExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "C");
                                        if (formula == null) return;

                                        carbonExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            model.AddEq(carbonExpr, 0.0, nc + "_carbon_atoms_balance");

                            var hydrogenExpr = model.LinearNumExpr();
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "H");
                                        if (formula == null) return;

                                        hydrogenExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "H");
                                        if (formula == null) return;

                                        hydrogenExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            model.AddEq(hydrogenExpr, 0.0, nc + "_hydrogen_atoms_balance");

                            var nitrogenExpr = model.LinearNumExpr();
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "N");
                                        if (formula == null) return;

                                        nitrogenExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "N");
                                        if (formula == null) return;

                                        nitrogenExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            model.AddEq(nitrogenExpr, 0.0, nc + "_nitrogen_atoms_balance");

                            var oxygenExpr = model.LinearNumExpr();
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && (cc.roleId == Db.ProductId || cc.roleId == Db.ReversibleId)).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "O");
                                        if (formula == null) return;

                                        oxygenExpr.AddTerm(formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            Db.Context.CycleConnections.Where(cc => cc.cycleId == nc && cc.roleId == Db.ReactantId).ToList()
                                .ForEach(
                                    cc =>
                                    {
                                        var formula =
                                            Db.Context.Formulae.SingleOrDefault(f => f.speciesId == cc.metaboliteId && f.atom == "O");
                                        if (formula == null) return;

                                        oxygenExpr.AddTerm(-formula.numAtoms, vars[cycleMets[Tuple.Create(cc.cycleId, cc.metaboliteId)]]);
                                    });
                            model.AddEq(oxygenExpr, 0.0, nc + "_oxygen_atoms_balance");
                        }


                        // add reversible pseudo reactions to every interface metabolite
                        if (cycle.CycleConnections.Any(cc => cc.metaboliteId == s))
                        {
                            var pseudo = Guid.NewGuid();
                            vars[pseudo] = model.NumVar(-1000, 1000, NumVarType.Float,
                                "pseudo" + Db.Context.Species.Find(s).sbmlId);
                            pseudoMetsVars[s] = vars[pseudo];
                            expr.AddTerm(1, vars[pseudo]);

                            if (cycleConnection.metaboliteId == s) mainPseudoReaction = pseudo;
                        }
                        model.AddEq(expr, 0, Db.Context.Species.Find(s).sbmlId);
                    });

                    ratios.ForEach(ra =>
                    {
                        var expr1 = model.LinearNumExpr();
                        expr1.AddTerm(1, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite1)]]);

                        var expr2Low = model.LinearNumExpr();
                        expr2Low.AddTerm(Math.Abs(ra.ratio) * 0.9, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]);

                        var expr2High = model.LinearNumExpr();
                        expr2High.AddTerm(Math.Abs(ra.ratio) * 1.1, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]);

                        var or = model.Or();

                        var low1 = model.Ge(model.Abs(expr1), model.Abs(expr2Low),
                            $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_low");
                        var high1 = model.Le(model.Abs(expr1), model.Abs(expr2High),
                            $"{ra.Species.sbmlId}_{ra.Species1.sbmlId}_ratio_high");
                        var withRatio1 = model.And();
                        withRatio1.Add(low1);
                        withRatio1.Add(high1);
                        or.Add(withRatio1);

                        var zeroLeft = model.Le(model.Abs(vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite1)]]), ZeroOutFlux);
                        var zeroRight = model.Le(model.Abs(vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]), ZeroOutFlux);

                        or.Add(zeroLeft);
                        or.Add(zeroRight);

                        model.Add(or);
                    });


                    // solving for producers
                    var fluxConstraint = model.AddGe(vars[mainPseudoReaction], 500);

                    var solved = model.Solve();
                    count++;
                    model.ExportModel($"{Core.Dir}{count}model_p.lp");
                    if (solved)
                    {



                        var list =
                            vars.ToList()
                                .Select(
                                    v =>
                                        (Db.Context.Reactions.Find(v.Key) != null
                                            ? Db.Context.Reactions.Find(v.Key).sbmlId
                                            : v.Value.Name) + "    " + model.GetValue(v.Value));
                        File.WriteAllLines($"{Core.Dir}{count}result_p.txt", list);


                        cycle.CycleConnections.ToList().ForEach(cc =>
                        {
                            var first = 0.0;
                            foreach (
                                var rs in
                                    Db.Context.ReactionSpecies.Where(
                                        rs => rs.speciesId == cc.metaboliteId && cReactions.Contains(rs.reactionId)))
                            {
                                if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) > ZeroOutFlux)
                                {
                                    first += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                }
                            }
                            first +=
                                nestedCycles.Where(
                                    nc =>
                                        cycleMets.ContainsKey(Tuple.Create(nc, cc.metaboliteId)) &&
                                        model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]) > ZeroOutFlux)
                                    .Sum(nc => model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]));
                            if (model.GetValue(pseudoMetsVars[cc.metaboliteId]) > ZeroOutFlux)
                            {
                                first += model.GetValue(pseudoMetsVars[cc.metaboliteId]);
                            }


                            cycle.CycleConnections.ToList()
                                .Where(cc2 => cc2.metaboliteId != cc.metaboliteId)
                                .ToList()
                                .ForEach(cc2 =>
                                {
                                    var second = 0.0;
                                    foreach (
                                        var rs in
                                            Db.Context.ReactionSpecies.Where(
                                                rs =>
                                                    rs.speciesId == cc2.metaboliteId &&
                                                    cReactions.Contains(rs.reactionId))
                                        )
                                    {
                                        // only pick producers
                                        if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) > ZeroOutFlux)
                                        {
                                            second += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                        }
                                    }
                                    second +=
                                        nestedCycles.Where(
                                            nc =>
                                                cycleMets.ContainsKey(Tuple.Create(nc, cc2.metaboliteId)) &&
                                                model.GetValue(
                                                    vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]) > ZeroOutFlux)
                                            .Sum(
                                                nc =>
                                                    model.GetValue(
                                                        vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]));
                                    if (model.GetValue(pseudoMetsVars[cc2.metaboliteId]) > ZeroOutFlux)
                                    {
                                        second += model.GetValue(pseudoMetsVars[cc2.metaboliteId]);
                                    }

                                    first = Math.Abs(first);
                                    second = Math.Abs(second);

                                    if (
                                        String.Compare(cc.Species.sbmlId, cc2.Species.sbmlId, StringComparison.Ordinal) <
                                        0)
                                    {
                                        //Console.Write(cc.Species.sbmlId + " " + cc2.Species.sbmlId + ", ");
                                        if (!recordedRatios.ContainsKey(cc.metaboliteId))
                                            recordedRatios[cc.metaboliteId] = new Dictionary<Guid, List<double>>();
                                        if (!recordedRatios[cc.metaboliteId].ContainsKey(cc2.metaboliteId))
                                            recordedRatios[cc.metaboliteId][cc2.metaboliteId] = new List<double>();
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId].Add(first/second);
                                    }
                                    else
                                    {
                                        //Console.Write(cc2.Species.sbmlId + " " + cc.Species.sbmlId + ", ");
                                        if (!recordedRatios.ContainsKey(cc2.metaboliteId))
                                            recordedRatios[cc2.metaboliteId] = new Dictionary<Guid, List<double>>();
                                        if (!recordedRatios[cc2.metaboliteId].ContainsKey(cc.metaboliteId))
                                            recordedRatios[cc2.metaboliteId][cc.metaboliteId] = new List<double>();
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId].Add(second/first);
                                    }
                                });
                        });
                    }


                    // solving for consumers
                    model.Remove(fluxConstraint);
                    fluxConstraint = model.AddLe(vars[mainPseudoReaction], -500);

                    solved = model.Solve();
                    model.ExportModel($"{Core.Dir}{count}model_n.lp");
                    if (solved)
                    {
                        var list =
                            vars.ToList()
                                .Select(
                                    v =>
                                        (Db.Context.Reactions.Find(v.Key) != null
                                            ? Db.Context.Reactions.Find(v.Key).sbmlId
                                            : v.Value.Name) + "    " + model.GetValue(v.Value));
                        File.WriteAllLines($"{Core.Dir}{count}result_n.txt", list);


                        cycle.CycleConnections.ToList().ForEach(cc =>
                        {
                            var first = 0.0;
                            foreach (
                                var rs in
                                    Db.Context.ReactionSpecies.Where(
                                        rs => rs.speciesId == cc.metaboliteId && cReactions.Contains(rs.reactionId)))
                            {
                                if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) < -ZeroOutFlux)
                                {
                                    first += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                }
                            }
                            first +=
                                nestedCycles.Where(
                                    nc =>
                                        cycleMets.ContainsKey(Tuple.Create(nc, cc.metaboliteId)) &&
                                        model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]) < -ZeroOutFlux)
                                    .Sum(nc => model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]));
                            if (model.GetValue(pseudoMetsVars[cc.metaboliteId]) < -ZeroOutFlux)
                            {
                                first += model.GetValue(pseudoMetsVars[cc.metaboliteId]);
                            }


                            cycle.CycleConnections.ToList()
                                .Where(cc2 => cc2.metaboliteId != cc.metaboliteId)
                                .ToList()
                                .ForEach(cc2 =>
                                {
                                    var second = 0.0;
                                    foreach (
                                        var rs in
                                            Db.Context.ReactionSpecies.Where(
                                                rs =>
                                                    rs.speciesId == cc2.metaboliteId &&
                                                    cReactions.Contains(rs.reactionId))
                                        )
                                    {
                                        // only pick consumers
                                        if (rs.stoichiometry * model.GetValue(vars[rs.reactionId]) < -ZeroOutFlux)
                                        {
                                            second += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                        }
                                    }
                                    second +=
                                        nestedCycles.Where(
                                            nc =>
                                                cycleMets.ContainsKey(Tuple.Create(nc, cc2.metaboliteId)) &&
                                                model.GetValue(
                                                    vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]) < -ZeroOutFlux)
                                            .Sum(
                                                nc =>
                                                    model.GetValue(
                                                        vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]));
                                    if (model.GetValue(pseudoMetsVars[cc2.metaboliteId]) < -ZeroOutFlux)
                                    {
                                        second += model.GetValue(pseudoMetsVars[cc2.metaboliteId]);
                                    }


                                    first = Math.Abs(first);
                                    second = Math.Abs(second);

                                    if (
                                        String.Compare(cc.Species.sbmlId, cc2.Species.sbmlId, StringComparison.Ordinal) <
                                        0)
                                    {
                                        if (!recordedRatios.ContainsKey(cc.metaboliteId))
                                            recordedRatios[cc.metaboliteId] = new Dictionary<Guid, List<double>>();
                                        if (!recordedRatios[cc.metaboliteId].ContainsKey(cc2.metaboliteId))
                                            recordedRatios[cc.metaboliteId][cc2.metaboliteId] = new List<double>();
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId].Add(first / second);
                                    }
                                    else
                                    {
                                        if (!recordedRatios.ContainsKey(cc2.metaboliteId))
                                            recordedRatios[cc2.metaboliteId] = new Dictionary<Guid, List<double>>();
                                        if (!recordedRatios[cc2.metaboliteId].ContainsKey(cc.metaboliteId))
                                            recordedRatios[cc2.metaboliteId][cc.metaboliteId] = new List<double>();
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId].Add(second / first);
                                    }
                                });
                        });
                    }
                }

                //File.WriteAllLines()
                recordedRatios.ToList().ForEach(f => f.Value.ToList().ForEach(s =>
                {
                    var values = s.Value.Where(v => Math.Abs(v) > 0 && !double.IsInfinity(v)).ToList();
                    Console.WriteLine("List of ratios of " + Db.Context.Species.Find(f.Key).sbmlId + " .. " +
                                      Db.Context.Species.Find(s.Key).sbmlId);
                    values.ForEach(v => Console.Write(v + ", "));
                    Console.WriteLine();

                    if (values.Count > 2 &&
                        values.All(v => Math.Round(Math.Abs(Math.Abs(v) - Math.Abs(values[0])), 2) < double.Epsilon))
                    {
                        Console.WriteLine("111111111111111111111111111   " + Db.Context.Species.Find(f.Key).sbmlId +
                                          " " + Db.Context.Species.Find(s.Key).sbmlId + " " + values[0]);
                        var record = new DB.cycleInterfaceMetabolitesRatio
                        {
                            cycleId = cycle.id,
                            metabolite1 = f.Key,
                            metabolite2 = s.Key,
                            ratio = Math.Abs(values[0])
                        };
                        Db.Context.cycleInterfaceMetabolitesRatios.Add(record);
                        Db.Context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("didn't :(");
                    }
                }));
            }

            Db.Context.SaveChanges();
            Console.WriteLine("Recorded all");
            Console.ReadKey();
        }

        private static void LimitReactionsFluxes()
        {
            Db.Context.ReactionBoundFixes.RemoveRange(Db.Context.ReactionBoundFixes.Where(e => true));
            Db.Context.SaveChanges();

            foreach (var reaction in Db.Context.Reactions.Where(r => r.sbmlId.StartsWith("EX_") && !r.reversible))
            {
                var rs = reaction.ReactionSpecies.Single();
                var lb = reaction.ReactionBounds.Single().lowerBound * Math.Abs(rs.stoichiometry);
                var ub = reaction.ReactionBounds.Single().upperBound * Math.Abs(rs.stoichiometry);

                var s = rs.Species;

                while (true)
                {
                    if (s.ReactionSpecies.Count != 2) break;
                    // ---> O --->
                    rs = s.ReactionSpecies.Single(e => e.id != rs.id);
                    var r = rs.Reaction;

                    if (s.ReactionSpecies.All(e => e.roleId == s.ReactionSpecies.First().roleId))
                    {
                        if (!r.reversible)
                        {
                            Console.WriteLine(s.sbmlId + " " + s.id);
                            r.ReactionBoundFix = new ReactionBoundFix
                            {
                                lowerbound = 0,
                                upperbound = 0
                            };
                        }
                        else
                        {
                            r.ReactionBoundFix = new ReactionBoundFix
                            {
                                lowerbound = -ub/Math.Abs(rs.stoichiometry),
                                upperbound = -lb/Math.Abs(rs.stoichiometry)
                            };
                        }
                    }
                    else
                    {
                        r.ReactionBoundFix = new ReactionBoundFix
                        {
                            lowerbound = lb / Math.Abs(rs.stoichiometry),
                            upperbound = ub / Math.Abs(rs.stoichiometry)
                        };
                    }

                    Db.Context.SaveChanges();

                    if (r.ReactionSpecies.Count != 2) break;
                    // ---> [] --->
                    rs = r.ReactionSpecies.Single(e => e.id != rs.id);
                    s = rs.Species;
                }
            }

            Console.ReadKey();
        }

        private static void CheckExchangeReactionsInCycles()
        {
            foreach (var cycle in Db.Context.Cycles)
            {
                foreach (var cycleReaction in cycle.CycleReactions.Where(cr => cr.isReaction))
                {
                    var r = Db.Context.Reactions.Find(cycleReaction.otherId);
                    if (r.sbmlId.Contains("Biomass")) Console.WriteLine(cycle.id + " " + r.sbmlId);
                }
            }
            Console.ReadKey();
        }

        private static void ValidateReactionsAtomsBalance()
        {
            foreach (var reaction in Db.Context.Reactions.Where(r => ! r.sbmlId.Contains("EX_")))
            {
                var hydrogens = 0.0;
                var carbon = 0.0;
                var nitrogen = 0.0;
                var oxygen = 0.0;

                foreach (var reactionSpecy in reaction.ReactionSpecies)
                {
                    if (reactionSpecy.roleId == Db.ProductId)
                    {
                        var formulae =
                            Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "H");
                        if (formulae != null) hydrogens += reactionSpecy.stoichiometry * formulae.numAtoms;

                        formulae = Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "C");
                        if (formulae != null) carbon += reactionSpecy.stoichiometry * formulae.numAtoms;

                        formulae = Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "N");
                        if (formulae != null) nitrogen += reactionSpecy.stoichiometry * formulae.numAtoms;

                        formulae = Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "O");
                        if (formulae != null) oxygen += reactionSpecy.stoichiometry * formulae.numAtoms;
                    }
                    else if (reactionSpecy.roleId == Db.ReactantId)
                    {
                        var formulae =
                            Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "H");
                        if (formulae != null) hydrogens += reactionSpecy.stoichiometry * formulae.numAtoms;

                        formulae = Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "C");
                        if (formulae != null) carbon += reactionSpecy.stoichiometry * formulae.numAtoms;

                        formulae = Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "N");
                        if (formulae != null) nitrogen += reactionSpecy.stoichiometry * formulae.numAtoms;

                        formulae = Db.Context.Formulae.SingleOrDefault(s => s.speciesId == reactionSpecy.speciesId && s.atom == "O");
                        if (formulae != null) oxygen += reactionSpecy.stoichiometry * formulae.numAtoms;
                    }
                }

                if (hydrogens != 0) Console.WriteLine(reaction.sbmlId + " remaining hydrogen: " + hydrogens);
                if (carbon != 0) Console.WriteLine(reaction.sbmlId + " remaining carbon: " + carbon);
                if (nitrogen != 0) Console.WriteLine(reaction.sbmlId + " remaining nitrogen: " + nitrogen);
                if (oxygen != 0) Console.WriteLine(reaction.sbmlId + " remaining oxygen: " + oxygen);
                if (hydrogens + carbon + nitrogen + oxygen != 0) Console.WriteLine();
            }
            Console.ReadKey();
        }

        static void EraseAndRecordToDb()
        {
            Console.WriteLine("WARNING: This is going to erase the whole cycle database and record it from scratch, and this is going to take a lot of time");
            Console.WriteLine("Press any key if you are sure you want to continue ...");
            Console.ReadKey();

            const int outlier = 61;

            var g = ConstructHyperGraphFromSpecies(Db.Context.Species.Where(s => s.ReactionSpecies.Count < outlier));

            Console.WriteLine("loaded the whole network");



            // delete all entries from DB
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleReaction");
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleConnection");
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleInterfaceMetabolitesRatios");
            Db.Context.Database.ExecuteSqlCommand("DELETE FROM Cycle");
            Console.WriteLine("deleted DB entries");

            Dictionary<HyperGraph.Cycle, List<HyperGraph.Entity>> cycles = DFS.DetectAndCollapseCycles(g);


            //foreach (var cycle in cycles)
            //{
            //    RecordToDatabase(cycle.Key, cycle.Value);
            //}

            Console.WriteLine("finished saving to DB");
            Console.ReadKey();
        }

        public static void RecordToDatabase(HyperGraph.Cycle cycleReaction, List<HyperGraph.Entity> cycle)
        {
            Guid cycleId = cycleReaction.Id;
            Db.Context.Database.ExecuteSqlCommand("INSERT INTO Cycle VALUES (@p0)", cycleId);


            foreach (HyperGraph.Edge reaction in cycle.OfType<HyperGraph.Edge>())
            {
                Db.Context.Database.ExecuteSqlCommand(
                    "INSERT INTO CycleReaction(cycleId, otherId, isReaction) VALUES (@p0, @p1, @p2)", cycleId,
                    reaction.Id, !(reaction is HyperGraph.Cycle));
            }

            // recording metabolites
            var reversibleMetabolites = cycleReaction.Reactants.Intersect(cycleReaction.Products);
            var reactants = cycleReaction.Reactants.Except(reversibleMetabolites);
            var products = cycleReaction.Products.Except(reversibleMetabolites);

            foreach (var reaction in cycleReaction.InterfaceReactions.Values)
            {
                if (reaction.IsReversible)
                {
                    reversibleMetabolites = reversibleMetabolites.Union(reaction.Reactants).Union(reaction.Products);
                }
                else
                {
                    reactants = reactants.Union(reaction.Reactants);
                    products = products.Union(reaction.Products);
                }
            }

            foreach (var reversibleMetabolite in reversibleMetabolites)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reversibleMetabolite.Key, roleId = Db.ReactantId, stoichiometry = reversibleMetabolite.Value.Weights[cycleId], isReversible = true });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, reversibleMetabolite.Key, Db.ReversibleId, 1, true);
            }
            foreach (var reactant in reactants)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = reactant.Key, roleId = Db.ReactantId, stoichiometry = reactant.Value.Weights[cycleId], isReversible = false });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, reactant.Key, Db.ReactantId, 1, false);
            }
            foreach (var product in products)
            {
                //                cycleModel.CycleConnections.Add(new CycleConnection() { cycleId = cycleId, metaboliteId = product.Key, roleId = Db.ProductId, stoichiometry = product.Value.Weights[cycleId], isReversible = false });
                Db.Context.Database.ExecuteSqlCommand("INSERT INTO CycleConnection (cycleId, metaboliteId, roleId, stoichiometry, isReversible) VALUES (@p0, @p1, @p2, @p3, @p4)", cycleId, product.Key, Db.ProductId, 1, false);

            }
        }

        static HyperGraph ConstructHyperGraphFromSpecies(IEnumerable<Species> species)
        {
            var g = new HyperGraph();
            foreach (var s in species)
            {
                g.AddSpeciesWithConnections(s);
            }
            return g;
        }

        private static void RecordJsonToDb()
        {
            using (
                StreamReader streamReader =
                    new StreamReader("C:\\Users\\sharno\\Dropbox\\Metabolomics\\Data\\ecoli_core.json"))
            {
                string json = streamReader.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);

                EcoliCoreModel model = Db.Context;

                // remove cycles first
                model.CycleConnections.RemoveRange(model.CycleConnections.Where(e => true));
                model.CycleReactions.RemoveRange(model.CycleReactions.Where(e => true));
                model.Cycles.RemoveRange(model.Cycles.Where(e => true));

                // remove everything else
                model.ReactionSpeciesRoles.RemoveRange(model.ReactionSpeciesRoles.Where(e => true));
                model.ReactionSpecies.RemoveRange(model.ReactionSpecies.Where(e => true));
                model.ReactionBounds.RemoveRange(model.ReactionBounds.Where(e => true));
                model.Reactions.RemoveRange(model.Reactions.Where(e => true));
                model.Species.RemoveRange(model.Species.Where(e => true));
                model.Compartments.RemoveRange(model.Compartments.Where(e => true));
                model.Models.RemoveRange(model.Models.Where(e => true));
                model.Sbases.RemoveRange(model.Sbases.Where(e => true));

                model.SaveChanges();

                Model ecoliModel = new Model();
                ecoliModel.id = Guid.NewGuid();
                ecoliModel.sbmlId = items.id;
                ecoliModel.sbmlFile = "ecoli_core.json";

                ecoliModel.Sbase = new Sbase();
                ecoliModel.Sbase.id = Guid.NewGuid();

                model.Models.Add(ecoliModel);

                {
                    ReactionSpeciesRole reactantRole = new ReactionSpeciesRole();
                    reactantRole.id = Db.ReactantId;
                    reactantRole.role = "Reactant";
                    model.ReactionSpeciesRoles.Add(reactantRole);

                    ReactionSpeciesRole productRole = new ReactionSpeciesRole();
                    productRole.id = Db.ProductId;
                    productRole.role = "Product";
                    model.ReactionSpeciesRoles.Add(productRole);
                }

                foreach (var c in items.compartments)
                {
                    Compartment compartment = new Compartment();
                    compartment.id = Guid.NewGuid();
                    compartment.sbmlId = c.Name;
                    compartment.name = c.Last;
                    compartment.Model = ecoliModel;

                    compartment.Sbase = new Sbase();
                    compartment.Sbase.id = Guid.NewGuid();

                    model.Compartments.Add(compartment);
                }
                model.SaveChanges();


                foreach (var m in items.metabolites)
                {
                    Species species = new Species();
                    species.id = Guid.NewGuid();
                    species.sbmlId = m.id;
                    species.name = m.name;
                    species.charge = m.charge;

                    species.modelId = ecoliModel.id;
                    species.Sbase = new Sbase();
                    species.Sbase.id = Guid.NewGuid();

                    string compartmentSbmlId = m.compartment;
                    species.Compartment = model.Compartments.First(e => e.sbmlId == compartmentSbmlId);

                    model.Species.Add(species);
                }
                model.SaveChanges();


                foreach (var r in items.reactions)
                {
                    Reaction reaction = new Reaction();
                    reaction.id = Guid.NewGuid();
                    reaction.sbmlId = r.id;
                    reaction.name = r.name;

                    reaction.modelId = ecoliModel.id;
                    reaction.Sbase = new Sbase();
                    reaction.Sbase.id = Guid.NewGuid();

                    ReactionBound reactionBound = new ReactionBound();
                    reactionBound.id = Guid.NewGuid();
                    reactionBound.reactionId = reaction.id;
                    reactionBound.upperBound = r.upper_bound;
                    reactionBound.lowerBound = r.lower_bound;
                    reaction.ReactionBounds.Add(reactionBound);

                    if (reactionBound.lowerBound < 0)
                    {
                        reaction.reversible = true;
                    }
                    else
                    {
                        reaction.reversible = false;
                    }

                    foreach (var m in r.metabolites)
                    {
                        string metaboliteSmblId = m.Name;
                        Species metabolite = model.Species.First(s => s.sbmlId == metaboliteSmblId);

                        ReactionSpecy reactionSpecy = new ReactionSpecy();
                        reactionSpecy.id = Guid.NewGuid();
                        reactionSpecy.reactionId = reaction.id;
                        reactionSpecy.speciesId = metabolite.id;
                        reactionSpecy.stoichiometry = m.Last;
                        if (reactionSpecy.stoichiometry > 0)
                        {
                            reactionSpecy.roleId = Db.ProductId;
                        }
                        else
                        {
                            reactionSpecy.roleId = Db.ReactantId;
                        }

                        reactionSpecy.Sbase = new Sbase();
                        reactionSpecy.Sbase.id = Guid.NewGuid();

                        reaction.ReactionSpecies.Add(reactionSpecy);
                    }

                    model.Reactions.Add(reaction);
                }

                model.SaveChanges();

                Console.WriteLine("Finished saving all to DB");
                Console.ReadLine();
            }
        }

        private static void RecordFormulaeToDb()
        {
            // delete formulae from database
            Db.Context.Formulae.RemoveRange(Db.Context.Formulae.Where(f => true));

            using (
                StreamReader streamReader =
                    new StreamReader("C:\\Users\\sharno\\Dropbox\\Metabolomics\\Data\\ecoli_core.json"))
            {
                string json = streamReader.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);

                var atoms = new Dictionary<string, Dictionary<string, int>>();
                foreach (var m in items.metabolites)
                {
                    if (m.formula == null) continue;
                    var label = (string)m.id;
                    var formula = (string)m.formula;
                    atoms[label] = new Dictionary<string, int>();

                    Console.WriteLine(formula);
                    for (var c = 0; c < formula.Length; c++)
                    {
                        // todo: count the atoms that have one as a number as in C11H15N2O
                        if ("CHNO".Contains(formula[c]) && (c + 1 < formula.Length) && char.IsDigit(formula[c + 1]))
                        {
                            var atom = formula[c];
                            var num = "";

                            while (c+1 < formula.Length && char.IsDigit(formula[c+1]))
                            {
                                c++;
                                num += formula[c];
                            }

                            if (atoms[label].ContainsKey(atom.ToString()))
                            {
                                atoms[label][atom.ToString()] = atoms[label][atom.ToString()] + int.Parse(num);
                            }
                            else
                            {
                                atoms[label][atom.ToString()] = int.Parse(num);
                            }
                        }
                        else if ("CHNO".Contains(formula[c]))
                        {
                            var atom = formula[c];
                            if (atoms[label].ContainsKey(atom.ToString()))
                            {
                                atoms[label][atom.ToString()] = atoms[label][atom.ToString()] + 1;
                            }
                            else
                            {
                                atoms[label][atom.ToString()] = 1;
                            }
                        }
                    }
                }

                foreach (var m in atoms)
                {
                    var dbMetabolite = Db.Context.Species.Single(metabolite => metabolite.sbmlId == m.Key);
                    foreach (var atom in m.Value)
                    {
                        var a = new Formula
                        {
                            atom = atom.Key,
                            numAtoms = atom.Value
                        };
                        dbMetabolite.Formulae.Add(a);
                    }
                    Db.Context.SaveChanges();
                }

                Console.WriteLine("recorded all atoms and formulae");
                Console.ReadKey();
                //atoms.ToList().ForEach(x =>
                //{
                //    Console.WriteLine(x.Key);
                //    x.Value.ToList().ForEach(y =>
                //    {
                //        Console.WriteLine("    " + y.Key + ": " + y.Value);
                //    });
                //});
            }
        }
    }
}
