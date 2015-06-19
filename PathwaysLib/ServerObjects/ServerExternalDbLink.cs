#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/ServerObjects/ServerExternalDbLinks.cs</filepath>
	///		<creation>2005/06/16</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Michael F. Starke</name>
	///				<initials>mfs</initials>
	///				<email>michael.starke@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@cwru.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerExternalDbLink.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to biological processes.
	/// </summary>
	#endregion
	public class ServerExternalDbLink : ServerObject
	{

		#region Constructor, Destructor, ToString
		private ServerExternalDbLink()
		{
		}

		/// <summary>
		/// Constructor for server external database link wrapper with fields initiallized
		/// </summary>
		/// <param name="localId"></param>
		/// <param name="externalDatabaseId"></param>
		/// <param name="idInExternalDb"></param>
		/// <param name="nameInExternalDb"></param>
		public ServerExternalDbLink ( Guid localId, int externalDatabaseId, string idInExternalDb, string nameInExternalDb )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.LocalID = localId;
			this.ExternalDatabaseID = externalDatabaseId;
			this.IdInExternalDatabase = idInExternalDb;
			this.NameInExternalDatabase = nameInExternalDb;
		}

		/// <summary>
		/// Constructor for server external database link wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerExternalDbLink object from a
		/// SoapExternalDbLink object.
		/// </remarks>
		/// <param name="data">
		/// A SoapExternalDbLink object from which to construct the
		/// ServerExternalDbLink object.
		/// </param>
		public ServerExternalDbLink ( SoapExternalDbLink data )
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
					__DBRow = LoadRow( data.LocalID, data.ExternalDatabaseID, data.IdInExternalDatabase );
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}

			// (BE) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);

		}

		/// <summary>
		/// Constructor for server process wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerExternalDbLink object from a
		/// DBRow.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerExternalDbLink ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

		/// <summary>
		/// Destructor for the ServerExternalDbLink class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerExternalDbLink()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "external_database_links";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the gene local ID.
		/// </summary>
		public Guid LocalID
		{
			get
			{
				return __DBRow.GetGuid("local_id");
			}
			set
			{
				__DBRow.SetGuid("local_id", value);
			}
		}

		/// <summary>
		/// Get/set the external database id.
		/// </summary>
		public int ExternalDatabaseID
		{
			get
			{
				return __DBRow.GetInt("external_database_id");
			}
			set
			{
				__DBRow.SetInt("external_database_id", value);
			}
		}
		
		/// <summary>
		/// Get/set the id of the object in the external database.
		/// </summary>
		public string IdInExternalDatabase
		{
			get
			{
				return __DBRow.GetString("id_in_external_database");
			}
			set
			{
				__DBRow.SetString("id_in_external_database", value);
			}
		}

		/// <summary>
		/// Get/set the id of the object in the external database.
		/// </summary>
		public string NameInExternalDatabase
		{
			get
			{
				return __DBRow.GetString("name_in_external_database");
			}
			set
			{
				__DBRow.SetString("name_in_external_database", value);
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
			SoapExternalDbLink retval = (derived == null) ? 
				retval = new SoapExternalDbLink() : retval = (SoapExternalDbLink)derived;

			retval.LocalID = this.LocalID;
			retval.ExternalDatabaseID = this.ExternalDatabaseID;
			retval.IdInExternalDatabase = this.IdInExternalDatabase;

			retval.Status = ObjectStatus.NoChanges;

			return retval;
		}

		/// <summary>
		/// Consumes a SoapObject object and updates the ServerExternalDbLink
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the ExternalDbLink relation.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
			SoapExternalDbLink c = o as SoapExternalDbLink;

			this.LocalID = c.LocalID;
			this.ExternalDatabaseID = c.ExternalDatabaseID;
			this.IdInExternalDatabase = c.IdInExternalDatabase;
		}
		
		#region ADO.NET SqlCommands

		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (BE) rewrote using BuildCommand()

			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + @" (local_id, external_database_id, id_in_external_database)
				VALUES (@local_id, @external_database_id, @id_in_external_database);",
				"@local_id", SqlDbType.Int, LocalID,
				"@external_database_id", SqlDbType.UniqueIdentifier, ExternalDatabaseID,
				"@id_in_external_database", SqlDbType.VarChar, IdInExternalDatabase);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName +
				"WHERE local_id = @local_id AND external_database_id = @external_database_id AND id_in_external_database = @id_in_external_database;",
				"@local_id", SqlDbType.Int, LocalID,
				"@external_database_id", SqlDbType.UniqueIdentifier, ExternalDatabaseID,
				"@id_in_external_database", SqlDbType.VarChar, IdInExternalDatabase);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + "SET local_id = @local_id, external_database_id = @external_database_id, id_in_external_database = @id_in_external_database;",
				"@local_id", SqlDbType.Int, LocalID,
				"@external_database_id", SqlDbType.Int, ExternalDatabaseID,
				"@id_in_external_database", SqlDbType.VarChar, IdInExternalDatabase);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName +
				"WHERE local_id = @local_id AND external_database_id = @external_database_id AND id_in_external_database = @id_in_external_database;",
				"@local_id", SqlDbType.Int, LocalID,
				"@external_database_id", SqlDbType.UniqueIdentifier, ExternalDatabaseID,
				"@id_in_external_database", SqlDbType.VarChar, IdInExternalDatabase);
		}
		#endregion

		#endregion


		#region Static Methods
		/// <summary>
		/// Return all external database links.
		/// </summary>
		/// <returns>
		/// Array of SoapExternalDbLink objects ready to be sent via SOAP.
		/// </returns>
		public static ServerExternalDbLink[] AllExternalDatabaseLinks ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerExternalDbLink( new DBRow( d ) ) );
			}

			return ( ServerExternalDbLink[] ) results.ToArray( typeof( ServerExternalDbLink ) );
		}

		/// <summary>
		/// Returns a single ServerExternalDbLink object
		/// </summary>
		/// <param name="localId"></param>
		/// <param name="externalDbId"></param>
		/// <param name="idInExternalDb"></param>
		/// <returns>
		/// Object ready to be sent via SOAP.
		/// </returns>
		public static ServerExternalDbLink Load ( Guid localId, int externalDbId, string idInExternalDb )
		{
			return new ServerExternalDbLink( LoadRow ( localId, externalDbId, idInExternalDb ) );
		}

		/// <summary>
		/// Return the DBRow for an object with the given parameters.
		/// </summary>
		/// <param name="localId"></param>
		/// <param name="externalDbId"></param>
		/// <param name="idInExternalDb"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( Guid localId, int externalDbId, string idInExternalDb )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + @"
				WHERE local_id = @local_id AND external_database_id = @external_database_id
				AND id_in_external_database = @id_in_external_database;",
				"@local_id", SqlDbType.UniqueIdentifier, localId,
				"@external_database_id", SqlDbType.Int, externalDbId,
				"@id_in_external_database", SqlDbType.VarChar, idInExternalDb);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		#region Relational Methods
		/// <summary>
		/// Get all of the external database link templates for a given database ID
		/// </summary>
		/// <param name="externalDatabaseId"></param>
		/// <returns></returns>
		public static ServerExternalDbLink[] GetAllTemplatesFromExternalDatabase( int externalDatabaseId )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE external_database_id = @external_database_id;",
				"@external_database_id", SqlDbType.Int, externalDatabaseId);

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerExternalDbLink( new DBRow( d ) ) );
			}

			return ( ServerExternalDbLink[] ) results.ToArray( typeof( ServerExternalDbLink ) );
		}

		/// <summary>
		/// Get all of the external database link templates for a given pathway
		/// </summary>
		/// <param name="pw">The pathway to consider</param>
		/// <returns>All external database links</returns>
		public static ServerExternalDbLink[] GetExternalDatabaseLinksForPathway( ServerPathway pw )
		{
            SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT edl.* FROM " + __TableName + @" edl INNER JOIN external_database_urls edu
					ON edu.external_database_id = edl.external_database_id
					WHERE local_id = @local_id AND type = 'pathway'
					ORDER BY edl.external_database_id;",
				"@local_id", SqlDbType.UniqueIdentifier, pw.ID);

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerExternalDbLink( new DBRow( d ) ) );
			}

			return ( ServerExternalDbLink[] ) results.ToArray( typeof( ServerExternalDbLink ) );
		}
		
		/// <summary>
		/// Check if a external database link already exists
		/// </summary>
		/// <param name="local_id"></param>
		/// <param name="external_database_id"></param>
		/// <param name="id_in_external_database"></param>
		/// <returns></returns>
		public static bool Exists( Guid local_id, int external_database_id, string id_in_external_database )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + @"
				WHERE local_id = @local_id AND external_database_id = @external_database_id
				AND id_in_external_database = @id_in_external_database;",
				"@local_id", SqlDbType.UniqueIdentifier, local_id,
				"@external_database_id", SqlDbType.Int, external_database_id,
				"@id_in_external_database", SqlDbType.VarChar, id_in_external_database);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Create a external database link
		/// </summary>
		/// <param name="local_id"></param>
		/// <param name="external_database_id"></param>
		/// <param name="id_in_external_database"></param>
		public static void AddExternalDatabaseLink( Guid local_id, int external_database_id, string id_in_external_database )
		{
			//(bse)
			// check if the process already belongs to the pathway
			//
			if ( !Exists( local_id, external_database_id, id_in_external_database ) )
			{
				DBWrapper db = DBWrapper.Instance;			
				db.ExecuteNonQuery(				
					"INSERT INTO " + __TableName + " ( local_id, external_database_id, id_in_external_database ) VALUES ( @local_id, @external_database_id, @id_in_external_database );",
					"@local_id", SqlDbType.UniqueIdentifier, local_id,
					"@external_database_id", SqlDbType.Int, external_database_id,
					"@id_in_external_database", SqlDbType.VarChar, id_in_external_database);
			}
		}		

		/// <summary>
		/// Removes the selected external database link from the table.
		/// </summary>
		/// <param name="local_id"></param>
		/// <param name="external_database_id"></param>
		/// <param name="id_in_external_database"></param>
		public static void RemoveExternalDatabaseLink( Guid local_id, int external_database_id, string id_in_external_database )
		{
			DBWrapper db = DBWrapper.Instance;
			db.ExecuteNonQuery(				
				"DELETE FROM " + __TableName + @"
				WHERE local_id = @local_id AND external_database_id = @external_database_id
					AND id_in_external_database = @id_in_external_database;",
				"@local_id", SqlDbType.UniqueIdentifier, local_id,
				"@external_database_id", SqlDbType.Int, external_database_id,
				"@id_in_external_database", SqlDbType.VarChar, id_in_external_database);
		}

		#endregion

		#endregion
	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerExternalDbLink.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerExternalDbLink.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.10  2006/07/09 15:22:09  greg
	 - Browser highlighting issues
	Apparently I hadn't fixed the highlighting issues like I thought I had, but it seems to be working fine now.
	
	 - Stuff autoloading that probably shouldn't
	All the ME data for processes (and perhaps other things) were automatically loading, even for items that weren't open.  I didn't really notice this until I started testing the browser with the Kegg data; since Kegg has a zillion more entries than our own database, the performance hit could only be realized when trying to load up extraneous information for hundreds of items at once.  Woof.
	
	 - 0x80040111 (NS_ERROR_NOT_AVAILABLE) error
	Apparently, certain kinds of browsing patterns may interrupt Ajax requests in a way Firefox doesn't like, thus causing this error to be thrown.  I resolved it with a try/catch block, but I'll keep my oeyes open for any additional fishiness.
	
	 - Built-in queries updates
	All built-in queries now include a link to jump to the graph visualization, which is useful when there are hundreds of results and it's not immediately clear that there is a graph to see.  Item pre-selection is fixed for several queries, but not all of them; Brandon and I are pumping those out as quickly as we can.
	
	Revision 1.9  2006/06/23 03:57:45  ali
	the extension "_greg" has been removed from the name of the external database links..
	
	Revision 1.8  2006/06/22 19:17:31  brandon
	added External Database links list to the DisplayPathwayDetail page
	
	Revision 1.7  2006/06/09 02:24:08  greg
	These changes address a significant number of GO pathway viewer bugs, and also introduce substantial changes to a lot of pages.  I'm refactoring a lot of the HTML to not only move the site closer to XHTML compliance (so we can once again use those buttons that are commented out on the main page!), but also to make the site display properly in non-IE browsers as well, because currently it doesn't look right in non-IE browsers (and hopefully soon everyone will be using Firefox!!).
	
	Revision 1.6  2006/05/23 18:29:32  greg
	Many old SQL queries were updated/optimized, and a bug that causes the system to crash when trying to navigate after viewing the details of an object through the Java applet is also fixed.  This required some semi-substantial modifications to LinkHelper.cs and SearchPagination.ascx.cs to allow for a slightly different method of dealing with query parameters.
	
	Revision 1.5  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.4  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.3  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.2  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.1  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	Revision 1.7  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.6  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.5  2005/07/08 21:55:07  brendan
	Debugging MolecularEntity/EntityNames/Gene inheritance.  Inheritance test not passing yet.
	
	Revision 1.4  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.3  2005/07/08 19:32:05  brandon
	fixed ServerExternalDbLink, sort of,  and uh, this project builds now
	
	Revision 1.2  2005/07/07 19:42:19  brandon
	did more on the catalyzes relation, don't know exactly how to get EC# more involved (?)
	
	Revision 1.1  2005/07/07 15:10:28  brandon
	Added ServerExternalDbLink.cs (gene_product_and_processes), it's not done yet, and added the GetAllOrganismGroups function to ServerProcess object
	

		
------------------------------------------------------------------------*/
#endregion