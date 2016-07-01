using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Metabol.DbModels;
using Metabol.DbModels.Models;
using Metabol.DbModels.DB2;
using Db = Metabol.DbModels.Db;

namespace Metabol
{
    public class TheAlgorithm
    {
        public readonly FbaMoma Fba = new FbaMoma();
        private const int CommonMetabolite = 61;
        public bool IsFeasable { get; set; }

        public readonly HyperGraph Sm = new HyperGraph();

        private bool init;

        public int Iteration = 1;

        public Dictionary<Guid, int> Z = new Dictionary<Guid, int>();

        public int IterationId => Iteration++;

        public IterationModels Step()
        {
            if (!init) return IterationModels.Empty;

            // steps 4, 5
            var fba = ApplyFba(Sm, Z, IterationId);

            //8. Let m’ be a border metabolite in S(m) involved in the smallest total number of reactions.
            var borderm = GetBorderMetabolites(Sm);
            if (borderm.Count == 0)
            {
                //Core.SaveAsDgs(Sm.Nodes.First().Value, Sm, Core.Dir);
                //Console.WriteLine("NO BORDER METABILTES");
                //Console.ReadKey();
                //Environment.Exit(0);
            }

            var m2 = borderm.Select(m => m).Where(m => !m.IsExtended).Select(m => m.Id).OrderBy(Db.TotalReactions).First();
            //Core.SaveAsDgs(Sm.Nodes[m2], Sm, Core.Dir);
            Sm.NextStep();

            //Extend S(m) with m’ and its reactions from M.
            ExtendGraph(m2, Sm);
            //ex.Wait(int.MaxValue);

            //Remove the exchange reaction that was introduced for m’ in step 4.
            //Add a constraint that total net flux of reactions of m’ should
            //be equal to those of the removed flux exchange reaction.
            RemoveExchangeReaction(Sm, Sm.Nodes[m2]);

            //Go to step 4 to add exchange fluxes for the new border metabolites. 
            //If S(m) cannot be extended, then go to step 3.
            return fba;
        }

        public void Start()
        {
            if (init) return;

            var id = InitZ();

            Init(id);

            init = true;
        }

        private Guid InitZ()
        {
            string[] zn =
            {
                "D-Fructose",
                "Glyceraldehyde", "L-threonine", "taurochenodeoxycholate",
                "D-glucose", "3-Phospho-D-glycerate", "pyruvate"
            };

            var zlist = new List<Species>();
            foreach (var s in zn)
            {
                zlist.AddRange(Db.Context.Species.Where(mm => mm.name.ToLower().Contains(s.ToLower())));
            }

            foreach (var s in zlist)
            {
                Z[s.id] = s.id.GetHashCode() % 2 == 0 ? -1 : 1;
                //Console.WriteLine("{0}:{1}", s.sbmlId, Z[s.id]);
            }

            //var id = Guid.Parse("{3ef51935-aafc-4e37-a829-1ca26f5f0966}");
            //Z[id] = id.GetHashCode() % 2 == 0 ? -1 : 1;

            return zlist[0].id;
        }

        private void Init(Guid id)
        {
            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            var m = Db.Context.Species.Single(s => s.id == id);

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that Iteration contains only m
            Sm.AddNode(m.id, m.sbmlId);
            //HyperGraph.Step++;
            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            ExtendGraph(m.id, Sm);
            //ex.Wait(int.MaxValue);

            ////If there is no such qualifying subset K, then record the current hypothesis and exit
            //if (sm.Edges.Count == 0)
            //{
            //    Environment.Exit(1);
            //}
            init = true;
        }

        internal void Start(ConcentrationChange[] z)
        {
            if (init) return;
            foreach (var s in z)
            {
                var sp = Db.Context.Species.Single(mm => mm.sbmlId == s.Name);
                Z[sp.id] = s.Change;
            }
            var id = Db.Context.Species.Single(mm => mm.sbmlId == "fru[e]").id;
            //var id = Z.Keys.OrderBy(Db.GetReactionCountSum).First();

            Init(id);

            init = true;
        }

        public void Stop()
        {
            init = false;
            Sm.Clear();
            Z.Clear();
            //Util.ClearCache();
        }

