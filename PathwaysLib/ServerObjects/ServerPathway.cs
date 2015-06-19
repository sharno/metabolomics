#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using PathwaysLib.Utilities;
using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.GraphObjects;
using System.Collections.Generic;
#endregion

namespace PathwaysLib.ServerObjects
{	
	
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/Server/ServerPathway.cs</filepath>
    ///		<creation>2005/06/08</creation>
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
	///				<name>Brandon S. Evans</name>
	///				<initials>bse</initials>
	///				<email>brandon.evans@case.edu</email>
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
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: murat $</cvs_author>
    ///			<cvs_date>$Date: 2010/11/19 21:13:29 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerPathway.cs,v 1.12 2010/11/19 21:13:29 murat Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.12 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Encapsulates database access related to biological pathways.
    /// </summary>
    #endregion
    public class ServerPathway : ServerObject, IGraphPathway
    {

        #region Constructor, Destructor, ToString
		/// <summary>
		/// Constructor
		/// </summary>
        private ServerPathway ( )
        {
        }

		/// <summary>
		/// Constructor, creates a new pathway with fields initiallized
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="pathwayStatus"></param>
		/// <param name="pathwayNotes"></param>
        public ServerPathway ( string name, string type, string pathwayStatus, string pathwayNotes)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow( __TableName );

            this.ID = DBWrapper.NewID(); // generate a new ID
            this.Name = name;
            this.Type = type;
			this.PathwayStatus = pathwayStatus;
			this.PathwayNotes = pathwayNotes;
           

        }

        /// <summary>
        /// Constructor for server pathway wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerPathway object from a
        /// SoapPathway object.
        /// </remarks>
        /// <param name="data">
        /// A SoapPathway object from which to construct the
        /// ServerPathway object.
        /// </param>
        public ServerPathway ( SoapPathway data )
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

