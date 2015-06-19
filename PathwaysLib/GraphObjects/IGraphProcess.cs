using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.Utilities;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphProcess
    {
        #region Properties

        /// <summary>
        /// Get the Process name.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the Process reversibility.
        /// </summary>
        Tribool Reversible
        {
            get;
        }

        /// <summary>
        /// Get/set the Process ID or GenericProcess ID.
        /// </summary>
        Guid ID
        {
            get;
        }

        /// <summary>
        /// Get/set the generic process id.
        /// </summary>
        Guid GenericProcessID
        {
            get;
        }

        /// <summary>
        /// Returns true if this is a generic process (i.e. a union
        /// of specific processes with the same GenericProcessId).
        /// This also implies that ID is the same as GenericProcessID if true.
        /// </summary>
        bool IsGenericProcess
        {
            get;
        }

        bool IsTransport
        {
            get;
        }
        #endregion       
    }
}
