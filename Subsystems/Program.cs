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

namespace Subsystems
{
    class Program
    {
        const double TActive = 0.5;
        const double TInactive = 0.0001;
        static int counter = 0;

        static void Main(string[] args)
        {
            Console.BufferHeight = Int16.MaxValue - 1;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Start();
        }

        private static void Start()
        {
            var network = new HyperGraph();
            var extendedSubsystems = new List<string>();
            var measuredMetabolites = new Dictionary<string, double>
            {
                ["acon_C_c"] = 1,
                //["o2_c"] = -1
            };

            var metabolitesSubsystems = new Dictionary<string, List<string>>();
            foreach (var m in measuredMetabolites.Keys)
            {
                var metabolite = Db.Context.Species.Single(s => s.sbmlId == m);
                metabolitesSubsystems[m] = metabolite.ReactionSpecies.Select(rs => rs.Reaction.subsystem).Distinct().ToList();
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
            }
            else
            {
                var species = Db.Context.Species.Single(s => s.sbmlId == crossroadMetabolite.Key);
                network.Nodes.GetOrAdd(species.id, new HyperGraph.Node(species, network.Step));
            }


            // calling ToList() to create a shallow clone
            Metabolitics(network, measuredMetabolites, extendedSubsystems.ToList(), new Dictionary<string, Dictionary<string, bool>>());
        }

        private static void Metabolitics(HyperGraph network, Dictionary<string, double> measuredMetabolites, List<string> extendedSubsystems, Dictionary<string, Dictionary<string, bool>> subsetsPath)
        {
            network = CopyHyperGraph(network);
            network.Step++;
            Console.WriteLine($"\n************** Iteration {network.Step} *************** ");

            var borderMetabolites = GetBorderMetabolites(network, extendedSubsystems);
            Console.WriteLine($"Border Mets: {string.Join(" ,", borderMetabolites.Select(b => b.Label))}");
            if (borderMetabolites.Count == 0)
            {
                var file = $"{Core.Dir}{counter}{string.Join("_", subsetsPath.Values.Select(s => string.Join(",", s.Keys.Where(k => s[k] == true).Select(sys => sys.Substring(0, 3)))))}";
                SaveAsDgs(network.Nodes.First().Value, network, extendedSubsystems, $"{file}_graph.dgs");
                SaveResults(network, $"{file}_results.txt");
                counter++;
                return;
            }


            var metabolitesSubsystems = new Dictionary<string, List<string>>();
            foreach (var m in borderMetabolites.Select(m => m.Label))
            {
                var metabolite = Db.Context.Species.Single(s => s.sbmlId == m);
                metabolitesSubsystems[m] = metabolite.ReactionSpecies.Select(rs => rs.Reaction.subsystem).Distinct().Except(extendedSubsystems).ToList();
            }

            var metaboliteToExtend = metabolitesSubsystems.Where(ms => ms.Value.Count == metabolitesSubsystems.Min(ms2 => ms2.Value.Count)).First();

            network.Nodes.Values.ToList().ForEach(n => n.RecentlyAdded = false);
            network.Edges.Values.ToList().ForEach(r => r.RecentlyAdded = false);
            // extending the graph
            metaboliteToExtend.Value.ForEach(s => AddSubsystemToNetwork(s, network));
            extendedSubsystems.AddRange(metaboliteToExtend.Value);

            var count = 0;
            var subsets = ListSubSetsOf(metaboliteToExtend.Value);
            foreach (var working in subsets)
            {
                count++;
                var notworking = metaboliteToExtend.Value.Except(working).ToList();
                var subset = working.ToDictionary(k => k, v => true);
                notworking.ForEach(s => subset[s] = false);

                subsetsPath[metaboliteToExtend.Key] = subset;

                var feasible = FBA(network, measuredMetabolites, metaboliteToExtend, subsetsPath);
                if (!feasible)
                {
                    Console.WriteLine($"Infeasible problem at subsets: {string.Join(" | ", subset)}");
                }
                else
                {
                    Metabolitics(network, measuredMetabolites, extendedSubsystems.ToList(), subsetsPath.ToDictionary(k => k.Key, v => v.Value)); // shallow cloning
                }

                subsetsPath.Remove(metaboliteToExtend.Key);
            }
        }

        private static void AddSubsystemToNetwork(string subsystem, HyperGraph network)
        {
            Console.WriteLine($"Adding subsystem: {subsystem}");
            var species = Db.Context.Reactions.Where(r => r.subsystem == subsystem).SelectMany(r => r.ReactionSpecies).Select(rs => rs.Species);
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

        private static void DefineExchangeReactions(HyperGraph.Node metabolite, HyperGraph network)
        {
            if (metabolite.Producers.Count != metabolite.ReactionCount.Producers && !metabolite.Producers.Any(s => s.IsPseudo))
                network.AddProduct(Guid.NewGuid(), $"_exr_{metabolite.Label}_prod", false, true, metabolite.Id, metabolite.Label);

            if (metabolite.Consumers.Count != metabolite.ReactionCount.Consumers && !metabolite.Consumers.Any(s => s.IsPseudo))
                network.AddReactant(Guid.NewGuid(), $"_exr_{metabolite.Label}_cons", false, true, metabolite.Id, metabolite.Label);
        }

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
                var reactionBounds = Db.Context.ReactionBounds.Single(rb => rb.reactionId == edge.Id);
                var fixedbounds = Db.Context.ReactionBoundFixes.SingleOrDefault(rbf => rbf.reactionId == edge.Id);

                if (fixedbounds != null)
                {
                    vars[edge.Id] = model.NumVar(fixedbounds.lowerbound, fixedbounds.upperbound, NumVarType.Float,
                        edge.Label);
                }
                else
                {
                    vars[edge.Id] = model.NumVar(reactionBounds.lowerBound, reactionBounds.upperBound,
                        NumVarType.Float, edge.Label);
                }
            }


            AddMetabolitesStableStateConstraints(network, model, vars);


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


            var feasible = model.Solve();

            // model.ExportModel($"{Core.Dir}{network.Step}-{count}model-{(feasible ? "feasible" : "infeasible")}.lp");
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

        private static void AddMetabolitesStableStateConstraints(HyperGraph network, Cplex model, Dictionary<Guid, INumVar> vars)
        {
            foreach (var metabolite in network.Nodes.Values)
            {
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
                var connections = Db.Context.ReactionSpecies.Where(rs => rs.Species.sbmlId == sub.Key && sub.Value.Keys.Contains(rs.Reaction.subsystem)).ToList();
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
            return (!extendedSubsystems.Contains(r.Subsystem)) || r.Products.Count + r.Reactants.Count != Db.Context.ReactionSpecies.Count(rs => rs.reactionId == r.Id);
        }
    }
}
