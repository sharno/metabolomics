#region Using Declarations
using System;
using System.Data;

using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Server/EntityNameManager.cs</filepath>
    ///		<creation>2005/07/05</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>Suleyman Fatih Akgul</name>
    ///				<initials>sfa</initials>
    ///				<email>fatih@case.edu</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/EntityNameManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Manages the name table (i.e. molecular_entity_names) used for
    /// both Molecular Entities and E.C. numbers via static methods.
    /// </summary>
    #endregion	
    public class EntityNameManager
	{
		private EntityNameManager()
		{
		}

		/// <summary>
		/// Look up the entity name corresponding to the supplied Guid.
		/// </summary>
		/// <param name="id">
		/// The Guid of the item you desire the name for.
		/// </param>
		/// <returns>
		/// The entity name of the requested object.
		/// </returns>
        public static string LookupName(Guid id)
        {
            DataSet results;
            int r = DBWrapper.Instance.ExecuteQuery(out results, 
                "SELECT [name] FROM molecular_entity_names WHERE id = @id",
                    "@id", SqlDbType.UniqueIdentifier, id);

            if (r < 1)
                return null;

            return (string)results.Tables[0].Rows[0]["name"];
        }

		/// <summary>
		/// Look up the Guid for an object from its name.
		/// </summary>
		/// <param name="name">
		/// The entity name of the object.
		/// </param>
		/// <returns>
		/// The Guid of the object.
		/// </returns>
        public static Guid LookupId(string name)
        {
            DataSet results;
            int r = DBWrapper.Instance.ExecuteQuery(out results, 
                "SELECT [id] FROM molecular_entity_names WHERE [name] = @name",
                    "@name", SqlDbType.VarChar, name);

            if (r < 1)
                return Guid.Empty;

            return (Guid)results.Tables[0].Rows[0]["id"];
        }

		/// <summary>
		/// Does the supplied name exist as an entity name?
		/// </summary>
		/// <param name="name">
		/// The entity name to test for existence.
		/// </param>
		/// <returns>
		/// Existence of parameter name within the entity names table.
		/// </returns>
        public static bool NameExists(string name)
        {
            return LookupId(name) != Guid.Empty;
        }

		/// <summary>
		/// Does the supplied Guid exist in the entity names table?
		/// </summary>
		/// <param name="id">
		/// The id to test for existence.
		/// </param>
		/// <returns>
		/// Existence of parameter id within the entity names table.
		/// </returns>
        public static bool IdExists(Guid id)
        {
            return LookupName(id) != null;
        }

		/// <summary>
		/// Delete an entity name from a supplied name.
		/// </summary>
		/// <param name="name">
		/// The name to remove.
		/// </param>
        public static void DeleteByName(string name)
        {
			// (sfa) Don't we have to delete the entity_name_lookups instances here? 

			// Delete the entity name itself
            DBWrapper.Instance.ExecuteNonQuery("DELETE FROM molecular_entity_names WHERE [name] = @name",
                "@name", SqlDbType.VarChar, name);
        }

		/// <summary>
		/// Delete an entity name from its id.
		/// </summary>
		/// <param name="id">
		/// The id of the entity name to delete.
		/// </param>
        public static void DeleteById(Guid id)
        {
			// (sfa) Delete the entity_name_lookups instance first
			DBWrapper.Instance.ExecuteNonQuery("DELETE FROM entity_name_lookups WHERE name_id = @id",
				"@id", SqlDbType.UniqueIdentifier, id);

			// Delete the entity name itself
            DBWrapper.Instance.ExecuteNonQuery("DELETE FROM molecular_entity_names WHERE id = @id",
                "@id", SqlDbType.UniqueIdentifier, id);
        }

		/// <summary>
		/// Adds the given name to the molecular_entity_names table and returns an id.
		/// If the given name already exists in the table, its ID is returned.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
        public static Guid AddName(string name)
        {
            Guid g = LookupId(name);
            if (g != Guid.Empty)
                return g;

            g = DBWrapper.NewID();
            DBWrapper.Instance.ExecuteNonQuery("INSERT INTO molecular_entity_names (id, [name]) VALUES (@id, @name);",
                "@id", SqlDbType.UniqueIdentifier, g,
                "@name", SqlDbType.VarChar, name);

            return g;
        }
	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: EntityNameManager.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: EntityNameManager.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.7  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.6  2005/10/31 00:39:36  fatih
	*** empty log message ***
	
	Revision 1.5  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.4  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.3  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.2  2005/07/07 23:30:51  brendan
	Work in progress on entity names.  MolecularEntityName virtually complete, but not tested.
	
------------------------------------------------------------------------*/
#endregion