using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphReaction
    {
        Guid ID
        {
            get;
            set;
        }

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

        bool Reversible
        {
            get;
            set;
        }

        bool Fast
        {
            get;
            set;
        }

        Guid KineticLawId
        {
            get;
            set;
        }
    }
}
