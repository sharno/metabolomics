using AQILib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class GuiAQIBasicInputData : IGuiData
    {
        private QInput _input;
        private string _fieldId;

        public QInput Input
        {
            get { return _input; }
        }

        public string FieldId
        {
            get { return _fieldId; }
        }

        private GuiAQIBasicInputData()
        { }

        public GuiAQIBasicInputData(QInput input, string fieldId)
        {
            _input = input;
            _fieldId = fieldId;
        }
    }
}