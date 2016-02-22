using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        reaction.reversible = false;
                    }
                    else
                    {
                        reaction.reversible = true;
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
    }
}
