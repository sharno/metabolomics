using System;
using System.Web.UI.WebControls;
using PathwaysLib.QueryObjects;
using PathwaysLib.WebControls;

namespace PathwaysLib.QueryObjects
{
    [Obsolete("This class will soon be removed and replaced by a new implementation of the new AQI library. Refer the PathwaysLib.AQI and AQILib in the future.")]
    public class QParam : System.Web.UI.WebControls.Panel
	{
		#region Private members

		private string parentid = string.Empty;
		private int paramid = 1;
		private string title = string.Empty;
		private QField[] fields = null;
		private QNodeType type = QNodeType.AQINode;

		#endregion

		#region Properties

		public string ParentID
		{
			get { return parentid; }
			set { parentid = value; }
		}

		public int ParamID
		{
			get { return paramid; }
			set { paramid = value; }
		}

		public QNodeType Type
		{
			get { return type; }
			set { type = value; }
		}

		public override string ID { get { return parentid + ";" + paramid.ToString(); } }
		public QField[] Fields { get { return fields; } }

		#endregion

		public QParam( string paramTitle, params QField[] containedFields )
		{
			title = paramTitle;
			fields = containedFields;
		}

		public void SetProperties( string parentID, int paramID, QNodeType nodeType )
		{
			parentid = parentID;
			paramid = paramID;
			type = nodeType;

			// Set properties of each field
			int fid = 0;
            foreach (QField field in fields)
            {
                field.SetProperties(ID, paramID, fid++, type);
            }
		}

		protected override void Render( System.Web.UI.HtmlTextWriter output )
		{
			string id = parentid + ";" + paramid.ToString();

			Panel rowPanel = new Panel();
			rowPanel.ID = id;
			rowPanel.CssClass = "row aqirow";

			Panel labelPanel = new Panel();
			rowPanel.Controls.Add( labelPanel );
			labelPanel.ID = id + ";Label";
			labelPanel.CssClass = "label aqilabel";
			if( type == QNodeType.AQINode )
			{
				labelPanel.Attributes.Add( "onclick", "AQI.ToggleParam('" + id + "')" );
				labelPanel.Attributes.Add( "title", "Click to include/exclude parameter" );
			}
			labelPanel.Controls.Add( ControlBuilder.BuildLiteral( title ) );

			Panel contentPanel = new Panel();
			rowPanel.Controls.Add( contentPanel );
			contentPanel.ID = id + ";Content";
			contentPanel.CssClass = "content";

			int fieldcount = 0;
			string fieldtype = string.Empty;
			foreach( QField field in fields )
			{
				// Render the field outline
				Label fieldOutline = new Label();
				contentPanel.Controls.Add( fieldOutline );
				fieldOutline.ID = id + "-" + fieldcount.ToString();
				fieldcount++;
				fieldOutline.Controls.Add( field );

				string[] fieldarray = field.GetType().ToString().Split( '.' );
				fieldtype = fieldarray[fieldarray.Length - 1];
			}

			// Add hidden field counter
			Label fieldPanel = new Label();
			contentPanel.Controls.Add( fieldPanel );
			fieldPanel.ID = id + ";Counter";
			fieldPanel.Attributes.Add( "style", "display:none" );
			fieldPanel.Text = fieldcount.ToString();
			
			// Render controls if requested
			if( type == QNodeType.AQINode )
			{
				HyperLink controlLink = new HyperLink();
				contentPanel.Controls.Add( controlLink );
				controlLink.NavigateUrl = "#";
				controlLink.Attributes.Add( "onclick", "AQI.AddField('" + id + "');return false" );
				controlLink.Text = " or...";

                //NOTE: additional fields & delete field buttons are added in javascript
			}

			rowPanel.RenderControl( output );
		}
	}
}