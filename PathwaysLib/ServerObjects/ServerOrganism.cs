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
	///		<filepath>PathwaysLib/ServerObjects/ServerOrganism.cs</filepath>
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
	///			<contributor>
	///				<name>Suleyman Fatih Akgul</name>
	///				<initials>sfa</initials>
	///				<email>fatih@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Gokhan Yavas</name>
	///				<initials>gy</initials>
	///				<email>gokhan.yavas@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Brian Lauber</name>
	///				<initials>bml</initials>
	///				<email>bml8@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: ann $</cvs_author>
	///			<cvs_date>$Date: 2009/09/17 20:40:38 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerOrganism.cs,v 1.2 2009/09/17 20:40:38 ann Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.2 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
    /// <summary>
    /// Encapsulates database access related to organisms.
    /// </summary>
    #endregion
    public class ServerOrganism : ServerOrganismGroup
    {

        #region Constructor, Destructor, ToString
        private ServerOrganism()
        {
        }


		/// <summary>
		/// Constructor for server organism wrapper with fields initiallized
		/// </summary>
		/// <param name="common"></param>
		/// <param name="scientific"></param>
		/// <param name="taxonomyId"></param>
		/// <param name="parent"></param>
		/// <param name="notes"></param>
		public ServerOrganism( string common, string scientific, string taxonomyId, Guid parent, string notes, int cM_unit_length ):
			base(scientific, common, parent, notes)
		{
			// not yet in DB, so create empty row
// GJS: Already taken care of...
//			__orgRow = new DBRow( __TableName );

			base.ID              = DBWrapper.NewID();
			this.IsOrganism      = true;		// VERY IMPORTANT!!
			this.TaxonomyID      = taxonomyId;
            this.CM_Unit_Length = cM_unit_length;  //by default 1,000,000 if not specified

			this.SetSqlCommandParameters();
		}

        /// <summary>
        /// Constructor for server Organism wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerOrganism object from a
        /// SoapOrganism object.
        /// </remarks>
        /// <param name="data">
        /// A SoapOrganism object from which to construct the
        /// ServerOrganism object.
        /// </param>
         public ServerOrganism ( SoapOrganism data ):
			base(data)
        {
            // (BE) setup database row
            switch(data.Status)
            {
                case ObjectStatus.Insert:
// GJS: Already taken care of...
//					__orgRow = new DBRow( __TableName    );
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
					__orgRow = ServerOrganism.LoadRow(data.ID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // (BE) get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

			// (mfs)
			// required call to setup SqlCommands
			
			SetSqlCommandParameters ( );
        }


		/// <summary>
		/// Constructor for server organism wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerOrganism object from its organism and organism_group rows
		/// </remarks>
		/// <param name="organism">
		/// The organism part
		/// </param>
		/// <param name="group">
		/// The group part of the organism
		/// </param>
		public ServerOrganism ( DBRow organism, DBRow group ):
			base(group)
		{
			__orgRow = organism;
			SetSqlCommandParameters();
		}



		/// <summary>
		/// Loads a ServerOrganism from a row of the organisms table
		/// </summary>
		/// <param name="organism">A row of the organisms table</param>
		public ServerOrganism(DBRow organism):
			base(ServerOrganismGroup.LoadRow(organism.GetGuid("id")))
		{
			__orgRow = organism;
			SetSqlCommandParameters ( );
		}

        /// <summary>
        /// Destructor for the ServerOrganism class.
        /// </summary>
        ~ServerOrganism()
        {
        }
        #endregion


        #region Member Variables
		private static readonly string __TableName = "organisms";

		/// <summary>
		/// The string representation of an unspecified organism
		/// </summary>
		public static readonly string UnspecifiedOrganism = "00000000-0000-0000-0000-000000000000";

		// GJS: Added a default value here so constructors would not crash...
		private DBRow __orgRow = new DBRow( __TableName ); // Contains the organism part
        #endregion


        #region Properties
		/// <summary>
		/// Get/set the Organism ID.
		/// </summary>
		public override Guid ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
                __orgRow.SetGuid("id", value);
			}
		}


				
		/// <summary>
		/// Get/set the organism's taxonomy id
		/// </summary>
		public string TaxonomyID
		{
			get
			{
				return __orgRow.GetString( "taxonomy_id" );
			}
			set
			{
				__orgRow.SetString( "taxonomy_id", value );
			}
		}

        /// <summary>
        /// Get/set the cM_unit_length
        /// </summary>
        public int CM_Unit_Length
        {
            get
            {
                return __orgRow.GetInt("cM_unit_length");
            }
            set
            {
                __orgRow.SetInt("cM_unit_length", value);
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
            SoapOrganism retval = ((derived == null) ? new SoapOrganism() : (SoapOrganism)derived);

			base.PrepareForSoap(retval);
			retval.TaxonomyID = this.TaxonomyID;
            retval.CM_Unit_Length = this.CM_Unit_Length;

			return retval;
		}
		/// <summary>
		/// Update the base class's data row, then the derived class's row
		/// </summary>
		public override void UpdateDatabase()
		{
			base.UpdateDatabase ();
			__orgRow.UpdateDatabase();

		}

		/// <summary>
		/// Consumes a SoapObject object and updates the ServerOrganism
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the organism.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
			base.UpdateFromSoap(o);
			SoapOrganism org = o as SoapOrganism;

			if(org.Status == ObjectStatus.Insert)
			{
				this.IsOrganism = true;            // Ensure this is an organism (only necessary on inserts)
			}

			this.ID = base.ID;                 // Make sure that the organism id and organism_group id match!
			this.TaxonomyID = org.TaxonomyID;
            this.CM_Unit_Length = org.CM_Unit_Length;
		}




		#region ADO.NET SqlCommands
		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			base.SetSqlCommandParameters();
			SqlCommand Insert = DBWrapper.BuildCommand("INSERT INTO " + __TableName + "(id, taxonomy_id, cm_unit_length) VALUES (@id, @tax, @cm_unit_length);",
				"@id", SqlDbType.UniqueIdentifier, this.ID,
				"@tax", SqlDbType.VarChar, this.TaxonomyID,
                "@cm_unit_length", SqlDbType.Int, this.CM_Unit_Length);
            SqlCommand Update = DBWrapper.BuildCommand("UPDATE " + __TableName + " SET taxonomy_id=@tax, cm_unit_length=@cm_unit_length WHERE id=@id;",
				"@id", SqlDbType.UniqueIdentifier, this.ID,
				"@tax", SqlDbType.VarChar, this.TaxonomyID);
			SqlCommand Delete = DBWrapper.BuildCommand("DELETE FROM " + __TableName + " WHERE id=@id;",
				"@id", SqlDbType.UniqueIdentifier, this.ID);
			SqlCommand Select = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE id=@id;",
				"@id", SqlDbType.UniqueIdentifier, this.ID);
			__orgRow.ADOCommands["insert"] = Insert;
			__orgRow.ADOCommands["update"] = Update;
			__orgRow.ADOCommands["delete"] = Delete;
			__orgRow.ADOCommands["select"] = Select;

		}
		#endregion
        #endregion



		#region Static Methods

		/// <summary>
		/// Return all organisms from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapOrganism objects ready to be sent via SOAP.
		/// </returns>
		public static ServerOrganism[] AllOrganisms ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			return ServerOrganism.LoadMultiple(command);
		}



        /// <summary>
        /// Returns all organisms that contain a pathway (i.e. contain any process in that pathway).
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>
        public static ServerOrganism[] AllOrganismsForPathway(Guid pathwayId)
        {
            //TODO: (BE) is this correct?
            SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT og.*
					FROM organism_groups og, catalyzes c, pathway_processes pp
					WHERE og.[id] = c.organism_group_id
					AND c.process_id = pp.process_id
					AND pp.pathway_id = @pathway_id",
                "@pathway_id", SqlDbType.UniqueIdentifier, pathwayId);

			return ServerOrganism.LoadMultiple(command);
        }

		/// <summary>
		/// Return a Organism with given ID.
		/// </summary>
		/// <param name="id">
		/// The Guid of the desired Organism.
		/// </param>
		/// <returns>
		/// SoapOrganism object ready to be sent via SOAP.
		/// </returns>
		new public static ServerOrganism Load ( Guid id )
		{
            return new ServerOrganism(LoadRow(id));
            
           // return new ServerOrganism( ServerOrganism.LoadRow( id ), ServerOrganismGroup.LoadRow ( id ) );
		}



        /// <summary>
        /// Returns the row information for the organism part only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        new internal static DBRow LoadRow ( Guid id )
        {
			SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE id = @id;",
				                                        "@id", SqlDbType.UniqueIdentifier, id);

			DataSet ds = new DataSet();
			DBWrapper.LoadSingle( out ds, ref command );

			return new DBRow(ds);
        }



		/// <summary>
		/// Loads a ServerOrganism from a row of its group table.  I would have made this a constructor,
		/// but alas, its parameter type has already been overloaded
		/// </summary>
		/// <param name="group">A row from the organism_groups table (where is_organism=1)</param>
		/// <returns></returns>
		internal static ServerOrganism LoadFromGroupRow(DBRow group)
		{
			// Load the organisms row with the same id as the organism_groups row
			DBRow org = ServerOrganism.LoadRow(group.GetGuid("id"));
			return new ServerOrganism(org, group);
		}



		/// <summary>
		/// Creates an array of ServerOrganism objects from an SQL query for multiple organism rows.
		/// </summary>
		/// <param name="command">A query for multiple rows of organism.  Note that the query must output
		/// all columns of organism (not just the id's)!</param>
		/// <returns></returns>
		new internal static ServerOrganism[] LoadMultiple(SqlCommand command)
		{
			DataSet[] ds;
			DBWrapper.LoadMultiple(out ds, ref command);

			ArrayList results = new ArrayList();
			foreach(DataSet r in ds)
				results.Add(new ServerOrganism(new DBRow(r)));

			return (ServerOrganism[])results.ToArray(typeof(ServerOrganism));
		}



		/// <summary>
		/// Creates a single ServerOrganism from an SQL query
		/// </summary>
		/// <param name="command">An SQL query that returns a single row from organism_groups.  Note that the query
		/// must output every column of organism_groups</param>
		/// <returns></returns>
		new internal static ServerOrganism LoadSingle(SqlCommand command)
		{
			DataSet ds;
			DBWrapper.LoadSingle(out ds, ref command);
			return new ServerOrganism(new DBRow(ds));
		}

		/// <summary>
		/// See if the organism_id is valid
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		new public static bool Exists(Guid id)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id;",
				"@id", SqlDbType.UniqueIdentifier, id);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		
		#endregion


	} // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerOrganism.cs,v 1.2 2009/09/17 20:40:38 ann Exp $
	$Log: ServerOrganism.cs,v $
	Revision 1.2  2009/09/17 20:40:38  ann
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.2  2006/10/19 01:24:42  ali
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.29  2006/07/18 17:04:53  greg
	 - Viewer panel
	The viewer now correctly works with both pathways and processes, both fullscreen and windowed, and with anything you want to throw at it.
	
	 - DisplayWindow rewrite
	DisplayWindow has had a TON of extraneous/confusing stuff in it for a long time, so I finally went through and refactored everything down to the kitchen sink.  Features that we no longer use have been ripped out, and piles of renundant code have been sliced.  Content messages have also been moved into text files that are loaded when required, so changing the messages does not require a recompile of the entire system.
	
	 - RichContent stuff
	I updated the Graph2ContentIntegrationManager thing to fit with what the new system (or whatever I convoluted it into) can better manage, but right now I don't know how to get it to display graphs when coming from a ME without using session variables (which I don't want to for some odd reason).  At least it works, though.
	
	 - Client info collection
	When there is no information in the client session variable, the system will briefly redirect the browser to a page to collect information about the user's screen size for use in displaying Java graphs.
	
	 - Graceful exception handling
	Most errors can now be caught by the application (as opposed to ASP.NET) and displayed in a nice debug window at the bottom of the screen.  Debug reporting can be turned on or off in the application settings.  Since C# doesn't track exceptions across different controls when using LoadControl, it's necessary to put try/catch statements in each control.  I guess that's not really a problem, but it seems like somewhat of an oversight in my opinion.
	
	 - Paging updates
	Brendan had some good ideas for some tweaks to the paging and stuff, do I stuck them in and they seem to work well.  I also found there were a bunch of problems with the paging system in general, but they should be resolved now.
	
	 - Help pages
	A full-fledged help system is now in the works.  Cool beans.
	
	 - Fixing the queries
	All of the queries have been refactored again to use the new graph viewer framework and to... well, work.  Better, at least.  There are still some unresolved right-click issues going on, but I'm working on 'em.
	
	Revision 1.28  2006/07/14 18:04:35  greg
	 - Ajax paging
	Navigating through pages of data on the content browser bar is now instantaneous and/or very fast (whichever is slower).  Yay for Ajax.  It's not 100% bug-free yet, but it's close.
	
	 - Java applets
	Java applets are now contained within collapsible panels.  This required some shuffling of how IGraphSource members are handled, but for the most part everything's more or less the same.  Also not 100% bug-free, but also close.
	
	Revision 1.27  2006/05/26 16:02:45  greg
	 - Testing components
	The PathwaysLibTester has been updated to include support for tests to be run based on command-line arguments using the awesomeness of reflection.
	
	 - LinkHelper
	Some important fixes were made to this class again.  The way it used to be broke some stuff in other pages, but most/all of those issues should be resolved now.
	
	 - Built-in queries
	The one-substrate-and-product query produces something now; whether it's correct or not still needs to be checked.  The fixed code will be moved into ServerObject classes soon.
	
	Revision 1.26  2006/05/23 18:29:32  greg
	Many old SQL queries were updated/optimized, and a bug that causes the system to crash when trying to navigate after viewing the details of an object through the Java applet is also fixed.  This required some semi-substantial modifications to LinkHelper.cs and SearchPagination.ascx.cs to allow for a slightly different method of dealing with query parameters.
	
	Revision 1.25  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.24  2006/05/17 21:02:17  brendan
	Fixed query in server org that was breaking collapsed pathway visualization
	
	Revision 1.23  2006/05/16 23:00:44  gokhan
	*** empty log message ***
	
	Revision 1.22  2006/05/11 21:18:33  brendan
	Fixed numerous bugs, basic browsing starting to work again
	
	Revision 1.21  2006/04/12 21:01:13  brian
	*** empty log message ***
	
	Revision 1.20  2006/03/03 22:33:21  brian
	This should clear all the LegacyBlaster code out of the head.  Let me know if something's still broken.
	
	Revision 1.19  2006/02/28 17:57:04  brian
	*** empty log message ***
	
	Revision 1.18.2.6  2006/02/24 04:47:18  brian
	The new functions appear to be working.  Right now, there is only a demonstration of the new pathway selection routine (we should modify our interface so that it's passing id's, not names of organisms).
	
	Revision 1.18.2.5  2006/02/23 18:13:04  brian
	*** empty log message ***
	
	Revision 1.18.2.4  2006/02/23 05:05:12  brian
	0. Created an OrganismMeta utility to load organisms and groups in batches
	1. Renamed ServerOrganismGroup.GetAllOrganisms() to ServerOrganismGroup.GetChildOrganisms()
	2. Created a ServerOrganismGroup.GetImmediateChildren() function to grab all organisms and groups 1 level deeper
	3. Modified GetChildGroups() so that it only returns groups (not organisms as well)
	                --> We might need to update a PathwaysService object that referenced this
	
	Revision 1.18.2.3  2006/02/22 23:41:42  brian
	1.  Unifying organism and organism_group tables
	2.  Operations to get pathways by organism or group are now handled by polymorphic functions
	
	Revision 1.18.2.2  2006/02/17 21:59:30  brian
	I want to perform a commit before I start blowing things up.  My plans are as follows:
	
	1. Extend IOrganismEntity to include an AllPaths() method.  Depending upon whether the instance is
	    an organism or an organism group, it will find the organism paths or the paths of all organisms under
	    the group, respectively.
	2. I'm leaning towards creating a meta-class to encapsulate organism-groups and organisms.  As we discussed
	    today, the functions of the two are merging, so we need to support this in the class implementation
	
	Revision 1.18.2.1  2006/02/16 23:25:51  brian
	1. Updated a few minor sections to use PathwaysLib3
	2. Need to find a way to query multiple entities (ex, organisms and organism groups)
	
	Revision 1.18  2005/12/05 03:04:17  gokhan
	FindOrganisms and FindOrganismGroups methods are added
	
	Revision 1.17  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.16  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.15  2005/08/19 21:33:42  brandon
	cleaned up some files, added some comments
	
	Revision 1.14  2005/07/28 16:06:05  brandon
	almost done with the ProcessInvolvingEntityInPathway.ascx, added a IOrganismEntity[] AllOrganismEntities( ) static function to ServerOrganism.cs
	
	Revision 1.13  2005/07/20 22:31:20  brandon
	Added exists to Pathway and Process, add Exists for names in Organism and OrganismGroup
	
	Revision 1.12  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.11  2005/07/15 21:02:00  brandon
	added more queries
	
	Revision 1.10  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.9  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.8  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.7  2005/06/29 16:44:53  brandon
	Added Insert, Update, and Delete support to these files if they didn't already have it
	
	Revision 1.6  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.5  2005/06/27 15:44:22  brandon
	revised ServerOrganism.cs to the new format, not sure when to use 'organism_notes' vs. 'notes'
	
	Revision 1.4  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.3  2005/06/22 18:39:11  michael
	Changing data model again to encapsulate the ADO.NET funcationality further.
	Updating the classes that used the old functionality to use the new DBRow class.
	
	Revision 1.2  2005/06/21 20:48:12  brandon
	separated get and set for the id (primary key), made the setter private
	
	Revision 1.1  2005/06/21 19:31:13  brandon
	Added ServerOrganism.cs and fixed some errors in ServerProcess.cs
	

------------------------------------------------------------------------*/
#endregion