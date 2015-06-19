using AQILib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace AQILib.Gui
{
    public class QFieldBasicRenderer : QFieldRenderer
    {
        public QFieldBasicRenderer()
            : base()
        { }

        public override IGuiComponent Render(QNode rootNode, IGuiData data)
        {
            GuiAQIBasicFieldData guiData = (GuiAQIBasicFieldData) data;
            IDPanel fieldPanel = QNodeRendererUtilities.FieldPanel(guiData.Field, guiData.ParentNodeID);

            return new GuiAQIBasicField(fieldPanel);
        }
    }
}