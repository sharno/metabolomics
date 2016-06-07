using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Ecoli.Util
{
    using DB;
    using Newtonsoft.Json;

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
    public class HyperGraph
    {
        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonIgnore]
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

        [JsonProperty("nodes")]
        public ConcurrentDictionary<Guid, Node> Nodes { get; protected set; }

        public HashSet<Guid> CommonMetabolites { get; protected set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> PseudoPath { get; protected set; }

        [JsonProperty("edges")]
        public ConcurrentDictionary<Guid, Edge> Edges { get; protected set; }

        [JsonProperty("cycles")]
        public ConcurrentDictionary<Guid, Cycle> Cycles { get; protected set; }

        private static int _cycleNum = 0;

        public List<Tuple<List<Guid>, List<double>, double>> ExchangeConstraints = new List<Tuple<List<Guid>, List<double>, double>>();

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

        private static int count = 0;
        public void AddSpeciesWithConnections(Species species)
        {
            count++;
            Console.WriteLine("adding species" + count);
            var metabolite = Nodes.GetOrAdd(species.id, new Node(species.id, species.sbmlId, Step));

            var producers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId && rs.Reaction.sbmlId != "R_biomass_reaction");
            var consumers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId && rs.Reaction.sbmlId != "R_biomass_reaction");

            foreach (var producer in producers)
            {
                var reaction = Edges.GetOrAdd(producer.reactionId, new Edge(producer.reactionId, producer.Reaction.sbmlId, producer.Reaction.reversible, false, Step));
                AddProduct(reaction, metabolite, producer.stoichiometry);
            }
            foreach (var consumer in consumers)
            {
                var reaction = Edges.GetOrAdd(consumer.reactionId, new Edge(consumer.reactionId, consumer.Reaction.sbmlId, consumer.Reaction.reversible, false, Step));
                AddReactant(reaction, metabolite, consumer.stoichiometry);
            }
        }

        public void AddReactant(HyperGraph.Edge reaction, HyperGraph.Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Consumers.Add(reaction);
            reaction.Reactants[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddReactant(HyperGraph.Cycle cycle, HyperGraph.Node metabolite)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);
            Edges.GetOrAdd(cycle.Id, cycle);

            metabolite.Consumers.Add(cycle);
            cycle.Reactants[metabolite.Id] = metabolite;
        }

        public void AddReactant(Guid eid, string elabel, bool isReversible, bool isPseudo, Guid nid, string label)
        {
            var node = Nodes.GetOrAdd(nid, new Node(nid, label, Step));
            var e = Edges.GetOrAdd(eid, new Edge(eid, elabel, isReversible, isPseudo, Step));
            e.AddReactant(node);
            if (!isPseudo) node.Weights[eid] = Db.Context.ReactionSpecies.Single(rs => rs.reactionId == eid && rs.speciesId == nid).stoichiometry;

        }

        public void AddProduct(HyperGraph.Edge reaction, HyperGraph.Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Producers.Add(reaction);
            reaction.Products[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddProduct(HyperGraph.Cycle cycle, HyperGraph.Node metabolite)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);
            Edges.GetOrAdd(cycle.Id, cycle);

            metabolite.Producers.Add(cycle);
            cycle.Products[metabolite.Id] = metabolite;
        }

        public void AddProduct(Guid eid, string elabel, bool isReversible, bool isPseudo, Guid nid, string nlabel)
        {
            var node = Nodes.GetOrAdd(nid, new Node(nid, nlabel, Step));
            var e = Edges.GetOrAdd(eid, new Edge(eid, elabel, isReversible, isPseudo, Step));
            e.AddProduct(node);
            if (!isPseudo) node.Weights[eid] = Db.Context.ReactionSpecies.Single(rs => rs.reactionId == eid && rs.speciesId == nid).stoichiometry;
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

        public abstract class Entity : IComparable<Entity>
        {
            [JsonProperty("id")]
            public Guid Id;

            [JsonProperty("label")]
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
                        InsideReactions.Add(new Edge(r.id, r.sbmlId, r.reversible, false, 0));
                    }
                }
            }
        }

        public class Edge : Entity
        {
            #region Properties
            [JsonProperty("isPseudo")]
            public bool IsPseudo { get; set; }

            [JsonProperty("value")]
            public double Flux { get; set; }

            [JsonProperty("preValue")]
            public double PreFlux { get; set; }

            [JsonProperty("level")]
            public int Level { get; set; }

            [JsonProperty("isReversible")]
            public bool IsReversible { get; set; }

            [JsonProperty("lowerBound")]
            public double LowerBound;

            [JsonProperty("upperBound")]
            public double UpperBound;

            [JsonProperty("inputNodes")]
            public Dictionary<Guid, Node> Reactants = new Dictionary<Guid, Node>();

            [JsonProperty("outputNodes")]
            public Dictionary<Guid, Node> Products = new Dictionary<Guid, Node>();

            public override Dictionary<Guid, Entity> Next
            {
                get { return Products.ToDictionary(e => e.Key, e => (Entity)e.Value); }
            }

            public override Dictionary<Guid, Entity> Previous
            {
                get { return Reactants.ToDictionary(e => e.Key, e => (Entity)e.Value); }
            }
            #endregion

            public Edge()
            {
                Id = new Guid();
            }

            public Edge(Guid id, string label, bool isReversible, bool isPseudo, int level)
            {
                Id = id;
                Label = label;
                IsReversible = isReversible;
                IsPseudo = isPseudo;
                Level = level;

                if (isPseudo) return;
                var rb = Db.Context.ReactionBounds.Single(e => e.reactionId == id);
                LowerBound = rb.lowerBound;
                UpperBound = rb.upperBound;
                var rbf = Db.Context.ReactionBoundFixes.SingleOrDefault(e => e.reactionId == id);
                if (rbf != null)
                {
                    LowerBound = rbf.lowerbound;
                    UpperBound = rbf.upperbound;
                }
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
                return new HashSet<Node>(this.Reactants.Values.Concat(this.Products.Values));
            }

            public string ToDgs(EdgeType type)
            {
                var uiclass = "";
                var hedgeclass = "";
                switch (type)
                {
                    case EdgeType.New:
                        hedgeclass = Math.Abs(this.Flux) < double.Epsilon ? " ui.class:newhedge0 " : " ui.class:newhedge ";
                        break;

                    case EdgeType.None:
                        hedgeclass = Math.Abs(this.Flux) < double.Epsilon ? " ui.class:hedge0 " : " ui.class:hedge ";

                        break;
                }

                uiclass = getStyle(this.Flux);

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

            private string getStyle(double flux)
            {
                if (Math.Abs(flux) < Double.Epsilon)
                    return " ui.class:new0 ";
                var r = (int)Math.Floor(Math.Abs(221 - flux / 30.0));
                var g = (int)Math.Floor(Math.Abs(221 - flux / 4.6));
                return $" ui.style:\"size:3px; fill-color:rgb({r}, {g}, 0); arrow-size:8px, 6px;\" ";
            }

        }

        public class Node : Entity
        {
            #region Properties
            public Guid RealId;

            [JsonProperty("isCommon")]
            public bool IsCommon;

            [JsonProperty("consumerEdge")]
            public HashSet<Edge> Consumers = new HashSet<Edge>();

            [JsonProperty("producerEdge")]
            public HashSet<Edge> Producers = new HashSet<Edge>();

            public override Dictionary<Guid, Entity> Next
            {
                get { return Consumers.ToDictionary(e => e.Id, e => (Entity)e); }
            }

            public override Dictionary<Guid, Entity> Previous
            {
                get { return Producers.ToDictionary(e => e.Id, e => (Entity)e); }
            }

            [JsonProperty("level")]
            public int Level { get; set; }

            [JsonProperty("reactionCount")]
            public readonly ReactionCountClass ReactionCount = new ReactionCountClass();

            public HashSet<Guid> RealConsumers;
            public HashSet<Guid> RealProducers;

            [JsonProperty("isExtended")]
            public bool IsExtended { get; set; }

            [JsonIgnore]
            public Species ToSpecies
            {
                get
                {
                    return Db.Context.Species.Find(Id);
                }
            }

            [JsonIgnore]
            public bool IsLonely => ReactionCount.Consumers == 0 || ReactionCount.Producers == 0;

            [JsonProperty("isBorder")]
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

            [JsonProperty("isConsumedBorder")]
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




            [JsonProperty("isProducedBorder")]
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

            [JsonIgnore]
            public bool IsTempBorder
            {
                get
                {
                    return ((this.Producers.Any(s => s.IsPseudo) && IsProducedBorder)
                        || (this.Consumers.Any(s => s.IsPseudo) && IsConsumedBorder));
                }
            }

            [JsonIgnore]
            public int TotalReaction
            {
                get
                {
                    return ReactionCount.Consumers + ReactionCount.Producers;
                }
            }

            [JsonProperty("weights")]
            public Dictionary<Guid, double> Weights = new Dictionary<Guid, double>();

            [JsonProperty("removedConsumerEx")]
            public Edge RemovedConsumerExchange { get; set; }

            [JsonProperty("removedProducerEx")]
            public Edge RemovedProducerExchange { get; set; }

            #endregion

            public Node(Species species, int level)
            {
                Id = species.id;
                Label = species.sbmlId;
                Level = level;

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
                var r = new SortedSet<Edge>();
                r.UnionWith(this.Consumers);
                r.UnionWith(this.Producers);
                return r;
            }

            public SortedSet<Node> AllNeighborNodes()
            {
                var r = new SortedSet<Node>();
                r.UnionWith(this.Consumers.SelectMany(e => e.AllNodes()));
                r.UnionWith(this.Producers.SelectMany(e => e.AllNodes()));
                r.Remove(this);
                return r;
            }

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