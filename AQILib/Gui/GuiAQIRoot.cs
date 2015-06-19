using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class GuiAQIRoot : IGuiComponent
    {
        private CollapsiblePanel _queryPanel;
        private Panel _resultsPanel;
        private Panel _tipsPanel;

        public CollapsiblePanel QueryPanel
        {
            get { return _queryPanel; }
        }

        public Panel ResultsPanel
        {
            get { return _resultsPanel; }
        }

        public Panel TipsPanel
        {
            get { return _tipsPanel; }
        }

        private GuiAQIRoot()
        { }

        public GuiAQIRoot(CollapsiblePanel queryPanel, Panel resultsPanel, Panel tipsPanel)
        {
            _queryPanel = queryPanel;
            _resultsPanel = resultsPanel;
            _tipsPanel = tipsPanel;
        }
    }
}