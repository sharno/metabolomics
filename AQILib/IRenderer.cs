using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A wrapper for renderers
    /// </summary>
    public interface IRenderer
    {
        IGuiComponent Render(QNode rootNode, IGuiData data);
    }
}