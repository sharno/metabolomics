using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol.DbModels;
using ILOG.Concert;
using ILOG.CPLEX;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Metabol.DbModels.Cache;
using Newtonsoft.Json;

namespace Subsystems
{
    public class Program
    {
        const double TActive = 10;
        const double TInactive = 0.0001;
        static int counter = 0;

        static StreamWriter log = File.AppendText("C:\\Users\\sharno\\Dropbox\\Metabolomics\\Results\\2016.11.30\\log.txt");
        // static List<string> FixedSubsystems = new List<string> { "Transport, Extracellular", "Exchange", "Extracellular exchange" }; // ecoli
        static List<string> FixedSubsystems = new List<string> {
            "",
            "Miscellaneous",
            "Exchange/demand reaction",
            "Transport, endoplasmic reticular",
            "Transport, extracellular",
            "Transport, golgi apparatus",
            "Transport, lysosomal",
            "Transport, mitochondrial",
            "Transport, nuclear",
            "Transport, peroxisomal",
        }; // recon 2.2

        static void Main(string[] args)
        {
            Console.BufferHeight = Int16.MaxValue - 1;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            //var measuredMetabolites = new Dictionary<string, double>
            //{
            //    ["2425dhvitd2_m"] = 1,
            //    //["dmantipyrine_e"] = -1,
            //};
            Dictionary<int, Dictionary<string, double>> measuredMetabolites;
            using (StreamReader streamReader = new StreamReader("C:\\Users\\sharno\\Desktop\\breast_cancer.json"))
            {
                string json = streamReader.ReadToEnd();
                measuredMetabolites = JsonConvert.DeserializeObject< Dictionary<int, Dictionary<string, double>> >(json);
            }
            Start(measuredMetabolites[100]);
        }

        public static Dictionary<string, string[]> Start(Dictionary<string, double> measuredMetabolites)
        {
            log.AutoFlush = true;
            // construct cache
            //DataUtils.JsonToCache("C:\\Users\\sharno\\Downloads\\MODEL1603150001.json");
            // loading cache
            Db.Cache = DataUtils.ReadFromBinaryFile<CacheModel>("C:\\Users\\sharno\\Downloads\\MODEL1603150001.bin");


            // test
            //var borders = Db.Cache.Species.Where(s => ! s.ReactionSpecies.Any(rs => rs.Reaction.reversible) && (s.ReactionSpecies.All(rs => rs.roleId == Db.ProductId) || s.ReactionSpecies.All(rs => rs.roleId == Db.ReactantId)));
            //Console.Write(string.Join("\n", borders.Select(s => s.sbmlId)));
            //Console.ReadKey();


            var network = new HyperGraph();
            var extendedSubsystems = new List<string>();
            

            var metabolitesSubsystems = new Dictionary<string, List<string>>();
            foreach (var m in measuredMetabolites.Keys)
            {
                var metabolite = Db.Cache.Species.Single(s => s.sbmlId == m);
                metabolitesSubsystems[m] = metabolite.ReactionSpecies.Select(rs => rs.Reaction.subsystem).Distinct().Except(FixedSubsystems).ToList();
            }

            var subsystemsMetabolites = new Dictionary<string, List<string>>();
            foreach (var m in measuredMetabolites.Keys)
            {
                foreach (var s in metabolitesSubsystems[m])
                {
                    if (!subsystemsMetabolites.ContainsKey(s))
                        subsystemsMetabolites[s] = new List<string>();
                    subsystemsMetabolites[s].Add(m);
                }
            }

            var crossroadMetabolite = metabolitesSubsystems.Where(ms => ms.Value.Count > 1).OrderBy(ms => ms.Value.Count).FirstOrDefault();

            if (crossroadMetabolite.Key == null)
            {
                var mostObservedMetabolitesSubsystem = subsystemsMetabolites.Where(sm => sm.Value.Count == subsystemsMetabolites.Max(sm2 => sm2.Value.Count)).First().Key;
                AddSubsystemToNetwork(mostObservedMetabolitesSubsystem, network);
                extendedSubsystems.Add(mostObservedMetabolitesSubsystem);

                var exchangeSubsystems = subsystemsMetabolites.Keys.Intersect(FixedSubsystems).ToList();
                if (exchangeSubsystems.Any())
                {
                    exchangeSubsystems.ForEach(s => AddSubsystemToNetwork(s, network));
                    extendedSubsystems.AddRange(exchangeSubsystems);
                }
            }
            else
            {
                var species = Db.Cache.Species.Single(s => s.sbmlId == crossroadMetabolite.Key);
                network.Nodes.GetOrAdd(species.id, new HyperGraph.Node(species));
                network.AddSpeciesWithConnections(species);
            }


            var results = new Dictionary<string, string[]>();
            var rootResult = new Result();
            // calling ToList() to create a shallow clone
            Metabolitics(network, measuredMetabolites, extendedSubsystems.ToList(), new Dictionary<string, Dictionary<string, bool>>(), results, rootResult);

            ResultsUtils.SaveAsTree(rootResult);

            // Decision Tree
            // ResultsUtils.

            return results;
        }

