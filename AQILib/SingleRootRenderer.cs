using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    public class SingleRootRenderer : IQueryRenderer
    {
        public IGuiComponent Render(QNode node, IQueryResults results, IGuiData data, out IGuiData dataOut)
        {
            if(node.Children.Count > 0)
            {
                return node.Children[0].RenderQuery(results, data, out dataOut);
            }
            else
            {
                dataOut = null;
                return null;
            }
        }
    }
}