using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Metabol.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>

    public class RelatedReactionsController : ApiController
    {
        /// <summary>
        /// GET's all connected reactions of metabolite
        /// </summary>
        /// <param name="metaboliteId">sbml id of metabolite</param>
        /// <returns>all related reactions of metabolite</returns>
        [Route("relatedreactions/{metaboliteId}")]
        [HttpGet]
        public dynamic Get(string metaboliteId)
        {
            var rval = DbModels.Db.GetRelatedReactionsOfMeta(metaboliteId);
            return rval;
        }
    }
}
