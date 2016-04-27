using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ecoli.Util.DB;
using Ecoli.Util.SimpleCycle;
using Newtonsoft.Json;

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
