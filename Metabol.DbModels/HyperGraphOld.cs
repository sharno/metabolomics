using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Metabol.DbModels.DB;
using Newtonsoft.Json;

namespace Metabol.DbModels
{
    //public enum NodeType
    //{
    //    New, Border, Selected, NewBorder, None
    //}

    //public enum EdgeType
    //{
    //    New, None
    //}

    /**
     * HyperGraph implementation  
     */
    public class HyperGraphOld
    {
        public int Step { get; set; }

        public int LastLevel
        {
            get { return Math.Max(Edges.Values.Select(v => v.Level).Max(), Nodes.Values.Select(n => n.Level).Max()); }
        }

        public ConcurrentDictionary<Guid, Node> Nodes { get; protected set; }

        //public HashSet<Guid> CommonMetabolites { get; protected set; }
        //public ConcurrentDictionary<Guid, HashSet<Guid>> PseudoPath { get; protected set; }

        public ConcurrentDictionary<Guid, Edge> Edges { get; protected set; }

        public ConcurrentDictionary<Guid, Cycle> Cycles { get; protected set; }

        private readonly ConcurrentDictionary<Guid, int> idMap = new ConcurrentDictionary<Guid, int>();
        private int idGen;

        public HyperGraphOld()
        {
            Edges = new ConcurrentDictionary<Guid, Edge>();
            Cycles = new ConcurrentDictionary<Guid, Cycle>();
            Nodes = new ConcurrentDictionary<Guid, Node>();
            //PseudoPath = new ConcurrentDictionary<Guid, HashSet<Guid>>();
            //CommonMetabolites = new HashSet<Guid>();
        }

        public void NextStep()
        {
            Step++;
        }

        //public void AddPseudoPath(Guid n1, Guid n2)
        //{
        //    PseudoPath.GetOrAdd(n1, new HashSet<Guid>()).Add(n2);
        //}

        //public bool ExistPseudoPath(Guid n1, Guid n2)
        //{
        //    return PseudoPath.ContainsKey(n1) && PseudoPath[n1].Contains(n2);
        //}

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

        private static int count;
        public void AddSpecies(Species species)
        {
            count++;
            Console.WriteLine("adding species" + count);
            var metabolite = Nodes.GetOrAdd(species.id, Node.Create(species.id, species.sbmlId, Step));

            var producers = species.ReactionSpecies.Where(rs => rs.roleId == DbModels.Db.ProductId && rs.Reaction.sbmlId != "biomass_reaction");
            var consumers = species.ReactionSpecies.Where(rs => rs.roleId == DbModels.Db.ReactantId && rs.Reaction.sbmlId != "biomass_reaction");

            foreach (var producer in producers)
            {
                var reaction = Edges.GetOrAdd(producer.reactionId, new Edge(producer.reactionId, producer.sbmlId, Step));
                AddProduct(reaction, metabolite, producer.stoichiometry);
            }
            foreach (var consumer in consumers)
            {
                var reaction = Edges.GetOrAdd(consumer.reactionId, new Edge(consumer.reactionId, consumer.sbmlId, Step));
                AddReactant(reaction, metabolite, consumer.stoichiometry);
            }
        }

        public void AddSpecies2(Species species)
        {
            count++;
            Console.WriteLine("adding species" + count);
            var metabolite = Nodes.GetOrAdd(species.id, Node.Create(species.id, species.sbmlId, Step));

            var producers = species.ReactionSpecies.Where(rs => rs.roleId == DbModels.Db.ProductId);
            var consumers = species.ReactionSpecies.Where(rs => rs.roleId == DbModels.Db.ReactantId);

            foreach (var producer in producers)
            {
                var reaction = Edges.GetOrAdd(producer.reactionId, new Edge(producer.reactionId, producer.sbmlId, Step));
                AddProduct(reaction, metabolite, producer.stoichiometry);
            }
            foreach (var consumer in consumers)
            {
                var reaction = Edges.GetOrAdd(consumer.reactionId, new Edge(consumer.reactionId, consumer.sbmlId, Step));
                AddReactant(reaction, metabolite, consumer.stoichiometry);
            }
        }

        public void AddReaction(Reaction reaction)
        {
            var r = Edges.GetOrAdd(reaction.id, Edge.Create(reaction.id, Step));

            var products = reaction.ReactionSpecies.Where(rs => rs.roleId == DbModels.Db.ProductId);
            var reactants = reaction.ReactionSpecies.Where(rs => rs.roleId == DbModels.Db.ReactantId);

            foreach (var product in products)
            {
                var m = Nodes.GetOrAdd(product.id, new Node(product.Species, Step));
                AddProduct(r, m, product.stoichiometry);
            }

            foreach (var reactant in reactants)
            {
                var m = Nodes.GetOrAdd(reactant.id, new Node(reactant.Species, Step));
                AddReactant(r, m, reactant.stoichiometry);
            }
        }

