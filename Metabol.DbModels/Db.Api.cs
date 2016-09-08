using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Metabol.DbModels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Metabol.Api.Models;
using System.Data.SqlClient;

namespace Metabol.DbModels
{
    public static class DynamicExtensions
    {
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            foreach (var property in value.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                expando.Add(property.Name, property.GetValue(value));
            return (ExpandoObject)expando;
        }
    }

    partial class Db
    {
        public static UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new MetabolApiDbContext()));

        public static IQueryable<dynamic> GetReactions(string term)
        {
            return from r in Context.Reactions
                   where r.name.Contains(term)
                   select new { r.name, id = r.sbmlId };
        }

        public static IEnumerable<dynamic> GetMetabolites(string term)
        {
            return from r in Db.Context.Species
                   where r.name.Contains(term)
                   select new { r.name, id = r.sbmlId };
        }

        public static IQueryable<dynamic> GetReactionNames(string prefix)
        {
            return (from r in Context.Reactions
                    where r.name.StartsWith(prefix)
                    select new { r.name, id = r.sbmlId }).Take(20);
        }

        public static IQueryable<dynamic> GetMetaboliteNames(string prefix)
        {
            return (from r in Db.Context.Species
                    where r.name.StartsWith(prefix)
                    select new { r.name, id = r.sbmlId }).Take(20);
        }

        public static dynamic GetMetaboliteById(string id)
        {
            try
            {
                var m = Db.Context.Species.Single(s => s.sbmlId == id);

                dynamic rval = new
                {
                    id = m.sbmlId,
                    m.name,
                    compartment = m.Compartment?.name,
                    initialAmount = m.initialAmount,
                    subsystems = m.ReactionSpecies.Select(x => x.Reaction.subsystem).Distinct().ToList(),
                    charge = m.charge,
                    model = m.Compartment?.Model?.sbmlId,
                    species_type = m.SpeciesType?.name,
                    annotation = m.Sbase?.annotation,
                    sboTerm = m.Sbase?.sboTerm,
                    notes = m.Sbase?.notes,
                };

                return rval;
            }
            catch (Exception e)
            {
                return new { error = $"metabolite '{id}' does not exists!", exception = e.ToString() };
            }
        }

        public static dynamic GetRelatedReactionsOfMeta(string id)
        {
            try
            {
                var m = Db.Context.Species.Single(s => s.sbmlId == id);

                dynamic rval = new
                {
                    reactions = from rid in m.ReactionSpecies
                                where rid.roleId != Db.ReversibleId
                                select new
                                {
                                    id = rid.Reaction.sbmlId,
                                    rid.Reaction.name,
                                    rid.Reaction.subsystem,
                                    stoichiometry = rid.stoichiometry,
                                    metabolites = GetMetabolites(id, rid.Reaction.sbmlId)
                                }
                };

                return rval;
            }
            catch (Exception e)
            {
                return new { error = $"metabolite '{id}' does not exists!", exception = e.ToString() };
            }
        }

        private static IQueryable<dynamic> GetMetabolites(string id, string rid)
        {
            return from rss in (from rs in Db.Context.ReactionSpecies
                                where rs.Reaction.sbmlId == rid
                                select rs)
                   where rss.Species.sbmlId != id && rss.roleId != Db.ReversibleId
                   select new
                   {
                       id = rss.Species.sbmlId,
                       stoichiometry = rss.stoichiometry,
                       reactions = from rss1 in (from rs in Db.Context.ReactionSpecies
                                                 where rs.Species.sbmlId == rss.Species.sbmlId
                                                 select rs)
                                   where rss1.Reaction.sbmlId != rid && rss1.roleId != Db.ReversibleId
                                   select new
                                   {
                                       id = rss1.Reaction.sbmlId,
                                       rss1.stoichiometry,
                                       rss1.Reaction.subsystem
                                   }
                   };
        }

        public static dynamic GetReactionById(string id)
        {
            try
            {
                var m = Db.Context.Reactions.Single(s => s.sbmlId == id);
                dynamic rval = new
                {
                    id = m.sbmlId,
                    m.name,
                    m.reversible,
                    m.subsystem,
                    model = m.Model.sbmlId,
                    m.Sbase?.annotation,
                    m.Sbase?.sboTerm,
                    m.Sbase?.notes,
                };

                return rval;
            }
            catch (Exception e)
            {
                return new { error = $"reaction '{id}' does not exists!", exception = e.ToString() };
            }
        }

        public static dynamic GetRelatedMetabolitesOfReact(string id)
        {
            try
            {
                {
                    var m = Context.Reactions.First(s => s.sbmlId == id);
                    dynamic rval = new
                    {

                        metabolites = from rid in m.ReactionSpecies
                                      where rid.roleId != Db.ReversibleId
                                      select new
                                      {
                                          id = rid.Species.sbmlId,
                                          rid.Species.name,
                                          stoichiometry = rid.stoichiometry,
                                          reactions = GetReactions(id, rid.Species.sbmlId)
                                      }
                    };

                    return rval;
                }
            }
            catch (Exception e)
            {
                return new { error = $"reaction '{id}' does not exists!", exception = e.ToString() };
            }
        }

        private static IQueryable<dynamic> GetReactions(string id, string sbmlId)
        {
            {
                return from rss in (from rs in Context.ReactionSpecies
                                    where rs.Species.sbmlId == sbmlId
                                    select rs)
                       where rss.Reaction.sbmlId != id && rss.roleId != Db.ReversibleId// 
                       select new
                       {
                           id = rss.Reaction.sbmlId,
                           rss.stoichiometry,
                           rss.Reaction.subsystem
                       };
            }
        }

        public static IEnumerable<AnalysisModels> GetUserAnalysis(string userId)
        {
            return UserManager.FindById(userId).Analyses.Select(models => new AnalysisModels
            {
                ConcentrationChanges = models.ConcentrationChanges,
                DateTime = models.DateTime,
                Iterations = models.Iterations,
                Name = models.Name,
                SessionKey = models.SessionKey
            });
        }

        public static AnalysisModels GetSingleAnalysis(string userId, string sessionId)
        {
            try
            {
                var single = ApiDbContext.Analyses.Where(models => models.User.Id == userId)
                    .First(a => a.SessionKey == sessionId);

                return new AnalysisModels
                {
                    ConcentrationChanges = single.ConcentrationChanges,
                    DateTime = single.DateTime,
                    Iterations = single.Iterations,
                    Name = single.Name,
                    SessionKey = single.SessionKey
                };
            }
            catch (InvalidOperationException)
            {
                throw new KeyNotFoundException();
            }

        }

        public static dynamic GetSubsystemReactions(string name)
        {
            return DbModels.Db.Context.Reactions.Where(x => x.subsystem == name)
                .GroupBy(x => x.subsystem)
                .Select(g => new
                {
                    reactions = g.Select(x => new
                    {
                        id = x.sbmlId,
                        x.name,
                        x.reversible,
                        model = x.Model.sbmlId,
                        x.Sbase.annotation,
                        x.Sbase.sboTerm,
                        x.Sbase.notes,
                        metabolites = x.ReactionSpecies.Where(y => y.roleId != Db.ReversibleId).Select(z => new
                        {
                            id = z.Species.sbmlId,
                            z.Species.name,
                            stoichiometry = z.stoichiometry
                        })
                    })
                }).Single().reactions;
        }

        public static List<ConnectedSubsystemViewModel> GetConnectedSubsystems(string name)
        {
            var connectedSubsystems = DbModels.Db.Context.Database
                .SqlQuery<ConnectedSubsystemRawViewModel>(
                    "dbo.SelectConnectedSubSystems @SubsystemName",
                    new SqlParameter("@SubsystemName", name)).ToList();

            return connectedSubsystems
                .GroupBy(x => x.Subsystem)
                .Select(x => new ConnectedSubsystemViewModel
                {
                    Subsystem = x.Key,
                    Metabolites = x.Select(y => new ConnectedSubsystemItemViewModel
                    {
                        Name = y.Name,
                        Id = y.SbmlId
                    }).ToList()
                }).ToList();
        }
    }
}
