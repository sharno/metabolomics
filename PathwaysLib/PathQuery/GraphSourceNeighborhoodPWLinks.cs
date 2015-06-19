using PathQueryLib;
//using PathwaysLib.GraphSources;
using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PathwaysLib.PathQuery
{
    /// <summary>
    /// The final GraphSource class used with the neighborhood query and the pathway links graph
    /// </summary>
    public class GraphSourceNeighborhoodPWLinks : GraphSourceNeighborhood
    {
        private GraphSourceNeighborhoodPWLinks()
        { }

        public GraphSourceNeighborhoodPWLinks(QueryNeighborhoodResults neighborhood)
            : base(neighborhood)
        { }

        public override Guid[] CollapsedPathways
        {
            get
            {
                List<Guid> retVal = new List<Guid>();

                foreach(INode node in _neighborhood.Nodes)
                    if(node.Type == NodeType.Node)
                        retVal.Add(node.Id);

                return retVal.ToArray();
            }
        }

        public override Guid[] ExpandedPathways
        {
            get { return null; }
        }

        public override Guid[] GenericProcessGraphIDs
        {
            get { return null; }
        }

        public override String CompHValue
        {
            get { return null; }
        }

        public override Guid[] MoleculeGraphIDs
        {
            get
            {
                List<Guid> retVal = new List<Guid>();

                foreach(INode node in _neighborhood.Nodes)
                    if(node.Type == NodeType.Edge)
                        retVal.Add(node.Id);

                return retVal.ToArray();
            }
        }
    }
}