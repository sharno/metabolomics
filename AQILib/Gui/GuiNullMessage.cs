using AQILib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class GuiNullMessage : IGuiComponent
    {
        private string _message;

        public string Message
        {
            get { return _message; }
        }

        private GuiNullMessage()
        { }

        public GuiNullMessage(string message)
        {
            _message = message;
        }
    }
}