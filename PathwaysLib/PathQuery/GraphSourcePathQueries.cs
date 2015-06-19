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
    /// A base class for the GraphSource implementation for all of the path-based queries (both path and neighborhood)
    /// </summary>
    public abstract class GraphSourcePathQueries : IGraphSource
    {
        protected GraphSourcePathQueries()
        { }

        /// <summary>
        /// A list of the IDs of collapsed models nodes to add to the graph.
        /// </summary>
        public Guid[] CollapsedModels
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// A list of the IDs of fully expanded models to add to the graph (i.e. add all the reactions in the model)
        /// </summary>
        public Guid[] ExpandedModels
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// A list of the IDs of reactions to add to the graph.
        /// </summary>
        public Guid[] ReactionIDs
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// A lsit of the IDs of species to add to the graph.
        /// </summary>
        public Guid[] SpeciesIDs
        {
            get
            {
                return null;
            }
        }


        /// <summary>
        /// Specifies the type of the graph content. 
        /// </summary>
        public GraphContent ContentType
        {
            get
            {
                return GraphContent.Pathway;
            }
        }

        public abstract Guid[] CollapsedPathways
        {
            get;
        }

        public abstract Guid[] ExpandedPathways
        {
            get;
        }

        public abstract Guid[] GenericProcessGraphIDs
        {
            get;
        }

        public abstract String CompHValue
        {
            get;
        }

        public abstract Guid[] MoleculeGraphIDs
        {
            get;
        }

        public GraphLayout Layout
        {
            get { return GraphLayout.Hierarchical; }
        }

        public abstract Guid SourceNode
        {
            get;
        }

        public string InitialOrganism
        {
            get { return ServerOrganism.UnspecifiedOrganism; }
        }

        public abstract string GraphType
        {
            get;
        }

        public abstract string GraphTitle
        {
            get;
        }

        public abstract GraphColoring[] Colorings
        {
            get;
        }

        public bool LegendVisible
        {
            get { return true; }
        }
    }
}