using Metabol.DbModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        [Route("subsystems/{*id}")]
        [HttpGet]
        public dynamic Get(string id)
        {
            return new
            {
                reactions = DbModels.Db.GetSubsystemReactions(id),
                connectedSubsystems = DbModels.Db.GetConnectedSubsystems(id)
            };

        }

    }
}