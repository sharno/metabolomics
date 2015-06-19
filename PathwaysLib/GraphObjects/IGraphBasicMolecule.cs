using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphBasicMolecule : IGraphMolecularEntity
    {
        /// <summary>
        /// Get/set whether the molecule is common or not.  This wraps the common_molecules database table.
        /// </summary>
        bool IsCommon
        {
            get;
            set;
        }
    }
}
