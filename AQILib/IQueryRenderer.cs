using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An interface for renderers of query results
    /// </summary>
    public interface IQueryRenderer
    {
        IGuiComponent Render(QNode node, IQueryResults results, IGuiData data, out IGuiData dataOut);
    }
}