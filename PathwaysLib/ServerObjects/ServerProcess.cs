#region Using Declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
//using PathwaysLib.SBObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
using PathwaysLib.GraphObjects;
#endregion

namespace PathwaysLib.ServerObjects
{	

	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/ServerObjects/ServerProcess.cs</filepath>
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
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: murat $</cvs_author>
	///			<cvs_date>$Date: 2010/11/19 21:13:30 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerProcess.cs,v 1.10 2010/11/19 21:13:30 murat Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.10 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to biological processes.
	/// </summary>
	#endregion
	public class ServerProcess : ServerObject, IProcess, IGraphSpecificProcess
	{

		#region Constructor, Destructor, ToString
		private ServerProcess()
		{
		}

		/// <summary>
		/// Constructor for server process wrapper with fields initiallized
		/// </summary>
		/// <param name="name"></param>
		/// <param name="reversible"></param>
		/// <param name="location"></param>
		/// <param name="notes"></param>
		/// <param name="genericProcessId"></param>
		public ServerProcess( string name, Tribool reversible, string location, string notes, Guid genericProcessId)
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ID = DBWrapper.NewID(); // generate a new ID
			this.Name = name;
			this.Reversible = reversible;
			this.Location = location;
			this.ProcessNotes = notes;
			this.GenericProcessID = genericProcessId;
            
		}

		/// <summary>
		/// Constructor for server process wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerProcess object from a
		/// SoapProcess object.
		/// </remarks>
		/// <param name="data">
		/// A SoapProcess object from which to construct the
		/// ServerProcess object.
		/// </param>
		public ServerProcess ( SoapProcess data )
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
		/// Constructor for server process wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerProcess object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerProcess ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

		/// <summary>
		/// Destructor for the ServerProcess class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerProcess()
		{
		}
		#endregion
        
		#region Member Variables
		private static readonly string __TableName = "processes";
		#endregion
        
		#region Properties
		/// <summary>
		/// Get/set the Process ID.
		/// </summary>
		public Guid ID
		{
			get
			{
				return __DBRow.GetGuid( "id" );
			}
			set
			{
				__DBRow.SetGuid( "id", value );
			}
		}
		/// <summary>
		/// Get/set the Process name.
		/// </summary>
		public string Name
		{
			get
			{
				return __DBRow.GetString( "name" );
			}
			set
			{
				__DBRow.SetString( "name", value );
			}
		}		
		/// <summary>
		/// Get/set the Process reversibility.
		/// </summary>
		public Tribool Reversible
		{
			get
			{
				return __DBRow.GetTribool( "reversible" );
			}
			set
			{
				__DBRow.SetTribool( "reversible", value );
			}
		}				
		/// <summary>
		/// Get/set the Process Location.
		/// </summary>
		public string Location
		{
			get
			{
				return __DBRow.GetString( "location" );
			}
			set
			{
				__DBRow.SetString( "location", value );
			}
		}			

		public string ProcessNotes
		{
			get
			{
				return __DBRow.GetString( "notes" );
			}
			set
			{
				__DBRow.SetString( "notes", value );
			}
		}

		/// <summary>
		/// Get/set the generic process id.
		/// </summary>
		public Guid GenericProcessID
		{
			get
			{
				return __DBRow.GetGuid( "generic_process_id" );
			}
			set
			{
				__DBRow.SetGuid( "generic_process_id", value );
			}
		}

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsGenericProcess
        {
            get {return false;}
        }

        public bool IsTransport
        {
            get
            {
                return __DBRow.GetBool("is_transport");
            }
            set
            {
                __DBRow.SetBool("is_transport", value);
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
			SoapProcess retval = (derived == null) ? 
				retval = new SoapProcess() : retval = (SoapProcess)derived;

			retval.ID = this.ID;
			retval.Name = this.Name;
			retval.Reversible = this.Reversible;
			retval.Location = this.Location;
			
			retval.ProcessNotes = this.ProcessNotes;
			retval.GenericProcessID = this.GenericProcessID;
           
			retval.Status = ObjectStatus.NoChanges;

			return retval;
		}

		/// <summary>
		/// Consumes a SoapObject object and updates the ServerProcess
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the Process.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
            //SoapProcessSB p = o as SoapProcessSB;

            //if (o.Status == ObjectStatus.Insert && p.ID == Guid.Empty)
            //    p.ID = DBWrapper.NewID(); // generate a new DIp

            //this.ID = p.ID;
            //this.Name = p.Name;
            //this.Reversible = p.Reversible;
            //this.Location = p.Location;
		
            ////this.ProcessNotes = p.ProcessNotes;
            //this.GenericProcessID = p.GenericProcessID;
		}

