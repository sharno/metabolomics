using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class GuiAQIBasicField : IGuiComponent
    {
        private IDPanel _fieldPanel;

        public IDPanel FieldPanel
        {
            get { return _fieldPanel; }
        }

        private GuiAQIBasicField()
        { }

        public GuiAQIBasicField(IDPanel fieldPanel)
        {
            _fieldPanel = fieldPanel;
        }
    }
}