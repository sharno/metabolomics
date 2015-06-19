using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.GraphObjects
{
    public interface IGraphPathway
    {
        /// <summary>
        /// Get/set the Pathway ID.
        /// </summary>
        Guid ID
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the Pathway name.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Returns all organisms and organism groups that contain this pathway (i.e. contain one of its processes).
        /// </summary>
        /// <returns></returns>
        IGraphOrganismGroup[] GetAllIGraphOrganismGroups();

        /// <summary>
        /// Returns an array of the entities that link this pathway to other pathways
        /// </summary>
        /// <returns></returns>
        IGraphMolecularEntity[] GetLinkingEntities();
    }
}
