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
    ///		<filepath>PathwaysLib/Server/ServerMolecularEntityName.cs</filepath>
    ///		<creation>2005/07/05</creation>
    ///		<author>
    ///			<name>Brendan Elliott</name>
    ///			<initials>BE</initials>
    ///			<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: mustafa $</cvs_author>
    ///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerMolecularEntityName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Represents one of possibly many names associated with a molecular entity.
    /// </summary>
    #endregion
    public class ServerMolecularEntityName : ServerObject
	{
        #region Constructor, Destructor, ToString

		private ServerMolecularEntityName()
		{
		}

		/// <summary>
		/// ServerMolecularEntityName constructor
		/// </summary>
		/// <param name="data">
		/// A SoapMolecularEntityName object from which to create this object.
		/// </param>
        public ServerMolecularEntityName ( SoapMolecularEntityName data )
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
                    __DBRow = LoadRow( data.MolecularEntityId, data.Name);
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
		/// ServerMolecularEntityName constructor.
		/// </summary>
		/// <param name="data">
		/// The DBrow from which to initialize this object.
		/// </param>
        public ServerMolecularEntityName ( DBRow data )
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
        private static readonly string __TableName = "entity_name_lookups";
        #endregion

        #region Properties

		/// <summary>
		/// Get/set the MolecularEntityId.
		/// </summary>
        public Guid MolecularEntityId
        {
            get{return __DBRow.GetGuid("entity_id");}
            set{__DBRow.SetGuid("entity_id", value);}
        }

		/// <summary>
		/// Get/set the NameId.
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

        /// <summary>
        /// Get/set entity name type as a string.  Wraps TypeId.
        /// </summary>
		public string Type
		{
			get
			{
				return NameTypeManager.GetNameType(TypeId);
			}
			set
			{
				TypeId = NameTypeManager.GetNameTypeId(value); // (ac) fails if the string is not in the db!
			}
		}

        /// <summary>
        /// Get/set entity name type ID.
        /// </summary>
		public int TypeId
		{
			get{return __DBRow.GetInt("name_type_id");}
			set
			{
				__DBRow.SetInt("name_type_id", value);

			}
		}

        string name = null;
		/// <summary>
		/// Get/set the Name.
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
            SoapMolecularEntityName retval = (derived == null) ? 
                retval = new SoapMolecularEntityName() : retval = (SoapMolecularEntityName)derived;

            retval.MolecularEntityId   = this.MolecularEntityId;
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
            SoapMolecularEntityName p = o as SoapMolecularEntityName;

            this.MolecularEntityId = p.MolecularEntityId;
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
                "INSERT INTO " + __TableName + " (entity_id, name_id, name_type_id) VALUES (@entity_id, @name_id, @name_type_id);",
                "@entity_id", SqlDbType.UniqueIdentifier, MolecularEntityId,
                "@name_id", SqlDbType.UniqueIdentifier, NameId,
                "@name_type_id", SqlDbType.TinyInt, TypeId);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE entity_id = @entity_id AND name_id = @name_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, MolecularEntityId,
                "@name_id", SqlDbType.UniqueIdentifier, NameId);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET name_type_id = @name_type_id WHERE entity_id = @entity_id AND name_id = @name_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, MolecularEntityId,
                "@name_id", SqlDbType.UniqueIdentifier, NameId,
                "@name_type_id", SqlDbType.TinyInt, TypeId);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE entity_id = @entity_id AND name_id = @name_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, MolecularEntityId,
                "@name_id", SqlDbType.UniqueIdentifier, NameId);
        }
        #endregion

        #endregion

        #region Static Methods     

		/// <summary>
		/// Return all of the names for the supplied molecular entity id.
		/// </summary>
		/// <param name="molecularEntityId">
		/// The id of the molecular entity on which to do the lookup.
		/// </param>
		/// <returns>
		/// A list of names for the molecular entity with id molecularEntityId.
		/// </returns>
        public static ServerMolecularEntityName[] AllNames(Guid molecularEntityId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE entity_id = @entity_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, molecularEntityId);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach(DataSet d in ds)
                {
                    results.Add(new ServerMolecularEntityName( new DBRow( d ) ) );
                }
            }
            return (ServerMolecularEntityName[])results.ToArray(typeof(ServerMolecularEntityName));
        }

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
//            ServerMolecularEntityName n = null;
//            if (Exists(molecularEntityId, name))
//            {
//                // update existing record
//                n = Load(molecularEntityId, name);
//                n.Type = "primary name";                
//            }
//            else
//            {
//                // create new name as primary
//                SoapMolecularEntityName newData = new SoapMolecularEntityName(molecularEntityId, name, "primary name");
//                n = new ServerMolecularEntityName(newData);
//            }
//            n.UpdateDatabase();
//        }

		/// <summary>
		/// Load a MolecularEntityName from its id and name.
		/// </summary>
		/// <param name="molecularEntityId">
		/// The supplied id.
		/// </param>
		/// <param name="name">
		/// The supplied name.
		/// </param>
		/// <returns>
		/// A SErverMolecularEntityName object corresponding to the input parameters.
		/// </returns>
        public static ServerMolecularEntityName Load ( Guid molecularEntityId, string name )
        {
            return new ServerMolecularEntityName ( LoadRow(molecularEntityId, name) );
        }

		/// <summary>
		/// Does this MolecularEntityName exist?
		/// </summary>
		/// <param name="molecularEntityId">
		/// The id to check.
		/// </param>
		/// <param name="name">
		/// The name to check.
		/// </param>
		/// <returns>
		/// Existence of a molecularentityname with corresponding parameters.
		/// </returns>
        public static bool Exists ( Guid molecularEntityId, string name )
        {
            Guid nameId = EntityNameManager.LookupId(name);

            if (nameId == Guid.Empty)
                return false;

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE entity_id = @entity_id AND name_id = @name_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, molecularEntityId,
                "@name_id", SqlDbType.UniqueIdentifier, nameId);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;

        }

        private static DBRow LoadRow ( Guid molecularEntityId, string name )
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE entity_id = @entity_id AND name_id = @name_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, molecularEntityId,
                "@name_id", SqlDbType.UniqueIdentifier, EntityNameManager.LookupId(name));

            DataSet ds;
            DBWrapper.LoadSingle( out ds, ref command );
            return new DBRow(ds);
        }

		/// <summary>
		/// Persist the ServerMolecularEntityNAme to the database.
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
	$Id: ServerMolecularEntityName.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerMolecularEntityName.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.10  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.9  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.8  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.7  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.6  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.5  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.4  2005/07/11 21:01:52  brendan
	Inheritance now working for Gene/MolecularEntity and the ServerObjectInheritance tests completes successfully.
	
	Revision 1.3  2005/07/08 21:55:07  brendan
	Debugging MolecularEntity/EntityNames/Gene inheritance.  Inheritance test not passing yet.
	
	Revision 1.2  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.1  2005/07/07 23:30:51  brendan
	Work in progress on entity names.  MolecularEntityName virtually complete, but not tested.
	
------------------------------------------------------------------------*/
#endregion