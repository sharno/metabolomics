using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A wrapper for a node renderer
    /// </summary>
    public abstract class QNodeRenderer : IRenderer
    {
        protected QNodeRenderer()
        { }

        public abstract IGuiComponent Render(QNode rootNode, IGuiData data);
    }
}