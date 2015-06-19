#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using System.Collections.Generic;
using PathwaysLib.Utilities;
using PathwaysLib.GraphObjects;
#endregion

namespace PathwaysLib.ServerObjects
{
    #region Document Comments
    /// <sourcefile>
    ///		<project>Pathways</project>
    ///		<filepath>PathwaysLib/ServerObjects/ServerGenericProcess.cs</filepath>
    ///		<creation>2005/10/19</creation>
    ///		<author>
    ///				<name>Brendan Elliott</name>
    ///				<initials>BE</initials>
    ///				<email>bxe7@cwru.edu</email>
    ///		</author>
    ///		<contributors>
    ///			<contributor>
    ///				<name>none</name>
    ///				<initials>none</initials>
    ///				<email>none</email>
    ///			</contributor>
    ///		</contributors>
    ///		<cvs>
    ///			<cvs_author>$Author: ali $</cvs_author>
    ///			<cvs_date>$Date: 2009/05/27 14:35:49 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerGenericProcess.cs,v 1.4 2009/05/27 14:35:49 ali Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.4 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Provides a virtual view of a Generic Process (which in the current model
    /// corresponds to a set of Processes sharing a common GenericProcess GUID).
    /// This class exposes operations similar to ServerProcess, but are
    /// implemented by doing a union of the corresponding operations in its
    /// corresponding ServerProcesses.  In other words, this class represents
    /// the UNION of a set of Processes with are essentially the same
    /// process in multiple organisms, originally created for graph drawing.
    /// </summary>
    #endregion	
    public class ServerGenericProcess : IProcess, IGraphGenericProcess
	{
        #region Constructor, Destructor, ToString

		private ServerGenericProcess(ServerProcess[] processes)
		{            
            if (processes.Length > 0)
            {
                this.firstProcess = processes[0];
                Guid genericProcessId = firstProcess.GenericProcessID;

                foreach(ServerProcess p in processes)
                {
                    if (p.GenericProcessID != genericProcessId)
                        throw new DataModelException("GenericProcess wrapper cannot be constructed from list of Processes with different generic process ID's!"); 

                    this.processes.Add(p.ID, p);
                }
            }
		}

        #endregion

        #region Member Variables

        // real members
        ServerProcess firstProcess = null; // for properties that shouldn't matter which one it comes from
        Hashtable processes = new Hashtable();

        // cached members

        #endregion

        #region Properties

        /// <summary>
        /// Get the Process name.
        /// </summary>
        public string Name
        {
            get
            {
                //ASSUME: this is the same for all processes
                return firstProcess.Name;
            }
        }

        /// <summary>
        /// Gets the Process reversibility.
        /// </summary>
        public Tribool Reversible
        {
            get
            {
                //ASSUME: this is the same for all processes
                return firstProcess.Reversible;
            }
        }

        /// <summary>
        /// Get the GenericProcess ID.
        /// </summary>
        public Guid ID
        {
            get
            {
                return GenericProcessID;
            }
        }

