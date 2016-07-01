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
    public class SessionsController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("sessions")]
        [HttpGet]
        public dynamic Get()
        {
            return Util.Session.GetAllSessions();
        }
    }
}
