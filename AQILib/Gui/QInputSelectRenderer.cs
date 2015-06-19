using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class QInputSelectRenderer : QInputRenderer
    {
        IAQIUtil util;

        public QInputSelectRenderer(IAQIUtil util)
            : base()
        {
            this.util = util;
        }

        public override IGuiComponent Render(QNode rootNode, IGuiData data)
        {
            GuiAQIBasicInputData guiData = (GuiAQIBasicInputData) data;
            IDLabel inputLabel = QNodeRendererUtilities.InputSelectLabel(guiData.Input, guiData.FieldId, util);

            return new GuiAQIBasicInput(inputLabel);
        }
    }
}