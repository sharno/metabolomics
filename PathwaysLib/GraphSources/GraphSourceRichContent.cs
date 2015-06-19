using System;
using System.Collections;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.GraphSources
{
	/// <summary>
	/// IGraphSource implementation for rich content
	/// </summary>
	public class GraphSourceRichContent : IGraphSource
	{
		private Guid[] collapsedPathways;
		private Guid[] expandedPathways;
		private Guid[] genericProcesses;
		private Guid[] molecules;
		private GraphLayout layout;
		private string initialOrganism;
		private string graphTitle;
		private string graphType;
		private GraphColoring[] colorings;

        /// <summary>
		/// Default constructor
		/// </summary>
		public GraphSourceRichContent( Guid id, string type )
		{
			IGraphSource gs;

			// Set up the IGraphSource object for the graph depending on the graph type
			if( type.IndexOf( "Pathway" ) != -1 ) gs = new GraphSourcePathway( id, type );
			else gs = new GraphSourceProcess( id, type );

			collapsedPathways = gs.CollapsedPathways;
			expandedPathways = gs.ExpandedPathways;
            genericProcesses = gs.GenericProcessGraphIDs;
            molecules = gs.MoleculeGraphIDs;
			layout = gs.Layout;
			initialOrganism = gs.InitialOrganism;
			graphTitle = gs.GraphTitle;
			graphType = gs.GraphType;
			colorings = gs.Colorings;
		}

		#region IGraphSource Members
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
		/// <summary>
		/// Gets collapsed pathways
		/// </summary>
		public Guid[] CollapsedPathways
		{
			get{ return collapsedPathways; }
		}

		/// <summary>
		/// Gets expanded pathways
		/// </summary>
		public Guid[] ExpandedPathways
		{
			get{ return expandedPathways; }
		}

        public String CompHValue
        {
            get { return "false"; }
        }

		/// <summary>
		/// Gets generic processes
		/// </summary>
        public Guid[] GenericProcessGraphIDs
		{
			get{ return genericProcesses; }
		}

		/// <summary>
		/// Gets molecules
		/// </summary>
        public Guid[] MoleculeGraphIDs
		{
			get{ return molecules; }
		}

		/// <summary>
		/// Whether to display in a hierarchical layout
		/// </summary>
        public GraphLayout Layout
		{
            get { return GraphLayout.Organic; }
		}

		/// <summary>
		/// Gets/sets the inital organism
		/// </summary>
		public string InitialOrganism
		{
			get{ return initialOrganism; }
			set{ initialOrganism = value; }
		}

		/// <summary>
		/// Gets/sets the graph title
		/// </summary>
		public string GraphTitle
		{
			get{ return graphTitle; }
			set{ graphTitle = value; }
		}

		/// <summary>
		/// Gets/sets the graph type
		/// </summary>
		public string GraphType
		{			
			get{ return graphType; }
			set{ graphType = value; }
		}

		/// <summary>
		/// Gets the graph coloring
		/// </summary>
		public GraphColoring[] Colorings
		{
			get{ return colorings; }
		}

        /// <summary>
        /// Display color legend at the bottom of the graph.
        /// </summary>
        public bool LegendVisible
        {
            get { return true; }
        }

        public Guid SourceNode
        {
            get { return Guid.Empty; }
        }

        #endregion
    }
}