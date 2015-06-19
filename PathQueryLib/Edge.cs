using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// This is a basic generic implementation of the IEdge interface
    /// </summary>
    [SerializableAttribute]
    public class Edge : IEdge
    {
        private INode _fromNode;
        private INode _toNode;
        private string _label;

        public INode FromNode
        {
            get { return _fromNode; }
        }

        public INode ToNode
        {
            get { return _toNode; }
        }

        public string Label
        {
            get { return _label; }
        }

        private Edge()
        { }

        public Edge(INode fromNode, INode toNode)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _label = "";
        }

        public Edge(INode fromNode, INode toNode, string label)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _label = label;
        }

        /// <summary>
        /// Creates a new edge objects; with cloned from and to nodes
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Edge((INode) this._fromNode.Clone(), (INode) this._toNode.Clone());
        }

        public override string ToString()
        {
            return String.Format("({0},{1} : {2})", _fromNode.ToString(), _toNode.ToString(), _label);
        }
    }
}