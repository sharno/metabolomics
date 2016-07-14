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

    public class RelatedMetabolitesController : ApiController
    {
        /// <summary>
        /// GET's all connected metabolites of reaction 
        /// </summary>
        /// <param name="reactionId"></param>
        /// <returns>all related metabolites of reaction</returns>
        [Route("relatedmetabolites/{reactionId}")]
        [HttpGet]
        public dynamic Get(string reactionId)
        {
            var rval = DbModels.Db.GetRelatedMetabolitesOfReact(reactionId);
            return rval;
        }
    }
}
