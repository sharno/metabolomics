using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// Encapsulates the type of node for the modified graph; either a node on the original graph or an edge on the original graph
    /// </summary>
    public enum NodeType
    {
        Node,
        Edge,
        Any //BE: only used for querying!
    }

    /// <summary>
    /// An interface for the nodes of the graph
    /// </summary>
    public interface INode : ICloneable
    {
        Guid Id
        {
            get;
        }

        NodeType Type
        {
            get;
        }

        string Label
        {
            get;
        }

        //object Clone();
    }
}