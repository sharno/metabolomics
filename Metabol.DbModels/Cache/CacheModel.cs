using Metabol.DbModels.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.DbModels.Cache
{
    [Serializable]
    public class CacheModel
    {
        public CacheModel () { }
        public CacheModel (EcoliCoreModel context)
        {
            updateCache(context);
        }

        public List<Compartment> Compartments = new List<Compartment>();
        public List<Species> Species = new List<Species>();
        public List<Reaction> Reactions = new List<Reaction>();
        public List<ReactionSpecy> ReactionSpecies = new List<ReactionSpecy>();

        private void updateCache(EcoliCoreModel context)
        {
            foreach (var c in context.Compartments)
            {
                var compartment = new Compartment
                {
                    id = c.id,
                    sbmlId = c.sbmlId,
                    name = c.name,
                };
                Compartments.Add(compartment);
            }

            foreach (var s in context.Species)
            {
                var metabolite = new Species
                {
                    id = s.id,
                    sbmlId = s.sbmlId,
                    name = s.name,
                    charge = s.charge,
                    compartmentId = s.compartmentId,
                    ReactionSpecies = null,
                };
                Species.Add(metabolite);
            }

            foreach (var r in context.Reactions)
            {
                var reaction = new Reaction
                {
                    id = r.id,
                    sbmlId = r.sbmlId,
                    name = r.name,
                    subsystem = r.subsystem,
                    reversible = r.reversible,
                    ReactionSpecies = null,
                };

                reaction.ReactionBounds = new List<ReactionBound> {
                    new ReactionBound() {
                        reactionId = r.id,
                        lowerBound = r.ReactionBounds.First().lowerBound,
                        upperBound = r. ReactionBounds.First().upperBound,
                    }
                };

                //if (r.ReactionBoundFix != null)
                //    reaction.ReactionBoundFix = new ReactionBoundFix {
                //        reactionId = r.id,
                //        lowerbound = r.ReactionBoundFix.lowerbound,
                //        upperbound = r. ReactionBoundFix.upperbound,
                //    };

                Reactions.Add(reaction);
            }

            foreach (var rs in context.ReactionSpecies)
            {
                var temp = new ReactionSpecy
                {
                    id = rs.id,
                    name = rs.name,
                    sbmlId = rs.sbmlId,
                    reactionId = rs.reactionId,
                    speciesId = rs.speciesId,
                    stoichiometry = rs.stoichiometry,
                    roleId = rs.roleId,
                    Reaction = Reactions.Find(r => r.id == rs.reactionId),
                    Species = Species.Find(s => s.id == rs.speciesId),
                };
                ReactionSpecies.Add(temp);
            }


            foreach (var s in Species)
            {
                s.ReactionSpecies = ReactionSpecies.Where(rs => rs.speciesId == s.id).ToList();
            }
            foreach (var r in Reactions)
            {
                r.ReactionSpecies = ReactionSpecies.Where(rs => rs.reactionId == r.id).ToList();
            }
        }
    }
}