        /// <summary>
        /// Get the generic process id.
        /// </summary>
        public Guid GenericProcessID
        {
            get
            {
                //ASSUME: this is the same for all processes
                return firstProcess.GenericProcessID;
            }
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        public bool IsGenericProcess
        {
            get {return true;}
        }

        public bool IsTransport
        {
            get { return firstProcess.IsTransport; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the EC Number(s) for these processes.
        /// </summary>
        /// <returns></returns>
        public ServerECNumber[] GetECNumbers()
        {
            return ServerCatalyze.GetECNumbersForGenericProcess(this.GenericProcessID);
        }

        /// <summary>
        /// Get all entries in the process_entities (catalyze) relation for this generic process
        /// </summary>
        /// <returns>
        /// an array of ServerProcessEntity objects
        /// </returns>
        public ServerProcessEntity[] GetAllProcessEntities()
        {
            return ServerProcessEntity.GetAllForGenericProcess(this.GenericProcessID);
        }

		/// <summary>
		/// Get all entries in the process_entities (catalyze) relation for this generic process
		/// which take a specific role
		/// </summary>
		/// <param name="role">The specific role of the process entity</param>
		/// <returns>
		/// an array of ServerProcessEntity objects
		/// </returns>
		public ServerProcessEntity[] GetSpecificProcessEntities(string role)
		{
			return ServerProcessEntity.GetSpecificRoleForGenericProcess(this.GenericProcessID, role);
		}

        public ServerMolecularEntity[] GetAllMolecularEntities
        {
            get
            {
                return ServerMolecularEntity.GetAllEntitiesInGenericProcess(this.GenericProcessID);
            }
        }

        /// <summary>
        /// Gets the list of specific processes that this generic process represents.
        /// </summary>
        /// <returns></returns>
        public ServerProcess[] GetAllProcesses()
        {
            ArrayList results = new ArrayList(processes.Values);
            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

        /// <summary>
        /// Returns all the organisms or organism groups for this generic process
        /// </summary>
        /// <returns></returns>
        public ServerOrganismGroup[] GetAllOrganisms( )
        {
            return ServerProcess.GetAllOrganismsForGenericProcess( this.ID );
        }

        /// <summary>
        /// Returns all the organisms or organism groups for this generic process
        /// </summary>
        /// <returns></returns>
        public ServerOrganismGroup[] GetAllOrganismGroups( )
        {
            return ServerProcess.GetAllOrganismGroupsForGenericProcess( this.ID );
        }

		/*  // Organism groups no longer associated w/ processes (because they're jerks like that)
        /// <summary>
        /// Returns all organism groups that this generic process takes place in
        /// </summary>
        /// <returns></returns>
        public ServerOrganismGroup[] GetAllOrganismGroups()
        {
            return ServerOrganismGroup.GetAllOrganismGroupsForGenericProcess(this.GenericProcessID);
        }
		*/

        /// <summary>
        /// Get all gene products
        /// </summary>
        /// <returns>
        /// Returns all of the gene products involved in the generic process
        /// </returns>
        public ServerGeneProduct[] GetAllGeneProducts()
        {
            return ServerCatalyze.GetAllGeneProductsForGenericProcess(this.GenericProcessID);
        }

		/// <summary>
		/// Returns true if the IDs of the objects match.  Send to the base
		/// Equals method if obj is not a ServerGenericProcess object
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>True if the objects are equal</returns>
		public override bool Equals(object obj)
		{
			ServerGenericProcess gen = obj as ServerGenericProcess;
			if(gen == null) return base.Equals(obj);
			return this.ID == gen.ID;
		}

		/// <summary>
		/// Returns the hashcode for this generic process.
		/// </summary>
		/// <returns>An integer hashcode</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        /// <summary>
        /// Return all reactions for this process.
        /// </summary>
        /// <returns></returns>
        public ServerReaction[] GetAllReactions()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM reaction RR
				WHERE [id] IN ( SELECT MP.reactionId
									FROM MapReactionsProcessEntities MP, Processes P
									WHERE MP.processId = P.id
                                    AND P.generic_process_id = @id)
				ORDER BY (RR.name + RR.sbmlId);",
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
                @"SELECT MP.reactionId, MP.qualifierId
				FROM MapReactionsProcessEntities MP, Processes P
									WHERE MP.processId = P.id
                                    AND P.generic_process_id = @id;",
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

        #endregion

        #region Static Methods

        /// <summary>
        /// From a pathway, convert a set of specific processes into a set of generic
        /// processes by forming the union of all processes that share a generic ID in the pathway.
        /// </summary>
        /// <param name="pathway"></param>
        /// <returns></returns>
        public static ServerGenericProcess[] LoadFromPathway(ServerPathway pathway)
        {
            return LoadFromProcessSet(pathway.GetAllProcesses());
        }

        /// <summary>
        /// From a set of processes, create a generic process as the union of those processes (which must all share
        /// the same generic process ID.
        /// </summary>
        /// <param name="specificProcesses"></param>
        /// <returns></returns>
        public static ServerGenericProcess LoadGenericProcess(ServerProcess[] specificProcesses)
        {
            return new ServerGenericProcess(specificProcesses);
        }

        /// <summary>
        /// Load a generic process by ID, forming a union of ALL specific processes in all pathways
        /// that share the same generic process ID.
        /// </summary>
        /// <param name="genericProcessId"></param>
        /// <returns></returns>
        public static ServerGenericProcess Load(Guid genericProcessId)
        {
            return LoadGenericProcess(ServerProcess.GetProcessesByGenericProcessId(genericProcessId));
        }

        /// <summary>
        /// Returns all generic process IDs found in the database.
        /// </summary>
        /// <returns></returns>
        public static Guid[] AllGenericProcessesIds()
        {
            ArrayList guids = new ArrayList();
            SqlCommand command = new SqlCommand("SELECT DISTINCT generic_process_id FROM processes p");
            DataSet ds;

            if (DBWrapper.Instance.ExecuteQuery(out ds, ref command) > 0)
            {
                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    guids.Add((Guid)r["generic_process_id"]);
                }
            }

            return (Guid[])guids.ToArray(typeof(Guid));
        }

        /// <summary>
        /// Convert a set of specific processes into a set of generic
        /// processes by forming the union of all processes that share a generic ID in the set.
        /// </summary>
        /// <param name="processes"></param>
        /// <returns></returns>
        public static ServerGenericProcess[] LoadFromProcessSet(ServerProcess[] processes)
        {
            if (processes.Length < 1)
                return new ServerGenericProcess[0];

            ArrayList results = new ArrayList();
            Hashtable genericProcesses = new Hashtable();

            // make separate lists by generic process ID
            foreach(ServerProcess p in processes)
            {
                if (genericProcesses.ContainsKey(p.GenericProcessID))
                {
                    ArrayList list = (ArrayList)genericProcesses[p.GenericProcessID];
                    list.Add(p);
                }
                else
                {
                    ArrayList list = new ArrayList();
                    list.Add(p);

                    genericProcesses.Add(p.GenericProcessID, list);
                }
            }

            if (genericProcesses.Count < 1)
                return new ServerGenericProcess[0];

            // create objects from each set of generic processes
            foreach(ArrayList list in genericProcesses.Values)
            {
                results.Add(LoadGenericProcess((ServerProcess[])list.ToArray(typeof(ServerProcess))));
            }

            return (ServerGenericProcess[])results.ToArray(typeof(ServerGenericProcess));
        }

        public static Dictionary<Guid, Guid> GetProcessMappingTableInPathway(Guid pathwayId)
        {
            return SelectSpecificToGenericProcessIdTable("processes genp, pathway_processes pp",
                "p.generic_process_id = genp.generic_process_id AND genp.id = pp.process_id AND pp.pathway_id = @pathwayId",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);
        }

        public static Dictionary<Guid, Guid> GetProcessMappingTableForGenericProcess(Guid genericProcessId)
        {
            return SelectSpecificToGenericProcessIdTable(
                null,
                "p.generic_process_id = @genericProcessId",
                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);
        }

        public static Dictionary<Guid, Guid> SelectSpecificToGenericProcessIdTable(string fromClause, string whereClause, params object[] paramNameTypeValueList)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.id, p.generic_process_id 
					FROM processes p " + (fromClause != null ? ", " + fromClause : "") + @"
							" + (whereClause != null ? " WHERE " + whereClause + " " : ""),
                paramNameTypeValueList);

            Dictionary<Guid, Guid> results = new Dictionary<Guid, Guid>();
            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                results.Add((Guid)r["id"], (Guid)r["generic_process_id"]);
            }

            return results;
        }

