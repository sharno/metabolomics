using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class QInputCheckboxRenderer : QInputRenderer
    {
        IAQIUtil _util;

        public QInputCheckboxRenderer(IAQIUtil util)
            : base()
        {
            _util = util;
        }

        public override IGuiComponent Render(QNode rootNode, IGuiData data)
        {
            GuiAQIBasicInputData guiData = (GuiAQIBasicInputData) data;
            IDLabel inputLabel = QNodeRendererUtilities.InputCheckboxLabel(guiData.Input, guiData.FieldId, _util);

            return new GuiAQIBasicInput(inputLabel);
        }
    }
}