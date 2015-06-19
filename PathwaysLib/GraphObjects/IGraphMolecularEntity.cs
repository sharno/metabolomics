using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphMolecularEntity
    {
        /// <summary>
        /// Get/set the molecular entity ID.
        /// This is virtual so the derived class
        /// can override it to change the value in both rows.
        /// </summary>
        Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the molecular entity name.
        /// </summary>
        string Name
        {
            get;
            set;
        }

    }
}