        private static void Metabolitics(HyperGraph network, Dictionary<string, double> measuredMetabolites, List<string> extendedSubsystems, Dictionary<string, Dictionary<string, bool>> subsetsPath, Dictionary<string, string[]> results, Result parentResult)
        {
            network = CopyHyperGraph(network);
            network.Step++;
            Console.WriteLine($"\n************** Iteration {network.Step} *************** ");

            var borderMetabolites = GetBorderMetabolites(network, extendedSubsystems);
            // Console.WriteLine($"Border Mets: {string.Join(" ,", borderMetabolites.Select(b => b.Label))}");

            // reached a solution
            if (borderMetabolites.Count == 0)
            {
                results[$"solution-{counter}"] = subsetsPath.Values.SelectMany(s => s.Keys.Where(k => s[k] == true)).ToArray();
                var file = $"{Core.Dir}{counter}{string.Join("_", subsetsPath.Values.Select(s => string.Join(",", s.Keys.Where(k => s[k] == true).Select(sys => sys.Substring(0, 3)))))}";
                SaveAsDgs(network.Nodes.First().Value, network, extendedSubsystems, $"{file}_graph.dgs");
                SaveResults(network, $"{file}_results.txt");
                counter++;
                return;
            }


            var metabolitesSubsystems = new Dictionary<string, List<string>>();
            foreach (var m in borderMetabolites.Select(m => m.Label))
            {
                var metabolite = Db.Cache.Species.Single(s => s.sbmlId == m);
                var subsystems = metabolite.ReactionSpecies.Select(rs => rs.Reaction.subsystem).Distinct().Except(extendedSubsystems);
                var exchangeSubsystems = subsystems.Intersect(FixedSubsystems).ToList();
                if (exchangeSubsystems.Any())
                {
                    exchangeSubsystems.ForEach(s => AddSubsystemToNetwork(s, network));
                    extendedSubsystems.AddRange(exchangeSubsystems);
                }

                var remainingSubsystems = subsystems.Except(extendedSubsystems).ToList();
                if (remainingSubsystems.Any())
                    metabolitesSubsystems[m] = remainingSubsystems;
            }

            var metaboliteToExtend = metabolitesSubsystems.Where(ms => ms.Value.Count == metabolitesSubsystems.Min(ms2 => ms2.Value.Count)).First();

            network.Nodes.Values.ToList().ForEach(n => n.RecentlyAdded = false);
            network.Edges.Values.ToList().ForEach(r => r.RecentlyAdded = false);
            // extending the graph
            metaboliteToExtend.Value.ForEach(s => AddSubsystemToNetwork(s, network));
            extendedSubsystems.AddRange(metaboliteToExtend.Value);

            var subsets = ListSubSetsOf(metaboliteToExtend.Value);
            foreach (var working in subsets)
            {
                counter++;
                var result = new Result();
                result.num = counter;
                result.ActiveSubsystems = working;
                result.InactiveSubsystems = metaboliteToExtend.Value.Except(working).ToList();
                result.MetaboliteExtended = metaboliteToExtend.Key;
                result.ParentResult = parentResult;

                parentResult.ChildrenResults.Add(result);

                var subset = working.ToDictionary(k => k, v => true);
                result.InactiveSubsystems.ForEach(s => subset[s] = false);

                subsetsPath[metaboliteToExtend.Key] = subset;

                result.Solved = FBA(network, measuredMetabolites, metaboliteToExtend, subsetsPath);
                ResultsUtils.SaveSubsystemsHypergraph(network, extendedSubsystems, counter);

                if (!result.Solved)
                {
                    log.WriteLineAsync($"{string.Concat(Enumerable.Repeat("    |", network.Step))}✘ Infeas : ACT[{string.Join(" | ", result.ActiveSubsystems)}] INACT[{string.Join(" | ", result.InactiveSubsystems)}] {DateTime.Now.ToString("HH:mm:ss")}");
                    // Console.WriteLine($"Infeasible problem at subsets: {string.Join(" | ", subset)}");
                }
                else
                {
                    log.WriteLineAsync($"{string.Concat(Enumerable.Repeat("    |", network.Step))}✓ Feas : ACT[{string.Join(" | ", result.ActiveSubsystems)}] INACT[{string.Join(" | ", result.InactiveSubsystems)}] {DateTime.Now.ToString("HH:mm:ss")}");
                    Metabolitics(network, measuredMetabolites, extendedSubsystems.ToList(), subsetsPath.ToDictionary(k => k.Key, v => v.Value), results, result); // shallow cloning
                }

                subsetsPath.Remove(metaboliteToExtend.Key);
            }
        }

