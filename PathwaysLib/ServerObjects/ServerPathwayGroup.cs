#region Using Declarations
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Text;

	using PathwaysLib.SoapObjects;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Exceptions;
#endregion

namespace PathwaysLib.ServerObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerPathwayGroup.cs</filepath>
	///		<creation>2005/06/30</creation>
	///		<author>
	///			<name>Michael Starke</name>
	///			<initials>mfs</initials>
	///			<email>michael.starke@case.edu</email>
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
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerPathwayGroup.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to PathwayGroups.
	/// </summary>
	#endregion
	public class ServerPathwayGroup : ServerObject
	{

		#region Constructor, Destructor, ToString
		/// <summary>
		/// Creating constructor for the server pathway group wrapper
		/// </summary>
		/// <remarks>Private; No insertion at this time.</remarks>
		private ServerPathwayGroup ()
		{
		}

		/// <summary>
		/// Constructor for server pathway group.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerPathwayGroup object from a
		/// SoapPathwayGroup object.
		/// </remarks>
		/// <param name="data">
		/// A SoapPathwayGroup object from which to construct the
		/// ServerPathwayGroup object.
		/// </param>
		public ServerPathwayGroup ( SoapPathwayGroup data )
		{
			// (mfs) setup database row
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
					__DBRow = LoadRow(data.Id);
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}

			// (mfs) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
			{
				UpdateFromSoap(data);
			}
		}

		/// <summary>
		/// Constructor for server pathway group.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerPathwayGroup object from a
		/// DBRow.
		/// </remarks>
		/// <param name="data">
		/// DBRow to load into the object.
		/// </param>
		public ServerPathwayGroup ( DBRow data ) 
		{
			// (mfs)
			// setup object
			__DBRow = data;
		}

		/// <summary>
		/// Destructor for the ServerPathwayGroup class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerPathwayGroup()
		{
		}
		#endregion


		#region Member Variables

        public static readonly string UnspecifiedGroup_id = "00000000-0000-0000-0000-000000000000";
		private static readonly string __TableName = "pathway_groups";
		private static readonly string __TablePathwayGroupAnnotationCounts = "go_pathway_group_annotation_counts";
		#endregion

		#region Properties
		/// <summary>
		/// Get/set the Id of this pathwaygroup.
		/// </summary>
		public Guid Id
		{
			get{return __DBRow.GetGuid("group_id");}
			set{__DBRow.SetGuid("group_id", value);}
		}
		/// <summary>
		/// Get/set the name of this pathwaygroup.
		/// </summary>
		public string Name 
		{
			get{return __DBRow.GetString("name");}
			set{__DBRow.SetString("name", value);}
		}
		/// <summary>
		/// Get/set the notes for this pathwaygroup.
		/// </summary>
		public string Notes
		{
			get{return __DBRow.GetString("notes");}
			set{__DBRow.SetString("notes", value);}
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
		public override SoapObject PrepareForSoap(SoapObject derivedObject)
		{
			SoapPathwayGroup retval = new SoapPathwayGroup();
			retval.Id     = this.Id;
			retval.Name   = this.Name;
			retval.Notes  = this.Notes;
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
			SoapPathwayGroup pg = o as SoapPathwayGroup;

            if (o.Status == ObjectStatus.Insert && pg.Id == Guid.Empty)
                pg.Id = DBWrapper.NewID(); // generate a new ID

			this.Id = pg.Id;
			this.Name = pg.Name;
			this.Notes = pg.Notes;
		}

		/// <summary>
		/// Get all pathways, unpaged.
		/// </summary>
		/// <returns>
		/// Set of all pathways in this pathwaygroup.
		/// </returns>
		public ServerPathway[] GetAllPathways()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT p.*
					FROM pathway_to_pathway_groups ptpg
					JOIN pathways p ON ptpg.pathway_id = p.id
					WHERE ptpg.group_id = @id
                    ORDER BY p.name",
					"@id", SqlDbType.UniqueIdentifier, this.Id
			);

			DataSet[] ds = new DataSet[0];
			int n = DBWrapper.LoadMultiple( out ds, ref command );
			
			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ));
			}

			return ((ServerPathway[]) results.ToArray( typeof(ServerPathway)));
		}

		#region GO Enrichment Methods
		/// <summary>
		/// Get the number of times a GO term annotates processes
		/// within pathways in this pathway group
		/// </summary>
		/// <param name="GOID">The 7-digit GO id with leading zeros</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <returns>The number of times the GO term annotates</returns>
		public int GetGOAnnotationCount(string GOID, int level)
		{
			return GetGOAnnotationCount(this.Id, GOID, level);
		}

