namespace PathwaysLib.WebControls
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using PathwaysLib.QueryObjects;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Utilities;

	/// <summary>
	/// The root AQI node.
	/// </summary>
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QNodeAQIRoot : QNode
	{
		public override string Title { get { return "Advanced Query Interface"; } }
		public override QNodeType Type { get { return QNodeType.AQIRoot; } }
		public override string[] AddTip { get { return new string[] { "Search for a pathway",
			"Search for a process", "Search for a molecular entity", "Search for an organism" }; } }

		/// <summary>
		/// Constructor to set the ID
		/// </summary>
		public QNodeAQIRoot( string id, string name ) : base( id, name )
		{
			// Create the special root panel and stuff it in the parameters list; sneaky~
			CollapsiblePanel rootPanel = new CollapsiblePanel();
			rootPanel.ID = id;
			rootPanel.Title = Title;
			rootPanel.Buttons.Add( ControlBuilder.BuildLiteral(
				"<a href=\"#\" onclick=\"AQI.SubmitQuery();return false\">Submit query</a> | " +
				"<a href=\"LinkForwarder.aspx?rid=Help_AQI&rtype=br\" onclick=\"return newWin(this)\">" +
				"AQI Help</a>" ) );

			Parameters.Add( rootPanel );
		}
	}
}