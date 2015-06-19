using AQILib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class GuiAQIBasicFieldData : IGuiData
    {
        private QField _field;
        private string _parentNodeID;

        public QField Field
        {
            get { return _field; }
        }

        public string ParentNodeID
        {
            get { return _parentNodeID; }
        }

        private GuiAQIBasicFieldData()
        { }

        public GuiAQIBasicFieldData(QField field, string parentNodeID)
        {
            _field = field;
            _parentNodeID = parentNodeID;
        }
    }
}