        private static void AddSubsystemToNetwork(string subsystem, HyperGraph network)
        {
            Console.WriteLine($"Adding subsystem: {subsystem}");
            var species = Db.Cache.Reactions.Where(r => r.subsystem == subsystem).SelectMany(r => r.ReactionSpecies).Select(rs => rs.Species);
            foreach (var s in species)
            {
                network.AddSpeciesWithConnections(s);
            }
        }

        public static List<HyperGraph.Node> GetBorderMetabolites(HyperGraph network, List<string> extendedSubsystems)
        {
            var borderMetabolites = new List<HyperGraph.Node>();
            foreach (var metabolite in network.Nodes.Values.Where(n => IsBorder(n, extendedSubsystems)))
                borderMetabolites.Add(metabolite);

            return borderMetabolites;
        }

        //public static List<HyperGraph.Edge> GetBorderReactions(HyperGraph network, List<string> extendedSubsystems)
        //{
        //    var borderReactions = new List<HyperGraph.Edge>();
        //    foreach (var reaction in network.Edges.Values.Where(r => IsBorder(r, extendedSubsystems)))
        //        borderReactions.Add(reaction);

        //    return borderReactions;
        //}

        public static void RemoveExchangeReactions(HyperGraph network)
        {
            foreach (var m in network.Nodes.Values)
            {
                if (m.Consumers.Any(c => c.IsPseudo) && m.Consumers.Count - 1 == m.ReactionCount.Consumers)
                    network.RemoveReaction(m.Consumers.Single(e => e.IsPseudo));

                if (m.Producers.Any(p => p.IsPseudo) && m.Producers.Count - 1 == m.ReactionCount.Producers)
                    network.RemoveReaction(m.Producers.Single(e => e.IsPseudo));
            }
        }

        public static IEnumerable<IEnumerable<T>> SubSetsOf<T>(IEnumerable<T> source)
        {
            if (!source.Any())
                return Enumerable.Repeat(Enumerable.Empty<T>(), 1);

            var element = source.Take(1);

            var haveNots = SubSetsOf(source.Skip(1));
            var haves = haveNots.Select(set => element.Concat(set));

            return haves.Concat(haveNots);
        }

