using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Metabol.DbModels.DB;

namespace Metabol.DbModels
{
    public enum NodeType
    {
        New, Border, Selected, NewBorder, None
    }

    public enum EdgeType
    {
        New, None
    }

    /**
     * HyperGraph implementation  
     */
    [Serializable]
    public class HyperGraph 
    {
        public int Step { get; set; }

        public int LastLevel
        {
            get
            {
                if (Edges.Any() && Nodes.Any())
                    return Math.Max(Edges.Values.Select(v => v.Level).Max(), Nodes.Values.Select(n => n.Level).Max());
                else
                    return 0;
            }
        }

        public ConcurrentDictionary<Guid, Node> Nodes { get; protected set; }

        public HashSet<Guid> CommonMetabolites { get; protected set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> PseudoPath { get; protected set; }

        public ConcurrentDictionary<Guid, Edge> Edges { get; protected set; }

        public ConcurrentDictionary<Guid, Cycle> Cycles { get; protected set; }

        private static int _cycleNum = 0;

        public List<Tuple<List<Guid>, List<double>, double>> ExchangeConstraints = new List<Tuple<List<Guid>, List<double>, double>>();

        private readonly ConcurrentDictionary<Guid, int> idMap = new ConcurrentDictionary<Guid, int>();
        private int idGen;

        public HyperGraph()
        {
            Edges = new ConcurrentDictionary<Guid, Edge>();
            Cycles = new ConcurrentDictionary<Guid, Cycle>();
            Nodes = new ConcurrentDictionary<Guid, Node>();
            PseudoPath = new ConcurrentDictionary<Guid, HashSet<Guid>>();
            CommonMetabolites = new HashSet<Guid>();
        }

        public void NextStep()
        {
            Step++;
        }

        public void AddPseudoPath(Guid n1, Guid n2)
        {
            PseudoPath.GetOrAdd(n1, new HashSet<Guid>()).Add(n2);
        }

        public bool ExistPseudoPath(Guid n1, Guid n2)
        {
            return PseudoPath.ContainsKey(n1) && PseudoPath[n1].Contains(n2);
        }

        public Node GetNode(Guid id)
        {
            return Nodes[id];
        }

        public Edge GetEdge(Guid id)
        {
            return Edges[id];
        }

        public Node AddNode(Guid id, string label)
        {
            return Nodes.AddOrUpdate(id, new Node(id, label, Step), (guid, node) => node);
        }

        public void AddSpeciesWithConnections(Species species)
        {
            Console.WriteLine($"adding species {species.sbmlId}");
            var metabolite = Nodes.GetOrAdd(species.id, new Node(species));

            var producers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId);
            var consumers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId);

