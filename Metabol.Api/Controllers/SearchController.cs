using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Metabol.Api.Controllers
{
    //[Route("api2/[controller]")]
    /// <summary>
    /// Search api for metabolites and reactions.
    /// </summary>

    public class SearchController : ApiController
    {
        /// <summary>
        /// Searches for metabolites and reactions containing term
        /// </summary>
        /// <param Name="term">term to seach for</param>
        /// <returns>list of metabolites and reactions containing term </returns>
        [Route("search/{term}")]
        [HttpGet]
        public dynamic Get(string term)
        {
            var rval = new
            {
                reactions = DbModels.Db.GetReactions(term),
                metabolites = DbModels.Db.GetMetabolites(term)
            };

            return rval;
        }


    }
}