        public static List<List<T>> ListSubSetsOf<T>(List<T> source)
        {
            if (!source.Any())
                return new List<List<T>> { new List<T>() };

            var element = source.Take(1);

            var haveNots = ListSubSetsOf(source.Skip(1).ToList());
            var haves = haveNots.Select(set => element.Concat(set).ToList()).ToList();

            return haves.Concat(haveNots).ToList();
        }

        public static bool FBA(HyperGraph network, Dictionary<string, double> measuredMetabolites, KeyValuePair<string, List<string>> metaboliteSubsystems, Dictionary<string, Dictionary<string, bool>> subsystemsPath)
        {
            var model = new Cplex { Name = "FBA" };
            var vars = new Dictionary<Guid, INumVar>();

            // make variables for all reactions
            foreach (var edge in network.Edges.Values)
            {
                vars[edge.Id] = model.NumVar(edge.LowerBound, edge.UpperBound, NumVarType.Float, edge.Label);
            }


            AddMetabolitesSteadyStateConstraints(network, model, vars);


            // Objective function
            var fobj = model.LinearNumExpr();

            foreach (var m in network.Nodes.Values.Where(m => measuredMetabolites.ContainsKey(m.Label)))
            {
                foreach (var p in m.Producers)
                {
                    fobj.AddTerm(measuredMetabolites[m.Label], vars[p.Id]);
                }
            }
            Console.WriteLine(fobj.ToString());
            model.Remove(model.GetObjective());
            model.AddObjective(ObjectiveSense.Maximize, fobj, "fobj");

            // subnetworks constraints
            AddSubnetworksConstraints(network, model, vars, subsystemsPath);

            // add measured metabolites rules
            AddMeasuredMetabolitesRules(network, model, vars, measuredMetabolites);

            var feasible = model.Solve();

            model.ExportModel($"{Core.Dir}{counter}model-{network.Step}-{(feasible ? "feasible" : "infeasible")}.lp");
            if (!feasible)
            {
                return false;
            }
            else
            {
                network.Edges.ToList().ForEach(d => d.Value.Flux = model.GetValue(vars[d.Value.Id]));
                return true;
            }
        }

        public static void AddMetabolitesSteadyStateConstraints(HyperGraph network, Cplex model, Dictionary<Guid, INumVar> vars)
        {
            foreach (var metabolite in network.Nodes.Values)
            {
                // cancel metabolites that are not balanced in steady state
                //if ((metabolite.AllReactions().Count() == 1) ||
                //    (!metabolite.AllReactions().Any(r => r.IsReversible) && (!metabolite.Producers.Any() || !metabolite.Consumers.Any()))
                //    )
                //{
                //    metabolite.AllReactions().ToList().ForEach(r => model.Add(vars[r.Id]));
                //    continue;
                //}

                var expr = model.LinearNumExpr();

                foreach (var reaction in metabolite.Producers)
                {
                    if (reaction.IsPseudo)
                    {
                        expr.AddTerm(Db.PseudoReactionStoichiometry, vars[reaction.Id]);
                    }
                    else
                    {
                        expr.AddTerm(metabolite.Weights[reaction.Id], vars[reaction.Id]);
                    }
                }

                foreach (var reaction in metabolite.Consumers)
                {
                    if (reaction.IsPseudo)
                    {
                        expr.AddTerm(-1 * Db.PseudoReactionStoichiometry, vars[reaction.Id]);
                    }
                    else
                    {
                        expr.AddTerm(metabolite.Weights[reaction.Id], vars[reaction.Id]);
                    }
                }

                model.AddEq(expr, 0.0, metabolite.Label);
            }
        }

