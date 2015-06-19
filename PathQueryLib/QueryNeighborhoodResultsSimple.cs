using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A container for the results of the simple neighborhood query
    /// </summary>
    [SerializableAttribute]
    public class QueryNeighborhoodResultsSimple : QueryNeighborhoodResults
    {
        private INode _startNode;
        private Dictionary<INode, int> _nodes; // An index mapping each node to its closest distance to the 'from node'
        
        private QueryNeighborhoodResultsSimple()
            : base()
        { }

        public QueryNeighborhoodResultsSimple(QueryNeighborhoodParameters parameters, INode startNode, Dictionary<INode, int> nodes, bool limitReached, bool timeoutReached)
            : base(parameters, limitReached, timeoutReached)
        {
            _startNode = startNode;
            _nodes = nodes;
        }

        public override List<INode> Nodes
        {
            get { return new List<INode>(_nodes.Keys); }
        }

        public override INode StartNode
        {
            get { return _startNode; }
        }

        public override int Distance(INode node)
        {
            if(_nodes.ContainsKey(node))
                return _nodes[node];
            else
                throw new ArgumentOutOfRangeException(String.Format("'{0}' not contained in these results.", node.ToString()));
        }
    }
}