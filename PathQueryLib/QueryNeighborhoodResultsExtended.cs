using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    // *** VERY BROKEN *** //
    // *** WILL NOT BUILD; NOT CURRENTLY INCLUDED IN BUILD *** //
    // *** NEEDS UPDATES FROM IQUERY AND POTENTIALLY OTHERS *** //
    [SerializableAttribute]
    public class QueryNeighborhoodResultsExtended : QueryNeighborhoodResults
    {
        public static int GetNodesCount(Dictionary<Dictionary<INode, bool>, bool> results)
        {
            Dictionary<INode, bool> nodes = new Dictionary<INode, bool>(new EqualityComparerINode());

            foreach(Dictionary<INode, bool> pathDict in results.Keys)
            {
                List<INode> path = new List<INode>(pathDict.Keys);
                nodes[path[path.Count - 1]] = true;
            }

            return nodes.Keys.Count;
        }

        private Dictionary<INode, int> _nodes;
        private List<List<INode>> _paths;

        public override List<INode> Nodes
        {
            get { return new List<INode>(_nodes.Keys); }
        }

        private QueryNeighborhoodResultsExtended()
            : base()
        { }

        public QueryNeighborhoodResultsExtended(QueryNeighborhoodParameters parameters, List<List<INode>> paths, bool limitReached, bool timeoutReached)
            : base(parameters, limitReached, timeoutReached)
        {
            _nodes = new Dictionary<INode, int>(new EqualityComparerINode());
            _paths = paths;

            foreach(List<INode> path in _paths)
                if(!_nodes.ContainsKey(path[path.Count - 1]))
                    _nodes[path[path.Count - 1]] = ShortestDistance(path[path.Count - 1]);
        }

        public override int Distance(INode node)
        {
            if(_nodes.ContainsKey(node))
                return _nodes[node];
            else
                throw new ArgumentOutOfRangeException(String.Format("'{0}' not contained in these results.", node.ToString()));
        }

        private int ShortestDistance(INode node)
        {
            int retVal = int.MaxValue;

            foreach(List<INode> path in _paths)
            {
                if(node.Equals(path[path.Count - 1]))
                {
                    int length = PathLength(path);

                    if(length < retVal)
                        retVal = length;
                }
            }

            return retVal;
        }

        private int PathLength(List<INode> path)
        {
            int retVal = 0;
            INode startNode = path[0];

            for(int i = 1; i < path.Count; i++)
                if(startNode.Type == path[i].Type)
                    retVal++;

            return retVal;
        }
    }
}