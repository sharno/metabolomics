using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphCompartment
    {
        Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the compartment name.
        /// </summary>
        string Name
        {
            get;
            set;
        }
        string sbmlID
        {
            get;
            set;
        }

        double Size
        {
            get;
            set;
        }
        
        Guid CompartmentTypeId
        {
            get;
            set;
        }

        int SpatialDimensions
        {
            get;
            set;
        }

        bool Constant
        {
            get;
            set;
        }

        Guid Outside
        {
            get;
            set;
        }
    }
}
