using AQILib;
using PathwaysLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI
{
    /// <summary>
    /// Stores data that is provided to the GUI
    /// </summary>
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