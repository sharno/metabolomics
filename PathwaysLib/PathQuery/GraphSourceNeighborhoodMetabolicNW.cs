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
    /// The final GraphSource class used with the neighborhood query and the metabolic network graph
    /// </summary>
    public class GraphSourceNeighborhoodMetabolicNW : GraphSourceNeighborhood
    {
        private GraphSourceNeighborhoodMetabolicNW()
            : base()
        { }

        public GraphSourceNeighborhoodMetabolicNW(QueryNeighborhoodResults neighborhood)
            : base(neighborhood)
        { }

        public override Guid[] CollapsedPathways
        {
            get { return null; }
        }

        public override String CompHValue
        {
            get { return null; }
        }

        public override Guid[] ExpandedPathways
        {
            get { return null; }
        }

        public override Guid[] GenericProcessGraphIDs
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

        public override Guid[] MoleculeGraphIDs
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
    }
}