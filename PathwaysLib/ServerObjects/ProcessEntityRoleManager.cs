#region Using Declarations
using System;
using System.Collections;
using System.Data;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Utilities/ProcessEntityRoleManager.cs</filepath>
    ///		<creation>2006/04/19</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>bde</initials>
    ///			<email>bxe7@case.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name></name>
    ///				<initials></initials>
    ///				<email></email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ProcessEntityRoleManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Manages the process_entity_roles lookup table.
    /// </summary>
    #endregion
    public class ProcessEntityRoleManager
	{
		private ProcessEntityRoleManager()
		{
		}

        static NameIdManager manager = new NameIdManager("process_entity_roles", "name", "role_id", SqlDbType.TinyInt);

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GetRoleName(int roleId)
        {
            return manager.GetName(roleId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static int GetRoleId(string roleName)
        {
            return manager.GetNameId(roleName);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetRoleId(string roleName, bool create)
        {
            return manager.GetNameId(roleName, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool DeleteRoleName(string roleName)
        {
            return manager.DeleteName(roleName);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="roleId"></param>
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
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ProcessEntityRoleManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ProcessEntityRoleManager.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.1  2006/04/19 19:14:46  brendan
	Added classes for managing name/id lookup tables, such as molecular_entity_types, etc.
	
------------------------------------------------------------------------*/
#endregion