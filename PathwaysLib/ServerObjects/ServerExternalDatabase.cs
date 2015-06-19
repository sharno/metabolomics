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
	///		<filepath>PathwaysLib/Server/ServerExternalDatabase.cs</filepath>
	///		<creation>2005/06/08</creation>
	///		<author>
	///			<name>Brandon Evans</name>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerExternalDatabase.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to biological pathways.
	/// </summary>
	#endregion
	public class ServerExternalDatabase : ServerObject
	{

		#region Constructor, Destructor, ToString
		private ServerExternalDatabase ( )
		{
		}

		/// <summary>
		/// Constructor, creates a new external database wrapper with fields initiallized
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fullname"></param>
		/// <param name="url"></param>
		public ServerExternalDatabase ( string name, string fullname, string url )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ID = int.Parse(__DBRow["id"].ToString());
			this.Name = name;
			this.Fullname = fullname;
			this.URL = url;
		}

		/// <summary>
		/// Constructor for server external database wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerExternalDatabase object from a
		/// SoapExternalDatabase object.
		/// </remarks>
		/// <param name="data">
		/// A SoapExternalDatabase object from which to construct the
		/// ServerExternalDatabase object.
		/// </param>
		public ServerExternalDatabase ( SoapExternalDatabase data )
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
					__DBRow = LoadRow(data.ID);
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}

			// (BE) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);
		}

		/// <summary>
		/// Constructor for server external database wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerExternalDatabase object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerExternalDatabase ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;
		}

		/// <summary>
		/// Destructor for the ServerExternalDatabase class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerExternalDatabase()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "external_databases";
		private static readonly string __TemplateKey = "*id*";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the External Database ID.
		/// </summary>
		public int ID
		{
			get
			{
				return __DBRow.GetInt("id");
			}
			set
			{
				__DBRow.SetInt("id", value);
			}
		}

		/// <summary>
		/// Get/set the External Database name.
		/// </summary>
		public string Name
		{
			get
			{
				return __DBRow.GetString("name");
			}
			set
			{
				__DBRow.SetString("name", value);
			}
		}
		
		/// <summary>
		/// Get/set the External Database's full name.
		/// </summary>
		public string Fullname
		{
			get
			{
				return __DBRow.GetString("fullname");
			}
			set
			{
				__DBRow.SetString("fullname", value);
			}
		}

		/// <summary>
		/// Get/set the External Database's URL.
		/// </summary>
		public string URL
		{
			get
			{
				return __DBRow.GetString("url");
			}
			set
			{
				__DBRow.SetString("url", value);
			}
		}

        /// <summary>
        /// Get the database's extended name
        /// </summary>
		public string ExtendedName
		{
			get
			{
				return Fullname == null ? Name :
					string.Format("{0} ({1})", Fullname, Name);
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
			SoapExternalDatabase retval = (derived == null) ? 
				retval = new SoapExternalDatabase() : retval = (SoapExternalDatabase)derived;

			retval.ID   = this.ID;
			retval.Name = this.Name;
			retval.Fullname = this.Fullname;
			retval.URL = this.URL;

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
			SoapExternalDatabase e = o as SoapExternalDatabase;

			if (o.Status == ObjectStatus.Insert)
			{
				DBRow temp = new DBRow( __TableName );
				e.ID = int.Parse(temp["id"].ToString()); // generate a new ID
			}

			this.ID = e.ID;
			this.Name = e.Name;
			this.Fullname = e.Fullname;
			this.URL = e.URL;
		}

		/// <summary>
		/// Return all links from this external database.
		/// </summary>
		/// <returns></returns>
		public ServerExternalDbLink[] GetAllTemplateLinks ( )
		{
			return ServerExternalDbLink.GetAllTemplatesFromExternalDatabase ( this.ID );
		}

		#region External Database Links relation
		/// <summary>
		/// Add a link for this external database
		/// </summary>
		/// <param name="localId"></param>
		/// <param name="idInExternalDb"></param>
		public void AddLink ( Guid localId, string idInExternalDb )
		{
			ServerExternalDbLink.AddExternalDatabaseLink ( localId, this.ID, idInExternalDb );
		}

		/// <summary>
		/// Remove the given link to this external database
		/// </summary>
		/// <param name="localId"></param>
		/// <param name="idInExternalDb"></param>
		public void RemoveLink ( Guid localId, string idInExternalDb )
		{
			ServerExternalDbLink.RemoveExternalDatabaseLink( localId, this.ID, idInExternalDb );
		}

		#endregion


		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (BE) rewrote using BuildCommand()

			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + " (id, name, fullname, url) VALUES (@id, @name, @fullname, @url);",
				"@id", SqlDbType.Int, ID,
				"@name", SqlDbType.VarChar, Name,
				"@fullname", SqlDbType.VarChar, Fullname,
				"@url", SqlDbType.VarChar, URL);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.Int, ID);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + " SET name = @name, fullname = @fullname, url = @url WHERE id = @id;",
				"@name", SqlDbType.VarChar, Name,
				"@fullname", SqlDbType.VarChar, Fullname,
				"@url", SqlDbType.VarChar, URL,
				"@id", SqlDbType.Int, ID);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.Int, ID);
		}
		#endregion
		#endregion


		#region Static Methods
		/// <summary>
		/// Return all external databases from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapExternalDatabase objects ready to be sent via SOAP.
		/// </returns>
		public static ServerExternalDatabase[] AllExternalDatabases ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerExternalDatabase( new DBRow( d ) ) );
			}

			return ( ServerExternalDatabase[] ) results.ToArray( typeof( ServerExternalDatabase ) );
		}

		/// <summary>
		/// Return an external database with given ID.
		/// </summary>
		/// <param name="id">
		/// The ID of the desired external database.
		/// </param>
		/// <returns>
		/// SoapExternalDatabase object ready to be sent via SOAP.
		/// </returns>
		public static ServerExternalDatabase Load ( int id )
		{
			return new ServerExternalDatabase( LoadRow ( id ) );
		}

		/// <summary>
		/// Returns true if there exists a database with the given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool Exists ( int id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.Int, id);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Return the DBRow for the external database with a given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( int id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.Int, id);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		/// <summary>
		/// Get translated links from database for pathway
		/// </summary>
		/// <param name="db_id"></param>
		/// <param name="local_id"></param>
		/// <returns></returns>
		public static ServerExternalDbUrl[] GetAllTranslatedLinksFromDatabaseForPathway( int db_id, Guid local_id )
		{
			// this used to call give the 
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT url_template, id_in_external_database
					FROM external_database_links edl INNER JOIN external_database_urls edu
					ON edu.external_database_id = edl.external_database_id
					WHERE edl.external_database_id = @db_id AND local_id = @local_id AND type = 'pathway'
					ORDER BY edl.external_database_id;",
				"@db_id", SqlDbType.Int, db_id,
				"@local_id", SqlDbType.UniqueIdentifier, local_id);

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				string url = d.Tables[0].Rows[0]["url_template"].ToString().Replace( __TemplateKey,
					d.Tables[0].Rows[0]["id_in_external_database"].ToString() );
				results.Add( new ServerExternalDbUrl(db_id, "pathway", url) );
			}

			return ( ServerExternalDbUrl[] ) results.ToArray( typeof( ServerExternalDbUrl[] ) );
		}

		/// <summary>
		/// Takes a ServerExternalDbLink and combines the id_in_external_db with the
		/// url template from the external_database_urls table to give a translated url
		/// </summary>
		/// <param name="link"></param>
		/// <param name="type">The type of link ("pathway", "process" etc.)</param>
		/// <returns>the complete url for an external database link</returns>
		public static string TranslateUrlFromExternalDbLink( ServerExternalDbLink link, string type )
		{
			return ServerExternalDbUrl.Load(link.ExternalDatabaseID, type).URL.Replace( __TemplateKey, link.IdInExternalDatabase );
		}

		#endregion
	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerExternalDatabase.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerExternalDatabase.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.8  2006/06/23 03:57:45  ali
	the extension "_greg" has been removed from the name of the external database links..
	
	Revision 1.7  2006/06/22 19:17:31  brandon
	added External Database links list to the DisplayPathwayDetail page
	
	Revision 1.6  2006/06/09 02:24:08  greg
	These changes address a significant number of GO pathway viewer bugs, and also introduce substantial changes to a lot of pages.  I'm refactoring a lot of the HTML to not only move the site closer to XHTML compliance (so we can once again use those buttons that are commented out on the main page!), but also to make the site display properly in non-IE browsers as well, because currently it doesn't look right in non-IE browsers (and hopefully soon everyone will be using Firefox!!).
	
	Revision 1.5  2006/05/23 18:29:32  greg
	Many old SQL queries were updated/optimized, and a bug that causes the system to crash when trying to navigate after viewing the details of an object through the Java applet is also fixed.  This required some semi-substantial modifications to LinkHelper.cs and SearchPagination.ascx.cs to allow for a slightly different method of dealing with query parameters.
	
	Revision 1.4  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.3  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.2  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.1  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
		
------------------------------------------------------------------------*/
#endregion