        private static void AddSubnetworksConstraints(HyperGraph network, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<string, Dictionary<string, bool>> subsystemsPath)
        {
            foreach (var sub in subsystemsPath)
            {
                var connections = Db.Cache.ReactionSpecies.Where(rs => rs.Species.sbmlId == sub.Key && sub.Value.Keys.Contains(rs.Reaction.subsystem)).ToList();
                var or = model.Or();
                foreach (var con in connections)
                {
                    if (sub.Value[con.Reaction.subsystem])
                        or.Add(model.Ge(model.Abs(vars[con.reactionId]), TActive));
                    else
                        model.Add(model.Le(model.Abs(vars[con.reactionId]), TInactive));
                }
                if (sub.Value.Values.Any(s => s == true))
                    model.Add(or);
            }
        }

        private static void AddMeasuredMetabolitesRules(HyperGraph network, Cplex model, Dictionary<Guid, INumVar> vars, Dictionary<string, double> measuredMetabolites)
        {
            foreach (var m in measuredMetabolites)
            {
                /* Rule 1: 
                 * If a metabolite is observed to increase (in comparison to the reference value), 
                 * then the total production flux going through this metabolite cannot be less than the inactive threshold that we use.
                */
                if (m.Value > 0)
                {
                    var metabolite = network.Nodes.Values.SingleOrDefault(n => n.Label == m.Key);
                    if (metabolite == null || !metabolite.Consumers.Any(r => r.UpperBound != 0) || !metabolite.Producers.Any(r => r.UpperBound != 0)) continue;
                    var exp = model.LinearNumExpr();
                    foreach (var r in metabolite.AllReactions())
                    {
                        exp.AddTerm(1, vars[r.Id]);
                    }
                    model.AddGe(exp, 2 * TInactive, $"rule1_{m.Key}");
                }

                /*
                 * Rule 2:
                 * If a metabolite's concentration is measured close to 0 (below some threshold that we may arbitrarily set at this initial stage), 
                 * then the total production flux going through this metabolite cannot be more than the inactive threshold that we use.
                */
                if (m.Value == 0)
                {
                    var metabolite = network.Nodes.Values.SingleOrDefault(n => n.Label == m.Key);
                    if (metabolite == null || !metabolite.Consumers.Any() || !metabolite.Producers.Any()) continue;
                    var exp = model.LinearNumExpr();
                    foreach (var r in metabolite.AllReactions())
                    {
                        exp.AddTerm(1, vars[r.Id]);
                    }
                    model.AddLe(exp, TInactive, $"rule2_{m.Key}");
                }
            }
        }


        private static HyperGraph CopyHyperGraph(HyperGraph hyperGraph)
        {
            var memoryStream = new System.IO.MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, hyperGraph);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return formatter.Deserialize(memoryStream) as HyperGraph;
        }

        public static void SaveAsDgs(HyperGraph.Node mi, HyperGraph graph, List<string> extendedSubsystems, string file)
        {
            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes" };
            lines.AddRange(graph.Nodes.Values
                .Select(node => new { node, type = IsBorder(node, extendedSubsystems) ? NodeType.Border : node.RecentlyAdded ? NodeType.New : NodeType.None })
                .Select(@t => @t.node.ToDgs(@t.type)));

            lines.Add("#Hyperedges");
            foreach (var edge in graph.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.RecentlyAdded == true)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type, graph.Edges.Values.Max(e => Math.Abs(e.Flux))));
            }

            File.AppendAllLines(file, lines);
        }

        public static void SaveResults(HyperGraph network, string file)
        {
            var list = network.Edges.ToList().Select(d => $"{d.Value.Label}:{d.Value.Flux}").ToList();
            list.Sort((decision, decision1) => string.Compare(decision, decision1, StringComparison.Ordinal));
            File.WriteAllLines(file, list);
        }

        public static bool IsBorder (HyperGraph.Node n, List<string> extendedSubsystems)
        {
            return n.Producers.Union(n.Consumers).Any(r => IsBorder(r, extendedSubsystems));
        }

        public static bool IsBorder (HyperGraph.Edge r, List<string> extendedSubsystems)
        {
            return (!extendedSubsystems.Contains(r.Subsystem)) || r.Products.Count + r.Reactants.Count != Db.Cache.ReactionSpecies.Count(rs => rs.reactionId == r.Id);
        }
    }
}
