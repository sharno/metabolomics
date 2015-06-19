using System;
using System.Collections.Generic;
using System.Web.Http;
using Metabol;

namespace MetabolApi.Controllers
{
    /// <summary>
    /// Flux balance analysis controller
    /// </summary>
    public class FbaController : ApiController
    {
        ///<summary>
        ///  Initialize algorithm
        ///  returns key
        /// </summary>
        [Route("fba/start")]
        [HttpGet]
        public object Start()
        {
            return Metabol.User.StartNew();
        }

        ///<summary>
        ///  Next Step 
        ///  api/fba/start must be called before this
        /// </summary>
        [Route("fba/{key}/{iteration:int}")]
        [HttpGet]
        public IEnumerable<Iteration> Next(string key, int iteration)
        {
            return Metabol.User.Step(key, iteration);
        }

    }
}
