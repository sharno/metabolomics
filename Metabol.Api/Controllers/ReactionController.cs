using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Metabol.Api.Controllers
{
    //[Route("api2/reaction")]
    /// <summary>
    /// Reactions related api
    /// </summary>

    public class ReactionController : ApiController
    {
        /// <summary>
        /// GET's short information about reaction with id.
        /// </summary>
        /// <param Name="id">sbml id of reaction</param>
        /// <returns>short information about </returns>
        [Route("reaction/{id}")]
        [HttpGet]
        public dynamic Get(string id)
        {
            return DbModels.Db.GetReactionById(id);
        }
    }
}
