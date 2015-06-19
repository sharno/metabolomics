using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A wrapper for an input renderer
    /// </summary>
    public abstract class QInputRenderer : IRenderer
    {
        protected QInputRenderer()
        { }

        public abstract IGuiComponent Render(QNode rootNode, IGuiData data);
    }
}