            foreach (var producer in producers)
            {
                var reaction = Edges.GetOrAdd(producer.reactionId, new Edge(producer.Reaction));
                AddProduct(reaction, metabolite, producer.stoichiometry);
            }
            foreach (var consumer in consumers)
            {
                var reaction = Edges.GetOrAdd(consumer.reactionId, new Edge(consumer.Reaction));
                AddReactant(reaction, metabolite, consumer.stoichiometry);
            }
        }
        public void AddSpeciesWithConnections(Cache.Species species)
        {
            Console.WriteLine($"adding species {species.sbmlId}");
            var metabolite = Nodes.GetOrAdd(species.id, new Node(species));

            var producers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId);
            var consumers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId);

            foreach (var producer in producers)
            {
                var reaction = Edges.GetOrAdd(producer.reactionId, new Edge(producer.Reaction));
                AddProduct(reaction, metabolite, producer.stoichiometry);
            }
            foreach (var consumer in consumers)
            {
                var reaction = Edges.GetOrAdd(consumer.reactionId, new Edge(consumer.Reaction));
                AddReactant(reaction, metabolite, consumer.stoichiometry);
            }
        }

        public void AddReactionWithConnections(Reaction dbReaction)
        {
            Console.WriteLine($"adding reaction {dbReaction.sbmlId}");
            var reaction = Edges.GetOrAdd(dbReaction.id, new Edge(dbReaction));

            var products = dbReaction.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId);
            var reactants = dbReaction.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId);

            foreach (var product in products)
            {
                var metabolite = Nodes.GetOrAdd(product.speciesId, new Node(product.Species));
                AddProduct(reaction, metabolite, product.stoichiometry);
            }
            foreach (var reactant in reactants)
            {
                var metabolite = Nodes.GetOrAdd(reactant.speciesId, new Node(reactant.Species));
                AddReactant(reaction, metabolite, reactant.stoichiometry);
            }
        }


        public void AddReactant(ReactionSpecy rs)
        {
            var metabolite = Nodes.GetOrAdd(rs.speciesId, new Node(rs.Species));
            var reaction = Edges.GetOrAdd(rs.reactionId, new Edge(rs.Reaction));

            metabolite.Consumers.Add(reaction);
            reaction.Reactants[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = rs.stoichiometry;
        }

        public void AddReactant(Edge reaction, Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Consumers.Add(reaction);
            reaction.Reactants[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddReactant(Cycle cycle, Node metabolite)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);
            Edges.GetOrAdd(cycle.Id, cycle);

            metabolite.Consumers.Add(cycle);
            cycle.Reactants[metabolite.Id] = metabolite;
        }


        public void AddProduct(ReactionSpecy rs)
        {
            var metabolite = Nodes.GetOrAdd(rs.speciesId, new Node(rs.Species));
            var reaction = Edges.GetOrAdd(rs.reactionId, new Edge(rs.Reaction));

            metabolite.Producers.Add(reaction);
            reaction.Products[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = rs.stoichiometry;
        }

        public void AddProduct(Edge reaction, Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Producers.Add(reaction);
            reaction.Products[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddProduct(Cycle cycle, Node metabolite)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);
            Edges.GetOrAdd(cycle.Id, cycle);

            metabolite.Producers.Add(cycle);
            cycle.Products[metabolite.Id] = metabolite;
        }

        public IEnumerable<object> JsonNodes(Dictionary<Guid, int> z)
        {
            // "name": "Glucose", "type": "m", "isBorder": 1, "concentration": 10.1, 'change': '+'
            var lastLevel = LastLevel;
            var result = new List<object>();

            foreach (var node in Nodes.Where(node => node.Value.Level == lastLevel))
            {
                var ch = "0";
                //TODO bring this back
                //if (z.ContainsKey(node.Key))
                //    ch = z[node.Key] > 0 ? "+" : z[node.Key] < 0 ? "-" : "0";

                var n = new
                {
                    id = idGen,
                    name = node.Value.Label,
                    type = "m",
                    isBorder = node.Value.IsBorder ? 1 : 0,
                    concentration = 10.0,
                    change = ch
                };
                idMap[node.Key] = idGen;
                idGen++;
                //yield return n;
                result.Add(n);
            }

            // { "name": "Pyruvate xxx", "type": "r", "V": 9 }
            foreach (var node in Edges.Where(node => node.Value.Level == lastLevel))
            {
                var n = new
                {
                    id = idGen,
                    name = node.Value.Label,
                    type = "r",
                    v = node.Value.Flux
                };
                idMap[node.Key] = idGen;
                idGen++;
                //yield return n;
                result.Add(n);
            }
            return result;
        }

        public IEnumerable<object> JsonLinks()
        {
            // { "source": 0, "target": 3, "role": "s" }
            // { "source": 3, "target": 4, "role": "p" }
            var lastLevel = LastLevel;
            var result = new List<object>();

            foreach (var link in Edges.Where(node => node.Value.Level == lastLevel))
            {
                foreach (var node in link.Value.Reactants.Values)
                {
                    object n = new { source = idMap[node.Id], target = idMap[link.Value.Id], role = "s", stoichiometry = node.Weights.ContainsKey(link.Key) ? node.Weights[link.Key] : 0 };
                    //yield return n;
                    result.Add(n);
                }

                foreach (var node in link.Value.Products.Values)
                {
                    object n = new { source = idMap[link.Value.Id], target = idMap[node.Id], role = "p", stoichiometry = node.Weights.ContainsKey(link.Key) ? node.Weights[link.Key] : 0 };
                    //yield return n;
                    result.Add(n);
                }
            }
            return result;

        }


        // region of removals
        #region removal of reaction or cycle or node

        public void RemoveReaction(Edge reaction)
        {
            foreach (var reactant in reaction.Reactants)
            {
                reactant.Value.Consumers.RemoveWhere(c => c.Id == reaction.Id);
            }
            foreach (var product in reaction.Products)
            {
                product.Value.Producers.RemoveWhere(p => p.Id == reaction.Id);
            }

            Edge _;
            Edges.TryRemove(reaction.Id, out _);
        }

        public void RemoveCycle(Cycle cycle)
        {
            foreach (var reactant in cycle.Reactants)
            {
                reactant.Value.Consumers.RemoveWhere(c => c.Id == cycle.Id);
            }
            foreach (var product in cycle.Products)
            {
                product.Value.Producers.RemoveWhere(p => p.Id == cycle.Id);
            }

            Cycle _;
            Cycles.TryRemove(cycle.Id, out _);

            Edge _2;
            Edges.TryRemove(cycle.Id, out _2);
        }

        public void RemoveNode(Guid nid)
        {
            //foreach (var consumer in Nodes[nid].Consumers)
            //{
            //    consumer.Reactants.Remove(nid);
            //    if (consumer.Reactants.Count == 0 && consumer.Products.Count == 0)
            //    {
            //        Edge ingoredEdge;
            //        Edges.TryRemove(consumer.Id, out ingoredEdge);
            //    }
            //}

            //foreach (var producer in Nodes[nid].Producers)
            //{
            //    producer.Products.Remove(nid);
            //    if (producer.Reactants.Count == 0 && producer.Products.Count == 0)
            //    {
            //        Edge ingoredEdge;
            //        Edges.TryRemove(producer.Id, out ingoredEdge);
            //    }
            //}

            foreach (var reaction in Nodes[nid].Producers.Union(Nodes[nid].Consumers))
            {
                reaction.Products.Remove(nid);
                reaction.Reactants.Remove(nid);
                if (reaction.Reactants.Count == 0 && reaction.Products.Count == 0)
                {
                    Edge ingoredEdge;
                    Edges.TryRemove(reaction.Id, out ingoredEdge);
                }
            }

            Node ignored;
            Nodes.TryRemove(nid, out ignored);

            // TODO check pseudo and stoichiometry
        }
        #endregion

        public void Clear()
        {
            foreach (var edge in Edges)
            {
                edge.Value.Reactants.Clear();
                edge.Value.Products.Clear();
            }
            foreach (var node in Nodes)
            {
                node.Value.Consumers.Clear();
                node.Value.Producers.Clear();
            }
        }

        [Serializable]
        public abstract class Entity : IComparable<Entity>
        {
            public Guid Id;
            
            public string Label;

            public bool RecentlyAdded = true;

            public abstract Dictionary<Guid, Entity> Next { get; }
            public abstract Dictionary<Guid, Entity> Previous { get; }

            int IComparable<Entity>.CompareTo(Entity other)
            {
                return other.Id.CompareTo(Id);
            }

            public override bool Equals(object obj)
            {
                return obj is Entity && ((Entity)obj).Id.Equals(Id);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override string ToString()
            {
                return Label;
            }
        }

        [Serializable]
        public class Cycle : Edge
        {
            public Dictionary<Guid, Edge> InterfaceReactions = new Dictionary<Guid, Edge>();
            public HashSet<Edge> InsideReactions = new HashSet<Edge>();
            public Dictionary<Guid, double> Fluxes = new Dictionary<Guid, double>();
            public Dictionary<Guid, double> PreFluxes = new Dictionary<Guid, double>();

            public Cycle()
            {
                Id = Guid.NewGuid();
                Label = "cycle";
            }

            public Cycle(DB.Cycle cycle)
            {
                Id = cycle.id;
                Label = "cycle" + _cycleNum++;

                foreach (var cycleReaction in cycle.CycleReactions)
                {
                    if (cycleReaction.isReaction)
                    {
                        var r = Db.Context.Reactions.Find(cycleReaction.otherId);
                        InsideReactions.Add(new Edge(r));
                    }
                }
            }
        }

        [Serializable]
        public class Edge : Entity
        {
            #region Properties
            public bool IsPseudo { get; set; }
            
            public double Flux { get; set; }
            
            public double PreFlux { get; set; }
            
            public int Level { get; set; }
            
            public bool IsReversible { get; set; }
            
            public double LowerBound;
            
            public double UpperBound;

            public string Subsystem;
            
            public Dictionary<Guid, Node> Reactants = new Dictionary<Guid, Node>();
            
            public Dictionary<Guid, Node> Products = new Dictionary<Guid, Node>();

            public override Dictionary<Guid, Entity> Next
            {
                get { return Products.ToDictionary(e => e.Key, e => (Entity)e.Value); }
            }

            public override Dictionary<Guid, Entity> Previous
            {
                get { return Reactants.ToDictionary(e => e.Key, e => (Entity)e.Value); }
            }

            public const double DgsThreshold = 0.001;
            #endregion
            protected Edge()
            {

            }

            public Edge(Reaction reaction)
            {
                Id = reaction.id;
                Label = reaction.sbmlId;
                IsReversible = reaction.reversible;
                IsPseudo = false;
                Subsystem = reaction.subsystem;
                if (reaction.ReactionBoundFix != null)
                {
                    LowerBound = reaction.ReactionBoundFix.lowerbound;
                    UpperBound = reaction.ReactionBoundFix.upperbound;
                } else
                {
                    var rb = reaction.ReactionBounds.First();
                    LowerBound = rb.lowerBound;
                    UpperBound = rb.upperBound;
                }
            }
            public Edge(Cache.Reaction reaction)
            {
                Id = reaction.id;
                Label = reaction.sbmlId;
                IsReversible = reaction.reversible;
                IsPseudo = false;
                Subsystem = reaction.subsystem;
                if (reaction.ReactionBoundFix != null)
                {
                    LowerBound = reaction.ReactionBoundFix.lowerbound;
                    UpperBound = reaction.ReactionBoundFix.upperbound;
                }
                else
                {
                    var rb = reaction.ReactionBounds.First();
                    LowerBound = rb.lowerBound;
                    UpperBound = rb.upperBound;
                }
            }

            public static Edge NewPseudoEdge(string label)
            {
                var edge = new Edge();

                edge.Id = Guid.NewGuid();
                edge.Label = label;
                edge.IsReversible = false;
                edge.IsPseudo = true;
                edge.Subsystem = "pseudo";

                return edge;
            }

            public Edge AddReactant(Node node)
            {
                Reactants[node.Id] = node;
                node.Consumers.Add(this);
                return this;
            }

            public Edge AddProduct(Node node)
            {
                Products[node.Id] = node;
                node.Producers.Add(this);
                return this;
            }

            public IEnumerable<Node> AllNodes()
            {
                return Reactants.Values.Union(Products.Values).Distinct();
            }

            public string ToDgs(EdgeType type, double maxFlux)
            {
                var uiclass = "";
                var hedgeclass = "";
                switch (type)
                {
                    case EdgeType.New:
                        hedgeclass = Math.Abs(this.Flux) < DgsThreshold ? " ui.class:newhedge0 " : /*" ui.class:newhedge "*/ "ui.style:\"fill-color: #00ffff; \"";
                        break;

                    case EdgeType.None:
                        hedgeclass = Math.Abs(this.Flux) < DgsThreshold ? " ui.class:hedge0 " : " ui.class:hedge ";
                        break;
                }

                uiclass = getStyle(this.Flux, maxFlux);

                var bu = new StringBuilder(
                    $"an \"{Id}\" {hedgeclass} label:\" {Label}({this.Flux:#.#####})({this.PreFlux:#.#####}) \"\r\n");

                foreach (var node in this.Reactants.Values)
                    bu.Append(
                        $"ae \"{node.Id}{Id}\" \"{node.Id}\" > \"{Id}\" {uiclass} label:\" {(node.Weights.ContainsKey(Id) ? node.Weights[Id] : Db.PseudoReactionStoichiometry)} \"\r\n");

                foreach (var node in this.Products.Values)
                    bu.Append(
                        $"ae \"{Id}{node.Id}\" \"{Id}\" > \"{node.Id}\" {uiclass} label:\" {(node.Weights.ContainsKey(Id) ? node.Weights[Id] : Db.PseudoReactionStoichiometry)} \"\r\n");

                return bu.ToString();
            }

            private string getStyle(double flux, double maxFlux)
            {
                if (Math.Abs(flux) < DgsThreshold)
                    return " ui.class:new0 ";

                var r = 0.0;
                if (flux < 0)
                    r = (int)Math.Ceiling(Math.Abs(255 * flux / maxFlux));

                var g = 0.0;
                if (flux > 0)
                    g = (int)Math.Ceiling(Math.Abs(255 * flux / maxFlux));

                return $" ui.style:\"size:1.5px; fill-color:rgb({r}, {g}, 0); arrow-size:8px, 6px;\" ";
            }

        }

        [Serializable]
        public class Node : Entity
        {
            #region Properties
            public Guid RealId;
            
            public bool IsCommon;
            
            public HashSet<Edge> Consumers = new HashSet<Edge>();
            
            public HashSet<Edge> Producers = new HashSet<Edge>();

            public override Dictionary<Guid, Entity> Next
            {
                get { return Consumers.ToDictionary(e => e.Id, e => (Entity)e); }
            }

            public override Dictionary<Guid, Entity> Previous
            {
                get { return Producers.ToDictionary(e => e.Id, e => (Entity)e); }
            }
            
            public int Level { get; set; }
            
            public readonly ReactionCountClass ReactionCount = new ReactionCountClass();

            public HashSet<Guid> RealConsumers;
            public HashSet<Guid> RealProducers;
            
            public bool IsExtended { get; set; }
            
            public Species ToSpecies
            {
                get
                {
                    return Db.Context.Species.Find(Id);
                }
            }
            
            public bool IsLonely => ReactionCount.Consumers == 0 || ReactionCount.Producers == 0;
            
            public bool IsBorder => IsConsumedBorder || IsProducedBorder;

            public Dictionary<Guid, HashSet<Guid>> SharedReactionsWithCyclesCache = new Dictionary<Guid, HashSet<Guid>>();
            public HashSet<Guid> SharedReactionsWithCycle(DB.Cycle cycle)
            {
                if (SharedReactionsWithCyclesCache.ContainsKey(cycle.id))
                {
                    return SharedReactionsWithCyclesCache[cycle.id];
                }

                var shared = new HashSet<Guid>();
                shared.UnionWith(new HashSet<Guid>(cycle.CycleReactions.Select(cr => cr.otherId)).Intersect(RealConsumers));
                shared.UnionWith(new HashSet<Guid>(cycle.CycleReactions.Select(cr => cr.otherId)).Intersect(RealProducers));
                shared.UnionWith(
                    cycle.CycleReactions.Where(cr => !cr.isReaction)
                        .Select(cr => Db.Context.Cycles.Find(cr.otherId))
                        .SelectMany(SharedReactionsWithCycle));

                SharedReactionsWithCyclesCache[cycle.id] = shared;
                return SharedReactionsWithCyclesCache[cycle.id];
            }
            
            public bool IsConsumedBorder
            {
                get
                {
                    HashSet<Guid> shared = new HashSet<Guid>(Consumers.Union(Producers).OfType<Cycle>().SelectMany(r => SharedReactionsWithCycle(Db.Context.Cycles.Find(r.Id))));
                    HashSet<Guid> separate = new HashSet<Guid>(Consumers.Union(Producers).Where(r => !r.IsPseudo && !(r is Cycle)).Select(r => r.Id));
                    var remainingConsumers = RealConsumers.Except(separate.Union(shared));
                    var remainingProducers = RealProducers.Except(separate.Union(shared));
                    // reaction bound fixes are only blocking or reversing the reaction so, normal reactions shouldn't be considered as normal
                    return remainingConsumers.Any(c => !Db.Context.ReactionBoundFixes.Any(e => c == e.reactionId)) || remainingProducers.Any(p => Db.Context.Reactions.Find(p).reversible /*&& !Db.Context.ReactionBoundFixes.Any(e => p == e.reactionId)*/);
                }
            }

            //public Dictionary<Guid, int> NumberOfSharedConsumersWithCyclesCache = new Dictionary<Guid, int>();
            //public int NumberOfSharedConsumersWithCycle(DB.Cycle cycle)
            //{
            //    if (NumberOfSharedConsumersWithCyclesCache.ContainsKey(cycle.id))
            //    {
            //        return NumberOfSharedConsumersWithCyclesCache[cycle.id];
            //    }

            //    var countOfShared = 0;
            //    countOfShared += new HashSet<Guid>(cycle.CycleReactions.Select(cr => cr.otherId)).Intersect(RealConsumers).Count();
            //    countOfShared +=
            //        cycle.CycleReactions.Where(cr => !cr.isReaction)
            //            .Select(cr => Db.Context.Cycles.Find(cr.otherId))
            //            .Sum(innerCycle => NumberOfSharedConsumersWithCycle(innerCycle));

            //    NumberOfSharedConsumersWithCyclesCache[cycle.id] = countOfShared;
            //    return NumberOfSharedConsumersWithCyclesCache[cycle.id];
            //}



                
            public bool IsProducedBorder
            {
                get
                {
                    HashSet<Guid> shared = new HashSet<Guid>(Consumers.Union(Producers).OfType<Cycle>().SelectMany(r => SharedReactionsWithCycle(Db.Context.Cycles.Find(r.Id))));
                    HashSet<Guid> separate = new HashSet<Guid>(Consumers.Union(Producers).Where(r => !r.IsPseudo && !(r is Cycle)).Select(r => r.Id));
                    var remainingConsumers = RealConsumers.Except(separate.Union(shared));
                    var remainingProducers = RealProducers.Except(separate.Union(shared));
                    // reaction bound fixes are only blocking or reversing the reaction so, normal reactions shouldn't be considered as normal
                    return remainingProducers.Any(p => !Db.Context.ReactionBoundFixes.Any(e => p == e.reactionId)) || remainingConsumers.Any(c => Db.Context.Reactions.Find(c).reversible /*&& !Db.Context.ReactionBoundFixes.Any(e => c == e.reactionId)*/);
                }
            }

            //public Dictionary<Guid, int> SharedProducersWithCyclesCache = new Dictionary<Guid, int>();
            //public int NumberOfSharedProducersWithCycle(DB.Cycle cycle)
            //{
            //    if (SharedProducersWithCyclesCache.ContainsKey(cycle.id))
            //    {
            //        return SharedProducersWithCyclesCache[cycle.id];
            //    }
            //    var countOfShared = 0;
            //    countOfShared += new HashSet<Guid>(cycle.CycleReactions.Select(cr => cr.otherId)).Intersect(RealProducers).Count();
            //    countOfShared +=
            //        cycle.CycleReactions.Where(cr => !cr.isReaction)
            //            .Select(cr => Db.Context.Cycles.Find(cr.otherId))
            //            .Sum(innerCycle => NumberOfSharedProducersWithCycle(innerCycle));

            //    SharedProducersWithCyclesCache[cycle.id] = countOfShared;
            //    return SharedProducersWithCyclesCache[cycle.id];
            //}
            
            public bool IsTempBorder
            {
                get
                {
                    return ((this.Producers.Any(s => s.IsPseudo) && IsProducedBorder)
                        || (this.Consumers.Any(s => s.IsPseudo) && IsConsumedBorder));
                }
            }
            
            public int TotalReaction
            {
                get
                {
                    return ReactionCount.Consumers + ReactionCount.Producers;
                }
            }
            
            public Dictionary<Guid, double> Weights = new Dictionary<Guid, double>();
            
            public Edge RemovedConsumerExchange { get; set; }
            
            public Edge RemovedProducerExchange { get; set; }

            #endregion

            public Node(Species species)
            {
                Id = species.id;
                Label = species.sbmlId;

                ReactionCount.Producers = species.ReactionSpecies.Count(rs => rs.roleId == Db.ProductId);
                ReactionCount.Consumers = species.ReactionSpecies.Count(rs => rs.roleId == Db.ReactantId);

                RealConsumers = new HashSet<Guid>(species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(c => c.reactionId));
                RealProducers = new HashSet<Guid>(species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId).Select(c => c.reactionId));
            }
            public Node(Cache.Species species)
            {
                Id = species.id;
                Label = species.sbmlId;

                ReactionCount.Producers = species.ReactionSpecies.Count(rs => rs.roleId == Db.ProductId);
                ReactionCount.Consumers = species.ReactionSpecies.Count(rs => rs.roleId == Db.ReactantId);

                RealConsumers = new HashSet<Guid>(species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(c => c.reactionId));
                RealProducers = new HashSet<Guid>(species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId).Select(c => c.reactionId));
            }

            public Node(Guid id, string label, int level)
            {
                Label = label;
                Id = id;
                Level = level;
                Species species = Db.Context.Species.Find(Id);

                ReactionCount.Producers = species.ReactionSpecies.Count(rs => rs.roleId == Db.ProductId);
                ReactionCount.Consumers = species.ReactionSpecies.Count(rs => rs.roleId == Db.ReactantId);

                RealConsumers = new HashSet<Guid>(species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId).Select(c => c.reactionId));
                RealProducers = new HashSet<Guid>(species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId).Select(c => c.reactionId));
            }

            public string ToDgs(NodeType type)
            {
                var uiclass = "";
                switch (type)
                {
                    case NodeType.New:
                        uiclass = " ui.class:new ";
                        break;

                    case NodeType.Border:
                        uiclass = " ui.class:border ";
                        break;

                    case NodeType.Selected:
                        uiclass = " ui.class:selected ";
                        break;

                    case NodeType.NewBorder:
                        uiclass = " ui.class:newborder ";
                        break;

                    case NodeType.None:
                        uiclass = " ";
                        break;
                }

                return String.Format("an \"{0}\"  label:\" {1} \"  {2}", Id, Label, uiclass);
            }

            public IEnumerable<Edge> AllReactions()
            {
                return Consumers.Union(Producers);
            }

            public IEnumerable<Node> AllNeighborNodes()
            {
                return Consumers.SelectMany(e => e.AllNodes())
                    .Union(Producers.SelectMany(e => e.AllNodes()))
                    .Distinct();
            }

            [Serializable]
            public class ReactionCountClass
            {
                public ReactionCountClass()
                {

                }

                public ReactionCountClass(int consumers, int producers)
                {
                    Producers = producers;
                    Consumers = consumers;
                }

                public int Producers = 0;
                public int Consumers = 0;
            }
        }
    }
}