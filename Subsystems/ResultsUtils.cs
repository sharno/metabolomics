using Metabol.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Subsystems
{
    class ResultsUtils
    {
        public static void SaveAsTree(Result rootResult)
        {
            File.AppendAllText($"{Core.Dir}result.json", $"result = {rootResult.ToJson("null")};");

            //var lines = new List<string> { "DGS004", "\"Subsystems Analysis\" 0 0", "#Nodes" };
            //var nodesNames = results.Select(ss => $"{string.Join(", ", ss.Where(s => s.Value).Select(s => s.Key))} AC|IN {string.Join(", ", ss.Where(s => !s.Value).Select(s => s.Key))}").ToList();
            //var nodesIndex = 0;
            //lines.AddRange(nodesNames.Select(n => $"an \"n{nodesIndex++}\" label:\"{n}\" ui.style:\"fill-color: rgb(0,255,0);\""));

            //lines.Add("#Edges");
            //for (var i = 0; i < nodesNames.Count-1; i++)
            //{
            //    lines.Add($"ae \"e{i}\" \"n{i}\" > \"n{i+1}\"");
            //}

            //File.AppendAllLines($"{Core.Dir}result{count}.dgs", lines);
        }

        public static void SaveSubsystemsHypergraph(HyperGraph network, List<string> extendedSubsystems, int num)
        {
            var lines = new List<string> { "DGS004", "\"Subsystems Analysis\" 0 0", "#Nodes" };
            var subsystems = network.Edges.Select(e => e.Value.Subsystem).Distinct();
            lines.AddRange(extendedSubsystems.Select(n => $"an \"{n}\" label:\"{n}\" ui.style:\"fill-color: rgb(0,255,0);\""));
            lines.AddRange(subsystems.Except(extendedSubsystems).Select(n => $"an \"{n}\" label:\"{n}\" ui.style:\"fill-color: rgb(255,255,0);\""));

            lines.Add("#Edges");
            var metsSubsFlux = new Dictionary<string, Dictionary<string, double>>();
            foreach (var met in network.Nodes.Values)
            {
                if (! metsSubsFlux.ContainsKey(met.Label))
                    metsSubsFlux[met.Label] = new Dictionary<string, double>();

                foreach (var r in met.AllReactions())
                {
                    if (r.Subsystem != null)
                    {
                        if (!metsSubsFlux[met.Label].ContainsKey(r.Subsystem))
                            metsSubsFlux[met.Label][r.Subsystem] = 0.0;

                        metsSubsFlux[met.Label][r.Subsystem] += met.Weights[r.Id] * r.Flux;
                    } else
                    {
                        metsSubsFlux[met.Label][r.Label] = met.Weights[r.Id] * r.Flux;
                    }
                }
            }
            
            foreach (var met in metsSubsFlux.Where(m => m.Value.Count >= 2))
            {
                lines.Add($"an \"{met.Key}\" label:\"{met.Key}\" ui.style:\"fill-color: rgb(0,0,255);\"");
                met.Value.ToList().ForEach(s => {
                    lines.Add($"ae \"{met.Key}_{s.Key}\" \"{met.Key}\" {(s.Value>=0 ? "<" : ">")} \"{s.Key}\" "+
                        $"ui.style:\"size:{3 * Math.Abs(Math.Ceiling(s.Value))/1000}px; fill-color: rgb({(s.Value!=0.0?"0,255,0":"100,100,100")});\" label:\" {Math.Ceiling(s.Value)} \"");
                });
            }

            File.AppendAllLines($"{Core.Dir}result{num}.dgs", lines);
        }

        public static void ResultToDecision(Result rootResult)
        {

        }
    }
}
