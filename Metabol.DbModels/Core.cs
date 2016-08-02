using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ILOG.Concert;
using ILOG.CPLEX;
using Exception = System.Exception;

namespace Metabol.DbModels
{
    public class Core
    {
        public static string Dir = ConfigurationManager.AppSettings["modelOutput"];//"model2/";//

        static Core()
        {
            Init();
        }

        private static void Init()
        {
            try
            {
                Directory.CreateDirectory(Dir);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        public static dynamic ToDynamic(object value)
        {
            return (ExpandoObject)value.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Aggregate((IDictionary<string, object>)new ExpandoObject(), (obj, info) =>
                {
                    obj[info.Name] = info.GetValue(value);
                    return obj;
                });
        }

        public static IEnumerable<string> Constraints(string file)
        {
            var model = new Cplex();
            //model.SetParam(Cplex.Param.Preprocessing.Presolve, false);
            model.ImportModel(file);
            dynamic link = ToDynamic(ToDynamic(ToDynamic(ToDynamic(model)._model)._gc)._link);
            link = ToDynamic(link._prev);
            var cconst = new List<CpxIfThen>();
            while (link._obj != null)
            {
                cconst.Add(link._obj as CpxIfThen);
                link = ToDynamic(link._prev);
            }

            var m = model.GetLPMatrixEnumerator();
            m.MoveNext();
            var mat = (CpxLPMatrix)m.Current;

            var vars = mat.GetNumVars();
            var ranges = mat.GetRanges();
            var obj = model.GetObjective();
            var exps = new Dictionary<string, IRange>();
            foreach (var range in ranges)
            {
                var exp = Regex.Replace(range.Expr.ToString(), "#\\d+", "")
                    .Replace("_ex", "ex")
                    .Replace("_EX", "EX");

                if (exps.ContainsKey(exp))
                {
                    if (Math.Abs(exps[exp].UB - double.MaxValue) < double.Epsilon ||
                        double.IsPositiveInfinity(exps[exp].UB))
                        exps[exp].UB = range.UB;

                    else if (Math.Abs(exps[exp].LB - double.MinValue) < double.Epsilon ||
                        double.IsNegativeInfinity(exps[exp].LB))
                        exps[exp].LB = range.LB;
                }
                else
                    exps[exp] = model.Range(range.LB, range.Expr, range.UB);

            }

            return exps.Select(range => Math.Abs(range.Value.UB - range.Value.LB) < 0.0001 ?
                $"{range.Key} == {range.Value.LB}"
                : $"{range.Value.UB} >= {range.Key} >= {range.Value.LB}");
        }

        public static void SaveAsDgs(HyperGraph.Node mi, HyperGraph graph, string dir)
        {
            try
            {
                var file = $"{dir}{graph.Step}graph.dgs";
                var maxLevel = graph.LastLevel;

                var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes", mi.ToDgs(NodeType.Selected) };
                lines.AddRange(graph.Nodes.Values
                    .Where(n => n.Id != mi.Id)
                    .Select(node => new { node, type = node.IsBorder ? NodeType.Border : node.RecentlyAdded? NodeType.New : NodeType.None })
                    .Select(@t => @t.node.ToDgs(@t.type)));

                lines.Add("#Hyperedges");
                foreach (var edge in graph.Edges.Values)
                {
                    var type = EdgeType.None;
                    if (edge.RecentlyAdded == true)
                        type = EdgeType.New;

                    lines.Add(edge.ToDgs(type));
                }

                File.AppendAllLines(file, lines);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void TimeSeries()
        {
            const string fbadir = "A:\\model2_fba";

            const string momadir = "A:\\model2_moma";
            const string momadir25 = "A:\\model2_moma25";
            const string momadir75 = "A:\\model2_moma75";

            //const string roomdir = "A:\\model2_room";


            var ts = GetTimeSeries(fbadir).Values.Select(doubles => string.Join(",", doubles));
            File.WriteAllLines("A:\\fba_change.csv", ts);

            var ts2 = GetTimeSeries(momadir).Values.Select(doubles => string.Join(",", doubles));
            File.WriteAllLines("A:\\moma_change.csv", ts2);

            var ts3 = GetTimeSeries(momadir25).Values.Select(doubles => string.Join(",", doubles));
            File.WriteAllLines("A:\\moma_change25.csv", ts3);

            var ts4 = GetTimeSeries(momadir75).Values.Select(doubles => string.Join(",", doubles));
            File.WriteAllLines("A:\\moma_change75.csv", ts4);

        }

        private static SortedDictionary<string, double[]> GetTimeSeries(string fbadir)
        {
            var rval = new SortedDictionary<string, double[]>();

            var files = Directory.GetFiles(fbadir).Where(p => p.EndsWith("result.txt")).ToList();
            var max = files.Select(e => Path.GetFileName(e)?.Replace("result.txt", "")).Select(int.Parse).Max();

            foreach (var file in files)
            {
                var s = Path.GetFileName(file)?.Replace("result.txt", "");
                Debug.Assert(s != null, "s != null");
                var i = int.Parse(s);
                foreach (var res in File.ReadAllLines(file).Select(line => line.Split(':')))
                {
                    if (!rval.ContainsKey(res[0])) rval[res[0]] = new double[max + 1];
                    rval[res[0]][i] = double.Parse(res[1]);
                }
            }

            return rval;
        }
    }
}
