using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PathwaysLib.ServerObjects;

namespace Metabol
{
    public class TheAlgorithm
    {
        internal readonly Fba Fba = new Fba();

        internal void RemoveExchangeReaction(HGraph sm, HGraph.Node m2)
        {
            // remove producer exchange reaction of  non-produced-border metabolite            
            //dont remove producer exchange reaction if producer exchange reaction is only producer reaction
            var removeox = !m2.IsProducedBorder && m2.OutputFromEdge.Any(edge => edge.IsImaginary) && m2.OutputFromEdge.Count(edge => !edge.IsImaginary) != 0;

            //var totalFlux = Fba.RemovedExchangeFlux.ContainsKey(m2.Id) ? Fba.RemovedExchangeFlux[m2.Id] : 0;
            if (removeox)
            {
                var outex = m2.OutputFromEdge.First(s => s.IsImaginary);
                Fba.RemovedProducerExchange[m2.Id] = Fba.Results[outex.Id];
                foreach (var edge in m2.OutputFromEdge.Where(edge => !edge.IsImaginary && edge.Level < sm.LastLevel))
                    Fba.RemovedProducerExchange[m2.Id] = Fba.RemovedProducerExchange[m2.Id] + Fba.Results[edge.Id];

                //totalFlux += Fba.Results[outex.Id];
                m2.OutputFromEdge.Remove(outex);
                //Fba.Results.Remove(outex.Id);
                HGraph.Edge ee1;
                sm.Edges.TryRemove(outex.Id, out ee1);
                //HashSet<Guid> v;
                //Fba.UpdateExchangeConstraint.TryRemove(outex.Id, out v);
            }

            // remove consumer exchange reaction of  non-consumed-border metabolite 
            //dont remove consumer exchange reaction if consumer exchange reaction is only consumer reaction
            var removeix = !m2.IsConsumedBorder && m2.InputToEdge.Any(edge => edge.IsImaginary) && m2.InputToEdge.Count(edge => !edge.IsImaginary) != 0;

            if (!removeix) return;
            var inex = m2.InputToEdge.First(s => s.IsImaginary);
            Fba.RemovedConsumerExchange[m2.Id] = Fba.Results[inex.Id];
            foreach (var edge in m2.InputToEdge.Where(edge => !edge.IsImaginary && edge.Level < sm.LastLevel))
                Fba.RemovedConsumerExchange[m2.Id] = Fba.RemovedConsumerExchange[m2.Id] + Fba.Results[edge.Id];
            //totalFlux = Fba.Results[inex.Id];
            m2.InputToEdge.Remove(inex);
            //Fba.Results.Remove(inex.Id);
            HGraph.Edge e;
            sm.Edges.TryRemove(inex.Id, out e);
            //HashSet<Guid> v;
            //Fba.UpdateExchangeConstraint.TryRemove(inex.Id, out v);

            //if (removeix || removeox)
            //    Fba.RemovedExchangeFlux[m2.Id] = totalFlux;
        }

        internal void UpdateNeighbores(HGraph sm, HGraph.Node m2)
        {
            var inex = m2.InputToEdge.Any(e => e.IsImaginary) ? m2.InputToEdge.First(s => s.IsImaginary) : null;
            var outex = m2.OutputFromEdge.Any(e => e.IsImaginary) ? m2.OutputFromEdge.First(s => s.IsImaginary) : null;
            if (inex != null)
                foreach (var s in m2.InputToEdge.Where(s => !s.IsImaginary))
                {
                    if (Fba.UpdateExchangeConstraint.ContainsKey(s.Id))
                        Fba.UpdateExchangeConstraint[inex.Id] = new HashSet<Guid>();

                    Fba.UpdateExchangeConstraint[inex.Id].Add(s.Id);
                }

            if (outex != null)
                foreach (var s in m2.OutputFromEdge.Where(s => !s.IsImaginary))
                {
                    if (Fba.UpdateExchangeConstraint.ContainsKey(s.Id))
                        Fba.UpdateExchangeConstraint[outex.Id] = new HashSet<Guid>();

                    Fba.UpdateExchangeConstraint[outex.Id].Add(s.Id);
                }
        }

