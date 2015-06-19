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
    ///		<filepath>PathwaysLib/Utilities/NameTypeManager.cs</filepath>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/NameTypeManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Manages the name_types lookup table.
    /// </summary>
    #endregion
    public class NameTypeManager
    {
        private NameTypeManager()
        {
        }

        static NameIdManager manager = new NameIdManager("name_types", "name", "name_type_id", SqlDbType.TinyInt);

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="nameTypeId"></param>
        /// <returns></returns>
        public static string GetNameType(int nameTypeId)
        {
            return manager.GetName(nameTypeId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="nameType"></param>
        /// <returns></returns>
        public static int GetNameTypeId(string nameType)
        {
            return manager.GetNameId(nameType);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="nameType"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetNameTypeId(string nameType, bool create)
        {
            return manager.GetNameId(nameType, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="nameType"></param>
        /// <returns></returns>
        public static bool DeleteNameType(string nameType)
        {
            return manager.DeleteName(nameType);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="nameTypeId"></param>
        /// <returns></returns>
        public static bool DeleteNameTypeId(int nameTypeId)
        {
            return manager.DeleteNameId(nameTypeId);
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public static string[] AllNameTypes
        {
            get
            {
                return manager.AllNames;
            }
        }

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public static int[] AllNameTypeIds
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
	$Id: NameTypeManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: NameTypeManager.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.1  2006/04/19 19:14:46  brendan
	Added classes for managing name/id lookup tables, such as molecular_entity_types, etc.
	
------------------------------------------------------------------------*/
#endregion