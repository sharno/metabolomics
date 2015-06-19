namespace PathwaysWeb.Web
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Configuration;
	using System.Collections;
    using System.Collections.Generic;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;
    using PathwaysLib.AQI;
    using PathwaysLib.AQI.Path;
	using PathwaysLib.Exceptions;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Utilities;
	using PathwaysLib.WebControls;
    using PathwaysWeb.ContentPanels;
    using AQILib;
    using AQILib.Gui;
    using AQILib.Sql;

	/// <summary>
	/// Handles Ajax requests from various parts of the site, using reflection to figure out
	/// which operation needs to be done.
	/// </summary>
	public partial class AjaxHandler : System.Web.UI.Page
	{
		protected static AjaxMethodInvoker invoker = null;

		#region Initialization & Output

		protected void Page_Load( object sender, EventArgs e )
		{
			EventLogger.ComponentName = "AjaxHandler.aspx";
			EventLogger.Url = Request.Url.ToString();
			DBWrapper.Instance = new DBWrapper();

			Session.Timeout = 15;

			Response.ContentType = "text/xml";
			Response.Cache.SetCacheability( HttpCacheability.NoCache );

			if( invoker == null ) invoker = new AjaxMethodInvoker( this.GetType() );
			
			if( Request.Params["op"] == null || Request.Params["op"] == string.Empty )
			{
				#region Displaying of opcodes disabled due to security concerns
				// no op found
				//Error( "Methods:" );

				//foreach( MethodInfo mi in invoker.Methods )
				//{
				//    StringBuilder info = new StringBuilder();
				//    info.Append( mi.Name );
				//    info.Append( " (" );
				//    ParameterInfo[] paramList = mi.GetParameters();
				//    for( int i = 0; i < paramList.Length; i++ )
				//    {
				//        ParameterInfo param = paramList[i];
				//        info.Append( param.ParameterType.Name );
				//        info.Append( " " );
				//        info.Append( param.Name );

				//        if( i < paramList.Length - 1 )
				//            info.Append( ", " );
				//    }
				//    info.Append( ")" );

				//    WriteLine( info.ToString() );
				//}
				//return;
				#endregion

				Response.Write( "Access denied." );
				return;
			}

			try
			{
				invoker.Execute( this, Request.Params["op"], Request );
			}
			catch( TargetInvocationException tie )
			{
				EventLogger.SystemEventLog( "Unhandled exception: " + tie.ToString() );
				Error( tie.InnerException.ToString() );
			}
			catch( AjaxMethodException ame )
			{
				EventLogger.SystemEventLog( "Unhandled exception: " + ame.ToString() );
				Error( ame.Message );
			}
			catch( Exception ex )
			{
				EventLogger.SystemEventLog( "Unhandled exception: " + ex.ToString() );
				Error( ex.ToString() );
			}
		}

		private void Page_Unload( object sender, System.EventArgs e )
		{
			EventLogger.ComponentName = null;
		}

		/// <summary>
		/// Writes text to the response output.
		/// </summary>
		/// <param name="msg">The string to write</param>
		protected void Write( string msg )
		{
			Response.Write( msg );
		}

		/// <summary>
		/// Writes formatted text to the response output.
		/// </summary>
		/// <param name="msg">The formatted string to write</param>
		/// <param name="args">Parameters for msg</param>
		protected void Write( string msg, params object[] args )
		{
			Response.Write( string.Format( msg, args ) );
		}

		/// <summary>
		/// Writes a line of text (separated by an XHTML line break) to the output.
		/// </summary>
		/// <param name="msg">The string to write</param>
		protected void WriteLine( string msg )
		{
			Response.Write( msg + "<br />\n" );
		}

		/// <summary>
		/// Writes a formatted line of text to the output.
		/// </summary>
		/// <param name="msg">The formatted string to write</param>
		/// <param name="args">Parameters for msg</param>
		protected void WriteLine( string msg, params object[] args )
		{
			Response.Write( string.Format( msg, args ) + "<br />\n" );
		}

		/// <summary>
		/// Writes a formatted error message to the output.
		/// </summary>
		/// <param name="msg">The formatted error message to write</param>
		/// <param name="args">Parameters for msg</param>
		protected new void Error( string msg, params object[] args )
		{
			Response.Write( "ERROR: " + string.Format( msg, args ) + "<br />\n" );
		}

		/// <summary>
		/// Renders a control to the response output.
		/// </summary>
		/// <param name="control">The control to render</param>
		/// <returns></returns>
		private string RenderControl( System.Web.UI.Control control )
		{
			StringWriter sw = new StringWriter();
			HtmlTextWriter writer = new HtmlTextWriter( sw );
			control.RenderControl( writer );

			return sw.ToString();
		}

		#endregion



		/// <summary>
		/// Adds an AQI node to the tree.
		/// </summary>
		/// <param name="id">The node's ID</param>
		/// <param name="type">A numerical representation of the type</param>
		[AjaxMethod]
		public void AQIAdd(string nodeId, string xml)
        {
            QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));

            Write(AQIWebInterface.AQIAdd(PathwaysAQIUtil.Instance, nodeId, xml));
		}

        [AjaxMethod]
        public void AQIGenerate(string tree)
        {
            QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));

            IGuiData dataOut;
            string output = AQIWebInterface.AQIGenerate(PathwaysAQIUtil.Instance, tree, new GuiQueryData(new LinkHelper(Request)), out dataOut);

            Write(output);
            if(dataOut != null)
            {
                if(dataOut is GuiPathQueryDataOut)
                {
                    GuiPathQueryDataOut guiDataOut = (GuiPathQueryDataOut) dataOut;

                    if(guiDataOut.GS != null)
                    {
                        AQILib.Gui.CollapsiblePanel graphPanel = new AQILib.Gui.CollapsiblePanel();
                        {
                            //graphPanel.Style.Add("font-size", "1em");

                            GraphViewer graphViewerPanel = (GraphViewer) LoadControl("../ContentPanels/GraphViewer.ascx");
                            {
                                LinkHelper LH = new LinkHelper(Request);

                                Hashtable args = new Hashtable();
                                args["graphtype"] = "QueryNeighborhood";
                                args["id"] = ServerOrganism.UnspecifiedOrganism;
                                args["viewid"] = LH.GetParameter(LH.ParameterName);
                                args["load"] = "load";
                                args["gs"] = guiDataOut.GS;

                                graphPanel.Title = graphViewerPanel.BuildControl(args)["title"].ToString();
                                args["load"] = "load";
                                graphPanel.Arguments = args;
                                graphPanel.ControlName = "GraphViewer";
                            }

                            graphPanel.Controls.Add(graphViewerPanel);
                        }

                        Write(RenderControl(graphPanel));
                    }
                }
            }

        }

        [AjaxMethod]
        public void InSearch(string fieldId, string nodeType, string fieldTypeId, string inputType, string queryString, string treeXmlString, bool xmlFlag, bool quoteFlag)
        {
            QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));

            Write(AQIWebInterface.InSearch(PathwaysAQIUtil.Instance, Response, fieldId, nodeType, fieldTypeId, inputType, queryString, treeXmlString, xmlFlag, quoteFlag, 200));
        }

	}
}