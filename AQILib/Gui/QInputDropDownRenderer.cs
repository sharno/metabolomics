using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class QInputDropDownRenderer : QInputRenderer
    {
        IAQIUtil _util;

        public QInputDropDownRenderer(IAQIUtil util)
            : base()
        {
            _util = util;
        }

        public override IGuiComponent Render(QNode rootNode, IGuiData data)
        {
            GuiAQIBasicInputData guiData = (GuiAQIBasicInputData) data;
            IDLabel inputLabel = QNodeRendererUtilities.InputDropDownLabel(guiData.Input, guiData.FieldId, _util);

            return new GuiAQIBasicInput(inputLabel);
        }
    }
}