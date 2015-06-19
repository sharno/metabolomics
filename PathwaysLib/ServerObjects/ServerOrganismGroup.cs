#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using System.Collections.Generic;
using PathwaysLib.GraphObjects;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.ServerObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerOrganismGroup.cs</filepath>
	///		<creation>2005/07/01</creation>
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
	///			<contributor>
	///				<name>Gokhan Yavas</name>
	///				<initials>gy</initials>
	///				<email>gokhan.yavas@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: murat $</cvs_author>
	///			<cvs_date>$Date: 2010/11/19 21:13:29 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerOrganismGroup.cs,v 1.11 2010/11/19 21:13:29 murat Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.11 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to organism groups.
	/// </summary>
	#endregion
    public class ServerOrganismGroup : ServerObject, IGraphOrganismGroup
	{

		#region Constructor, Destructor, ToString
		internal ServerOrganismGroup ( )
		{
		}

		/// <summary>
		/// Constructor for server organism group wrapper with fields initiallized
		/// </summary>
		/// <param name="scientific_name"></param>
		/// <param name="parentId"></param>
		/// <param name="notes"></param>
		public ServerOrganismGroup ( string scientific_name, Guid parentId, string notes )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ID             = DBWrapper.NewID(); // generate a new ID
			this.ScientificName = scientific_name;
			this.CommonName     = null;
			this.ParentID       = parentId;
			this.Notes          = notes;
			this.IsOrganism     = false; // I AM NOT AN ANIMAL!!

			if(!this.IsOrganism)
				SetSqlCommandParameters();
		}

		/// <summary>
		/// Same as the above, but also including a common name
		/// </summary>
		/// <param name="scientific_name"></param>
		/// <param name="common_name"></param>
		/// <param name="parentID"></param>
		/// <param name="notes"></param>
		public ServerOrganismGroup(string scientific_name, string common_name, Guid parentID, string notes)
		{
			__DBRow             = new DBRow( __TableName );

			this.ID             = DBWrapper.NewID();
			this.ScientificName = scientific_name;
			this.CommonName     = common_name;
			this.ParentID       = parentID;
			this.Notes          = notes;
			this.IsOrganism     = false; // I AM NOT AN ANIMAL!!!

			if(!this.IsOrganism)
				SetSqlCommandParameters();
		}


		/// <summary>
		/// Constructor for server organism group wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerOrganismGroup object from a
		/// SoapOrganismGroup object.
		/// </remarks>
		/// <param name="data">
		/// A SoapOrganismGroup object from which to construct the
		/// ServerOrganismGroup object.
		/// </param>
		public ServerOrganismGroup ( SoapOrganismGroup data )
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
			if (!(data is SoapOrganism))
			{
				if (data.Status != ObjectStatus.ReadOnly)
					UpdateFromSoap(data);
	
				SetSqlCommandParameters();
				
			}
			//if(!this.IsOrganism)
				
		}

		/// <summary>
		/// Constructor for server organism group wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerOrganismGroup object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerOrganismGroup ( DBRow data )
		{
			__DBRow = data;

			if(!this.IsOrganism)
				this.SetSqlCommandParameters();
		}

		/// <summary>
		/// Destructor for the ServerOrganismGroup class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerOrganismGroup()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "organism_groups";
		#endregion


		#region Properties

		/// <summary>
		/// Get/set the organism group ID.
		/// </summary>
		virtual public Guid ID
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
		/// Formats the name depending upon whether the scientific name, common name, or both are available
		/// </summary>
		public string Name
		{
			get
			{
				if(this.ScientificName == null || this.ScientificName.Trim().Length == 0 || this.ScientificName.ToLower() == "not known")
					return this.CommonName;
				else if(this.CommonName == null || this.CommonName.Trim().Length == 0 || this.CommonName.ToLower() == "not known")
					return this.ScientificName;
				else
					return (this.ScientificName + " (" + this.CommonName + ")"   );
			}
		}


		/// <summary>
		/// Get/set the organism group scientific name.
		/// </summary>
		public string ScientificName
		{
			get
			{
				return __DBRow.GetString("scientific_name");
			}
			set
			{
				__DBRow.SetString("scientific_name", value);
			}
		}


		/// <summary>
		/// Get/Set the common name for the organism group
		/// </summary>
		public string CommonName
		{
			get
			{
				return __DBRow.GetString("common_name");
			}
			set
			{
				__DBRow.SetString("common_name", value);
			}
		}



		/// <summary>
		/// Get/set the organism_group parent ID.
		/// </summary>
		public Guid ParentID
		{
			get
			{
				return __DBRow.GetGuid("parent_id");
			}
			set
			{
                if (value != ParentID)
                {
                    __DBRow.SetGuid("parent_id", value);

                    // update labeling
                    NodeLabel = NextChildNodeLabel(value);
                }
			}
		}

		/// <summary>
		/// Get/set the organism group notes.
		/// </summary>
		public string Notes
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
		/// Get/Set whether or not this is an organism
		/// </summary>
		protected bool IsOrganism
		{
			get{return __DBRow.GetBool("is_organism");}
			set
			{
				__DBRow.SetBool("is_organism", value); // Set to true only if the object is really an organism!!
			}
		}

        /// <summary>
        /// Node label for quickly determining ancestor/descendant organism groups.
        /// </summary>
        protected string NodeLabel
        {
            get { return __DBRow.GetString("nodeLabel"); }
            set
            {
                __DBRow.SetString("nodeLabel", value); 
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
			SoapOrganismGroup retval = (derived == null) ? new SoapOrganismGroup() : (SoapOrganismGroup)derived;

			retval.ID                 = this.ID;
			retval.ScientificName     = this.ScientificName;
			retval.CommonName         = this.CommonName;
			retval.ParentID           = this.ParentID;
			retval.Notes              = this.Notes;

			retval.Status             = ObjectStatus.NoChanges;

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
			SoapOrganismGroup og = o as SoapOrganismGroup;

            if (o.Status == ObjectStatus.Insert)
			{
                if (og.ID == Guid.Empty)
				    og.ID           = DBWrapper.NewID(); // generate a new ID
				this.IsOrganism = false;             // I AM NOT AN ANIMAL!!!!
			}

			this.ID             = og.ID;
			this.ScientificName = og.ScientificName;
			this.CommonName     = og.CommonName;
			this.ParentID       = og.ParentID;
			this.Notes          = og.Notes;
		}



		/// <summary>
		/// Finds all pathways assocated w/ this organism group (and everything under it)
		/// </summary>
		virtual public ServerPathway[] GetPathways()
		{
			// Double check this query.  Should it always act recursively?
			string RequiredIds = ServerOrganismGroup.RequireIdInList(this.GetAllChildrenAndParent(), "c", "organism_group_id");
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT DISTINCT p.id, p.name, p.pathway_type_id, p.status 
					FROM pathways p, pathway_processes pp, catalyzes c
					WHERE p.id = pp.pathway_id
						AND pp.process_id = c.process_id
						AND " + RequiredIds + @"
					ORDER BY p.name;");

			//SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM pathways p WHERE p.id IN (SELECT po.id FROM v_pathway_organisms po WHERE " + RequiredIds + ") ORDER BY p.name;");
			return ServerPathway.LoadMultiple(command);
		}


		/// <summary>
		/// Find all molecular entities associated with this organism
		/// </summary>
		/// <returns>A list of ServerMolecularEntity objects containing molecules, rnas, ...</returns>
		virtual public ServerMolecularEntity[] GetMolecularEntities()
		{
			string RequiredIds = ServerOrganismGroup.RequireIdInList(this.GetAllChildrenAndParent(), "c", "organism_group_id");

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT me.*
					FROM molecular_entities me
					WHERE me.id IN ( SELECT pe.entity_id
										FROM catalyzes c, process_entities pe, molecular_entities me
										WHERE " + RequiredIds + @" AND c.process_id = pe.process_id)
					ORDER BY me.name");

//"SELECT me.* FROM molecular_entities me"
//+   " WHERE me.id IN (SELECT pe.entity_id"
//+   "  FROM process_organisms_xv po, v_process_entities pe, molecular_entities me"
//+   " WHERE " + RequiredIds + " "
//+   "   AND po.process_id = pe.process_id)"
//+   " ORDER BY me.name");

			return ServerMolecularEntity.LoadMultiple(command);
		}

        /// <summary>
        /// Return all models for this organism.
        /// </summary>
        /// <returns></returns>
        public ServerModel[] GetAllModels()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM model
				WHERE [id] IN ( SELECT modelId
									FROM ModelOrganism
									WHERE organismGroupId = @id )
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
        /// Return all model annotations for this organism.
        /// </summary>
        /// <returns></returns>
        
        public Dictionary<Guid, BiomodelAnnotation> GetAllModelAnnotations()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT modelId, qualifierId
				FROM ModelOrganism
				WHERE organismGroupId = @id;",
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

		#region Organism Hierarchy


		/// <summary>
		/// Get all organisms that are immediately (ie, 1 level) under this group
		/// </summary>
		public ServerOrganism[] GetImmediateChildOrganisms ( )
		{
			return ServerOrganismGroup.GetImmediateChildOrganisms(this.ID);
		}


		/// <summary>
		/// Get all groups immediately under this group
		/// </summary>
		public ServerOrganismGroup[] GetImmediateChildGroups ( )
		{
			return ServerOrganismGroup.GetImmediateChildGroups( this.ID );
		}


		/// <summary>
		/// Find all of the ServerOrganism's and ServerOrganismGroups 1 level under this group
		/// </summary>
		/// <returns>An array of ServerOrganismGroups (Note that some of these may actually be ServerOrganism's)</returns>
		public ServerOrganismGroup[] GetImmediateChildren()
		{
			return ServerOrganismGroup.GetImmediateChildren(this.ID);
		}

        /// <summary>
        /// Find all of the ServerOrganism's and ServerOrganismGroups 1 level under this group with at least one model
        /// </summary>
        /// <returns>An array of ServerOrganismGroups (Note that some of these may actually be ServerOrganism's)</returns>
        public ServerOrganismGroup[] GetImmediateChildrenWithModel()
        {
            return ServerOrganismGroup.GetImmediateChildrenWithModel(this.ID);
        }


		/// <summary>
		/// Find all groups under the current group (if a group has an organism part, the group is not returned)
		/// </summary>
		/// <returns>An array of ServerOrganismGroup's containing only groups</returns>
		public ServerOrganismGroup[] GetAllChildGroups()
		{
			return ServerOrganismGroup.GetAllChildGroups(this.ID);
		}

		/// <summary>
		/// Find all organisms under the current group
		/// </summary>
		/// <returns>An array of ServerOrganism's</returns>
		public ServerOrganism[] GetAllChildOrganisms()
		{
			return ServerOrganismGroup.GetAllChildOrganisms(this.ID);
		}

		/// <summary>
		/// Trace down the group hierarchy to return all organisms and groups under this one
		/// </summary>
		/// <returns>An array of ServerOrganismGroup's containing groups and organisms</returns>
		public ServerOrganismGroup[] GetAllChildren()
		{
			return ServerOrganismGroup.GetAllChildren(this.ID);
		}



		/// <summary>
		/// Grab all organisms and groups under current group, plus the current group
		/// </summary>
		/// <returns></returns>
		public ServerOrganismGroup[] GetAllChildrenAndParent()
		{
			ArrayList results = new ArrayList();
			results.Add(this);
			ServerOrganismGroup.GetAllChildren(this.ID, ref results);
			return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
		}


		


		#endregion

        #region Organism Hierarchy Labeling

        protected static string NextChildNodeLabel(Guid serverOrganismGroupId)
        {
            ServerOrganismGroup parent = null;
            if (serverOrganismGroupId != Guid.Empty && ServerOrganismGroup.Exists(serverOrganismGroupId))
            {
                parent = ServerOrganismGroup.Load(serverOrganismGroupId);
                return NextChildNodeLabel(parent.NodeLabel, ChildNodeLabels(parent));
            }
            return "";
        }

        protected static string NextChildNodeLabel(ServerOrganismGroup parent)
        {
            return NextChildNodeLabel(parent.NodeLabel, ChildNodeLabels(parent));
        }

        protected static string NextChildNodeLabel(string parentLabel, ArrayList childLabels)
        {
            if (childLabels.Count < 1)
                return parentLabel + "1."; // first child

            int maxChildLabel = 0;

            string l;
            foreach (string label in childLabels)
            {
                try
                {
                    l = label.Remove(label.Length - 1, 1);

                    int n = 0;
                    if (l.LastIndexOf('.') > -1)
                        n = int.Parse(l.Substring(l.LastIndexOf('.') + 1)); 
                    else
                        n = int.Parse(l); // root child label (i.e. first label)
                    if (n > maxChildLabel)
                        maxChildLabel = n;
                }
                catch
                {
                    //invalid label!
                }
            }

            maxChildLabel++;
            return parentLabel + maxChildLabel.ToString() + ".";
        }

        protected static ArrayList ChildNodeLabels(ServerOrganismGroup parent)
        {
            ArrayList results = new ArrayList();

            DBWrapper db = DBWrapper.Instance;
            DataSet ds = new DataSet();

            if (parent != null)
            {
                // get child labels of this parent
                db.ExecuteQuery(out ds, "SELECT nodeLabel FROM organism_groups i WHERE i.parent_id = @parentId AND nodeLabel IS NOT NULL",
                                            "@parentId", SqlDbType.UniqueIdentifier, parent.ID);
            }
            else
            {
                // get root labels
                db.ExecuteQuery(out ds, "SELECT nodeLabel FROM organism_groups i WHERE i.parent_id IS NULL");
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (!(r["nodeLabel"] is DBNull))
                        results.Add(((string)r["nodeLabel"]).Trim());
                }
            }
            return results;
        }

        public static int RelabelAllOrganismGroups()
        {
            return RelabelChildOrganismGroups(null, false);
        }

        public static int RelabelAllOrganismGroups(bool fromScratch)
        {
            return RelabelChildOrganismGroups(null, fromScratch);
        }

        public static int RelabelChildOrganismGroups(ServerOrganismGroup subtreeRoot, bool fromScratch)
        {
            int count = 0;

            // label this node
            if (subtreeRoot != null && subtreeRoot.NodeLabel == "")
            {
                throw new DataModelException("Please label subtree root first. Use RelabelAllOrganismGroups() to relabel from scratch.");
            }

            // label children
            ArrayList childLabels;
            if (fromScratch)
            {
                childLabels = new ArrayList();
            }
            else
            {
                childLabels = ChildNodeLabels(subtreeRoot);
            }

            ServerOrganismGroup[] children;
            if (subtreeRoot != null)
            {
                children = subtreeRoot.GetImmediateChildren(); //subtreeRoot.GetAllChildren();
            }
            else
            {
                // root
                children = ServerOrganismGroup.GetRoots();
            }

            if (children.Length < 1)
                return count;

            foreach (ServerOrganismGroup c in children)
            {
                // fix label of this child
                if (subtreeRoot != null)
                    c.NodeLabel = ServerOrganismGroup.NextChildNodeLabel(subtreeRoot.NodeLabel, childLabels);
                else
                    c.NodeLabel = ServerOrganismGroup.NextChildNodeLabel("", childLabels); // new child of the root
                c.UpdateDatabase();

                childLabels.Add(c.NodeLabel);
                count++;

                if (!c.IsOrganism)
                {
                    // not a leaf, recurse
                    count += RelabelChildOrganismGroups(c, fromScratch);
                }
            }
            return count;
        }

        #endregion

        #region OrganismGroup "has" Process relation


        // This function was rewritten to query all organisms under group
        /// <summary>
        /// Return all processes for this organism group
        /// </summary>
        /// <returns>
        /// an array of processes for this organism group
        /// </returns>
        virtual public ServerProcess[] GetAllProcesses()
        {
            string RequiredIds = ServerOrganismGroup.RequireIdInList(this.GetAllChildrenAndParent(), "c", "organism_group_id");
            SqlCommand command = new SqlCommand(
                @"SELECT p.*
					FROM catalyzes c INNER JOIN processes p 
					ON c.process_id = p.[id]
					WHERE " + RequiredIds + " ORDER BY p.[name];");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }

            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }


        /// <summary>
        /// Return all processes for this organism group
        /// </summary>
        /// <returns>
        /// an array of processes for this organism group
        /// </returns>
        virtual public EntityRoleProcessAndPathway[] GetAllEntityRoleProcessAndPathways()
        {
            string RequiredIds = ServerOrganismGroup.RequireIdInList(this.GetAllChildrenAndParent(), "c", "organism_group_id");
            SqlCommand command = new SqlCommand(
                    @"SELECT pp.pathway_id AS pathwayId, c.process_id AS processId, role='none' 
					FROM catalyzes c INNER JOIN pathway_processes pp 
					ON c.process_id = pp.process_id
					WHERE " + RequiredIds + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new EntityRoleProcessAndPathway(new DBRow(d)));
            }

            return (EntityRoleProcessAndPathway[])results.ToArray(typeof(EntityRoleProcessAndPathway));
        }

        #endregion

		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			SqlCommand Insert = DBWrapper.BuildCommand("INSERT INTO " + __TableName + " VALUES (@id, @sname, @cname, @parent, @notes, @is_org, @nodeLabel);",
													   "@id", SqlDbType.UniqueIdentifier, this.ID,
													   "@sname", SqlDbType.VarChar, this.ScientificName,
													   "@cname", SqlDbType.VarChar, this.CommonName,
													   "@parent", SqlDbType.UniqueIdentifier, this.ParentID,
													   "@notes",  SqlDbType.VarChar, this.Notes,
													   "@is_org", SqlDbType.Bit, this.IsOrganism,
                                                       "@nodeLabel", SqlDbType.VarChar, this.NodeLabel);
			__DBRow.ADOCommands["insert"] = Insert;


			SqlCommand Select = DBWrapper.BuildCommand("SELECT FROM " + __TableName + " WHERE id = @id;",
													   "@id", SqlDbType.UniqueIdentifier, this.ID);
			__DBRow.ADOCommands["select"] = Select;




            SqlCommand Update = DBWrapper.BuildCommand("UPDATE " + __TableName + " SET scientific_name=@sname, common_name=@cname, parent_id=@parent, notes=@notes, is_organism=@is_org, nodeLabel=@nodeLabel WHERE id=@id;",
													   "@id", SqlDbType.UniqueIdentifier, this.ID,
													   "@sname", SqlDbType.VarChar, this.ScientificName,
													   "@cname", SqlDbType.VarChar, this.CommonName,
													   "@parent", SqlDbType.UniqueIdentifier, this.ParentID,
													   "@notes",  SqlDbType.Text, this.Notes,
													   "@is_org", SqlDbType.Bit, this.IsOrganism,
                                                       "@nodeLabel", SqlDbType.VarChar, this.NodeLabel);
			__DBRow.ADOCommands["update"] = Update;




			SqlCommand Delete = DBWrapper.BuildCommand("DELETE FROM " + __TableName + " WHERE id=@id;",
													   "@id", SqlDbType.UniqueIdentifier, this.ID);
			__DBRow.ADOCommands["delete"] = Delete;
		}
		#endregion


		#region Static Methods
		/// <summary>
		/// Return all organism groups from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapOrganismGroup objects ready to be sent via SOAP.
		/// </returns>
		public static ServerOrganismGroup[] AllOrganismGroups ( )
		{
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + " ORDER BY  [scientific_name], [common_name], [id];");
			return ServerOrganismGroup.LoadMultiple(command);
		}


        /// <summary>
        /// Return all organism groups for a pathway
        /// </summary>
        /// <param name="pathwayId">The pathway id</param>
        /// <returns>ServerOrganismGroup array</returns>
        public static ServerOrganismGroup[] AllOrganismGroupsForPathway(Guid pathwayId)
        {
			SqlCommand command = DBWrapper.BuildCommand(@"
                    SELECT og.* FROM " + __TableName + @" AS og
                    WHERE og.id IN (
                        SELECT DISTINCT c.organism_group_id
                        FROM catalyzes c INNER JOIN pathway_processes pp ON c.process_id = pp.process_id
                        WHERE pp.pathway_id = @pathway_id AND c.organism_group_id IS NOT NULL) ORDER BY og.common_name, og.id;",
                        "@pathway_id", SqlDbType.UniqueIdentifier, pathwayId);
			return ServerOrganismGroup.LoadMultiple(command);

        }

        /// <summary>
        /// Return all organism groups for a pathway Group
        /// </summary>
        /// <param name="cspathwayIds">comma separated list of pathway ids</param>
        /// <returns>ServerOrganismGroup array</returns>
        public static ServerOrganismGroup[] AllOrganismGroupsForPathway(string cspathwayIds)
        {
            char[] separator = {','};
            string[] pathwayIds = cspathwayIds.Split(separator);
            string ids = "";
            foreach (string pathwayId in pathwayIds)
            {
                ids += "'" + pathwayId + "',";
            }
            ids = ids.Substring(0, ids.Length - 1);

            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + @"
                    WHERE id IN ( SELECT DISTINCT c.organism_group_id
                        FROM catalyzes c INNER JOIN pathway_processes pp ON c.process_id = pp.process_id
                        WHERE pp.pathway_id IN (" + ids + ") AND c.organism_group_id IS NOT NULL) ORDER BY [scientific_name], [common_name];");
                        
            return ServerOrganismGroup.LoadMultiple(command);

        }

        public static ServerOrganismGroup[] AllOrganismGroupsWithGenesForPathway(string cspathwayIds)
        {
            char[] separator = { ',' };
            string[] pathwayIds = cspathwayIds.Split(separator);
            string ids = "";
            foreach (string pathwayId in pathwayIds)
            {
                ids += "'" + pathwayId + "',";
            }
            ids = ids.Substring(0, ids.Length - 1);

            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + @"
                    WHERE id IN ( SELECT DISTINCT c.organism_group_id
                        FROM catalyzes c INNER JOIN pathway_processes pp ON c.process_id = pp.process_id
                        WHERE pp.pathway_id IN (" + ids + @") AND c.organism_group_id IS NOT NULL
                        AND c.organism_group_id IN (SELECT organism_group_id from genes))
                                                  ORDER BY [scientific_name], [common_name];");

            return ServerOrganismGroup.LoadMultiple(command);

        }
		
		/// <summary>
		/// Create a list of id's that can be inserted into an SQL query (ex:    id IN (id_1, id_2, ...) )
		/// </summary>
		/// <param name="groups"></param>
		/// <returns></returns>
		public static string RequireIdInList( ServerOrganismGroup[] groups )
		{
			return RequireIdInList(groups, null, "id");
		}

		/// <summary>
		/// Create a list of id's that can be inserted into an SQL query (ex:    id IN (id_1, id_2, ...) )
		/// </summary>
		/// <param name="table">The pseudonym given to the organism_groups table.  Ex: grp for grp.id</param>
		/// <param name="column">The column name (usually just "id")</param>
		/// <param name="groups"></param>
		/// <returns>A list of ServerOrganismGroup id's ready to be inserted into an sql query</returns>
		public static string RequireIdInList( ServerOrganismGroup[] groups, string table, string column)
		{
			// This is kind of a hack: if groups is empty, build a query that will always fail
			if(groups.Length == 0)
				return "(1 = 0)";

			// Otherwise, Create a query requesting for the id to be in a list of id's
			StringBuilder list = new StringBuilder("(");
			if(table != null)             // Append the table name, if necessary
				list.Append(table + ".");
			list.Append(column + " IN (");

			foreach(ServerOrganismGroup item in groups)
				list.Append("'" + item.ID.ToString() + "', ");

			list.Remove(list.Length - 2, 2); // More efficient than checking at each iteration of the loop
			list.Append("))");
			return list.ToString();
		}


		#region Organism Hierarchy

		/// <summary>
		/// Get all organisms that are immediately (ie, 1 level) under this group
		/// </summary>
		static public ServerOrganism[] GetImmediateChildOrganisms ( Guid id )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
				FROM " + __TableName + @"
				WHERE parent_id = @id AND is_organism = 1;",
				"@id", SqlDbType.UniqueIdentifier, id);

			return ServerOrganism.LoadMultiple(command);
		}


		/// <summary>
		/// Get all groups immediately under this group
		/// </summary>
		static public ServerOrganismGroup[] GetImmediateChildGroups ( Guid id )
		{/*

			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
				FROM " + __TableName + @"
				WHERE parent_id = @id AND is_organism = 0;",
				"@id", SqlDbType.UniqueIdentifier, id);
			*/
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
				FROM " + __TableName + @"
				WHERE parent_id = @id",
				"@id", SqlDbType.UniqueIdentifier, id);
			return ServerOrganismGroup.LoadMultiple(command);
		}


		/// <summary>
		/// Get all groups and organisms immediately under this group
		/// </summary>
		/// <param name="id">id of an organism or group</param>
		/// <returns></returns>
		static public ServerOrganismGroup[] GetImmediateChildren( Guid id )
		{
			ArrayList results = new ArrayList();
			GetImmediateChildren(id, ref results);
			return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
		}

        /// <summary>
        /// Get all groups and organisms immediately under this group with at least one model
        /// </summary>
        /// <param name="id">id of an organism or group</param>
        /// <returns></returns>
        static public ServerOrganismGroup[] GetImmediateChildrenWithModel(Guid id)
        {
            ArrayList results = new ArrayList();
            GetImmediateChildrenWithModel(id, ref results);
            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }

		/// <summary>
		/// Get all organisms and groups immediately under the one specified by id.  This is a private version
		/// that uses a reference to an array list for efficiency in recursive calls
		/// </summary>
		/// <param name="id"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		static private void GetImmediateChildren( Guid id, ref ArrayList results )
		{
			SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE parent_id = @id ORDER BY [scientific_name], [common_name];",
				"@id", SqlDbType.UniqueIdentifier, id);

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple(out ds, ref command);

			foreach(DataSet r in ds)
				results.Add(LoadFromDBRow(new DBRow(r)));
        }

        /// <summary>
        /// Get all organisms and groups immediately under the one specified by id and has a model.  This is a private version
        /// that uses a reference to an array list for efficiency in recursive calls
        /// </summary>
        /// <param name="id"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        static private void GetImmediateChildrenWithModel(Guid id, ref ArrayList results)
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " ogr WHERE parent_id = @id" + 
                                                        @" AND EXISTS (SELECT * FROM organism_groups og
                                                                WHERE og.nodeLabel like ogr.nodeLabel + '%'
                                                                AND og.id IN (SELECT organismGroupId FROM ModelOrganism))
                                                                ORDER BY [scientific_name], [common_name];",
                "@id", SqlDbType.UniqueIdentifier, id);           


            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            foreach (DataSet r in ds)
                results.Add(LoadFromDBRow(new DBRow(r)));
        }

        //TODO
        #region BRENDAN: REDO TO USE LABELS!

        /// <summary>
		/// Get all organisms under the current group (not just immediately under)
		/// </summary>
		/// <param name="id"></param>
		/// <returns>An array of ServerOrganisms</returns>
		static public ServerOrganism[] GetAllChildOrganisms( Guid id )
		{
			ArrayList everything = new ArrayList();
			ServerOrganismGroup.GetAllChildren(id, ref everything); // Get everything under current group

			ArrayList retval = new ArrayList();
			ServerOrganism IsOrg;
			foreach(ServerOrganismGroup Org in everything)          // If item is an organism, add it to a list
				if( (IsOrg = (Org as ServerOrganism)) != null)
					retval.Add(IsOrg);
			return (ServerOrganism[])retval.ToArray(typeof(ServerOrganism)); // List should contain only organisms
		}


		/// <summary>
		/// Return all groups under the current group (note: does not return any group w/ an organism part.  If
		/// you need that, just call GetAllChildren )
		/// </summary>
		/// <param name="id"></param>
		/// <returns>An array of ServerOrganismGroup's containing only groups</returns>
		static public ServerOrganismGroup[] GetAllChildGroups( Guid id )
		{
			ArrayList everything = new ArrayList();
			ServerOrganismGroup.GetAllChildren(id, ref everything); // Get everything under current group

			ArrayList retval = new ArrayList();
			foreach(ServerOrganismGroup Org in everything)          // If item is not an organism, add it to group
				if( (Org is ServerOrganism) == false)
					retval.Add(Org);
			return (ServerOrganism[])retval.ToArray(typeof(ServerOrganism)); // Should contain only groups
		}


		/// <summary>
		/// Get all organisms and groups under the current group
		/// </summary>
		/// <param name="id"></param>
		/// <returns>An array of ServerOrganismGroup's containing both groups and organisms</returns>
		static public ServerOrganismGroup[] GetAllChildren( Guid id )
		{
			ArrayList results = new ArrayList();
			ServerOrganismGroup.GetAllChildren(id, ref results);
			return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
		}


		/// <summary>
		/// Grab all organisms and groups under current group, plus the current group
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		static public ServerOrganismGroup[] GetAllChildrenAndParent( Guid id )
		{
			ArrayList results = new ArrayList();
			results.Add(LoadFromID(id));
			ServerOrganismGroup.GetAllChildren(id, ref results);
			return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
		}

		/// <summary>
		/// Get all children under the specified group or organism.  This is a private version used
		/// for efficient recursive calls
		/// </summary>
		/// <param name="id"></param>
		/// <param name="results"></param>
		static private void GetAllChildren( Guid id, ref ArrayList results )
		{
			ArrayList children = new ArrayList();
			GetImmediateChildren( id, ref children );

			foreach(ServerOrganismGroup child in children)
			{
				results.Add(child);
				ServerOrganismGroup.GetAllChildren(child.ID, ref results);
			}
        }

        #endregion

        /// <summary>
        /// Return all organisms and organism groups as ServerOrganismGroup Objects from the system.
        /// </summary>
        /// <returns>
        /// Array of ServerOrganismGroups (some of which are just organisms)
        /// </returns>
        public static ServerOrganismGroup[] AllOrganismEntities( )
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " +  __TableName + " ORDER BY [scientific_name], [common_name];" );

            return LoadMultiple( command );
        }

        public static ServerOrganismGroup[] AllOrganismsHasModels()
        {
            string queryString = @"SELECT og.* 
                                    FROM organism_groups og, ModelOrganism mo
                                    WHERE og.is_organism = 1 AND mo.organismGroupId = og.id
                                    ORDER BY og.scientific_name, og.common_name";

            SqlCommand command = DBWrapper.BuildCommand(queryString);

            return LoadMultiple(command);

        }

        /// <summary>
        /// Return all organisms as ServerOrganismGroup Objects from the system.
        /// </summary>
        /// <returns>
        /// Array of ServerOrganismGroups (all of which are just organisms)
        /// </returns>
        public static ServerOrganismGroup[] AllOrganisms()
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE is_organism = 1 ORDER BY [scientific_name], [common_name];");

            return LoadMultiple(command);
        }


        public static ServerOrganismGroup[] OrganismsHasModel()
        {
            //string sqlString = @" SELECT og.* From organism_groups og, Catalyzes c" +
            //                " WHERE  og.is_organism = 1 AND " +
            //                " og.id = c.organism_group_id AND c.process_id IN " +
            //                " ( SELECT DISTINCT pe.processId  " +
            //                  " FROM MapReactionsProcessEntities pe ) " +
            //                " ORDER BY og.scientific_name";

            string sqlString = @"select * from organism_groups
                                where id in
                                (
                                SELECT DISTINCT o.id
                                From organisms o, Catalyzes c
                                WHERE  c.process_id IN 
                                      (SELECT DISTINCT pe.processId  
                                       FROM MapReactionsProcessEntities pe ) 
                                AND o.id = c.organism_group_id 
                                )
                                ORDER BY scientific_name";

            SqlCommand command = DBWrapper.BuildCommand(sqlString);
            return LoadMultiple(command);

        }
        
        /// <summary>
        /// Return all organism groups as ServerOrganismGroup Objects from the system.
        /// </summary>
        /// <returns>
        /// Array of ServerOrganismGroups (all of which are just organism groups)
        /// </returns>
        public static ServerOrganismGroup[] AllOrganismsGroup()
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE is_organism = 0 ORDER BY [scientific_name], [common_name];");

            return LoadMultiple(command);
        }

        /// <summary>
        /// Return all organisms which has a gene in the database.
        /// </summary>
        /// <returns>
        /// Array of ServerOrganismGroups (all of which are just organisms)
        /// </returns>
        public static ServerOrganismGroup[] AllOrganismsWithGenes()
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE is_organism = 1 AND id IN (SELECT organism_group_id from genes) ORDER BY [scientific_name], [common_name];");

            return LoadMultiple(command);
        }



        /// <summary>
        /// Returns an array containing organisms and groups
        /// </summary>
        /// <param name="command">A query returning mutltiple rows from the organism_groups table.  Note
        /// that it must return all columns, and not just the id</param>
        /// <returns></returns>
        internal static ServerOrganismGroup[] LoadMultiple(SqlCommand command)
        {
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach(DataSet r in ds)
                results.Add(LoadFromDBRow(new DBRow(r))); // Decides if this is a group or an organism

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }

        /// <summary>
        /// Attempts to load an organism group from a row of data.  If this is an organism, it will pull the
        /// additional information that it needs
        /// </summary>
        /// <param name="r">A row from organism_groups containing information for the organism or group</param>
        /// <returns></returns>
        public static ServerOrganismGroup LoadFromDBRow(DBRow r)
        {
            if(r.GetBool("is_organism") == false)
                return new ServerOrganismGroup(r);         // Load organism group from row data
            else
                return ServerOrganism.LoadFromGroupRow(r); // Figure out the organism part and load the object
        }


        /// <summary>
        /// Given an id, it will load an organism or an organism_group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ServerOrganismGroup LoadFromID(Guid id)
        {
            DBRow group_row = ServerOrganismGroup.LoadRow(id);
            return LoadFromDBRow(group_row);
        }

		#endregion


        public static List<ServerOrganismGroup> SelectOrganismGroups(string fromClause, string whereClause, params object[] paramNameTypeValueList)
        {
            string meSubSelect = @"SELECT DISTINCT og.id
							FROM organism_groups og " + (fromClause != null ? ", " + fromClause : "") + @"
							" + (whereClause != null ? " WHERE " + whereClause + " " : "");

            SqlCommand commandOrgGroups = DBWrapper.BuildCommand(
                    @"SELECT og2.*
					FROM organism_groups og2, (" + meSubSelect + @" ) AS uniqueOG
					WHERE uniqueOG.id = og2.id
                    ORDER BY [scientific_name], [common_name];",
                paramNameTypeValueList);

            List<ServerOrganismGroup> results = new List<ServerOrganismGroup>();
            Dictionary<Guid, DBRow> orgRows = new Dictionary<Guid, DBRow>();
            DataSet[] ds;

            DBWrapper.LoadMultiple(out ds, ref commandOrgGroups);

            foreach (DataSet d in ds)
            {
                DBRow r = new DBRow(d);
                if (r.GetBool("is_organism") == false)
                {
                    results.Add(new ServerOrganismGroup(r));
                }
                else
                {
                    orgRows.Add(r.GetGuid("id"), r);
                }
            }

            if (orgRows.Count > 0)
            {
                SqlCommand commandOrgs = DBWrapper.BuildCommand(
                    @"SELECT o.*
					FROM organisms o, (" + meSubSelect + @" ) AS uniqueOG
					WHERE uniqueOG.id = o.id;",
                    paramNameTypeValueList);

                DBWrapper.LoadMultiple(out ds, ref commandOrgs);

                foreach (DataSet d in ds)
                {
                    DBRow r = new DBRow(d);
                    results.Add(new ServerOrganism(r, orgRows[r.GetGuid("id")]));
                }
            }

            return results;
        }


        #region Organism Browsing

        /// <summary>
        /// Returns the GUID IDs of all pathways in the database.
        /// </summary>
        /// <returns></returns>
        public static Guid[] AllOrganismGroupIds()
        {
            ArrayList guids = new ArrayList();
            SqlCommand command = new SqlCommand("SELECT DISTINCT id FROM organism_groups");
            DataSet ds;

            if (DBWrapper.Instance.ExecuteQuery(out ds, ref command) > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
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
        public static ServerPathway[] AllOrganismGroups(int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											ORDER BY common_name ) " + __TableName + @"
									ORDER BY common_name DESC ) " + __TableName + @"
				ORDER BY common_name");

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerPathway[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }

        /// <summary>
        /// Returns a count of all the organisms and groups in the database
        /// </summary>
        /// <returns></returns>
        public static int CountAllOrganismGroups()
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        /// <summary>
        /// Returns all organims groups whose name contains the given substring
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] FindByName(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE [common_name] " +
                (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring "  +
                    "OR  [scientific_name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring " +
                "ORDER BY [scientific_name];",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }


        /// <summary>
        /// A search function for paging
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] FindByName(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											WHERE [common_name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
											OR [scientific_name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
                                            ORDER BY [scientific_name] ) " + __TableName + @"
									ORDER BY [scientific_name] DESC ) " + __TableName + @"
				ORDER BY [scientific_name], [common_name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }

        /// <summary>
        /// A search function for paging
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] FindByNameWithModel(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @" ogr
											WHERE ([common_name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
											OR [scientific_name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring)
                                            AND EXISTS (SELECT * FROM organism_groups og
                                                                WHERE og.nodeLabel like ogr.nodeLabel + '%'
                                                                AND og.id IN (SELECT organismGroupId FROM ModelOrganism))
                                            ORDER BY [scientific_name] ) " + __TableName + @"
									ORDER BY [scientific_name] DESC ) " + __TableName + @"
				ORDER BY [scientific_name], [common_name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }

        /// <summary>
        /// Count of the number of organisms and groups that would be found with the supplied search parameters.
        /// Searches for both common and scientific name.
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
        public static int CountFindByName(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " WHERE [common_name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring" +
                    " OR  [scientific_name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring;",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        /// <summary>
        /// Count of the number of organisms and groups that would be found with the supplied search parameters.
        /// Searches for both common and scientific name.
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
        public static int CountFindByNameWithModel(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " ogr WHERE ([common_name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring" +
                    " OR  [scientific_name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring)" +
                    @" AND EXISTS (SELECT * FROM organism_groups og
                                   WHERE og.nodeLabel like ogr.nodeLabel + '%'
                                   AND og.id IN (SELECT organismGroupId FROM ModelOrganism));",
                "@substring", SqlDbType.VarChar, substring);            

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        /// <summary>
        /// Returns all organims groups whose name contains the given substring
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] FindByCommonName(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE [common_name] " +
                (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") +
                " @substring ORDER BY [common_name];",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }


        /// <summary>
        /// A search function for paging
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public static ServerOrganismGroup[] FindByCommonName(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											WHERE [common_name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
											ORDER BY [common_name] ) " + __TableName + @"
									ORDER BY [common_name] DESC ) " + __TableName + @"
				ORDER BY [common_name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }

        /// <summary>
        /// Count of the number of organisms and groups that would be found with the supplied search parameters.
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
        public static int CountFindByCommonName(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " WHERE [common_name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring;",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        #endregion

        /// <summary>
		/// Returns all organism groups that don't have parent
		/// </summary>
		/// <returns></returns>
		public static ServerOrganismGroup[] GetRoots()
		{
			ServerOrganismGroup og = new ServerOrganismGroup();
			SqlCommand command = new SqlCommand( "SELECT * FROM organism_groups WHERE parent_id IS NULL ORDER BY [scientific_name], [common_name];" );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerOrganismGroup( new DBRow( d ) ) );
			}

			return ( ServerOrganismGroup[] ) results.ToArray( typeof( ServerOrganismGroup ) );
		}

        /// <summary>
        /// Returns all organism groups that don't have parent and has an associated model
        /// </summary>
        /// <returns></returns>
        public static ServerOrganismGroup[] GetRootsWithModel()
        {
            ServerOrganismGroup og = new ServerOrganismGroup();
            SqlCommand command = new SqlCommand(@"SELECT * FROM organism_groups ogg 
                                                  WHERE ogg.parent_id IS NULL 
                                                  AND EXISTS (SELECT * FROM organism_groups ogr
                                                                WHERE ogr.parent_id=ogg.id
                                                                AND EXISTS (SELECT * FROM organism_groups og
                                                                    WHERE og.nodeLabel like ogr.nodeLabel + '%'
                                                                    AND og.id IN (SELECT organismGroupId FROM ModelOrganism)))
                                                  ORDER BY [scientific_name], [common_name];");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerOrganismGroup(new DBRow(d)));
            }

            return (ServerOrganismGroup[])results.ToArray(typeof(ServerOrganismGroup));
        }


		/// <summary>
		/// Adds its subhierarchy to the input XML fragment
		/// </summary>
		/// <returns></returns>
		public string AddSubHierarchyToXML()
		{
			StringBuilder xml = new StringBuilder();

			xml.Append( "<OrganismGroup id=\"" + this.ID.ToString() + "\" commonName=\"" + this.CommonName + "\" scientificName=\"" 
							+ this.ScientificName + "\">" );

			foreach(ServerOrganismGroup child in this.GetImmediateChildGroups())
			{
				xml.Append(child.AddSubHierarchyToXML());
			}

			xml.Append( "</OrganismGroup>" );

			return xml.ToString();
		}



		/// <summary>
		/// Return a organism group with given ID.
		/// </summary>
		/// <param name="id">
		/// The Guid of the desired organism group.
		/// </param>
		public static ServerOrganismGroup Load ( Guid id )
		{
			return new ServerOrganismGroup( LoadRow ( id ) );
		}

		/// <summary>
		/// Return the DBRow for a organism group with a given ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		internal static DBRow LoadRow ( Guid id )
		{
			SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " WHERE id = @id;",
														"@id", SqlDbType.UniqueIdentifier, id);

			DataSet ds = new DataSet();

			DBWrapper.LoadSingle( out ds, ref command );

			return new DBRow(ds);
		}

		/// <summary>
		/// Creates a single ServerOrganism from an SQL query
		/// </summary>
		/// <param name="command">An SQL query that returns a single row from organism_groups.  Note that the query
		/// must output every column of organism_groups</param>
		/// <returns></returns>
		internal static ServerOrganismGroup LoadSingle(SqlCommand command)
		{
			DataSet ds;
			DBWrapper.LoadSingle(out ds, ref command);
			return new ServerOrganismGroup(new DBRow(ds));
		}
		

		/// <summary>
		/// See if given id corresponds to an existing organism group
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool Exists(Guid id)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE id = @id AND is_organism = 0;",
				"@id", SqlDbType.UniqueIdentifier, id);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}


		/// <summary>
		/// Check for the existance of the group by scientific or common name
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static bool Exists(string Name)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT *
				FROM " + __TableName + @"
				WHERE (scientific_name = @name OR common_name = @name) AND is_organism = 0;",
				"@name", SqlDbType.VarChar, Name);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}



		/// <summary>
		/// Creates an xml string representing the hierarchy of all groups
		/// </summary>
		/// <returns></returns>
		public static string GetOrganismHierarchy()
		{
			if ( DBWrapper.IsInstanceNull )
			{
				DBWrapper.Instance = new DBWrapper();
			}

			ServerOrganismGroup[] organisms = ServerOrganismGroup.GetRoots();

			StringBuilder result = new StringBuilder();
			result.Append("<OrganismHierarchy>");

			foreach(ServerOrganismGroup root in organisms)
			{
				result.Append( root.AddSubHierarchyToXML() );
			}

			result.Append( "</OrganismHierarchy>" );

			return result.ToString();
		}

		#endregion

		#endregion


	} // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerOrganismGroup.cs,v 1.11 2010/11/19 21:13:29 murat Exp $
	$Log: ServerOrganismGroup.cs,v $
	Revision 1.11  2010/11/19 21:13:29  murat
	1-) Insert sql query was not structure in ServerSBase and ServerModel files those
	queries are fixed.
	
	2-) Some of the queries from ServerGoTerms, ServerOrganismGroup, ServerPathway and ServerProsess
	were wrong, they are fixed (m.sbmlId changed to sbmlId)
	
	3-) SMBLParser now can work automatically even the model names (or any other model attributes) have changed.
	
	Revision 1.10  2009/09/17 20:40:38  ann
	*** empty log message ***
	
	Revision 1.9  2009/07/15 14:41:59  ann
	*** empty log message ***
	
	Revision 1.8  2009/07/14 20:30:08  ann
	*** empty log message ***
	
	Revision 1.7  2009/07/14 15:33:22  ann
	*** empty log message ***
	
	Revision 1.6  2009/05/27 14:04:25  ann
	*** empty log message ***
	
	Revision 1.5  2009/05/14 14:28:17  ann
	*** empty log message ***
	
	Revision 1.4  2009/04/29 14:12:04  ann
	*** empty log message ***
	
	Revision 1.3  2009/04/07 14:44:12  ali
	*** empty log message ***
	
	Revision 1.2  2009/04/03 13:31:09  ali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.12  2008/04/28 17:34:53  brendan
	fixed bugs in insert/delete/exists/load functions of ServerCatalyze involving org_id; updated unit test for ServerCatalyze
	
	Revision 1.11  2007/09/20 19:25:56  brendan
	*** empty log message ***
	
	Revision 1.10  2007/09/07 20:09:07  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.9  2007/08/24 19:44:38  ann
	Changes made in queries based on the new dataset field
	
	Revision 1.8  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.7  2007/05/18 06:26:54  steve
	*** empty log message ***
	
	Revision 1.6  2007/05/11 08:28:48  ali
	Fixed the issues related to significant pathways query for a given set of genes.
	
	Revision 1.5  2007/04/09 17:14:31  ali
	*** empty log message ***
	
	Revision 1.4  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.3  2006/10/03 17:47:44  brendan
	*** empty log message ***
	
	Revision 1.2  2006/09/11 17:04:55  brandon
	Added a bunch of stuff for organism details, plus stuff for process GO term details.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.39  2006/06/19 20:59:18  greg
	All of the built-in queries appear to return the correct results in all cases now, but we'll still want some more thorough testing done eventually.  The GO pathway viewer tools should now work correctly, and menu items on the content bar on the left are now displayed in bold when they're what's on the screen (though there are still a few cases where this doesn't work 100% correctly; I'm trying to figure out how to address that).
	
	Revision 1.38  2006/06/12 15:08:31  ali
	Additional Links added to browser section...
	GraphLayout XML Generator has been modified to output organism ids..
	
	Revision 1.37  2006/06/02 21:50:44  brandon
	The two Molecular Entity queries are done and work correctly, although I couldn't test the graph drawing on my machine.  Added some queries to ServerMolecularEntity, ServerOrganismGroup, and MolecularEntityQueries
	
	Revision 1.36  2006/05/29 18:29:09  greg
	The following queries appear to function correctly now:
	
	- Processes involving exactly one substrate and one product
	- Processes with the given number of molecules in a specific use
	- Processes involving a particular pathway and molecular entity
	- Processes sharing activators and inhibitors with a process of particular pathway
	
	Some of the other files were tweaked a bit to fix errors and/or improve performance.
	
	Revision 1.35  2006/05/26 16:02:45  greg
	 - Testing components
	The PathwaysLibTester has been updated to include support for tests to be run based on command-line arguments using the awesomeness of reflection.
	
	 - LinkHelper
	Some important fixes were made to this class again.  The way it used to be broke some stuff in other pages, but most/all of those issues should be resolved now.
	
	 - Built-in queries
	The one-substrate-and-product query produces something now; whether it's correct or not still needs to be checked.  The fixed code will be moved into ServerObject classes soon.
	
	Revision 1.34  2006/05/19 03:24:51  fatih
	*** empty log message ***
	
	Revision 1.33  2006/05/18 19:50:54  fatih
	organism hierarchy function updated for Mustafa
	
	Revision 1.32  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.31  2006/05/17 21:02:17  brendan
	Fixed query in server org that was breaking collapsed pathway visualization
	
	Revision 1.30  2006/05/16 23:00:44  gokhan
	*** empty log message ***
	
	Revision 1.29  2006/05/16 19:47:27  gokhan
	*** empty log message ***
	
	Revision 1.28  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.27  2006/04/17 18:59:53  brian
	1. Sections of code that are dependent on PathwayLib2 will throw an exception
	2. Updated a few queries to reflect new database layout
	
	Revision 1.26  2006/04/12 22:02:56  brian
	*** empty log message ***
	
	Revision 1.25  2006/04/12 21:01:13  brian
	*** empty log message ***
	
	Revision 1.22.2.11  2006/04/10 22:17:25  brian
	Aside from a strange duplicate entry bug, all pre-query dropdowns appear to be working now
	
	Revision 1.22.2.10  2006/04/07 19:29:27  brian
	Fixed several "pre-queries".  Not too happy w/ some of the solutions, but I'll come up w/ something better later
	
	Revision 1.22.2.9  2006/03/29 22:56:51  brian
	*** empty log message ***
	
	Revision 1.22.2.8  2006/03/23 18:42:11  brian
	ServerOrganism should work correctly now
	
	Revision 1.22.2.7  2006/03/22 21:25:21  brian
	Removed and renamed a few more functions for consistency
	
	Revision 1.22.2.6  2006/03/22 20:24:16  brian
	I forgot to list my changes over the last few commits, so here's a quick overview:
	1. IOrganismEntity interface removed
	2. All ServerObject queries that return IOrganismEntity now return either ServerOrganism or ServerOrganismGroup
	3. All OrganismGroup-Process relations have been commented out (pending removal)
	4. Several new functions have been added to ServerOrganism and ServerOrganismGroup too simplify the initialization of objects
	
	Revision 1.22.2.5  2006/03/22 19:48:06  brian
	*** empty log message ***
	
	Revision 1.22.2.4  2006/03/20 19:26:32  brian
	*** empty log message ***
	
	Revision 1.22.2.3  2006/03/15 03:44:57  brian
	Reorganized ServerOrganismGroup and SoapOrganismGroup to allow for ServerOrganism to be a derived type
	
	Revision 1.22.2.2  2006/03/12 06:43:16  brian
	I've rewired several parts of ServerOrganism so that it derives from ServerOrganismGroup.  I'm not too crazy about this new model, but I'll see what happens.
	
	Revision 1.22.2.1  2006/03/03 01:48:24  brian
	*** empty log message ***
	
	Revision 1.22  2006/02/27 22:21:40  brian
	*** empty log message ***
	
	Revision 1.20.2.5  2006/02/24 04:47:18  brian
	The new functions appear to be working.  Right now, there is only a demonstration of the new pathway selection routine (we should modify our interface so that it's passing id's, not names of organisms).
	
	Revision 1.20.2.4  2006/02/23 18:13:04  brian
	*** empty log message ***
	
	Revision 1.20.2.3  2006/02/23 05:05:12  brian
	0. Created an OrganismMeta utility to load organisms and groups in batches
	1. Renamed ServerOrganismGroup.GetAllOrganisms() to ServerOrganismGroup.GetChildOrganisms()
	2. Created a ServerOrganismGroup.GetImmediateChildren() function to grab all organisms and groups 1 level deeper
	3. Modified GetChildGroups() so that it only returns groups (not organisms as well)
	                --> We might need to update a PathwaysService object that referenced this
	
	Revision 1.20.2.2  2006/02/22 23:41:42  brian
	1.  Unifying organism and organism_group tables
	2.  Operations to get pathways by organism or group are now handled by polymorphic functions
	
	Revision 1.20.2.1  2006/02/16 23:25:51  brian
	1. Updated a few minor sections to use PathwaysLib3
	2. Need to find a way to query multiple entities (ex, organisms and organism groups)
	
	Revision 1.20  2006/01/27 06:50:07  fatih
	Added helper methods for retrieving the organism hierarchy
	
	Revision 1.19  2005/12/05 03:04:17  gokhan
	FindOrganisms and FindOrganismGroups methods are added
	
	Revision 1.18  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.17  2005/11/02 20:35:16  fatih
	Test functions are working for pathwayslib objects
	
	Revision 1.16  2005/10/31 20:27:45  fatih
	*** empty log message ***
	
	Revision 1.15  2005/10/28 10:48:29  fatih
	Pathway, process, organism group tested and corrected
	
	Revision 1.14  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.13  2005/08/19 21:33:42  brandon
	cleaned up some files, added some comments
	
	Revision 1.12  2005/07/20 22:31:20  brandon
	Added exists to Pathway and Process, add Exists for names in Organism and OrganismGroup
	
	Revision 1.11  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.10  2005/07/15 21:02:00  brandon
	added more queries
	
	Revision 1.9  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.8  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.7  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.6  2005/07/08 19:32:05  brandon
	fixed ServerCatalyze, sort of,  and uh, this project builds now
	
	Revision 1.5  2005/07/07 15:10:28  brandon
	Added ServerCatalyze.cs (gene_product_and_processes), it's not done yet, and added the GetAllOrganismGroups function to ServerProcess object
	
	Revision 1.4  2005/07/06 21:24:17  brandon
	Fixed Tribool, and got AddProcessToOrganismGroup to work!
	
	Revision 1.3  2005/07/06 20:18:21  brandon
	Added server objects for RNA and EC number.  Done with the relation between Pathway and Process, still working on relation between Process and Organism Group.  Function AddProcessToOrganismGroup still not working, can't figure out why
	
	Revision 1.2  2005/07/01 16:45:21  brandon
	not sure if it worked when I tried to commit OrganismGroup classes, so I'm trying again
	
	Revision 1.1  2005/07/01 16:40:12  brandon
	Added OrganismGroup objects
	
		
------------------------------------------------------------------------*/
#endregion