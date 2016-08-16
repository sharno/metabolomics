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
    /// Search api for metabolite and reaction Name starting with term.
    /// </summary>

    public class SearchprefixController : ApiController
    {
        /// <summary>
        /// Searchs metabolite and reaction names starting with term.
        /// </summary>
        /// <param Name="term">term to search for</param>
        /// <returns>first 10 reaction and metabolites starting with term </returns>
        [Route("searchprefix/{term}")]
        [HttpGet]
        public dynamic Get(string term)
        {
            var rval = new
            {
                reactions = DbModels.Db.GetReactionNames(term),
                metabolites = DbModels.Db.GetMetaboliteNames(term)
            };

            return rval;
        }


    }
}
