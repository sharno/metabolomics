using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphModel
    {
        Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the model name.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        string SbmlFile
        {
            get;
            set;
        }


    }
}
