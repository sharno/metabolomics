using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    public class Util
    {
        private static readonly MemoryCache CacheSpecieses2 = new MemoryCache("species");
        private static readonly MemoryCache CacheReactions2 = new MemoryCache("reactions");

        //private static readonly ConcurrentDictionary<Guid, ServerSpecies> CacheSpecieses = new ConcurrentDictionary<Guid, ServerSpecies>();
        //private static readonly ConcurrentDictionary<Guid, ServerReaction> CacheReaction = new ConcurrentDictionary<Guid, ServerReaction>();
        //private static readonly ConcurrentDictionary<Guid, Fba.Reaction> CacheReaction2 = new ConcurrentDictionary<Guid, Fba.Reaction>();

        internal static Dictionary<Guid, Tuple<int, int>> AllReactionCache = new Dictionary<Guid, Tuple<int, int>>();
        internal static readonly Dictionary<string, Fba.Metabolite> Meta = new Dictionary<string, Fba.Metabolite>();
        internal static readonly Fba Fba = new Fba();
        internal const string Product = "Product";
        internal const string Reactant = "Reactant";
        internal const string Modifier = "Modifier";

        internal static readonly string Dir = ConfigurationManager.AppSettings["modelOutput"];
        internal static readonly string AllReactionFile = ConfigurationManager.AppSettings["allReaction"];
        internal static readonly string SelectedMetaFile = ConfigurationManager.AppSettings["selection"];

        static Util()
        {
            Fba.Label = FbaLabel();
            //var list = (from species in ServerSpecies.AllSpecies()
            //            let reactant = species.getAllReactions(Util.Product).Length
            //            let product = species.getAllReactions(Util.Reactant).Length
            //            select $"{species.ID},{reactant},{product}").ToList();
            //File.AppendAllLines(Util.AllReactionFile, list);


            var lines = File.ReadAllLines(AllReactionFile);
            foreach (var result in lines.Select(s => s.Split(',')))
                AllReactionCache[Guid.Parse(result[0])] = new Tuple<int, int>(int.Parse(result[1]), int.Parse(result[2]));//int.Parse(result[1]);
        }

        internal static void RemoveExchangeReaction(HGraph sm, HGraph.Node m2)
        {
            #region eski

            //double totalFlux = 0;
            //var inex = m2.InputToEdge.Count(s => s.IsImaginary) != m2.InputToEdge.Count;
            //var outex = m2.OutputFromEdge.Count(s => s.IsImaginary) != m2.OutputFromEdge.Count;

            //foreach (var s in m2.InputToEdge.Where(s => s.IsImaginary && inex))
            //{
            //    HGraph.Edge e;
            //    sm.Edges.TryRemove(s.Id, out e);
            //    totalFlux -= Fba.Results[s.Id];
            //}
            //m2.InputToEdge.RemoveWhere(s => s.IsImaginary && inex);

            //foreach (var s in m2.OutputFromEdge.Where(s => s.IsImaginary && outex))
            //{
            //    HGraph.Edge e;
            //    sm.Edges.TryRemove(s.Id, out e);
            //    totalFlux += Fba.Results[s.Id];
            //}
            //m2.OutputFromEdge.RemoveWhere(s => s.IsImaginary && outex);

            //if (inex || outex)
            //    Fba.RemovedExchangeFlux[m2.Id] = totalFlux;

            #endregion
            var inex = m2.InputToEdge.First(s => s.IsImaginary);
            var outex = m2.OutputFromEdge.First(s => s.IsImaginary);

            //dont remove consumer exchange reaction if consumer exchange reaction is only consumer reaction
            var removeix = m2.InputToEdge.Count(s => s.IsImaginary) != m2.InputToEdge.Count;
            //dont remove producer exchange reaction if producer exchange reaction is only producer reaction
            var removeox = m2.OutputFromEdge.Count(s => s.IsImaginary) != m2.OutputFromEdge.Count;

            var totalFlux = 0.0;
            if (removeox)
            {
                totalFlux += Fba.Results[inex.Id];
                m2.OutputFromEdge.Remove(outex);
                Fba.Results.Remove(outex.Id);
                HGraph.Edge e;
                sm.Edges.TryRemove(outex.Id, out e);
                HashSet<Guid> v;
                Fba.UpdateExchangeConstraint.TryRemove(outex.Id, out v);
            }

            if (removeix)
            {
                totalFlux -= Fba.Results[inex.Id];
                m2.InputToEdge.Remove(inex);
                Fba.Results.Remove(inex.Id);
                HGraph.Edge e;
                sm.Edges.TryRemove(inex.Id, out e);
                HashSet<Guid> v;
                Fba.UpdateExchangeConstraint.TryRemove(inex.Id, out v);
            }

            if (removeix || removeox)
                Fba.RemovedExchangeFlux[m2.Id] = totalFlux;
        }

        internal static void UpdateExchangeReaction(HGraph sm, HGraph.Node m2)
        {
            var inex = m2.InputToEdge.First(s => s.IsImaginary);
            var outex = m2.OutputFromEdge.First(s => s.IsImaginary);

            //dont remove consumer exchange reaction if consumer exchange reaction is only consumer reaction
            var removeix = m2.InputToEdge.Count(s => s.IsImaginary) != m2.InputToEdge.Count;
            //dont remove producer exchange reaction if producer exchange reaction is only producer reaction
            var removeox = m2.OutputFromEdge.Count(s => s.IsImaginary) != m2.OutputFromEdge.Count;

            foreach (var s in m2.InputToEdge.Where(s => !s.IsImaginary && !removeix))
            {
                if (Fba.UpdateExchangeConstraint.ContainsKey(s.Id))
                    Fba.UpdateExchangeConstraint[inex.Id] = new HashSet<Guid>();

                Fba.UpdateExchangeConstraint[inex.Id].Add(s.Id);
            }

            foreach (var s in m2.OutputFromEdge.Where(s => !s.IsImaginary && !removeox))
            {
                if (Fba.UpdateExchangeConstraint.ContainsKey(s.Id))
                    Fba.UpdateExchangeConstraint[outex.Id] = new HashSet<Guid>();

                Fba.UpdateExchangeConstraint[outex.Id].Add(s.Id);
            }

            var totalFlux = Fba.RemovedExchangeFlux.ContainsKey(m2.Id) ? Fba.RemovedExchangeFlux[m2.Id] : 0;
            if (removeox)
            {
                totalFlux += Fba.Results[inex.Id];
                m2.OutputFromEdge.Remove(outex);
                Fba.Results.Remove(outex.Id);
                HGraph.Edge e;
                sm.Edges.TryRemove(outex.Id, out e);
                HashSet<Guid> v;
                Fba.UpdateExchangeConstraint.TryRemove(outex.Id, out v);
            }

            if (removeix)
            {
                totalFlux -= Fba.Results[inex.Id];
                m2.InputToEdge.Remove(inex);
                Fba.Results.Remove(inex.Id);
                HGraph.Edge e;
                sm.Edges.TryRemove(inex.Id, out e);
                HashSet<Guid> v;
                Fba.UpdateExchangeConstraint.TryRemove(inex.Id, out v);
            }

            if (removeix || removeox)
                Fba.RemovedExchangeFlux[m2.Id] = totalFlux;
        }

        internal static ServerReaction CachedR(Guid id)
        {
            if (CacheReactions2.Contains(id.ToString()))
                return (ServerReaction)CacheReactions2.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = ServerReaction.Load(id);
            CacheReactions2.Add(id.ToString(), ss, policy);
            return ss;
        }

        internal static ServerSpecies CachedS(Guid id)
        {
            if (CacheSpecieses2.Contains(id.ToString()))
                return (ServerSpecies)CacheSpecieses2.Get(id.ToString());

            var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(6) };
            var ss = ServerSpecies.Load(id);
            CacheSpecieses2.Add(id.ToString(), ss, policy);
            return ss;
        }

        internal static Iteration ApplyFba(HGraph sm, Dictionary<Guid, int> z, int iteration)
        {
            //4. Let a metabolite mb be labeled as a border metabolite 
            // if there is at least one consumer or producer of mb that is not included in the current subnetwork, S(m). 
            var borderm = GetBorderMetabolites(sm);
            //borderm.Remove(sm.Nodes[m.ID]);

            // Define exchange reactions for all border metabolites in S(m).
            Task.WaitAll(borderm.Select(mb => CachedS(mb.Id)).Select(mbs => DefineExReaction(mbs, sm)).ToArray());

            //5. Apply Flux Balance Analysis on S(m) with the objective function F defined as follows. 
            //For  each non-border metabolite  m  that  is  included  in  both S(m)and  Z,  perform  the following checks:
            Fba.Label = FbaLabel();
            var nonborder = GetNonBorderMetabolites(sm);
            var reactions = sm.Edges.Values.Select(ToReaction).ToList();

            var dic = new Dictionary<Guid, int>();
            foreach (var guid in z.Select(s => s.Key).Intersect(nonborder.Select(n => n.Id)))
                dic[guid] = z[guid];

            var timer = new Stopwatch();
            timer.Start();
            var f = Fba.Solve(reactions, dic, sm);
            Fba.LastRuntime = timer.ElapsedMilliseconds * 1.0 / 1000.0;
            timer.Stop();
            var it = new Iteration(iteration)
            {
                Fba = f ? 1 : 0,
                Time = Fba.LastRuntime,
                //BorderMCount = borderm.Count,
                Nodes = sm.JsonNodes(z),
                Links = sm.JsonLinks()
            };
            //sm.NextStep();
            return it;
        }

        private static string FbaLabel()
        {
            return DateTime.Now.ToFileTimeUtc() + "";
        }

        internal static void SaveAsDgs(HGraph.Node m2, HGraph sm, string label)
        {
            var file = Dir + label + "graph.dgs";
            var maxLevel = sm.Nodes.Max(n => n.Value.Level);//Math.Max(sm.Nodes.Max(n => n.Value.Level), sm.Edges.Max(e => e.Value.Level));

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes", m2.ToDgs(NodeType.Selected) };

            foreach (var node in sm.Nodes.Values)
            {
                if (node.Id == m2.Id)
                    //type = NodeType.Selected;
                    continue;

                var type = NodeType.None;
                if (node.Level == maxLevel && node.IsBorder)
                    type = NodeType.NewBorder;
                //else if (node.Level == maxLevel)
                //    type = NodeType.New;
                else if (node.IsBorder)
                    type = NodeType.Border;

                lines.Add(node.ToDgs(type));
            }

            lines.Add("#Hyperedges");
            foreach (var edge in sm.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type));
            }
            File.AppendAllLines(file, lines);
        }

        internal static Fba.Reaction ToReaction(HGraph.Edge edge1)
        {
            //if (CacheReaction2.ContainsKey(edge1.Id))
            //    return CacheReaction2[edge1.Id];

            if (edge1.IsImaginary)
            {
                var rea = new Fba.Reaction
                {
                    Id = edge1.Id,
                    Name = edge1.Label,
                    Reversible = false,
                    Reactants = GetMetabolites(edge1, Reactant),
                    Products = GetMetabolites(edge1, Product),
                    Level = edge1.Level
                };
                //CacheReaction2[edge1.Id] = rea;
                return rea;
            }
            else
            {
                var re = ServerReactionSpecies.GetAllReactionsSpeciesForOneReaction(edge1.Id);
                var rea = new Fba.Reaction
                {
                    Id = edge1.Id,
                    Name = Regex.Replace(edge1.ToServerReaction.SbmlId, "[^0-9a-zA-Z]+", "_"),
                    Reversible = edge1.ToServerReaction.Reversible,
                    Reactants = GetMetabolites(re, Reactant),
                    Products = GetMetabolites(re, Product),
                    Level = edge1.Level
                };
                //CacheReaction2[edge1.Id] = rea;
                return rea;
            }
        }

        internal static async Task DefineExReaction(ServerSpecies m, HGraph sm)
        {
            if (sm.Nodes.ContainsKey(m.ID) && sm.Nodes[m.ID].InputToEdge.Any(s => s.IsImaginary) ||
                sm.Nodes.ContainsKey(m.ID) && sm.Nodes[m.ID].OutputFromEdge.Any(s => s.IsImaginary))
                return;

            var r = Guid.NewGuid();
            sm.AddOuputNode(r, ExchangeLabel(), m.ID, m.SbmlId, true);

            r = Guid.NewGuid();
            sm.AddInputNode(r, ExchangeLabel(), m.ID, m.SbmlId, true);
            await Task.Delay(TimeSpan.Zero);
        }

        internal static async Task DefineExReactionLonely(HGraph.Node m, HGraph sm)
        {
            if (!m.IsLonely) return;

            if (m.OutputFromEdge.Count == 0)
            {
                var r = Guid.NewGuid();
                sm.AddOuputNode(r, ExchangeLabel(), m.Id, m.Label, true);
            }
            else if (m.InputToEdge.Count == 0)
            {
                var r = Guid.NewGuid();
                sm.AddInputNode(r, ExchangeLabel(), m.Id, m.Label, true);
            }

            await Task.Delay(TimeSpan.Zero);
        }

        internal static Dictionary<Guid, Fba.MetaboliteWithStoichiometry> GetMetabolites(HGraph.Edge react, string role)
        {
            if (!react.InputNodes.IsEmpty && role == Reactant)
            {
                return new Dictionary<Guid, Fba.MetaboliteWithStoichiometry>
                {
                    { react.InputNodes.First().Value.Id,
                    new Fba.MetaboliteWithStoichiometry
                    {
                        Metabolite = new Fba.Metabolite(react.InputNodes.First().Value.ToSpecies),
                        Stoichiometry = 1
                    }}
                };
            }
            if (!react.OuputNodes.IsEmpty && role == Product)
            {
                return new Dictionary<Guid, Fba.MetaboliteWithStoichiometry>
                {
                    {react.OuputNodes.First().Value.Id,
                    new Fba.MetaboliteWithStoichiometry
                    {
                        Metabolite = new Fba.Metabolite(react.OuputNodes.First().Value.ToSpecies),
                        Stoichiometry = 1
                    }}
                };
            }

            return new Dictionary<Guid, Fba.MetaboliteWithStoichiometry>();
        }

        internal static Dictionary<Guid, Fba.MetaboliteWithStoichiometry> GetMetabolites(IEnumerable<ServerReactionSpecies> re, string role)
        {
            return (from s in re
                    where ServerReactionSpeciesRole.Load(s.RoleId).Role == role
                    select new Fba.MetaboliteWithStoichiometry
                    {
                        Metabolite = new Fba.Metabolite(CachedS(s.SpeciesId)),
                        Stoichiometry = s.Stoichiometry
                    }).ToDictionary(s => s.Metabolite.Id);
        }

        internal static HashSet<HGraph.Node> GetBorderMetabolites(HGraph sm)
        {
            var borderm = new HashSet<HGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => node.IsBorder))
                borderm.Add(node);
            return borderm;
        }

        internal static HashSet<HGraph.Node> GetNonBorderMetabolites(HGraph sm)
        {
            var borderm = new HashSet<HGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsBorder))
                borderm.Add(node);
            return borderm;
        }

        internal static async Task ExtendGraph(ServerSpecies m, HGraph sm)
        {
            //Task.Run(() =>
            {
                foreach (var r in m.getAllReactions(Product).Where(r => r.SbmlId != "R_biomass_reaction"))
                {
                    sm.AddOuputNode(r.ID, r.SbmlId, m.ID, m.SbmlId);

                    var products = r.GetAllProducts();
                    var reactant = r.GetAllReactants();

                    foreach (var p in reactant)
                        sm.AddInputNode(r.ID, r.SbmlId, p.ID, p.SbmlId);

                    foreach (var p in products)
                        sm.AddOuputNode(r.ID, r.SbmlId, p.ID, p.SbmlId);
                }
            }
            //);

            //Task.Run(() =>
            {
                foreach (var r in m.getAllReactions(Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
                {
                    sm.AddInputNode(r.ID, r.SbmlId, m.ID, m.SbmlId);

                    var products = r.GetAllProducts();
                    var reactant = r.GetAllReactants();

                    foreach (var p in reactant)
                        sm.AddInputNode(r.ID, r.SbmlId, p.ID, p.SbmlId);

                    foreach (var p in products)
                        sm.AddOuputNode(r.ID, r.SbmlId, p.ID, p.SbmlId);
                }
            }
            //);

            // add exchange reaction to lonely(metabol. with only input or output reactions) metabolites   
            foreach (var lon in sm.Nodes.Values.Where(n => n.IsLonely))
                await DefineExReactionLonely(lon, sm);

            foreach (var node in sm.Nodes[m.ID].AllNeighborNodes().Where(node => node.IsInBorder || node.IsOutBorder))
                UpdateExchangeReaction(sm, node);

        }

        internal static string ExchangeLabel()
        {
            return "exr" + ((Guid.NewGuid().GetHashCode() & 0x7fffffff) % 99999999);
        }

        internal static int TotalReactions(HGraph.Node k)
        {
            var sum = k.AllReactions.Item1 + k.AllReactions.Item2;
            return sum == 0 ? int.MaxValue : sum;
        }

        internal static int TotalReactions(Guid id)
        {
            var sum = AllReactionCache[id].Item1 + AllReactionCache[id].Item2;
            //if sum==0, k is probably Modifier
            return sum == 0 ? int.MaxValue : sum;
        }

        //involved in the smallest total number of reactions
        internal static HGraph.Node LonelyMetabolite(ICollection<HGraph.Node> borderm)
        {
            var nodes = borderm.OrderBy(TotalReactions).ToList();
            return nodes.First();
        }

    }
}