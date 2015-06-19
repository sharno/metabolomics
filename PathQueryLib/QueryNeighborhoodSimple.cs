using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A simple naive implementation of the neighborhood query using a breadth-first search
    /// </summary>
    [SerializableAttribute]
    public class QueryNeighborhoodSimple : IQuery
    {
        //private EventLog _eventLog;

        public QueryNeighborhoodSimple()
        {
            //_eventLog = new EventLog("Application", ".", "PathQueryLib");
        }

        public QueryNeighborhoodSimple(QueryNeighborhoodSimple q)
        {
            //_eventLog = new EventLog(q._eventLog.Log, q._eventLog.MachineName, q._eventLog.Source);
        }

        public IQueryResults Execute(IGraph graph, EdgeType edgeType, IQueryParameters parameters)
        {
            // verify satisfiability
            try
            {
                parameters.VerifySatisfiability(graph);
            }
            catch (SatisfiabilityException e)
            {
                return new QueryFailureResult(e.Message);
            }

            QueryNeighborhoodParameters neighborhoodParameters = (QueryNeighborhoodParameters) parameters;

            if(neighborhoodParameters.IncludedNodes.Count != 0)
                throw new ArgumentOutOfRangeException("parameters", "The simple version of the neighborhood query does not support the 'included' nodes. Therefore, this array must be empty.");

            bool limitReached;
            bool timeoutReached;
            Dictionary<INode, int> results;
            INode startNode = graph.GetNode(neighborhoodParameters.StartNodeId);

            if (startNode != null)
            {
                results = BFS(ref graph, edgeType, neighborhoodParameters, out limitReached, out timeoutReached);
            }
            else
            {
                results = new Dictionary<INode, int>();
                limitReached = false;
                timeoutReached = false;
            }

            return new QueryNeighborhoodResultsSimple(neighborhoodParameters, startNode, results, limitReached, timeoutReached);
        }

        private Dictionary<INode, int> BFS(ref IGraph graph, EdgeType edgeType, QueryNeighborhoodParameters parameters, out bool limitReached, out bool timeoutReached)
        {
            DateTime startTime = DateTime.Now;
            Dictionary<INode, bool> visited = new Dictionary<INode, bool>(new EqualityComparerINode());
            Dictionary<INode, int> results = new Dictionary<INode, int>(new EqualityComparerINode());

            int minLength = parameters.MinLength;
            int maxLength = parameters.MaxLength;
            INode startNode = graph.GetNode(parameters.StartNodeId);
            List<INode> excludedNodes = graph.GetNodes(parameters.ExcludedNodes);
            NodeType findType = ((QueryNeighborhoodParameters)parameters).FindType;

            limitReached = false;
            timeoutReached = false;

            try
            {
                Queue<INode> nodes = new Queue<INode>();
                Queue<int> distances = new Queue<int>();
                //Queue<string> edgeLabels = new Queue<string>();

                nodes.Enqueue(startNode);
                distances.Enqueue(0);
                //edgeLabels.Enqueue("");

                while(nodes.Count > 0)
                {
                    INode currentNode = nodes.Dequeue();
                    int currentDistance = distances.Dequeue();
                    //string currentEdgeLabel = edgeLabels.Dequeue();

                    // We're visiting this node now!
                    visited.Add(currentNode, true);

                    // If the current node's path length falls within the accepted boundaries,
                    // add it to the results list.
                    if(minLength <= currentDistance && currentDistance <= maxLength)
                    {
                        // Check for a top-k limit and return if reached or surpassed
                        if(results.Keys.Count >= parameters.MaxResultLimit)
                            throw new QueryReachedLimitException();

                        // Add the result
                        results.Add(currentNode, currentDistance);
                    }

                    // Run a BFS on all of this node's neighbors
                    List<INode> adjacentNodes = graph.AdjacentNodes(currentNode, edgeType);
                    foreach(INode adjacentNode in adjacentNodes)
                    {
                        // The distance increases by one every time a node of the same type as the start node is encountered
                        int adjacentNodeDistance = (startNode.Type == adjacentNode.Type) ? currentDistance + 1 : currentDistance;

                        //BE: Find type parameter can override the inclusion of the opposite type when
                        // we reach the last row (max distance).  Technically the distance
                        // to these connected nodes is distance=0 (so they can be included),
                        // but in practice we may not want these in the graph (i.e. when implementing find
                        // molecules N steps from a molecule built-in query).
                        if (findType == NodeType.Any && adjacentNodeDistance == maxLength &&
                            startNode.Type != adjacentNode.Type && findType != adjacentNode.Type)
                            continue;

                        // If the distance of this node is larger than the max length, neither it nor any of its adjacent neighbors that have yet to be discovered can be reuslts. Truncate the BFS of this branch.
                        if(adjacentNodeDistance > maxLength)
                            continue;

                        // If we have already visited this node (or have it queued), don't visit it again.
                        if(visited.ContainsKey(adjacentNode) || nodes.Contains(adjacentNode))
                            continue;

                        // Do not include this node in the search if we aren't allowed.
                        if(excludedNodes.Contains(adjacentNode))
                            continue;

                        // Search this branch
                        nodes.Enqueue(adjacentNode);
                        distances.Enqueue(adjacentNodeDistance);
                        //edgeLabels.Enqueue(graph.GetEdgeLabel(currentNode, adjacentNode));
                    }

                    // Have we timed out?
                    if(DateTime.Now.Subtract(startTime).TotalSeconds > parameters.TimeoutLimit)
                        throw new TimeoutException();
                }
            }
            catch(QueryReachedLimitException e)
            {
                limitReached = true;
            }
            catch(TimeoutException e)
            {
                timeoutReached = true;
            }

            return results;
        }

        public object Clone()
        {
            return new QueryNeighborhoodSimple(this);
        }

        public override string ToString()
        {
            return "Neighborhood (Simple)";
        }
    }
}