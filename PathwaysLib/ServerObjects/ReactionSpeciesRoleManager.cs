#region Using Declarations
using System;
using System.Collections;
using System.Data;
#endregion

namespace PathwaysLib.ServerObjects
{
   
    public class ReactionSpeciesRoleManager
    {
        private ReactionSpeciesRoleManager()
        {
        }

        static NameIdManager manager = new NameIdManager("ReactionSpeciesRole", "role", "id", SqlDbType.TinyInt);

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static string GetRoleName(int roleId)
        {
            return manager.GetName(roleId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static int GetRoleId(string roleName)
        {
            return manager.GetNameId(roleName);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetRoleId(string roleName, bool create)
        {
            return manager.GetNameId(roleName, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static bool DeleteRoleName(string roleName)
        {
            return manager.DeleteName(roleName);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static bool DeleteRoleId(int roleId)
        {
            return manager.DeleteNameId(roleId);
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public static string[] AllRoles
        {
            get
            {
                return manager.AllNames;
            }
        }

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public static int[] AllRoleIds
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