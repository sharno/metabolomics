using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Metabol.Util
{
    using Metabol.Util.DB2;

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
            get { return Math.Max(Edges.Values.Select(v => v.Level).Max(), Nodes.Values.Select(n => n.Level).Max()); }
        }

        [JsonProperty("nodes")]
        public ConcurrentDictionary<Guid, Node> Nodes { get; protected set; }

        public HashSet<Guid> CommonMetabolites { get; protected set; }
        public ConcurrentDictionary<Guid, HashSet<Guid>> PseudoPath { get; protected set; }

        [JsonProperty("edges")]
        public ConcurrentDictionary<Guid, Edge> Edges { get; protected set; }

        [JsonProperty("cycles")]
        public ConcurrentDictionary<Guid, Cycle> Cycles { get; protected set; }


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
            return Nodes.AddOrUpdate(id, Node.Create(id, label, Step), (guid, node) => node);
        }

        public Node AddNode(Guid id, string label, bool b)
        {
            return Nodes.AddOrUpdate(id, Node.Create(id, label, Step, b), (guid, node) => node);
        }

        private static int count = 0;
        public void AddSpecies(Species species)
        {
            count ++;
            Console.WriteLine("adding species" + count);
            var metabolite = Nodes.GetOrAdd(species.id, Node.Create(species.id, species.sbmlId, Step));

            var producers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ProductId && rs.Reaction.sbmlId != "R_biomass_reaction");
            var consumers = species.ReactionSpecies.Where(rs => rs.roleId == Db.ReactantId && rs.Reaction.sbmlId != "R_biomass_reaction");

            foreach (var producer in producers)
            {
                var reaction = Edges.GetOrAdd(producer.reactionId, new Edge(producer.reactionId, producer.sbmlId, Step));
                AddProduct(reaction, metabolite, producer.stoichiometry);
            }
            foreach (var consumer in consumers)
            {
                var reaction = Edges.GetOrAdd(consumer.reactionId, Edge.Create(consumer.reactionId, Step));
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

        public void AddReactant(HyperGraph.Cycle cycle, HyperGraph.Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);

            metabolite.Consumers.Add(cycle);
            cycle.Reactants[metabolite.Id] = metabolite;

            metabolite.Weights[cycle.Id] = weight;
        }

        public void AddReactant(Guid eid, string elabel, Guid nid, string label, bool isPseudo)
        {
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label, Step));
            var e = Edges.GetOrAdd(eid, Edge.Create(eid, Step));
            e.Label = elabel;
            e.AddReactant(node);
            e.IsPseudo = isPseudo;
            try
            {
                if (!isPseudo) node.Weights[eid] = Math.Abs(Db.CachedRs(eid, nid).stoichiometry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            node.UpdatePseudo();
        }

        public void AddProduct(HyperGraph.Edge reaction, HyperGraph.Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Producers.Add(reaction);
            reaction.Products[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddProduct(HyperGraph.Cycle cycle, HyperGraph.Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);

            metabolite.Producers.Add(cycle);
            cycle.Products[metabolite.Id] = metabolite;

            metabolite.Weights[cycle.Id] = weight;
        }

        public void AddProduct(Guid eid, string elabel, Guid nid, string label, bool isPseudo)
        {
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label, Step));
            var e = Edges.GetOrAdd(eid, Edge.Create(eid, Step));
            e.Label = elabel;
            e.AddProduct(node);
            e.IsPseudo = isPseudo;
            try
            {
                if (!isPseudo) node.Weights[eid] = Math.Abs(Db.CachedRs(eid, nid).stoichiometry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            node.UpdatePseudo();
        }

        public void AddProduct(Guid eid, string id, Guid nid, string sbmlId)
        {
            AddProduct(eid, id, nid, sbmlId, false);
        }

        public void AddReactant(Guid eid, string elabel, Guid nid, string sbmlId)
        {
            AddReactant(eid, elabel, nid, sbmlId, false);
        }

        public void AddReactant(Guid eid, string elabel, Guid nid, string label, bool isPseudo, bool isCycle)
        {
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label, Step));
            var e = Edges.GetOrAdd(eid, Edge.Create(eid, Step));
            e.Label = elabel;
            e.AddReactant(node);
            e.IsPseudo = isPseudo;
            // TODO remove is cycle
        }

        public void AddProduct(Guid eid, string elabel, Guid nid, string label, bool isPseudo, bool isCycle)
        {
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label, Step));
            var e = Edges.GetOrAdd(eid, Edge.Create(eid, Step));
            e.Label = elabel;
            e.AddProduct(node);
            e.IsPseudo = isPseudo;
            // TODO remove is cycle
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
        }

        public void RemoveNode(Guid nid)
        {
            foreach (var consumer in Nodes[nid].Consumers)
            {
                consumer.Reactants.Remove(nid);
                if (consumer.Reactants.Count == 0 && consumer.Products.Count == 0)
                {
                    Edge ingoredEdge;
                    Edges.TryRemove(consumer.Id, out ingoredEdge);
                }
            }

            foreach (var producer in Nodes[nid].Producers)
            {
                producer.Products.Remove(nid);
                if (producer.Reactants.Count == 0 && producer.Products.Count == 0)
                {
                    Edge ingoredEdge;
                    Edges.TryRemove(producer.Id, out ingoredEdge);
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

            public Cycle()
            {
                Id = Guid.NewGuid();
                Label = "cycle";
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

            [JsonProperty("updatePseudo")]
            public HashSet<Guid> UpdatePseudo { get; set; }

            public HashSet<Guid> Reactions { get; set; }
            #endregion

            public Edge()
            {
                Id = new Guid();
            }

            public Edge(Guid id, int i)
            {
                UpdatePseudo = new HashSet<Guid>();
                Reactions = new HashSet<Guid>();

                Id = id;
                Level = i;
                this.PreFlux = double.NegativeInfinity;
            }

            public Edge(Guid id, string label, int level)
            {
                Id = id;
                Label = label;
                Level = level;
            }

            public static Edge Create(Guid id, int i)
            {
                return new Edge(id, i);
            }

            public Edge AddReactant(Node node)
            {
                Reactants[node.Id] = node;
                //Reactants.AddOrUpdate(node.Id, node, (guid, node1) => node1);
                node.Consumers.Add(this);
                //if (node.IsPseudo)
                //{
                //    //s2
                //    node.Weights[Id] = 1.0;
                //}
                return this;
            }

            public Edge AddProduct(Node node)
            {
                Products[node.Id] = node;
                //Products.AddOrUpdate(node.Id, node, (guid, node1) => node1);
                node.Producers.Add(this);
                //if (node.IsPseudo)
                //{
                //    //s1
                //    node.Weights[Id] = 1.0;
                //}
                return this;
            }

            public IEnumerable<Node> AllNodes()
            {
                return new HashSet<Node>(this.Reactants.Values.Concat(this.Products.Values));
            }

            public string ToDgs(EdgeType type)
            {
                //var d1 = theAlgorithm.Fba.Results.ContainsKey(Label) ? theAlgorithm.Fba.Results[Label] : -1.0;
                //var d2 = theAlgorithm.Fba.PrevResults.ContainsKey(Label) ? theAlgorithm.Fba.PrevResults[Label] : -1.0;

                var uiclass = "";
                var hedgeclass = "";
                switch (type)
                {
                    case EdgeType.New:
                        if (Math.Abs(this.Flux) < double.Epsilon)
                        {
                            hedgeclass = " ui.class:newhedge0 ";
                            uiclass = " ui.class:new ";
                        }
                        else
                        {
                            hedgeclass = " ui.class:newhedge ";
                            uiclass = " ui.class:new ";
                        }
                        break;

                    case EdgeType.None:
                        if (Math.Abs(this.Flux) < double.Epsilon)
                        {
                            uiclass = " ui.class:new0 ";
                            hedgeclass = " ui.class:hedge0 ";
                        }
                        else
                        {
                            uiclass = " ";
                            hedgeclass = " ui.class:hedge ";
                        }

                        break;
                }
                var bu = new StringBuilder(string.Format("an \"{0}\" {1} label:\" {2}({3:#.#####})({4:#.#####}) \"\r\n", Id, hedgeclass, Label, this.Flux, this.PreFlux)); //

                foreach (var node in this.Reactants.Values)
                    bu.Append(string.Format("ae \"{0}{1}\" \"{2}\" > \"{3}\" {4}  label:\" {5} \"\r\n", node.Id, Id, node.Id, Id, uiclass, node.Weights[Id] == 0 ? 1 : node.Weights[Id]));

                foreach (var node in this.Products.Values)
                    bu.Append(string.Format("ae \"{0}{1}\" \"{2}\" > \"{3}\" {4} label:\" {5} \"\r\n", Id, node.Id, Id, node.Id, uiclass, node.Weights[Id] == 0 ? 1 : node.Weights[Id]));

                return bu.ToString();
            }

            public string ToDgs1(EdgeType type)
            {
                //var d1 = results.ContainsKey(Label) ? results[Label] : -1.0;
                //var d2 = prevResults.ContainsKey(Label) ? prevResults[Label] : -1.0;

                var uiclass = "";
                var hedgeclass = "";
                switch (type)
                {
                    case EdgeType.New:
                        uiclass = " ui.class:new ";
                        hedgeclass = " ui.class:newhedge ";
                        break;

                    case EdgeType.None:
                        uiclass = " ";
                        hedgeclass = " ui.class:hedge ";
                        break;
                }
                var bu = new StringBuilder(string.Format("an \"{0}\" {1} label:\" {2}({3:#.#####})({4:#.#####}) \"\r\n", Id, hedgeclass, Label, this.Flux, this.PreFlux)); //

                foreach (var node in this.Reactants.Values)
                    bu.Append(string.Format("ae \"{0}{1}\" \"{2}\" > \"{3}\" {4}\r\n", node.Id, Id, node.Id, Id, uiclass));

                foreach (var node in this.Products.Values)
                    bu.Append(string.Format("ae \"{0}{1}\" \"{2}\" > \"{3}\" {4}\r\n", Id, node.Id, Id, node.Id, uiclass));

                return bu.ToString();

            }

            public string ToDgs2(EdgeType type)
            {
                var uiclass = "";
                var hedgeclass = "";
                switch (type)
                {
                    case EdgeType.New:
                        uiclass = " ui.class:new ";
                        hedgeclass = " ui.class:newhedge ";
                        break;

                    case EdgeType.None:
                        uiclass = " ";
                        hedgeclass = " ui.class:hedge ";
                        break;
                }
                var bu = new StringBuilder(string.Format("an \"{0}\" {1} label:\" {2} \"\r\n", Id, hedgeclass, Label)); //

                foreach (var node in this.Reactants.Values)
                    bu.Append(string.Format("ae \"{0}{1}\" \"{2}\" > \"{3}\" {4}\r\n", node.Id, Id, node.Id, Id, uiclass));

                foreach (var node in this.Products.Values)
                    bu.Append(string.Format("ae \"{0}{1}\" \"{2}\" > \"{3}\" {4}\r\n", Id, node.Id, Id, node.Id, uiclass));

                return bu.ToString();
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
            public readonly dynamic ReactionCount;

            [JsonProperty("isExtended")]
            public bool IsExtended { get; set; }

            [JsonIgnore]
            public Species ToSpecies
            {
                get
                {
                    return Db.CachedS(Id);
                }
            }

            [JsonIgnore]
            public bool IsLonely
            {
                get
                {
                    //return !IsBorder
                    //       && ((Consumers.Count == 0 && Producers.Count != 0)
                    //           || (Consumers.Count != 0 && Producers.Count == 0));
                    return ReactionCount.Consumers == 0 || ReactionCount.Producers == 0;
                }
            }

            [JsonProperty("isBorder")]
            public bool IsBorder
            {
                get
                {
                    return IsConsumedBorder || IsProducedBorder;
                }
            }

            [JsonProperty("isConsumedBorder")]
            public bool IsConsumedBorder
            {
                get
                {
                    return ReactionCount.Consumers != this.Consumers.Count(e => !e.IsPseudo);
                }
            }

            [JsonProperty("isProducedBorder")]
            public bool IsProducedBorder
            {
                get
                {
                    return ReactionCount.Producers != this.Producers.Count(e => !e.IsPseudo);
                }
            }

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
            public Dictionary<Guid, double> Weights { get; set; }

            [JsonProperty("removedConsumerEx")]
            public Edge RemovedConsumerExchange { get; set; }

            [JsonProperty("removedProducerEx")]
            public Edge RemovedProducerExchange { get; set; }

            //[JsonProperty("pseudoConsumerVars")]
            //public HashSet<string> PseudoConsumerVars { get; set; }

            //[JsonProperty("pseudoProducerVars")]
            //public HashSet<string> PseudoProducerVars { get; set; }

            #endregion

            public Node(Guid id, string label, int level)
            {
                Label = label;
                Id = id;
                Level = level;
                Weights = new Dictionary<Guid, double>();
                //PseudoConsumerVars = new HashSet<string>();
                //PseudoProducerVars = new HashSet<string>();

                //try
                //{
                //    ReactionCount = Db.GetReactionCount(id);
                //    Db.GetStoichiometry(id);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //}
            }
            

            private Node(Guid id, string label, int lastLevel, bool common)
                : this(id, label, lastLevel)
            {
                Id = Guid.NewGuid();
                RealId = id;
                IsCommon = common;
                //ReactionCount = Tuple.Create(0, 0);
                //Util.AllStoichiometryCache[id] = Tuple.Create(1.0, 1.0);
            }

            public static Node Create(Guid id, string label, int i)
            {
                return new Node(id, label, i);
            }

            public static Node Create(Guid newGuid, string label, int lastLevel, bool common)
            {
                return new Node(newGuid, label, lastLevel, common);
            }

            public void UpdatePseudo()
            {
                var im = this.Consumers.Count(e => e.IsPseudo) != 0 ? this.Consumers.First(e => e.IsPseudo) : null;
                var io = this.Producers.Count(e => e.IsPseudo) != 0 ? this.Producers.First(e => e.IsPseudo) : null;

                if (im != null)
                {
                    var wsum = this.Consumers.Count == 1
                          ? 0
                          : this.Consumers.Where(edge => !edge.IsPseudo).Sum(edge => Weights[edge.Id]);
                    Weights[im.Id] = Db.GetStoichiometry(Id).Consumers - wsum;
                }

                if (io != null)
                {
                    var wsum = this.Producers.Count == 1
                          ? 0
                          : this.Producers.Where(edge => !edge.IsPseudo).Sum(edge => Weights[edge.Id]);

                    Weights[io.Id] = Db.GetStoichiometry(Id).Producers - wsum;
                }
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

                return string.Format("an \"{0}\"  label:\" {1} \"  {2}", Id, Label, uiclass);
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
        }

    }
}