        public void AddReactant(Edge reaction, Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Consumers.Add(reaction);
            reaction.Reactants[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddReactant(Cycle cycle, Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);
            // todo remove this line when expanding and FBAing
            Edges.GetOrAdd(cycle.Id, cycle);

            metabolite.Consumers.Add(cycle);
            cycle.Reactants[metabolite.Id] = metabolite;

            metabolite.Weights[cycle.Id] = weight;
        }

        public void AddReactant(Guid eid, string elabel, Guid nid, string label, bool isPseudo)
        {
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label, Step));
            var edge = Edges.GetOrAdd(eid, Edge.Create(eid, Step));
            edge.Label = elabel;
            edge.AddReactant(node);
            edge.IsPseudo = isPseudo;
            if (!isPseudo)
            {
                var stoichiometry = DbModels.Db.Context.ReactionSpecies.Single(rs => rs.reactionId == eid && rs.speciesId == nid).stoichiometry;
                node.Weights[eid] = Math.Abs(stoichiometry);
            }

            node.UpdatePseudo();
        }

        public void AddProduct(Edge reaction, Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            reaction = Edges.GetOrAdd(reaction.Id, reaction);

            metabolite.Producers.Add(reaction);
            reaction.Products[metabolite.Id] = metabolite;

            metabolite.Weights[reaction.Id] = weight;
        }

        public void AddProduct(Cycle cycle, Node metabolite, double weight)
        {
            metabolite = Nodes.GetOrAdd(metabolite.Id, metabolite);
            cycle = Cycles.GetOrAdd(cycle.Id, cycle);
            // todo remove this line when expanding and FBAing
            Edges.GetOrAdd(cycle.Id, cycle);

            metabolite.Producers.Add(cycle);
            cycle.Products[metabolite.Id] = metabolite;

            metabolite.Weights[cycle.Id] = weight;
        }

