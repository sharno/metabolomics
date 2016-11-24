using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subsystems
{
    class Result
    {
        public int num;
        public List<string> ActiveSubsystems = new List<string>();
        public List<string> InactiveSubsystems = new List<string>();
        public Result ParentResult;
        public List<Result> ChildrenResults = new List<Result>();
        public bool Solved;
        public string MetaboliteExtended;

        public string ToJson (string parent)
        {
            var name = $"{string.Join(", ", ActiveSubsystems)} <A [{MetaboliteExtended}|{num}] I> {string.Join(", ", InactiveSubsystems)}";
            var json = $"{{\"name\": \"{name}\", \"parent\": \"{parent}\", \"solved\": {Solved.ToString().ToLower()}, \"children\": [{string.Join(", ", ChildrenResults.Select(r => r.ToJson(name)))}]}}";

            return json;
        }

        public static Dictionary<int, Dictionary<string, bool>> AllConfigurations (Result rootResult)
        {
            var leaves = rootResult.AllSolvedLeaves();
            var configs = new Dictionary<int, Dictionary<string, bool>>();

            foreach (var l in leaves)
            {
                configs[l.num] = l.ToConfiguration();
            }

            return configs;
        }

        public IEnumerable<Result> AllSolvedLeaves()
        {
            if (Solved && ! ChildrenResults.Any())
            {
                return new List<Result> { this };
            }
            return ChildrenResults.SelectMany(c => c.AllSolvedLeaves());
        }

        public Dictionary<string, bool> ToConfiguration()
        {
            var result = this;
            var config = new Dictionary<string, bool>();
            while(result != null)
            {
                foreach (var a in result.ActiveSubsystems)
                {
                    if (config.ContainsKey(a))
                        throw new InvalidOperationException("configuartion cannot have duplicate subsystems");
                    config[a] = true;
                }
                foreach (var i in result.InactiveSubsystems)
                {

                    if (config.ContainsKey(i))
                        throw new InvalidOperationException("configuartion cannot have duplicate subsystems");
                    config[i] = false;
                }

                result = result.ParentResult;
            }
            return config;
        }
    }
}
