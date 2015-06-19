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
    ///		<filepath>PathwaysLib/Utilities/RnaTypeManager.cs</filepath>
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
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/RnaTypeManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Manages the rna_types lookup table.
    /// </summary>
    #endregion
    public class RnaTypeManager
    {
        private RnaTypeManager()
        {
        }

        static NameIdManager manager = new NameIdManager("rna_types", "name", "rna_type_id", SqlDbType.TinyInt);

        /// <summary>
        /// Returns the name associated with an id.
        /// </summary>
        /// <param name="rnaTypeId"></param>
        /// <returns></returns>
        public static string GetRnaTypeName(int rnaTypeId)
        {
            return manager.GetName(rnaTypeId);
        }

        /// <summary>
        /// Returns the id associated with a name.
        /// </summary>
        /// <param name="rnaTypeName"></param>
        /// <returns></returns>
        public static int GetRnaTypeId(string rnaTypeName)
        {
            return manager.GetNameId(rnaTypeName);
        }

        /// <summary>
        /// Returns the id associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="rnaTypeName"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static int GetRnaTypeId(string rnaTypeName, bool create)
        {
            return manager.GetNameId(rnaTypeName, create);
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="rnaTypeName"></param>
        /// <returns></returns>
        public static bool DeleteRnaTypeName(string rnaTypeName)
        {
            return manager.DeleteName(rnaTypeName);
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="rnaTypeId"></param>
        /// <returns></returns>
        public static bool DeleteRnaTypeId(int rnaTypeId)
        {
            return manager.DeleteNameId(rnaTypeId);
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public static string[] AllRnaTypeNames
        {
            get
            {
                return manager.AllNames;
            }
        }

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public static int[] AllRnaTypeIds
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
	$Id: RnaTypeManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: RnaTypeManager.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.1  2006/04/19 19:14:46  brendan
	Added classes for managing name/id lookup tables, such as molecular_entity_types, etc.
	
------------------------------------------------------------------------*/
#endregion