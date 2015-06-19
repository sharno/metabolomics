using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphSpecies
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
        
        string SbmlId
        {
            get;
            set;
        }

        Guid SpeciesTypeId
        {
            get;
            set;
        }

        double InitialAmount
        {
            get;
            set;
        }

        double InitialConcentration
        {
            get;
            set;
        }


        Guid SubstanceUnitsId
        {
            get;
            set;
        }

        bool HasOnlySubstanceUnits
        {
            get;
            set;
        }

        bool BoundaryCondition
        {
            get;
            set;
        }

        int Charge
        {
            get;
            set;
        }

        bool Constant
        {
            get;
            set;
        }

    }
}