        internal Iteration ApplyFba(HGraph sm, Dictionary<Guid, int> z, int iteration)
        {
            //4. Let a metabolite mb be labeled as a border metabolite 
            // if there is at least one consumer or producer of mb that is not included in the current subnetwork, S(m). 
            var borderm = GetBorderMetabolites(sm);
            //borderm.Remove(sm.Nodes[m.ID]);

            // Define exchange reactions for all border metabolites in S(m).
            Task.WaitAll(borderm.Select(mb => Util.CachedS(mb.Id)).Select(mbs => DefineExReaction(mbs, sm)).ToArray());

            Fba.Label = Util.FbaLabel();
            var nonborder = GetNonBorderMetabolites(sm);
            var reactions = sm.Edges.Values.Select(ToReaction).ToList();

            var dic = new Dictionary<Guid, int>();
            foreach (var guid in z.Select(s => s.Key).Intersect(nonborder.Select(n => n.Id)))
                dic[guid] = z[guid];

            //5. Apply Flux Balance Analysis on S(m) with the objective function F defined as follows. 
            //For  each non-border metabolite  m  that  is  included  in  both S(m)and  Z,  perform  the following checks:
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

        internal void SaveAsDgs(HGraph.Node m2, HGraph sm, string label)
        {
            var file = Util.Dir + label + "graph.dgs";
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

                lines.Add(edge.ToDgs(type, this));
            }
            File.AppendAllLines(file, lines);
        }

        internal Fba.Reaction ToReaction(HGraph.Edge edge1)
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
                    Reactants = GetMetabolites(edge1, Util.Reactant),
                    Products = GetMetabolites(edge1, Util.Product),
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
                    Reactants = GetMetabolites(re, Util.Reactant),
                    Products = GetMetabolites(re, Util.Product),
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

            if (sm.Nodes[m.ID].IsProducedBorder || sm.Nodes[m.ID].AllReactions.Item1 == 0)
            {
                var r = Guid.NewGuid();
                sm.AddOuputNode(r, Util.ExchangeLabel(), m.ID, m.SbmlId, true);
            }

            if (sm.Nodes[m.ID].IsConsumedBorder || sm.Nodes[m.ID].AllReactions.Item2 == 0)
            {
                var r = Guid.NewGuid();
                sm.AddInputNode(r, Util.ExchangeLabel(), m.ID, m.SbmlId, true);
            }

            await Task.Delay(TimeSpan.Zero);

        }

        internal static async Task DefineExReactionLonely(HGraph.Node m, HGraph sm)
        {
            if (!m.IsLonely) return;

            if (m.OutputFromEdge.Count == 0)
            {
                var r = Guid.NewGuid();
                sm.AddOuputNode(r, Util.ExchangeLabel(), m.Id, m.Label, true);
            }
            else if (m.InputToEdge.Count == 0)
            {
                var r = Guid.NewGuid();
                sm.AddInputNode(r, Util.ExchangeLabel(), m.Id, m.Label, true);
            }

            await Task.Delay(TimeSpan.Zero);
        }

        internal static Dictionary<Guid, Fba.MetaboliteWithStoichiometry> GetMetabolites(HGraph.Edge react, string role)
        {
            if (!react.InputNodes.IsEmpty && role == Util.Reactant)
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
            if (!react.OuputNodes.IsEmpty && role == Util.Product)
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
                        Metabolite = new Fba.Metabolite(Util.CachedS(s.SpeciesId)),
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

        internal async Task ExtendGraph(ServerSpecies m, HGraph sm)
        {
            //Task.Run(() =>
            {
                foreach (var r in m.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
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
                foreach (var r in m.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
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

            foreach (
                var node in
                    sm.Nodes[m.ID].AllNeighborNodes().Where(node => node.IsBorder))
            {
                UpdateNeighbores(sm, node);
                RemoveExchangeReaction(sm, node);
            }

        }

        //involved in the smallest total number of reactions
        internal static HGraph.Node LonelyMetabolite(ICollection<HGraph.Node> borderm)
        {
            var nodes = borderm.OrderBy(Util.TotalReactions).ToList();
            return nodes.First();
        }
    }
}