        public void RemoveExchangeReaction(HyperGraph sm, HyperGraph.Node m2)
        {
            // remove producer exchange reaction of  non-produced-border metabolite            
            //dont remove producer exchange reaction if producer exchange reaction is only producer reaction
            var removeox = !m2.IsProducedBorder && m2.Producers.Any(edge => edge.IsPseudo)
                           && m2.Producers.Count(edge => !edge.IsPseudo) != 0;

            foreach (var ps in Sm.Edges.Values.Where(e => e.IsPseudo))
                ps.Reactions.ExceptWith(Sm.Edges.Keys);
            if (removeox)
            {
                var outex = m2.Producers.First(s => s.IsPseudo);
                m2.RemovedProducerExchange = outex;
                m2.Producers.Remove(outex);
                HyperGraph.Edge ee1;
                sm.Edges.TryRemove(outex.Id, out ee1);
            }

            // remove consumer exchange reaction of  non-consumed-border metabolite 
            //dont remove consumer exchange reaction if consumer exchange reaction is only consumer reaction
            var removeix = !m2.IsConsumedBorder && m2.Consumers.Any(edge => edge.IsPseudo)
                           && m2.Consumers.Count(edge => !edge.IsPseudo) != 0;

            if (removeix)
            {
                var inex = m2.Consumers.First(s => s.IsPseudo);
                m2.RemovedConsumerExchange = inex;
                m2.Consumers.Remove(inex);

                HyperGraph.Edge e;
                sm.Edges.TryRemove(inex.Id, out e);
            }
        }

        public void UpdateNeighbore(HyperGraph sm, HyperGraph.Node m2)
        {
            var inex = m2.Consumers.Any(e => e.IsPseudo) ? m2.Consumers.First(s => s.IsPseudo) : null;
            var outex = m2.Producers.Any(e => e.IsPseudo) ? m2.Producers.First(s => s.IsPseudo) : null;

            if (inex != null)
            {
                inex.UpdatePseudo.Clear();
                foreach (var s in m2.Consumers.Where(s => !s.IsPseudo && sm.LastLevel <= s.Level)) //
                    inex.UpdatePseudo.Add(s.Id);
            }

            if (outex != null)
            {
                outex.UpdatePseudo.Clear();
                foreach (var s in m2.Producers.Where(s => !s.IsPseudo && sm.LastLevel <= s.Level)) //
                    outex.UpdatePseudo.Add(s.Id);
            }
        }

        public IterationModels ApplyFba(HyperGraph sm, Dictionary<Guid, int> z, int iteration)
        {
            var timer = new Stopwatch();
            timer.Start();
            //4. Let a metabolite mb be labeled as a border metabolite 
            // if there is at least one consumer or producer of mb that is not included in the current subnetwork, S(m). 
            var borderm = GetBorderMetabolites(sm);

            // Define exchange reactions for all border metabolites in S(m).
            borderm.ToList().ForEach(mbs => DefinePseudo(mbs, sm));
            sm.Nodes.Values.Where(n => n.IsCommon).ToList().ForEach(mbs => DefinePseudo(mbs, sm));

            var nonborder = GetNonBorderMetabolites(sm);

            var dic = Intersect(z, nonborder);

            //5. Apply Flux Balance Analysis on S(m) with the objective function F defined as follows. 
            //For  each non-border metabolite  m  that  is  included  in  both S(m)and  Z,  perform  the following checks:

            IsFeasable = Fba.Solve(dic, sm);
            Fba.Iteration = iteration;
            Fba.LastRuntime = timer.ElapsedMilliseconds * 1.0 / 1000.0;
            timer.Stop();

            var it = new IterationModels(iteration)
            {
                Fba = IsFeasable ? 1 : 0,
                Time = Fba.LastRuntime,
                Fluxes = sm.Edges.Values.ToDictionary(r => r.Label, r => r.Flux),
                // TODO add constraints
                Constraints = Enumerable.Range(0, 10).Select(e => Guid.NewGuid().ToString()),
                Nodes = sm.JsonNodes(z),
                Links = sm.JsonLinks()
                // MetabolicNetwork = sm
            };
            return it;
        }

        public ConcurrentDictionary<Guid, int> Intersect(Dictionary<Guid, int> z, HashSet<HyperGraph.Node> nonborder)
        {
            var dic = new ConcurrentDictionary<Guid, int>();
            foreach (var guid in z.Select(s => s.Key).Intersect(nonborder.Select(n => n.Id)))
            {
                dic[guid] = z[guid];
            }
            return dic;
        }

        public void DefinePseudo(HyperGraph.Node node, HyperGraph sm)
        {
            DefineExReaction(node, sm);
        }

