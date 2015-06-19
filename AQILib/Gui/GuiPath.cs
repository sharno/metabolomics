using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class GuiPath : IGuiComponent
    {
        private string _errorString;
        private Panel _gridPanel;

        public string ErrorString
        {
            get { return _errorString; }
        }

        public Panel GridPanel
        {
            get { return _gridPanel; }
        }

        public bool IsErrorResult
        {
            get { return _errorString != null; }
        }

        private GuiPath()
        { }

        public GuiPath(string errorString)
        {
            _errorString = errorString;
            _gridPanel = null;
        }

        public GuiPath(Panel gridPanel)
        {
            _errorString = null;
            _gridPanel = gridPanel;
        }
    }
}