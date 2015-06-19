using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// A basic edge implementation
    /// </summary>
    public interface IEdge : ICloneable
    {
        INode FromNode
        {
            get;
        }

        INode ToNode
        {
            get;
        }

        string Label
        {
            get;
        }

        //object Clone();
    }
}