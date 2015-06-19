using System;
using System.Collections;
using System.Web;  // Temp
using PathwaysLib.ServerObjects;

namespace PathwaysLib.GraphSources
{
	/// <summary>
	/// IGraphSource implementation for fullscreen graphs
	/// </summary>
	public class GraphSourceFullscreen : IGraphSource
	{
		private Guid[] collapsedPathways;
		private Guid[] expandedPathways;
		private Guid[] genericProcesses;
		private Guid[] molecules;
		private string initialOrganism = string.Empty;
		private string graphTitle = string.Empty;
		private string graphType = string.Empty;
		private GraphColoring[] colorings;

		private bool canDisplay = true;

        /// <summary>
		/// Default constructor
		/// </summary>
		public GraphSourceFullscreen( Guid id, string type )
		{
			IGraphSource gs;
            if (type!=null && type != "" && type.Contains("FullScreen"))
			    type = type.Substring( 0, type.Length - "FullScreen".Length );

			// Set up the IGraphSource object for the graph depending on the graph type
			if( type.StartsWith("AllPathways") )
			{
				gs = this;
				initialOrganism = "unspecified";
			}
			else if( type.StartsWith( "RichContent" ) || type.StartsWith( "Query" ) )
			{
				// TODO: (GJS) This is only a temporary workaround, but for now, get graph
				// information from session variables.
				gs = (IGraphSource)HttpContext.Current.Session["CurrentGraphSource"];
			}
			else if( type == "Process" ) gs = new GraphSourceProcess( id, type );
			else gs = new GraphSourcePathway( id, type );  // "Pathway" or "ConnectedPathway"

			if( canDisplay )
			{
				collapsedPathways = gs.CollapsedPathways;
				expandedPathways = gs.ExpandedPathways;
				genericProcesses = gs.GenericProcessGraphIDs;
				molecules = gs.MoleculeGraphIDs;
				initialOrganism = gs.InitialOrganism;
				graphType = type;
				graphTitle = gs.GraphTitle;
				colorings = gs.Colorings;
			}
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
		/// Get collapsed pathways
		/// </summary>
		public Guid[] CollapsedPathways
		{
			get
			{				
				if( graphType == "AllPathways" )
				{
					ServerPathway[] pws = ServerPathway.AllPathways();
					Guid[] ids = new Guid[pws.Length];
					for( int i=0; i<pws.Length; i++ ) ids[i] = pws[i].ID;
					return ids;
				}
				else
				{
					return collapsedPathways;
				}
			}
		}

		/// <summary>
		/// Get expanded pathways
		/// </summary>
		public Guid[] ExpandedPathways
		{
			get
			{
				if( graphType == "AllPathways" ) return null;
				else return expandedPathways;
			}
		}


        public String CompHValue
        {
            get { return "false"; }
        }

		/// <summary>
		/// Get generic processes
		/// </summary>
        public Guid[] GenericProcessGraphIDs
		{
			get
			{
				if( graphType == "AllPathways" ) return null;
				else return genericProcesses;
			}
		}

		/// <summary>
		/// Get molecules
		/// </summary>
        public Guid[] MoleculeGraphIDs
		{
			get
			{
				if( graphType == "AllPathways" ) return null;
				else return molecules;
			}
		}

		/// <summary>
		/// Whether this is a hierarchical layout or not
		/// </summary>
        public GraphLayout Layout
		{
			get
			{
                return GraphLayout.Organic;
                /*
				if( graphType.StartsWith( "AllPathways" ) || graphType.StartsWith( "ConnectedPathway" ) ) return GraphLayout.Organic;
				else return GraphLayout.Hierarchical;
                 * */
			}
		}

		/// <summary>
		/// Get/set the initial organism
		/// </summary>
		public string InitialOrganism
		{
			get{ return initialOrganism; }
			set{ initialOrganism = value; }
		}

		/// <summary>
		/// Get/set the graph title
		/// </summary>
		public string GraphTitle
		{
			get
			{
				if( graphType == "AllPathways" ) return "PathCase Pathway Network";
				else return graphTitle;
			}
			set{ graphTitle = value; }
		}

		/// <summary>
		/// Get/set the graph type
		/// </summary>
		public string GraphType
		{
			get{ return graphType + "FullScreen"; }
			set{ graphType = value; }
		}

		/// <summary>
		/// Get the graph colorings
		/// </summary>
		public GraphColoring[] Colorings
		{
			get
			{
				if( graphType == "AllPathways" ) return null;
				else return colorings;
			}
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