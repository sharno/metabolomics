using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PathQueryLib
{
    // *** VERY BROKEN *** //
    // *** WILL NOT BUILD; NOT CURRENTLY INCLUDED IN BUILD *** //
    // *** NEEDS UPDATES FROM IQUERY AND POTENTIALLY OTHERS *** //
    [SerializableAttribute]
    public class QueryNeighborhoodExtended : IQuery
    {
        private EventLog _eventLog;

        public QueryNeighborhoodExtended()
        {
            _eventLog = new EventLog("Application", ".", "PathQueryLib");
        }

        public QueryNeighborhoodExtended(QueryNeighborhoodExtended q)
        {
            _eventLog = new EventLog(q._eventLog.Log, q._eventLog.MachineName, q._eventLog.Source);
        }

        public IQueryResults Execute(IGraph graph, IQueryParameters parameters)
        {
            QueryNeighborhoodParameters neighborhoodParameters = (QueryNeighborhoodParameters) parameters;

            bool limitReached;
            bool timeoutReached;
            List<List<INode>> results = DFS(ref graph, neighborhoodParameters, out limitReached, out timeoutReached);

            return new QueryNeighborhoodResultsExtended(neighborhoodParameters, results, limitReached, timeoutReached);
        }

        private List<List<INode>> DFS(ref IGraph graph, QueryNeighborhoodParameters parameters, out bool limitReached, out bool timeoutReached)
        {
            Dictionary<Dictionary<INode, bool>, bool> results = new Dictionary<Dictionary<INode, bool>, bool>();

            limitReached = false;
            timeoutReached = false;
            try
            {
                DFS(ref graph, parameters, parameters.StartNode, 0, new Dictionary<INode, bool>(new EqualityComparerINode()), DateTime.Now, ref results);
            }
            catch(QueryReachedLimitException e)
            {
                limitReached = true;
            }
            catch(TimeoutException e)
            {
                timeoutReached = true;
            }

            List<List<INode>> retVal = new List<List<INode>>();
            foreach(Dictionary<INode, bool> path in results.Keys)
                retVal.Add(new List<INode>(path.Keys));

            return retVal;
        }

        private void DFS(ref IGraph graph, QueryNeighborhoodParameters parameters, INode currentNode, int currentPathLength, Dictionary<INode, bool> path, DateTime startTime, ref Dictionary<Dictionary<INode, bool>, bool> results)
        {
            int minLength = ((QueryNeighborhoodParameters) parameters).MinLength;
            int maxLength = ((QueryNeighborhoodParameters) parameters).MaxLength;
            INode startNode = ((QueryNeighborhoodParameters) parameters).StartNode;
            INode[] includedNodes = ((QueryNeighborhoodParameters) parameters).IncludedNodes;
            INode[] notIncludedNodes = ((QueryNeighborhoodParameters) parameters).NotIncludedNodes;

            // Add the current node to the path list.
            path.Add(currentNode, true);

            // If the current node's path length falls within the accepted boundaries,
            // and if the path includes all of the necessary nodes,
            // and if the path does not include all of the necessary nodes,
            // add it to the results list.
            bool lengthRestriction = minLength <= currentPathLength && currentPathLength <= maxLength;

            bool inclusionRestriction = true;
            foreach(INode includedNode in includedNodes)
                if(!path.ContainsKey(includedNode))
                    inclusionRestriction = false;

            bool nonInclusionRestriction = true;
            foreach(INode notIncludedNode in notIncludedNodes)
                if(path.ContainsKey(notIncludedNode))
                    nonInclusionRestriction = false;

            if(lengthRestriction && inclusionRestriction && nonInclusionRestriction)
            {
                // Check for a top-k limit and return if reached or surpassed
                if(QueryNeighborhoodResultsExtended.GetNodesCount(results) >= parameters.MaxResultLimit)
                    throw new QueryReachedLimitException();

                // Add the result
                results.Add(path, true);
            }

            // Run a DFS on all of this node's neighbors
            List<INode> adjacentNodes = graph.AdjacentNodes(currentNode);
            foreach(INode adjacentNode in adjacentNodes)
            {
                // The path length increases by one every time a node of the same type as the start node is encountered
                int adjacentNodePathLength = (startNode.Type == adjacentNode.Type) ? currentPathLength + 1 : currentPathLength;

                // If the path length of this node is larger than the max length, neither it nor any of its adjacent neighbors that have yet to be discovered can be reuslts. Truncate the DFS of this branch.
                if(adjacentNodePathLength > maxLength)
                    continue;

                // If we have already visited this node, don't visit it again.
                if(path.ContainsKey(adjacentNode))
                    continue;

                // Search this branch
                DFS(ref graph, parameters, adjacentNode, adjacentNodePathLength, new Dictionary<INode, bool>(path, new EqualityComparerINode()), startTime, ref results);
            }

            // Have we timed out?
            if(DateTime.Now.Subtract(startTime).TotalSeconds > parameters.TimeoutLimit)
                throw new TimeoutException();

            return;
        }

        public object Clone()
        {
            return new QueryNeighborhoodExtended(this);
        }

        public override string ToString()
        {
            return "Neighborhood (Extended)";
        }
    }
}