			// (mfs)
			// required call to setup SqlCommands
    	    //SetSqlCommandParameters( );
        }

		/// <summary>
		/// Constructor for server pathway wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerPathway object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerPathway ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;
		}

        /// <summary>
        /// Destructor for the ServerPathway class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerPathway()
        {
        }
        #endregion


        #region Member Variables
      
        public static readonly string UnspecifiedPathway = "00000000-0000-0000-0000-000000000000";
		private static readonly string __TableName = "pathways";
		private static readonly string __TablePathwayAnnotationCounts = "go_pathway_annotation_counts";
        #endregion


        #region Properties
		/// <summary>
		/// Get/set the Pathway ID.
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
		/// Get/set the Pathway name.
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
		/// Get/set the Pathway type. Wraps TypeId.
		/// </summary>
		public string Type
		{
            get
            {
                return PathwaysTypeManager.GetPathwayTypeName(TypeId);
            }
            set
            {
                TypeId = PathwaysTypeManager.GetPathwayTypeId(value); // (ac) fails if the string is not in the db.
            }
        }

        /// <summary>
        /// Get/set pathway type ID.
        /// </summary>
		public int TypeId
		{
			get
			{
				return __DBRow.GetInt("pathway_type_id");
			}
			set
			{
				__DBRow.SetInt("pathway_type_id", value);
			}
		}

		/// <summary>
		/// Get/set the Pathway notes.
		/// </summary>
		public string PathwayNotes
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

		/// <summary>
		/// Get/set the Pathway status.
		/// </summary>
		public string PathwayStatus
		{
			get
			{
				return __DBRow.GetString("status");
			}
			set
			{
				__DBRow.SetString("status", value);
			}
		}

      
        #endregion


        #region Methods
        /// <summary>
        /// Returns the set of organisms that have at least one mapped gene encoding an enzyme of a pathway in a given set of comma separated pathways.
        /// </summary>
        /// <param name="pathwayIdSet"></param>
        /// <returns></returns>
        public static string GetGenomesForASetOfPathways(string pathwayIdSet)
        {
            char[] delim = { ',' };
            string[] ids = pathwayIdSet.Split(delim);
            string queryInPredicate = "";
            foreach (string id in ids)
                queryInPredicate += "\'" + id.Trim() + "\',";

            queryInPredicate = queryInPredicate.Substring(0, queryInPredicate.Length - 1);
            queryInPredicate = " (" + queryInPredicate + ") ";


            //get chromosome list for each organism that contains the given pathway
            string query = @"select distinct g.organism_group_id 
                                from pathway_processes pp, catalyzes c, gene_encodings ge, genes g, chromosomes ch
                                where pp.pathway_id IN" + queryInPredicate + @"
                                and pp.process_id = c.process_id
                                and c.gene_product_id = ge.gene_product_id
                                and ge.gene_id = g.id
                                and c.organism_group_id = g.organism_group_id
                                and ch.id = g.chromosome_id
                                and ch.length > 0";

            SqlCommand command;
            command = DBWrapper.BuildCommand(query);
            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            string orgId;
            StringBuilder xml = new StringBuilder("");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                orgId = dr["organism_group_id"].ToString();
                xml.Append("<genome organismgroupid=\"" + orgId + "\"></genome>");
            }
            return xml.ToString();

        }
		/// <summary>
		/// Adds a tuple to the pathway_links table, which denotes a linking molecular_entity between two pathways
		/// </summary>
		/// <param name="pathwayId1"></param>
		/// <param name="pathwayId2"></param>
		/// <param name="entityId"></param>
		/// <param name="notes"></param>
		/// <returns></returns>
		public static void AddPathwayLink(Guid pathwayId1, Guid pathwayId2, Guid entityId, string notes)
		{
			if ( !PathwayLinkExists(pathwayId1, pathwayId2, entityId) )
			{
				DBWrapper.Instance.ExecuteNonQuery(				
					"INSERT INTO pathway_links ( pathway_id_1, pathway_id_2, entity_id, notes ) VALUES ( @i_pathway_id1, @i_pathway_id2, @i_entity_id, @i_notes);",
					"@i_pathway_id1", SqlDbType.UniqueIdentifier, pathwayId1,
					"@i_pathway_id2", SqlDbType.UniqueIdentifier, pathwayId2,
					"@i_entity_id", SqlDbType.UniqueIdentifier, entityId,
					"@i_notes", SqlDbType.Text, notes);
			}
			else 
			{
				//do nothing, the relation already exists
			}
		}

		/// <summary>
		/// Removes the given pathway link.
		/// </summary>
		/// <param name="pathwayId1"></param>
		/// <param name="pathwayId2"></param>
		/// <param name="entityId"></param>
		public static void RemovePathwayLink ( Guid pathwayId1, Guid pathwayId2, Guid entityId )
		{
			if (DBWrapper.Instance.ExecuteNonQuery(				
				"DELETE FROM pathway_links WHERE pathway_id_1 = @i_pathway_id_1 AND pathway_id_2 = @i_pathway_id_2 AND entity_id = @i_entity_id;",
				"@i_pathway_id_1", SqlDbType.UniqueIdentifier, pathwayId1,
				"@i_entity_id", SqlDbType.UniqueIdentifier, entityId,
				"@i_pathway_id_2", SqlDbType.UniqueIdentifier, pathwayId2) < 1)
			{
				throw new DataModelException("Remove pathway link between {0} and {1} with molecule {2} failed!", pathwayId1, pathwayId2, entityId);
			}
		}

		/// <summary>
		/// Tells whether an entry already exists in the pathway_links table with
		/// the given pathwayId1, pathwayId2, and entityId
		/// </summary>
		/// <param name="pathwayId1"></param>
		/// <param name="pathwayId2"></param>
		/// <param name="entityID"></param>
		/// <returns>
		/// True if the tuple exists in the pathway_links table, false if it doesn't
		/// </returns>
		public static bool PathwayLinkExists ( Guid pathwayId1,Guid pathwayId2, Guid entityID)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + "pathway_links" + " WHERE pathway_id_1 = @pathway_id1 AND pathway_id_2 = @pathway_id2 AND entity_id = @entityId;",
				"@pathway_id1", SqlDbType.UniqueIdentifier, pathwayId1,
				"@pathway_id2", SqlDbType.UniqueIdentifier, pathwayId2,
				"@entityId", SqlDbType.UniqueIdentifier, entityID);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
        public static ArrayList GetMappingPathwayByModelID(Guid modelid)        
        {
                      
            string mapTable="MapModelsPathways";
            string query = "SELECT * FROM " + mapTable + " WHERE modelId='"+ modelid.ToString()+"'";

            //SqlCommand command = DBWrapper.BuildCommand(query, "@modelid", SqlDbType.UniqueIdentifier, modelid);
            SqlCommand command = DBWrapper.BuildCommand(query);// new SqlCommand(query);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            
            //int i = 0;
            if (ds.Length == 0) return null;
            foreach (DataSet d in ds)
            {
                results.Add(d.Tables[0].Rows[0]["pathwayId"]);
            }
            return results;  
        }

        /// <summary>
        /// Returns all organisms and organism groups that contain this pathway (i.e. contain one of its processes).
        /// </summary>
        /// <returns></returns>
        public ServerOrganismGroup[] GetAllOrganismGroups()
        {
            return ServerOrganismGroup.AllOrganismGroupsForPathway(this.ID);
			//return ServerOrganism.AllOrganismGroupsForPathway(this.ID);

			/*
            ServerOrganism orgs = ServerOrganism.AllOrganismsForPathway(this.ID);
            ServerOrganismGroup[] groups = ServerOrganismGroup.GetAllOrganismGroupsForPathway(this.ID);

            ServerOrganismGroup[] entities = new ServerOrganismGroup[orgs.Length + groups.Length];

            orgs.CopyTo(entities, 0);
            groups.CopyTo(entities, orgs.Length);

            return entities;
			*/
        }

        IGraphOrganismGroup[] IGraphPathway.GetAllIGraphOrganismGroups()
        {
            return (IGraphOrganismGroup[])GetAllOrganismGroups();
        }


		/// <summary>
		/// Returns a representation of this object suitable for being
		/// sent to a client via SOAP.
		/// </summary>
		/// <returns>
		/// A SoapObject object capable of being passed via SOAP.
		/// </returns>
		public override SoapObject PrepareForSoap ( SoapObject derived )
		{
            SoapPathway retval = (derived == null) ? 
                retval = new SoapPathway() : retval = (SoapPathway)derived;

			retval.ID   = this.ID;
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
			SoapPathway p = o as SoapPathway;

            if (o.Status == ObjectStatus.Insert && p.ID == Guid.Empty)
                p.ID = DBWrapper.NewID(); // generate a new ID

			this.ID = p.ID;
			this.Name = p.Name;
			this.Type = p.Type;
			this.PathwayStatus = p.PathwayStatus;
			this.PathwayNotes = p.PathwayNotes;
		}

		/// <summary>
		/// Derives the Hashcode from the Guid ID of the pathway
		/// </summary>
		/// <returns>The Hashcode</returns>
		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		/// <summary>
		/// Returns a set of GenericProcessID / GOTerm annotation sets for this pathway
		/// at the specified level.
		/// </summary>
		/// <param name="level">The level of the GO hierarchy from which annotations come</param>
		/// <param name="mergeTermsWithSameAnnotations">true to merge set of generic processes 
		/// into one ServerGOTermProcess object if they share the same GO annotations</param>
		/// <returns>A set of GenericProcessID / GOTerm annotation sets</returns>
		public ServerGOTermProcess[] GetGOAnnotatingTerms(int level, bool mergeTermsWithSameAnnotations)
		{
			return ServerGOTermProcess.LoadFromPathway(this, level, mergeTermsWithSameAnnotations);
		}

		
        /// <summary>
        /// Return all models for this pathway.
        /// </summary>
        /// <returns></returns>
        public ServerModel[] GetAllModels()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM model
				WHERE [id] IN ( SELECT modelId
									FROM MapModelsPathways
									WHERE pathwayId = @id )
				ORDER BY sbmlId;",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModel(new DBRow(d)));
            }

            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        /// <summary>
        /// Return all model annotations for this pathway.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, BiomodelAnnotation> GetAllModelAnnotations()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT modelId, qualifierId
				FROM MapModelsPathways
				WHERE pathwayId = @id;",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            if (ds.Length == 0)
                return null;

            Dictionary<Guid, BiomodelAnnotation> anns = new Dictionary<Guid, BiomodelAnnotation>();
            BiomodelAnnotation ann = null;
            foreach (DataSet d in ds)
            {
                DataRow row = d.Tables[0].Rows[0];
                Guid modelId = new Guid(row["modelId"].ToString());
                if (anns.ContainsKey(modelId))
                    ann = anns[modelId];
                else
                {
                    ann = new BiomodelAnnotation(modelId);
                    anns.Add(ann.EntityId, ann);
                }
                ann.QualifierIds.AddFirst(int.Parse(row["qualifierId"].ToString()));
            }

            return anns;
        }

        #region Process "belongs to" pathway relation
        /// <summary>
		/// Return all processes for this pathway.
		/// </summary>
		/// <returns></returns>
		public ServerProcess[] GetAllProcesses ( )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT pro.*
				FROM processes pro
				WHERE pro.[id] IN ( SELECT pp.process_id
									FROM pathway_processes pp
									WHERE pp.pathway_id = @id )
				ORDER BY pro.[name];",
				"@id", SqlDbType.UniqueIdentifier, this.ID );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
               
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );
		}

        /// <summary>
        /// Return all generic processes for this pathway.
        /// </summary>
        /// <returns></returns>
        public ServerGenericProcess[] GetAllGenericProcesses()
        {
            return ServerGenericProcess.LoadFromPathway(this);
        }

		/// <summary>
		/// Get all GOTerm / Generic Process pairings in this pathway
		/// </summary>
		/// <param name="mergeProcessesWithSameGOAnnotations">true to merge set of generic processes 
		/// into one ServerGOTermProcess object if they share the same GO annotations</param>
		/// <returns></returns>
		public ServerGOTermProcess[] GetAllGOTermProcesses(bool mergeProcessesWithSameGOAnnotations)
		{
			return ServerGOTermProcess.LoadFromPathway(this, mergeProcessesWithSameGOAnnotations);
		}

		/// <summary>
		/// Get all GOTerm / Generic Process pairings in this pathway
		/// </summary>
		/// <param name="mergeProcessesWithSameGOAnnotations">true to merge set of generic processes 
		/// into one ServerGOTermProcess object if they share the same GO annotations</param>
		/// <param name="hierarchyLevel">The level of the GO hierarchy from which annotations will come</param>
		/// <returns></returns>
		public ServerGOTermProcess[] GetAllGOTermProcesses(int hierarchyLevel, bool mergeProcessesWithSameGOAnnotations)
		{
			return ServerGOTermProcess.LoadFromPathway(this, hierarchyLevel, mergeProcessesWithSameGOAnnotations);
		}

		/// <summary>
		/// Add the given process to this pathway
		/// </summary>
		/// <param name="process_id"></param>
		/// <param name="notes"></param>
		public void AddProcess ( Guid process_id, string notes )
		{
			ServerPathway.AddProcessToPathway(this.ID, process_id, notes);
		}

		/// <summary>
		/// Remove the given process from this pathway
		/// </summary>
		/// <param name="process_id"></param>
		public void RemoveProcess ( Guid process_id )
		{
			ServerPathway.RemoveProcessFromPathway(this.ID, process_id);
		}

		#endregion

		#region Pathway Links relation
		/// <summary>
		/// Returns an array of the entities that link this pathway to other pathways
		/// </summary>
		/// <returns></returns>
		public ServerMolecularEntity[] GetLinkingEntities ( )
		{
			return ServerMolecularEntity.GetLinkingEntitiesForPathway( this.ID );
		}

        IGraphMolecularEntity[] IGraphPathway.GetLinkingEntities()
        {
            return (IGraphMolecularEntity[])GetLinkingEntities();
        }

		/// <summary>
		/// Returns an array of the pathways linked to this pathway
		/// </summary>
		/// <returns></returns>
		public ServerPathway[] GetLinkedPathways ( )
		{
			return ServerPathway.GetPathwaysLinkedToPathway( this.ID );
		}

		/// <summary>
		/// Returns an array of ConnectedPathwayAndCommonProcesses objects
		/// </summary>
		/// <returns></returns>
		public ConnectedPathwayAndCommonProcesses[] GetConnectedPathways ( )
		{
			return ServerPathway.GetAllConnectedPathwaysForPathway ( this.ID );
		}

		/// <summary>
		/// Load the pathway group to which a pathway belongs
		/// </summary>
		/// <returns>The pathway group</returns>
		public ServerPathwayGroup GetPathwayGroup()
		{
			return GetPathwayGroup(this.ID);
		}

		#endregion

        /// <summary>
		/// Get the external database links for this pathway, ordered by the db id
		/// </summary>
		/// <returns>the external database links for this pathway</returns>
		public ServerExternalDbLink[] GetExternalDatabaseLinks()
		{
			return ServerExternalDbLink.GetExternalDatabaseLinksForPathway(this);
		}

        /// <summary>
        /// Pathways that a contains more than one model, for comparison.
        /// </summary>
        /// <returns>Array of pathways that contain more than one model.</returns>
        public static ServerPathway[] GetPathwaysThatHaveMoreThanOneModel()
        {
            string query = @"SELECT *
                             FROM " + __TableName + " WHERE id IN (" +
                                     @"SELECT pathwayId  FROM dbo.MapModelsPathways  GROUP BY pathwayId HAVING COUNT(*)> 1)";

            SqlCommand command;
            command = DBWrapper.BuildCommand(query);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }
            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }


		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
            // (BE) rewrote using BuildCommand()

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, name, pathway_type_id, status, notes) VALUES (@id, @name, @pathway_type_id, @pathway_status, @pathway_notes);",
                    "@id", SqlDbType.UniqueIdentifier, ID,
                    "@name", SqlDbType.VarChar, Name,
                    "@pathway_type_id", SqlDbType.TinyInt, TypeId,
					"@pathway_status", SqlDbType.VarChar, PathwayStatus, 
					"@pathway_notes", SqlDbType.VarChar, PathwayNotes
				);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                    "@id", SqlDbType.UniqueIdentifier, ID);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET name = @name, pathway_type_id = @pathway_type_id WHERE id = @id;",
                    "@name", SqlDbType.VarChar, Name,
                    "@pathway_type_id", SqlDbType.TinyInt, Type,
                    "@id", SqlDbType.UniqueIdentifier, ID, 
					"@pathway_status", SqlDbType.VarChar, PathwayStatus, 
					"@pathway_notes", SqlDbType.VarChar, PathwayNotes
				);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                    "@id", SqlDbType.UniqueIdentifier, ID);
		}
		#endregion
        #endregion


        #region Static Methods

		#region GO Term Enrichment Methods

		/// <summary>
		/// Get the number of times a GO term annotates processes
		/// within all pathways
		/// </summary>
		/// <param name="GOID">The 7-digit GO id with leading zeros</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <returns>The number of times the GO term annotates</returns>
		public static int GetGOAnnotationCount(string GOID, int level)
		{
			object o = DBWrapper.Instance.ExecuteScalar(
				"select number_annotations from " + __TablePathwayAnnotationCounts + " where go_id=@goid and hierarchy_level=@level",
				"@goid", SqlDbType.VarChar, GOID,
				"@level", SqlDbType.Int, level);
			if(o == DBNull.Value || o == null) return 0;
			if(!(o is int)) throw new InvalidCastException("Could not cast number_annotatoins while in ServerPathway.GetAnnotationCount(string, int)");
			return (int)o;
		}

		/// <summary>
		/// Get the total number of annotations from all GO terms
		/// within all pathways at the given level
		/// </summary>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <returns>The total number of annotations</returns>
		public static int GetGOAnnotationCount(int level)
		{
			object o = DBWrapper.Instance.ExecuteScalar(
				"select sum(number_annotations) from " + __TablePathwayAnnotationCounts + " where hierarchy_level=@level",
				"@level", SqlDbType.Int, level);
			if(o == DBNull.Value) return 0;
			if(!(o is int)) throw new InvalidCastException("Could not cast number_annotations while in ServerPathway.GetGOAnnotationCount(int)");
			return (int)o;
		}

		/// <summary>
		/// Get the significance of an annotation count within all pathways
		/// using hypergeometric distribution
		/// </summary>
		/// <param name="GOID">The GO id</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
		/// <returns>The P-value of the annotation significance</returns>
		public static double GetGOEnrichmentSignificance(string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses)
		{
			//get the total count for this group
			int totalCount = GetGOAnnotationCount(level);

			//get the term annotation count for this group
			int totalSuccesses = GetGOAnnotationCount(GOID, level);

			//compute the hypergeometric distribution
			double ret = PathwaysLib.Utilities.HypergeometricDistribution.Evaluate(sampleAnnotationSize, sampleAnnotationSuccesses, totalCount, totalSuccesses);
			return ret;
		}

		/// <summary>
		/// Get the significance of an annotation count within all pathways
		/// by computing the enrichment ratio, which compares the expected
		/// annotation rate (the rate of annotation in all pathways)
		/// with the observed annotation rate
		/// </summary>
		/// <param name="GOID">The GO term id</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
		/// <returns>observed annotation rate / expected annotation rate</returns>
		public double GetGOEnrichmentRatio(string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses)
		{
			//get the total annotation count for this group
			double totalCount = (double)GetGOAnnotationCount(level);

			//get the term annotation count for this group
			double totalSuccesses = (double)GetGOAnnotationCount(GOID, level);

			double expectedRate = ((double)sampleAnnotationSize/totalCount) * totalSuccesses;

			return ((double)sampleAnnotationSuccesses)/expectedRate;
		}

		/// <summary>
		/// Returns the expected annotation rate for a given GO term at a level of the GO hierarchy
		/// </summary>
		/// <param name="GOID">The ID of the GO term</param>
		/// <param name="level">The level of the go hierarchy</param>
		/// <returns></returns>
		public static double GetGOExpectedRatio(string GOID, int level)
		{
			return (double)GetGOAnnotationCount(GOID, level)/(double)GetGOAnnotationCount(level);
		}

		/// <summary>
		/// Get the significance of an annotation count within all pathways
		/// by computing the enrichment ratio and enrichment significance
		/// </summary>
		/// <param name="GOID">The GO term id</param>
		/// <param name="level">The level of the GO hierarchy</param>
		/// <param name="sampleAnnotationSize">The total number of annotations (by any GO term) within the sample</param>
		/// <param name="sampleAnnotationSuccesses">The total number of times the given GO term annotates within the sample</param>
		/// <param name="enrichmentRatio">observed annotation rate / expected annotation rate</param>
		/// <param name="enrichmentSignificance">The significance of the annotation, based upon the hypergeometric distribution</param>
		/// <param name="expectedSuccesses">The number of successes expected</param>
		public static void GetGOEnrichment(string GOID, int level, int sampleAnnotationSize, int sampleAnnotationSuccesses, out double enrichmentRatio, out double enrichmentSignificance, out double expectedSuccesses)
		{
			//get the total count for this group
			int totalCount = GetGOAnnotationCount(level);

			//get the term annotation count for this group
			int totalSuccesses = GetGOAnnotationCount(GOID, level);

			if(totalSuccesses< sampleAnnotationSuccesses)
			{
				//maybe we're queried the wrong level
				ServerGOTerm term = ServerGOTerm.Load(GOID);
				if(term.GetMaximumTermLevel() < level)
				{
					totalSuccesses = GetGOAnnotationCount(GOID, term.GetMaximumTermLevel());
					if(totalSuccesses < sampleAnnotationSuccesses)
						throw new ApplicationException(string.Format("Number of sample successes ({2}) for term {0} at level {1} was greater than {3}", GOID, level, sampleAnnotationSuccesses, totalSuccesses));
				}
				else
				{
					throw new ApplicationException(string.Format("Number of sample successes ({2}) for term {0} at level {1} was greater than {3}", GOID, level, sampleAnnotationSuccesses, totalSuccesses));
				}
			}

			//compute the hypergeometric distribution
			enrichmentSignificance = PathwaysLib.Utilities.HypergeometricDistribution.Evaluate(sampleAnnotationSize, sampleAnnotationSuccesses, totalCount, totalSuccesses);
			
			
			expectedSuccesses = ((double)sampleAnnotationSize/totalCount) * totalSuccesses;

			enrichmentRatio = ((double)sampleAnnotationSuccesses)/expectedSuccesses;
		}

		#endregion

		/// <summary>
		/// Return all pathways from the system.
		/// </summary>
		/// <returns>
		/// Array of ServerPathways
		/// </returns>
		public static ServerPathway[] AllPathways ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + " ORDER BY [name], [id];" );
			
			DataSet[] ds;
			int r = DBWrapper.LoadMultiple( out ds, ref command );
            if (r < 1)
                return new ServerPathway[0];

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}


        public static ServerPathway[] GetAllPathwaysHasModels()
        {
             string sqlString = @" SELECT pa.* From Catalyzes c, Pathway_Processes pp, Pathways pa" +
                              " WHERE c.process_id = pp.process_id AND " +
                              " pp.pathway_id = pa.id AND " +
                              " pp.process_id IN " +
                              " ( SELECT DISTINCT pe.processId  " +
                                " FROM MapReactionsProcessEntities pe ) " +
                              " ORDER BY pa.name, pa.id";

            SqlCommand command = DBWrapper.BuildCommand(sqlString);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }


        public static ServerPathway[] GetPathwaysHasModels(Guid organismId)
        {
            string sqlString = @" SELECT pa.* From Catalyzes c, Pathway_Processes pp, Pathways pa" +
                               " WHERE c.organism_group_id = @organismId AND " + 
                               " c.process_id = pp.process_id AND " +
                               " pp.pathway_id = pa.id AND " +
                               " pp.process_id IN " +
                               " ( SELECT DISTINCT pe.processId  " +
                                 " FROM MapReactionsProcessEntities pe ) " +
                               " ORDER BY pa.name, pa.id";

            SqlCommand command = DBWrapper.BuildCommand(sqlString, "@organismId ", SqlDbType.UniqueIdentifier, organismId);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }

        /// <summary>
        /// Finds all pathways assocated w/ this organism group (and everything under it)
        /// </summary>
        /// 

        public static ServerPathway[] GetPathwaysOfOrganism(Guid organismId)
        {
            string queryString = @" SELECT p.*
					                FROM pathways p, pathway_processes pp, catalyzes c
					                WHERE p.id = pp.pathway_id
						            AND pp.process_id = c.process_id
						            AND c.organism_group_id = @organismId 
                                    ORDER BY p.name;";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@organismId ", SqlDbType.UniqueIdentifier, organismId);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }


        /// <summary>
        /// Return names of the pathways in the comma-separated list.
        /// </summary>
        /// <param name="cspathwayIds">
        /// comma-separated list of pathways.
        /// </param>
        /// <returns>
        /// ArrayList with its element as pathwayId|pathwayname
        /// </returns>
        public static ArrayList getNames(string cspathwayIds)
        {
            ArrayList pathway = new ArrayList();
            char[] separator = {','};
            string[] pathwayIds = cspathwayIds.Split(separator);
            string ids = "";
            foreach (string pathwayId in pathwayIds)
            {
                ids += "'" + pathwayId + "',";
            }
            ids = ids.Substring(0, ids.Length - 1);
            SqlCommand command = new SqlCommand("select id, name from " + __TableName + " where id IN ( " + ids + " ) ORDER BY name;");
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);
            int i = 0;
            
            foreach (DataSet d in ds)
            {
                DBRow row = new DBRow(d);
                pathway.Add(row["name"].ToString() + "|" + row["id"].ToString());
            }
           
            return pathway;
        }

        /// <summary>
        /// Returns the GUID IDs of all pathways in the database.
        /// </summary>
        /// <returns></returns>
        public static Guid[] AllPathwayIds()
        {
            ArrayList guids = new ArrayList();
            SqlCommand command = new SqlCommand("SELECT DISTINCT id FROM pathways p");
            DataSet ds;

            if (DBWrapper.Instance.ExecuteQuery(out ds, ref command) > 0)
            {
                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    guids.Add((Guid)r["id"]);
                }
            }

            return (Guid[])guids.ToArray(typeof(Guid));
        }

		/// <summary>
		/// Return all pathways, paginated.
		/// </summary>
		/// <param name="startRecord">
		/// Record to start the page from.
		/// </param>
		/// <param name="maxRecords">
		/// The number of records for the page.
		/// </param>
		/// <returns>
		/// Array of ServerPathways.
		/// </returns>
		public static ServerPathway[] AllPathways ( int startRecord, int maxRecords )
		{
			int bigNum = startRecord + maxRecords;

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											ORDER BY [name] ) " + __TableName + @"
									ORDER BY [name] DESC ) " + __TableName + @"
				ORDER BY [name]");
			
			DataSet[] ds;
			int r = DBWrapper.LoadMultiple( out ds, ref command );
			if (r < 1)
				return new ServerPathway[0];

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}

		/// <summary>
		/// Returns a count of all the pathways in the database
		/// </summary>
		/// <returns></returns>
		public static int CountAllPathways ( )
		{
			SqlCommand command = new SqlCommand( "SELECT COUNT(*) FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			return ( int ) DBWrapper.Instance.ExecuteScalar( ref command );
		}

		/// <summary>
		/// Enumeration of methods available for searching pathways
		/// </summary>
		public enum PathwaySearchMethod
		{
			/// <summary>
			/// The pathway name contains the given string
			/// </summary>
			Contains,
			/// <summary>
			/// The pathway name ends with the given string
			/// </summary>
			EndsWith,
			/// <summary>
			/// The pathway name is exactly the same as the given string
			/// </summary>
			ExactMatch,
			/// <summary>
			/// The pathway name starts with the given string
			/// </summary>
			StartsWith
		}

		/// <summary>
		/// Returns all pathways who's name contains the given substring.
		/// 
		/// SearchMethod must be one of the following strings (exact case/spacing):
		/// "Contains", "Ends with", "Exact match", or "Starts with".
		/// </summary>
		/// <param name="substring"></param>
		/// <param name="searchMethod"></param>
		/// <returns></returns>
		public static ServerPathway[] FindPathways(string substring, SearchMethod searchMethod)
		{
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE [name] " +
				( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + " @substring ORDER BY [name]",
				"@substring", SqlDbType.VarChar, substring );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}

		/// <summary>
		/// A search function for paging
		/// </summary>
		/// <param name="substring"></param>
		/// <param name="searchMethod"></param>
		/// <param name="startRecord"></param>
		/// <param name="maxRecords"></param>
		/// <returns></returns>
		public static ServerPathway[] FindPathways(string substring, SearchMethod searchMethod, int startRecord, int maxRecords )
		{
			int bigNum = startRecord + maxRecords;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											WHERE [name] " + ( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + @" @substring
											ORDER BY [name] ) " + __TableName + @"
									ORDER BY [name] DESC ) " + __TableName + @"
				ORDER BY [name]",
				"@substring", SqlDbType.VarChar, substring );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );			

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}
        /// <summary>
        /// A search function for paging
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public static ServerPathway[] FindPathwaysWithModel(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											WHERE [name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
											AND [id] IN
                                                (SELECT pathwayId FROM MapModelsPathways) 
                                    ORDER BY [name] ) " + __TableName + @"
									ORDER BY [name] DESC ) " + __TableName + @"
				ORDER BY [name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }

		/// <summary>
		/// Count of the number of pathways that would be found with the supplied search parameters.
		/// </summary>
		/// <param name="substring">
		/// The substring we're searching for.
		/// </param>
		/// <param name="searchMethod">
		/// The search method.
		/// </param>
		/// <returns>
		/// Count of found pathways.
		/// </returns>
        public static int CountFindPathwaysWithModel(string substring, SearchMethod searchMethod)
		{
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT COUNT(*) FROM " + __TableName + " WHERE [name] " +
					( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + @" @substring " +
                    @" AND [id] IN
                          (SELECT pathwayId FROM MapModelsPathways);",
				"@substring", SqlDbType.VarChar, substring );
			
			DataSet[] ds = new DataSet[0];
			return ( int ) DBWrapper.Instance.ExecuteScalar( ref command );
		}

        /// <summary>
        /// Count of the number of pathways that would be found with the supplied search parameters.
        /// </summary>
        /// <param name="substring">
        /// The substring we're searching for.
        /// </param>
        /// <param name="searchMethod">
        /// The search method.
        /// </param>
        /// <returns>
        /// Count of found pathways.
        /// </returns>
        public static int CountFindPathways(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " WHERE [name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring;",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

		/// <summary>
		/// Return a pathway with given ID.
		/// </summary>
		/// <param name="id">
		/// The Guid of the desired pathway.
		/// </param>
		/// <returns>
		/// SoapPathway object ready to be sent via SOAP.
		/// </returns>
		public static ServerPathway Load ( Guid id )
		{
			return new ServerPathway( LoadRow ( id ) );
		}


		/// <summary>
		/// Creates an array of ServerPathway objects from an SQL query for multiple pathways rows.
		/// </summary>
		/// <param name="command">A query for multiple rows of pathways.  Note that the query must output
		/// all columns(not just the id's)!</param>
		/// <returns></returns>
		internal static ServerPathway[] LoadMultiple(SqlCommand command)
		{
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet r in ds)
                results.Add(new ServerPathway(new DBRow(r)));

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
            //return DBWrapper.LoadMultiple<ServerPathway>(command).ToArray();
        }


		/// <summary>
		/// Creates a single ServerPathway from an SQL query
		/// </summary>
		/// <param name="command">An SQL query that returns a single row from pathways.  Note that the query
		/// must output every column</param>
		/// <returns></returns>
		internal static ServerPathway LoadSingle(SqlCommand command)
		{
            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new ServerPathway(new DBRow(ds));
            //return DBWrapper.LoadSingle<ServerPathway>(command);
		}

        /// <summary>
        /// Returns true if there exists a pathway with the given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exists ( Guid id )
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                    "@id", SqlDbType.UniqueIdentifier, id);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }

        /// <summary>
        /// Return the dataset for a pathway with a given ID.
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
        /// Return the mappings of the genes on chromosomes for a given set of comma-separated pathways
        /// </summary>
        /// <param name="pathwayidSet"></param>
        /// <param name="organismGroupId"></param>
        /// <returns></returns>
        public static string GetGeneMappingForASetOfPathways(string pathwayIdSet, Guid organismGroupId)
        {
            //get chromosome list for each organism that contains the given pathway
            string query = @"SELECT DISTINCT ch.id, ch.name, ch.length, ch.centromere_location
                             FROM chromosomes ch
                             WHERE ch.organism_group_id = @orgId  
                             and ch.length > 0";


            SqlCommand command;
            command = DBWrapper.BuildCommand(query, "@orgId", SqlDbType.UniqueIdentifier, organismGroupId);
            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            Dictionary<string, SoapChromosome> chromosomes = new Dictionary<string, SoapChromosome>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SoapChromosome ch = new SoapChromosome(dr["id"].ToString(), dr["name"].ToString(), dr["length"].ToString(), dr["centromere_location"]);
                chromosomes.Add(dr["id"].ToString(), ch);
            }

            Dictionary<string, List<SoapGene>> chromosomeGenesDict = null;
            if (chromosomes.Count > 0)
            {
                // at least one organism was found in this pathway ... get the chromosomes
                char[] delim = { ',' };
                string[] ids = pathwayIdSet.Split(delim);
                string queryInPredicate = "";
                foreach (string id in ids)
                    queryInPredicate += "\'" + id.Trim() + "\',";

                queryInPredicate = queryInPredicate.Substring(0, queryInPredicate.Length - 1);
                queryInPredicate = " (" + queryInPredicate + ") ";

                query = @"select DISTINCT g.id, g.chromosome_id, me.name, g.relative_address, g.cytogenic_address, p.generic_process_id 
                    from pathway_processes pp, catalyzes c, gene_encodings ge, genes g, molecular_entities me, processes p
                    where pathway_id IN" + queryInPredicate + @"
                    and p.id = pp.process_id
                    and pp.process_id=c.process_id
                    and c.organism_group_id=@orgId
                    and c.gene_product_id = ge.gene_product_id
                    and g.id = ge.gene_id
                    and g.organism_group_id = @orgId
                    and me.id = g.id
                    and g.chromosome_id is not null";

                command = DBWrapper.BuildCommand(query, "@orgId", SqlDbType.UniqueIdentifier, organismGroupId);
                DBWrapper.Instance.ExecuteQuery(out ds, ref command);

                chromosomeGenesDict = new Dictionary<string, List<SoapGene>>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = (DataRow)ds.Tables[0].Rows[i];
                    string chrmId = dr["chromosome_id"].ToString();
                    if (!chromosomeGenesDict.ContainsKey(chrmId))
                    {
                        chromosomeGenesDict.Add(chrmId, new List<SoapGene>());
                    }

                    SoapGene gene = new SoapGene(dr["id"].ToString(), dr["name"].ToString(), dr["relative_address"].ToString(), dr["cytogenic_address"].ToString(), dr["generic_process_id"].ToString(), dr["chromosome_id"] is DBNull ? null : dr["chromosome_id"].ToString());
                    chromosomeGenesDict[chrmId].Add(gene);
                }
            }
            else
                return "";

            if (chromosomeGenesDict.Count == 0)
                return "";

            StringBuilder xml = new StringBuilder("");
            List<SoapGene> genes;

            xml.Append("<genome organismgroupid=\"" + organismGroupId.ToString() + "\" numberofchromosomes=\"" + chromosomes.Count + "\"" + ">");
            foreach (SoapChromosome ch in chromosomes.Values)
            {
                xml.Append("<chromosome number=\"" + ch.Name + "\" length=\"" + ch.Length + "\" centromerlocation=\"" + (ch.CentromereLocation <= 1 ? "" : ch.CentromereLocation.ToString()) + "\"" + ">");

                if (chromosomeGenesDict.TryGetValue(ch.ID.ToString(), out genes))
                {
                    for (int j = 0; genes != null && j < genes.Count; j++)
                    {
                        SoapGene gene = genes[j];
                        xml.Append("<gene id=\"" + gene.ID.ToString()
                            + "\" name=\"" + gene.Name
                            + "\" location=\"" + gene.RawAddress
                            + "\" cytogeneticAddress=\"" + gene.CytogenicAddress
                            + "\" genericprocess=\"" + gene.GeneGroupID + "\"" + "/>");
                    }
                }
                xml.Append("</chromosome>");
            }
            xml.Append("</genome>");
            return xml.ToString();
        }

        /// <summary>
        /// Return the mappings of the genes on chromosomes for a given pathway
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public static string GetGeneMapping(Guid pathwayId, Guid organismGroupId)
        {
            //get chromosome list for each organism that contains the given pathway
            string query = @"SELECT DISTINCT ch.id, ch.name, ch.length, ch.centromere_location
                             FROM chromosomes ch
                             WHERE ch.organism_group_id = @orgId and ch.id in
                             (select chromosome_id from chromosome_bands)";                             

            SqlCommand command;
            command = DBWrapper.BuildCommand(query, "@orgId", SqlDbType.UniqueIdentifier, organismGroupId);
            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            Dictionary<string, SoapChromosome> chromosomes = new Dictionary<string, SoapChromosome>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {                
                SoapChromosome ch = new SoapChromosome(dr["id"].ToString(), dr["name"].ToString(), dr["length"].ToString(), dr["centromere_location"]);
                chromosomes.Add(dr["id"].ToString(), ch);
            }

            Dictionary<string, List<SoapGene>> chromosomeGenesDict = null;
            if (chromosomes.Count > 0)
            {
                // at least one organism was found in this pathway ... get the chromosomes
                query = @"select DISTINCT g.id, g.chromosome_id, me.name, g.relative_address, g.cytogenic_address, p.generic_process_id 
                    from pathway_processes pp, catalyzes c, gene_encodings ge, genes g, molecular_entities me, processes p
                    where pathway_id=@pathwayId 
                    and p.id = pp.process_id
                    and pp.process_id=c.process_id
                    and c.organism_group_id=@orgId
                    and c.gene_product_id = ge.gene_product_id
                    and g.id = ge.gene_id
                    and g.organism_group_id = @orgId
                    and me.id = g.id
                    and g.chromosome_id is not null";

                command = DBWrapper.BuildCommand(query, "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId,
                                                           "@orgId", SqlDbType.UniqueIdentifier, organismGroupId);
                DBWrapper.Instance.ExecuteQuery(out ds, ref command);

                chromosomeGenesDict = new Dictionary<string, List<SoapGene>>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = (DataRow)ds.Tables[0].Rows[i];
                    string chrmId = dr["chromosome_id"].ToString();
                    if (!chromosomeGenesDict.ContainsKey(chrmId))
                    {
                        chromosomeGenesDict.Add(chrmId, new List<SoapGene>());
                    }

                    SoapGene gene = new SoapGene(dr["id"].ToString(), dr["name"].ToString(), dr["relative_address"].ToString(), dr["cytogenic_address"].ToString(), dr["generic_process_id"].ToString(), dr["chromosome_id"] is DBNull ? null : dr["chromosome_id"].ToString());
                    chromosomeGenesDict[chrmId].Add(gene);
                }
            }
            else
                return "";

            if (chromosomeGenesDict.Count == 0)
                return "";

            StringBuilder xml = new StringBuilder("");
            List<SoapGene> genes;            

            xml.Append("<genome organismgroupid=\"" + organismGroupId.ToString() + "\" numberofchromosomes=\"" + chromosomes.Count + "\"" + ">");
            foreach(SoapChromosome ch in chromosomes.Values)
            {                
                xml.Append("<chromosome number=\"" + ch.Name + "\" length=\"" + ch.Length + "\" centromerlocation=\"" + (ch.CentromereLocation == 1 ? "" : ch.CentromereLocation.ToString()) + "\"" + ">");                
                
                if (chromosomeGenesDict.TryGetValue(ch.ID.ToString(), out genes))
                {                    
                    for (int j = 0; genes != null && j < genes.Count; j++)
                    {
                        SoapGene gene = genes[j];
                        xml.Append("<gene id=\"" + gene.ID.ToString()
                            + "\" name=\"" + gene.Name
                            + "\" location=\"" + gene.RawAddress
                            + "\" cytogeneticAddress=\"" + gene.CytogenicAddress
                            + "\" genericprocess=\"" + gene.GeneGroupID + "\"" + "/>");
                    }
                }                
                xml.Append("</chromosome>");
            }
            xml.Append("</genome>");       
            return xml.ToString();
        }

        /// <summary>
        /// Return the mappings of the genes on chromosomes for a given pathway
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public static string GetGeneMapping(Guid pathwayId)
        {
            StringBuilder xml = new StringBuilder("");
            //get chromosome list for each organism that contains the given pathway
            string query = @"SELECT DISTINCT ch.organism_group_id, ch.id, ch.name, ch.length, ch.centromere_location
                             FROM chromosomes ch
                             WHERE ch.organism_group_id IN
                             (SELECT organism_group_id 
                              FROM catalyzes c, pathway_processes pp
                              WHERE c.process_id = pp.process_id
                              AND pp.pathway_id=@pathwayId)
                             GROUP BY ch.organism_group_id, ch.id, ch.name, ch.length, ch.centromere_location
                             ORDER BY ch.organism_group_id, ch.name";

            SqlCommand command;
            command = DBWrapper.BuildCommand(query, "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);
            DataSet ds;
            //DBWrapper.LoadMultiple(out ds, ref command); //NOTE (BE) BAD FOR LARGE RESULT SETS!
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            Dictionary<string, List<SoapChromosome>> genomes = new Dictionary<string,List<SoapChromosome>>();
            //xml.Append("1 ");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //xml.Append("2 ");
                string orgId = dr["organism_group_id"].ToString();
                if (!genomes.ContainsKey(orgId))
                {
                    genomes.Add(orgId, new List<SoapChromosome>());
                }

                SoapChromosome ch = new SoapChromosome(dr["id"].ToString(), dr["name"].ToString(), dr["length"].ToString(), dr["centromere_location"]);
                genomes[orgId].Add(ch);
            }
            
            Dictionary<string, List<SoapGene>> chTable = new Dictionary<string, List<SoapGene>>();
            if (genomes.Count > 0)
            {
                //xml.Append("3 ");
                // at least one organism was found in this pathway ... get the chromosomes

                query = @"select DISTINCT g.organism_group_id, g.chromosome_id, me.name, g.relative_address, g.cytogenic_address, p.generic_process_id 
                    from pathway_processes pp, catalyzes c, gene_encodings ge, genes g, molecular_entities me, processes p
                    where pathway_id=@pathwayId 
                    and p.id = pp.process_id
                    and pp.process_id=c.process_id
                    and c.gene_product_id = ge.gene_product_id
                    and g.id = ge.gene_id
                    and me.id = g.id
                    order by g.chromosome_id, g.organism_group_id";

                command = DBWrapper.BuildCommand(query, "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);
                //DBWrapper.LoadMultiple(out ds, ref command); //NOTE (BE) BAD FOR LARGE RESULT SETS!
                DBWrapper.Instance.ExecuteQuery(out ds, ref command);
                
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //xml.Append("4 ");
                    DataRow dr = (DataRow)ds.Tables[0].Rows[i];
                    string chrmId = dr["chromosome_id"].ToString();
                    //return xml.ToString();
                    if (!chTable.ContainsKey(chrmId))
                    {
                        chTable.Add(chrmId, new List<SoapGene>());
                    }

                    SoapGene gene = new SoapGene(dr["name"].ToString(), dr["relative_address"].ToString(), dr["cytogenic_address"].ToString(), dr["generic_process_id"].ToString(), dr["chromosome_id"] is DBNull ? null : dr["chromosome_id"].ToString());
                    chTable[chrmId].Add(gene);

                    
                }
            }

            //StringBuilder xml = new StringBuilder("");
            //return xml.ToString();
            foreach (string orgId in genomes.Keys)
            {
                //xml.Append("5 ");
                List<SoapChromosome> chromosomes = genomes[orgId];
                xml.Append("<genome organismgroupid=\"" + orgId + "\" numberofchromosomes=\"" + chromosomes.Count + "\"" + ">");
                for (int i = 0; i < chromosomes.Count; i++)
                {
                    SoapChromosome ch = chromosomes[i];
                    List<SoapGene> genes;
                    if (chTable.ContainsKey(ch.ID.ToString()))
                    {
                        genes = chTable[ch.ID.ToString()];
                    }
                    else
                    {
                        genes = new List<SoapGene>();
                    }
                    xml.Append("<chromosome number=\"" + ch.Name + "\" length=\"" + ch.Length + "\" centromerlocation=\"" + (ch.CentromereLocation == 1 ? "" : ch.CentromereLocation.ToString()) + "\"" + ">");
                    for (int j = 0; genes != null && j < genes.Count; j++)
                    {
                        SoapGene gene = genes[j];
                        xml.Append("<gene name=\"" + gene.Name
                            + "\" location=\"" + gene.RawAddress
                            + "\"  cytogeneticAddress=\"" + gene.CytogenicAddress
                            + "\" genericprocess=\"" + gene.GeneGroupID + "\"" + "/>");
                        //xml.Append("<name>" + gene.Name + "</name>");
                        //xml.Append("<location>" + gene.RawAddress + "</location>");
                        //xml.Append("<genericprocess>" + gene.GeneGroupID + "</genericprocess>");
                        //xml.Append("</gene>");
                    }
                    xml.Append("</chromosome>");
                    //break;
                }
                xml.Append("</genome>");
            }            
            return xml.ToString();
        }

        /// <summary>
        /// Returns the set of organisms that have at least one mapped gene encoding an enzyme of a given pathway
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>

        public static string GetGenomes(Guid pathwayId)
        {
            //get chromosome list for each organism that contains the given pathway
            string query = @"select distinct g.organism_group_id 
                                from pathway_processes pp, catalyzes c, gene_encodings ge, genes g
                                where pp.pathway_id = @pathwayId
                                and pp.process_id = c.process_id
                                and c.gene_product_id = ge.gene_product_id
                                and ge.gene_id = g.id
                                and c.organism_group_id = g.organism_group_id
                                and g.chromosome_id IN
                                (select chromosome_id from chromosome_bands)";

            SqlCommand command;
            command = DBWrapper.BuildCommand(query, "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);
            DataSet ds;          
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            string orgId;
            StringBuilder xml = new StringBuilder("");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                orgId = dr["organism_group_id"].ToString();
                xml.Append("<genome organismgroupid=\"" + orgId + "\"></genome>");                
            }                        
            return xml.ToString();
        }

		#region Pathway Process relation
		/// <summary>
		/// Add a process to a pathway
		/// </summary>
		/// <param name="pathway_id"></param>
		/// <param name="process_id"></param>
		/// <param name="notes"></param>
		public static void AddProcessToPathway ( Guid pathway_id, Guid process_id, string notes )
		{
			//(bse)
			// check if the process already belongs to the pathway
			//
			if ( !PathwayProcessExists(pathway_id, process_id ) )
			{
				DBWrapper.Instance.ExecuteNonQuery(				
					"INSERT INTO pathway_processes (pathway_id, process_id, notes) VALUES (@pathway_id, @process_id, @notes);",
					"@pathway_id", SqlDbType.UniqueIdentifier, pathway_id,
					"@process_id", SqlDbType.UniqueIdentifier, process_id, 
					"@notes", SqlDbType.Text, notes);
			}
			else 
			{
				//do nothing, the process is already part of the pathway
			}
		}

		/// <summary>
		/// Check if a process already belongs to a pathway
		/// </summary>
		/// <param name="pathway_id"></param>
		/// <param name="process_id"></param>
		public static bool PathwayProcessExists ( Guid pathway_id, Guid process_id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM pathway_processes WHERE pathway_id = @pathway_id AND process_id = @process_id;",
				"@pathway_id", SqlDbType.UniqueIdentifier, pathway_id,
				"@process_id", SqlDbType.UniqueIdentifier, process_id);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Remove a process from a pathway
		/// </summary>
		/// <param name="pathway_id"></param>
		/// <param name="process_id"></param>
		public static void RemoveProcessFromPathway ( Guid pathway_id, Guid process_id )
		{
			if (DBWrapper.Instance.ExecuteNonQuery(				
				"DELETE FROM pathway_processes WHERE pathway_id = @pathway_id AND process_id = @process_id;",
				"@pathway_id", SqlDbType.UniqueIdentifier, pathway_id,
				"@process_id", SqlDbType.UniqueIdentifier, process_id) < 1)
			{
				throw new DataModelException("Remove process {0} from pathway {1} failed!", pathway_id, process_id);
			}
		}

		#endregion

		#region Pathway Links relation
		/// <summary>
		/// Get a DataSet of linked pathways and the common entity between them
		/// </summary>
		/// <param name="pathwayId"></param>
		/// <returns>
		/// A data set of pathways and entities
		/// </returns>
		public static DataSet[] GetLinkedPathwaysAndEntitiesForPathway ( Guid pathwayId )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT pathway_id_2, entity_id
					FROM pathway_links, pathways
					WHERE pathway_id_1 = @pathway_id_1 AND pathways.id = pathway_id_2
					ORDER BY name",
				"@pathway_id_1", SqlDbType.UniqueIdentifier, pathwayId );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );
			return ds;
		}

		/// <summary>
		/// Returns an array of the pathways linked to the given pathway
		/// </summary>
		/// <param name="pathwayId"></param>
		/// <returns></returns>
		public static ServerPathway[] GetPathwaysLinkedToPathway ( Guid pathwayId )
		{
			SqlCommand command = DBWrapper.BuildCommand( 
				@"SELECT p.*
					FROM pathways p
					WHERE p.[id] IN ( SELECT pl.[pathway_id_2]
										FROM pathway_links pl
										WHERE pl.pathway_id_1 = @pathway_id_1 );",
				"@pathway_id_1", SqlDbType.UniqueIdentifier, pathwayId );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );
			
			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}

		public static ServerPathway[] GetPathwaysLinkedToPathwayIncoming ( Guid pathwayId )
		{
			SqlCommand command = DBWrapper.BuildCommand( 
				@"SELECT p.*
					FROM pathways p
					WHERE p.[id] IN ( SELECT pl.[pathway_id_1]
										FROM pathway_links pl
										WHERE pl.pathway_id_2 = @pathway_id );",
				"@pathway_id", SqlDbType.UniqueIdentifier, pathwayId );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );
			
			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}
		/// <summary>
		/// Returns an array of processes that two pathways have in common
		/// Created for use by ConnectedPathwayAndCommonProcesses.
		/// </summary>
		/// <param name="pathwayId1"></param>
		/// <param name="pathwayId2"></param>
		/// <returns></returns>
		public static ServerProcess[] GetProcessesInCommonForPathways ( Guid pathwayId1, Guid pathwayId2 )
		{
			SqlCommand command = DBWrapper.BuildCommand( 
				@"SELECT pro.*
					FROM processes pro
					WHERE pro.[id] IN ( SELECT pp.process_id
											FROM pathway_processes pp
											INNER JOIN pathway_processes patpro ON pp.process_id = patpro.process_id
											WHERE pp.pathway_id = @pathwayId1 AND patpro.pathway_id = @pathwayId2 );",
				"@pathwayId1", SqlDbType.UniqueIdentifier, pathwayId1,
				"@pathwayId2", SqlDbType.UniqueIdentifier, pathwayId2);
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );
			
			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );
		}

		/// <summary>
		/// Load the pathway group to which a pathway belongs
		/// </summary>
		/// <param name="pathwayID">The id of the pathway</param>
		/// <returns>The pathway group</returns>
		public static ServerPathwayGroup GetPathwayGroup(Guid pathwayID)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"select t2.* from
					pathway_to_pathway_groups t1
					inner join
					pathway_groups t2
					on t1.group_id = t2.group_id
					where pathway_id = @pathway_id",
				"@pathway_id", SqlDbType.UniqueIdentifier, pathwayID);

			DataSet ds;
			DBWrapper.LoadSingle(out ds, ref command);

			return new ServerPathwayGroup(new DBRow(ds));
		}

		// Had to move this to ServerMolecularEntity
