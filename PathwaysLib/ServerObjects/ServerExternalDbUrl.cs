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
	///		<filepath>PathwaysLib/Server/ServerExternalDbUrl.cs</filepath>
	///		<creation>2006/06/08</creation>
	///		<author>
	///			<name>Greg Strnad</name>
	///			<initials>gjs</initials>
	///			<email>gjs4@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brandon Evans</name>
	///				<initials>bse</initials>
	///				<email>brandon.evans@case.edu</email>
	///			</contributor>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerExternalDbUrl.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to external database URLs.
	/// </summary>
	#endregion
	public class ServerExternalDbUrl : ServerObject
	{

		#region Constructor, Destructor, ToString
		private ServerExternalDbUrl ( )
		{
		}

		/// <summary>
		/// Constructor, creates a new external database url wrapper with fields initiallized
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		/// <param name="url"></param>
		public ServerExternalDbUrl ( int id, string type, string url )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ID   = id;
			this.Type = type;
			this.URL  = url;
		}

		/// <summary>
		/// Constructor for server external database url wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerExternalDbUrl object from a
		/// SoapExternalDbUrl object.
		/// </remarks>
		/// <param name="data">
		/// A SoapExternalDbUrl object from which to construct the
		/// ServerExternalDbUrl object.
		/// </param>
		public ServerExternalDbUrl ( SoapExternalDbUrl data )
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
					__DBRow = LoadRow(data.ID, data.Type);
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}

			// (BE) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);
		}

		/// <summary>
		/// Constructor for server external database URL wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerExternalDbUrl object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerExternalDbUrl ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;
		}

		/// <summary>
		/// Destructor for the ServerExternalDbUrl class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerExternalDbUrl()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "external_database_urls";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the External Database ID.
		/// </summary>
		public int ID
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
		/// Get/set the type.
		/// </summary>
		public string Type
		{
			get
			{
				return __DBRow.GetString("type");
			}
			set
			{
				__DBRow.SetString("type", value);
			}
		}

		/// <summary>
		/// Get/set the External Database's URL (template)
		/// </summary>
		public string URL
		{
			get
			{
				return __DBRow.GetString("url_template");
			}
			set
			{
				__DBRow.SetString("url_template", value);
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
			SoapExternalDbUrl retval = (derived == null) ? 
				retval = new SoapExternalDbUrl() : retval = (SoapExternalDbUrl)derived;

			retval.ID   = this.ID;
			retval.Type = this.Type;
			retval.URL  = this.URL;

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
			SoapExternalDbUrl e = o as SoapExternalDbUrl;

			this.ID = e.ID;
			this.Type = e.Type;
			this.URL = e.URL;
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
				"INSERT INTO " + __TableName + @" (external_database_id, type, url_template)
					VALUES (@id, @type, @url)",
				"@id", SqlDbType.Int, ID,
				"@type", SqlDbType.VarChar, Type,
				"@url", SqlDbType.VarChar, URL);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE external_database_id = @id AND type = @type",
				"@id", SqlDbType.Int, ID,
				"@type", SqlDbType.VarChar, Type);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + @" SET url_template = @url
					WHERE external_database_id = @id AND type = @type",
				"@url", SqlDbType.VarChar, URL,
				"@id", SqlDbType.Int, ID,
				"@type", SqlDbType.VarChar, Type);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE external_database_id = @id AND type = @type",
				"@id", SqlDbType.Int, ID,
				"@type", SqlDbType.VarChar, Type);
		}
		#endregion
		#endregion


		#region Static Methods
		/// <summary>
		/// Return all external database URLs from the system.
		/// </summary>
		/// <returns>
		/// Array of ServerExternalDbUrl objects.
		/// </returns>
		public static ServerExternalDbUrl[] AllExternalDbUrls ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerExternalDbUrl( new DBRow( d ) ) );
			}

			return ( ServerExternalDbUrl[] ) results.ToArray( typeof( ServerExternalDbUrl ) );
		}

		/// <summary>
		/// Return an external database URL with given ID and type.
		/// </summary>
		/// <param name="id">The ID of the desired external database.</param>
		/// <param name="type">The item type.</param>
		/// <returns>
		/// ServerExternalDbUrl object.
		/// </returns>
		public static ServerExternalDbUrl Load ( int id, string type )
		{
			return new ServerExternalDbUrl( LoadRow ( id, type ) );
		}

		/// <summary>
		/// Returns true if there exists a database URL with the given ID and type
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool Exists ( int id, string type )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE external_database_id = @id AND type = @type",
				"@id", SqlDbType.Int, id,
				"@type", SqlDbType.VarChar, type);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Return the DBRow for the external database URL with a given ID and type.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( int id, string type )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE external_database_id = @id AND type = @type",
				"@id", SqlDbType.Int, id,
				"@type", SqlDbType.VarChar, type);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		#endregion

	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerExternalDbUrl.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerExternalDbUrl.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.3  2006/06/23 03:57:45  ali
	the extension "_greg" has been removed from the name of the external database links..
	
	Revision 1.2  2006/06/22 19:17:31  brandon
	added External Database links list to the DisplayPathwayDetail page
	
	Revision 1.1  2006/06/09 02:24:08  greg
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
