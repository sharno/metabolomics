using Metabol.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Metabol.Api.Controllers
{
    public class SubsystemController : ApiController
    {
        // GET: Subsystem
        [Route("subsystems")]
        [HttpGet]
        public dynamic Get()
        {
            return DbModels.Db.Context.Reactions
                .GroupBy(x => x.subsystem)
                .Select(g => g.Key)
                .ToList();
        }

        // GET: Subsystem detail
        [Route("subsystems/{id}")]
        [HttpGet]
        public dynamic Get(string id)
        {
            return DbModels.Db.Context.Reactions.Where(x => x.subsystem == id)
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
                        }).ToList()
                    }).ToList()
                }).Single();
        }


    }
}