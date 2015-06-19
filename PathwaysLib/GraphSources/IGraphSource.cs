using System;
using System.Drawing;

namespace PathwaysLib.GraphSources
{
	/// <summary>
	/// Information regarding how graphs should be colored.
	/// </summary>
    public struct GraphColoring
    {
        private static Color[] colors = { Color.Chartreuse, Color.Blue, Color.Orange, 
                                          Color.Pink, Color.Gray, Color.Brown, Color.Chocolate,
                                          Color.Cyan, Color.Wheat, Color.SteelBlue};

		/// <summary>
		/// Constructor with explicit coloring information
		/// </summary>
		/// <param name="id"></param>
		/// <param name="color"></param>
		/// <param name="description"></param>
        public GraphColoring(Guid id, Color color, string description)
        {
            this.ID = id;
            this.Color = color;
			this.Description = description;
        }

		/// <summary>
		/// Constructor with coloring information stored in a string
		/// </summary>
		/// <param name="str"></param>
		public GraphColoring(string str)
		{
			char[] delim = {'@'};
			string[] tokens = str.Split(delim);
			if(tokens.Length == 3)
			{
				this.ID = new Guid(tokens[0]);
				this.Color = Color.FromName(tokens[1]);
				this.Description = tokens[2];
			}
			else
			{
				this.Color = Color.FromName("white");
				this.ID = Guid.Empty;
				this.Description = string.Empty;
			}

		}
        /// <summary>
		/// ToString override
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = ID.ToString();
			//str += "@" + this.Color.Name;
			str += "@[" + this.Color.R + "," + this.Color.G + "," + this.Color.B + "]";
			return str;
		}

		/// <summary>
		/// The item's ID
		/// </summary>
        public Guid ID;

		/// <summary>
		/// The item's color
		/// </summary>
        public Color Color;

		/// <summary>
		/// The item's description
		/// </summary>
		public string Description;

        public static Color GetNextColor(int index)
        {
            return colors[(index - 1) % colors.Length];
        }
    }

    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysWeb/Utilities/IGraphSource.cs</filepath>
    ///		<creation>2005/11/28</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: xjqi $</cvs_author>
    ///			<cvs_date>$Date: 2009/12/22 19:13:31 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/GraphSources/IGraphSource.cs,v 1.6 2009/12/22 19:13:31 xjqi Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.6 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Common interface for components that generate data that can be visualized
    /// using the new generic graph drawing functionality.
    /// </summary>
    public interface IGraphSource
	{
        #region Graph Layout (used by GraphLayoutLib)

        /// <summary>
        /// A list of the IDs of collapsed pathways nodes to add to the graph.
        /// </summary>
        Guid[] CollapsedPathways
        {
            get;
        }
        
        /// <summary>
        /// A list of the IDs of fully expanded pathways to add to the graph (i.e. add all the processes in the pathway)
        /// </summary>
        Guid[] ExpandedPathways
        {
            get;
        }

        /// <summary>
        /// A list of the IDs of generic processes to add to the graph.
        /// </summary>
        Guid[] GenericProcessGraphIDs
        {
            get;
        }
        
        /// <summary>
        /// A lsit of the IDs of molecules to add to the graph.
        /// </summary>
        Guid[] MoleculeGraphIDs
        {
            get;
        }
        
        /// <summary>
        /// A list of the IDs of collapsed models nodes to add to the graph.
        /// </summary>
        Guid[] CollapsedModels
        {
            get;
        }

        /// <summary>
        /// A list of the IDs of fully expanded models to add to the graph (i.e. add all the reactions in the model)
        /// </summary>
        Guid[] ExpandedModels
        {
            get;
        }

        /// <summary>
        /// A list of the IDs of reactions to add to the graph.
        /// </summary>
        Guid[] ReactionIDs
        {
            get;
        }

        /// <summary>
        /// A lsit of the IDs of species to add to the graph.
        /// </summary>
        Guid[] SpeciesIDs
        {
            get;
        }        


        /// <summary>
        /// Specifies the type of the graph content. 
        /// </summary>
        GraphContent ContentType
        {
            get;
        }

        /// <summary>
        /// Specifies the layout algorithm to use. 
        /// </summary>
        GraphLayout Layout
        {
            get;
        }

        /// <summary>
        /// Specifies the source node for hierarchical layout in neighborhood queries. 
        /// </summary>
        Guid SourceNode
        {
            get;
        }
        #endregion

        #region Graph Coloring (used by the Java graph viewer)

        /// <summary>
        /// The name of the initial organism/group to display on the graph (default: "unspecified")
        /// </summary>
        string InitialOrganism
        {
            get;
        }

		/// <summary>
		/// The graph type
		/// </summary>
		string GraphType
		{
			get;
		}

		/// <summary>
		/// The graph title
		/// </summary>
		string GraphTitle
		{
			get;
		}

        /// <summary>
        /// A list of graph coloring pairs (i.e. GUID ID of the object and the color to draw it in)
        /// </summary>
        GraphColoring[] Colorings
        {
            get;
        }

        /// <summary>
        /// Display color legend at the bottom of the graph.
        /// </summary>
        bool LegendVisible
        {
            get;
        }

        #endregion
	}

    public enum GraphLayout
    {
        Hierarchical,
        Circular, 
        Orthogonal,
        Tree,
        Organic
    }

    public enum GraphContent
    {
        Pathway,
        Model,
        Hybrid,
        UserModel
    }

}