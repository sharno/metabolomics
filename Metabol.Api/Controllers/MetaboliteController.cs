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
    /// Metabolite related api
    /// </summary>

    public class MetaboliteController : ApiController
    {
        /// <summary>
        /// GET's short information about metabolite with id.
        /// </summary>
        /// <param name="id">metabolite sbml id</param>
        /// <returns>short metabolite information</returns>
        [Route("metabolite/{id}")]
        [HttpGet]
        public dynamic Get(string id)
        {
            var rval = DbModels.Db.GetMetaboliteById(id);

            return rval;
        }
    }
}
