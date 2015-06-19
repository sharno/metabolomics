using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class QNodeRootRenderer : QNodeRenderer
    {
        public override IGuiComponent Render(QNode rootNode, IGuiData data)
        {
            if(data == null)
                throw new ArgumentNullException("Parameter 'data' is null.");

            GuiAQIRootData guiData = null;
            try
            {
                guiData = (GuiAQIRootData) data;
            }
            catch
            {
                throw new ArgumentException("Parameter 'data' is not of type GuiAQIRootData.");
            }

            IDCollapsiblePanel queryPanel = new IDCollapsiblePanel();
            {
                queryPanel.ID = "root";
                queryPanel.Title = "Advanced Query Interface";
                queryPanel.Buttons.Add(ControlBuilder.BuildLiteral(@"<a href=""#"" onclick=""AQI.SubmitQuery();return false"">Submit query</a>"));
                queryPanel.Buttons.Add(ControlBuilder.BuildLiteral(@" | "));
                queryPanel.Buttons.Add(ControlBuilder.BuildLiteral(@"<a href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br"" onclick=""return newWin(this)"">AQI Help</a>"));

                IDPanel queryContentPanel = new IDPanel();
                {
                    queryContentPanel.ID = "root;Content";
                    queryContentPanel.CssClass = "whitebg";

                    Dictionary<string, IDPanel> linkPanels = new Dictionary<string, IDPanel>();
                    {
                        foreach(KeyValuePair<string, QLink> kvp in rootNode.Links)
                            linkPanels.Add(kvp.Key, QNodeRendererUtilities.LinkPanel(rootNode, kvp.Value, rootNode.NodeTypeName, guiData.Util));
                    }

                    Dictionary<string, List<IDPanel>> childPanels = new Dictionary<string, List<IDPanel>>();
                    {
                        foreach(QNode child in rootNode.Children)
                        {
                            IDPanel childPanel = ((GuiAQIBasic) child.RenderNode(new GuiAQIBasicData(child, guiData.IdCounter, rootNode.NodeTypeName, guiData.Util))).QueryContentPanel;
                            if(!childPanels.ContainsKey(child.NodeTypeName))
                                childPanels.Add(child.NodeTypeName, new List<IDPanel>());
                            childPanels[child.NodeTypeName].Add(childPanel);
                        }
                    }

                    string idPtr = String.Empty;
                    while(linkPanels.ContainsKey(idPtr))
                    {
                        queryContentPanel.Controls.Add(linkPanels[idPtr]);

                        foreach(string linkedNodeType in rootNode.Links[idPtr].LinkedNodeTypes)
                            if(childPanels.ContainsKey(linkedNodeType))
                                foreach(IDPanel childPanel in childPanels[linkedNodeType])
                                    queryContentPanel.Controls.Add(childPanel);

                        idPtr = rootNode.Links[idPtr].LinkTypeId;
                    }

                    //IDPanel queryAddPanel;
                    //bool useAddPanel;
                    //{
                    //    useAddPanel = QNodeRendererUtilities.AddLinksPanel(null, String.Empty, out queryAddPanel, guiData.Util);
                    //    if(useAddPanel)
                    //        queryAddPanel.ID = "root;Add";
                    //}

                    //if(useAddPanel)
                    //    queryContentPanel.Controls.Add(queryAddPanel);
                }

                IDPanel querySubmitPanel = new IDPanel();
                {
                    querySubmitPanel.ID = "aqisubmit";

                    HyperLink submitLink = new HyperLink();
                    {
                        submitLink.NavigateUrl = "#";
                        submitLink.Attributes.Add("onclick", "AQI.SubmitQuery();return false");
                        submitLink.Text = "Submit query";
                    }

                    querySubmitPanel.Controls.Add(submitLink);
                }

                queryPanel.Controls.Add(queryContentPanel);
                queryPanel.Controls.Add(querySubmitPanel);
            }

            IDPanel resultsPanel = new IDPanel();
            {
                resultsPanel.ID = "aqiresults;Content";
                resultsPanel.CssClass = "whitebg";
                resultsPanel.Controls.Add(ControlBuilder.BuildLiteral("Please build your query."));
            }

            IDPanel tipsPanel = new IDPanel();
            {
                tipsPanel.ID = "aqitips;Content";
                tipsPanel.CssClass = "whitebg";
                tipsPanel.Controls.Add(ControlBuilder.BuildLiteral(@"
                    <ul style=""line-height: 1em;"">
                        <li style=""text-indent: 0em; margin-bottom: 0.6em;"">To start a query, select one of the blue links above under the ""Search for"" heading. You can start a <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#relationship"">relationship query</a>, a <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#path2"">neighborhood query</a>, or a <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#path3"">path query</a>.</li>
                        <li style=""text-indent: 0em; margin-bottom: 0.6em;"">In <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#relationship"">relationship queries</a>, you can highlight the titles of the fields in red to indicate that you wish to have those fields sent to the results for output.</li>
                        <li style=""text-indent: 0em; margin-bottom: 0.6em;"">When using the drop-down auto-completing boxes, most of these boxes will filter themselves based on the current query so that they do not suggest results that will lead to no results.</li>
                        <li style=""text-indent: 0em;"">Click the following links for examples of <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#relationship3"">relationship queries</a>, <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#path6.1"">neighborhood queries</a>, or <a onclick=""return newWin(this)"" href=""LinkForwarder.aspx?rid=Help_AQI&rtype=br#path6.3"">path queries</a>.</li>
                    </ul>
                    <script type=""text/javascript"">
                        toggleRegion('AQITips','','','block',true);
                    </script>"));
            }

            guiData.Results.Title = "Query Results";
            guiData.Results.Collapsed = false;

            guiData.Tips.Title = "Query Tips";
            guiData.Tips.Collapsed = false;

            return new GuiAQIRoot(queryPanel, resultsPanel, tipsPanel);
        }
    }
}