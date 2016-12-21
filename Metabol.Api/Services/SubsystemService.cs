using Metabol.DbModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Subsystems;
using Metabol.Api.Cache;
using System.Threading.Tasks;

namespace Metabol.Api.Services
{
    public static class SubsystemService
    {
        public static Dictionary<string, string[]> GetSubsystemAnalyzeResult(ConcentrationChange[] concentrationChanges)
        {
            //var result = new Dictionary<string, string[]>();

            //result["solution-1"] = new string[] { "pathway-1", "pathway-2", "pathway-3" };
            //result["solution-2"] = new string[] { "pathway-1", "pathway-2" };
            //result["solution-3"] = new string[] { "pathway-1", "pathway-3" };
            //result["solution-4"] = new string[] { "pathway-2", "pathway-4" };

            var subsystemData = mapConcentrationChanges(concentrationChanges);
            var results = Program.Start(subsystemData);
            return results;
        }

        public static Guid GetSubsystemAnalyzeResultCacheKey(ConcentrationChange[] concentrationChanges)
        {
            var analyzeResult = SubsystemService.GetSubsystemAnalyzeResult(concentrationChanges);
            return SubsystemCache.Cache(analyzeResult);
        }

        public static Guid GetSubsystemAnalyzeResultCacheKeyAsync(ConcentrationChange[] concentrationChanges)
        {
            Guid key = Guid.NewGuid();

            Task.Factory.StartNew(() => {
                var analyzeResult = SubsystemService.GetSubsystemAnalyzeResult(concentrationChanges);
                SubsystemCache.Cache(analyzeResult, key);
            });
   
            return key;
        }

        private static Dictionary<string, double> mapConcentrationChanges(ConcentrationChange[] concentrationChanges)
        {
            return concentrationChanges.ToDictionary(kvp => kvp.Name, kvp => kvp.Change);
        }
    }
}