//		/// <summary>
//		/// Get the significance of an annotation count within the pathway group
//		/// using hypergeometric distribution
//		/// </summary>
//		/// <param name="GOID">The GO id</param>
//		/// <param name="level">The level of the GO hierarchy</param>
//		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
//		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
//		/// <returns>The P-value of the annotation significance</returns>
//		public double GetGOEnrichmentSignificance(string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses)
//		{
//			return GetGOEnrichmentSignificance(this.Id, GOID, level, sampleAnnotationSize, sampleAnnotationSuccesses);
//		}

		/// <summary>
		/// Get the significance of an annotation count within a pathway group
		/// by computing the enrichment ratio and enrichment significance
		/// </summary>
		/// <param name="GOID">The GO term id</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
		/// <param name="enrichmentRatio">observed annotation rate / expected annotation rate</param>
		/// <param name="enrichmentSignificance">The significance of the annotation, based upon the hypergeometric distribution</param>
		/// <param name="expectedSuccesses">Expected successes</param>
		public void GetGOEnrichment(string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses, out double enrichmentRatio, out double enrichmentSignificance, out double expectedSuccesses)
		{
			GetGOEnrichment(this.Id, GOID, level, sampleAnnotationSize, sampleAnnotationSuccesses, out enrichmentRatio, out enrichmentSignificance, out expectedSuccesses);
		}

		/// <summary>
		/// Retrieves the intersection of 'GOIDs' and all annotating terms in the pathway group
		/// </summary>
		/// <param name="annotatingTerms">The terms which we know annotate the pathway in the group</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="missingTerms">The array of terms which annotate in the group, but are not in the 'GOIDs' list</param>
		/// <param name="missingTotalCounts">The total number of times a term at the same index in 'missingTerms' annotates in the group</param>
		public void GetMissingGroupGOTerms(ServerGOTerm[] annotatingTerms, int level, out ServerGOTerm[] missingTerms, out int[] missingTotalCounts)
		{
			GetMissingGroupGOTerms(this.Id, annotatingTerms, level, out missingTerms, out missingTotalCounts);
		}

		#endregion

		#region ADO.NET SqlCommands
		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + "(group_id, name, notes) VALUES (@id, @name, @notes )",
					"@id", SqlDbType.UniqueIdentifier, this.Id,
					"@name", SqlDbType.VarChar, this.Name,
					"@notes", SqlDbType.Text, this.Notes
			);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE group_id = @id;",
					"@id", SqlDbType.UniqueIdentifier, this.Id
			);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + " SET name = @name, notes = @notes WHERE group_id = @id;",
					"@name", SqlDbType.VarChar, this.Name,
					"@notes", SqlDbType.VarChar, this.Notes,
					"@id", SqlDbType.UniqueIdentifier, this.Id
			);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE group_id = @id;",
					"@id", SqlDbType.UniqueIdentifier, this.Id
			);
		}
		#endregion
		#endregion


		#region Static Methods

		#region GO Term Enrichment Methods
		/// <summary>
		/// Get the number of times a GO term annotates processes
		/// within pathways in this pathway group
		/// </summary>
		/// <param name="GOID">The 7-digit GO id with leading zeros</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="pathwayGroupID">The ID of the pathway group</param>
		/// <returns>The number of times the GO term annotates</returns>
		public static int GetGOAnnotationCount(Guid pathwayGroupID, string GOID, int level)
		{
			object o = DBWrapper.Instance.ExecuteScalar(
				"select number_annotations from " + __TablePathwayGroupAnnotationCounts + " where pathway_group_id=@groupid and go_id=@goid and hierarchy_level=@level",
				"@groupid", SqlDbType.UniqueIdentifier, pathwayGroupID,
				"@goid", SqlDbType.VarChar, GOID,
				"@level", SqlDbType.Int, level);
			if(o == DBNull.Value || o == null) return 0;
			if(!(o is int)) throw new InvalidCastException("Could not cast number_annotatoins while in ServerPathwayGroup.GetAnnotationCount(Guid, string, int)");
			return (int)o;
		}

		/// <summary>
		/// Get the total number of annotations from all GO terms
		/// within pathways in this pathway group at the given level
		/// </summary>
		/// <param name="pathwayGroupID">The pathway group</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <returns>The total number of annotations</returns>
		public static int GetGOAnnotationCount(Guid pathwayGroupID, int level)
		{
			object o = DBWrapper.Instance.ExecuteScalar(
				"select sum(number_annotations) from " + __TablePathwayGroupAnnotationCounts + " where pathway_group_id=@groupid and hierarchy_level=@level",
				"@groupid", SqlDbType.UniqueIdentifier, pathwayGroupID,
				"@level", SqlDbType.Int, level);
			if(o == DBNull.Value) return 0;
			if(!(o is int)) throw new InvalidCastException("Could not cast number_annotations while in ServerPathwayGroup.GetGOAnnotationCount(Guid, int)");
			return (int)o;
		}

