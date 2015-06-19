#region Using Declarations
using System;
using System.Collections;
using System.Data;
#endregion

namespace PathwaysLib.ServerObjects
{

    public class RuleTypeManager
    {
        private RuleTypeManager()
        {
        }

        static NameIdManager manager = new NameIdManager("RuleType", "type", "id", SqlDbType.TinyInt);

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static string GetTypeName(int typeId)
        {
            return manager.GetName(typeId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static int GetTypeId(string typeName)
        {
            return manager.GetNameId(typeName);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetTypeId(string typeName, bool create)
        {
            return manager.GetNameId(typeName, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static bool DeleteTypeName(string typeName)
        {
            return manager.DeleteName(typeName);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static bool DeleteTypeId(int typeId)
        {
            return manager.DeleteNameId(typeId);
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public static string[] AllTypes
        {
            get
            {
                return manager.AllNames;
            }
        }

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public static int[] AllTypeIds
        {
            get
            {
                return manager.AllNameIds;
            }
        }
    }
}

#region Change Log

#endregion
