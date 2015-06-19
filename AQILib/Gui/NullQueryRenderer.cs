using AQILib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class NullQueryRenderer : IQueryRenderer
    {
        private IAQIUtil _util;

        private NullQueryRenderer()
        { }

        public NullQueryRenderer(IAQIUtil util)
        {
            this._util = util;
        }

        public IGuiComponent Render(QNode node, IQueryResults results, IGuiData data, out IGuiData dataOut)
        {
            dataOut = null;

            // Are there no nodes in the query? (If Render is called with a null IQueryResults, it should be directly from the AjaxHandler and not from any other code.)
            if(results == null)
                return new GuiNullMessage("Please build your query.");
            else
                return new GuiNullMessage("Invalid inter-code call to the null query renderer on receipt of a non-null data variable.");
        }
    }
}