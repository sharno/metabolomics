using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A wrapper for a field renderer
    /// </summary>
    public abstract class QFieldRenderer : IRenderer
    {
        protected QFieldRenderer()
        { }

        public abstract IGuiComponent Render(QNode rootNode, IGuiData data);
    }
}