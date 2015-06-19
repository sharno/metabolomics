using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// An adjacency list representation of a graph
    /// </summary>
    [SerializableAttribute]
    public class GraphAdjacencyList : IGraph
    {
        //private List<INode> _vertices;
        private Dictionary<Guid, INode> _vertices;
        private Dictionary<Guid, Dictionary<string, List<Guid>>> _edges; // An index on the directed edges
        private Dictionary<Guid, Dictionary<string, List<Guid>>> _edgesReversed; // An index on the directed edges, in the reverse direction as stored in _edges

        public List<INode> Vertices
        {
            get
            {
                return new List<INode>(_vertices.Values);
            }
        }

        public List<IEdge> Edges
        {
            get
            {
                List<IEdge> retVal = new List<IEdge>();
                foreach(Guid fromNode in _edges.Keys)
                    foreach(string edgeLabel in _edges[fromNode].Keys)
                        foreach(Guid toNode in _edges[fromNode][edgeLabel])
                            retVal.Add(new Edge(GetNode(fromNode), GetNode(toNode), edgeLabel));

                return retVal;
            }
        }

        public GraphAdjacencyList()
        {
            _vertices = new Dictionary<Guid, INode>();
            _edges = new Dictionary<Guid, Dictionary<string, List<Guid>>>();
            _edgesReversed = new Dictionary<Guid, Dictionary<string, List<Guid>>>();
        }

        public void AddVertex(INode node)
        {
            if(!_vertices.ContainsKey(node.Id))
                _vertices.Add(node.Id, node);
        }

        public INode GetNode(Guid id)
        {
            if (!_vertices.ContainsKey(id))
                return null;
            return _vertices[id];
        }

        public List<INode> GetNodes(List<Guid> nodeIds)
        {
            List<INode> results = new List<INode>(nodeIds.Count);
            foreach (Guid id in nodeIds)
            {
                INode n = GetNode(id);
                if (n != null)
                    results.Add(n);
            }
            return results;
        }

        public string GetNodeLabel(Guid id)
        {
            if (!_vertices.ContainsKey(id))
                return string.Format("<Unlabeled Node {0}>", id);
            return _vertices[id].Label;
        }

        public bool ContainsNode(Guid id)
        {
            return GetNode(id) != null;
        }

        public void AddEdge(IEdge edge)
        {
            /* --- Directed Edges (_edges) --- */
            // Add another dictionary to the dictionary if the from node hasn't yet been seen
            if(!_edges.ContainsKey(edge.FromNode.Id))
            {
                _edges[edge.FromNode.Id] = new Dictionary<string, List<Guid>>(); // labeled edges
                _edges[edge.FromNode.Id][""] = new List<Guid>(); // unlabeled edges
            }

            // Add another linked list to the dictionary if the edge label hasn't yet been seen
            if (!_edges[edge.FromNode.Id].ContainsKey(edge.Label))
                _edges[edge.FromNode.Id][edge.Label] = new List<Guid>();

            // Update the linked list only if the to node doesn't have a connection with the from node
            if (!_edges[edge.FromNode.Id][edge.Label].Contains(edge.ToNode.Id))
                _edges[edge.FromNode.Id][edge.Label].Add(edge.ToNode.Id);
            if(edge.Label != "" && !_edges[edge.FromNode.Id][""].Contains(edge.ToNode.Id))
                _edges[edge.FromNode.Id][""].Add(edge.ToNode.Id);

            /* --- Reversed Directed Edges (_edgesReversed) --- */

            // Add another linked list to the reversed dictionary if the to node hasn't yet been seen
            if(!_edgesReversed.ContainsKey(edge.ToNode.Id))
            {
                _edgesReversed[edge.ToNode.Id] = new Dictionary<string, List<Guid>>();
                _edgesReversed[edge.ToNode.Id][""] = new List<Guid>();
            }

            // Add another linked list to the dictionary if the edge label hasn't yet been seen
            if (!_edgesReversed[edge.ToNode.Id].ContainsKey(edge.Label))
                _edgesReversed[edge.ToNode.Id][edge.Label] = new List<Guid>();

            // Update the linked list only if the to node doesn't have a connection with the from node
            if (!_edgesReversed[edge.ToNode.Id][edge.Label].Contains(edge.FromNode.Id))
                _edgesReversed[edge.ToNode.Id][edge.Label].Add(edge.FromNode.Id);
            if (edge.Label != "" && !_edgesReversed[edge.ToNode.Id][""].Contains(edge.FromNode.Id))
                _edgesReversed[edge.ToNode.Id][""].Add(edge.FromNode.Id);

            return;
        }

        /// <summary>
        /// Fetch the nodes that are adjacent to a node with type NodeType.Node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="edgeType"></param>
        /// <returns></returns>
        public List<INode> AdjacentNodes(INode node, EdgeType edgeType)
        {
            switch(edgeType)
            {
                case EdgeType.Directed: // Search only _edges
                    return _edges.ContainsKey(node.Id) ? GetNodes(_edges[node.Id][""]) : new List<INode>();
                case EdgeType.DirectedReversed: // Search only _edgesReversed
                    return _edgesReversed.ContainsKey(node.Id) ? GetNodes(_edgesReversed[node.Id][""]) : new List<INode>();
                case EdgeType.Undirected: // Search both _edges and _edgesReversed
                    List<INode> retVal = new List<INode>();
                    if(_edges.ContainsKey(node.Id))
                        foreach (Guid nid in _edges[node.Id][""])
                        {
                            INode n = GetNode(nid);
                            if (!retVal.Contains(n))
                                retVal.Add(n);
                        }
                    if(_edgesReversed.ContainsKey(node.Id))
                        foreach (Guid nid in _edgesReversed[node.Id][""])
                        {
                            INode n = GetNode(nid);
                            if (!retVal.Contains(n))
                                retVal.Add(n);
                        }
                    return retVal;
                default: // This case *should* never happen
                    return new List<INode>();
            }
        }

        /// <summary>
        /// Fetch the nodes that are adjacent to a node with type NodeType.Edge given the way we took in; either 'product' or 'substrate'
        /// </summary>
        /// <param name="node"></param>
        /// <param name="edgeType"></param>
        /// <param name="previousEdgeLabel"></param>
        /// <returns></returns>
        public List<INode> AdjacentNodes(INode node, EdgeType edgeType, string previousEdgeLabel)
        {
            switch(edgeType)
            {
                case EdgeType.Directed:
                    if(!_edges.ContainsKey(node.Id))
                        return new List<INode>();
                    else if (previousEdgeLabel.Equals("p") && _edges[node.Id].ContainsKey("s"))
                        return GetNodes(_edges[node.Id]["s"]);
                    else if (previousEdgeLabel.Equals("s") && _edges[node.Id].ContainsKey("p"))
                        return GetNodes(_edges[node.Id]["p"]);
                    else
                        return new List<INode>();

                case EdgeType.DirectedReversed:
                    if (!_edges.ContainsKey(node.Id))
                        return new List<INode>();
                    else if (previousEdgeLabel.Equals("p") && _edgesReversed[node.Id].ContainsKey("s"))
                        return GetNodes(_edgesReversed[node.Id]["s"]);
                    else if (previousEdgeLabel.Equals("s") && _edgesReversed[node.Id].ContainsKey("p"))
                        return GetNodes(_edgesReversed[node.Id]["p"]);
                    else
                        return new List<INode>();

                case EdgeType.Undirected:
                    List<INode> retVal = new List<INode>();
                    if(previousEdgeLabel.Equals("p"))
                    {
                        if (_edges.ContainsKey(node.Id) && _edges[node.Id].ContainsKey("s"))
                            foreach (Guid nid in _edges[node.Id]["s"])
                            {
                                INode n = GetNode(nid);
                                if (!retVal.Contains(n))
                                    retVal.Add(n);
                            }
                        if (_edgesReversed.ContainsKey(node.Id) && _edgesReversed[node.Id].ContainsKey("s"))
                            foreach (Guid nid in _edgesReversed[node.Id]["s"])
                            {
                                INode n = GetNode(nid);
                                if (!retVal.Contains(n))
                                    retVal.Add(n);
                            }
                        return retVal;
                    }
                    else if(previousEdgeLabel.Equals("s"))
                    {
                        if (_edges.ContainsKey(node.Id) && _edges[node.Id].ContainsKey("p"))
                            foreach (Guid nid in _edges[node.Id]["p"])
                            {
                                INode n = GetNode(nid);
                                if (!retVal.Contains(n))
                                    retVal.Add(n);
                            }
                        if (_edgesReversed.ContainsKey(node.Id) && _edgesReversed[node.Id].ContainsKey("p"))
                            foreach (Guid nid in _edgesReversed[node.Id]["p"])
                            {
                                INode n = GetNode(nid);
                                if (!retVal.Contains(n))
                                    retVal.Add(n);
                            }
                        return retVal;
                    }
                    else
                    {
                        return retVal;
                    }

                default:
                    return new List<INode>();
            }
        }

        /// <summary>
        /// Fetch the edge label ('p' for products, 's' for substrates) that spans between the from node/molecule and to edge/process or from edge/process and to node/molecule.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <param name="edgeType"></param>
        /// <returns></returns>
        public string GetEdgeLabel(Guid fromNode, Guid toNode, EdgeType edgeType)
        {
            switch(edgeType)
            {
                case EdgeType.Directed:
                    if(!_edges.ContainsKey(fromNode))
                        throw new ArgumentOutOfRangeException("fromNode");

                    foreach(string edgeLabel in _edges[fromNode].Keys)
                    {
                        if(edgeLabel.Equals(""))
                            continue;
                        if(_edges[fromNode][edgeLabel].Contains(toNode))
                            return edgeLabel;
                    }

                    break;
                case EdgeType.DirectedReversed:
                    if(!_edgesReversed.ContainsKey(fromNode))
                        throw new ArgumentOutOfRangeException("fromNode");

                    foreach(string edgeLabel in _edgesReversed[fromNode].Keys)
                    {
                        if(edgeLabel.Equals(""))
                            continue;
                        if(_edgesReversed[fromNode][edgeLabel].Contains(toNode))
                            return edgeLabel;
                    }

                    break;
                case EdgeType.Undirected:
                    if(!_edges.ContainsKey(fromNode) && !_edgesReversed.ContainsKey(fromNode))
                        throw new ArgumentOutOfRangeException("fromNode");

                    if(_edges.ContainsKey(fromNode))
                    {
                        foreach(string edgeLabel in _edges[fromNode].Keys)
                        {
                            if(edgeLabel.Equals(""))
                                continue;
                            if(_edges[fromNode][edgeLabel].Contains(toNode))
                                return edgeLabel;
                        }
                    }

                    if(_edgesReversed.ContainsKey(fromNode))
                    {
                        foreach(string edgeLabel in _edgesReversed[fromNode].Keys)
                        {
                            if(edgeLabel.Equals(""))
                                continue;
                            if(_edgesReversed[fromNode][edgeLabel].Contains(toNode))
                                return edgeLabel;
                        }
                    }

                    break;

                default:
                    return "";
            }

            return "";
        }

        /// <summary>
        /// Clone the graph, its nodes, and its index structures
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            GraphAdjacencyList obj = new GraphAdjacencyList();

            foreach(INode vertex in this._vertices.Values)
                obj._vertices.Add(vertex.Id, (INode) vertex.Clone());

            foreach(KeyValuePair<Guid, Dictionary<string, List<Guid>>> kvp in this._edges)
            {
                Guid fromNode = kvp.Key;
                obj._edges[fromNode] = new Dictionary<string, List<Guid>>();

                foreach(KeyValuePair<string, List<Guid>> kvp2 in kvp.Value)
                {
                    string edgeLabel = (string) kvp2.Key.Clone();
                    obj._edges[fromNode][edgeLabel] = new List<Guid>();

                    foreach(Guid n in kvp2.Value)
                        obj._edges[fromNode][edgeLabel].Add(n);
                }
            }

            foreach (KeyValuePair<Guid, Dictionary<string, List<Guid>>> kvp in this._edgesReversed)
            {
                Guid fromNode = kvp.Key;
                obj._edgesReversed[fromNode] = new Dictionary<string, List<Guid>>();

                foreach (KeyValuePair<string, List<Guid>> kvp2 in kvp.Value)
                {
                    string edgeLabel = (string) kvp2.Key.Clone();
                    obj._edgesReversed[fromNode][edgeLabel] = new List<Guid>();

                    foreach (Guid n in kvp2.Value)
                        obj._edgesReversed[fromNode][edgeLabel].Add(n);
                }
            }

            return obj;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Graph, Adj List --- ");

            sb.Append(Vertices.Count);
            sb.Append(", ");
            sb.Append(Edges.Count);

            //sb.Append("(");

            //List<string> nodeStrings = new List<string>();
            //foreach(INode n in Vertices)
            //    nodeStrings.Add(n.ToString());
            //sb.Append(String.Format("{{{0}}}", String.Join(", ", nodeStrings.ToArray())));

            //sb.Append("; ");

            //List<string> edgeStrings = new List<string>();
            //foreach(IEdge e in Edges)
            //    edgeStrings.Add(e.ToString());
            //sb.Append(String.Format("{{{0}}}", String.Join(", ", edgeStrings.ToArray())));

            //sb.Append(")");

            return sb.ToString();
        }
    }
}