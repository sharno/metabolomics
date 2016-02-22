namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Metabol.Util;
    using Metabol.Util.DB2;

    using Newtonsoft.Json;


    public class TheAlgorithm
    {
        public readonly Fba3 Fba = new Fba3();
        public readonly LinkedList<string> Pathway = new LinkedList<string>();
        //public readonly HashSet<Guid> BlockedReactions = new HashSet<Guid>();
        private const int CommonMetabolite = 61;
        [JsonProperty("isFeasable")]
        public bool IsFeasable { get; set; }

        [JsonProperty("graph")]
        public readonly HyperGraph Sm = new HyperGraph();

        private bool init;

        //public readonly Stopwatch Timer = new Stopwatch();

        [JsonProperty("iter")]
        public int Iteration = 1;

        [JsonProperty("Z")]
        public Dictionary<Guid, int> Z = new Dictionary<Guid, int>();

        public int IterationId
        {
            get
            {
                return Iteration++;
            }
        }

        //public void Init(Dictionary<string, int> change)
        //{
        //    if (init)
        //    {
        //        return;
        //    }

        //    var strCon = ConfigurationManager.AppSettings["dbConnectString"];
        //    //DBWrapper.Instance = new DBWrapper(strCon);

        //    var zlist =
        //      (from s in change select Util.MnContext.Species.AllSpeciesByName(s.Key) into spec where spec.Length > 0 select spec[0])
        //          .ToList();
        //    zlist.Sort((species, serverSpecies) => string.Compare(species.SbmlId, serverSpecies.SbmlId, StringComparison.Ordinal));
        //    foreach (var s in zlist)
        //        Z[s.ID] = (s.ID.GetHashCode() % 2) == 0 ? -1 : 1;//rand.NextDouble() >= 0.5 ? 1 : -1;
        //    init = true;
        //}

        public void Start()
        {
            if (init) return;
            //Db.Context.Species.ToList();
            //Db.Context.Reactions.ToList();
            //Db.Context.ReactionSpecies.ToList();
            //Db.Context.Cycles.ToList();
            //Db.Context.CycleConnections.ToList();
            //Db.Context.CycleReactions.ToList();
            //Console.WriteLine("loaded cache");

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

            //zlist.Sort((species, serverSpecies) => string.Compare(species.SbmlId, serverSpecies.SbmlId, StringComparison.Ordinal));
            //var rand = new Random((int)DateTime.UtcNow.ToBinary());

            foreach (var s in zlist)
            {
                Z[s.id] = (s.id.GetHashCode() % 2) == 0 ? -1 : 1;//rand.NextDouble() >= 0.5 ? 1 : -1;
                Console.WriteLine("{0}:{1}", s.sbmlId, Z[s.id]);
            }
            //ak2g_hs[c]
            //var id = Guid.Parse("47ae0af5-9f7d-443d-b0d5-064e4707e8b5");

            //creat[c]
            //var id = Guid.Parse("1B81D656-115E-4477-B205-026001CF2847");

            //577F7B2F-B654-4719-80F9-012095E07E6D	CE2180[l]
            var id = Guid.Parse("577F7B2F-B654-4719-80F9-012095E07E6D");


            //var id = Guid.Parse("0EDB6D23-7802-4F31-839F-28F9911FE819");

            // Guid.Parse("817069E0-5D9D-4FEB-B66A-BE5E79C1822B");//Guid.Parse("64893C3E-331F-4B24-8CA8-61D9D3D39D03");
            Z[id] = (id.GetHashCode() % 2) == 0 ? -1 : 1;

            //var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            //DBWrapper.Instance = new DBWrapper(strCon);

            //var reconId = Guid.Parse("c7b42b40-ccd9-42f3-b6bd-9a4111fcbec5");
            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            //var m = Util.CachedS(Z.Keys.OrderBy(Util.GetReactionCountSum).First()); //e => Z[e] > 0
            var m = Db.CachedS(id);
            //Fba.M = m;
            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.id, m.sbmlId);
            //HyperGraph.Step++;
            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            var ex = ExtendGraph(m.id, Sm);
            //ex.Wait(int.MaxValue);

            ////If there is no such qualifying subset K, then record the current hypothesis and exit
            //if (sm.Edges.Count == 0)
            //{
            //    Environment.Exit(1);
            //}

            init = true;
        }

        public void Step()
        {
            if (!init) return;

            // steps 4, 5
            ApplyFba(Sm, Z, IterationId);

            //Task.Run(delegate
            //{
            //    //File.AppendAllText(file1, $"=========== {iteration.Id}. iteration ===========\n");
            //    //File.AppendAllText(file1, $"FBA feasable: {iteration.Fba}  time:{Util.Fba.LastRuntime}\n");
            //    //File.AppendAllText(file1, $"Nodes: {sm.Nodes.Count}  BorderM:{iteration.BorderMCount}  Edges: {sm.Edges.Count}\n");
            //    File.AppendAllText(file2,
            //        $"{sm.Nodes.Count},{iteration.BorderMCount},{sm.Edges.Count},{timer.ElapsedMilliseconds * 1.0 / 1000.0}\n");
            //});

            //8. Let m’ be a border metabolite in S(m) involved in the smallest total number of reactions.
            var borderm = GetBorderMetabolites(Sm);
            if (borderm.Count == 0)
            {
                Core.SaveAsDgs(Sm.Nodes.First().Value, Sm, Core.Dir);
                Console.WriteLine("NO BORDER METABILTES");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var m2 = borderm.Select(m => m.Id).OrderBy(Db.TotalReactions).First();
            Core.SaveAsDgs(Sm.Nodes[m2], Sm, Core.Dir);
            Sm.NextStep();

            //Extend S(m) with m’ and its reactions from M.
            var ex = ExtendGraph(m2, Sm);
            //ex.Wait(int.MaxValue);

            //Remove the exchange reaction that was introduced for m’ in step 4.
            //Add a constraint that total net flux of reactions of m’ should
            //be equal to those of the removed flux exchange reaction.
            RemoveExchangeReaction(Sm, Sm.Nodes[m2]);

            //Go to step 4 to add exchange fluxes for the new border metabolites. 
            //If S(m) cannot be extended, then go to step 3.
            //yield return fba;
        }

        public void Start2()
        {
            //78419436-263A-4B48-BCB1-F508A50C9119
            //2D4A4044-940F-41A5-9AFD-E710D93FDC50
            //91889477-FFC2-4318-BF02-91E32F34A890
            //F42F7DA3-61EA-4FAF-B16C-385229C92DFB
            if (init)
            {
                return;
            }
            var mid = Guid.Parse("F42F7DA3-61EA-4FAF-B16C-385229C92DFB");

            var g = new HyperGraph();
            var id = mid;
            g.AddNode(id, Db.CachedS(id).name);
            while (g.Nodes.Count < 5)
            {
                ExtendGraph(id, g);
                foreach (var node in g.Nodes[id].AllNeighborNodes())
                {
                    ExtendGraph((node.Id), g);
                    id = node.Id;
                }
            }

            foreach (var node in g.Nodes)
            {
                Z[node.Value.Id] = (node.Value.Id.GetHashCode() % 2) == 0 ? -1 : 1;
            }

            Sm.AddNode(
                Db.CachedS(mid).id,
                Db.CachedS(mid).sbmlId);

            var ex = ExtendGraph((mid), Sm);
            init = true;
        }

        public void Start1()
        {
            if (init) return;

            //var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            //DBWrapper.Instance = new DBWrapper(strCon);

            //1. Among a user-provided set of observed metabolite changes Z,
            //var zn = File.ReadAllLines(Util.SelectedMetaFile).Select(s => s.Split(';'));

            //foreach (var s in zn)
            //{
            //    Z[Guid.Parse(s[0])] = int.Parse(s[1]);
            //}

            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            var m =
                Db.CachedS(
                    Z.Keys.Where(guid => Db.GetReactionCountSum(guid) > 0)
                        .OrderBy(Db.GetReactionCountSum)
                        .First());

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.id, m.sbmlId);

            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            ExtendGraph(m.id, Sm);
            //ex.Wait(int.MaxValue);
            //Util.SaveAsDgs(sm.Nodes[m.ID], sm, "start");

            ////If there is no such qualifying subset K, then record the current hypothesis and exit
            //if (sm.Edges.Count == 0)
            //{
            //    Environment.Exit(1);
            //}

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
            if (m2.IsExtended)
            {
                if (m2.Producers.Count(e => !e.IsPseudo) != 0)
                {
                    foreach (var reaction in m2.Producers.Where(e => e.IsPseudo).ToList())
                    {
                        sm.RemoveReaction(reaction);
                    }
                }

                if (m2.Consumers.Count(e => !e.IsPseudo) != 0)
                {
                    foreach (var reaction in m2.Consumers.Where(e => e.IsPseudo).ToList())
                    {
                        sm.RemoveReaction(reaction);
                    }
                }
            }
            // remove producer exchange reaction of  non-produced-border metabolite
            //dont remove producer exchange reaction if producer exchange reaction is only producer reaction
            //var removeox = !m2.IsProducedBorder && m2.Producers.Any(edge => edge.IsPseudo)
            //               && m2.Producers.Count(edge => !edge.IsPseudo) != 0;

            //foreach (var ps in Sm.Edges.Values.Where(e => e.IsPseudo))
            //    ps.Reactions.ExceptWith(Sm.Edges.Keys);
            //if (removeox)
            //{
            //    var outex = m2.Producers.First(s => s.IsPseudo);
            //    m2.RemovedProducerExchange = outex;
            //    m2.Producers.Remove(outex);
            //    HyperGraph.Edge ee1;
            //    sm.Edges.TryRemove(outex.Id, out ee1);
                //if (ee1.Reactants.Values.Any(n => n.IsPseudo))
                //{
                //    foreach (var id in ee1.Reactants.Where(n => n.Value.IsPseudo))
                //    {
                //        HyperGraph.Node b;
                //        sm.Nodes.TryRemove(id.Key, out b);
                //    }
                //}
            //}

            // remove consumer exchange reaction of  non-consumed-border metabolite 
            //dont remove consumer exchange reaction if consumer exchange reaction is only consumer reaction
            //var removeix = !m2.IsConsumedBorder && m2.Consumers.Any(edge => edge.IsPseudo)
            //               && m2.Consumers.Count(edge => !edge.IsPseudo) != 0;

            //if (removeix)
            //{
            //    var inex = m2.Consumers.First(s => s.IsPseudo);
            //    m2.RemovedConsumerExchange = inex;
            //    m2.Consumers.Remove(inex);

            //    HyperGraph.Edge e;
            //    sm.Edges.TryRemove(inex.Id, out e);
                //if (e.Products.Values.Any(n => n.IsPseudo))
                //{
                //    foreach (var id in e.Products.Where(n => n.Value.IsPseudo))
                //    {
                //        HyperGraph.Node b;
                //        sm.Nodes.TryRemove(id.Key, out b);
                //    }
                //}
            //}
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

        public void ApplyFba(HyperGraph sm, Dictionary<Guid, int> z, int iteration)
        {
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
            var timer = new Stopwatch();
            timer.Start();
            IsFeasable = Fba.Solve(dic, sm);
            Fba.LastRuntime = timer.ElapsedMilliseconds * 1.0 / 1000.0;
            timer.Stop();

            //return it;
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

        public async Task DefinePseudo(HyperGraph.Node node, HyperGraph sm)
        {
            await DefineExReaction(node, sm);

            //var consumer = sm.Nodes[m.ID].Consumers.First(e => e.IsPseudo).Id;

            //foreach (var meta in
            //    from r in Util.GetAllReaction(m, Util.Reactant).Where(e => !sm.Edges.ContainsKey(e.ID))
            //    from meta in r.GetAllProducts()
            //    where sm.Nodes.ContainsKey(meta.ID) && sm.Nodes[meta.ID].IsBorder
            //    select meta)
            //{
            //    //if (meta.SbmlId.Equals("M_xolest_hs_c"))
            //    //    Console.WriteLine();
            //    await DefineExReaction(Util.CachedS(meta.ID), sm);

            //    if (sm.ExistPseudoPath(m.ID, meta.ID)) continue;
            //    sm.AddPseudoPath(m.ID, meta.ID);
            //    var id = Guid.NewGuid();
            //    var n = sm.AddNode(id, m.SbmlId + "_" + meta.SbmlId, true);
            //    //var n = HyperGraph.Node.Create(id, m.SbmlId + "_" + meta.SbmlId, sm.LastLevel, true);
            //    //if (n.Label.Equals("M_chsterol_c_M_xolest_hs_c"))
            //    //    Console.WriteLine("");
            //    sm.Edges[consumer].AddProduct(n);
            //    var producer = sm.Nodes[meta.ID].Producers.First(e => e.IsPseudo).Id;
            //    sm.Edges[producer].AddReactant(n);
            //}

            //await Task.Delay(TimeSpan.Zero);
        }

        private async Task DefineExReaction(HyperGraph.Node node, HyperGraph sm)
        {
            //if (sm.Nodes.ContainsKey(m.ID) && sm.Nodes[m.ID].Consumers.Any(s => s.IsPseudo)
            //    || sm.Nodes.ContainsKey(m.ID) && sm.Nodes[m.ID].Producers.Any(s => s.IsPseudo))
            //{
            //    return;
            //}
            try
            {
                if ((node.IsProducedBorder || node.ReactionCount.Producers == 0) && !node.Producers.Any(s => s.IsPseudo))
                {
                    var producer = Guid.NewGuid();
                    sm.AddProduct(producer, string.Format("exr_{0}_prod", node.Label), node.Id, node.Label, true);
                    sm.Edges[producer].Reactions.UnionWith(
                        node.ToSpecies.ReactionSpecies.Where(
                            rs =>
                            rs.speciesId == node.Id && rs.roleId == Db.ProductId && !sm.Edges.ContainsKey(rs.reactionId))
                            .Select(e => e.reactionId));

                    //sm.Edges[producer].InitReactions.UnionWith(sm.Edges[producer].Reactions);
                }

                if ((node.IsConsumedBorder || node.ReactionCount.Consumers == 0) && !node.Consumers.Any(s => s.IsPseudo))
                {
                    var consumer = Guid.NewGuid();
                    sm.AddReactant(consumer, string.Format("exr_{0}_cons", node.Label), node.Id, node.Label, true);
                    sm.Edges[consumer].Reactions.UnionWith(
                        //node.IsCommon
                        //    ? Util.CachedS(node.RealId)
                        //          .getAllReactions(Util.Product)
                        //          .Select(e => e.ID)
                        //          .Where(id => !sm.Edges.ContainsKey(id)): 
                        node.ToSpecies.ReactionSpecies.Where(
                            rs =>
                            rs.speciesId == node.Id && rs.roleId == Db.ReactantId
                            && !sm.Edges.ContainsKey(rs.reactionId)).Select(e => e.id));

                    //sm.Edges[consumer].InitReactions.UnionWith(sm.Edges[consumer].Reactions);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await Task.Delay(TimeSpan.Zero);
        }

        public static void DefineExReactionLonely(HyperGraph.Node m, HyperGraph sm)
        {
            if (m.IsExtended) return;

            if (m.Consumers.Count == 0)
            {
                sm.AddReactant(Guid.NewGuid(), string.Format("exr_{0}_cons", m.Label), m.Id, m.Label, true);
            }

            if (m.Producers.Count == 0)
            {
                sm.AddProduct(Guid.NewGuid(), string.Format("exr_{0}_prod", m.Label), m.Id, m.Label, true);
            }

            //if ()
        }
        //public static async Task DefineExReactionLonely(HyperGraph.Node m, HyperGraph sm)
        //{
        //    if (!m.IsLonely)
        //    {
        //        return;c
        //    }

        //    if (m.Producers.Count == 0)
        //    {
        //        sm.AddProduct(Guid.NewGuid(), string.Format("exr_{0}_prod", m.Label), m.Id, m.Label, true);
        //    }
        //    else if (m.Consumers.Count == 0)
        //    {
        //        sm.AddReactant(Guid.NewGuid(), string.Format("exr_{0}_cons", m.Label), m.Id, m.Label, true);
        //    }

        //    await Task.Delay(TimeSpan.Zero);
        //}


        public static HashSet<HyperGraph.Node> GetBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsCommon && node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public static HashSet<HyperGraph.Node> GetNonBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsCommon && !node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public async Task ExtendGraph(Guid mid, HyperGraph sm)
        {
            var sp = Db.Context.Species.Single(s => s.id == mid);
            //Pathway.AddLast(sp.sbmlId);

            foreach (var r in sp.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId).Select(rs => rs.Reaction))
            {
                if (!AddCycleFromReaction(sm, r))
                {
                    sm.AddProduct(r.id, r.sbmlId, sp.id, sp.sbmlId);
                    AddMetabolites(sm, r);
                }
            }

            foreach (var r in sp.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(rs => rs.Reaction))
            {
                if (!AddCycleFromReaction(sm, r))
                {
                    sm.AddReactant(r.id, r.sbmlId, sp.id, sp.sbmlId);
                    AddMetabolites(sm, r);
                }
            }


            // add exchange reaction to lonely(metabol. with only input or output reactions) metabolites   
            foreach (var lon in sm.Nodes.Values.Where(n => n.IsLonely))
            {
                DefineExReactionLonely(lon, sm);
                RemoveExchangeReaction(sm, lon);
            }

            foreach (var node in sm.Nodes[sp.id].AllNeighborNodes()) //.Where(node => !node.IsBorder)
            {
                if (node.IsBorder) UpdateNeighbore(sm, node);
                RemoveExchangeReaction(sm, node);
            }

            sm.Nodes[mid].IsExtended = true;
        }

        private static bool AddCycleFromReaction(HyperGraph hyperGraph, Reaction reaction)
        {
            var cycle = Db.Context.CycleReactions.SingleOrDefault(cr => cr.otherId == reaction.id);
            if (cycle == null) return false;

            var cycleReaction = new HyperGraph.Cycle(Db.Context.Cycles.Find(cycle.cycleId));

            var products = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ProductId);
            var reactants = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ReactantId);
            var reversibles = Db.Context.CycleConnections.Where(cc => cc.cycleId == cycle.cycleId && cc.roleId == Db.ReversibleId);

            foreach (var product in products)
            {
                var m = new HyperGraph.Node(product.Species, hyperGraph.Step);
                hyperGraph.AddProduct(cycleReaction, m, 1);
            }

            foreach (var reactant in reactants)
            {
                var m = new HyperGraph.Node(reactant.Species, hyperGraph.Step);
                hyperGraph.AddReactant(cycleReaction, m, 1);
            }

            foreach (var reversible in reversibles)
            {
                var m = new HyperGraph.Node(reversible.Species, hyperGraph.Step);
                hyperGraph.AddProduct(cycleReaction, m, 1);
                hyperGraph.AddReactant(cycleReaction, m, 1);
            }

            return true;
        }

        private static void AddCycleFromMetabolite(HyperGraph hyperGraph, Species sp)
        {
            // get cycles connected to this metabolite
            var cycleConnections = Db.Context.CycleConnections.Where(cc => cc.metaboliteId == sp.id);
            if (!cycleConnections.Any()) return;
            foreach (var cycleConnection in cycleConnections)
            {
                var cycle = Db.Context.Cycles.Find(cycleConnection.cycleId);
                foreach (var connection in cycle.CycleConnections)
                {
                    foreach (var cycleReaction in connection.Cycle.CycleReactions)
                    {
                        if (cycleReaction.isReaction)
                        {
                            var reaction = Db.Context.Reactions.Find(cycleReaction.otherId);
                            hyperGraph.AddReaction(reaction);
                        }
                        else
                        {
                            var innerCycleDb = Db.Context.Cycles.Find(cycleReaction.otherId);
                            var innerCycle = new HyperGraph.Cycle(innerCycleDb);
                            foreach (var innerCycleConnection in innerCycleDb.CycleConnections)
                            {
                                // todo change level value to real step number
                                if (innerCycleConnection.roleId == Db.ProductId)
                                {
                                    var m = HyperGraph.Node.Create(innerCycleConnection.metaboliteId, innerCycleConnection.Species.sbmlId, 0);
                                    hyperGraph.AddProduct(innerCycle, m, 1);
                                }
                                else if (innerCycleConnection.roleId == Db.ReactantId)
                                {
                                    var m = HyperGraph.Node.Create(innerCycleConnection.metaboliteId, innerCycleConnection.Species.sbmlId, 0);
                                    hyperGraph.AddReactant(innerCycle, m, 1);
                                }
                                else
                                {
                                    var m = HyperGraph.Node.Create(innerCycleConnection.metaboliteId, innerCycleConnection.Species.sbmlId, 0);
                                    hyperGraph.AddProduct(innerCycle, m, 1);
                                    hyperGraph.AddReactant(innerCycle, m, 1);
                                }
                            }
                        }

                    }
                }
            }
        }

        //private static void AddCycleReactions(HyperGraph sm, Reaction r)
        //{
        //    try
        //    {
        //        var cycles = Db.Context.CycleReactions.Where(cr => cr.otherId == r.id && !cr.isReaction).Select(cr => cr.Cycle);
        //        if (!cycles.Any()) return;
        //        foreach (var c in cycles)
        //            foreach (var cr in c.CycleReactions)
        //                // TODO make sure it's not a cycle
        //                AddMetabolites(sm, Db.Context.Reactions.Find(cr.otherId));
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        private static void AddMetabolites(HyperGraph sm, Reaction reaction)
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
                    /*.Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false)*/)
                    sm.AddReactant(reaction.id, reaction.sbmlId, meta.id, meta.sbmlId);

                foreach (var meta in products
                    /*.Where(p => Db.GetReactionCountSum(p.id) < CommonMetabolite && p.boundaryCondition == false)*/)
                    sm.AddProduct(reaction.id, reaction.sbmlId, meta.id, meta.sbmlId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void DefineBlockedReactions(HyperGraph graph)
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