//		/// <summary>
//		/// Returns an array of molecular entities that two pathways share, but are not
//		/// in their shared processes
//		/// </summary>
//		/// <param name="pathwayId1"></param>
//		/// <param name="pathwayId2"></param>
//		/// <returns></returns>
//		public static ServerMolecularEntity[] GetExclusiveEntitiesForPathways ( Guid pathwayId1, Guid pathwayId2 )
//		{
//			SqlCommand command = DBWrapper.BuildCommand( 
//				"SELECT me.* FROM molecular_entities me  WHERE me.[id] IN ( SELECT pe.entity_id FROM process_entities pe WHERE pe.process_id IN (  SELECT pp.process_id  FROM pathway_processes pp INNER JOIN pathway_processes patpro ON pp.process_id <> patpro.process_id  WHERE pp.pathway_id = @pathwayId1 AND patpro.pathway_id = @pathwayId2 ) );",
//				"@pathwayId1", SqlDbType.UniqueIdentifier, pathwayId1,
//				"@pathwayId2", SqlDbType.UniqueIdentifier, pathwayId2);
//			
//			DataSet[] ds = new DataSet[0];
//			DBWrapper.LoadMultiple( out ds, ref command );
//			
//			ArrayList results = new ArrayList();
//			foreach ( DataSet d in ds )
//			{
//				results.Add( ServerMolecularEntity.LoadDerived( new DBRow( d ) ) );
//			}
//
//			return ( ServerMolecularEntity[] ) results.ToArray( typeof( ServerMolecularEntity ) );
//
//		}

		
		/// <summary>
		/// Returns an array of ConnectedPathway objects
		/// </summary>
		/// <param name="pathwayId"></param>
		/// <returns></returns>
		public static ConnectedPathwayAndCommonProcesses[] GetAllConnectedPathwaysForPathway( Guid pathwayId )
		{
			SqlCommand command = DBWrapper.BuildCommand( 
				@"SELECT DISTINCT pathway_id_1 AS originalPathwayId, pathway_id_2 AS connectedPathwayId
				FROM pathway_links
				WHERE pathway_id_1 = @pathway_id_1;",
				"@pathway_id_1", SqlDbType.UniqueIdentifier, pathwayId );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );
			
			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ConnectedPathwayAndCommonProcesses( new DBRow( d ) ) );
			}

			return ( ConnectedPathwayAndCommonProcesses[] ) results.ToArray( typeof( ConnectedPathwayAndCommonProcesses ) );
		}

        /// <summary>
        /// For a given pathway, returns a dictionary that maps the IDs of the 
        /// linked pathways to the list of molecules that are shared.
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, List<Guid>> GetConnectedPathwaysWithEntities(Guid pathwayId)
        {
            return DBWrapper.Instance.ExecuteQueryDictionaryList<Guid, Guid>(
                "SELECT pathway_id_2, entity_id FROM pathway_links WHERE pathway_id_1 = @pwId",
                "@pwId", SqlDbType.UniqueIdentifier, pathwayId);
        }

        public static Dictionary<Guid, List<object[]>> GetConnectedPathwaysWithTissueAwareEntities(Guid pathwayId)
        {
            return DBWrapper.Instance.ExecuteQueryDictionaryObjectList<Guid, List<object[]>>(
               "SELECT pathway_id_2, entity_id, entity_tissue_id FROM pathway_links WHERE pathway_id_1 = @pwId",
               "@pwId", SqlDbType.UniqueIdentifier, pathwayId);
        }


        public static Dictionary<Guid, List<Guid>> GetConnectedPathwaysWithEntitiesIncoming(Guid pathwayId)
        {
            return DBWrapper.Instance.ExecuteQueryDictionaryList<Guid, Guid>(
                "SELECT pathway_id_1, entity_id FROM pathway_links WHERE pathway_id_2 = @pwId",
                "@pwId", SqlDbType.UniqueIdentifier, pathwayId);
        }
        public static Dictionary<Guid, List<object[]>> GetConnectedPathwaysWithTissueAwareEntitiesIncoming(Guid pathwayId)
        {
            return DBWrapper.Instance.ExecuteQueryDictionaryObjectList<Guid, List<object[]>>(
                "SELECT pathway_id_1, entity_id,entity_tissue_id FROM pathway_links WHERE pathway_id_2 = @pwId",
                "@pwId", SqlDbType.UniqueIdentifier, pathwayId);
        }
		#endregion

		#region PathwaysWeb queries help
		/// <summary>
		/// this is specifically for getting the name and ID of all pathways as a dataset
		///  (written for the PathwaysWithinStepsFromPathway user control)
		/// </summary>
		/// <returns></returns>
		public static DataSet GetAllPathwaysForQueryInterface ( )
		{
			SqlCommand command = new SqlCommand( "SELECT [id], [name] FROM " + __TableName + " ORDER BY [name];" );
			
			DataSet ds;
			DBWrapper d = new DBWrapper();
			int r = d.ExecuteQuery( out ds, ref command );
			
			return ds;
		}

		#endregion

        #endregion

        #region Layout Related Methods
        /// <summary>
        /// Check if the current pathway has freezed layout in the database.
        /// </summary>
        public bool HavingFreezedLayout()
        {
            return HavingFreezedLayout(this.ID);
        }

        /// <summary>
        /// Check if the current pathway has freezed layout in the database.
        /// </summary>
        public static bool HavingFreezedLayout(Guid pathwayid)
        {
            string layout = GetPathwayLayout(pathwayid);
            if (layout != null && layout != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the freezed layout for a single pathway.
        /// </summary>
        public static string GetPathwayLayout(Guid pathwayid)
        {
            try
            {
                DBWrapper.Instance = new DBWrapper();
                //construct select sql
                String select = "select layout from pathways where id='";
                select += (pathwayid.ToString() + "';");
                SqlCommand cmd = new SqlCommand(select);

                return DBWrapper.Instance.ExecuteScalar(ref cmd).ToString();
            }
            catch(Exception e)
            {
                Console.WriteLine("when retrieving layout field: " + e);
                return null;            
            }
            finally
            {
                DBWrapper.Instance.Close();
            }
        }

        /// <summary>
        /// Get all pathways which have frozen layout
        /// </summary>
        /// <returns>
        /// Set of all pathways which have frozen layout.
        /// </returns>
        public static ServerPathway[] GetPathwaysWithFrozenLayout()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT p.*
					FROM pathways p					
					WHERE layout is not null and datalength(layout)>0
                    ORDER BY p.name"                   
            );

            DataSet[] ds = new DataSet[0];
            int n = DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return ((ServerPathway[])results.ToArray(typeof(ServerPathway)));
        }

        /// <summary>
        /// Store the layout string of the pathway into the database.
        /// </summary>
        public static void StorePathwayLayout()
        {
            //to be done
        }

        #endregion

    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerPathway.cs,v 1.12 2010/11/19 21:13:29 murat Exp $
	$Log: ServerPathway.cs,v $
	Revision 1.12  2010/11/19 21:13:29  murat
	1-) Insert sql query was not structure in ServerSBase and ServerModel files those
	queries are fixed.
	
	2-) Some of the queries from ServerGoTerms, ServerOrganismGroup, ServerPathway and ServerProsess
	were wrong, they are fixed (m.sbmlId changed to sbmlId)
	
	3-) SMBLParser now can work automatically even the model names (or any other model attributes) have changed.
	
	Revision 1.11  2009/10/31 02:33:57  sarp
	New references are added(Jarnac, SBW and ZedGraph).
	Example config file now includes the latest version (3.5) of System.Web.Extensions.
	New simulation library methods are added.
	
	Revision 1.10  2009/09/09 15:48:14  xjqi
	*** empty log message ***
	
	Revision 1.9  2009/07/14 20:30:08  ann
	*** empty log message ***
	
	Revision 1.8  2009/07/14 15:33:22  ann
	*** empty log message ***
	
	Revision 1.7  2009/05/27 14:04:30  ann
	*** empty log message ***
	
	Revision 1.6  2009/05/14 14:28:17  ann
	*** empty log message ***
	
	Revision 1.5  2009/04/07 14:44:12  ali
	*** empty log message ***
	
	Revision 1.4  2009/04/03 21:16:03  xjqi
	Visualization Part: Given a model id, visualize compartments in hierarchy with species,reactions.
	Not integrating with web interface.
	
	Revision 1.3  2009/04/01 15:41:55  ali
	*** empty log message ***
	
	Revision 1.2  2009/03/27 03:18:45  rishi
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.28  2008/05/09 18:33:25  divya
	*** empty log message ***
	
	Revision 1.27  2008/05/01 17:53:29  divya
	*** empty log message ***
	
	Revision 1.26  2008/05/01 02:20:24  divya
	*** empty log message ***
	
	Revision 1.25  2008/04/29 21:45:02  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.24  2008/04/29 21:00:43  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.23  2008/04/29 20:57:13  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.22  2008/04/25 15:28:32  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.21  2008/04/25 14:06:45  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.20  2008/03/07 19:53:13  brendan
	AQI refactoring
	
	Revision 1.19  2008/02/20 18:51:55  brendan
	*** empty log message ***
	
	Revision 1.18  2008/02/12 20:49:13  yuan
	adding GetPathwayLayout, HavingFreezedLayout
	
	Revision 1.17  2007/12/30 22:22:06  divya
	*** empty log message ***
	
	Revision 1.16  2007/08/24 19:44:38  ann
	Changes made in queries based on the new dataset field
	
	Revision 1.14  2007/06/15 16:31:13  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.13  2007/06/15 08:09:21  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.12  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.11  2006/12/07 04:47:25  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.10  2006/12/07 02:21:08  ali
	*** empty log message ***
	
	Revision 1.9  2006/12/01 04:48:29  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.8  2006/11/26 21:49:25  ali
	*** empty log message ***
	
	Revision 1.7  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.6  2006/10/19 01:24:42  ali
	*** empty log message ***
	
	Revision 1.5  2006/10/13 19:26:32  ali
	*** empty log message ***
	
	Revision 1.4  2006/10/13 19:21:27  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.3  2006/09/06 14:41:40  pathwaysdeploy
	A new web method for gene viewer has been added.
	
	Revision 1.2  2006/08/17 15:04:43  ali
	A new web method "GetGeneMappingForPathway" is added for the gene viewer.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.56  2006/07/07 19:28:04  greg
	The bulk of this update focuses on integrating Ajax browsing into the content browser bar on the left.  It currently only works from the pathways dropdown option, but the framework is now in place for the other lists to function in the same manner.
	
	Revision 1.55  2006/06/30 20:02:38  greg
	There have been some very big changes here lately...
	
	 - Query logger
	The DBWrapper class now has support for logging queries in a format that you can import into Excel, etc. for analysis.  There are some Web.config lines you'll have to add, though.
	
	 - JavaScript redirects
	The dropdown list on the main browser bar uses JavaScript for redirects.  Yay.
	
	 - Visual issues
	There were several unresolved visual issues (mostly stemming from the way IE and Firefox render pages differently), but most of them should now be resolved.
	
	 - Ajax browsing
	The biggest part of this update involves Ajax.  All pages load significantly faster now, and data requests are made asynchronously.  The fine details about which panels will start open by default and everything can be worked out later... but for now it appears that everything is working nicely.
	
	Revision 1.54  2006/06/22 19:17:31  brandon
	added External Database links list to the DisplayPathwayDetail page
	
	Revision 1.53  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.52  2006/05/17 21:02:17  brendan
	Fixed query in server org that was breaking collapsed pathway visualization
	
	Revision 1.51  2006/05/17 16:26:04  greg
	 - Search pagination errors
	Trying to access pages that are out-of-bounds results in SQL exceptions.  Additionally, the pagination function seemed to generate more pages than it should have at times, specifically when viewing the last page.  These are both resolved, which also seems to fix the "xx" bug.
	
	 - Potential SQL injection issue
	Search terms were not checked for SQL injection attacks.  Many SQL queries in the different server classes were rewritten to use parameters and thus eliminate the potential issue of SQL injection.
	
	 - Interface enhancement
	The search-type dropdown box now reverts to the last-selected option when the page reloads; prior to that it always went to the top.
	
	 - User input validation
	Most (if not all) user input was not being validated on entry, meaning it was possible to perform cross-site scripting and all that kind of nasty stuff.  Input is now stripped of HTML tags.  Note that validateRequest is now turned off, so all user input has to be validated in this way.
	
	Revision 1.50  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	
	Revision 1.49  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.48  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.47  2006/04/12 20:24:20  brian
	*** empty log message ***
	
	Revision 1.46.8.3  2006/03/29 22:56:51  brian
	*** empty log message ***
	
	Revision 1.46.8.2  2006/03/22 21:25:21  brian
	Removed and renamed a few more functions for consistency
	
	Revision 1.46.8.1  2006/03/22 19:48:06  brian
	*** empty log message ***
	
	Revision 1.46.6.4  2006/05/11 15:53:27  marc
	HUGE COMMIT!!!!
	
	Revision 1.46.6.3  2006/03/08 20:11:04  marc
	Added GetAllGOTermProcesses and GetPathwayGroup methods
	
	Revision 1.46.6.2  2006/03/06 21:22:20  marc
	added GetHashcode method
	
	Revision 1.46.6.1  2006/03/02 04:32:14  marc
	Enumerated PathwaySearchMethods
	
	Revision 1.46  2006/02/07 23:22:26  brendan
	Added drawing support for generic co-factors.
	
	Added graph caching support.  Will require a new value in the .config file.
	
	Revision 1.45  2005/12/06 23:37:22  brendan
	Modified doc comment for FindPathways to include input parameter values.
	
	Revision 1.44  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.43  2005/11/02 20:35:16  fatih
	Test functions are working for pathwayslib objects
	
	Revision 1.42  2005/10/31 20:27:45  fatih
	*** empty log message ***
	
	Revision 1.41  2005/10/28 10:48:29  fatih
	Pathway, process, organism group tested and corrected
	
	Revision 1.40  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.39  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.38  2005/08/19 21:33:42  brandon
	cleaned up some files, added some comments
	
	Revision 1.37  2005/08/04 01:29:59  michael
	Debugging search and pagination
	
	Revision 1.36  2005/08/03 05:31:17  michael
	Working on searh and results/display pagination.
	
	Revision 1.35  2005/08/01 16:32:31  brandon
	added "ORDER BY name" clause to the All... and Find... functions in the server objects
	
	Revision 1.34  2005/07/28 23:01:54  brandon
	fixed bug in GetAllPathwaysForQueryInterface
	
	Revision 1.33  2005/07/28 22:46:15  brandon
	finished the PathwaysWithinStepsFromPathway.ascx query interface, hope it works
	
	Revision 1.32  2005/07/28 21:00:52  michael
	Updating display window to accept simple query controls
	
	Revision 1.31  2005/07/27 22:16:25  brandon
	Added find (search by substring) functions in ServerPathway and ServerProcess.  Fixed the find function in the others ( the 'Ends with' query was wrong )
	
	Revision 1.30  2005/07/26 21:13:05  brendan
	Fixed bug in ServerObject that was breaking AllPathways in the web service.
	
	Revision 1.29  2005/07/21 19:59:13  brandon
	add function to ServerGene.cs to get the organism or organism group for a gene
	
	Revision 1.28  2005/07/20 18:02:19  brandon
	added function to ServerPathway: GetConnectedPathways ( ), which returns an array of ConnectedPathwayAndCommonProcesses objects.  This new object has three properties:
	ServerPathway ConnectedPathway- (to be listed as a connected pathway)
	ServerProcess[] SharedProcesses - (shared by two pathways)
	ServerMolecularEntity[] SharedExclusiveMolecules - (molecules shared
	by two pathways but are not included in any process in SharedProcesses)
	
	Revision 1.27  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.26  2005/07/18 17:23:39  brandon
	Added queries:
	1. get all pathways that a given molecular entity is involved in
	2. get all pathways that a given pathway is linked to, as well as the molecular entity they have in common
	
	Revision 1.25  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.24  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
	Revision 1.23  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.22  2005/07/11 18:48:56  brendan
	Moved SetSqlCommandParameters() call from constructors to ServerObject.UpdateDatabase().  Added object creation constructor to ServerPathway/SoapPathway.
	
	Revision 1.21  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.20  2005/07/07 23:30:51  brendan
	Work in progress on entity names.  MolecularEntityName virtually complete, but not tested.
	
	Revision 1.19  2005/07/06 20:18:21  brandon
	Added server objects for RNA and EC number.  Done with the relation between Pathway and Process, still working on relation between Process and Organism Group.  Function AddProcessToOrganismGroup still not working, can't figure out why
	
	Revision 1.18  2005/07/05 22:10:56  brandon
	Added test file, created static methods in ServerPathway.cs to handle the pathway_process relation, and an instance method for it in the ServerProcess.  TODO: add instance method for the static method in ServerPathway
	
	Revision 1.17  2005/07/05 21:08:15  brendan
	Added a CommandBuild convience function to DBWrapper to simplify the code for calling SQL commands with @ params.  Modified ServerPathway to use this new function and tested it.
	
	Revision 1.16  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.15  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.14  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.13  2005/06/28 21:53:06  brendan
	ServerPathway now works properly for INSERT, SELECT, UPDATE and DELETE.  Changes have been to DBWrapper, DBRow and ServerObject to support this properly.
	
	Revision 1.12  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.11  2005/06/22 22:06:31  brendan
	Progress on data model.  FieldLoader functions moved to DBRow.  Working on support for insert.
	
	Revision 1.10  2005/06/22 18:39:11  michael
	Changing data model again to encapsulate the ADO.NET funcationality further.
	Updating the classes that used the old functionality to use the new DBRow class.
	
	Revision 1.9  2005/06/20 19:39:31  michael
	debugging ADO updating of database.
	
	Revision 1.8  2005/06/20 17:53:15  michael
	Bug fixes
	
	Revision 1.7  2005/06/16 21:14:11  michael
	testing data model
	
	Revision 1.6  2005/06/16 19:09:16  michael
	Demo of ServerPathway.
	
	Revision 1.5  2005/06/16 17:14:21  michael
	further work on the new object data model
	
	Revision 1.4  2005/06/16 16:10:50  michael
	finishing up DBWrapper class and beginning work on creating the object model.
	
	Revision 1.3  2005/06/14 20:50:38  michael
	Finishing refactoring of DBWrapper and begin implementation of ServerPathway
	
	Revision 1.2  2005/06/10 20:31:52  brendan
	Added ServerObject and SoapObject base classes and code in progress for ServerPathway and Pathway.
	
	Revision 1.1  2005/06/08 20:44:10  brendan
	Adding skeleton projects for refactoring into the 3.0 version.
		
------------------------------------------------------------------------*/
#endregion