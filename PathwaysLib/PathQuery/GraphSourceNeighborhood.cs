using PathQueryLib;
using PathwaysLib.GraphSources;
using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PathwaysLib.PathQuery
{
    /// <summary>
    /// A base class for the GraphSource implementation for all of the neighborhood based queries
    /// </summary>
    public abstract class GraphSourceNeighborhood : GraphSourcePathQueries
    {
        protected QueryNeighborhoodResults _neighborhood;

        protected GraphSourceNeighborhood()
            : base()
        { }

        public GraphSourceNeighborhood(QueryNeighborhoodResults neighborhood)
            : base()
        {
            _neighborhood = neighborhood;
        }

        public override Guid SourceNode
        {
            get { return _neighborhood.Parameters.StartNodeId; }
        }

        public override string GraphType
        {
            get { return "Neighborhood"; }
        }

        public override string GraphTitle
        {
            get { return "Neighborhood Graph"; }
        }

        /// <summary>
        /// Provides colorings based on the involvement of each node in the results and the distance from the 'from node'
        /// </summary>
        public override GraphColoring[] Colorings
        {
            get
            {
                Guid startNodeId = _neighborhood.Parameters.StartNodeId;

                List<GraphColoring> retVal = new List<GraphColoring>();
                retVal.Add(new GraphColoring(startNodeId, Color.OrangeRed, "Starting Entity"));

                int minDistance;
                int maxDistance;
                _neighborhood.Distances(out minDistance, out maxDistance);

                // For each step...
                for(int i = minDistance; i <= maxDistance; i++)
                {
                    // Get the nodes and edges for this step, removing the starting node if need be
                    Dictionary<Guid, bool> nodes = new Dictionary<Guid, bool>(); // (new EqualityComparerINode());
                    List<INode> nodes1 = _neighborhood.GetNodes(NodeType.Node, i);
                    List<INode> nodes2 = _neighborhood.GetNodes(NodeType.Edge, i);
                    foreach(INode node in nodes1)
                        nodes[node.Id] = true;
                    foreach(INode node in nodes2)
                        nodes[node.Id] = true;
                    if(nodes.ContainsKey(startNodeId))
                        nodes.Remove(startNodeId);

                    // Which color are we using?
                    int j = (i - minDistance) % 4;
                    Color c = j == 0 ? Color.DarkOrange :
                              j == 1 ? Color.Chartreuse :
                              j == 2 ? Color.DodgerBlue :
                              j == 3 ? Color.Orchid : Color.Black;

                    // Construct the string to use with this color (include all steps for this color)
                    List<string> stepStrings = new List<string>();
                    string lastStepString;
                    string stepString;
                    for(int k = minDistance + j; k <= maxDistance; k += 4)
                    {
                        List<INode> nodesTemp = new List<INode>();
                        nodesTemp.AddRange(_neighborhood.GetNodes(NodeType.Node, i));
                        nodesTemp.AddRange(_neighborhood.GetNodes(NodeType.Edge, i));

                        if(nodesTemp.Count > 0)
                            stepStrings.Add(k.ToString());
                    }
                    lastStepString = stepStrings[stepStrings.Count - 1];
                    stepStrings.Remove(lastStepString);
                    stepString = String.Format("{0}{1}{2}{3}",
                                               String.Join(", ", stepStrings.ToArray()),
                                               stepStrings.Count > 1 ? "," : "",
                                               stepStrings.Count > 0 ? " or " : "",
                                               lastStepString);

                    // And lastly add the colorings
                    foreach(Guid nodeId in nodes.Keys)
                        retVal.Add(new GraphColoring(nodeId, c, String.Format("Entities that are {0} steps away from <i>{1}</i>", stepString, _neighborhood.StartNode.Label)));
                }

                return retVal.ToArray();
            }
        }
    }
}