        #endregion

        public static bool Exists(Guid id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT generic_process_id FROM processes p WHERE generic_process_id = @id;",
                "@id", SqlDbType.UniqueIdentifier, id);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }
    }
}



#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerGenericProcess.cs,v 1.4 2009/05/27 14:35:49 ali Exp $
	$Log: ServerGenericProcess.cs,v $
	Revision 1.4  2009/05/27 14:35:49  ali
	*** empty log message ***
	
	Revision 1.3  2009/05/19 13:22:25  ali
	*** empty log message ***
	
	Revision 1.2  2009/04/03 21:16:03  xjqi
	Visualization Part: Given a model id, visualize compartments in hierarchy with species,reactions.
	Not integrating with web interface.
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.6  2007/02/07 00:02:17  brendan
	*** empty log message ***
	
	Revision 1.5  2006/11/26 21:49:25  ali
	*** empty log message ***
	
	Revision 1.4  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.3  2006/10/03 22:35:14  gokhan
	*** empty log message ***
	
	Revision 1.2  2006/10/03 17:47:44  brendan
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.8  2006/07/21 14:26:36  greg
	In this update:
	
	 - File cleanout: there were a lot of extraneous files hanging around, so I went through and checked to see which ones were never used and removed them.  Less clutter = less headache.
	
	 - Help updates: more help information was added, and existing help pages were linked up properly.  The applet now has its own special help page with a full description of all its features.
	
	 - Page cleanup: just about every page was touched to address residual layout issues, clean up a substantial amount of HTML and convert it into XHTML, and also fix bugs that were present on the current Nashua release build that were as of yet unknown.
	
	Revision 1.7  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.6  2006/05/12 21:25:46  ali
	For processes no organisms were listed. This was due to the fact that all the functions returning organism lists contained queries that join organism and organism group tables which can technically work only for mouse, and human or real organisms. Now all such methods operate on organism group table as it contains all organisms and organism groups.
	
	Revision 1.5  2006/05/11 21:18:33  brendan
	Fixed numerous bugs, basic browsing starting to work again
	
	Revision 1.4  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	

	Revision 1.3  2006/04/12 20:22:32  brian
	*** empty log message ***
	
	Revision 1.2.8.3  2006/03/22 21:25:21  brian
	Removed and renamed a few more functions for consistency
	
	Revision 1.2.8.2  2006/03/22 20:24:16  brian
	I forgot to list my changes over the last few commits, so here's a quick overview:
	1. IOrganismEntity interface removed
	2. All ServerObject queries that return IOrganismEntity now return either ServerOrganism or ServerOrganismGroup
	3. All OrganismGroup-Process relations have been commented out (pending removal)
	4. Several new functions have been added to ServerOrganism and ServerOrganismGroup too simplify the initialization of objects
	
	Revision 1.2.8.1  2006/03/22 19:48:06  brian
	*** empty log message ***
	
	Revision 1.2.6.2  2006/03/02 04:31:50  marc
	Added an Equals method so that if Generic Processes have the same ID, they are considered equal
	
	Revision 1.2.6.1  2006/02/21 22:15:08  marc
	Added GetSpecificProcessEntities method
	
	Revision 1.2  2006/02/07 23:22:26  brendan
	Added drawing support for generic co-factors.
	
	Added graph caching support.  Will require a new value in the .config file.
	
	Revision 1.1  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
------------------------------------------------------------------------*/
#endregion