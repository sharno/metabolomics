using AQILib;
using PathwaysLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI
{
    public class GuiQueryData : IGuiData
    {
        private LinkHelper _lh;

        public LinkHelper LH
        {
            get { return _lh; }
        }

        private GuiQueryData()
        { }

        public GuiQueryData(LinkHelper lh)
        {
            _lh = lh;
        }
    }
}