        private void DefineExReaction(HyperGraph.Node node, HyperGraph sm)
        {
            try
            {
                if ((node.IsProducedBorder || node.ReactionCount.Producers == 0) && !node.Producers.Any(s => s.IsPseudo))
                {
                    var producer = Guid.NewGuid();
                    sm.AddProduct(producer, $"exr_{node.Label}_prod", node.Id, node.Label, true);
                    sm.Edges[producer].Reactions.UnionWith(
                        node.ToSpecies.ReactionSpecies.Where(
                            rs =>
                            rs.speciesId == node.Id && rs.roleId == Db.ProductId && !sm.Edges.ContainsKey(rs.reactionId))
                            .Select(e => e.reactionId));
                }

                if ((node.IsConsumedBorder || node.ReactionCount.Consumers == 0) && !node.Consumers.Any(s => s.IsPseudo))
                {
                    var consumer = Guid.NewGuid();
                    sm.AddReactant(consumer, $"exr_{node.Label}_cons", node.Id, node.Label, true);
                    sm.Edges[consumer].Reactions.UnionWith(

                        node.ToSpecies.ReactionSpecies.Where(
                            rs =>
                            rs.speciesId == node.Id && rs.roleId == Db.ReactantId
                            && !sm.Edges.ContainsKey(rs.reactionId)).Select(e => e.id));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DefineExReactionLonely(HyperGraph.Node m, HyperGraph sm)
        {
            if (!m.IsLonely)
            {
                return;
            }

            if (m.Producers.Count == 0)
            {
                sm.AddProduct(Guid.NewGuid(), $"exr_{m.Label}_prod", m.Id, m.Label, true);
            }
            else if (m.Consumers.Count == 0)
            {
                sm.AddReactant(Guid.NewGuid(), $"exr_{m.Label}_cons", m.Id, m.Label, true);
            }

            //await Task.Delay(TimeSpan.Zero);
        }

        public HashSet<HyperGraph.Node> GetBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsCommon && node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public HashSet<HyperGraph.Node> GetNonBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsCommon && !node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public void ExtendGraph(Guid mid, HyperGraph sm)
        {
            var sp = Db.Context.Species.Single(s => s.id == mid);
            //Pathway.AddLast(sp.sbmlId);
            sm.Nodes[mid].IsExtended = true;

            foreach (var r in sp.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId).Select(rs => rs.Reaction).Where(rs => !rs.sbmlId.ToLower().Contains("biomass")))
            {
                sm.AddProduct(r.id, r.sbmlId, sp.id, sp.sbmlId);
                AddMetabolites(sm, r);
            }

            foreach (var r in sp.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(rs => rs.Reaction).Where(rs => !rs.sbmlId.ToLower().Contains("biomass")))
            {
                sm.AddReactant(r.id, r.sbmlId, sp.id, sp.sbmlId);
                AddMetabolites(sm, r);
            }

            // add exchange reaction to lonely(metabol. with only input or output reactions) metabolites   
            foreach (var lon in sm.Nodes.Values.Where(n => n.IsLonely))
                DefineExReactionLonely(lon, sm);

            foreach (var node in sm.Nodes[sp.id].AllNeighborNodes()) //.Where(node => !node.IsBorder)
            {
                if (node.IsBorder) UpdateNeighbore(sm, node);
                RemoveExchangeReaction(sm, node);
            }
        }

        private void AddMetabolites(HyperGraph sm, Reaction reaction)
        {
            try
            {
                var products = Db.Context.ReactionSpecies
                    .Where(rs => rs.roleId == Db.ProductId && rs.reactionId == reaction.id)
                    .Select(rs => rs.Species).ToList();

                var reactants = Db.Context.ReactionSpecies
                    .Where(rs => rs.roleId == Db.ReactantId && rs.reactionId == reaction.id)
                    .Select(rs => rs.Species).ToList();

                foreach (var meta in reactants
                    .Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false))
                    sm.AddReactant(reaction.id, reaction.sbmlId, meta.id, meta.sbmlId);

                foreach (var meta in products
                    .Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false))
                    sm.AddProduct(reaction.id, reaction.sbmlId, meta.id, meta.sbmlId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DefineBlockedReactions(HyperGraph graph)
        {
            Console.WriteLine("Running FVA...");
            var fva = new FVA();
            //var smodel = ServerModel.Load(Guid.Parse("74737800-92B5-41E3-9DAA-077CCAE71F0F"));
            //var g = new HyperGraph();
            //foreach (var m in smodel.GetAllSpecies())
            //{
            //    g.AddNode(m.ID, m.SbmlId);
            //    foreach (var prod in m.getAllReactions(Util.Product))
            //        g.AddProduct(prod.ID, prod.SbmlId, m.ID, m.SbmlId);

            //    foreach (var react in m.getAllReactions(Util.Reactant))
            //        g.AddReactant(react.ID, react.SbmlId, m.ID, m.SbmlId);

            //    if (g.Nodes[m.ID].Producers.Count == 0)
            //        g.AddProduct(Guid.NewGuid(), string.Format("exr{0}_prod", m.SbmlId), m.ID, m.SbmlId, true);
            //    else if (g.Nodes[m.ID].Consumers.Count == 0)
            //        g.AddReactant(Guid.NewGuid(), string.Format("exr{0}_cons", m.SbmlId), m.ID, m.SbmlId, true);
            //}
            //Util.SaveAsDgs(g.Nodes.Values.First(),g,"B:\\model2\\");
            fva.Solve(graph);
            foreach (var minmax in fva.Results.Where(minmax => Math.Abs(minmax.Value.Item1 - minmax.Value.Item2) < 0.0001))//&& Math.Abs(minmax.Value.Item2) < double.Epsilon
            {
                //File.AppendAllText(Util.BlockedReactionsFile, string.Format("{0};{1}\n", graph.Edges[minmax.Key].Label, minmax.Value.Item1));
            }
            Console.WriteLine("Running FVA Done!");
        }
    }
}