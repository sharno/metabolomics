namespace PathwaysWeb.Utilities
{
    using AQILib;
    using AQILib.Gui;

	using System;
	using System.Collections;
    using System.Reflection;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;
    using PathwaysLib.AQI;
	using PathwaysLib.WebControls;
    using System.Collections.Generic;

	public partial class AQI : System.Web.UI.UserControl
	{
        private void Page_Load(object sender, System.EventArgs e)
        {
            // Initialize the types of nodes for the renderer
            QNode.InitializeTypes(Assembly.GetAssembly(typeof(QNodePathway)));

            // Set up the page by setting the title and adding the two rendered panels to the page
            Page.Title = "PathCase - Advanced Query Interface";

            AQIWebInterface.CreateAQIOnPage(AQIQuery, AQIResults, AQITips, this.Request.Params["xml"], PathwaysAQIUtil.Instance);
        }
	}
}