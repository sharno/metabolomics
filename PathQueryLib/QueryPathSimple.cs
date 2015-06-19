using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A simple naive implementation of the path query using depth-first search
    /// </summary>
    [SerializableAttribute]
    public class QueryPathSimple : IQuery
    {
        public QueryPathSimple()
        { }

        public QueryPathSimple(QueryPathSimple q)
        { }

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

            QueryPathParameters pathParameters = (QueryPathParameters) parameters;

            bool limitReached;
            bool timeoutReached;
            List<List<INode>> results;
            INode startNode = graph.GetNode(pathParameters.FromNodeId);

            if (startNode != null)
            {
                results = DFS(ref graph, edgeType, pathParameters, out limitReached, out timeoutReached);
            }
            else
            {
                results = new List<List<INode>>();
                limitReached = false;
                timeoutReached = false;
            }

            return new QueryPathResultsSimple(pathParameters, startNode, results, limitReached, timeoutReached);
        }

        public List<List<INode>> DFS(ref IGraph graph, EdgeType edgeType, QueryPathParameters parameters, out bool limitReached, out bool timeoutReached)
        {
            List<List<List<INode>>> hops = new List<List<List<INode>>>();

            // Loop through the hops and perform separate DFSes
            // Then, perform a cross-product to join the different hops together
            // Lastly, run a check for the entire path w/ those restrictions

            limitReached = false;
            timeoutReached = false;
            DateTime timeoutDt = DateTime.Now;
            try
            {
                INode fromNode = graph.GetNode(parameters.FromNodeId);
                foreach(QueryPathParametersToNode toNodeParameters in parameters.ToNodes)
                {
                    List<List<INode>> hop = new List<List<INode>>();

                    Dictionary<INode, bool> path = new Dictionary<INode, bool>(new EqualityComparerINode());
                    path.Add(fromNode, true);
                    DFS(ref graph, edgeType, Math.Max(parameters.MinLength, toNodeParameters.MinLength), Math.Min(parameters.MaxLength, toNodeParameters.MaxLength), fromNode, graph.GetNode(toNodeParameters.NodeId), graph.GetNodes(toNodeParameters.IncludedNodes), graph.GetNodes(toNodeParameters.ExcludedNodes), fromNode, 0, "", ref path, timeoutDt, parameters.TimeoutLimit, ref hop);

                    hops.Add(hop);

                    fromNode = graph.GetNode(toNodeParameters.NodeId);
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

            // Don't return any partial paths or incomplete paths if the algorithm is stopped before it is finished
            if(limitReached || timeoutReached)
                return new List<List<INode>>();

            List<List<INode>> retVal = hops[0];
            try
            {
                for(int i = 1; i < hops.Count; i++)
                {
                    List<List<INode>> paths = new List<List<INode>>();

                    foreach(List<INode> path in retVal)
                    {
                        foreach(List<INode> pathI in hops[i])
                        {
                            List<INode> tempPath = new List<INode>();
                            tempPath.AddRange(path);
                            tempPath.RemoveAt(tempPath.Count - 1); // Remove last of path since path.Last and pathI.First are equal
                            tempPath.AddRange(pathI);

                            paths.Add(tempPath);
                        }
                    }

                    // Have we timed out?
                    if(DateTime.Now.Subtract(timeoutDt).TotalSeconds > parameters.TimeoutLimit)
                        throw new TimeoutException();

                    retVal = paths;
                }
            }
            catch(TimeoutException e)
            {
                timeoutReached = true;
            }

            if(timeoutReached)
                return new List<List<INode>>();

            for(int i = 0; i < retVal.Count; i++)
            {
                try
                {
                    // Have we timed out?
                    if(DateTime.Now.Subtract(timeoutDt).TotalSeconds > parameters.TimeoutLimit)
                        throw new TimeoutException();

                    if(!(parameters.MinLength <= Length(retVal[i]) && Length(retVal[i]) <= parameters.MaxLength))
                        throw new Exception();

                    foreach(Guid includedNode in parameters.IncludedNodes)
                    {
                        bool hasThisNode = false;
                        foreach(INode pathNode in retVal[i])
                            if(includedNode.Equals(pathNode.Id))
                                hasThisNode = true;

                        if(!hasThisNode)
                            throw new Exception();
                    }

                    foreach(Guid excludedNode in parameters.ExcludedNodes)
                    {
                        bool doesNotHaveThisNode = true;
                        foreach(INode pathNode in retVal[i])
                            if(excludedNode.Equals(pathNode.Id))
                                doesNotHaveThisNode = false;

                        if(!doesNotHaveThisNode)
                            throw new Exception();
                    }
                }
                catch(TimeoutException e)
                {
                    timeoutReached = true;
                    break;
                }
                catch(Exception e)
                {
                    retVal.RemoveAt(i);
                    i -= 1;
                    continue;
                }
            }

            if(timeoutReached)
                return new List<List<INode>>();

            // Check for a top-k limit and return if reached or surpassed
            if(retVal.Count >= parameters.MaxResultLimit)
                limitReached = true;

            return retVal;
        }

        public void DFS(ref IGraph graph, EdgeType edgeType, int minLength, int maxLength, INode fromNode, INode toNode, List<INode> includedNodes, List<INode> excludedNodes, INode currentNode, int currentPathLength, string currentEdgeLabel, ref Dictionary<INode, bool> path, DateTime startTime, int timeoutLimit, ref List<List<INode>> results)
        {
            // If the current node's path length falls within the accepted boundaries,
            // and if the path includes all of the necessary nodes,
            // and if the path does not include all of the necessary nodes,
            // add it to the results list.
            bool lengthRestriction = minLength <= currentPathLength && currentPathLength <= maxLength;

            bool inclusionRestriction = true;
            foreach(INode includedNode in includedNodes)
                if(!path.ContainsKey(includedNode))
                    inclusionRestriction = false;

            bool exclusionRestriction = true;
            foreach(INode excludedNode in excludedNodes)
                if(path.ContainsKey(excludedNode))
                    exclusionRestriction = false;

            // Add the result
            if(lengthRestriction && inclusionRestriction && exclusionRestriction && currentNode.Equals(toNode))
                results.Add(new List<INode>(path.Keys));

            // Run a DFS on all of this node's neighbors
            List<INode> adjacentNodes;
            if(path.Count == 1 || currentNode.Type == NodeType.Node)
                adjacentNodes = graph.AdjacentNodes(currentNode, edgeType);
            else
                adjacentNodes = graph.AdjacentNodes(currentNode, edgeType, currentEdgeLabel);
            foreach(INode adjacentNode in adjacentNodes)
            {
                // The path length increases by one every time a node of the same type as the start node is encountered
                int adjacentNodePathLength = (fromNode.Type == adjacentNode.Type) ? currentPathLength + 1 : currentPathLength;

                // If the path length of this node is larger than the max length, neither it nor any of its adjacent neighbors that have yet to be discovered can be reuslts. Truncate the DFS of this branch.
                if (adjacentNodePathLength > maxLength)
                    continue;

                // If we have already visited this node, don't visit it again.
                if(path.ContainsKey(adjacentNode))
                    continue;

                // Push the node on to the end of the path
                path.Add(adjacentNode, true);

                // Search this branch
                DFS(ref graph, edgeType, minLength, maxLength, fromNode, toNode, includedNodes, excludedNodes, adjacentNode, adjacentNodePathLength, graph.GetEdgeLabel(currentNode.Id, adjacentNode.Id, edgeType), ref path, startTime, timeoutLimit, ref results);

                // Pop the node off of the end of the path
                path.Remove(adjacentNode);
            }

            // Have we timed out?
            if(DateTime.Now.Subtract(startTime).TotalSeconds > timeoutLimit)
                throw new TimeoutException();

            return;
        }

        public int Length(List<INode> path)
        {
            int retVal = 0;
            INode fromNode = path[0];

            for(int i = 1; i < path.Count; i++)
                if(fromNode.Type == path[i].Type)
                    retVal++;

            return retVal;
        }

        public object Clone()
        {
            return new QueryPathSimple(this);
        }

        public override string ToString()
        {
            return "Path (Simple)";
        }
    }
}