        /// <summary>
        /// Return all reactions for this process.
        /// </summary>
        /// <returns></returns>
        public ServerReaction[] GetAllReactions()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM reaction
				WHERE [id] IN ( SELECT reactionId
									FROM MapReactionsProcessEntities
									WHERE processId = @id )
				ORDER BY sbmlId;",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));
            }

            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }

        /// <summary>
        /// Return all reaction annotations for this process.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, BiomodelAnnotation> GetAllReactionAnnotations()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT reactionId, qualifierId
				FROM MapReactionsProcessEntities
				WHERE processId = @id;",
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
                Guid modelId = new Guid(row["reactionId"].ToString());
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
		/// Add this process to the specified pathway
		/// </summary>
		/// <param name="pathway_id"></param>
		/// <param name="notes"></param>
		public void AddToPathway ( Guid pathway_id, string notes )
		{
			ServerPathway.AddProcessToPathway(pathway_id, this.ID, notes);
		}

		/// <summary>
		/// Remove this process from the given pathway
		/// </summary>
		/// <param name="pathway_id"></param>
		public void RemoveFromPathway ( Guid pathway_id )
		{
			ServerPathway.RemoveProcessFromPathway(pathway_id, this.ID);
		}

		/// <summary>
		/// Return all pathways for this process.
		/// </summary>
		/// <returns></returns>
		public ServerPathway[] GetAllPathways ( )
		{
			System.Data.SqlClient.SqlCommand command = DBWrapper.BuildCommand(
					@"SELECT pat.*
					FROM pathways pat
					WHERE pat.id IN ( SELECT pp.pathway_id
										FROM pathway_processes pp
										WHERE pp.process_id = @process_id )
					ORDER BY pat.[name];",
				"@process_id", SqlDbType.UniqueIdentifier, this.ID);

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerPathway( new DBRow( d ) ) );
			}

			return ( ServerPathway[] ) results.ToArray( typeof( ServerPathway ) );
		}
		#endregion


		#region Additional Queries
		/// <summary>
		/// Returns all the organisms or organism groups for this process
		/// </summary>
		/// <returns></returns>
		public ServerOrganismGroup[] GetAllOrganisms( )
		{
			return ServerProcess.GetAllOrganismsForProcess( this.ID);
		}

		/// <summary>
		/// Returns all organism groups for this process
		/// </summary>
		/// <returns></returns>
		public ServerOrganismGroup[] GetAllOrganismGroups( )
		{
			return ServerProcess.GetAllOrganismGroupsForProcess( this.ID);
		}

		/// <summary>
		/// Returns all the processes that share this process' generic process id
		/// </summary>
		/// <returns></returns>
		public ServerProcess[] AllProcessesSharingThisGenericProcessId ( )
		{
			return ServerProcess.GetProcessesByGenericProcessId( this.GenericProcessID );
		}

		#endregion

		#region Catalyzes Relation
		/// <summary>
		/// Adds a gene product to the catalyzing relation
		/// </summary>
		/// <param name="gene_product_id"></param>
		/// <param name="orgGroupId"></param>
		/// <param name="ec_number"></param>
		public void AddGeneProduct(Guid gene_product_id, Guid orgGroupId, string ec_number)
		{
			ServerCatalyze.AddGeneProductToProcess( gene_product_id, this.ID, orgGroupId, ec_number );
		}

		/// <summary>
		/// Remove a gene product from the process
		/// </summary>
		/// <param name="gene_product_id"></param>
		/// <param name="orgGroupId"></param>
		/// <param name="ec_number"></param>
		public void RemoveGeneProduct(Guid gene_product_id, Guid orgGroupId, string ec_number)
		{
			ServerCatalyze.RemoveGeneProductFromProcess( gene_product_id, this.ID, orgGroupId, ec_number);
		}

		/// <summary>
		/// Get all gene products
		/// </summary>
		/// <returns>
		/// Returns all of the gene products involved in the process
		/// </returns>
		public ServerGeneProduct[] GetAllGeneProducts( )
		{
			return ServerCatalyze.GetAllGeneProductsForProcess( this.ID );
		}

        /// <summary>
        /// Get all gene products in a specific organism
        /// </summary>
        /// <returns>
        /// Returns all of the gene products involved in the process within a specific organism
        /// </returns>
        public ServerGeneProduct[] GetAllGeneProductsInOrganism(Guid orgId)
        {
            return ServerCatalyze.GetAllGeneProductsForProcessInOrganism(this.ID, orgId);
        }
		

		/// <summary>
		/// Returns the ec number for an enzyme in this process
		/// </summary>
		/// <param name="geneProductId"></param>
		/// <returns></returns>
		public ServerECNumber GetECNumberByGeneProduct ( Guid geneProductId )
		{
			return ServerCatalyze.GetECNumberForGeneProductAndProcess( this.ID, geneProductId );
		}

		/// <summary>
		/// Returns the EC Number(s) for this process from 
		/// gene_products_and_processes table
		/// </summary>
		/// <returns></returns>
		public ServerECNumber[] GetECNumbers ( )
		{
			return ServerCatalyze.GetECNumbersForProcess ( this.ID );
		}
		#endregion

		#region Process Entities Relation
		/// <summary>
		/// Returns all Molecular Entities involved in this process
		/// </summary>
		/// <returns></returns>
		public ServerMolecularEntity[] GetAllMolecularEntities ( )
		{
			return ServerMolecularEntity.GetAllEntitiesInProcess( this.ID );
		}
		
		/// <summary>
		/// Get all entries in the process_entities (catalyze) relation for this process
		/// </summary>
		/// <returns>
		/// an array of ServerProcessEntity objects
		/// </returns>
		public ServerProcessEntity[] GetAllProcessEntities ( )
		{
			return ServerProcessEntity.GetAllForProcess( this.ID );
		}

		/// <summary>
		/// Add the given molecular entity to this process with the specified role
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="role"></param>
		/// <param name="quantity"></param>
		/// <param name="notes"></param>
		public void AddMolecularEntity ( Guid entityId, string role, int quantity, string notes )
		{
			ServerProcessEntity.AddMolecularEntityToProcess ( this.ID, entityId, role, quantity, notes );
		}

		/// <summary>
		/// Remove the given molecular entity from the process
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="role"></param>
		public void RemoveMolecularEntity ( Guid entityId, string role )
		{
			ServerProcessEntity.RemoveMolecularEntityFromProcess ( this.ID, entityId, role );
		}

		#endregion


		#region ADO.NET SqlCommands
		/// <summary>
		/// Required function for settign up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (GJS)
			// Rewrote using DBWrapper.BuildCommand

        /*    __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, name, formula) VALUES (@id, @name, @formula);",
                    "@id", SqlDbType.UniqueIdentifier, ID,
                    "@name", SqlDbType.VarChar, Name,
                    "@sbmlData", SqlDbType.VarChar, SbmlData,
                    "@creationDate",SqlDbType.DateTime, CreationDate,
                    "@lastModDate", SqlDbType.DateTime, LastModDate
                );

*/

           
			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + @"(id, name, reversible, location, notes, generic_process_id) VALUES (@i_id, @i_name, @i_reversible, @i_location, @i_notes, @i_generic_process_id);",
				"@i_id", SqlDbType.UniqueIdentifier, ID,
				"@i_name", SqlDbType.VarChar, Name,
				"@i_reversible", SqlDbType.Bit, Reversible,
				"@i_location", SqlDbType.VarChar, Location,
				"@i_notes", SqlDbType.Text, ProcessNotes,
				"@i_generic_process_id", SqlDbType.UniqueIdentifier, GenericProcessID);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @s_id;",
				"@s_id", SqlDbType.UniqueIdentifier, ID );

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + @"
				SET [name] = @u_name, reversible = @u_reversible, location = @u_location,
					notes = @u_notes, generic_process_id = @u_generic_process_id
                    
				WHERE id = @u_id;",
				"@u_name", SqlDbType.VarChar, Name,
				"@u_reversible", SqlDbType.Bit, Reversible,
				"@u_location", SqlDbType.VarChar, Location,
				"@u_notes", SqlDbType.Text, ProcessNotes,
				"@u_generic_process_id", SqlDbType.UniqueIdentifier, ID);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE id = @d_id;",
				"@d_id", SqlDbType.UniqueIdentifier, ID );			
		}
		#endregion
		#endregion
           

		#region Static Methods
        public static ServerProcess[] FindProcessesWithReactions(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
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
                                                (SELECT processId FROM MapReactionsProcessEntities) 
                                    ORDER BY [name] ) " + __TableName + @"
									ORDER BY [name] DESC ) " + __TableName + @"
				ORDER BY [name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }

            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

       
        public static int CountFindProcessesWithReaction(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " WHERE [name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring " +
                    @" AND [id] IN
                          (SELECT processId FROM MapReactionsProcessEntities);",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }


		/// <summary>
		/// Expands a neighborhood by one step (I guess you knew that from the function name, huh?)
		/// </summary>
		/// <param name="expansionSeed"></param>
		/// <param name="excludeSet"></param>
		/// <param name="organismGroupId"></param>
		/// <returns></returns>
		public static Guid[] ExpandNeighborhoodOneStep(Guid[] expansionSeed, 
			ref ArrayList excludeSet, Guid organismGroupId)
		{
			ArrayList results = new ArrayList();

			foreach (Guid process1 in expansionSeed)
			{
				SqlCommand command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT p2.processes_id
					FROM process_entities p1, process_entities p2
					WHERE p1.process_id = @process1 AND p1.entity_id = p2.entity_id
						AND p1.process_id <> p2.process_id AND (p1.role_id = @substrateid OR p1.role_id = @productid)
						AND (p2.role_id = @substrateid OR p2.role_id = @productid
						AND (p1.entity_id NOT IN (SELECT * FROM common_molecules))",
					"@process1", SqlDbType.UniqueIdentifier, process1,
					"@substrateid", SqlDbType.TinyInt, ProcessEntityRoleManager.GetRoleId("substrate"),
					"@productid", SqlDbType.TinyInt, ProcessEntityRoleManager.GetRoleId("product"));
				
				//sqlString += " AND p2.process_id IN (SELECT process_id FROM process_organism_groups WHERE organism_group_id='" + organismGroupId + "')";
				DataSet[] ds = new DataSet[0];
				DBWrapper.LoadMultiple( out ds, ref command );
				
				foreach ( DataSet d in ds )
				{
					DataRow row = d.Tables[0].Rows[0];
					Guid neighborGuid = (Guid)row[0];
					
					if (!excludeSet.Contains(neighborGuid))
					{
						results.Add(neighborGuid);
						excludeSet.Add(neighborGuid);
					}
				}

			}

//			string sqlString2 = "SELECT count(*) FROM pathways";
//			SqlCommand command2 = new SqlCommand(sqlString2);
//
//			DataSet ds2 = new DataSet();
//			DBWrapper.LoadSingle(out ds2, ref command2);
//
//			string countStr = ds2.Tables[0].Rows[0][0].ToString();
//
//			return int.Parse(countStr);

			return (( Guid[] ) results.ToArray( typeof( Guid ) ));
		}

		/// <summary>
		/// Return all processes from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapPreocess objects ready to be sent via SOAP.
		/// </returns>
		public static ServerProcess[] AllProcesses ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + " ORDER BY [name];" );
			
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
		/// Returns all of the processes for a specified organism.
		/// </summary>
		/// <param name="organism">
		/// The organism to filter on.
		/// </param>
		/// <returns>
		/// Set of server processes from the specified organism.
		/// </returns>
		public static ServerProcess[] AllProcessesForOrganism( string organism )
		{
			if ( organism == "All" || organism == null )
			{
				return AllProcesses();
			}

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT p.*
					FROM " + __TableName + @"p
					INNER JOIN catalyzes c ON p.id = c.process_id
					INNER JOIN organism_groups o ON c.organism_group_id = o.id
					WHERE (o.scientific_name = @sname OR o.common_name = @cname);",
				"@sname", SqlDbType.VarChar, organism,
				"@cname", SqlDbType.VarChar, organism);

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
		/// Return a page of ServerProcesses.
		/// </summary>
		/// <param name="startRecord">
		/// The starting record.
		/// </param>
		/// <param name="maxRecords">
		/// The number of records to return.
		/// </param>
		/// <returns>
		/// Page of ServerProcesses
		/// </returns>
		public static ServerProcess[] AllProcesses ( int startRecord, int maxRecords )
		{
			int bigNum = startRecord + maxRecords;

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											ORDER BY name ) " + __TableName + @"
									ORDER BY name DESC ) " + __TableName + @"
				ORDER BY name");
			
			DataSet[] ds;
			int r = DBWrapper.LoadMultiple( out ds, ref command );
			if (r < 1)
				return new ServerProcess[0];

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );
		}

        public static ServerProcess[] GetAllProcessesHasModels(Guid pathway_id)
        {
            SqlCommand command = DBWrapper.BuildCommand(@" SELECT p.* " +
                              " FROM Pathway_Processes pp, Processes p " +
                              " WHERE pp.pathway_id = @pathway_id AND pp.process_id = p.id " +
                              " AND p.id IN " +
                              " ( SELECT DISTINCT pe.processId  " +
                              " FROM MapReactionsProcessEntities pe ) " +
                              " ORDER BY p.name",
                              "@pathway_id", SqlDbType.UniqueIdentifier, pathway_id);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerProcess[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }

            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

		/// <summary>
		/// Get all processes for a pathway
		/// </summary>
		/// <param name="pathway_id">Pathway ID to use</param>
		/// <returns>
		/// All processes in that pathway
		/// </returns>
		public static ServerProcess[] GetAllProcessesForPathway ( Guid pathway_id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT pr2.*
					FROM processes pr2
					WHERE pr2.id IN (
						SELECT DISTINCT pr.id
							FROM processes pr, pathway_processes pp
							WHERE pp.process_id = pr.id AND pp.pathway_id = @pathway_id )
					ORDER BY pr2.name",
				"@pathway_id", SqlDbType.UniqueIdentifier, pathway_id);
			
			DataSet[] ds;
			int r = DBWrapper.LoadMultiple( out ds, ref command );
			if (r < 1)
				return new ServerProcess[0];

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );
		}

        public static List<Guid> GetAllProcessesIDsForPathway(Guid pwid)
        {
            DataSet results;
            List<Guid> returnList = new List<Guid>();
            DBWrapper.Instance.ExecuteQuery(out results,
                "SELECT process_id FROM pathway_processes where pathway_id='" + pwid + "'");

            foreach (DataRow r in results.Tables[0].Rows)
            {
                //Guid pwId = (r["pathwayId"] is DBNull) ? Guid.Empty : (Guid)r["pathwayId"];
                Guid entityId = (Guid)r["process_id"];
                //Guid graphNodeId = (Guid)r["graphNodeId"];
                returnList.Add(entityId);
            }
            return returnList;
        }
		
		/// <summary>
		/// Returns a page of processes filtered on organism.
		/// </summary>
		/// <param name="organism">
		/// The organism on which to filter.
		/// </param>
		/// <param name="startRecord">
		/// The starting record.
		/// </param>
		/// <param name="maxRecords">
		/// The size of the page.
		/// </param>
		/// <returns>
		/// A page of server processes.
		/// </returns>
		public static ServerProcess[] AllProcessesForOrganism ( string organism, int startRecord, int maxRecords )
		{
			if ( organism == "All" || organism == null )
			{
				return AllProcesses( startRecord, maxRecords );
			}

			int bigNum = startRecord + maxRecords;
			string query = "";
			query += "SELECT * FROM ( ";
			query += "	SELECT TOP " + maxRecords.ToString() + " * FROM ( ";
			//query += "		SELECT TOP " + bigNum.ToString() + " p.* FROM processes p INNER JOIN process_organism_groups pog ON p.id = pog.process_id INNER JOIN organism_groups og ON pog.organism_group_id = og.id INNER JOIN organisms o ON og.id = o.organism_group_id WHERE (o.common_name = @organism) ORDER BY p.[name];";
			query += "			SELECT TOP " + bigNum.ToString() + " p.* FROM "+__TableName+" p INNER JOIN catalyzes c ON p.id=c.process_id INNER JOIN organism_groups o ON c.organism_group_id=o.id WHERE (o.scientific_name=@sname OR o.common_name=@cname)"; 
			query += "	) " + __TableName + " ORDER BY name DESC";
			query += ") " + __TableName + " ORDER BY name";
			SqlCommand command = DBWrapper.BuildCommand(
				query,
					"@organism", SqlDbType.VarChar, organism
			);
			
			DataSet[] ds;
			int r = DBWrapper.LoadMultiple( out ds, ref command );
			if (r < 1)
				return new ServerProcess[0];

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );
		}

		/// <summary>
		/// The count of all processes in the system.
		/// </summary>
		/// <returns>
		/// The count of all processes in the system.
		/// </returns>
		public static int CountAllProcesses()
		{
			SqlCommand command = new SqlCommand( "SELECT COUNT(*) FROM " + __TableName + ";" );
			
			return ( int ) DBWrapper.Instance.ExecuteScalar( ref command );
		}

		/// <summary>
		/// The count of processes from a specific organism.
		/// </summary>
		/// <param name="organism">
		/// The supplied organism.
		/// </param>
		/// <returns>
		/// The count of processes from a specific organism.
		/// </returns>
		public static int CountAllProcessesForOrganism ( string organism )
		{
			if ( organism == "All" || organism == null )
			{
				return CountAllProcesses();
			}

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT COUNT(p.*)
					FROM " + __TableName + @" p
					INNER JOIN catalyzes c ON p.id = c.process_id
					INNER JOIN organism_groups o ON c.organism_group_id = o.id
					WHERE (o.scientific_name = @sname OR o.common_name = @cname);",
					"@sname", SqlDbType.VarChar, organism,
					"@cname", SqlDbType.VarChar, organism
			);

			return ( int ) DBWrapper.Instance.ExecuteScalar( ref command );
		}

        /// <summary>
		/// Returns all processes whose name contains the given substring
		/// </summary>
		/// <param name="substring"></param>
		/// <param name="searchMethod"></param>
		/// <returns></returns>
        public static ServerProcess[] FindProcesses(string substring, SearchMethod searchMethod)
        {
            return FindProcesses(substring, searchMethod, -1);
        }

		/// <summary>
		/// Returns all processes whose name contains the given substring
		/// </summary>
		/// <param name="substring"></param>
		/// <param name="searchMethod"></param>
        /// <param name="topk"></param>
		/// <returns></returns>
		public static ServerProcess[] FindProcesses(string substring, SearchMethod searchMethod, int topk)
		{
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

            string topkClause = "";
            if (topk > 0)
                topkClause = " TOP " + topk + " ";

			SqlCommand command = DBWrapper.BuildCommand(
                "SELECT " + topkClause + " * FROM " + __TableName + " WHERE [name] " +
				( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + " @substring ORDER BY [name]",
				"@substring", SqlDbType.VarChar, substring );

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
		/// Find a set of processes within the subset for a specific organism.
		/// </summary>
		/// <param name="organism">
		/// The filtering organism.
		/// </param>
		/// <param name="substring">
		/// The substring to search for.
		/// </param>
		/// <param name="searchMethod">
		/// The search method.
		/// </param>
		/// <returns>
		/// Set of processes matching search criteria.
		/// </returns>
		public static ServerProcess[] FindProcessesForOrganism( string organism, string substring, SearchMethod searchMethod)
		{
			if ( organism == "All" || organism == null )
			{
				return FindProcesses( substring, searchMethod );
			}

			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT p.*
					FROM " + __TableName + @" p
					INNER JOIN catalyzes c ON p.id = c.process_id
					INNER JOIN organism_groups o ON c.organism_group_id = o.id
					WHERE (o.scientific_name = @sname OR o.common_name = @cname)
						AND p.[name] WHERE [name] " + ( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + " @substring ORDER BY p.[name]",
				"@sname", SqlDbType.VarChar, organism,
				"@cname", SqlDbType.VarChar, organism,
				"@substring", SqlDbType.VarChar, substring );
			
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
		/// Page of processes meeting the search criteria.
		/// </summary>
		/// <param name="substring">
		/// The substring to search for.
		/// </param>
		/// <param name="searchMethod">
		/// The search method.
		/// </param>
		/// <param name="startRecord">
		/// The record to start at.
		/// </param>
		/// <param name="maxRecords">
		/// The page size.
		/// </param>
		/// <returns>
		/// Page of processes meeting the search criteria.
		/// </returns>
		public static ServerProcess[] FindProcesses(string substring, SearchMethod searchMethod, int startRecord, int maxRecords )
		{
			int bigNum = startRecord + maxRecords;

			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
					FROM ( SELECT TOP " + maxRecords.ToString() + @" *
							FROM ( SELECT TOP " + bigNum.ToString() + @" *
								FROM " + __TableName + " WHERE [name] " +
								( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + @" @substring
								ORDER BY name ) " + __TableName + @"
							ORDER BY name DESC ) " + __TableName + @"
                    where id NOT IN (select local_id from external_database_links)
				ORDER BY name",
				"@substring", SqlDbType.VarChar, substring );
			
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
		/// Find page of processes based on the search criteria.
		/// </summary>
		/// <param name="organism">
		/// The filtering organism.
		/// </param>
		/// <param name="substring">
		/// The search substring.
		/// </param>
		/// <param name="searchMethod">
		/// The search method.
		/// </param>
		/// <param name="startRecord">
		/// The starting record.
		/// </param>
		/// <param name="maxRecords">
		/// The page size.
		/// </param>
		/// <returns>
		/// Page of processes based on the search criteria.
		/// </returns>
		public static ServerProcess[] FindProcessesForOrganism(string organism, string substring, SearchMethod searchMethod, int startRecord, int maxRecords )
		{
			if ( organism == "All" || organism == null )
			{
				return FindProcesses( substring, searchMethod, startRecord, maxRecords );
			}

			int bigNum = startRecord + maxRecords;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT p.*
					FROM " + __TableName + @" p
					INNER JOIN catalyzes c ON p.id = c.process_id
					INNER JOIN organism_groups o ON c.organism_group_id = o.id
					WHERE (o.scientific_name = @sname OR o.common_name = @cname) AND p.[name] " +
					( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + " @substring ORDER BY p.[name] " +
                    " and p.id NOT IN (select local_id from external_database_links);",
				"@sname", SqlDbType.VarChar, organism,
				"@cname", SqlDbType.VarChar, organism,
				"@substring", SqlDbType.VarChar, substring );
			
			//query += "		SELECT TOP " + bigNum.ToString() + " p.* FROM processes p INNER JOIN process_organism_groups pog ON p.id = pog.process_id INNER JOIN organism_groups og ON pog.organism_group_id = og.id INNER JOIN organisms o ON og.id = o.organism_group_id WHERE (o.common_name = '" + organism + "') " + commandString + " ORDER BY p.[name]";
						
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
		/// The count of process responding to the search criteria.
		/// </summary>
		/// <param name="substring">
		/// The substring to search for.
		/// </param>
		/// <param name="searchMethod">
		/// The search method to use.
		/// </param>
		/// <returns>
		/// Count of matching processes.
		/// </returns>
		public static int CountFindProcesses ( string substring, SearchMethod searchMethod )
		{
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT COUNT(*)
					FROM " + __TableName + " WHERE [name] " +
					( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + " @substring "+
                    " and id NOT IN (select local_id from external_database_links);",
				"@substring", SqlDbType.VarChar, substring );
			
			DataSet[] ds = new DataSet[0];
			return ( int ) DBWrapper.Instance.ExecuteScalar( ref command );
		}

		/// <summary>
		/// Count processes matching search criteria.
		/// </summary>
		/// <param name="organism">
		/// The filtering organism.
		/// </param>
		/// <param name="substring">
		/// The substring to search for.
		/// </param>
		/// <param name="searchMethod">
		/// The search method to use.
		/// </param>
		/// <returns>
		/// Count of processes matching the search criteria.
		/// </returns>
		public static int CountFindProcessesForOrganism ( string organism, string substring, SearchMethod searchMethod )
		{
			if ( organism == "All" || organism == null )
			{
				return CountFindProcesses( substring, searchMethod );
			}

			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith ) substring = "%" + substring;
			if ( searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith ) substring += "%";

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT COUNT(p.name)
					FROM " + __TableName + @" p
					INNER JOIN catalyzes c ON p.id = c.process_id
					INNER JOIN organism_groups o ON c.organism_group_id = o.id
					WHERE (o.scientific_name = @sname OR o.common_name = @cname) AND p.[name] " +
					( searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=" ) + " @substring;",
				"@sname", SqlDbType.VarChar, organism,
				"@cname", SqlDbType.VarChar, organism,
				"@substring", SqlDbType.VarChar, substring );

//			SqlCommand command = new SqlCommand( "SELECT COUNT(p.name) AS Expr1 FROM processes p INNER JOIN process_organism_groups pog ON p.id = pog.process_id INNER JOIN organism_groups og ON pog.organism_group_id = og.id INNER JOIN organisms o ON og.id = o.organism_group_id WHERE (o.common_name = '" + organism + "') " + commandString );

			return ( int ) DBWrapper.Instance.ExecuteScalar( ref command );
		}

		/// <summary>
		/// Finds all processes related to a pathway and organism (or group).
		/// </summary>
		/// <param name="Org"></param>
		/// <param name="Path"></param>
		/// <returns></returns>
		public static ServerProcess[] ByOrganismAndPathway( ServerOrganismGroup Org, ServerPathway Path)
		{
			// Find all organisms under the current organism group
			SqlCommand command;

			if( Org == null )
			{
				command = DBWrapper.BuildCommand(
					@"SELECT p2.*
						FROM processes p2
						WHERE p2.id IN (
							SELECT DISTINCT p.id
								FROM processes p, pathway_processes pp
								WHERE pp.pathway_id = @pathID AND pp.process_id = p.id )
						ORDER BY p2.name",
					"@pathID", SqlDbType.UniqueIdentifier, Path.ID);
			}
			else
			{
				string RequiredIds = ServerOrganismGroup.RequireIdInList(Org.GetAllChildrenAndParent(), "c", "organism_group_id");
				command = DBWrapper.BuildCommand(
					@"SELECT p2.*
						FROM processes p2
						WHERE p2.id IN (
							SELECT DISTINCT p.id
								FROM processes p, pathway_processes pp, catalyzes c
								WHERE pp.pathway_id = @pathID AND pp.process_id = p.id AND p.id = c.process_id AND " + RequiredIds + @" )
						ORDER BY p2.name",
					"@pathID", SqlDbType.UniqueIdentifier, Path.ID);
			}

			return ServerProcess.LoadMultiple(command);
		}

		/// <summary>
		/// Return a process with given ID.
		/// </summary>
		/// <param name="id">
		/// The Guid of the desired process.
		/// </param>
		/// <returns>
		/// Object ready to be sent via SOAP.
		/// </returns>
		public static ServerProcess Load ( Guid id )
		{
			return new ServerProcess( LoadRow ( id ) );
		}

		/// <summary>
		/// Return the dataset for an object with a given ID.
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

		/// <summary>
		/// Creates an array of ServerProcess objects from an SQL query for multiple processes rows.
		/// </summary>
		/// <param name="command">A query for multiple rows of processes.  Note that the query must output
		/// all columns(not just the id's)!</param>
		/// <returns></returns>
		internal static ServerProcess[] LoadMultiple(SqlCommand command)
		{
			DataSet[] ds;
			DBWrapper.LoadMultiple(out ds, ref command);

			ArrayList results = new ArrayList();
			foreach(DataSet r in ds)
				results.Add(new ServerProcess(new DBRow(r)));

			return (ServerProcess[])results.ToArray(typeof(ServerProcess));
		}
        
        /// <summary>
		/// Creates a single ServerPathway from an SQL query
		/// </summary>
		/// <param name="command">An SQL query that returns a single row from processes.  Note that the query
		/// must output every column</param>
		/// <returns></returns>
		internal static ServerProcess LoadSingle(SqlCommand command)
		{
			DataSet ds;
			DBWrapper.LoadSingle(out ds, ref command);
			return new ServerProcess(new DBRow(ds));
		}

        /// <summary>
		/// Returns true if there exists a process with the given ID.
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
		/// given a process, gets the organism, or organism groups that have genes encoding at least one enzyme in the process.
		/// </summary>
		/// <param name="processId"></param>
		/// <returns></returns>
		public static ServerOrganismGroup[] GetAllOrganismsForProcess(Guid processId)
		{
			
//			SqlCommand command = DBWrapper.BuildCommand(
//				@"SELECT o.[id], o.[taxonomy_id]
//					FROM organisms o INNER JOIN 
//						organism_groups og ON o.id = og.id INNER JOIN
//						catalyzes c ON og.id = c.organism_group_id
//					WHERE c.process_id = @process_id
//				",
//					"@process_id", SqlDbType.UniqueIdentifier, processId);
					
			//(ac) Why are we joining organism with organism_group? Using organism_group should be enough..
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
					FROM organism_groups og
					INNER JOIN catalyzes c ON og.id = c.organism_group_id
					WHERE c.process_id = @process_id",
				"@process_id", SqlDbType.UniqueIdentifier, processId);
			
			return ServerOrganismGroup.LoadMultiple(command);
		}

		/// <summary>
		/// given a process, gets the organism groups that have genes encoding at least one enzyme in the process.
		/// </summary>
		/// <param name="processId"></param>
		/// <returns></returns>
		public static ServerOrganismGroup[] GetAllOrganismGroupsForProcess(Guid processId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT DISTINCT og.[id], og.[common_name]
					FROM organism_groups og
					INNER JOIN catalyzes c ON og.id = c.organism_group_id
					WHERE c.process_id = @process_id AND is_organism = 0",
				"@process_id", SqlDbType.UniqueIdentifier, processId);
			
			return ServerOrganismGroup.LoadMultiple(command);
		}

        /// <summary>
        /// given a process, gets the organism that have genes encoding at least one enzyme in the generic process.
        /// </summary>
        /// <param name="genericProcessId"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] GetAllOrganismsForGenericProcess(Guid genericProcessId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                /*"SELECT o.[id], o.[taxonomy_id]
				 * FROM organisms o INNER JOIN
				 *		chromosomes c  ON o.[id] = c.organism_id
				 * WHERE c.id IN (
				 *		SELECT g.chromosome_id
				 *		FROM genes g WHERE g.[id] IN (
				 *			SELECT ge.gene_id
				 *			FROM gene_encodings ge INNER JOIN
				 *				gene_product_and_processes gpp ON ge.gene_product_id = gpp.gene_product_id INNER JOIN
				 *				processes p ON p.id = gpp.process_id
				 *			WHERE p.generic_process_id = @generic_process_id
				 *		)
				 * );",*/
				@"SELECT o.[id] o.[taxonomy_id]
					FROM organism o
					INNER JOIN organism_groups og ON o.id = og.id
					INNER JOIN catalyzes c ON og.id = c.organism_group_id
					INNER JOIN processes p ON c.process_id = p.id
					WHERE p.generic_process_id = @generic_process_id",
                "@generic_process_id", SqlDbType.UniqueIdentifier, genericProcessId);
			
			return ServerOrganismGroup.LoadMultiple(command);
        }

        /// <summary>
        /// given a process, gets the organism, or organism groups that have genes encoding at least one enzyme in the generic process.
        /// </summary>
        /// <param name="genericProcessId"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] GetAllOrganismGroupsForGenericProcess(Guid genericProcessId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT og.*
					FROM organism_groups og
					INNER JOIN catalyzes c ON og.id = c.organism_group_id
					INNER JOIN processes p ON c.process_id = p.id
					WHERE p.generic_process_id = @generic_process_id
				",
                "@generic_process_id", SqlDbType.UniqueIdentifier, genericProcessId);
			
			return ServerOrganismGroup.LoadMultiple(command);
        }
		/// <summary>
		/// Returns all processes with the given generic process id
		/// </summary>
		/// <param name="genericProcessId"></param>
		/// <returns></returns>
		public static ServerProcess[] GetProcessesByGenericProcessId(Guid genericProcessId)
		{
            //BE: check if this is a graph node ID instead
            
            genericProcessId = GraphNodeManager.GetGenericProcessId(genericProcessId); // if not a graph node ID, returns same ID

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
					FROM processes
					WHERE generic_process_id = @generic_process_id;",
				"@generic_process_id", SqlDbType.UniqueIdentifier, genericProcessId);

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );

		}

		#endregion

	} // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerProcess.cs,v 1.10 2010/11/19 21:13:30 murat Exp $
	$Log: ServerProcess.cs,v $
	Revision 1.10  2010/11/19 21:13:30  murat
	1-) Insert sql query was not structure in ServerSBase and ServerModel files those
	queries are fixed.
	
	2-) Some of the queries from ServerGoTerms, ServerOrganismGroup, ServerPathway and ServerProsess
	were wrong, they are fixed (m.sbmlId changed to sbmlId)
	
	3-) SMBLParser now can work automatically even the model names (or any other model attributes) have changed.
	
	Revision 1.9  2010/05/26 20:37:15  ann
	*** empty log message ***
	
	Revision 1.8  2009/09/09 15:48:14  xjqi
	*** empty log message ***
	
	Revision 1.7  2009/07/14 15:33:22  ann
	*** empty log message ***
	
	Revision 1.6  2009/05/27 14:04:18  ann
	*** empty log message ***
	
	Revision 1.5  2009/05/19 13:22:25  ali
	*** empty log message ***
	
	Revision 1.4  2009/04/03 21:16:03  xjqi
	Visualization Part: Given a model id, visualize compartments in hierarchy with species,reactions.
	Not integrating with web interface.
	
	Revision 1.3  2009/01/07 19:47:15  ann
	*** empty log message ***
	
	Revision 1.2  2008/06/17 15:38:46  akaraca
	graph drawing coloring information is now sent to the client
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.14  2008/05/16 15:29:44  akaraca
	*** empty log message ***
	
	Revision 1.13  2008/05/09 18:33:25  divya
	*** empty log message ***
	
	Revision 1.12  2008/05/07 05:02:45  akaraca
	*** empty log message ***
	
	Revision 1.11  2008/05/02 18:46:19  divya
	*** empty log message ***
	
	Revision 1.10  2008/05/01 02:20:24  divya
	*** empty log message ***
	
	Revision 1.9  2008/03/26 18:16:34  akaraca
	user interface changes for Pathcase SB
	
	Revision 1.8  2008/03/07 20:55:17  brendan
	bugfixes
	
	Revision 1.7  2008/03/07 19:53:13  brendan
	AQI refactoring
	
	Revision 1.6  2008/02/20 22:46:59  divya
	*** empty log message ***
	
	Revision 1.5  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.4  2007/04/06 18:09:25  chirag
	work on autocomplete
	
	Revision 1.3  2006/11/26 21:49:25  ali
	*** empty log message ***
	
	Revision 1.2  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.54  2006/07/24 15:08:14  greg
	Minor tweaks here and there; nothing major.
	
	Revision 1.53  2006/07/07 19:28:04  greg
	The bulk of this update focuses on integrating Ajax browsing into the content browser bar on the left.  It currently only works from the pathways dropdown option, but the framework is now in place for the other lists to function in the same manner.
	
	Revision 1.52  2006/06/16 10:23:35  greg
	This update handles another handful of appearance-related issues, particularly those concerning IE, and also addresses some issues with built-in queries.  At this time, however, the queries still need to be more thoroughly tested.
	
	Revision 1.51  2006/06/13 01:52:04  ali
	*** empty log message ***
	
	Revision 1.50  2006/06/07 19:49:14  greg
	These updates more or less cover some additional issues with the ServerObjects as well as a bug where the GO would crash if it tried to compute a hypergeometric distribution using invalid parameters (now it just gives a subtle error message and continues on).
	
	Revision 1.49  2006/06/02 16:10:15  greg
	All built-in queries except the bottom three on the list (on the main website) should work correctly now.  However, there is still an issue communicating with the Java viewer; it appears as if these new queries are not passing the viewer the correct organism information.  This causes the graph to start off with no organism selected, but the links are still all correct.
	
	ServerObjects have also been updated to accomodate for the new queries.
	
	The HyperGraph class has also been moved over from the old service.  It seems like it may be a bit clunky (as with all the old query code), but it should work properly at least.  In the future perhaps we can streamline the code a bit to make it more efficient/cleaner.
	
	Revision 1.48  2006/05/23 18:29:32  greg
	Many old SQL queries were updated/optimized, and a bug that causes the system to crash when trying to navigate after viewing the details of an object through the Java applet is also fixed.  This required some semi-substantial modifications to LinkHelper.cs and SearchPagination.ascx.cs to allow for a slightly different method of dealing with query parameters.
	
	Revision 1.47  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.46  2006/05/17 17:07:28  brendan
	Fixed a bug in server process that was breaking graph generation and bad exception in the constructor of graph control; added a bit more error output to graph drawing
	
	Revision 1.45  2006/05/17 16:26:04  greg
	 - Search pagination errors
	Trying to access pages that are out-of-bounds results in SQL exceptions.  Additionally, the pagination function seemed to generate more pages than it should have at times, specifically when viewing the last page.  These are both resolved, which also seems to fix the "xx" bug.
	
	 - Potential SQL injection issue
	Search terms were not checked for SQL injection attacks.  Many SQL queries in the different server classes were rewritten to use parameters and thus eliminate the potential issue of SQL injection.
	
	 - Interface enhancement
	The search-type dropdown box now reverts to the last-selected option when the page reloads; prior to that it always went to the top.
	
	 - User input validation
	Most (if not all) user input was not being validated on entry, meaning it was possible to perform cross-site scripting and all that kind of nasty stuff.  Input is now stripped of HTML tags.  Note that validateRequest is now turned off, so all user input has to be validated in this way.
	
	Revision 1.44  2006/05/12 21:25:46  ali
	For processes no organisms were listed. This was due to the fact that all the functions returning organism lists contained queries that join organism and organism group tables which can technically work only for mouse, and human or real organisms. Now all such methods operate on organism group table as it contains all organisms and organism groups.
	
	Revision 1.43  2006/05/11 21:18:33  brendan
	Fixed numerous bugs, basic browsing starting to work again
	
	Revision 1.42  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.41  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.40  2006/04/21 17:37:29  michael
	*** empty log message ***
	
	Revision 1.39  2006/04/12 20:24:14  brian
	*** empty log message ***
	
	Revision 1.38.2.5  2006/03/29 22:56:51  brian
	*** empty log message ***
	
	Revision 1.38.2.4  2006/03/22 21:25:21  brian
	Removed and renamed a few more functions for consistency
	
	Revision 1.38.2.3  2006/03/22 20:24:16  brian
	I forgot to list my changes over the last few commits, so here's a quick overview:
	1. IOrganismEntity interface removed
	2. All ServerObject queries that return IOrganismEntity now return either ServerOrganism or ServerOrganismGroup
	3. All OrganismGroup-Process relations have been commented out (pending removal)
	4. Several new functions have been added to ServerOrganism and ServerOrganismGroup too simplify the initialization of objects
	
	Revision 1.38.2.2  2006/03/22 19:48:06  brian
	*** empty log message ***
	
	Revision 1.38.2.1  2006/03/12 06:43:16  brian
	I've rewired several parts of ServerOrganism so that it derives from ServerOrganismGroup.  I'm not too crazy about this new model, but I'll see what happens.
	
	Revision 1.38  2006/02/28 18:03:39  brian
	*** empty log message ***
	
	Revision 1.37.2.1  2006/02/14 20:57:35  brian
	1. Broken the code to reveal (all?) references to PathwaysService2
	2. Fixed several warnings due to outdated params tags
	
	Revision 1.37  2006/02/10 15:42:40  fatih
	*** empty log message ***
	
	Revision 1.36  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.35  2005/10/31 20:27:45  fatih
	*** empty log message ***
	
	Revision 1.34  2005/10/28 10:48:29  fatih
	Pathway, process, organism group tested and corrected
	
	Revision 1.33  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.32  2005/08/09 00:28:34  michael
	Adding organism selection to browser
	
	Revision 1.31  2005/08/05 16:05:58  brandon
	Added query for getting all processes with a given generic process id
	
	Revision 1.30  2005/08/04 01:29:59  michael
	Debugging search and pagination
	
	Revision 1.29  2005/08/03 19:18:44  brandon
	*** empty log message ***
	
	Revision 1.28  2005/08/03 05:31:17  michael
	Working on searh and results/display pagination.
	
	Revision 1.27  2005/08/01 16:32:31  brandon
	added "ORDER BY name" clause to the All... and Find... functions in the server objects
	
	Revision 1.26  2005/07/29 20:48:25  brandon
	added an "ORDER BY name" clause to some "GetAll" functions
	
	Revision 1.25  2005/07/27 22:16:25  brandon
	Added find (search by substring) functions in ServerPathway and ServerProcess.  Fixed the find function in the others ( the 'Ends with' query was wrong )
	
	Revision 1.24  2005/07/22 21:16:34  ali
	*** empty log message ***
	
	Revision 1.23  2005/07/20 22:31:20  brandon
	Added exists to Pathway and Process, add Exists for names in Organism and OrganismGroup
	
	Revision 1.22  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.21  2005/07/15 21:02:00  brandon
	added more queries
	
	Revision 1.20  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.19  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	Revision 1.18  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
	Revision 1.17  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.16  2005/07/11 16:54:39  brandon
	Added ServerProcessEntity and Soap...  for the process_entities relation.  Added funtion GetAllProcesses in ServerMolecularEntity, but GetAllEntities won't work, maybe because the ServerMolecularEntity constructor is protected.  Haven't done any testing yet.
	
	Revision 1.15  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.14  2005/07/08 19:32:05  brandon
	fixed ServerCatalyze, sort of,  and uh, this project builds now
	
	Revision 1.13  2005/07/07 19:42:20  brandon
	did more on the catalyzes relation, don't know exactly how to get EC# more involved (?)
	
	Revision 1.12  2005/07/07 15:10:28  brandon
	Added ServerCatalyze.cs (gene_product_and_processes), it's not done yet, and added the GetAllOrganismGroups function to ServerProcess object
	
	Revision 1.11  2005/07/06 20:18:21  brandon
	Added server objects for RNA and EC number.  Done with the relation between Pathway and Process, still working on relation between Process and Organism Group.  Function AddProcessToOrganismGroup still not working, can't figure out why
	
	Revision 1.10  2005/07/05 22:10:56  brandon
	Added test file, created static methods in ServerPathway.cs to handle the pathway_process relation, and an instance method for it in the ServerProcess.  TODO: add instance method for the static method in ServerPathway
	
	Revision 1.9  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.8  2005/06/29 16:50:20  brandon
	Fixed a couple of syntax errors
	
	Revision 1.7  2005/06/29 16:44:53  brandon
	Added Insert, Update, and Delete support to these files if they didn't already have it
	
	Revision 1.6  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.5  2005/06/22 18:39:11  michael
	Changing data model again to encapsulate the ADO.NET funcationality further.
	Updating the classes that used the old functionality to use the new DBRow class.
	
	Revision 1.4  2005/06/21 19:31:13  brandon
	Added ServerOrganism.cs and fixed some errors in ServerProcess.cs
	
	Revision 1.3  2005/06/20 21:56:07  brandon
	ServerProcess.cs builds.  It should execute too if you wanna add it to the project and try it
	
	Revision 1.2  2005/06/17 20:52:06  brandon
	It builds!
	
	Revision 1.1  2005/06/16 21:38:49  brandon
	ServerObject for processes
	It won't build yet because there is no 'SoapProcess' class

------------------------------------------------------------------------*/
#endregion