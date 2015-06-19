using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    // *** COMPLETELY AND UTTERLY BROKEN *** //
    // *** WILL NOT BUILD; NOT CURRENTLY INCLUDED IN BUILD *** //
    // *** NEEDS UPDATING TO THE CURRENT IGRAPH, INODE, AND IEDGE IMPLEMENTATIONS *** //
    // *** NEEDS REWRITING *** //
    [SerializableAttribute]
    public class GraphAdjacencyMatrix : IGraph
    {
        private List<INode> _vertices;
        private Dictionary<INode, Dictionary<INode, bool>> _edges;

        public List<INode> Vertices
        {
            get { return _vertices; }
        }

        public GraphAdjacencyMatrix()
        {
            _vertices = new List<INode>();
            _edges = new Dictionary<INode, Dictionary<INode, bool>>();
        }

        public void AddVertex(INode node)
        {
            if(!_vertices.Contains(node))
                _vertices.Add(node);

            // Set up another row/col in the matrix
            _edges[node] = new Dictionary<INode, bool>();
            foreach(INode v in _vertices)
                _edges[node][v] = (node == v);
            foreach(INode v in _vertices)
                _edges[v][node] = (node == v);
        }

        public void AddEdge(int id, INode fromNode, INode toNode)
        {
            if(_vertices.Contains(fromNode) && _vertices.Contains(toNode))
                _edges[fromNode][toNode] = true;
        }

        public List<INode> AdajcentNodes(INode node)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            GraphAdjacencyMatrix obj = new GraphAdjacencyMatrix();

            foreach(INode vertex in this._vertices)
                obj._vertices.Add((INode) vertex.Clone());

            foreach(KeyValuePair<INode, Dictionary<INode, bool>> kvp in this._edges)
            {
                INode fromNode = (INode) kvp.Key.Clone();
                obj._edges[fromNode] = new Dictionary<INode,bool>();

                foreach(KeyValuePair<INode, bool> kvp2 in kvp.Value)
                {
                    INode toNode = (INode) kvp2.Key.Clone();
                    obj._edges[fromNode][toNode] = kvp2.Value;
                }
            }

            return obj;
        }
    }
}