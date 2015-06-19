using System;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.GraphSources
{
	/// <summary>
	/// IGraphSource implementation for processes
	/// </summary>
	public class GraphSourceProcess : IGraphSource
	{
		private ServerProcess sProcess;
		private string graphType = string.Empty;

        /// <summary>
		/// Default constructor
		/// </summary>
		public GraphSourceProcess( Guid prid, string type )
		{
			sProcess = ServerProcess.GetProcessesByGenericProcessId( prid )[0];
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
			// TODO: Add GraphSourceProcess.CollapsedPathways implementation
			get{ return null; }
		}

        public String CompHValue
        {
            get { return "false"; }
        }

		/// <summary>
		/// Get expanded pathways
		/// </summary>
		public Guid[] ExpandedPathways
		{
			// TODO: Add GraphSourceProcess.ExpandedPathways implementation
			get{ return null; }
		}

        /// <summary>
        /// Get generic processes
        /// </summary>
        public Guid[] GenericProcessGraphIDs
        {
            get 
            {
                Guid[] id = new Guid[1];                
                id[0] = GraphNodeManager.GetAnyProcessGraphNodeId(sProcess.GenericProcessID);
             
                return id;
            }
        }

        /// <summary>
        /// Get molecules
        /// </summary>
        public Guid[] MoleculeGraphIDs
        {
            get { return null; }
        }

        ///// <summary>
        ///// Get generic processes
        ///// </summary>
        //public Guid[] GenericProcesses
        //{
        //    // TODO: Add GraphSourceProcess.GenericProcesses implementation
        //    get
        //    {
        //        Guid[] id = new Guid[1];
        //        id[0] = sProcess.GenericProcessID;
        //        return id;
        //    }
        //}

        ///// <summary>
        ///// Get molecules
        ///// </summary>
        //public Guid[] Molecules
        //{
        //    // TODO: Add GraphSourceProcess.Molecules implementation
        //    get{ return null; }
        //}

        /// <summary>
        /// Whether this is a hierarchical layout or not
        /// </summary>
        public GraphLayout Layout
		{
			// TODO: Add GraphSourceProcess.HierarchicalLayout implementation
            get { return GraphLayout.Hierarchical; }
		}

		/// <summary>
		/// Get the initial organism
		/// </summary>
		public string InitialOrganism
		{
			// TODO: Add GraphSourceProcess.InitialOrganism implementation
			get{ return string.Empty; }
		}

		/// <summary>
		/// Get the graph title
		/// </summary>
		public string GraphTitle
		{
			get{ return "Interactive Process Graph"; }
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
			// TODO: Add GraphSourceProcess.Colorings implementation
			get{ return null; }
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