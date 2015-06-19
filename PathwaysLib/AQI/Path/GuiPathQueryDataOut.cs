using AQILib;
using PathwaysLib.GraphSources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using PathwaysLib.Utilities;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// Data that is passed out of the GUI renderer to the rest of the interface
    /// </summary>
    public class GuiPathQueryDataOut : IGuiData
    {
        private IGraphSource _gs;

        public IGraphSource GS
        {
            get { return _gs; }
        }

        private GuiPathQueryDataOut()
        { }

        public GuiPathQueryDataOut(IGraphSource gs)
        {
            _gs = gs;
        }


    }
}