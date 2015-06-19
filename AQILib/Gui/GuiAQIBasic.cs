using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class GuiAQIBasic : IGuiComponent
    {
        private string _nodeId;
        private Literal _nodeCollapsedScript;
        private IDPanel _titlePanel;
        private IDPanel _contentPanel;

        public String NodeId
        {
            get { return _nodeId; }
        }

        public Literal NodeCollapsedScript
        {
            get { return _nodeCollapsedScript; }
        }

        public IDPanel TitlePanel
        {
            get { return _titlePanel; }
        }

        public IDPanel ContentPanel
        {
            get { return _contentPanel; }
        }

        public IDPanel QueryContentPanel
        {
            get
            {
                IDPanel queryPanel = new IDPanel();
                {
                    // Add any style property changes to aqi.js!
                    // This is because the new node is created in the javascript on the client and this is used as a dummy placeholder.
                    queryPanel.ID = this.NodeId;
                    queryPanel.CssClass = "aqinode";
                    if(this.NodeId.Contains(":"))
                        queryPanel.Style.Add(HtmlTextWriterStyle.MarginLeft, "20px");
                    queryPanel.Attributes.Add("domNodeType", "node");

                    queryPanel.Controls.Add(this.NodeCollapsedScript);
                    queryPanel.Controls.Add(this.TitlePanel);
                    queryPanel.Controls.Add(this.ContentPanel);
                }

                return queryPanel;
            }
        }

        private GuiAQIBasic()
        { }

        public GuiAQIBasic(string nodeId, Literal nodeCollapsedScript, IDPanel titlePanel, IDPanel contentPanel)
        {
            _nodeId = nodeId;
            _nodeCollapsedScript = nodeCollapsedScript;
            _titlePanel = titlePanel;
            _contentPanel = contentPanel;
        }
    }
}