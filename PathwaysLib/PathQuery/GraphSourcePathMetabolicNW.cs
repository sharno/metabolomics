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
    /// A final class for using the GraphSource with path queries and the metabolic network graph
    /// </summary>
    public class GraphSourcePathMetabolicNW : GraphSourcePath
    {
        private GraphSourcePathMetabolicNW()
            : base()
        { }

        public GraphSourcePathMetabolicNW(QueryPathResults path)
            : base(path)
        { }

        public override String CompHValue
        {
            get { return null; }
        }

        public override Guid[] CollapsedPathways
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

                foreach(List<INode> path in _path.Paths)
                    foreach(INode pathNode in path)
                        if(pathNode.Type == NodeType.Edge)
                            retVal.Add(pathNode.Id);

                return retVal.ToArray();
            }
        }

        public override Guid[] MoleculeGraphIDs
        {
            get
            {
                List<Guid> retVal = new List<Guid>();

                foreach(List<INode> path in _path.Paths)
                    foreach(INode pathNode in path)
                        if(pathNode.Type == NodeType.Node)
                            retVal.Add(pathNode.Id);

                return retVal.ToArray();
            }
        }
    }
}