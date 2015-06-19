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
    ///		<filepath>PathwaysLib/Utilities/PathwaysTypeManager.cs</filepath>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/PathwaysTypeManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Manages the pathway_types lookup table.
    /// </summary>
    #endregion
    public class PathwaysTypeManager
    {
        private PathwaysTypeManager()
        {
        }

        static NameIdManager manager = new NameIdManager("pathway_types", "name", "pathway_type_id", SqlDbType.TinyInt);

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="pathwayTypeId"></param>
        /// <returns></returns>
        public static string GetPathwayTypeName(int pathwayTypeId)
        {
            return manager.GetName(pathwayTypeId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="pathwayTypeName"></param>
        /// <returns></returns>
        public static int GetPathwayTypeId(string pathwayTypeName)
        {
            return manager.GetNameId(pathwayTypeName);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="pathwayTypeName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetPathwayTypeId(string pathwayTypeName, bool create)
        {
            return manager.GetNameId(pathwayTypeName, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="pathwayTypeName"></param>
        /// <returns></returns>
        public static bool DeletePathwayTypeName(string pathwayTypeName)
        {
            return manager.DeleteName(pathwayTypeName);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="pathwayTypeId"></param>
        /// <returns></returns>
        public static bool DeletePathwayTypeId(int pathwayTypeId)
        {
            return manager.DeleteNameId(pathwayTypeId);
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public static string[] AllPathwayTypeNames
        {
            get
            {
                return manager.AllNames;
            }
        }

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public static int[] AllPathwayTypeIds
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
	$Id: PathwaysTypeManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: PathwaysTypeManager.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.1  2006/04/19 19:14:46  brendan
	Added classes for managing name/id lookup tables, such as molecular_entity_types, etc.
	
------------------------------------------------------------------------*/
#endregion