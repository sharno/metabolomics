using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphProcessEntity
    {
        /// <summary>
        /// ID of Process in process entities relation
        /// </summary>
        Guid ProcessID
        {
            get;
            set;
        }

        /// <summary>
        /// ID of molecular entity in process entities relation
        /// </summary>
        Guid EntityID
        {
            get;
            set;
        }

        /// <summary>
        /// Entity role in a process as a string.  Wraps RoleId.
        /// </summary>
        string Role
        {
            get;
            set;
        }     
    }
}
