using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A generic container for the results of the neighborhood query
    /// </summary>
    [SerializableAttribute]
    public abstract class QueryNeighborhoodResults : IQueryResults
    {
        public abstract INode StartNode
        {
            get;
        }

        public abstract List<INode> Nodes
        {
            get;
        }

        public abstract int Distance(INode node);

        private QueryNeighborhoodParameters _parameters;
        private bool _limitReached;
        private bool _timeoutReached;

        public QueryNeighborhoodParameters Parameters
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

        public bool DisplayGraph
        {
            get
            {
                bool hasMolecules = false;
                bool hasProcesses = false;

                foreach(INode node in Nodes)
                {
                    hasMolecules = hasMolecules || node.Type == NodeType.Node;
                    hasProcesses = hasProcesses || node.Type == NodeType.Edge;
                }

                return (0 < Nodes.Count && Nodes.Count <= _parameters.MaxGraphLimit && hasMolecules && hasProcesses);
            }
        }

        public string HiddenGraphText
        {
            get
            {
                bool hasMolecules = false;
                bool hasProcesses = false;

                foreach(INode node in Nodes)
                {
                    hasMolecules = hasMolecules || node.Type == NodeType.Node;
                    hasProcesses = hasProcesses || node.Type == NodeType.Edge;
                }

                if(!(0 < Nodes.Count && Nodes.Count <= _parameters.MaxGraphLimit) && !(hasMolecules && hasProcesses))
                    return String.Format("The graph will only be shown if there are {0} results or fewer and the results contain at least one of each type of entity.", _parameters.MaxGraphLimit);
                else if(!(0 < Nodes.Count && Nodes.Count <= _parameters.MaxGraphLimit) && hasMolecules && hasProcesses)
                    return String.Format("The graph will only be shown if there are {0} results or fewer.", _parameters.MaxGraphLimit);
                else if(0 < Nodes.Count && Nodes.Count <= _parameters.MaxGraphLimit && !(hasMolecules && hasProcesses))
                    return "The graph will only be shown if the results contain at least one of each type of entity.";
                else
                    return "";
            }
        }

        protected QueryNeighborhoodResults()
        { }

        public QueryNeighborhoodResults(QueryNeighborhoodParameters parameters, bool limitReached, bool timeoutReached)
        {
            _parameters = parameters;
            _limitReached = limitReached;
            _timeoutReached = timeoutReached;
        }

        /// <summary>
        /// A helper function to determine the shortest and longest distance that the nodes occur away from the 'from node'
        /// </summary>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        public void Distances(out int minDistance, out int maxDistance)
        {
            minDistance = int.MaxValue;
            maxDistance = int.MinValue;

            foreach(INode node in Nodes)
            {
                int distance = Distance(node);
                minDistance = Math.Min(minDistance, distance);
                maxDistance = Math.Max(maxDistance, distance);
            }

            return;
        }

        /// <summary>
        /// A helper function to extract the nodes from the results that are of a particular node type and a particular distance away from the 'from node'.
        /// These nodes are sorted according their labels.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public List<INode> GetNodes(NodeType type, int distance)
        {
            SortedDictionary<string, INode> retVal = new SortedDictionary<string, INode>();

            foreach(INode node in Nodes)
                if(   node.Type == type
                   && Distance(node) == distance
                   && !retVal.ContainsKey(String.Format("{0}-{1}", node.Label, node.Id)))
                    retVal.Add(String.Format("{0}-{1}", node.Label, node.Id), node);

            return new List<INode>(retVal.Values);
        }
    }
}