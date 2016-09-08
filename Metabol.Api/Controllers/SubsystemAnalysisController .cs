using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;
using Ecoli;
using Metabol.Api.Services;
using Metabol.DbModels;
using Metabol.DbModels.Models;
using Metabol.DbModels.ViewModels;
using Microsoft.AspNet.Identity;

namespace Metabol.Api.Controllers
{
    /// <summary>
    /// Subsystem Analysis 
    /// </summary>

    public class SubsystemAnalysisController : ApiController
    {
         // GET: Subsystem Analyze
        [Route("subsystems-analyze")]
        [HttpPost]
        public dynamic Analyze(AnalysisViewModel z)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            return SubsystemService.GetSubsystemAnalyzeResult(z.ConcentrationChanges);
        }

    }
}
