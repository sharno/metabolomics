#region Using Declarations
using System;
using System.Collections;
using System.Data;
#endregion

namespace PathwaysLib.ServerObjects
{

    public class UnitDefinitionManager
    {
        private UnitDefinitionManager()
        {
        }

        static NameIdManager manager = new NameIdManager("UnitDefinition", "name", "id", SqlDbType.UniqueIdentifier, "where modelId is null");

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static string GetBaseUnitName(Guid typeId)
        {
            return manager.GetName(typeId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Guid GetBaseUnitId(string typeName)
        {
            return manager.GetGuidNameId(typeName);
        }                
    }
}

#region Change Log

#endregion
