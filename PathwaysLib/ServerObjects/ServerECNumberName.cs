#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerECNumberName.cs</filepath>
	///		<creation>2005/07/05</creation>
	///		<author>
	///			<name>Brendan Elliott</name>
	///			<initials>BE</initials>
	///			<email>bxe7@cwru.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brandon S. Evans</name>
	///				<initials>bse</initials>
	///				<email>brandon.evans@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerECNumberName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents one of possibly many names associated with an ec number.
	/// </summary>
	#endregion
	public class ServerECNumberName : ServerObject
	{
		#region Constructor, Destructor, ToString

		/// <summary>
		/// Constructor
		/// </summary>
		private ServerECNumberName()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data"></param>
		public ServerECNumberName ( SoapECNumberName data )
		{
			// (BE) setup database row
			switch(data.Status)
			{
				case ObjectStatus.Insert:
					// not yet in DB, so create empty row
					__DBRow = new DBRow( __TableName );
					break;
				case ObjectStatus.ReadOnly:
				case ObjectStatus.Update:
				case ObjectStatus.NoChanges:
					// need to load existing row first so update works properly
					__DBRow = LoadRow( data.ECNumber, data.Name);
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}

			// (BE) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);

			// (mfs)
			// required call to setup SqlCommands
			//SetSqlCommandParameters( ); // (BE) moved call to ServerObject.UpdateDatabase()
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data"></param>
		public ServerECNumberName ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

			// (mfs)
			// required call to setup SqlCommands
			//SetSqlCommandParameters( ); // (BE) moved call to ServerObject.UpdateDatabase()
		}

		#endregion

		#region Member Variables
		private static readonly string __TableName = "ec_number_name_lookups";
		#endregion

		#region Properties

		/// <summary>
		/// Get/set the EC number
		/// </summary>
		public string ECNumber
		{
			get{return __DBRow.GetString("ec_number");}
			set{__DBRow.SetString("ec_number", value);}
		}

		/// <summary>
		/// get set the name id
		/// </summary>
		public Guid NameId
		{
			get{return __DBRow.GetGuid("name_id");}
			set
			{
				__DBRow.SetGuid("name_id", value);
				name = null;
			}
		}

		string name = null;
		/// <summary>
		/// Get/set the name
		/// </summary>
		public string Name
		{
			get
			{
				if (name == null)
					name = EntityNameManager.LookupName(__DBRow.GetGuid("name_id"));
				return name;
			}
			set
			{
				// db row updated in Update
				NameId = Guid.Empty; // set by string, lookup Guid on save
				name = value;     
			}
		}

		/// <summary>
		/// Get/set the type
		/// (as of 7/18/05) the type for all ec_number_names is primary
		/// 
		/// Now wraps TypeId.
		/// </summary>
		public string Type
		{
			get
			{
				return NameTypeManager.GetNameType(TypeId);
			}
			set
			{
				TypeId = NameTypeManager.GetNameTypeId(value); //(ac) fails if the value does not exist in db
			}
		}

        /// <summary>
        /// Get/set ECNumber type ID.
        /// </summary>
		public int TypeId
		{
			get{return __DBRow.GetInt("name_type_id");}
			set{__DBRow.SetInt("name_type_id", value);}
		}
		#endregion

		#region Methods

		/// <summary>
		/// Returns a representation of this object suitable for being
		/// sent to a client via SOAP.
		/// </summary>
		/// <returns>
		/// A SoapObject object capable of being passed via SOAP.
		/// </returns>
		public override SoapObject PrepareForSoap ( SoapObject derived )
		{
			SoapECNumberName retval = (derived == null) ? 
				retval = new SoapECNumberName() : retval = (SoapECNumberName)derived;

			retval.ECNumber   = this.ECNumber;
			retval.Name = this.Name;
			retval.Type = this.Type;

			retval.Status = ObjectStatus.NoChanges;

			return retval;
		}

		/// <summary>
		/// Consumes a SoapObject object and updates the object
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the object.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
			SoapECNumberName p = o as SoapECNumberName;

			this.ECNumber = p.ECNumber;
			this.Name = p.Name;
			this.Type = p.Type;
		}

		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (BE) rewrote using BuildCommand()

			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + " (ec_number, name_id, name_type_id) VALUES (@ec_number, @name_id, @name_type_id);",
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@name_id", SqlDbType.UniqueIdentifier, NameId,
				"@name_type_id", SqlDbType.TinyInt, TypeId);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number AND name_id = @name_id;",
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@name_id", SqlDbType.UniqueIdentifier, NameId);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + " SET name_type_id = @name_type_id WHERE ec_number = @ec_number AND name_id = @name_id;",
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@name_id", SqlDbType.UniqueIdentifier, NameId,
				"@name_type_id", SqlDbType.TinyInt, TypeId);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE ec_number = @ec_number AND name_id = @name_id;",
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@name_id", SqlDbType.UniqueIdentifier, NameId);
		}
		#endregion

		#endregion

		#region Static Methods     
        /// <summary>
        /// returns all the names for a given ec number from the ec_number_names table
        /// </summary>
        /// <param name="ecNumber"></param>
        /// <returns></returns>
		public static ServerECNumberName[] AllNames(string ecNumber)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number;",
				"@ec_number", SqlDbType.VarChar, ecNumber);

			ArrayList results = new ArrayList();
			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
			{
				foreach(DataSet d in ds)
				{
					results.Add(new ServerECNumberName( new DBRow( d ) ) );
				}
			}
			return (ServerECNumberName[])results.ToArray(typeof(ServerECNumberName));
		}

		//TODO: put a similar function in ServerECNumber
		// Now handled in object wrapper at ServerMolecularEntity.RemoveOldPrimaryNames()
		//        /// <summary>
		//        /// Updates the entity_name_lookups table to reflect a change in the primary name of a molecular entity.
		//        /// Note, this does not update the 'name' field in the molecular_entities table.
		//        /// This function is designed to be called from ServerMolecularEntity.UpdateDatabase().
		//        /// </summary>
		//        /// <param name="molecularEntityId"></param>
		//        /// <param name="name"></param>
		//        public static void SetPrimaryName(Guid molecularEntityId, string name)
		//        {
		//            // set any other named labeled as 'primary name' to 'other name'
		//            DBWrapper.Instance.ExecuteNonQuery(
		//                "UPDATE " + __TableName + " SET type='other name' WHERE entity_id=@entity_id AND type='primary name'",
		//                "@entity_id", SqlDbType.UniqueIdentifier, molecularEntityId);
		//
		//            ServerECNumberName n = null;
		//            if (Exists(molecularEntityId, name))
		//            {
		//                // update existing record
		//                n = Load(molecularEntityId, name);
		//                n.Type = "primary name";                
		//            }
		//            else
		//            {
		//                // create new name as primary
		//                SoapECNumberName newData = new SoapECNumberName(molecularEntityId, name, "primary name");
		//                n = new ServerECNumberName(newData);
		//            }
		//            n.UpdateDatabase();
		//        }

		/// <summary>
		/// Loads a single ServerECNumberName object for a given ec number and name
		/// </summary>
		/// <param name="ecNumber"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ServerECNumberName Load ( string ecNumber, string name )
		{
			return new ServerECNumberName ( LoadRow(ecNumber, name) );
		}

		/// <summary>
		/// Returns true if an entry with the given name and ec number exists in the
		/// ec_number_names table, otherwise returns false
		/// </summary>
		/// <param name="ecNumber"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool Exists ( string ecNumber, string name )
		{
			Guid nameId = EntityNameManager.LookupId(name);

			if (nameId == Guid.Empty)
				return false;

			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number AND name_id = @name_id;",
				"@ec_number", SqlDbType.VarChar, ecNumber,
				"@name_id", SqlDbType.UniqueIdentifier, nameId);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;

		}

		/// <summary>
		/// Return the DBRow for a given ec number and name.
		/// </summary>
		/// <param name="ecNumber"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( string ecNumber, string name )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number AND name_id = @name_id;",
				"@ec_number", SqlDbType.VarChar, ecNumber,
				"@name_id", SqlDbType.UniqueIdentifier, EntityNameManager.LookupId(name));

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		/// <summary>
		/// Persist the ECNumberName to the database.
		/// </summary>
		public override void UpdateDatabase ( )
		{
			if (this.NameId == Guid.Empty && name != null)
			{
				__DBRow.SetGuid("name_id", EntityNameManager.AddName(name));
			}
			base.UpdateDatabase();
		}

		#endregion
	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerECNumberName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerECNumberName.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.6  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.5  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.4  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.3  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.2  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.1  2005/07/15 14:30:43  brandon
	Added ECNumberName objects and made changes to ServerECNumber for name lookups.
	Also added functions for common molecules in BasicMolecule objects
	
	
------------------------------------------------------------------------*/
#endregion