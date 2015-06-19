using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// Encapsulate the desired edge type that a user would like to use in the query.
    /// The user can either search the graph in an undirected, directed (downstream), or reverse directed (upstream) manner.
    /// </summary>
    [SerializableAttribute]
    public enum EdgeType { Undirected, Directed, DirectedReversed }

    public interface IGraph : ICloneable
    {
        List<INode> Vertices
        {
            get;
        }

        List<IEdge> Edges
        {
            get;
        }

        void AddVertex(INode node);
        void AddEdge(IEdge edge);

        List<INode> AdjacentNodes(INode node, EdgeType edgeType);
        List<INode> AdjacentNodes(INode node, EdgeType edgeType, string previousEdgeLabel);
        string GetEdgeLabel(Guid fromNode, Guid toNode, EdgeType edgeType);

        bool ContainsNode(Guid nodeId);

        INode GetNode(Guid nodeId);
        List<INode> GetNodes(List<Guid> nodeIds);

        string GetNodeLabel(Guid nodeId);

        //object Clone();
    }
}