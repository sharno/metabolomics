using System;
using System.Collections.Generic;
using System.Web.Http;
using Metabol;

namespace MetabolApi.Controllers
{
    using System.Linq;
    using System.Web.Http.Cors;

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
            return "";
            //return Metabol.User.StartNew();
        }

        ///<summary>
        ///  Next Step 
        ///  api/fba/start must be called before this
        /// </summary>
        //[Route("fba/{key}/{iteration:int}")]
        //[HttpGet]
        //public IEnumerable<Iteration> Next(string key, int iteration)
        //{
        //    return Metabol.User.Step(key, iteration);
        //}

        [Route("fba/{key}/{iteration:int}")]
        [HttpGet]
        public Iteration Get(string key, int iteration)
        {
            //var list = Metabol.User.Step(key, iteration).ToList();
            //return list.Count == 0 ? Iteration.Empty : list.First();
            return Iteration.Empty;
        }


    }
}
