using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class QNodeBasicRenderer : QNodeRenderer
    {
        private bool _showNumberInTitle;

        public QNodeBasicRenderer()
            : base()
        {
            _showNumberInTitle = true;
        }

        public QNodeBasicRenderer(bool showNumberInTitle)
            : base()
        {
            _showNumberInTitle = showNumberInTitle;
        }

        public override IGuiComponent Render(QNode rootNode, IGuiData data)
        {
            GuiAQIBasicData guiData = (GuiAQIBasicData) data;
            string nodeId;
            Literal nodeCollapsedScript;
            IDPanel titlePanel;
            IDPanel contentPanel;
            QNodeRendererUtilities.QueryContentPanel(guiData.RootNode, guiData.IdCounter, guiData.ParentNodeID, _showNumberInTitle, out nodeId, out nodeCollapsedScript, out titlePanel, out contentPanel, guiData.Util);

            return new GuiAQIBasic(nodeId, nodeCollapsedScript, titlePanel, contentPanel);
        }
    }
}