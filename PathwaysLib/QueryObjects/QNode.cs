namespace PathwaysLib.QueryObjects
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Web.UI.WebControls;
	using System.Xml;
	using PathwaysLib.QueryObjects;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Utilities;
	using PathwaysLib.WebControls;

	#region QNode type enumeration
	/// <summary>
	/// An enumeration of possible QNode types
	/// </summary>
    [Obsolete("This enum will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public enum QNodeType
	{
		/// <summary>
		/// This QNode is the root of an AQI instance
		/// </summary>
		AQIRoot,
		
		/// <summary>
		/// This QNode is a node inside an AQI instance
		/// </summary>
		AQINode,
		
		/// <summary>
		/// This QNode is part of a built-in query
		/// </summary>
		Query
	}
	#endregion

	/// <summary>
	/// A generic query node.
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public abstract class QNode : System.Web.UI.WebControls.Panel
	{
		#region Private members and static methods

		private string title = string.Empty;
		private string name = string.Empty;
		private ArrayList parameters = new ArrayList();

		private static int ExtractID( string id )
		{
			string[] nodes = id.Split( ':' );
			return int.Parse( nodes[nodes.Length - 1].Split( '-' )[0] );
		}

		public static QNode CreateFromType( int type, string id, string name )
		{
			QNode newNode;
			switch( type )
			{
				case 0: newNode = new QNodeAQIPathway( id, name ); break;
				case 1: newNode = new QNodeAQIProcess( id, name ); break;
				case 2: newNode = new QNodeAQIMolecule( id, name ); break;
				case 3: newNode = new QNodeAQIOrganism( id, name ); break;
				default: newNode = null; break;
			}
			return newNode;
		}

		#endregion

		#region Properties (read-only)
		/// <summary>
		/// The node title; what is displayed in the outermost border
		/// </summary>
		public virtual string Title { get { return string.Empty; } }

		/// <summary>
		/// A friendly name for this node
		/// </summary>
		public string Name { get { return name; } }

		/// <summary>
		/// This node's type
		/// </summary>
		public virtual QNodeType Type { get { return QNodeType.AQINode; } }

		public virtual string[] AddTip { get { return null; } }

		/// <summary>
		/// All of the parameters that this node will encompass
		/// </summary>
		public ArrayList Parameters { get { return parameters; } }
		#endregion

		/// <summary>
		/// Default constructor; sets the ID
		/// </summary>
		public QNode( string id, string uName ) { ID = id; name = uName; }

		#region Query-building static methods

		/// <summary>
		/// Creates a QueryBuilder object that represents the provided DOM node
		/// </summary>
		/// <param name="node">The DOM node to process</param>
		/// <returns>A QueryBuilder object that represents the node</returns>
        public static QueryBuilder BuildQuery(XmlElement node, QIdCounter counter)
        {
            QueryBuilder query = new QueryBuilder(counter);
            string queryTable = string.Empty;

            // Figure out which kind of node this is
            QNode nodeClass = QNode.CreateFromType(ExtractID(node.GetAttribute("id")),
                string.Empty, string.Empty);

            bool onNodes = false;

            // Nodes will contains a list of parameters followed by a list of child nodes.
            // If there are nodes before parameters or parameters after nodes, the XML is malformed.
            foreach(XmlElement item in node.ChildNodes)
            {
                if(item.Name == "param")
                {
                    if(onNodes) throw new Exception("Malformed AQI XML");

                    // Create the QField object this parameter uses
                    int paramId = int.Parse(item.GetAttribute("id"));
                    QField field = ((QParam)nodeClass.Parameters[paramId]).Fields[0];

                    // Obtain the tables this parameter must access
                    query.AddFrom(field.FromTables);

                    // add other conditions needed (i.e. join clauses)
                    if(field.WhereAndClauses() != null)
                    {
                        foreach(string clause in field.WhereAndClauses())
                        {
                            query.AddWhere(clause);
                        }
                    }

                    // Add to the output if the parameter is included
                    if(item.HasAttribute("inc"))
                        query.AddSelect(field.SelectSQL.Replace("#", node.GetAttribute("name")));

                    // Process the actual field values, if any; multiple values are ORed together
                    ArrayList where = new ArrayList();
                    foreach(XmlElement fld in item.ChildNodes)
                    {
                        if(fld.HasAttribute("value"))
                        {
                            // Modified to avoid problems with '{' or '}'
                            where.Add(field.WhereValueClause(fld.GetAttribute("value").Replace("{", "{{").Replace("}", "}}")));
                        }
                    }

                    if(where.Count > 0)
                        query.AddWhere("(" + string.Join(" OR ",
                            (string[])where.ToArray(typeof(string))) + ")");
                }
                else if(item.Name == "node")
                {
                    onNodes = true;

                    // Join with this node; handled by the specific QNode object instantiated here
                    nodeClass.JoinWith(ref query, QNode.BuildQuery(item, counter),
                        ExtractID(item.GetAttribute("id")));
                }
                else throw new Exception("Malformed AQI XML");
            }

            // Finally, we have a complete query!
            return query;
        }

        /// <summary>
        /// Joins two QueryBuilder objects together using node-specific join operations
        /// </summary>
        /// <param name="ownQuery">The QueryBuilder object that represents this node</param>
        /// <param name="foreignQuery">The QueryBuilder object that represents the node to join with</param>
        /// <param name="id">A numerical ID representing the node type (see GetNodeClass)</param>
        public virtual void JoinWith(ref QueryBuilder ownQuery, QueryBuilder childQuery, int parentType) { }

		#endregion

		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output">The HTML writer to write out to.</param>
		protected override void Render( System.Web.UI.HtmlTextWriter output )
		{
			// Activate contained parameters
			int pid = 0;
			if( Type != QNodeType.AQIRoot )
				foreach( QParam param in parameters ) param.SetProperties( ID, pid++, Type );

			// Build the inner content panel
			Panel contentPanel = new Panel();
			contentPanel.ID = ID + ";Content";

			// Build the parameter panel
			if( Type != QNodeType.AQIRoot && parameters.Count > 0 )
			{
				Panel paramPanel = new Panel();
				paramPanel.ID = ID + ";Params";
				paramPanel.CssClass = "aqiparams";
				foreach( QParam param in parameters ) paramPanel.Controls.Add( param );
				contentPanel.Controls.Add( paramPanel );
			}

			// Construct an "add new" panel if this is part of the AQI
			if( Type != QNodeType.Query )
			{
				contentPanel.CssClass = "whitebg";
				Panel addPanel = new Panel();
				contentPanel.Controls.Add( addPanel );
				addPanel.ID = ID + ";Add";
				addPanel.CssClass = "aqiaddnode";

				ArrayList add = new ArrayList();
				if( !ID.Contains( "0-" ) ) add.Add( new DictionaryEntry( 0, "Pathway" ) );
				if( !ID.Contains( "1-" ) ) add.Add( new DictionaryEntry( 1, "Process" ) );
				if( !ID.Contains( "2-" ) ) add.Add( new DictionaryEntry( 2, "Molecular Entity" ) );
				if( !ID.Contains( "3-" ) ) add.Add( new DictionaryEntry( 3, "Organism" ) );

				if( add.Count > 0 )
				{
					addPanel.Controls.Add( ControlBuilder.BuildLiteral( "Add new: " ) );

					for( int x = 0; x < add.Count; x++ )
					{
						HyperLink link = new HyperLink();
						link.NavigateUrl = "#";
						link.Attributes.Add( "onclick", "AQI.AddNode('" + ID + "'," +
							( (DictionaryEntry)add[x] ).Key.ToString() + ");return false" );
						link.Attributes.Add( "onmouseover", "System.AttachToolTip(this,'" +
							AddTip[(int)((DictionaryEntry)add[x]).Key] + "')" );
						link.Text = ( (DictionaryEntry)add[x] ).Value.ToString();
						addPanel.Controls.Add( link );
						if( x != add.Count - 1 ) addPanel.Controls.Add( ControlBuilder.BuildLiteral( ", " ) );
					}
				}
			}

			if( Type == QNodeType.Query )
			{
				// Use the normal CollapsiblePanel for drawing if this is a query
				CollapsiblePanel mainPanel = new CollapsiblePanel();
				mainPanel.ID = ID;
				mainPanel.Title = Title;
				mainPanel.Controls.Add( contentPanel );
				mainPanel.RenderControl( output );
			}
			else if( Type == QNodeType.AQIRoot )
			{
				// The AQI root needs some special features and such; its control is
				// handily stored in the parameters list.
				CollapsiblePanel mainPanel = (CollapsiblePanel)Parameters[0];
				mainPanel.Controls.Add( contentPanel );

				Panel submitPanel = new Panel();
				submitPanel.ID = "aqisubmit";
				mainPanel.Controls.Add( submitPanel );

				HyperLink submitLink = new HyperLink();
				submitLink.NavigateUrl = "#";
				submitLink.Attributes.Add( "onclick", "AQI.SubmitQuery();return false" );
				submitLink.Text = "Submit query";
				submitPanel.Controls.Add( submitLink );

				mainPanel.RenderControl( output );
			}
			else
			{
				// Use a special AQI layout otherwise
				contentPanel.CssClass = "aqicontent";

				// Title region: collapse icon, title (click to activate node), delete icon
				Panel titlePanel = new Panel();
				titlePanel.ID = ID + ";Title";
				titlePanel.CssClass = "aqititle";

				// Left title area
				Panel titleLeftPanel = new Panel();
				titlePanel.Controls.Add( titleLeftPanel );
				titleLeftPanel.CssClass = "aqititleleft";
				titleLeftPanel.Attributes.Add( "onclick", "AQI.CollapseNode('" + ID + "')" );
				titleLeftPanel.ToolTip = "Click to collapse/expand this node";

				// Collapse button
                //BUG: in firefox, javascript onclick not working here!!
				Image collapseButton = new Image();
				titleLeftPanel.Controls.Add( collapseButton );
				collapseButton.ID = ID + ";Collapse";
				collapseButton.CssClass = "collapsebutton";
				collapseButton.AlternateText = "Click to collapse/expand this node";
				collapseButton.ToolTip = collapseButton.AlternateText;
				collapseButton.ImageUrl = "../Images/collapse.gif";
				//collapseButton.Attributes.Add( "onclick", "AQI.CollapseNode('" + ID + "')" );

				// Title (and toggle)
				Label titleLabel = new Label();
				titleLeftPanel.Controls.Add( titleLabel );
				titleLabel.Text = Title + " " + Name;
				titleLabel.CssClass = "aqinodetitle";
				//				titleLabel.Attributes.Add( "onclick", "toggleNode('" + ID + "')" );
				//				titleLabel.ToolTip = "Include/exclude this node in the output";
				// node activation removed until necessary

				// Delete button
				Image deleteButton = new Image();
				titlePanel.Controls.Add( deleteButton );
				deleteButton.CssClass = "aqideletebutton";
				deleteButton.AlternateText = "Delete this node";
				deleteButton.ToolTip = deleteButton.AlternateText;
				deleteButton.ImageUrl = "../Images/delete.gif";
				deleteButton.Attributes.Add( "onclick", "AQI.DeleteNode('" + ID + "')" );

				// Render the title and content panels
				titlePanel.RenderControl( output );
				contentPanel.RenderControl( output );
			}
		}
	}
}