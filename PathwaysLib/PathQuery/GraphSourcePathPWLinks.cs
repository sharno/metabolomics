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
    /// A final class for using the GraphSource with path queries and the pathway links graph
    /// </summary>
    public class GraphSourcePathPWLinks : GraphSourcePath
    {
        private GraphSourcePathPWLinks()
        { }

        public GraphSourcePathPWLinks(QueryPathResults path)
            : base(path)
        { }

        public override Guid[] CollapsedPathways
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

        public override Guid[] ExpandedPathways
        {
            get { return null; }
        }

        public override String CompHValue
        {
            get { return null; }
        }

        public override Guid[] GenericProcessGraphIDs
        {
            get { return null; }
        }

        public override Guid[] MoleculeGraphIDs
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
    }
}