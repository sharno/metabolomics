#region Using Declarations
using System;
using System.Collections;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using System.Data;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Utilities/NameIdManager.cs</filepath>
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
    ///			<cvs_author>$Author: murat $</cvs_author>
    ///			<cvs_date>$Date: 2011/03/31 02:33:17 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/NameIdManager.cs,v 1.6 2011/03/31 02:33:17 murat Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.6 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Provides a generic implemention of a int id/string name lookup table manager.
    /// Designed to be used as a static member of a static manager class.
    /// 
    /// On first request, the table values are loaded and cached in a hashtable 
    /// for efficient in-memory value lookup later.  Designed for reasonably-sized
    /// mapping tables that can trivially be stored in main memory 
    /// (i.e. not millions/billions of entries).
    /// </summary>
    #endregion
    public class NameIdManager
	{
        /// <summary>
        /// Initializes the manager with the database table and stirng name/int id field names.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="nameField"></param>
        /// <param name="idField"></param>
        /// <param name="idFieldType"></param>
		public NameIdManager(string tableName, string nameField, string idField, SqlDbType idFieldType)
		{
            this.tableName = tableName;
            this.nameField = nameField;
            this.idField = idField;
            this.idFieldType = idFieldType;
            this.whereClause = "";
		}

        /// <summary>
        /// Initializes the manager with the database table and stirng name/int id field names.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="nameField"></param>
        /// <param name="idField"></param>
        /// <param name="idFieldType"></param>
        public NameIdManager(string tableName, string nameField, string idField, SqlDbType idFieldType, string whereClause)
        {
            this.tableName = tableName;
            this.nameField = nameField;
            this.idField = idField;
            this.idFieldType = idFieldType;
            this.whereClause = whereClause;
        }

        #region Member Variables

        string tableName;
        string nameField;
        string idField;
        string whereClause;
        SqlDbType idFieldType;
        bool loaded = false;
        Hashtable nameTable = new Hashtable();
        Hashtable idTable = new Hashtable();

        #endregion

        /// <summary>
        /// Returns the name associated with an id (int).
        /// </summary>
        /// <param name="nameId"></param>
        /// <returns></returns>
        public string GetName(int nameId)
        {
            if (!loaded || !idTable.ContainsKey(nameId))
                Load(); // not loaded or cache out of date?
            if (idTable.ContainsKey(nameId))
                return (string)idTable[nameId];
            throw new DataModelException("ID not found: {0}", nameId);
        }

        /// <summary>
        /// Returns the name associated with an id (Guid).
        /// </summary>
        /// <param name="nameId"></param>
        /// <returns></returns>
        public string GetName(Guid nameId)
        {
            if (!loaded || !idTable.ContainsKey(nameId))
                Load(); // not loaded or cache out of date?
            if (idTable.ContainsKey(nameId))
                return (string)idTable[nameId];
            throw new DataModelException("ID not found: {0}", nameId);
        }

        /// <summary>
        /// Returns the id (int) associated with a name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetNameId(string name)
        {
            return GetNameId(name, false);
        }       

        /// <summary>
        /// Returns the id (int) associated with a name, 
        /// optionally creating it if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public int GetNameId(string name, bool create)
        {
            if (name == null)
                throw new DataModelException("Null {0} not allowed!", nameField);

            if (!loaded || !nameTable.ContainsKey(name))
                Load(); // not loaded or cache out of date?
            if (nameTable.ContainsKey(name))
                return (int)nameTable[name];
            /*else // This code was active on 03/29/2011 which is meaningless : Murat Kurtcephe
                return Utilities.Util.NullPathwayTypeId;*/
            // When you uncomment the else above, it is not possible to create a name id if it does not exists in database
            // not found
            if (create)
            {
                // need to add to db
                lock(nameTable)
                {
                    DBWrapper db = DBWrapper.Instance;

                    if (db.ExecuteNonQuery("INSERT INTO " + tableName + " (" + nameField + ") VALUES (@name)", 
                        "@name", SqlDbType.VarChar, name) > 0)
                    {
                        int newId = int.Parse(db.ExecuteScalar("SELECT MAX(" + idField + ") FROM " + tableName + " WHERE " + nameField + " = @name",  
                            "@name", SqlDbType.VarChar, name).ToString());
                        nameTable.Add(name, newId);
                        idTable.Add(newId, name);
                        return newId;
                    }
                    else
                        throw new DataModelException("Insert failed: {0}", name);
                }
            }
            else
            {
                throw new DataModelException("Name not found: {0}", name);
            }
        }

        /// <summary>
        /// Returns the id (Guid) associated with a name,         
        /// </summary>
        /// <param name="name"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public Guid GetGuidNameId(string name)
        {
            if (name == null)
                throw new DataModelException("Null {0} not allowed!", nameField);

            if (!loaded || !nameTable.ContainsKey(name))
                Load(); // not loaded or cache out of date?
            if (nameTable.ContainsKey(name))
                return (Guid)nameTable[name];
            else
                return Guid.Empty;            
        }

        /// <summary>
        /// Remove a name (and it's id) from the lookup table.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteName(string name)
        {
            if (!loaded)
                Load();

            if (nameTable.ContainsKey(name))
            {
                idTable.Remove(nameTable[name]);
                nameTable.Remove(name);
            }

            DBWrapper db = DBWrapper.Instance;
            return db.ExecuteNonQuery("DELETE FROM " + tableName + " WHERE " + nameField + " = @name",
                "@name", SqlDbType.VarChar, name) > 0;
        }

        /// <summary>
        /// Remove an id (and it's name) from the lookup table.
        /// </summary>
        /// <param name="nameId"></param>
        /// <returns></returns>
        public bool DeleteNameId(int nameId)
        {
            if (!loaded)
                Load();

            if (idTable.ContainsKey(nameId))
            {
                nameTable.Remove(idTable[nameId]);
                idTable.Remove(nameId);
            }

            DBWrapper db = DBWrapper.Instance;
            return db.ExecuteNonQuery("DELETE FROM " + tableName + " WHERE " + idField + " = @idField",
                "@idField", idFieldType, nameId) > 0;
        }

        /// <summary>
        /// Return all names.
        /// </summary>
        public string[] AllNames
        {
            get
            {
                if (!loaded)
                    Load();
                ArrayList results = new ArrayList(nameTable.Keys);
                results.Sort(); // alphabetical order
                return (string[])results.ToArray(typeof(string));
            }
        }

        //ToBeDone : Guid version of the following method is required

        /// <summary>
        /// Return all name Ids
        /// </summary>
        public int[] AllNameIds
        {
            get
            {
                if (!loaded)
                    Load();
                int[] results = new int[nameTable.Count];
                string[] names = AllNames;
                for(int i = 0; i < names.Length; i++)
                {
                    results[i] = GetNameId(names[i]);
                }
                return results;
            }
        }
   
        private void Load()
        {
            lock(nameTable)
            {
                nameTable.Clear();
                idTable.Clear();

                DBWrapper db = DBWrapper.Instance;
                DataSet ds;
                if (db.ExecuteQuery(out ds, "SELECT " + idField + ", " + nameField + " FROM " + tableName + " " + whereClause) > 0)
                {
                    foreach(DataRow r in ds.Tables[0].Rows)
                    {
                        int id = -1;
                        Guid gid = Guid.Empty;
                        switch(idFieldType)
                        {
                            case SqlDbType.Int:
                                id = (int)r[idField];
                                break;
                            case SqlDbType.SmallInt:
                                id = (int)((short)r[idField]);
                                break;
                            case SqlDbType.TinyInt:
                                id = (int)((byte)r[idField]);
                                break;
                            case SqlDbType.UniqueIdentifier:
                                gid = (Guid)r[idField];
                                break;
                            default:
                                throw new DataModelException("Unexpected field type: {0}", idFieldType);
                        }
                        string name = ((string)r[nameField]).Trim();
                        if(nameTable.ContainsKey(name)) continue;
                        if (this.idFieldType == SqlDbType.UniqueIdentifier)
                        {
                            nameTable.Add(name, gid);
                            idTable.Add(gid, name);
                        }
                        else
                        {
                            nameTable.Add(name, id);
                            idTable.Add(id, name);
                        }
                    }
                }
                loaded = true;
            }
        }	

	}
}

#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: NameIdManager.cs,v 1.6 2011/03/31 02:33:17 murat Exp $
	$Log: NameIdManager.cs,v $
	Revision 1.6  2011/03/31 02:33:17  murat
	GetNameId function is fixed. With the fix, it not possible to create a name id if it does not exists in database
	
	Revision 1.5  2010/07/20 14:50:54  xjqi
	The kegg protein browser crash bug is fixed.
	
	Revision 1.4  2009/03/26 04:00:50  ali
	*** empty log message ***
	
	Revision 1.3  2009/02/11 04:45:44  ali
	*** empty log message ***
	
	Revision 1.2  2009/02/02 18:02:11  ali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2007/04/09 17:14:31  ali
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.1  2006/04/19 19:14:46  brendan
	Added classes for managing name/id lookup tables, such as molecular_entity_types, etc.
	
------------------------------------------------------------------------*/
#endregion