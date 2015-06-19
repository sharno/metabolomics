using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphOrganismGroup
    {
        /// <summary>
        /// Get/set the organism group ID.
        /// </summary>
        Guid ID
        {
            get;
            set;
        }
    }
}
