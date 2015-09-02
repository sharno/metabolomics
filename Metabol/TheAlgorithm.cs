namespace Metabol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using ILOG.CPLEX;

    using Newtonsoft.Json;

    using PathwaysLib.ServerObjects;

    public class TheAlgorithm
    {
        public readonly Fba3N Fba = new Fba3N();

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

        public void Init(Dictionary<string, int> change)
        {
            if (init)
            {
                return;
            }

            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);

            var zlist =
              (from s in change select ServerSpecies.AllSpeciesByName(s.Key) into spec where spec.Length > 0 select spec[0])
                  .ToList();
            zlist.Sort((species, serverSpecies) => string.Compare(species.SbmlId, serverSpecies.SbmlId, StringComparison.Ordinal));
            foreach (var s in zlist)
                Z[s.ID] = (s.ID.GetHashCode() % 2) == 0 ? -1 : 1;//rand.NextDouble() >= 0.5 ? 1 : -1;
            init = true;
        }

        public void Step()
        {
            if (!this.init) return;

            //for (var i = 0; i < step; i++)
            {
                //Timer.Reset();
                //Timer.Start();

                // steps 4, 5
                ApplyFba(Sm, Z, IterationId);
                //while (iteration.Fba == 0)
                //Console.ReadKey();


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
                    Util.SaveAsDgs(Sm.Nodes.First().Value, Sm, this);
                    Console.WriteLine("NO BORDER METABILTES");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                var m2 = LonelyMetabolite(borderm);
                Util.SaveAsDgs(m2, Sm, this);
                Sm.NextStep();
                //Console.WriteLine("{0} ignore*****************************", Fba.IgnoreSet.Count);
                //Console.WriteLine("{0} update*****************************", Fba.UpdateExchangeConstraint.Count);

                //Fba.IgnoreSet.Clear();
                //Fba.UpdateExchangeConstraint.Clear();
                //Extend S(m) with m’ and its reactions from M.
                var ex = ExtendGraph(m2.ToSpecies, Sm);
                //ex.Wait(int.MaxValue);

                //Remove the exchange reaction that was introduced for m’ in step 4.
                //Add a constraint that total net flux of reactions of m’ should
                //be equal to those of the removed flux exchange reaction.
                RemoveExchangeReaction(Sm, m2);

                //Go to step 4 to add exchange fluxes for the new border metabolites. 
                //If S(m) cannot be extended, then go to step 3.
                //yield return fba;
            }
        }

        public void Start2()
        {
            //78419436-263A-4B48-BCB1-F508A50C9119
            //2D4A4044-940F-41A5-9AFD-E710D93FDC50
            //91889477-FFC2-4318-BF02-91E32F34A890
            if (init)
            {
                return;
            }
            var mid = Guid.Parse("2D4A4044-940F-41A5-9AFD-E710D93FDC50");

            var g = new HyperGraph();
            var id = mid;
            while (g.Nodes.Count < 9)
            {
                ExtendGraph(Util.CachedS(id), g);
                foreach (var node in g.Nodes[id].AllNeighborNodes())
                {
                    ExtendGraph(Util.CachedS(node.Id), g);
                    id = node.Id;
                }
            }

            foreach (var node in g.Nodes)
            {
                Z[node.Value.Id] = (node.Value.Id.GetHashCode() % 2) == 0 ? -1 : 1;
            }

            Sm.AddNode(
                Util.CachedS(mid).ID,
                Util.CachedS(mid).SbmlId);

            var ex = ExtendGraph(Util.CachedS(mid), Sm);
            init = true;
        }

        public void Start()
        {
            if (init) return;

            string[] zn =
            {
                "D-Fructose 6-phosphate", "D-Fructose 1,6-bisphosphate", "Dihydroxyacetone phosphate",
                "Glyceraldehyde 3-phosphate", "L-threonine", "taurochenodeoxycholate",
                "D-glucose", "3-Phospho-D-glycerate", "D-Glycerate 2-phosphate", "Phosphoenolpyruvate", "pyruvate"
            };

            var zlist =
                (from s in zn select ServerSpecies.AllSpeciesByNameOnly(s) into spec where spec.Length > 0 select spec[0])
                    .ToList();
            //zlist.Sort((species, serverSpecies) => string.Compare(species.SbmlId, serverSpecies.SbmlId, StringComparison.Ordinal));
            //var rand = new Random((int)DateTime.UtcNow.ToBinary());

            foreach (var s in zlist)
            {
                Z[s.ID] = (s.ID.GetHashCode() % 2) == 0 ? -1 : 1;//rand.NextDouble() >= 0.5 ? 1 : -1;
                Console.WriteLine("{0}:{1}", s.SbmlId, Z[s.ID]);
            }

            var id = Guid.Parse("{05954e8b-244a-4b59-b650-315f2c8e0f43}");
            Z[id] = (id.GetHashCode() % 2) == 0 ? -1 : 1; //rand.NextDouble() >= 0.5 ? 1 : -1;

            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);
            //var reconId = Guid.Parse("c7b42b40-ccd9-42f3-b6bd-9a4111fcbec5");
            //1. Among a user-provided set of observed metabolite changes Z,
            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            //var m = Util.CachedS(Z.Keys.OrderBy(Util.GetReactionCountSum).First()); //e => Z[e] > 0
            var m = Util.CachedS(Guid.Parse("{ae3aa029-b36a-4106-8802-9a65ef225fce}"));
            //Fba.M = m;
            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.ID, m.SbmlId);
            //HyperGraph.Step++;
            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            var ex = ExtendGraph(m, Sm);
            //ex.Wait(int.MaxValue);

            ////If there is no such qualifying subset K, then record the current hypothesis and exit
            //if (sm.Edges.Count == 0)
            //{
            //    Environment.Exit(1);
            //}

            init = true;
        }

        public void Start1()
        {
            if (init) return;

            var strCon = ConfigurationManager.AppSettings["dbConnectString"];
            DBWrapper.Instance = new DBWrapper(strCon);
            //1. Among a user-provided set of observed metabolite changes Z,
            var zn = File.ReadAllLines(Util.SelectedMetaFile).Select(s => s.Split(';'));

            foreach (var s in zn)
            {
                Z[Guid.Parse(s[0])] = int.Parse(s[1]);
            }

            //  let m be the metabolite with the least total number of producer and consumer reactions in the respective metabolic network M
            var m =
                Util.CachedS(
                    Z.Keys.Where(guid => Util.GetReactionCountSum(guid) > 0)
                        .OrderBy(Util.GetReactionCountSum)
                        .First());

            //2. Let S(m) be a subnetwork of the whole metabolic network M. 
            //Initialize S(m) so that iteration contains only m
            Sm.AddNode(m.ID, m.SbmlId);

            //3. Extend S(m) with a subset K of m’s consumers and producers such that K has not been used before to extend the current subnetwork. 
            //HashSet<ServerSpecies> K = new HashSet<ServerSpecies>();
            this.ExtendGraph(m, this.Sm);
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
                if (ee1.Reactants.Values.Any(n => n.IsPseudo))
                {
                    foreach (var id in ee1.Reactants.Where(n => n.Value.IsPseudo))
                    {
                        HyperGraph.Node b;
                        sm.Nodes.TryRemove(id.Key, out b);
                    }
                }
                //try
                //{
                //    if (sm.PseudoPath.ContainsKey(m2.Id))
                //    {
                //        foreach (var n in sm.PseudoPath[m2.Id])
                //        {
                //            var nodes = sm.Nodes[n].Consumers.First(pe => pe.IsPseudo).Products;
                //            var guid = nodes.First(pn => pn.Value.IsPseudo && pn.Value.Label.Equals(m2.Label + "_" + sm.Nodes[n].Label)).Key;
                //            nodes.Remove(guid);


                //        }
                //    }
                //}
                //catch
                //{
                //    // ignored
                //    Console.WriteLine();

                //}
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
                if (e.Products.Values.Any(n => n.IsPseudo))
                {
                    foreach (var id in e.Products.Where(n => n.Value.IsPseudo))
                    {
                        HyperGraph.Node b;
                        sm.Nodes.TryRemove(id.Key, out b);
                    }
                }
                //try
                //{
                //    if (sm.PseudoPath.ContainsKey(m2.Id))
                //    {
                //        foreach (var n in sm.PseudoPath[m2.Id])
                //        {
                //            var nodes = sm.Nodes[n].Producers.First(pe => pe.IsPseudo).Reactants;
                //            var guid = nodes.First(pn => pn.Value.IsPseudo && pn.Value.Label.Equals(m2.Label + "_" + sm.Nodes[n].Label)).Key;
                //            nodes.Remove(guid);

                //            HyperGraph.Node b;
                //            sm.Nodes.TryRemove(guid, out b);
                //        }
                //    }
                //}
                //catch
                //{
                //    // ignored
                //    Console.WriteLine();
                //}
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
                {
                    //if (!Fba.UpdateExchangeConstraint.ContainsKey(inex.Id))
                    //{
                    //    Fba.UpdateExchangeConstraint[inex.Id] = new HashSet<Guid>();
                    //}
                    inex.UpdatePseudo.Add(s.Id);
                    //Fba.UpdateExchangeConstraint[inex.Id].Add(s.Id);
                }
            }

            if (outex != null)
            {
                outex.UpdatePseudo.Clear();
                foreach (var s in m2.Producers.Where(s => !s.IsPseudo && sm.LastLevel <= s.Level)) //
                {
                    //if (!Fba.UpdateExchangeConstraint.ContainsKey(outex.Id))
                    //{
                    //    Fba.UpdateExchangeConstraint[outex.Id] = new HashSet<Guid>();
                    //}
                    outex.UpdatePseudo.Add(s.Id);
                    //Fba.UpdateExchangeConstraint[outex.Id].Add(s.Id);
                }
            }
        }

        public void ApplyFba(HyperGraph sm, Dictionary<Guid, int> z, int iteration)
        {
            //4. Let a metabolite mb be labeled as a border metabolite 
            // if there is at least one consumer or producer of mb that is not included in the current subnetwork, S(m). 
            var borderm = GetBorderMetabolites(sm);
            //borderm.Remove(sm.Nodes[m.ID]);

            // Define exchange reactions for all border metabolites in S(m).
            borderm.Select(mb => Util.CachedS(mb.Id)).ToList().ForEach(mbs => DefinePseudo(mbs, sm));

            //Fba.Label = Util.FbaLabel();
            var nonborder = GetNonBorderMetabolites(sm);

            var dic = Intersect(z, nonborder);

            //5. Apply Flux Balance Analysis on S(m) with the objective function F defined as follows. 
            //For  each non-border metabolite  m  that  is  included  in  both S(m)and  Z,  perform  the following checks:
            var timer = new Stopwatch();
            timer.Start();
            IsFeasable = Fba.Solve(dic, sm);
            Fba.LastRuntime = timer.ElapsedMilliseconds * 1.0 / 1000.0;
            timer.Stop();
            //var it = new Iteration(iteration)
            //             {
            //                 Fba = f ? 1 : 0,
            //                 Time = Fba.LastRuntime,
            //                 //BorderMCount = borderm.Count,
            //                 Nodes = sm.JsonNodes(z),
            //                 Links = sm.JsonLinks()
            //             };
            ////sm.NextStep();
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

        public async Task DefinePseudo(ServerSpecies m, HyperGraph sm)
        {
            await DefineExReaction(m, sm);

            var consumer = sm.Nodes[m.ID].Consumers.First(e => e.IsPseudo).Id;

            foreach (var meta in
                from r in Util.GetAllReaction(m, Util.Reactant).Where(e => !sm.Edges.ContainsKey(e.ID))
                from meta in r.GetAllProducts()//.Where(n => sm.Nodes.ContainsKey(n.ID))
                where sm.Nodes.ContainsKey(meta.ID) && sm.Nodes[meta.ID].IsBorder
                select meta)
            {
                if (meta.SbmlId.Equals("M_xolest_hs_c"))
                   Console.WriteLine();
                await DefineExReaction(Util.CachedS(meta.ID), sm);

                if (sm.ExistPseudoPath(m.ID, meta.ID)) continue;
                sm.AddPseudoPath(m.ID, meta.ID);
                var id = Guid.NewGuid();
                var n = sm.AddNode(id, m.SbmlId + "_" + meta.SbmlId, true);
                //var n = HyperGraph.Node.Create(id, m.SbmlId + "_" + meta.SbmlId, sm.LastLevel, true);
                if (n.Label.Equals("M_chsterol_c_M_xolest_hs_c"))
                    Console.WriteLine("");
                sm.Edges[consumer].AddProduct(n);
                var producer = sm.Nodes[meta.ID].Producers.First(e => e.IsPseudo).Id;
                sm.Edges[producer].AddReactant(n);
            }


            //foreach (var meta in
            //  from r in Util.GetAllReaction(m, Util.Product)
            //  from meta in r.GetAllReactants()
            //  where sm.Nodes.ContainsKey(meta.ID)
            //  select meta)
            //{
            //    await DefineExReaction(Util.CachedS(meta.ID), sm);
            //    var id = Guid.NewGuid();
            //    var n = sm.AddNode(id, m.SbmlId + "_" + meta.SbmlId, true);
            //    //var n = HyperGraph.Node.Create(id, m.SbmlId + "_" + meta.SbmlId, sm.LastLevel, true);
            //    sm.Edges[consumer].AddProduct(n);
            //    var producer = sm.Nodes[m.ID].Producers.First(e => e.IsPseudo).Id;
            //    sm.Edges[producer].AddReactant(n);
            //}

            await Task.Delay(TimeSpan.Zero);
        }

        private async Task DefineExReaction(ServerSpecies m, HyperGraph sm)
        {
            //if (sm.Nodes.ContainsKey(m.ID) && sm.Nodes[m.ID].Consumers.Any(s => s.IsPseudo)
            //    || sm.Nodes.ContainsKey(m.ID) && sm.Nodes[m.ID].Producers.Any(s => s.IsPseudo))
            //{
            //    return;
            //}

            if ((sm.Nodes[m.ID].IsProducedBorder || sm.Nodes[m.ID].ReactionCount.Item2 == 0) && !sm.Nodes[m.ID].Producers.Any(s => s.IsPseudo))
            {
                var producer = Guid.NewGuid();
                sm.AddProduct(producer, string.Format("exr_{0}_prod", m.SbmlId), m.ID, m.SbmlId, true);
                sm.Edges[producer].Reactions.UnionWith(m.getAllReactions(Util.Product).Select(e => e.ID).Where(id => !sm.Edges.ContainsKey(id)));
                sm.Edges[producer].InitReactions.UnionWith(sm.Edges[producer].Reactions);
            }

            if ((sm.Nodes[m.ID].IsConsumedBorder || sm.Nodes[m.ID].ReactionCount.Item1 == 0) && !sm.Nodes[m.ID].Consumers.Any(s => s.IsPseudo))
            {
                var consumer = Guid.NewGuid();
                sm.AddReactant(consumer, string.Format("exr_{0}_cons", m.SbmlId), m.ID, m.SbmlId, true);
                sm.Edges[consumer].Reactions.UnionWith(m.getAllReactions(Util.Reactant).Select(e => e.ID).Where(id => !sm.Edges.ContainsKey(id)));
                sm.Edges[consumer].InitReactions.UnionWith(sm.Edges[consumer].Reactions);
            }
            await Task.Delay(TimeSpan.Zero);
        }

        public static async Task DefineExReactionLonely(HyperGraph.Node m, HyperGraph sm)
        {
            if (!m.IsLonely)
            {
                return;
            }

            if (m.Producers.Count == 0)
            {
                sm.AddProduct(Guid.NewGuid(), string.Format("exr{0}_prod", m.Label), m.Id, m.Label, true);
            }
            else if (m.Consumers.Count == 0)
            {
                sm.AddReactant(Guid.NewGuid(), string.Format("exr{0}_cons", m.Label), m.Id, m.Label, true);
            }

            await Task.Delay(TimeSpan.Zero);
        }

        public static HashSet<HyperGraph.Node> GetBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => node.IsBorder && !node.IsPseudo))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public static HashSet<HyperGraph.Node> GetNonBorderMetabolites(HyperGraph sm)
        {
            var borderm = new HashSet<HyperGraph.Node>();
            foreach (var node in sm.Nodes.Values.Where(node => !node.IsBorder))
            {
                borderm.Add(node);
            }
            return borderm;
        }

        public async Task ExtendGraph(ServerSpecies m, HyperGraph sm)
        {
            const int Outliear = 52;

            foreach (var r in m.getAllReactions(Util.Product).Where(r => r.SbmlId != "R_biomass_reaction"))
            {
                sm.AddProduct(r.ID, r.SbmlId, m.ID, m.SbmlId);

                var products = r.GetAllProducts();
                var reactant = r.GetAllReactants();

                foreach (var p in reactant.Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
                    sm.AddReactant(r.ID, r.SbmlId, p.ID, p.SbmlId);

                foreach (var p in products.Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
                    sm.AddProduct(r.ID, r.SbmlId, p.ID, p.SbmlId);
            }

            foreach (var r in m.getAllReactions(Util.Reactant).Where(r => r.SbmlId != "R_biomass_reaction"))
            {
                sm.AddReactant(r.ID, r.SbmlId, m.ID, m.SbmlId);

                var products = r.GetAllProducts();
                var reactant = r.GetAllReactants();

                foreach (var p in reactant.Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
                    sm.AddReactant(r.ID, r.SbmlId, p.ID, p.SbmlId);

                foreach (var p in products.Where(p => Util.GetReactionCountSum(p.ID) < Outliear))
                    sm.AddProduct(r.ID, r.SbmlId, p.ID, p.SbmlId);
            }

            // add exchange reaction to lonely(metabol. with only input or output reactions) metabolites   
            foreach (var lon in sm.Nodes.Values.Where(n => n.IsLonely))
                await DefineExReactionLonely(lon, sm);

            foreach (var node in sm.Nodes[m.ID].AllNeighborNodes()) //.Where(node => !node.IsBorder)
            {
                if (node.IsBorder)
                    UpdateNeighbore(sm, node);
                //if (node.IsTempBorder)
                RemoveExchangeReaction(sm, node);
            }
        }

        // involved in the smallest total number of reactions
        public static HyperGraph.Node LonelyMetabolite(ICollection<HyperGraph.Node> borderm)
        {
            var nodes = borderm.OrderBy(Util.TotalReactions).ToList();
            return nodes.First();
        }
    }
}