using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class GuiAQIBasicInput : IGuiComponent
    {
        private IDLabel _inputLabel;

        public IDLabel InputLabel
        {
            get { return _inputLabel; }
        }

        private GuiAQIBasicInput()
        { }

        public GuiAQIBasicInput(IDLabel inputLabel)
        {
            _inputLabel = inputLabel;
        }
    }
}