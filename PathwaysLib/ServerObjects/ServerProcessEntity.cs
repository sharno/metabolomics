#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
using PathwaysLib.GraphObjects;
using System.Collections.Generic;
#endregion

namespace PathwaysLib.ServerObjects
{
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerProcessEntity.cs</filepath>
	///		<creation>2005/07/05</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@cwru.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@cwru.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Marc R. Reynolds</name>
	///				<initials>mrr</initials>
	///				<email>marc.reynolds@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: ali $</cvs_author>
	///			<cvs_date>$Date: 2009/04/07 14:44:13 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerProcessEntity.cs,v 1.3 2009/04/07 14:44:13 ali Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.3 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Represents one of possibly many names associated with a molecular entity.
	/// </summary>
	#endregion
    public class ServerProcessEntity : ServerObject, IGraphProcessEntity
    {
        #region Constructor, Destructor, ToString

        private ServerProcessEntity()
        {
        }
		
        /// <summary>
        /// Constructor for server catalyze wrapper with fields initiallized 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="entityId"></param>
        /// <param name="role"></param>
        /// <param name="quantity"></param>
        /// <param name="notes"></param>
        public ServerProcessEntity ( Guid processId, Guid entityId, string role, string quantity, string notes )
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow( __TableName );

            this.ProcessID = processId;
            this.EntityID = entityId;
            this.Role = role;
            this.Quantity = quantity;
            this.ProcessEntityNotes = notes;
            
        }

        /// <summary>
        /// Create a ServerProcessEntity object from the given Soap object
        /// </summary>
        /// <param name="data"></param>
        public ServerProcessEntity ( SoapProcessEntity data )
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
                    __DBRow = LoadRow(data.ProcessID, data.EntityID, data.Role);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // (BE) get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public ServerProcessEntity ( DBRow data )
        {
            // (mfs)
            // setup object
            __DBRow = data;

        }

        #endregion

        #region Member Variables
        private static readonly string __TableName = "process_entities";
        #endregion

        #region Properties
		
        /// <summary>
        /// ID of Process in process entities relation
        /// </summary>
        public Guid ProcessID
        {
            get{return __DBRow.GetGuid("process_id");}
            set{__DBRow.SetGuid("process_id", value);}
        }

        /// <summary>
        /// Convenience property to load the process specified by the ProcessID property.
        /// </summary>
        public ServerProcess Process
        {
            get
            {
                return ServerProcess.Load(ProcessID);
            }
        }

        /// <summary>
        /// ID of molecular entity in process entities relation
        /// </summary>
        public Guid EntityID
        {
            get{return __DBRow.GetGuid("entity_id");}
            set{__DBRow.SetGuid("entity_id", value);}
        }

        /// <summary>
        /// Convenience property to load the molecule specified by the EntityID property.
        /// </summary>
        public ServerMolecularEntity Entity
        {
            get
            {
                return ServerMolecularEntity.Load(EntityID);
            }
        }

		/// <summary>
		/// Entity role in a process as a string.  Wraps RoleId.
		/// </summary>
		public string Role
		{
			get{return ProcessEntityRoleManager.GetRoleName(RoleId);}
			set{ RoleId = ProcessEntityRoleManager.GetRoleId(value);} // (ac)fails if the string does not exist in the db
		}

        /// <summary>
        /// Get/set process entity role ID.
        /// </summary>
		public int RoleId
		{
			get{return __DBRow.GetInt("role_id");}
			set{__DBRow.SetInt("role_id", value);}
		}

		/// <summary>
		/// The number of times the given molecule appears in this process
		/// </summary>
		public string Quantity
		{
			get{return __DBRow.GetString("quantity");}
			set{__DBRow.SetString("quantity", value);}
		}

