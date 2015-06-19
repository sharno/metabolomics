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
    /// A base class for using the GraphSource with path queries
    /// </summary>
    public abstract class GraphSourcePath : GraphSourcePathQueries
    {
        protected QueryPathResults _path;

        protected GraphSourcePath()
            : base()
        { }

        public GraphSourcePath(QueryPathResults path)
            : base()
        {
            _path = path;
        }

        public override Guid SourceNode
        {
            get { return _path.Parameters.FromNodeId; }
        }

        public override string GraphType
        {
            get { return "PathBetweenMolecules"; }
        }

        public override string GraphTitle
        {
            get { return "Path Graph"; }
        }

        public override GraphColoring[] Colorings
        {
            get
            {
                Dictionary<Guid, GraphColoring> specialNodeColorings = new Dictionary<Guid, GraphColoring>(); //(new EqualityComparerINode());
                Dictionary<Guid, GraphColoring> pathNodeColorings = new Dictionary<Guid, GraphColoring>(); // (new EqualityComparerINode());

                specialNodeColorings.Add(_path.Parameters.FromNodeId, new GraphColoring(_path.Parameters.FromNodeId, Color.Chartreuse, "Starting Entity"));
                QueryPathParametersToNode destinationToNode = _path.Parameters.ToNodes[_path.Parameters.ToNodes.Count - 1];
                foreach(QueryPathParametersToNode toNode in _path.Parameters.ToNodes)
                    specialNodeColorings.Add(toNode.NodeId, toNode == destinationToNode ? new GraphColoring(toNode.NodeId, Color.OrangeRed, "Destination Entity") : new GraphColoring(toNode.NodeId, Color.DarkOrange, "Intermediate Hop Entity"));

                foreach(List<INode> path in _path.Paths)
                    foreach(INode pathNode in path)
                        if(!specialNodeColorings.ContainsKey(pathNode.Id))
                            if(!pathNodeColorings.ContainsKey(pathNode.Id))
                                pathNodeColorings.Add(pathNode.Id, new GraphColoring(pathNode.Id, Color.Pink, "Entities on the Path"));

                List<GraphColoring> retVal = new List<GraphColoring>();
                retVal.AddRange(specialNodeColorings.Values);
                retVal.AddRange(pathNodeColorings.Values);

                return retVal.ToArray();
            }
        }
    }
}