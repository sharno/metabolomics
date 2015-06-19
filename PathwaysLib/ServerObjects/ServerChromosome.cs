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
	///		<filepath>PathwaysLib/Server/ServerChromosome.cs</filepath>
	///		<creation>2005/06/29</creation>
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
	///			<contributor>
	///				<name>Suleyman Fatih Akgul</name>
	///				<initials>sfa</initials>
	///				<email>fatih@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:53 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerChromosome.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to chromosomes.
	/// </summary>
	#endregion
	public class ServerChromosome : ServerObject
	{

		#region Constructor, Destructor, ToString
		private ServerChromosome ( )
		{
		}
		
		/// <summary>
		/// Constructor for server chromosome wrapper with fields initiallized
		/// </summary>
		/// <param name="name"></param>
		/// <param name="length"></param>
		/// <param name="notes"></param>
		public ServerChromosome ( string name, Guid organism_group_id, long length, int centromere_location, string notes )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ID = DBWrapper.NewID(); // generate a new ID
			this.Name = name;
			this.Length = length;
			this.ChromosomeNotes = notes;
            this.OrganismGroupId = organism_group_id;
            this.CentromereLocation = centromere_location;
		}

		/// <summary>
		/// Constructor for server chromosome wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerPathway object from a
		/// SoapChromosome object.
		/// </remarks>
		/// <param name="data">
		/// A SoapChromosome object from which to construct the
		/// ServerChromosome object.
		/// </param>
		public ServerChromosome ( SoapChromosome data )
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
		/// Constructor for server chromosome wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerChromosome object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerChromosome ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

        

		/// <summary>
		/// Destructor for the ServerChromosome class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerChromosome()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "chromosomes";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the chromosome ID.
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

		/// <summary>
		/// Get/set the chromosome name.
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
        /// Get/set the organism id
        /// </summary>
        public Guid OrganismGroupId
        {
            get
            {
                return __DBRow.GetGuid("organism_group_id");
            }
            set
            {
                __DBRow.SetGuid("organism_group_id", value);
            }
        }
		
		/// <summary>
		/// Get/set the chromosome length.
		/// </summary>
		public long Length
		{
			get
			{
				return __DBRow.GetLong("length");
			}
			set
			{
				__DBRow.SetLong("length", value);
			}
		}

        /// <summary>
        /// Get/set the centromere location.
        /// </summary>
        public int CentromereLocation
        {
            get
            {
                return __DBRow.GetInt("centromere_location");
            }
            set
            {
                __DBRow.SetInt("centromere_location", value);
            }
        }

		/// <summary>
		/// Get/set the notes.
		/// </summary>
		public string ChromosomeNotes
		{
			get
			{
				return __DBRow.GetString("notes");
			}
			set
			{
				__DBRow.SetString("notes", value);
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
			SoapChromosome retval = (derived == null) ? 
                retval = new SoapChromosome() : retval = (SoapChromosome)derived;

			retval.ID   = this.ID;
			retval.Name = this.Name;
			retval.Length = this.Length;
            retval.OrganismGroupId = this.OrganismGroupId;
            retval.CentromereLocation = this.CentromereLocation;
			retval.ChromosomeNotes = this.ChromosomeNotes;

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
			SoapChromosome c = o as SoapChromosome;

			if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
				c.ID = DBWrapper.NewID(); // generate a new ID

			this.ID = c.ID;
			this.Name = c.Name;
			this.Length = c.Length;
            this.OrganismGroupId = c.OrganismGroupId;
            this.CentromereLocation = c.CentromereLocation;
			this.ChromosomeNotes = c.ChromosomeNotes;
		}

		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (mfs)
			// add the INSERT command
			SqlCommand Insert   = new SqlCommand( "INSERT INTO " + __TableName + " (id, organism_group_id, name, length, centromere_location, notes) VALUES (@i_id, @i_organism_group_id, @i_name, @i_length, @i_centromere_location, @i_notes);" );
			
			SqlParameter i_id = new SqlParameter( "@i_id", SqlDbType.UniqueIdentifier, 32, "id" );
			i_id.Value = ID;
			Insert.Parameters.Add( i_id );

            SqlParameter i_organism_group_id = new SqlParameter("@i_organism_group_id", SqlDbType.UniqueIdentifier, 32, "organism_group_id");
            i_organism_group_id.Value = OrganismGroupId;
            Insert.Parameters.Add( i_organism_group_id );
			
            SqlParameter i_name = new SqlParameter( "@i_name", SqlDbType.VarChar, 255, "name" );
			i_name.Value = Name;
			Insert.Parameters.Add( i_name );
			
			SqlParameter i_length = new SqlParameter( "@i_length", SqlDbType.BigInt, 8, "length" );
			i_length.Value = Length;
			Insert.Parameters.Add( i_length );

            SqlParameter i_centromere_location = new SqlParameter("@i_centromere_location", SqlDbType.Int, 4, "centromere_location");
            i_centromere_location.Value = CentromereLocation;
            Insert.Parameters.Add(i_centromere_location);
			
			SqlParameter i_notes = new SqlParameter( "@i_notes", SqlDbType.Text, 255, "notes" );
			i_notes.Value = ChromosomeNotes;
			Insert.Parameters.Add( i_notes );
			__DBRow.ADOCommands["insert"] = Insert;

			// (mfs)
			// add the SELECT command
			SqlCommand Select = new SqlCommand( "SELECT * FROM " + __TableName + " WHERE id = @s_id;" );
			SqlParameter s_id = new SqlParameter( "@s_id", SqlDbType.UniqueIdentifier, 32, "id");
			s_id.SourceVersion = DataRowVersion.Original;
			s_id.Value = ID;
			Select.Parameters.Add( s_id );
			__DBRow.ADOCommands["select"] = Select;

			// (mfs)
			// add the UPDATE command
            SqlCommand Update = new SqlCommand("UPDATE " + __TableName + " SET name = @u_name, organism_group_id = @u_organism_group_id, length = @u_length, centromere_location = @u_centromere_location, notes = @u_notes WHERE id = @u_id;");
			
			SqlParameter u_name = new SqlParameter( "@u_name", SqlDbType.VarChar, 255, "name" );
			u_name.Value = Name;
			Update.Parameters.Add( u_name );

            SqlParameter u_organism_group_id = new SqlParameter("@u_organism_group_id", SqlDbType.UniqueIdentifier, 32, "organism_group_id");
            u_organism_group_id.Value = OrganismGroupId;
            Insert.Parameters.Add(u_organism_group_id);
			
			SqlParameter u_length = new SqlParameter( "@u_length", SqlDbType.BigInt, 8, "length" );
			u_length.Value = Length;
			Update.Parameters.Add( u_length );

            SqlParameter u_centromere_location = new SqlParameter("@u_centromere_location", SqlDbType.Int, 4, "centromere_location");
            u_centromere_location.Value = CentromereLocation;
            Insert.Parameters.Add(u_centromere_location);
			
			SqlParameter u_notes = new SqlParameter( "@u_notes", SqlDbType.Text, 255, "notes" );
			u_notes.Value = ChromosomeNotes;
			Update.Parameters.Add( u_notes );
			
			SqlParameter u_id   = new SqlParameter( "@u_id", SqlDbType.UniqueIdentifier, 32, "id");
			u_id.SourceVersion = DataRowVersion.Original;
			u_id.Value = ID;
			Update.Parameters.Add( u_id );
			
			__DBRow.ADOCommands["update"] = Update;

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
		/// Return all chromosomes from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapChromosome objects ready to be sent via SOAP.
		/// </returns>
		public static ServerChromosome[] AllChromosomes ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerChromosome( new DBRow( d ) ) );
			}

			return ( ServerChromosome[] ) results.ToArray( typeof( ServerChromosome ) );
		}

        /// <summary>
        /// Return all chromosomes of an organism.
        /// </summary>
        /// <returns>
        /// Array of SoapChromosome objects ready to be sent via SOAP.
        /// </returns>
        public static ServerChromosome[] GetAllChromosomesForOrganism(Guid organismId)
        {
            SqlCommand command = DBWrapper.BuildCommand(@"SELECT * FROM " + __TableName + 
                                                          @" WHERE organism_group_id = @orgId;",
                                                          "@orgId", SqlDbType.UniqueIdentifier, organismId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerChromosome(new DBRow(d)));
            }

            return (ServerChromosome[])results.ToArray(typeof(ServerChromosome));
        }

		/// <summary>
		/// Return a chromosome with given ID.
		/// </summary>
		/// <param name="id">
		/// The Guid of the desired chromosome.
		/// </param>
		/// <returns>
		/// SoapChromosome object ready to be sent via SOAP.
		/// </returns>
		public static ServerChromosome Load ( Guid id )
		{
			return new ServerChromosome( LoadRow ( id ) );
		}

		/// <summary>
		/// Return the dataset for a chromosome with a given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( Guid id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.UniqueIdentifier, id);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		/// <summary>
		/// Returns true if the chromosome_id is in the chromosomes table,
		/// otherwise returns false
		/// </summary>
		/// <param name="chromosome_id"></param>
		/// <returns></returns>
		public static bool Exists(Guid chromosome_id)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.UniqueIdentifier, chromosome_id);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}
		/// <summary>
		/// Returns all chromosomes who's name contains the given substring
		/// </summary>
		/// <param name="substring"></param>
		/// <param name="searchMethod"></param>
		/// <returns></returns>
		public static ServerChromosome[] FindChromosomes(string substring, SearchMethod searchMethod)
		{
			string commandString;

			switch(searchMethod)
			{
					
				case SearchMethod.Contains:
					commandString = "SELECT * FROM " + __TableName + " WHERE name LIKE '%" + substring + "%' ORDER BY me.[name];";
					break;
				case SearchMethod.EndsWith:
					commandString = "SELECT * FROM " + __TableName + " WHERE name LIKE '%" + substring + "' ORDER BY [name];";
					break;
				case SearchMethod.ExactMatch:
					commandString = "SELECT * FROM " + __TableName + " WHERE name = '" + substring + "' ORDER BY [name];";
					break;
				case SearchMethod.StartsWith:
				default:
					commandString = "SELECT * FROM " + __TableName + " WHERE name LIKE '" + substring + "%' ORDER BY [name];";
					break;
			}

			SqlCommand command = DBWrapper.BuildCommand( commandString );
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerChromosome( new DBRow( d ) ) );
			}

			return ( ServerChromosome[] ) results.ToArray( typeof( ServerChromosome ) );
		}


		#endregion

	} // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerChromosome.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerChromosome.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.5  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.4  2006/11/26 21:49:25  ali
	*** empty log message ***
	
	Revision 1.3  2006/10/19 01:24:42  ali
	*** empty log message ***
	
	Revision 1.2  2006/08/17 15:04:43  ali
	A new web method "GetGeneMappingForPathway" is added for the gene viewer.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.13  2006/05/18 05:33:30  gokhan
	*** empty log message ***
	
	Revision 1.12  2006/05/18 02:03:13  gokhan
	*** empty log message ***
	
	Revision 1.11  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.10  2006/04/21 17:37:29  michael
	*** empty log message ***
	
	Revision 1.9  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.8  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.7  2005/07/20 22:55:56  brandon
	I think I fixed the GetChromosome problem in ServerGene.cs, but maybe not
	
	Revision 1.6  2005/07/20 18:02:19  brandon
	added function to ServerPathway: GetConnectedPathways ( ), which returns an array of ConnectedPathwayAndCommonProcesses objects.  This new object has three properties:
	ServerPathway ConnectedPathway- (to be listed as a connected pathway)
	ServerProcess[] SharedProcesses - (shared by two pathways)
	ServerMolecularEntity[] SharedExclusiveMolecules - (molecules shared
	by two pathways but are not included in any process in SharedProcesses)
	
	Revision 1.5  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.4  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.3  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.2  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.1  2005/06/30 19:46:32  brandon
	added a bunch of new classes for all the boxed on the ER diagram, even the ones that only have an id attribute
	

------------------------------------------------------------------------*/
#endregion