using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A container for the results of the simple version of the path query
    /// </summary>
    [SerializableAttribute]
    public class QueryPathResultsSimple : QueryPathResults
    {
        private Dictionary<List<INode>, int> _paths;
        private INode _startNode;

        public override List<List<INode>> Paths
        {
            get { return new List<List<INode>>(_paths.Keys); }
        }

        public override INode StartNode
        {
            get { return _startNode; }
        }

        private QueryPathResultsSimple()
            : base()
        { }

        public QueryPathResultsSimple(QueryPathParameters parameters, INode startNode, List<List<INode>> paths, bool limitReached, bool timeoutReached)
            : base(parameters, limitReached, timeoutReached)
        {
            _startNode = startNode;
            _paths = new Dictionary<List<INode>, int>();
            foreach(List<INode> path in paths)
                _paths.Add(path, Length(path));
        }

        public override int Length(List<INode> path)
        {
            if(_paths.ContainsKey(path))
                return _paths[path];

            int retVal = 0;
            INode fromNode = path[0];

            // The length of a path is equivalent to the number of times the first node's type appears in the path (not including the first node)
            for(int i = 1; i < path.Count; i++)
                if(fromNode.Type == path[i].Type)
                    retVal++;

            return retVal;
        }
    }
}