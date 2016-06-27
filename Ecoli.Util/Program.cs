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
            //EraseAndRecordToDb();
            //RecordFormulaeToDb();
            //ValidateReactionsAtomsBalance();
            //ValidateCyclesAtomsBalance();
            //CheckExchangeReactionsInCycles();
            //LimitReactionsFluxes();
            RecordCyclesInterfaceMetabolitesRations();
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

            foreach (var sortedCycle in sortedCycles)
            {
                Console.WriteLine("contains cycles " + sortedCycle.CycleReactions.Count(reaction => !reaction.isReaction));
            }
            return sortedCycles;
        }

        private static void RecordCyclesInterfaceMetabolitesRations()
        {
            Db.Context.cycleInterfaceMetabolitesRatios.RemoveRange(Db.Context.cycleInterfaceMetabolitesRatios.Where(e => true));
            Db.Context.SaveChanges();

            //var cycles = Db.Context.Cycles.Select(c => c.id).ToList();
            int count = 0;

            //while (cycles.Any())
            //{
                //foreach (var cycle in Db.Context.Cycles.Where(cy => cycles.Contains(cy.id)))
                //{

                var sortedCycles = TopologicalSortCycles(Db.Context.Cycles.ToList());
                foreach (var cycle in sortedCycles) {
                    //Console.WriteLine((cycle.id == Guid.Parse("b88f7627-fd15-4173-a33f-ea80c7147681")) + "\n\n\n\n\n\n");
                    var recordedRatios = new Dictionary<Guid, Dictionary<Guid, List<double>>>();

                var nestedCycles = cycle.CycleReactions.Where(cr => !cr.isReaction).Select(cr => cr.otherId).ToList();
                //var satisfiedNestedCycles = nestedCycles.All(c => Db.Context.cycleInterfaceMetabolitesRatios.Any(ci => ci.cycleId == c));

                //if (cycle.CycleReactions.All(cr => cr.isReaction) || satisfiedNestedCycles)
                //{

                var ratios = nestedCycles.SelectMany(nc => Db.Context.cycleInterfaceMetabolitesRatios.Where(ci => ci.cycleId == nc)).ToList();

                        var cReactions = cycle.CycleReactions.Select(cr => cr.otherId).ToList();
                        var allSpecies =
                                Db.Context.ReactionSpecies.Where(rs => cReactions.Contains(rs.reactionId))
                                    .Select(rs => rs.speciesId).Distinct().ToList();
                        allSpecies = allSpecies.Union(Db.Context.CycleConnections.Where(cc => nestedCycles.Contains(cc.cycleId)).Select(cc => cc.metaboliteId)).Distinct().ToList();

                        //var allReactions =
                        //        Db.Context.ReactionSpecies.Where(rs => allSpecies.Contains(rs.speciesId))
                        //            .Select(rs => rs.reactionId).Distinct().ToList();
                        //var insideReactions = nestedCycles.SelectMany(GetAllReactionsOfCycle);
                        //allReactions.RemoveAll(re => insideReactions.Contains(re));

                        //allReactions.ForEach(r =>
                        //{
                        //    var bound = Db.Context.ReactionBounds.Single(rb => rb.reactionId == r);
                        //    vars[r] = model.NumVar(bound.lowerBound, bound.upperBound, NumVarType.Float);
                        //});
                        foreach (var cycleConnection in cycle.CycleConnections)
                        {
                            var model = new Cplex {Name = "FBA"};
                            var vars = new Dictionary<Guid, INumVar>();
                            var cycleMets = new Dictionary<Tuple<Guid, Guid>, Guid>();

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

                                if (cycle.CycleConnections.Any(cc => cc.metaboliteId == s))
                                {
                                    var pseudo = Guid.NewGuid();
                                    vars[pseudo] = model.NumVar(-1000, 1000, NumVarType.Float, "pseudo" + Db.Context.Species.Find(s).sbmlId);
                                    expr.AddTerm(1, vars[pseudo]);

                                    if (cycleConnection.metaboliteId == s) model.AddGe(vars[pseudo], new Random().Next(1, 900));
                                }
                                model.AddEq(expr, 0, Db.Context.Species.Find(s).sbmlId);
                            });

                            ratios.ForEach(ra =>
                            {
                                var expr1 = model.LinearNumExpr();
                                expr1.AddTerm(1, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite1)]]);
                                var expr2 = model.LinearNumExpr();
                                expr2.AddTerm(ra.ratio, vars[cycleMets[Tuple.Create(ra.cycleId, ra.metabolite2)]]);
                                model.AddEq(model.Abs(expr1), model.Abs(expr2));
                            });

                            var solved = model.Solve();
                            count++;
                            model.ExportModel($"{Core.Dir}{count}model.lp");
                            if (!solved)
                            {
                                continue;
                            }


                            var list = vars.ToList().Select(v => (Db.Context.Reactions.Find(v.Key) != null? Db.Context.Reactions.Find(v.Key).sbmlId: v.Value.Name) + "    " + model.GetValue(v.Value));
                            File.WriteAllLines($"{Core.Dir}result{count}.txt", list);
                            

                            cycle.CycleConnections.ToList().ForEach(cc => {
                                var first = 0.0;
                                foreach (var rs in Db.Context.ReactionSpecies.Where(rs => rs.speciesId == cc.metaboliteId && cReactions.Contains(rs.reactionId))) {
                                    if (model.GetValue(vars[rs.reactionId]) > 0) {
                                        first += rs.stoichiometry * model.GetValue(vars[rs.reactionId]);
                                    }
                                }
                                first +=
                                    nestedCycles.Where(
                                        nc =>
                                            cycleMets.ContainsKey(Tuple.Create(nc, cc.metaboliteId)) &&
                                            model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]) > 0)
                                        .Sum(nc => model.GetValue(vars[cycleMets[Tuple.Create(nc, cc.metaboliteId)]]));


                                cycle.CycleConnections.ToList().Where(cc2 => cc2.metaboliteId != cc.metaboliteId).ToList().ForEach(cc2 => {
                                    var second = 0.0;
                                    foreach (var rs in Db.Context.ReactionSpecies.Where(rs => rs.speciesId == cc2.metaboliteId && cReactions.Contains(rs.reactionId)))
                                    {
                                        // only pick producers
                                        if (model.GetValue(vars[rs.reactionId]) > 0)
                                        {
                                            second += rs.stoichiometry*model.GetValue(vars[rs.reactionId]);
                                        }
                                    }
                                    second +=
                                        nestedCycles.Where(
                                            nc =>
                                                cycleMets.ContainsKey(Tuple.Create(nc, cc2.metaboliteId)) &&
                                                model.GetValue(
                                                    vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]) > 0)
                                            .Sum(
                                                nc =>
                                                    model.GetValue(
                                                        vars[cycleMets[Tuple.Create(nc, cc2.metaboliteId)]]));

                                    if (String.Compare(cc.Species.sbmlId, cc2.Species.sbmlId, StringComparison.Ordinal) < 0)
                                    {
                                        //Console.Write(cc.Species.sbmlId + " " + cc2.Species.sbmlId + ", ");
                                        if (!recordedRatios.ContainsKey(cc.metaboliteId))
                                            recordedRatios[cc.metaboliteId] = new Dictionary<Guid, List<double>>();
                                        if (!recordedRatios[cc.metaboliteId].ContainsKey(cc2.metaboliteId))
                                            recordedRatios[cc.metaboliteId][cc2.metaboliteId] = new List<double>();
                                        recordedRatios[cc.metaboliteId][cc2.metaboliteId].Add(first / second);
                                    }
                                    else
                                    {
                                        //Console.Write(cc2.Species.sbmlId + " " + cc.Species.sbmlId + ", ");
                                        if (!recordedRatios.ContainsKey(cc2.metaboliteId))
                                            recordedRatios[cc2.metaboliteId] = new Dictionary<Guid, List<double>>();
                                        if (!recordedRatios[cc2.metaboliteId].ContainsKey(cc.metaboliteId))
                                            recordedRatios[cc2.metaboliteId][cc.metaboliteId] = new List<double>();
                                        recordedRatios[cc2.metaboliteId][cc.metaboliteId].Add(second / first);
                                    }
                                    // record in DB
                                    //var record = new DB.cycleInterfaceMetabolitesRatio
                                    //{
                                    //    cycleId = cycle.id,
                                    //    metabolite1 = cycleConnection.metaboliteId,
                                    //    metabolite2 = cc2.metaboliteId,
                                    //    ratio = Math.Round(first/second, 7)
                                    //};

                                    //Db.Context.cycleInterfaceMetabolitesRatios.Add(record);
                                });
                            });
                        }

                        //Console.WriteLine("removing cycle" + "  ===========================>  " + cycles.Count);
                        //cycles.Remove(cycle.id);
                    //}
                    recordedRatios.ToList().ForEach(f => f.Value.ToList().ForEach(s =>
                    {
                        var values = s.Value.Where(v => Math.Abs(v) > 0 && !double.IsInfinity(v)).ToList();
                        Console.WriteLine("List of ratios of " + Db.Context.Species.Find(f.Key).sbmlId + " .. " + Db.Context.Species.Find(s.Key).sbmlId);
                        values.ForEach(v => Console.Write(v + ", "));
                        Console.WriteLine();

                        if (values.Count > 1 &&
                            values.All(v => Math.Abs(Math.Abs(v) - Math.Abs(values[0])) < double.Epsilon))
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
            //}

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
            var count = 0;

            var g = ConstructHyperGraphFromSpecies(Db.Context.Species.Where(s => s.ReactionSpecies.Count < outlier));

            Console.WriteLine("loaded the whole network");


            // delete all entries from DB
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleReaction");
            Db.Context.Database.ExecuteSqlCommand("TRUNCATE TABLE CycleConnection");
            Db.Context.Database.ExecuteSqlCommand("DELETE FROM Cycle");
            Console.WriteLine("deleted DB entries");


            Dictionary<HyperGraph.Cycle, List<HyperGraph.Entity>> cycles = DFS.DetectAndCollapseCycles(g);

            foreach (var cycle in cycles)
            {
                RecordToDatabase(cycle.Key, cycle.Value);
            }

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