//		/// <summary>
//		/// Get the significance of an annotation count within a pathway group
//		/// using hypergeometric distribution
//		/// </summary>
//		/// <param name="pathwayGroupID">The pathway group</param>
//		/// <param name="GOID">The GO id</param>
//		/// <param name="level">The level of the GO hierarchy</param>
//		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
//		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
//		/// <returns>The P-value of the annotation significance</returns>
//		public static double GetGOEnrichmentSignificance(Guid pathwayGroupID, string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses)
//		{
//			//get the total count for this group
//			int totalCount = GetGOAnnotationCount(pathwayGroupID, level);
//
//			//get the term annotation count for this group
//			int totalSuccesses = GetGOAnnotationCount(pathwayGroupID, GOID, level);
//
//			//compute the hypergeometric distribution
//			double ret = PathwaysLib.Utilities.HypergeometricDistribution.Evaluate(sampleAnnotationSize, sampleAnnotationSuccesses, totalCount, totalSuccesses);
//			return ret;
//		}

//		/// <summary>
//		/// Get the significance of an annotation count within a pathway group
//		/// by computing the enrichment ratio, which compares the expected
//		/// annotation rate (the rate of annotation in the given pathway group)
//		/// with the observed annotation rate
//		/// </summary>
//		/// <param name="pathwayGroupID">The pathway group</param>
//		/// <param name="GOID">The GO term id</param>
//		/// <param name="level">The level of the GO hierarchy</param>
//		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
//		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
//		/// <returns>observed annotation rate / expected annotation rate</returns>
//		public double GetGOEnrichmentRatio(Guid pathwayGroupID, string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses)
//		{
//			//get the total annotation count for this group
//			double totalCount = (double)GetGOAnnotationCount(pathwayGroupID, level);
//
//			//get the term annotation count for this group
//			double totalSuccesses = (double)GetGOAnnotationCount(pathwayGroupID, GOID, level);
//
//			double expectedRate = ((double)sampleAnnotationSize/totalCount) * totalSuccesses;
//
//			if(expectedRate == 0 && sampleAnnotationSuccesses > 0)
//				return double.MaxValue;
//
//			return ((double)sampleAnnotationSuccesses)/expectedRate;
//		}

		/// <summary>
		/// Returns the expected annotation rate for a given GO term at a level of the GO hierarchy
		/// </summary>
		/// <param name="pathwayGroupID">The id of the pathway group</param>
		/// <param name="GOID">The ID of the GO term</param>
		/// <param name="level">The level of the go hierarchy</param>
		/// <returns></returns>
		public static double GetGOExpectedRatio(Guid pathwayGroupID, string GOID, int level)
		{
			return (double)GetGOAnnotationCount(pathwayGroupID, GOID, level)/(double)GetGOAnnotationCount(pathwayGroupID, level);
		}

		/// <summary>
		/// Get the significance of an annotation count within a pathway group
		/// by computing the enrichment ratio and enrichment significance
		/// </summary>
		/// <param name="pathwayGroupID">The pathway group</param>
		/// <param name="GOID">The GO term id</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
		/// <param name="enrichmentRatio">observed annotation rate / expected annotation rate</param>
		/// <param name="enrichmentSignificance">The significance of the annotation, based upon the hypergeometric distribution</param>
		/// <param name="expectedSuccesses">The number of successes expected</param>
		public static void GetGOEnrichment(Guid pathwayGroupID, string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses, out double enrichmentRatio, out double enrichmentSignificance, out double expectedSuccesses)
		{
			//get the total count for this group
			int totalCount = GetGOAnnotationCount(pathwayGroupID, level);

			//get the term annotation count for this group
			int totalSuccesses = GetGOAnnotationCount(pathwayGroupID, GOID, level);

			//compute the hypergeometric distribution
			enrichmentSignificance = PathwaysLib.Utilities.HypergeometricDistribution.Evaluate(sampleAnnotationSize, sampleAnnotationSuccesses, totalCount, totalSuccesses);
			
			
			expectedSuccesses = ((double)sampleAnnotationSize/totalCount) * totalSuccesses;

			enrichmentRatio = ((double)sampleAnnotationSuccesses)/expectedSuccesses;
		}

		/// <summary>
		/// Retrieves the intersection of 'GOIDs' and all annotating terms in the pathway group
		/// </summary>
		/// <param name="pathwayGroupID">The group to check</param>
		/// <param name="annotatingTerms">The GO terms which we know annotate the pathway in the group</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="missingTerms">The array of terms which annotate in the group, but are not in the 'GOIDs' list</param>
		/// <param name="missingTotalCounts">The total number of times a term at the same index in 'missingTerms' annotates in the group</param>
		public static void GetMissingGroupGOTerms(Guid pathwayGroupID, ServerGOTerm[] annotatingTerms, int level, out ServerGOTerm[] missingTerms, out int[] missingTotalCounts)
		{
			string at = string.Empty;
			foreach( ServerGOTerm term in annotatingTerms ) at += " AND go_id <> '" + term.ID + "'";				

			SqlCommand com = DBWrapper.BuildCommand(
				@"SELECT go_id, number_annotations
					FROM " + __TablePathwayGroupAnnotationCounts + @"
					WHERE pathway_group_id = @groupid AND hierarchy_level = @level" + at,
				"@groupid", SqlDbType.UniqueIdentifier, pathwayGroupID,
				"@level", SqlDbType.Int, level);
			DataSet[] dsets;
			int count = DBWrapper.LoadMultiple(out dsets, ref com);
			missingTerms = new ServerGOTerm[count];
			missingTotalCounts = new int[count];
			for(int i=0; i<count; i++)
			{
				// (GJS) TODO: Sometimes this load call fails because the ID isn't in the database;
				// is this an error in the database?
				// Fix (for now): anything that causes an error becomes null
				try
				{
					missingTerms[i] = ServerGOTerm.Load((string)dsets[i].Tables[0].Rows[0]["go_id"]);
				}
				catch( DBWrapperException )
				{
					missingTerms[i] = null;
				}

				missingTotalCounts[i] = (int)dsets[i].Tables[0].Rows[0]["number_annotations"];
			}
		}

		#endregion

		
        /// <summary>
		/// Returns all pathway groups who's name contains the given substring
		/// </summary>
		/// <param name="substring"></param>
		/// <param name="searchMethod"></param>
		/// <returns></returns>
		public static ServerPathwayGroup[] FindPathwayGroups(string substring, SearchMethod searchMethod)
		{
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE [name] " +
				( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + @" @substring
				ORDER BY [name];",
				"@substring", SqlDbType.VarChar, substring );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathwayGroup( new DBRow( d ) ) );
			}

			return ( ServerPathwayGroup[] ) results.ToArray( typeof( ServerPathwayGroup ) );
		}

		/// <summary>
		/// Get all of the pathway groups from the system.
		/// </summary>
		/// <returns>
		/// Set of pathway groups.
		/// </returns>
		public static ServerPathwayGroup[] AllPathwayGroups()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " ORDER BY name ASC"
			);
			DataSet[] ds = new DataSet[0];
			int n = DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathwayGroup( new DBRow( d ) ) );
			}

			return ((ServerPathwayGroup[])results.ToArray( typeof(ServerPathwayGroup)));
		}

        /// <summary>
        /// this is specifically for getting the name and ID of all pathwaygroup as a dataset
        ///  (written for the PathwaysWithinStepsFromPathway user control)
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllPathwayGroupsForQueryInterface()
        {
            SqlCommand command = new SqlCommand("SELECT [group_id], [name] FROM " + __TableName + " ORDER BY [name];");
            DataSet ds;
            DBWrapper d = new DBWrapper();
            int r = d.ExecuteQuery(out ds, ref command);

            return ds;
        }

		/// <summary>
		/// Load a pathwaygroup from its id.
		/// </summary>
		/// <param name="id">
		/// The id of the desired object.
		/// </param>
		/// <returns>
		/// The DBRow holding the object's data.
		/// </returns>
		public static DBRow LoadRow( Guid id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE group_id = @id",
					"@id", SqlDbType.UniqueIdentifier, id 
			);
			DataSet ds = DBWrapper.GetSchema( __TableName );
			int n = DBWrapper.LoadSingle( out ds, ref command );

			return new DBRow( ds );
		}

		/// <summary>
		/// Load a pathway group from its id
		/// </summary>
		/// <param name="groupID">The id of the pathwaygroup</param>
		/// <returns></returns>
		public static ServerPathwayGroup Load(Guid groupID)
		{
			return new ServerPathwayGroup(LoadRow(groupID));
		}

		/// <summary>
		/// Adds a tuple to the pathway_to_pathway_groups table, which denotes the group of a pathway
		/// </summary>
		/// <param name="pathwayId"></param>
		/// <param name="groupId"></param>
		/// <returns></returns>
		public static void AddPathwayGroupLink(Guid pathwayId, Guid groupId)
		{
			if ( !PathwayGroupLinkExists(pathwayId, groupId) )
			{
				DBWrapper.Instance.ExecuteNonQuery(				
					"INSERT INTO pathway_to_pathway_groups ( pathway_id, group_id) VALUES ( @pathwayId, @groupId);",
					"@pathwayId", SqlDbType.UniqueIdentifier, pathwayId,
					"@groupId", SqlDbType.UniqueIdentifier, groupId);
			}
			else 
			{
				//do nothing, the relation already exists
			}
		}

		/// <summary>
		/// Removes the given pathway_to_pathway_groups link.
		/// </summary>
		/// <param name="pathwayId"></param>
		/// <param name="groupId"></param>
		
		public static void RemovePathwayGroupLink ( Guid pathwayId, Guid groupId)
		{
			if (DBWrapper.Instance.ExecuteNonQuery(				
				"DELETE FROM pathway_to_pathway_groups WHERE pathway_id = @i_pathway_id AND group_id = @i_groupId;",
				"@i_pathway_id", SqlDbType.UniqueIdentifier, pathwayId,
				"@i_group_id", SqlDbType.UniqueIdentifier, groupId) < 1)
			{
				throw new DataModelException("Remove pathway group link between {0} and {1} failed!", pathwayId, groupId);
			}
		}

		/// <summary>
		/// Tells whether an entry already exists in the pathway_to_pathway_groups table with
		/// the given pathwayId and groupId
		/// </summary>
		/// <param name="pathwayId"></param>
		/// <param name="groupId"></param>
		/// <returns>
		/// True if the tuple exists in the pathway_to_pathway_groups table, false if it doesn't
		/// </returns>
		public static bool PathwayGroupLinkExists ( Guid pathwayId,Guid groupId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM pathway_to_pathway_groups WHERE pathway_id = @i_pathway_id AND group_id = @i_group_id ;",
				"@i_pathway_id", SqlDbType.UniqueIdentifier, pathwayId,
				"@i_group_id", SqlDbType.UniqueIdentifier, groupId);

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
	$Id: ServerPathwayGroup.cs,v 1.1 2008/05/16 21:15:53 mustafa Exp $
	$Log: ServerPathwayGroup.cs,v $
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.4  2007/08/24 19:44:38  ann
	Changes made in queries based on the new dataset field
	
	Revision 1.3  2007/05/22 20:12:22  brendan
	added new build-in query for show a set of pathways, fixed bug with displaying graph legend
	
	Revision 1.2  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.11  2006/07/09 15:22:09  greg
	 - Browser highlighting issues
	Apparently I hadn't fixed the highlighting issues like I thought I had, but it seems to be working fine now.
	
	 - Stuff autoloading that probably shouldn't
	All the ME data for processes (and perhaps other things) were automatically loading, even for items that weren't open.  I didn't really notice this until I started testing the browser with the Kegg data; since Kegg has a zillion more entries than our own database, the performance hit could only be realized when trying to load up extraneous information for hundreds of items at once.  Woof.
	
	 - 0x80040111 (NS_ERROR_NOT_AVAILABLE) error
	Apparently, certain kinds of browsing patterns may interrupt Ajax requests in a way Firefox doesn't like, thus causing this error to be thrown.  I resolved it with a try/catch block, but I'll keep my oeyes open for any additional fishiness.
	
	 - Built-in queries updates
	All built-in queries now include a link to jump to the graph visualization, which is useful when there are hundreds of results and it's not immediately clear that there is a graph to see.  Item pre-selection is fixed for several queries, but not all of them; Brandon and I are pumping those out as quickly as we can.
	
	Revision 1.10  2006/06/30 20:02:38  greg
	There have been some very big changes here lately...
	
	 - Query logger
	The DBWrapper class now has support for logging queries in a format that you can import into Excel, etc. for analysis.  There are some Web.config lines you'll have to add, though.
	
	 - JavaScript redirects
	The dropdown list on the main browser bar uses JavaScript for redirects.  Yay.
	
	 - Visual issues
	There were several unresolved visual issues (mostly stemming from the way IE and Firefox render pages differently), but most of them should now be resolved.
	
	 - Ajax browsing
	The biggest part of this update involves Ajax.  All pages load significantly faster now, and data requests are made asynchronously.  The fine details about which panels will start open by default and everything can be worked out later... but for now it appears that everything is working nicely.
	
	Revision 1.9  2006/06/15 00:36:38  greg
	The GO pathway viewer should be almost completely operational now; any issues should be relatively minor and easily fixable.  Substantial XHTML/CSS updates were done to make more pages compatable with both IE and non-IE browsers, and some spelling/grammar updates were made.  There are still some known issues with content displaying funkily in IE that will be addressed soon.
	
	Revision 1.8  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.7  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	
	Revision 1.6  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.5.6.2  2006/05/11 15:53:27  marc
	HUGE COMMIT!!!!
	
	Revision 1.5.6.1  2006/03/08 20:11:30  marc
	Added methods for pathway enrichment
	
	Revision 1.5  2005/11/08 21:04:14  gokhan
	There are some changes in the ServerPathwayGroup (such as new static methods for searching a pathway group and adding a tuple to the relation pathway_to_pathway_groups)
	
	Revision 1.4  2005/11/07 23:03:42  gokhan
	this is the new version added the insert functionality to the pathway_groups class
	
	Revision 1.3  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.2  2005/10/13 21:27:44  michael
	*** empty log message ***
	
	Revision 1.1  2005/10/13 18:55:58  michael
	Adding groupings for Pathways
		
------------------------------------------------------------------------*/
#endregion