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
using Metabol.Api.Cache;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Web;

namespace Metabol.Api.Controllers
{
    /// <summary>
    /// Subsystem Analysis 
    /// </summary>

    public class SubsystemAnalysisController : ApiController
    {
        // GET: Subsystem Analyze
        [Route("subsystems-analyze-start")]
        [HttpPost]
        public dynamic StartAnalyze(AnalysisViewModel z)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            return SubsystemService.GetSubsystemAnalyzeResultCacheKey(z.ConcentrationChanges);
        }


        // GET: Subsystem Analyze
        [Route("subsystems-analyze-start-async")]
        [HttpPost]
        public dynamic StartAnalyzeAsync(AnalysisViewModel z)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            return SubsystemService.GetSubsystemAnalyzeResultCacheKeyAsync(z.ConcentrationChanges);
        }

        // GET: Subsystem Analyze
        [Route("subsystems-analyze/{key}")]
        [HttpGet]
        public dynamic Analyze(Guid key)
        {
            return SubsystemCache.GetFromCache(key);
        }
    }

}
