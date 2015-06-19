using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A generic container for the path query results
    /// </summary>
    [SerializableAttribute]
    public abstract class QueryPathResults : IQueryResults
    {
        public abstract INode StartNode
        {
            get;
        }

        public abstract List<List<INode>> Paths
        {
            get;
        }

        public abstract int Length(List<INode> path);

        private QueryPathParameters _parameters;
        private bool _limitReached;
        private bool _timeoutReached;
        private Dictionary<NodeType, List<INode>> _uniqueNodes;

        public QueryPathParameters Parameters
        {
            get { return _parameters; }
        }

        public bool LimitReached
        {
            get { return _limitReached; }
        }

        public bool TimeoutReached
        {
            get { return _timeoutReached; }
        }

        /// <summary>
        /// Do we display the graph to the user?
        /// </summary>
        public bool DisplayGraph
        {
            get
            {
                List<INode> uniqueNodes = UniqueNodes(NodeType.Node);
                List<INode> uniqueEdges = UniqueNodes(NodeType.Edge);
                int uniqueCount = uniqueNodes.Count + uniqueEdges.Count;

                return    (0 < uniqueCount && uniqueCount <= _parameters.MaxGraphLimit)
                       && (uniqueNodes.Count > 0)
                       && (uniqueEdges.Count > 0);
            }
        }

        public string HiddenGraphText
        {
            get
            {
                List<INode> uniqueNodes = UniqueNodes(NodeType.Node);
                List<INode> uniqueEdges = UniqueNodes(NodeType.Edge);
                int uniqueCount = uniqueNodes.Count + uniqueEdges.Count;

                if(!(0 < uniqueCount && uniqueCount <= _parameters.MaxGraphLimit) && !(uniqueNodes.Count > 0 && uniqueEdges.Count > 0))
                    return String.Format("The graph will only be shown if there are {0} unique nodes or fewer and the results contain at least one of each type of entity.", _parameters.MaxGraphLimit);
                else if(!(0 < uniqueCount && uniqueCount <= _parameters.MaxGraphLimit) && (uniqueNodes.Count > 0 && uniqueEdges.Count > 0))
                    return String.Format("The graph will only be shown if there are {0} unique nodes or fewer.", _parameters.MaxGraphLimit);
                else if((0 < uniqueCount && uniqueCount <= _parameters.MaxGraphLimit) && !(uniqueNodes.Count > 0 && uniqueEdges.Count > 0))
                    return "The graph will only be shown if the results contain at least one of each type of entity.";
                else
                    return "";
            }
        }

        protected QueryPathResults()
        {
            _uniqueNodes = new Dictionary<NodeType, List<INode>>();
        }

        public QueryPathResults(QueryPathParameters parameters, bool limitReached, bool timeoutReached)
        {
            _parameters = parameters;
            _limitReached = limitReached;
            _timeoutReached = timeoutReached;
            _uniqueNodes = new Dictionary<NodeType, List<INode>>();
        }

        /// <summary>
        /// Get the smallest and largest path length that were found in the results. This is used by the interface.
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        public void Lengths(out int minLength, out int maxLength)
        {
            minLength = int.MaxValue;
            maxLength = int.MinValue;

            foreach(List<INode> path in Paths)
            {
                int legnth = Length(path);
                minLength = Math.Min(minLength, legnth);
                maxLength = Math.Max(maxLength, legnth);
            }

            return;
        }

        public List<List<INode>> GetPaths(int length)
        {
            Dictionary<List<INode>, bool> retVal = new Dictionary<List<INode>, bool>();

            foreach(List<INode> path in Paths)
                if(Length(path) == length)
                    retVal.Add(path, true);

            return new List<List<INode>>(retVal.Keys);
        }

        /// <summary>
        /// Get a list of the nodes of a particular node type. The list contains only one unique copy of each node for the particular node type.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public List<INode> UniqueNodes(NodeType nodeType)
        {
            if(!_uniqueNodes.ContainsKey(nodeType))
            {
                Dictionary<INode, bool> retVal = new Dictionary<INode, bool>(new EqualityComparerINode());

                foreach(List<INode> path in Paths)
                    foreach(INode pathNode in path)
                        if(pathNode.Type == nodeType)
                            if(!retVal.ContainsKey(pathNode))
                                retVal.Add(pathNode, true);

                _uniqueNodes.Add(nodeType, new List<INode>(retVal.Keys));
            }

            return _uniqueNodes[nodeType];
        }
    }
}