        public void AddProduct(Guid eid, string elabel, Guid nid, string label, bool isPseudo)
        {
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label, Step));
            var edge = Edges.GetOrAdd(eid, Edge.Create(eid, Step));
            edge.Label = elabel;
            edge.AddProduct(node);
            edge.IsPseudo = isPseudo;
            if (!isPseudo)
            {
                var stoichiometry = DbModels.Db.Context.ReactionSpecies.Single(rs => rs.reactionId == eid && rs.speciesId == nid).stoichiometry;
                node.Weights[eid] = Math.Abs(stoichiometry);
            }
            //try
            //{
            //    if (!isPseudo) node.Weights[eid] = Math.Abs(Db.CachedRs(eid, nid).stoichiometry);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
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

        public IEnumerable<object> JsonNodes(Dictionary<Guid, int> z)
        {
            // "name": "Glucose", "type": "m", "isBorder": 1, "concentration": 10.1, 'change': '+'
            var lastLevel = LastLevel;
            var result = new List<object>();

            foreach (var node in Nodes.Where(node => node.Value.Level == lastLevel))
            {
                var ch = "0";
                if (z.ContainsKey(node.Key))
                    ch = z[node.Key] > 0 ? "+" : z[node.Key] < 0 ? "-" : "0";

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
                    V = node.Value.Flux
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
                    object n = new { source = idMap[node.Id], target = idMap[link.Value.Id], role = "s" };
                    //yield return n;
                    result.Add(n);
                }

                foreach (var node in link.Value.Products.Values)
                {
                    object n = new { source = idMap[link.Value.Id], target = idMap[node.Id], role = "p" };
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
            public Guid Id;

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

            public Cycle(DB2.Cycle cycle)
            {
                Id = cycle.id;
                Label = "cycle_" + cycle.id;
            }
        }

        public class Edge : Entity
        {
            #region Properties
            public bool IsPseudo { get; set; }

            public double Flux { get; set; }

            public double PreFlux { get; set; }

            public int Level { get; set; }

            public bool IsReversible { get; set; }

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

            public HashSet<Guid> UpdatePseudo = new HashSet<Guid>();
            public HashSet<Guid> Reactions = new HashSet<Guid>();
            #endregion

            public Edge()
            {
                Id = new Guid();
            }

            public Edge(Guid id, int i)
            {
                Id = id;
                Level = i;
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
                return new HashSet<Node>(Reactants.Values.Concat(Products.Values));
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
                        if (Math.Abs(Flux) < double.Epsilon)
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
                        if (Math.Abs(Flux) < double.Epsilon)
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
                var bu = new StringBuilder(
                    $"an \"{Id}\" {hedgeclass} label:\" {Label}({Flux:#.#####})({PreFlux:#.#####}) \"\r\n"); //

                foreach (var node in Reactants.Values)
                    bu.Append(
                        $"ae \"{node.Id}{Id}\" \"{node.Id}\" > \"{Id}\" {uiclass}  label:\" {(node.Weights[Id] == 0 ? 1 : node.Weights[Id])} \"\r\n");

                foreach (var node in Products.Values)
                    bu.Append(
                        $"ae \"{Id}{node.Id}\" \"{Id}\" > \"{node.Id}\" {uiclass} label:\" {(node.Weights[Id] == 0 ? 1 : node.Weights[Id])} \"\r\n");

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
                var bu = new StringBuilder(
                    $"an \"{Id}\" {hedgeclass} label:\" {Label}({Flux:#.#####})({PreFlux:#.#####}) \"\r\n"); //

                foreach (var node in Reactants.Values)
                    bu.Append($"ae \"{node.Id}{Id}\" \"{node.Id}\" > \"{Id}\" {uiclass}\r\n");

                foreach (var node in Products.Values)
                    bu.Append($"ae \"{Id}{node.Id}\" \"{Id}\" > \"{node.Id}\" {uiclass}\r\n");

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
                var bu = new StringBuilder($"an \"{Id}\" {hedgeclass} label:\" {Label} \"\r\n"); //

                foreach (var node in Reactants.Values)
                    bu.Append($"ae \"{node.Id}{Id}\" \"{node.Id}\" > \"{Id}\" {uiclass}\r\n");

                foreach (var node in Products.Values)
                    bu.Append($"ae \"{Id}{node.Id}\" \"{Id}\" > \"{node.Id}\" {uiclass}\r\n");

                return bu.ToString();
            }
        }

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

            public readonly dynamic ReactionCount;

            public bool IsExtended { get; set; }

            [JsonIgnore]
            public Species ToSpecies => DbModels.Db.Context.Species.Find(Id);

            [JsonIgnore]
            public bool IsLonely => ReactionCount.Consumers == 0 || ReactionCount.Producers == 0;

            public bool IsBorder => IsConsumedBorder || IsProducedBorder;

            public bool IsConsumedBorder
            {
                get
                {
                    return ReactionCount.Consumers != Consumers.Count(e => !e.IsPseudo);
                }
            }

            public bool IsProducedBorder
            {
                get
                {
                    return ReactionCount.Producers != Producers.Count(e => !e.IsPseudo);
                }
            }

            [JsonIgnore]
            public bool IsTempBorder
            {
                get
                {
                    return ((Producers.Any(s => s.IsPseudo) && IsProducedBorder)
                        || (Consumers.Any(s => s.IsPseudo) && IsConsumedBorder));
                }
            }

            [JsonIgnore]
            public int TotalReaction => ReactionCount.Consumers + ReactionCount.Producers;

            public Dictionary<Guid, double> Weights = new Dictionary<Guid, double>();

            public Edge RemovedConsumerExchange { get; set; }

            public Edge RemovedProducerExchange { get; set; }

            //[JsonProperty("pseudoConsumerVars")]
            //public HashSet<string> PseudoConsumerVars { get; set; }

            //[JsonProperty("pseudoProducerVars")]
            //public HashSet<string> PseudoProducerVars { get; set; }

            #endregion

            public Node(Species species, int level)
            {
                Id = species.id;
                Label = species.sbmlId;
                Level = level;
                ReactionCount = new
                {
                    Consumers = species.ReactionSpecies.Count(rs => rs.roleId == DbModels.Db.ReactantId),
                    Producers = species.ReactionSpecies.Count(rs => rs.roleId == DbModels.Db.ProductId)
                };
            }

            public Node(Guid id, string label, int level)
            {
                Label = label;
                Id = id;
                Level = level;
                ReactionCount = new
                {
                    Consumers = DbModels.Db.Context.ReactionSpecies.Count(rs => rs.roleId == DbModels.Db.ReactantId && rs.speciesId == Id),
                    Producers = DbModels.Db.Context.ReactionSpecies.Count(rs => rs.roleId == DbModels.Db.ProductId && rs.speciesId == Id)
                };
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
                var im = Consumers.Count(e => e.IsPseudo) != 0 ? Consumers.First(e => e.IsPseudo) : null;
                var io = Producers.Count(e => e.IsPseudo) != 0 ? Producers.First(e => e.IsPseudo) : null;

                if (im != null)
                {
                    var wsum = Consumers.Count == 1
                          ? 0
                          : Consumers.Where(edge => !edge.IsPseudo).Sum(edge => Weights[edge.Id]);
                    Weights[im.Id] = 0;//DbModels.Db.GetStoichiometry(Id).Consumers - wsum;
                }

                if (io != null)
                {
                    var wsum = Producers.Count == 1
                          ? 0
                          : Producers.Where(edge => !edge.IsPseudo).Sum(edge => Weights[edge.Id]);

                    Weights[io.Id] = 0;//DbModels.Db.GetStoichiometry(Id).Producers - wsum;
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

                return $"an \"{Id}\"  label:\" {Label} \"  {uiclass}";
            }

            public IEnumerable<Edge> AllReactions()
            {
                var r = new SortedSet<Edge>();
                r.UnionWith(Consumers);
                r.UnionWith(Producers);
                return r;
            }

            public SortedSet<Node> AllNeighborNodes()
            {
                var r = new SortedSet<Node>();
                r.UnionWith(Consumers.SelectMany(e => e.AllNodes()));
                r.UnionWith(Producers.SelectMany(e => e.AllNodes()));
                r.Remove(this);
                return r;
            }
        }

    }
}