using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphReactionSpecies
    {
        Guid ID
        {
            get;
            set;
        }

        Guid SpeciesId
        {
            get;
            set;
        }
        short RoleId
        {
            get;
            set;
        }

        double Stoichiometry
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }
    }
}