		/// <summary>
		/// notes on the relation
		/// </summary>
		public string ProcessEntityNotes
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
		public override SoapObject PrepareForSoap ( SoapObject derived )
		{
			SoapProcessEntity retval = (derived == null) ? 
				retval = new SoapProcessEntity() : retval = (SoapProcessEntity)derived;
			
			retval.ProcessID = this.ProcessID;
			retval.EntityID   = this.EntityID;		
			retval.Role = this.Role;
			retval.Quantity = this.Quantity;
			retval.ProcessEntityNotes = this.ProcessEntityNotes;
            

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
			SoapProcessEntity p = o as SoapProcessEntity;

			this.ProcessID = p.ProcessID;
			this.EntityID   = p.EntityID;		
			this.Role = p.Role;
			this.Quantity = p.Quantity;
			this.ProcessEntityNotes = p.ProcessEntityNotes;
            
		}

		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (BE) rewrote using BuildCommand()

			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + " (process_id, entity_id, role_id, quantity, notes) VALUES (@process_id, @entity_id, @role_id, @quantity, @notes);",
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@entity_id", SqlDbType.UniqueIdentifier, EntityID,
				"@role_id", SqlDbType.TinyInt, RoleId,
				"@quantity", SqlDbType.VarChar, Quantity,
				"@notes", SqlDbType.VarChar, ProcessEntityNotes
                );

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE process_id = @process_id AND entity_id = @entity_id AND role_id = @role_id;",
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@entity_id", SqlDbType.UniqueIdentifier, EntityID,
				"@role_id", SqlDbType.VarChar, RoleId);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + " SET quantity = @quantity, notes = @notes WHERE process_id = @process_id AND entity_id = @entity_id AND role_id = @role_id ; ",
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@entity_id", SqlDbType.UniqueIdentifier, EntityID,
				"@role_id", SqlDbType.TinyInt, RoleId,
				"@quantity", SqlDbType.VarChar, Quantity,
				"@notes", SqlDbType.VarChar, ProcessEntityNotes);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE entity_id = @entity_id AND name_id = @name_id;",
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@entity_id", SqlDbType.UniqueIdentifier, EntityID,
				"@role_id", SqlDbType.TinyInt, RoleId);
		}
		#endregion

		#endregion

		#region Static Methods     

		/// <summary>
		/// Returns the rows in the process_entities table in which the given entity appears
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns>
		/// An array of ServerProcessEntity objects where entity_id is the given id
		/// </returns>
		public static ServerProcessEntity[] GetAllForEntity(Guid entityId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE entity_id = @entity_id;",
				"@entity_id", SqlDbType.UniqueIdentifier, entityId);

			ArrayList results = new ArrayList();
			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
			{
				foreach(DataSet d in ds)
				{
					results.Add(new ServerProcessEntity( new DBRow( d ) ) );
				}
			}
			return (ServerProcessEntity[])results.ToArray(typeof(ServerProcessEntity));
		}


		/// <summary>
		/// Returns all rows in the process_entities table that involve the given process
		/// </summary>
		/// <param name="processId"></param>
		/// <returns>
		/// An array of ServerProcessEntity objects where entity_id is the given id
		/// </returns>
		public static ServerProcessEntity[] GetAllForProcess(Guid processId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE process_id = @process_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processId);

			ArrayList results = new ArrayList();
			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
			{
				foreach(DataSet d in ds)
				{
					results.Add(new ServerProcessEntity( new DBRow( d ) ) );
				}
			}
			return (ServerProcessEntity[])results.ToArray(typeof(ServerProcessEntity));
		}

        /// <summary>
        /// Returns all rows in the process_entities table that involve the given generic process with all specific process combinations
        /// </summary>
        /// <param name="genericProcessId"></param>
        /// <returns>
        /// An array of ServerProcessEntity objects where entity_id is the given id
        /// </returns>
        public static ServerProcessEntity[] GetAllForGenericProcess(Guid genericProcessId)
        {
//            SqlCommand command = DBWrapper.BuildCommand(
//                @"SELECT pe2.*
//					FROM " + __TableName + @" pe2, (
//						SELECT DISTINCT pe.process_id, pe.entity_id, pe.role_id
//							FROM " + __TableName + @" pe, processes p
//							WHERE p.id = pe.process_id
//								AND p.generic_process_id = @genericProcessId ) AS uniquePE
//					WHERE uniquePE.process_id = pe2.process_id AND uniquePE.entity_id = pe2.entity_id
//					AND uniquePE.role_id = pe2.role_id;",
//                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);

//            ArrayList results = new ArrayList();
//            DataSet[] ds;
//            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
//            {
//                foreach(DataSet d in ds)
//                {
//                    results.Add(new ServerProcessEntity( new DBRow( d ) ) );
//                }
//            }
//            return (ServerProcessEntity[])results.ToArray(typeof(ServerProcessEntity));

            return SelectProcessEntities("processes p",
                "p.id = pe.process_id AND p.generic_process_id = @genericProcessId",
                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);
        }

        public static ServerProcessEntity[] GetAllForPathway(Guid pathwayId)
        {
            return SelectProcessEntities("processes p, processes genp, pathway_processes pp",
                "p.[id] = pe.process_id AND p.generic_process_id = genp.generic_process_id AND genp.id = pp.process_id AND pp.pathway_id=@pathwayId",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);
        }

        public static ServerProcessEntity[] SelectProcessEntities(string fromClause, string whereClause, params object[] paramNameTypeValueList)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT pe2.*
					FROM " + __TableName + @" pe2, (
						SELECT DISTINCT pe.process_id, pe.entity_id, pe.role_id
							FROM " + __TableName + @" pe " + (fromClause != null ? ", " + fromClause : "") + @"
							" + (whereClause != null ? " WHERE " + whereClause + " " : "") + @" ) AS uniquePE
					WHERE uniquePE.process_id = pe2.process_id AND uniquePE.entity_id = pe2.entity_id
					AND uniquePE.role_id = pe2.role_id;",
                paramNameTypeValueList);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach(DataSet d in ds)
                {
                    results.Add(new ServerProcessEntity( new DBRow( d ) ) );
                }
            }
            return (ServerProcessEntity[])results.ToArray(typeof(ServerProcessEntity));
        }   

        public static ServerMolecularEntity[] GetProcessEntitiesMoleculesForPathway(Guid pathwayId)
        {
            //return ServerMolecularEntity.SelectMolecularEntities(
            //    "process_entities pe, pathway_processes pp",
            //    "m.id = pe.entity_id AND pe.process_id = pp.process_id AND pp.pathway_id = @pathwayId",
            //    "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);
            return ServerMolecularEntity.SelectMolecularEntities(
                "process_entities pe, processes p, processes genp, pathway_processes pp",
                "m.id = pe.entity_id AND p.[id] = pe.process_id AND p.generic_process_id = genp.generic_process_id AND genp.id = pp.process_id AND pp.pathway_id=@pathwayId",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId).ToArray();
        }

        public static ServerMolecularEntity[] GetProcessEntitiesMoleculesForGenericProcess(Guid genericProcessId)
        {
            return ServerMolecularEntity.SelectMolecularEntities(
                "process_entities pe, processes p",
                "m.id = pe.entity_id AND p.[id] = pe.process_id AND p.generic_process_id = @genericProcessId",
                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId).ToArray();
        }

		/// <summary>
		/// Returns all rows in the process_entities table that involve the given generic process
		/// via a specified role
		/// </summary>
		/// <param name="genericProcessId">The process the entitiy is related to</param>
		/// <param name="role">The role the entity takes</param>
		/// <returns>
		/// An array of ServerProcessEntity objects where entity_id is the given id
		/// </returns>
		/// <remarks>Added mrr 2006-02-21</remarks>
		public static ServerProcessEntity[] GetSpecificRoleForGenericProcess(Guid genericProcessId, string role)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT pe2.*
					FROM " + __TableName + @" pe2, (
						SELECT DISTINCT pe.process_id, pe.entity_id, pe.role_id
							FROM " + __TableName + @" pe, process_entity_roles per, processes p
							WHERE p.id = pe.process_id AND p.generic_process_id = @genericProcessId
								AND pe.role_id = per.role_id AND per.name = @role ) AS uniquePE
					WHERE uniquePE.process_id = pe2.process_id AND uniquePE.entity_id = pe2.entity_id
					AND uniquePE.role_id = pe2.role_id",
				"@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId,
				"@role", SqlDbType.VarChar, role);

			ArrayList results = new ArrayList();
			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
			{
				foreach(DataSet d in ds)
				{
					results.Add(new ServerProcessEntity( new DBRow( d ) ) );
				}
			}
			return (ServerProcessEntity[])results.ToArray(typeof(ServerProcessEntity));
		}

		/// <summary>
		/// Gets all processes that a molecular entity is involved in,
		/// will be used by ServerMolecularEntity object
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns></returns>
		public static ServerProcess[] GetAllProcessesForEntity(Guid entityId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT p.*
					FROM processes p
					INNER JOIN " + __TableName + @" pe ON p.id = pe.process_id
					WHERE pe.entity_id = @entity_id;",
				"@entity_id", SqlDbType.UniqueIdentifier, entityId);

			ArrayList results = new ArrayList();
			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
			{
				foreach(DataSet d in ds)
				{
					results.Add(new ServerProcess( new DBRow( d ) ) );
				}
			}
			return (ServerProcess[])results.ToArray(typeof(ServerProcess));
		}

        /// <summary>
        /// Gets all processes that a molecular entity is involved in a particular tissue
        /// will be used by ServerMolecularEntity object
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public static ServerProcess[] GetAllProcessesForEntity(Guid entityId, int tissueId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT p.*
					FROM processes p
					INNER JOIN " + __TableName + @" pe ON p.id = pe.process_id
					WHERE pe.entity_id = @entity_id AND pe.tissue_id =@tissue_id;",
                "@entity_id", SqlDbType.UniqueIdentifier, entityId,
                "@tissue_id", SqlDbType.TinyInt, tissueId);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach (DataSet d in ds)
                {
                    results.Add(new ServerProcess(new DBRow(d)));
                }
            }
            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }



        /// <summary>
        /// Gets the transport process (if one exists for the entityid-- tissueid combo
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tissueId"></param>
        /// <returns></returns>
        public static ServerProcess[] GetTransportProcessForEntity(Guid entityId, int tissueId)
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT p.*
                    From processes p
                    INNER JOIN " + __TableName + @" pe ON p.id =pe.process_id
                    WHERE pe.entity_id=@entity_id
                    AND pe.tissue_id=@tissue_id
                    AND p.parent_process='00000000-0000-0000-0000-000000000000'
                    AND p.is_transport='True'",
                   "@entity_id", SqlDbType.UniqueIdentifier, entityId,
                   "@tissue_id", SqlDbType.TinyInt, tissueId);

            List<ServerProcess> results = new List<ServerProcess>();
            DataSet[] datasets;
            int i = DBWrapper.LoadMultiple(out datasets, ref command);
            if (i > 0)
            {
                foreach (DataSet ds in datasets)
                {
                    DBRow row = new DBRow(ds);
                    //string name = row.GetString("name");
                    //Guid id = row.GetGuid("id");
                    //Guid parent_processId = row.GetGuid("parent_process"); 
                    //Trace.WriteLine(id +"  " + name+" "+parent_processId); 
                    results.Add(new ServerProcess(row));
                }
            }
            return (ServerProcess[])results.ToArray();
        }


		// (bse)
		// GetAllEntitiesInProcess has moved to ServerProcessEntity due to access restrictions on function calls
		//

		/// <summary>
		/// Inserts an entry into the 'process_entities' table for the 
		/// given molecular entity and process.
		/// </summary>
		/// <remarks>
		/// TODO: add implementation, not yet implemented
		/// </remarks>
		/// <param name="processId"></param>
		/// <param name="entityId"></param>
		/// <param name="role"></param>
		/// <param name="quantity"></param>
		/// <param name="notes"></param>
		public static void AddMolecularEntityToProcess( Guid processId, Guid entityId, string role, int quantity, string notes )
		{
			// TODO: add logic for inserting into this relation
			if ( !Exists(processId, entityId, role) )
			{
				DBWrapper.Instance.ExecuteNonQuery(				
					"INSERT INTO " + __TableName + " ( process_id, entity_id, role_id, quantity, notes ) VALUES ( @i_process_id, @i_entity_id, @i_role_id, @i_quantity, @i_notes);",
					"@i_process_id", SqlDbType.UniqueIdentifier, processId,
					"@i_entity_id", SqlDbType.UniqueIdentifier, entityId,
					"@i_role_id", SqlDbType.TinyInt, ProcessEntityRoleManager.GetRoleId(role),
					"@i_quantity", SqlDbType.VarChar, quantity,
					"@i_notes", SqlDbType.Text, notes);
			}
			else 
			{
				//do nothing, the relation already exists
			}
		}

		/// <summary>
		/// Removes the given molecular entity and process from the process_molecular_entities table.
		/// </summary>
		/// <param name="processId"></param>
		/// <param name="entityId"></param>
		/// <param name="role"></param>
		public static void RemoveMolecularEntityFromProcess ( Guid processId, Guid entityId, string role )
		{
			//TODO
			if (DBWrapper.Instance.ExecuteNonQuery(				
				"DELETE FROM " + __TableName + " WHERE process_id = @i_process_id AND entity_id = @i_entity_id AND role_id = @i_role_id;",
				"@i_process_id", SqlDbType.UniqueIdentifier, processId,
				"@i_entity_id", SqlDbType.UniqueIdentifier, entityId,
				"@i_role_id", SqlDbType.TinyInt, ProcessEntityRoleManager.GetRoleId(role)) < 1)
			{
				throw new DataModelException("Remove molecular entity {0} with role {1} from process {2} failed!", entityId, processId, role);  
			}
		}

		/// <summary>
		/// Calls LoadRow
		/// </summary>
		/// <param name="processID"></param>
		/// <param name="entityID"></param>
		/// <param name="role"></param>
		/// <returns></returns>
		public static ServerProcessEntity Load ( Guid processID, Guid entityID, string role )
		{
			return new ServerProcessEntity ( LoadRow(processID, entityID, role) );
		}

		/// <summary>
		/// Tells whether an entry already exists in the process_entities table with
		/// the given process_id, entity_id, and role
		/// </summary>
		/// <param name="processID"></param>
		/// <param name="entityID"></param>
		/// <param name="role"></param>
		/// <returns>
		/// True if the tuple exists in the process_entities table, false if it doesn't
		/// </returns>
		public static bool Exists ( Guid processID, Guid entityID, string role )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
					FROM " + __TableName + @"
					WHERE process_id = @process_id AND entity_id = @entity_id AND role_id = @role_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processID,
				"@entity_id", SqlDbType.UniqueIdentifier, entityID,
				"@role_id", SqlDbType.TinyInt, ProcessEntityRoleManager.GetRoleId(role));

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;

		}

		/// <summary>
		/// Loads a row from the process_entities table
		/// </summary>
		/// <param name="processID"></param>
		/// <param name="entityID"></param>
		/// <param name="role"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( Guid processID, Guid entityID, string role )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
					FROM " + __TableName + @"
					WHERE process_id = @process_id AND entity_id = @entity_id AND role_id = @role_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processID,
				"@entity_id", SqlDbType.UniqueIdentifier, entityID,
				"@role_id", SqlDbType.TinyInt, ProcessEntityRoleManager.GetRoleId(role));

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		#region Additional Queries
		/// <summary>
		/// given a molecular entity and a process, gets all the entries in the process_entities table.
		/// this is used for getting the role and amount of the molecular entity.
		/// </summary>
		/// <param name="processId"></param>
		/// <param name="entityId"></param>
		/// <returns></returns>
		public static ServerProcessEntity[] GetAllForMolecularEntityAndProcess ( Guid processId, Guid entityId )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE entity_id = @entity_id AND process_id = @process_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processId,
				"@entity_id", SqlDbType.UniqueIdentifier, entityId);

			ArrayList results = new ArrayList();
			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
			{
				foreach(DataSet d in ds)
				{
					results.Add(new ServerProcessEntity( new DBRow( d ) ) );
				}
			}
			return (ServerProcessEntity[])results.ToArray(typeof(ServerProcessEntity));
		}

		#endregion

		#endregion
	}
}


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerProcessEntity.cs,v 1.3 2009/04/07 14:44:13 ali Exp $
	$Log: ServerProcessEntity.cs,v $
	Revision 1.3  2009/04/07 14:44:13  ali
	*** empty log message ***
	
	Revision 1.2  2009/04/03 21:16:03  xjqi
	Visualization Part: Given a model id, visualize compartments in hierarchy with species,reactions.
	Not integrating with web interface.
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.7  2008/05/09 18:33:25  divya
	*** empty log message ***
	
	Revision 1.6  2008/05/01 02:20:24  divya
	*** empty log message ***
	
	Revision 1.5  2008/04/30 23:17:04  divya
	*** empty log message ***
	
	Revision 1.4  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.3  2006/10/03 22:35:14  gokhan
	*** empty log message ***
	
	Revision 1.2  2006/10/03 17:47:44  brendan
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.17  2006/06/12 15:23:20  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.16  2006/06/09 02:24:08  greg
	These changes address a significant number of GO pathway viewer bugs, and also introduce substantial changes to a lot of pages.  I'm refactoring a lot of the HTML to not only move the site closer to XHTML compliance (so we can once again use those buttons that are commented out on the main page!), but also to make the site display properly in non-IE browsers as well, because currently it doesn't look right in non-IE browsers (and hopefully soon everyone will be using Firefox!!).
	
	Revision 1.15  2006/06/07 19:49:14  greg
	These updates more or less cover some additional issues with the ServerObjects as well as a bug where the GO would crash if it tried to compute a hypergeometric distribution using invalid parameters (now it just gives a subtle error message and continues on).
	
	Revision 1.14  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.13  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	
	Revision 1.12  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.11  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.10.6.1  2006/02/21 22:14:44  marc
	Added GetSpecificRoleForGenericProcess method
	
	Revision 1.10  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.9  2005/11/02 20:35:16  fatih
	Test functions are working for pathwayslib objects
	
	Revision 1.8  2005/10/31 20:27:45  fatih
	*** empty log message ***
	
	Revision 1.7  2005/07/15 21:02:00  brandon
	added more queries
	
	Revision 1.6  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.5  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.4  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	Revision 1.3  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
	Revision 1.2  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.1  2005/07/11 16:54:39  brandon
	Added ServerProcessEntity and Soap...  for the process_entities relation.  Added funtion GetAllProcesses in ServerMolecularEntity, but GetAllEntities won't work, maybe because the ServerMolecularEntity constructor is protected.  Haven't done any testing yet.
	
    
	
------------------------------------------------------------------------*/
#endregion