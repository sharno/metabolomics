using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathwaysLib.ServerObjects;

namespace Metabol
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
    public class HGraph
    {
        private int step;
        private readonly ConcurrentDictionary<Guid, int> idMap = new ConcurrentDictionary<Guid, int>();
        private int idGen;

        public HGraph()
        {
            Edges = new ConcurrentDictionary<Guid, Edge>();
            Nodes = new ConcurrentDictionary<Guid, Node>();
        }

        internal void NextStep()
        {
            step++;
        }

        protected internal ConcurrentDictionary<Guid, Node> Nodes { get; protected set; }

        protected internal ConcurrentDictionary<Guid, Edge> Edges { get; protected set; }

        internal void AddInputNode(Guid eid, string elabel, Guid nid, string label, bool b)
        {
            var exist = Nodes.ContainsKey(nid);
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label));
            if (!exist) node.Level = step;

            exist = Edges.ContainsKey(eid);
            var e = Edges.GetOrAdd(eid, Edge.Create(eid));
            e.Label = elabel;
            e.AddInput(node);
            e.IsImaginary = b;
            if (!exist) e.Level = step;
        }

        internal void AddOuputNode(Guid eid, string elabel, Guid nid, string label, bool b)
        {
            var exist = Nodes.ContainsKey(nid);
            var node = Nodes.GetOrAdd(nid, Node.Create(nid, label));
            if (!exist) node.Level = step;

            exist = Edges.ContainsKey(eid);
            var e = Edges.GetOrAdd(eid, Edge.Create(eid));
            e.Label = elabel;
            e.AddOutput(node);
            e.IsImaginary = b;
            if (!exist) e.Level = step;
        }

        internal void AddNode(Guid id, string label)
        {
            Nodes.AddOrUpdate(id, Node.Create(id, label), (guid, node) => node);
        }

        internal void AddOuputNode(Guid eid, string id, Guid nid, string sbmlId)
        {
            AddOuputNode(eid, id, nid, sbmlId, false);
        }

        internal void AddInputNode(Guid eid, string elabel, Guid nid, string sbmlId)
        {
            AddInputNode(eid, elabel, nid, sbmlId, false);
        }

        internal void Clear()
        {
            idMap.Clear();
            foreach (var edge in Edges)
            {
                edge.Value.InputNodes.Clear();
                edge.Value.OuputNodes.Clear();
            }
            foreach (var node in Nodes)
            {
                node.Value.InputToEdge.Clear();
                node.Value.OutputFromEdge.Clear();
            }
        }

        /**
         * fancy  Hyperedge
         */
        public class Edge
        {
            internal readonly Guid Id;
            internal bool IsImaginary { get; set; }
            internal string Label { get; set; }
            internal int Level { get; set; }

            internal Edge(Guid id)
            {
                OuputNodes = new ConcurrentDictionary<Guid, Node>();
                InputNodes = new ConcurrentDictionary<Guid, Node>();
                Id = id;
                //Level = step;
            }

            internal ServerReaction ToServerReaction => Util.CachedR(Id);

            internal ConcurrentDictionary<Guid, Node> InputNodes { get; set; }

            internal ConcurrentDictionary<Guid, Node> OuputNodes { get; set; }

            internal static Edge Create(Guid id)
            {
                return new Edge(id);
            }

            internal Edge AddInput(Node node)
            {
                InputNodes.AddOrUpdate(node.Id, node, (guid, node1) => node1);
                node.InputToEdge.Add(this);
                return this;
            }

            internal Edge AddOutput(Node node)
            {
                OuputNodes.AddOrUpdate(node.Id, node, (guid, node1) => node1);
                node.OutputFromEdge.Add(this);
                return this;
            }

            internal IEnumerable<Node> AllNodes()
            {
                return InputNodes.Values.Concat(OuputNodes.Values);
            }

            internal string ToDgs(EdgeType type, TheAlgorithm theAlgorithm)
            {
                var d1 = theAlgorithm.Fba.Results.ContainsKey(Id) ? theAlgorithm.Fba.Results[Id] : -1.0;
                var d2 = theAlgorithm.Fba.PrevResults.ContainsKey(Id) ? theAlgorithm.Fba.PrevResults[Id] : -1.0;

                var bu = new StringBuilder($"an \"{Id}\" ui.class:hedge label:\"{Label}({d1})({d2})\"\r\n"); //
                var uiclass = "";
                switch (type)
                {
                    case EdgeType.New:
                        uiclass = " ui.class:new ";
                        break;

                    case EdgeType.None:
                        uiclass = " ";
                        break;
                }

                foreach (var node in InputNodes.Values)
                    bu.Append($"ae \"{node.Id}{Id}\" \"{node.Id}\" > \"{Id}\" {uiclass}\r\n");

                foreach (var node in OuputNodes.Values)
                    bu.Append($"ae \"{Id}{node.Id}\" \"{Id}\" > \"{node.Id}\" {uiclass}\r\n");

                return bu.ToString();
            }

            public override string ToString()
            {
                return Label;
            }

            public override bool Equals(object obj)
            {
                return obj is Edge && ((Edge)obj).Id.Equals(Id);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }

        /**
         * simple node ( not hypernode )
         */
        public class Node
        {
            internal readonly Guid Id;
            internal string Label;
            internal HashSet<Edge> InputToEdge = new HashSet<Edge>();
            internal HashSet<Edge> OutputFromEdge = new HashSet<Edge>();
            internal int Level { get; set; }
            //private bool isNonBorder;
            public readonly Tuple<int, int> AllReactions;

            internal Node(Guid id, string l)
            {
                Label = l;
                Id = id;
                AllReactions = Util.AllReactionCache[id];
            }

            internal ServerSpecies ToSpecies => Util.CachedS(Id);

            internal static Node Create(Guid id, string label)
            {
                return new Node(id, label);
            }

            internal bool IsLonely => !IsBorder && ((InputToEdge.Count == 0 && OutputFromEdge.Count != 0) || (InputToEdge.Count != 0 && OutputFromEdge.Count == 0));

            internal bool IsBorder => IsConsumedBorder || IsProducedBorder;

            internal bool IsConsumedBorder
            {
                get { return AllReactions.Item2 != InputToEdge.Count(e => !e.IsImaginary); }
            }

            internal bool IsProducedBorder
            {
                get { return AllReactions.Item1 != OutputFromEdge.Count(e => !e.IsImaginary); }
            }

            internal bool IsTempBorder
            {
                get
                {
                    return !IsBorder && ((OutputFromEdge.Any(s => s.IsImaginary)) || (InputToEdge.Any(s => s.IsImaginary)));
                }
            }

            internal int TotalReaction => AllReactions.Item1 + AllReactions.Item2;

            internal IEnumerable<Node> AllNeighborNodes()
            {
                var r = new HashSet<Node>();
                r.UnionWith(InputToEdge.SelectMany(e => e.AllNodes()));
                r.UnionWith(OutputFromEdge.SelectMany(e => e.AllNodes()));
                r.Remove(this);
                //return InputToEdge.SelectMany(e => e.AllNodes()).Concat(OutputFromEdge.SelectMany(e => e.AllNodes())).Where(n => n.Id != Id);
                return r;
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

                return $"an \"{Id}\"  label:\"{Label}\"  {uiclass}";
            }

            public override string ToString()
            {
                return Label;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is Node && ((Node)obj).Id.Equals(Id);
            }
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
                    V = 1.0
                };
                idMap[node.Key] = idGen;
                idGen++;
                //yield return n;
                result.Add(n);
            }
            return result;
        }

        public int LastLevel
        {
            get { return Math.Max(Edges.Values.Select(v => v.Level).Max(), Nodes.Values.Select(n => n.Level).Max()); }
        }

        public IEnumerable<object> JsonLinks()
        {
            // { "source": 0, "target": 3, "role": "s" }
            // { "source": 3, "target": 4, "role": "p" }
            var lastLevel = LastLevel;
            var result = new List<object>();

            foreach (var link in Edges.Where(node => node.Value.Level == lastLevel))
            {
                foreach (var node in link.Value.InputNodes.Values)
                {
                    object n = new { source = idMap[node.Id], target = idMap[link.Value.Id], role = "s" };
                    //yield return n;
                    result.Add(n);
                }

                foreach (var node in link.Value.OuputNodes.Values)
                {
                    object n = new { source = idMap[link.Value.Id], target = idMap[node.Id], role = "p" };
                    //yield return n;
                    result.Add(n);
                }
            }
            return result;

        }
    }
}