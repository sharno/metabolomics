using System;
using System.Drawing;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;

namespace PathwaysLib.GraphSources
{
	/// <summary>
	/// IGraphSource implementation for pathways
	/// </summary>
	public class GraphSourcePathway : IGraphSource
	{
		private ServerPathway sPathway;
		private string graphType = string.Empty;

        /// <summary>
		/// Default constructor
		/// </summary>
		public GraphSourcePathway( Guid pwid, string type )
		{
			sPathway = ServerPathway.Load( pwid );
			graphType = type;
		}

		#region IGraphSource members
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
				if( GraphType == "Pathway" ) return null;
				else
				{
					ConnectedPathwayAndCommonProcesses[] pws = sPathway.GetConnectedPathways();
					Guid[] ids = new Guid[pws.Length + 1];
					for( int i=0; i<ids.Length-1; i++ ) ids[i] = pws[i].ConnectedPathway.ID;
					ids[ids.Length-1] = sPathway.ID;
					return ids;
				}
			}
		}

		/// <summary>
		/// Get expanded pathways
		/// </summary>
		public Guid[] ExpandedPathways
		{

			get{
				if( GraphType == "Pathway" ) 
					return new Guid[]{sPathway.ID}; 
				else
					return null;
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
			get{ return null; }
		}

		/// <summary>
		/// Get molecules
		/// </summary>
        public Guid[] MoleculeGraphIDs
		{
			get{ return null; }
		}

        /// <summary>
        /// Whether this is a hierarchical layout or not
        /// </summary>
        public GraphLayout Layout
		{
			get{
                
                if (System.Configuration.ConfigurationManager.AppSettings["DataSource"].ToString().ToLower() == "biocyc")
					return GraphLayout.Hierarchical; 
				else
                    return GraphLayout.Organic;
			}
		}

		/// <summary>
		/// Get the initial organism
		/// </summary>
		public string InitialOrganism
		{
			get{ return string.Empty; }
		}

		/// <summary>
		/// Get the graph title
		/// </summary>
		public string GraphTitle
		{
			get
			{
				if( GraphType == "Pathway" ) return "Interactive Pathway Graph";
				else return "Neighbor Pathways Graph";
			}
		}

		/// <summary>
		/// Get the graph type
		/// </summary>
		public string GraphType
		{
			get
			{
				return graphType;
			}
		}

		/// <summary>
		/// Get coloring information
		/// </summary>
		public GraphColoring[] Colorings
		{
			get
			{ 
				if(GraphType.StartsWith("Pathway"))
					return null; 
				else
				{
					GraphColoring[] gc = new GraphColoring[1];
					gc[0]= new GraphColoring( sPathway.ID, Color.Chartreuse,
						"Source pathway (" + sPathway.Name + ")" );
					return gc;
				}
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
            get { return Guid.Empty; ; }
        }

        #endregion
    }
}