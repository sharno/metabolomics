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
	///		<filepath>PathwaysLib/Server/ServerCommonMolecule.cs</filepath>
	///		<creation>2005/06/30</creation>
	///		<author>
	///			<name>Brendan Elliott</name>
	///			<initials>BE</initials>
	///			<email>bxe7@cwru.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Michael F. Starke</name>
	///				<initials>mfs</initials>
	///				<email>michael.starke@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Brandon Evans</name>
	///				<initials>bse</initials>
	///				<email>brandon.evans@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerCommonMolecule.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to common molecules.
	/// </summary>
	#endregion
	public class ServerCommonMolecule : ServerObject
	{

		#region Constructor, Destructor, ToString
		private ServerCommonMolecule ( )
		{
		}

		/// <summary>
		/// Constructor for server common molecule wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerCommonMolecule object from a
		/// SoapCommonMolecule object.
		/// </remarks>
		/// <param name="data">
		/// A SoapCommonMolecule object from which to construct the
		/// ServerCommonMolecule object.
		/// </param>
		public ServerCommonMolecule ( SoapCommonMolecule data )
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
		/// Constructor for server common molecule wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerCommonMolecule object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerCommonMolecule ( DBRow data )
		{
			__DBRow = data;
		}

		/// <summary>
		/// Destructor for the ServerCommonMolecule class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerCommonMolecule()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "common_molecules";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the CommonMolecule ID.
		/// </summary>
		public Guid ID
		{
			get
			{
				return __DBRow.GetGuid("id");
			}
			set
			{
				__DBRow.SetGuid("id", value);
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
			SoapCommonMolecule retval = (derived == null) ? 
				retval = new SoapCommonMolecule() : retval = (SoapCommonMolecule)derived;

			retval.ID   = this.ID;

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
			SoapCommonMolecule bm = o as SoapCommonMolecule;

            if (o.Status == ObjectStatus.Insert && bm.ID == Guid.Empty)
				bm.ID = DBWrapper.NewID(); // generate a new ID

			this.ID = bm.ID;

		}

		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (mfs)
			// add the INSERT command
			SqlCommand Insert   = new SqlCommand( "INSERT INTO " + __TableName + " (id) VALUES (@i_id);" );
			SqlParameter i_id = new SqlParameter( "@i_id", SqlDbType.UniqueIdentifier, 32, "id" );
			i_id.Value = ID;
			Insert.Parameters.Add( i_id );
			__DBRow.ADOCommands["insert"] = Insert;

			// (mfs)
			// add the SELECT command
			SqlCommand Select = new SqlCommand( "SELECT * FROM " + __TableName + " WHERE id = @s_id;" );
			SqlParameter s_id = new SqlParameter( "@s_id", SqlDbType.UniqueIdentifier, 32, "id");
			s_id.SourceVersion = DataRowVersion.Original;
			s_id.Value = ID;
			Select.Parameters.Add( s_id );
			__DBRow.ADOCommands["select"] = Select;

			// (bse)
			// I don't think there can be an UPDATE command for CommonMolecule

			// (mfs)
			// add the DELETE command
			SqlCommand Delete = new SqlCommand ( "DELETE FROM " + __TableName + " WHERE id = @d_id;" );
			SqlParameter d_id = new SqlParameter( "@d_id", SqlDbType.UniqueIdentifier, 32, "id");
			d_id.SourceVersion = DataRowVersion.Original;
			d_id.Value = ID;
			Delete.Parameters.Add( d_id );
			__DBRow.ADOCommands["delete"] = Delete;

		}
		#endregion
		#endregion


		#region Static Methods
		/// <summary>
		/// Return all common molecules from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapCommonMolecule objects ready to be sent via SOAP.
		/// </returns>
		public static ServerCommonMolecule[] AllCommonMolecules ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerCommonMolecule( new DBRow( d ) ) );
			}

			return ( ServerCommonMolecule[] ) results.ToArray( typeof( ServerCommonMolecule ) );
		}

		/// <summary>
		/// Return a common molecule with given ID.
		/// </summary>
		/// <param name="id">
		/// The Guid of the desired common molecule.
		/// </param>
		/// <returns>
		/// SoapCommonMolecule object ready to be sent via SOAP.
		/// </returns>
		public static ServerCommonMolecule Load ( Guid id )
		{
			return new ServerCommonMolecule( LoadRow ( id ) );
		}

		/// <summary>
		/// Return the dataset for a common molecule with a given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( Guid id )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + " WHERE id = @id;" );
			SqlParameter ident = new SqlParameter( "@id", SqlDbType.UniqueIdentifier );
			ident.SourceVersion = DataRowVersion.Original;
			ident.Value = id;
			command.Parameters.Add( ident );

			DataSet ds = new DataSet();
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}
		#endregion

	} // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerCommonMolecule.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerCommonMolecule.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.4  2006/07/07 19:28:04  greg
	The bulk of this update focuses on integrating Ajax browsing into the content browser bar on the left.  It currently only works from the pathways dropdown option, but the framework is now in place for the other lists to function in the same manner.
	
	Revision 1.3  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.2  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	

		
------------------------------------------------------------------------*/
#endregion