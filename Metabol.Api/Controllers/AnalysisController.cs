using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;
using Ecoli;
using Metabol.DbModels;
using Metabol.DbModels.Models;
using Metabol.DbModels.ViewModels;
using Microsoft.AspNet.Identity;

namespace Metabol.Api.Controllers
{
    /// <summary>
    /// Metabolomics FBA
    /// </summary>

    public class AnalysisController : ApiController
    {
        ///<summary>
        ///  Initialize algorithm with default metabolite changes
        /// </summary>
        ///  <returns>key</returns>
        [Route("fba/start")]
        [HttpGet]
        public object Start()
        {
            var key = Session.StartNew();

            if (User.Identity.IsAuthenticated)
            {
                var analysis = new AnalysisModels
                {
                    SessionKey = key,
                    ConcentrationChanges = new ConcentrationChange[0],
                    DateTime = DateTime.Now,
                    Name = "kim"
                };
                var userId = User.Identity.GetUserId();
                var user = Db.UserManager.FindById(userId);
                user.Analyses.Add(analysis);
                Db.UserManager.Update(user);
            }
            return new { key = key };
        }

        /// <summary>
        /// Initialize algorithm with given metabolite changes
        /// </summary>
        /// <param name="z">obeserved metabolite changes</param>
        /// <returns>key</returns>

        //[Authorize]
        [Route("fba/start")]
        [HttpPost]
        public object Start(AnalysisViewModel z)
        {
            var key = Session.StartNew(z.ConcentrationChanges);
            if (User.Identity.IsAuthenticated)
            {
                var analysis = new AnalysisModels
                {
                    SessionKey = key,
                    ConcentrationChanges = z.ConcentrationChanges,
                    DateTime = DateTime.Now,
                    Name = z.Name
                };
                var userId = User.Identity.GetUserId();
                var user = Db.UserManager.FindById(userId);
                user.Analyses.Add(analysis);
                Db.UserManager.Update(user);
            }

            return new { key = key };
        }


        /// <summary>
        /// GET's FBA result of given key and iteration
        /// </summary>
        /// <param name="key">generated key</param>
        /// <param name="iteration">iteration of FBA</param>
        /// <returns></returns>
        [Route("fba/{key}/{iteration:int}")]
        [HttpGet]
        public IterationModels Get(string key, int iteration)
        {
            if (iteration <= 0) return IterationModels.Empty;
            var list = Session.Step(key, iteration).ToList();
            return list.Count == 0 ? IterationModels.Empty : (IterationModels) list[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("fba/list")]
        [HttpGet]
        public IEnumerable<AnalysisModels> ListAll()
        {
            var userId = User.Identity.GetUserId();
            return Db.GetUserAnalysis(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [Route("fba/detail/{sessionId}")]
        [HttpGet]
        public AnalysisModels Get(string sessionId)
        {
            var userId = User.Identity.GetUserId();
            return Db.GetSingleAnalysis(userId, sessionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        [Route("fba/save")]
        [HttpPost]
        [Authorize]
        public HttpResponseMessage Save([FromBody]string key)
        {
            try
            {
                Guid.Parse(key);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId();
            var user = Db.UserManager.FindById(userId);
            var single = user.Analyses.SingleOrDefault(a => a.SessionKey == key);

            if (single == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"Analysis Key = {key}"),
                    ReasonPhrase = "Analysis Key Not Found. It is too late to save analysis"
                };
            }

            Task.Run(() =>
            {
                var it = Session.CacheOrGetWorker(key).Iteration;
                for (var i = 0; i < it; i++)
                {
                    var iteration = Session.GetResult(key, i + 1);
                    //var analysisModels = Db.ApiDbContext.Analyses.Single(
                    //    models => models.SessionKey == key && models.User.Id == userId);
                    //analysisModels?.Iterations.Add(iteration);
                    single.Iterations.Add(iteration);
                }
                // Db.ApiDbContext.SaveChanges();
                Db.UserManager.Update(user);

            });

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }
}
