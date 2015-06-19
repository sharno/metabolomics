#region Using Declarations
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

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
    ///		<filepath>PathwaysLib/Server/ServerMolecularEntity.cs</filepath>
    ///		<creation>2005/06/23</creation>
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
    ///				<contributor>
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
    ///			<cvs_author>$Author: rishi $</cvs_author>
    ///			<cvs_date>$Date: 2009/07/05 22:17:06 $</cvs_date>
    ///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerMolecularEntityclass.cs,v 1.1 2009/07/05 22:17:06 rishi Exp $</cvs_header>
    ///			<cvs_branch>$Name:  $</cvs_branch>
    ///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
    ///		</cvs>
    ///</sourcefile>
    /// <summary>
    /// Encapsulates database access related to molecular entities.  
    /// </summary>
    /// <remarks>
    /// This is an abstract base class, and it's query methods return 
    /// instances of derived classes. 
    /// </remarks>
    #endregion
    public class ServerMolecularEntityclass : ServerMolecularEntity, IGraphMolecularEntity
    {

        #region Constructor, Destructor, ToString
        /// <summary>
        /// Constructor
        /// </summary>
        protected ServerMolecularEntityclass()
        {
        }

        //(BE) This constructor never worked!!!
        //		/// <summary>
        //		/// Creation constructor
        //		/// </summary>
        //		/// <param name="name"></param>
        //		/// <param name="type"></param>
        //		/// <param name="molecular_entity_notes"></param>
        //        public ServerMolecularEntity (string name, string type, string molecular_entity_notes)
        //        {
        //            // not yet in DB, so create empty row
        //            __DBRow = new DBRow( __TableName );
        //
        //			Guid id = DBWrapper.NewID();
        //			Console.WriteLine(id.ToString());
        //
        //            this.ID = id; // generate a new ID
        //			
        //			//Console.WriteLine(this.ID.ToString());
        //            this.Name = name;
        //            this.Type = type;
        //            this.MolecularEntityNotes = molecular_entity_notes;
        //        }

        /// <summary>
        /// Constructor for server molecular entity wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerMolecularEntity object from a
        /// SoapMolecularEntity object.
        /// </remarks>
        /// <param name="data">
        /// A SoapMolecularEntity object from which to construct the
        /// ServerMolecularEntity object.
        /// </param>
        /// <param name="type"></param>
        public ServerMolecularEntityclass(SoapMolecularEntity data, string type)
        {
            // (BE) setup database row
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __DBRow = new DBRow(__TableName);
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

            this.Type = type;

            // (BE) MolecularEntity's UpdateFromSoap is called from the derived classes's UpdateFromSoap method, after all DBRow's have been initialized
            //if (data.Status != ObjectStatus.ReadOnly && data.GetType() == typeof(SoapMolecularEntity))
            //    UpdateFromSoap(data);
        }

        /// <summary>
        /// Constructor for server molecular entity wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerMolecularEntity object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerMolecularEntityclass(DBRow data)
        {
            // (mfs)
            // setup object
            __DBRow = data;
        }

        /// <summary>
        /// Destructor for the ServerMolecularEntity class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerMolecularEntityclass()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "molecular_entities";

        bool nameDirty = false;
        Hashtable molecularEntityNames = null;
        #endregion


        #region Properties
        /// <summary>
        /// Get/set the molecular entity ID.
        /// This is virtual so the derived class
        /// can override it to change the value in both rows.
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
        /// Get/set the molecular entity name.
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
                AddName(value, "primary name"); // set this as the primary name
                nameDirty = true;
            }
        }

        /// <summary>
        /// Get/set the molecular entity type.
        /// Only modifiable by derived classes,
        /// as this property is used for inheritance!
        /// 
        /// This now wraps TypeId.
        /// </summary>
        public string Type
        {
            get
            {
                return MolecularEntityTypeManager.GetTypeName(this.TypeId);
            }
            set
            {
                TypeId = MolecularEntityTypeManager.GetTypeId(value); // (be) will fail if string is not in db!
            }
        }

        /// <summary>
        /// Get/set molecular entity type ID.
        /// </summary>
        public int TypeId
        {
            get
            {
                return __DBRow.GetInt("type_id");
            }
            set
            {
                __DBRow.SetInt("type_id", value);
            }
        }

        /// <summary>
        /// Get/set the molecular entity notes.
        /// </summary>
        public string MolecularEntityNotes
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
        /// Returns the type of the MolecularEntity because Type is protected
        /// </summary>
        /// <returns></returns>
        public string EntityType()
        {
            return this.Type;
        }


        /// <summary>
        /// Return a list of roles that this entity performs in an organism
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public string[] GetRolesInOrganism(ServerOrganismGroup org)
        {
            // TODO: This function should be modified if we choose to implement MoleculeRoleManager
            return GetRolesInOrganism(org.ID);
        }


        /// <summary>
        /// Return a list of roles that this entity performs in an organism
        /// </summary>
        /// <param name="orgID"></param>
        /// <returns></returns>
        public string[] GetRolesInOrganism(Guid orgID)
        {
            // TODO: This function should be modified if we choose to implement MoleculeRoleManager
            string RequiredIds = ServerOrganismGroup.RequireIdInList(ServerOrganismGroup.GetAllChildrenAndParent(orgID), "c", "organism_group_id");
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT role
					FROM view_process_entities pe, catalyzes c
					WHERE pe.entity_id = @entID AND c.process_id = pe.process_id AND " + RequiredIds + @"
					ORDER BY role",
                "@entID", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataRow dr in ds.Tables[0].Rows)
                results.Add(dr["role"].ToString());

            return (string[])results.ToArray(typeof(string));
        }


        /// <summary>
        /// Return a list of roles that this entity performs in an organism
        /// </summary>
        /// <returns>All the roles</returns>
        public string[] GetRoles()
        {
            // TODO: This function should be modified if we choose to implement MoleculeRoleManager
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT role
					FROM view_process_entities pe, catalyzes c
					WHERE pe.entity_id = @entID AND c.process_id = pe.process_id 
					ORDER BY role",
                "@entID", SqlDbType.UniqueIdentifier, this.ID);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataRow dr in ds.Tables[0].Rows)
                results.Add(dr["role"].ToString());

            return (string[])results.ToArray(typeof(string));
        }

        /// <summary>
        /// Returns a representation of this object suitable for being
        /// sent to a client via SOAP.
        /// </summary>
        /// <returns>
        /// A SoapObject object capable of being passed via SOAP.
        /// </returns>
        public override SoapObject PrepareForSoap(SoapObject derived)
        {
            // this is an abstract class, so this must be created by the derived class
            SoapMolecularEntity retval = (SoapMolecularEntity)derived;

            retval.ID = this.ID;
            retval.Name = this.Name;
            // retval.Type = this.Type; // (BE) used for inheritance, can't be assigned to
            retval.MolecularEntityNotes = this.MolecularEntityNotes;

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
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapMolecularEntity me = o as SoapMolecularEntity;

            if (o.Status == ObjectStatus.Insert && me.ID == Guid.Empty)
                me.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = me.ID;
            this.Name = me.Name;
            this.Type = me.Type;
            this.MolecularEntityNotes = me.MolecularEntityNotes;

        }

        #region Entity name lookup relation ('entity_name_lookups' table)

        private void LoadMolecularEntityNames()
        {
            if (molecularEntityNames == null)
            {
                molecularEntityNames = new Hashtable();
                ServerMolecularEntityName[] names = ServerMolecularEntityName.AllNames(this.ID);
                if (names.Length > 0)
                {
                    foreach (ServerMolecularEntityName name in names)
                    {
                        molecularEntityNames.Add(name.NameId, name);
                    }
                }
            }
        }

        /// <summary>
        /// Get all of the names for this molecular entity
        /// </summary>
        public ServerMolecularEntityName[] AllNames
        {
            get
            {
                LoadMolecularEntityNames();
                ArrayList results = new ArrayList(molecularEntityNames.Values);
                return (ServerMolecularEntityName[])results.ToArray(typeof(ServerMolecularEntityName));
            }
        }

        /// <summary>
        /// Adds a name for this molecular entity to the molecular_entity_names
        /// table and sets the type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void AddName(string name, string type)
        {
            LoadMolecularEntityNames();
            Guid nameId = EntityNameManager.AddName(name);
            if (!HasName(name))
            {
                // add the name to this object
                ServerMolecularEntityName n = new ServerMolecularEntityName(new SoapMolecularEntityName(this.ID, name, type));
                molecularEntityNames.Add(name, n);
            }
            else
            {
                // modify type of existing name
                //((ServerMolecularEntityName)molecularEntityNames[name]).Type = type;
                ServerMolecularEntityName nameObj = GetName(name);
                nameObj.Type = type;
            }
        }

        private void RemoveOldPrimaryNames(string newName)
        {
            LoadMolecularEntityNames();
            if (molecularEntityNames.Count > 0)
            {
                foreach (ServerMolecularEntityName name in molecularEntityNames.Values)
                {
                    if (name.Type == "primary name" && name.Name != newName)
                    {
                        name.Type = "other name";
                    }
                }
            }
        }

        /// <summary>
        /// Does this MolecularEntity have name amongst its names.
        /// </summary>
        /// <param name="name">
        /// The name we're checking.
        /// </param>
        /// <returns>
        /// Existence of parameter name in this molecularentity's names list.
        /// </returns>
        public bool HasName(string name)
        {
            return GetName(name) != null;
        }

        /// <summary>
        /// Gets the ServerMolecularEntityName object associated with a name, or null if the name is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ServerMolecularEntityName GetName(string name)
        {
            //return ServerMolecularEntityName.Exists(this.ID, name);
            LoadMolecularEntityNames();
            if (molecularEntityNames.Count > 0)
            {
                foreach (ServerMolecularEntityName n in molecularEntityNames.Values)
                {
                    if (n.Name == name)
                    {
                        return n;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Dlete a name from this molecular entity's name list.
        /// </summary>
        /// <param name="name">
        /// The name to be removed.
        /// </param>
        public void DeleteName(string name)
        {
            //ServerMolecularEntityName n = ServerMolecularEntityName.Load(this.ID, name);
            //n.Delete();

            LoadMolecularEntityNames();
            if (molecularEntityNames.Count > 0)
            {
                foreach (ServerMolecularEntityName n in molecularEntityNames.Values)
                {
                    if (n.Name == name)
                    {
                        n.Delete();
                    }
                }
            }

        }

        /// <summary>
        /// Return all reaction annotations for this molecular entity-species mapping.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, BiomodelAnnotation> GetAllSpeciesAnnotations()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT MP.speciesId, MP.qualifierId
				FROM MapSpeciesMolecularEntities MP
			    WHERE MP.molecularEntityId = @id;",
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
                Guid modelId = new Guid(row["speciesId"].ToString());
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

        /// <summary>
        /// Persist the molecular entity to the database.
        /// </summary>
        public override void UpdateDatabase()
        {
            ObjectStatus currentStatus = this.__DBRow.Status;

            // (sfa) if the current command is a delete command, 
            // we have to delete the entity_name_lookup relationship first
            if (currentStatus == ObjectStatus.Delete)
            {
                deleteNames();
            }

            // ensure primary name exists
            EntityNameManager.AddName(Name);

            // save changes in this row
            base.UpdateDatabase();

            // remove old primary names
            if (nameDirty)
            {
                RemoveOldPrimaryNames(this.Name);
            }

            // (sfa) No need to update the names if the current command is a delete command
            // They must have already been deleted
            if (currentStatus != ObjectStatus.Delete)
            {
                updateNames();
            }
        }

        /// <summary>
        /// Update the database based on the names of the molecular entity
        /// </summary>
        private void updateNames()
        {
            // save changes in alternative names
            if (molecularEntityNames != null && molecularEntityNames.Count > 0)
            {
                foreach (ServerMolecularEntityName name in molecularEntityNames.Values)
                {
                    if (name.MolecularEntityId == Guid.Empty)
                    {
                        // initial insert - need to populate entity name ID's
                        name.MolecularEntityId = this.ID;
                    }
                    name.UpdateDatabase();
                }
            }
        }

        /// <summary>
        /// Delete the names of the molecular entity so that the molecular entity can be deleted. 
        /// </summary>
        private void deleteNames()
        {
            if (molecularEntityNames != null && molecularEntityNames.Count > 0)
            {
                foreach (ServerMolecularEntityName name in molecularEntityNames.Values)
                {
                    // delete the name from tables 'entity_name_lookups', and 'molecular_entity_names'
                    EntityNameManager.DeleteById(name.NameId);
                }
            }
        }

        #region Process Entities Relation
        /// <summary>
        /// Returns all processes this molecular entity is involved in
        /// </summary>
        /// <returns></returns>
        public ServerProcess[] GetAllProcesses()
        {
            return ServerProcessEntity.GetAllProcessesForEntity(this.ID);
        }

        /// <summary>
        /// Get all entries in the process_entities relation for this molecular entity
        /// </summary>
        /// <returns>
        /// an array of ServerProcessEntity objects
        /// </returns>
        public ServerProcessEntity[] GetAllProcessEntities()
        {
            return ServerProcessEntity.GetAllForEntity(this.ID);
        }

        /// <summary>
        /// Add this molecular entity to the given process
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="role"></param>
        /// <param name="quantity"></param>
        /// <param name="notes"></param>
        public void AddToProcess(Guid processId, string role, int quantity, string notes)
        {
            ServerProcessEntity.AddMolecularEntityToProcess(processId, this.ID, role, quantity, notes);
        }

        /// <summary>
        /// Remove this from the given
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="role"></param>
        public void RemoveFromProcess(Guid processId, string role)
        {
            ServerProcessEntity.RemoveMolecularEntityFromProcess(processId, this.ID, role);
        }

        #endregion

        #region Additional Queries
        /// <summary>
        /// Returns all of the pathways that this molecular entity is involved in.
        ///	All queries return pathways that involve certain processes (using the pathway_processes relation)
        ///	Depending on the entity type, those certain processes are:
        ///		- Basic Molecules:
        ///			processes that are related to the entity through the process_entities table
        ///		- Gene Products (proteins or rnas):
        ///			processes related to the gene product through the process_entities table AND those related
        ///				through the gene_products_and_processes table
        ///		- Genes:
        ///			processes related to gene products ( only using the gene_products_and_processes table )
        ///				that are encoded by the gene
        /// </summary>
        /// <returns></returns>
        public ServerPathway[] GetAllPathways()
        {
            switch (this.Type)
            {
                case "basic_molecules":
                    return ServerMolecularEntity.GetAllPathwaysForEntity(this.ID);
                case "proteins":
                case "rnas":
                    ServerPathway[] forGeneProd = ServerGeneProduct.GetAllPathwaysForGeneProduct(this.ID);
                    ServerPathway[] forEntity = ServerMolecularEntity.GetAllPathwaysForEntity(this.ID);

                    Hashtable pathwaysFound = new Hashtable();
                    ArrayList results = new ArrayList();

                    if (forGeneProd.Length > 0)
                    {
                        foreach (ServerPathway p in forGeneProd)
                        {
                            results.Add(p);
                            pathwaysFound.Add(p.ID, null);
                        }
                    }

                    if (forEntity.Length > 0)
                    {
                        foreach (ServerPathway p in forEntity)
                        {
                            if (!pathwaysFound.ContainsKey(p.ID))
                            {
                                results.Add(p);
                                pathwaysFound.Add(p.ID, null);
                            }
                        }
                    }

                    return (ServerPathway[])results.ToArray(typeof(ServerPathway));
                case "genes":
                    return ServerGene.GetAllPathwaysForGene(this.ID);
            }
            throw new Exception("Missing or invalid molecular entity type!");
        }

        /// <summary>
        /// Return all ServerSpecies for this molecule.
        /// </summary>
        /// <returns></returns>
        public ServerSpecies[] GetAllSpecies()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM Species S
				WHERE [id] IN ( SELECT speciesId
									FROM MapSpeciesMolecularEntities
									WHERE molecularEntityId = @id )
				ORDER BY (S.name + S.sbmlId);",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }

        /// <summary>
        ///		Returns an array of objects with the following properties:
        ///	ServerProcess Process: the process that this molecular entity is in
        ///	ServerPathway Pathway: the pathway that Process is in
        ///	string Role: the molecular entity's role in Process
        /// </summary>
        /// <returns></returns>
        public EntityRoleProcessAndPathway[] GetAllPathwaysProcessesAndRoles()
        {
            return ServerMolecularEntity.GetAllRolesProcessesAndPathwaysForEntity(this.ID);
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Deletes the molecular entity from table entity_name_lookups and molecular_entities
        /// </summary>
        public override void Delete()
        {
            // (sfa) Delete the entity_name_lookups tuples
            DBWrapper.Instance.ExecuteNonQuery("DELETE FROM entity_name_lookups WHERE entity_id = @id"
                , "@id", SqlDbType.UniqueIdentifier, this.ID);

            // Delete the molecular entity itself
            base.Delete();
        }

        /// <summary>
        /// Required function for settign up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            // add the INSERT command
            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, name, type_id, notes) VALUES (@id, @name, @type_id, @notes);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@name", SqlDbType.VarChar, Name,
                "@type_id", SqlDbType.TinyInt, TypeId,
                "@notes", SqlDbType.Text, MolecularEntityNotes);

            // add the SELECT command
            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            // add the UPDATE command
            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET name = @name, type_id = @type_id, notes = @notes WHERE id = @id;",
                "@name", SqlDbType.VarChar, Name,
                "@type_id", SqlDbType.TinyInt, TypeId,
                "@notes", SqlDbType.Text, MolecularEntityNotes,
                "@id", SqlDbType.UniqueIdentifier, ID);

            // add the DELETE command
            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

        }
        #endregion
        #endregion


        #region Static Methods
        /// <summary>
        /// Return all molecular entities from the system.
        /// </summary>
        /// <returns>
        /// Array of ServerMolecularEntity objects.
        /// </returns>
        public static ServerMolecularEntity[] AllMolecularEntities()
        {
            SqlCommand command = new SqlCommand(
                @"SELECT *
					FROM " + __TableName + @"
					WHERE id IN (
						SELECT DISTINCT me.id
							FROM " + __TableName + @" me )
					ORDER BY name");

            return ServerMolecularEntity.LoadMultiple(command);
            /*	DataSet[] ds = new DataSet[0];
                DBWrapper.LoadMultiple( out ds, ref command );

                ArrayList results = new ArrayList();
                foreach ( DataSet d in ds )
                {
                    // (BE) fill the array with instances of the derived classes
                    results.Add( LoadDerived( new DBRow(d) ) );
                }
		
                return ( ServerMolecularEntity[] ) results.ToArray( typeof( ServerMolecularEntity ) );
            */
        }

        /// <summary>
        /// Returns a select number of records from the molecular_entities table starting at
        /// the startRecord (in alphabetical order by name).  Used for paging.
        /// </summary>
        /// <param name="startRecord">
        /// the number of the first record returned
        /// </param>
        /// <param name="maxRecords">
        /// the maximum number of records to be returned
        /// </param>
        /// <returns></returns>
        public static ServerMolecularEntity[] AllMolecularEntities(int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
				FROM ( SELECT TOP " + maxRecords.ToString() + @" *
						FROM ( SELECT TOP " + bigNum.ToString() + @" *
								FROM " + __TableName + @"
								ORDER BY name ) " + __TableName + @"
						ORDER BY name DESC ) " + __TableName + @"
				ORDER BY name");

            DataSet[] ds = new DataSet[0];
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerMolecularEntity[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(LoadDerived(new DBRow(d)));
            }

            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        /// <summary>
        /// Count all of the molecular entites within the database.
        /// </summary>
        /// <returns>
        /// Integer of the count of the number of molecular entites within the database.
        /// </returns>
        public static int CountAllMolecularEntities()
        {
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        //TODO: implement with Join to entity_name_lookups
        //        public static ServerMolecularEntity[] AllMolecularEntitiesWithName(string name)
        //        {
        //        }


        /// <summary>
        /// Return a molecular entity with given ID.
        /// </summary>
        /// <param name="id">
        /// The Guid of the desired molecular entity.
        /// </param>
        /// <returns>
        /// SoapMolecularEntity object ready to be sent via SOAP.
        /// </returns>
        public static ServerMolecularEntity Load(Guid id)
        {
            //return new ServerMolecularEntity( LoadDataSet( id ) );
            return LoadDerived(LoadRow(id));
        }


        internal static ServerMolecularEntity[] LoadMultiple(SqlCommand command)
        {
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet r in ds)
                results.Add(ServerMolecularEntity.LoadDerived(new DBRow(r)));

            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }


        /// <summary>
        /// Loads a concrete instance of a derived class from the
        /// corresponding 
        /// </summary>
        /// <param name="molecularEntityRow"></param>
        /// <returns></returns>
        protected static ServerMolecularEntity LoadDerived(DBRow molecularEntityRow)
        {
            // (BE) since MolecularEntity is abstract, load the correct type instead
            switch (MolecularEntityTypeManager.GetTypeName(molecularEntityRow.GetInt("type_id")))
            {
                case "basic_molecules":
                    return ServerBasicMolecule.LoadFromBaseRow(molecularEntityRow);
                case "proteins":
                    return ServerGeneProduct.LoadFromBaseRow(molecularEntityRow);
                case "rnas":
                    return ServerGeneProduct.LoadFromBaseRow(molecularEntityRow);
                case "genes":
                    return ServerGene.LoadFromBaseRow(molecularEntityRow);
            }
            throw new Exception("Unexpected derived class type!");
        }

        /// <summary>
        /// Return the dataset for a pathway with a given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected static DBRow LoadRow(Guid id)
        {
            //BE: check if this is a graph node ID instead
            id = GraphNodeManager.GetEntityId(id); // if not a graph node ID, returns same ID

            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + " WHERE id = @id;");
            SqlParameter ident = new SqlParameter("@id", SqlDbType.UniqueIdentifier);
            ident.SourceVersion = DataRowVersion.Original;
            ident.Value = id;
            command.Parameters.Add(ident);

            DataSet ds = new DataSet();
            DBWrapper.LoadSingle(out ds, ref command);

            return new DBRow(ds);
        }

        /// <summary>
        /// Returns true if the given molecular entity 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exists(Guid id)
        {
            //BE: check if this is a graph node ID instead
            id = GraphNodeManager.GetEntityId(id); // if not a graph node ID, returns same ID

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, id);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }


        public static List<ServerMolecularEntity> SelectMolecularEntities(string fromClause, string whereClause,  /*string orderBy,*/ params object[] paramNameTypeValueList)
        {
            string meSubSelect = @"SELECT DISTINCT m.id
							FROM molecular_entities m " + (fromClause != null ? ", " + fromClause : "") + @"
							" + (whereClause != null ? " WHERE " + whereClause + " " : "");
            //+ (orderBy != null ? orderBy : "");


            SqlCommand commandMolecule = DBWrapper.BuildCommand(
                    @"SELECT m2.*
					FROM molecular_entities m2, (" + meSubSelect + @" ) AS uniqueM
					WHERE uniqueM.id = m2.id;",
                paramNameTypeValueList);

            DataSet[] ds;
            List<ServerMolecularEntity> results = new List<ServerMolecularEntity>();

            Dictionary<Guid, DBRow> basicMeRows = new Dictionary<Guid, DBRow>();
            Dictionary<Guid, DBRow> proteinMeRows = new Dictionary<Guid, DBRow>();
            Dictionary<Guid, DBRow> rnaMeRows = new Dictionary<Guid, DBRow>();
            Dictionary<Guid, DBRow> geneMeRows = new Dictionary<Guid, DBRow>();

            if (DBWrapper.LoadMultiple(out ds, ref commandMolecule) > 0)
            {
                foreach (DataSet d in ds)
                {
                    DBRow r = new DBRow(d);

                    switch (MolecularEntityTypeManager.GetTypeName(r.GetInt("type_id")))
                    {
                        case "basic_molecules": basicMeRows.Add(r.GetGuid("id"), r); break;
                        case "proteins": proteinMeRows.Add(r.GetGuid("id"), r); break;
                        case "rnas": rnaMeRows.Add(r.GetGuid("id"), r); break;
                        case "genes": geneMeRows.Add(r.GetGuid("id"), r); break;
                    }

                }
            }

            if (basicMeRows.Count > 0)
            {
                SqlCommand commandBasic = DBWrapper.BuildCommand(
                    @"SELECT m2.*
					FROM basic_molecules m2, ( " + meSubSelect + @" ) AS uniqueM
					WHERE uniqueM.id = m2.id and m2.id not in (select local_id from external_database_links);",
                    paramNameTypeValueList);

                DBWrapper.LoadMultiple(out ds, ref commandBasic);
                foreach (DataSet d in ds)
                {
                    DBRow r = new DBRow(d);
                    results.Add(new ServerBasicMolecule(r, basicMeRows[r.GetGuid("id")]));
                }
            }

            if (proteinMeRows.Count > 0 || rnaMeRows.Count > 0)
            {
                Dictionary<Guid, DBRow> geneProductRows = new Dictionary<Guid, DBRow>();

                SqlCommand commandGeneProduct = DBWrapper.BuildCommand(
                    @"SELECT m2.*
					FROM gene_products m2, ( " + meSubSelect + @" ) AS uniqueM
					WHERE uniqueM.id = m2.id and m2.id not in (select local_id from external_database_links);",
                    paramNameTypeValueList);

                DBWrapper.LoadMultiple(out ds, ref commandGeneProduct);
                foreach (DataSet d in ds)
                {
                    DBRow r = new DBRow(d);
                    geneProductRows.Add(r.GetGuid("id"), r);
                }

                if (proteinMeRows.Count > 0)
                {
                    SqlCommand commandProtein = DBWrapper.BuildCommand(
                        @"SELECT m2.*
					FROM proteins m2, ( " + meSubSelect + @" ) AS uniqueM
					WHERE uniqueM.id = m2.id and m2.id not in (select local_id from external_database_links);",
                        paramNameTypeValueList);

                    DBWrapper.LoadMultiple(out ds, ref commandProtein);
                    foreach (DataSet d in ds)
                    {
                        DBRow r = new DBRow(d);
                        results.Add(new ServerProtein(r, geneProductRows[r.GetGuid("id")], proteinMeRows[r.GetGuid("id")]));
                    }
                }

                if (rnaMeRows.Count > 0)
                {
                    SqlCommand commandRna = DBWrapper.BuildCommand(
                        @"SELECT m2.*
					    FROM rnas m2, ( " + meSubSelect + @" ) AS uniqueM
					    WHERE uniqueM.id = m2.id and m2.id not in (select local_id from external_database_links);",
                        paramNameTypeValueList);

                    DBWrapper.LoadMultiple(out ds, ref commandRna);
                    foreach (DataSet d in ds)
                    {
                        DBRow r = new DBRow(d);
                        results.Add(new ServerRNA(r, geneProductRows[r.GetGuid("id")], rnaMeRows[r.GetGuid("id")]));
                    }
                }
            }

            if (geneMeRows.Count > 0)
            {
                SqlCommand commandGene = DBWrapper.BuildCommand(
                    @"SELECT m2.*
					FROM genes m2, ( " + meSubSelect + @" ) AS uniqueM
					WHERE uniqueM.id = m2.id and m2.id not in (select local_id from external_database_links);",
                    paramNameTypeValueList);

                DBWrapper.LoadMultiple(out ds, ref commandGene);
                foreach (DataSet d in ds)
                {
                    DBRow r = new DBRow(d);
                    results.Add(new ServerGene(r, geneMeRows[r.GetGuid("id")]));
                }
            }

            return results;
        }

        #region Process Entities relation
        /// <summary>
        /// Gets all molecular entities involved in a process,
        /// will be used by ServerProcess object
        /// </summary>
        /// <remarks>
        /// This function is in this class because of access restrictions on LoadDerived()
        /// </remarks>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] GetAllEntitiesInProcess(Guid processId)
        {
            //            SqlCommand command = DBWrapper.BuildCommand(
            //                @"SELECT me.*
            //					FROM " + __TableName + @" me
            //					INNER JOIN process_entities pe ON me.id = pe.entity_id
            //					WHERE pe.process_id = @process_id
            //					ORDER BY me.name",
            //                "@process_id", SqlDbType.UniqueIdentifier, processId );

            //            ArrayList results = new ArrayList();
            //            DataSet[] ds;
            //            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            //            {
            //                foreach(DataSet d in ds)
            //                {
            //                    results.Add(ServerMolecularEntity.LoadDerived( new DBRow( d ) ) );
            //                }
            //            }
            //            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
            return SelectMolecularEntities("process_entities pe",
                "m.id = pe.entity_id AND pe.process_id = @process_id",
                //"ORDER BY m.name", 
                "@process_id", SqlDbType.UniqueIdentifier, processId).ToArray();
        }

        public static ServerMolecularEntity[] GetAllEntitiesInGenericProcess(Guid genericProcessId)
        {
            return SelectMolecularEntities("process_entities pe, processes p",
                "m.id = pe.entity_id AND pe.process_id = p.id AND p.generic_process_id = @generic_process_id",
                //"ORDER BY m.name",
                "@generic_process_id", SqlDbType.UniqueIdentifier, genericProcessId).ToArray();
        }

        #endregion

        #region Pathway Links relation

        /// <summary>
        /// Returns an array of the entities that link the given pathway to other pathways
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] GetLinkingEntitiesForPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT me2.* 
                  FROM molecular_entities me2,
                    (SELECT DISTINCT me.id
					FROM pathway_links pl
					INNER JOIN molecular_entities me ON pl.entity_id = me.id
					WHERE pl.pathway_id_1 = @pathway_id_1) AS me
                  WHERE me.id = me2.id;",
                "@pathway_id_1", SqlDbType.UniqueIdentifier, pathwayId);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach (DataSet d in ds)
                {
                    results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));
                }
            }
            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        /// <summary>
        /// Returns an array of the entities that this pathway contains
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] GetAllEntitiesForPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT me.* 
					FROM molecular_entities me
					WHERE me.id IN ( SELECT DISTINCT pe1.entity_id
										FROM process_entities pe1, catalyzes
										WHERE pe1.process_id IN (
											SELECT pp1.process_id
												FROM pathway_processes pp1
												WHERE pp1.pathway_id = @pathway_id )
									UNION
									SELECT DISTINCT gene_product_id AS entity_id
										FROM catalyzes
										WHERE process_id IN (
											SELECT pp2.process_id
												FROM pathway_processes pp2
												WHERE pp2.pathway_id = @pathway_id ) )
					ORDER BY me.name",
                "@pathway_id", SqlDbType.UniqueIdentifier, pathwayId);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach (DataSet d in ds)
                {
                    results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));
                }
            }
            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        /// <summary>
        /// Returns an array of the entities for the pathways 
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] GetAllEntitiesForPathways(string cspathwayIds)
        {
            ArrayList pathway = new ArrayList();
            char[] separator = { ',' };
            string[] pathwayIds = cspathwayIds.Split(separator);
            string ids = "(";
            foreach (string pathwayId in pathwayIds)
            {
                ids += "'" + pathwayId + "',";
            }
            ids = ids.Substring(0, ids.Length - 1);
            ids = ids + ")";
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT me.* 
					FROM molecular_entities me
					WHERE me.id IN ( SELECT DISTINCT pe1.entity_id
										FROM process_entities pe1, catalyzes
										WHERE pe1.process_id IN (
											SELECT pp1.process_id
												FROM pathway_processes pp1
												WHERE pp1.pathway_id IN " + ids + @")
									UNION
									SELECT DISTINCT gene_product_id AS entity_id
										FROM catalyzes
										WHERE process_id IN (
											SELECT pp2.process_id
												FROM pathway_processes pp2
												WHERE pp2.pathway_id IN " + ids + @"))
					ORDER BY me.name");

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach (DataSet d in ds)
                {
                    results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));
                }
            }
            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        public static ServerMolecularEntity[] GetAllSubstratesProductsForPathway(Guid pathwayId)
        {
            //Note that the query also eliminates the common molecules
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT me.* 
					FROM molecular_entities me
					WHERE me.id IN ( SELECT DISTINCT pe1.entity_id
										FROM process_entities pe1, catalyzes, process_entity_roles per
										WHERE pe1.process_id IN (
											    SELECT pp1.process_id
												FROM pathway_processes pp1
												WHERE pp1.pathway_id = @pathway_id )
                                            AND per.role_id = pe1.role_id 
                                            AND (per.name = 'substrate' OR per.name = 'product')
                                            AND pe1.entity_id NOT IN (SELECT id from common_molecules)
									     )
					ORDER BY me.name",
                "@pathway_id", SqlDbType.UniqueIdentifier, pathwayId);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach (DataSet d in ds)
                {
                    results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));
                }
            }
            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        /// <summary>
        /// Returns an array of molecular entities that two pathways share, but are not
        /// in their shared processes
        /// </summary>
        /// <param name="pathwayId1"></param>
        /// <param name="pathwayId2"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] GetExclusiveEntitiesForPathways(Guid pathwayId1, Guid pathwayId2)
        {
            //TODO: (BE) is this still correct???
            // (GJS) I believe it is, but it can be optimized
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT me.* 
					FROM molecular_entities me
					INNER JOIN ( SELECT DISTINCT prEnt.entity_id AS entity_id
									FROM process_entities prEnt
									INNER JOIN ( SELECT procE.entity_id AS entity_id
													FROM process_entities procE
													WHERE procE.process_id IN ( SELECT pappy.process_id
																					FROM pathway_processes pappy 
																					WHERE pappy.pathway_id = @pathwayId1 ) )
									AS special ON prEnt.entity_id = special.entity_id 
									WHERE prEnt.process_id IN ( SELECT pap.process_id 
																	FROM pathway_processes pap 
																	WHERE pap.pathway_id = @pathwayId2 )
									UNION SELECT DISTINCT gpp.gene_product_id AS entity_id 
									FROM catalyzes gpp
									INNER JOIN ( SELECT gpp.gene_product_id AS entity_id
													FROM catalyzes gpp
													WHERE gpp.process_id IN ( SELECT pappy.process_id
																				FROM pathway_processes pappy 
																				WHERE pappy.pathway_id = @pathwayId1 ) )
									AS pathway1GeneProd ON gpp.gene_product_id = pathway1GeneProd.entity_id
									WHERE gpp.process_id IN ( SELECT pap.process_id
																FROM pathway_processes pap 
																WHERE pap.pathway_id = @pathwayId2 ) )
					AS reallyBig ON me.[id] = reallyBig.entity_id
					WHERE reallyBig.entity_id NOT IN ( SELECT pe.entity_id
														FROM process_entities pe
														WHERE pe.process_id IN ( SELECT pp.process_id
																					FROM pathway_processes pp
																					INNER JOIN pathway_processes patpro ON pp.process_id = patpro.process_id
																					WHERE pp.pathway_id = @pathwayId1 AND patpro.pathway_id = @pathwayId2 )
														UNION SELECT gpp.gene_product_id
														FROM catalyzes gpp
														WHERE gpp.process_id IN ( SELECT pp.process_id
																					FROM pathway_processes pp
																					INNER JOIN pathway_processes patpro ON pp.process_id = patpro.process_id
																					WHERE pp.pathway_id = @pathwayId1 AND patpro.pathway_id = @pathwayId2 ) )
					ORDER BY me.[name];",
                "@pathwayId1", SqlDbType.UniqueIdentifier, pathwayId1,
                "@pathwayId2", SqlDbType.UniqueIdentifier, pathwayId2);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));
            }

            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        /// <summary>
        /// Return a list of molecule IDs for a given organism (or group) and pathway combination
        /// </summary>
        /// <param name="org"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] ByOrganismAndPathway(ServerOrganismGroup org, ServerPathway path, bool SubstratesProductsOnly)
        {
            SqlCommand command;
            if (SubstratesProductsOnly)
            { // Note that the query also eliminates the common molecules
                if (org == null)
                {
                    command = DBWrapper.BuildCommand(
                        @"SELECT m.*
						FROM molecular_entities m
						WHERE m.id IN (
							SELECT DISTINCT me.id
								FROM process_entities pe, pathway_processes pp,
									molecular_entities me, catalyzes c, process_entity_roles per
								WHERE me.id = pe.entity_id 
                                    AND pe.process_id = pp.process_id
									AND pp.pathway_id = @pathway_id 
                                    AND c.process_id = pp.process_id
                                    AND per.role_id = pe.role_id 
                                    AND (per.name = 'substrate' OR per.name = 'product')
                               )
                        AND me.id NOT IN (SELECT id from common_molecules)
						ORDER BY m.name",
                        "@pathway_id", SqlDbType.UniqueIdentifier, path.ID);
                }
                else
                {
                    string RequiredIds = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                    command = DBWrapper.BuildCommand(
                        @"SELECT m.*
						FROM molecular_entities m
						WHERE m.id IN (
							SELECT DISTINCT me.id
								FROM process_entities pe, pathway_processes pp,
									molecular_entities me, catalyzes c, process_entity_roles per
								WHERE me.id = pe.entity_id AND pe.process_id = pp.process_id
									AND pp.pathway_id = @pathway_id AND c.process_id = pp.process_id
                                    AND per.role_id = pe.role_id 
                                    AND (per.name = 'substrate' OR per.name = 'product')
									AND " + RequiredIds + @"	
                            )						
						ORDER BY m.name",
                        "@pathway_id", SqlDbType.UniqueIdentifier, path.ID);
                }
            }
            else
            {
                if (org == null)
                {
                    command = DBWrapper.BuildCommand(
                        @"SELECT m.*
						FROM molecular_entities m
						WHERE m.id IN (
							SELECT DISTINCT me.id
								FROM process_entities pe, pathway_processes pp,
									molecular_entities me, catalyzes c
								WHERE me.id = pe.entity_id AND pe.process_id = pp.process_id
									AND pp.pathway_id = @pathway_id AND c.process_id = pp.process_id
							UNION
							SELECT DISTINCT me.id
								FROM molecular_entities me, pathway_processes pp, catalyzes c
								WHERE me.id = c.gene_product_id AND pp.pathway_id = @pathway_id
									AND pp.process_id = c.process_id )
						ORDER BY m.name",
                        "@pathway_id", SqlDbType.UniqueIdentifier, path.ID);
                }
                else
                {
                    string RequiredIds = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                    command = DBWrapper.BuildCommand(
                        @"SELECT m.*
						FROM molecular_entities m
						WHERE m.id IN (
							SELECT DISTINCT me.id
								FROM process_entities pe, pathway_processes pp,
									molecular_entities me, catalyzes c
								WHERE me.id = pe.entity_id AND pe.process_id = pp.process_id
									AND pp.pathway_id = @pathway_id AND c.process_id = pp.process_id
									AND " + RequiredIds + @"
							UNION
							SELECT DISTINCT me.id
								FROM molecular_entities me, pathway_processes pp, catalyzes c
								WHERE me.id = c.gene_product_id AND pp.pathway_id = @pathway_id
									AND pp.process_id = c.process_id
									AND " + RequiredIds + @" )
						ORDER BY m.name",
                        "@pathway_id", SqlDbType.UniqueIdentifier, path.ID);
                }
            }

            return ServerMolecularEntity.LoadMultiple(command);
        }

        /// <summary>
        /// Return a list of molecule IDs for a given organism (or group) and a list of pathways
        /// </summary>
        /// <param name="org"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] ByOrganismAndPathways(ServerOrganismGroup org, string cspathwayIds, bool SubstratesProductsOnly)
        {
            char[] separator = { ',' };
            string[] pathwayIds = cspathwayIds.Split(separator);
            string ids = "(";
            foreach (string pathwayId in pathwayIds)
            {
                ids += "'" + pathwayId + "',";
            }
            ids = ids.Substring(0, ids.Length - 1);
            ids = ids + ")";
            SqlCommand command;
            if (SubstratesProductsOnly)
            { // Note that the query also eliminates the common molecules

                string RequiredIds = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                command = DBWrapper.BuildCommand(
                    @"SELECT m.*
					FROM molecular_entities m
					WHERE m.id IN (
						SELECT DISTINCT me.id
							FROM process_entities pe, pathway_processes pp,
								molecular_entities me, catalyzes c, process_entity_roles per
							WHERE me.id = pe.entity_id AND pe.process_id = pp.process_id
								AND pp.pathway_id IN " + ids + @" AND c.process_id = pp.process_id
                                AND per.role_id = pe.role_id 
                                AND (per.name = 'substrate' OR per.name = 'product')
								AND " + RequiredIds + @"	
                        )						
					ORDER BY m.name");
            }
            else
            {
                string RequiredIds = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                command = DBWrapper.BuildCommand(
                    @"SELECT m.*
					FROM molecular_entities m
					WHERE m.id IN (
						SELECT DISTINCT me.id
							FROM process_entities pe, pathway_processes pp,
								molecular_entities me, catalyzes c
							WHERE me.id = pe.entity_id AND pe.process_id = pp.process_id
								AND pp.pathway_id IN @ids AND c.process_id = pp.process_id
								AND " + RequiredIds + @"
						UNION
						SELECT DISTINCT me.id
							FROM molecular_entities me, pathway_processes pp, catalyzes c
							WHERE me.id = c.gene_product_id AND pp.pathway_id IN @ids
								AND pp.process_id = c.process_id
								AND " + RequiredIds + @" )
					ORDER BY m.name",
                    "@ids", SqlDbType.Text, ids);
            }

            return ServerMolecularEntity.LoadMultiple(command);
        }
        /// <summary>
        /// Return all molecular entities associated with the current organism or group
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] AllMolecularEntitiesForOrganism(ServerOrganismGroup org)
        {
            return AllMolecularEntitiesForOrganism(org.ID);
        }

        /// <summary>
        /// Find all molecular entities associated w/ a particular organism
        /// </summary>
        /// <param name="orgID"></param>
        /// <returns>A list of ServerMolecularEntity objects containing molecules, rnas, ...</returns>
        public static ServerMolecularEntity[] AllMolecularEntitiesForOrganism(Guid orgID)
        {
            string RequiredIds = ServerOrganismGroup.RequireIdInList(ServerOrganismGroup.GetAllChildrenAndParent(orgID), "c", "organism_group_id");
            // Is this correct still..?
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT me.*
					FROM molecular_entities me
					WHERE me.id IN (
						SELECT pe.entity_id
							FROM catalyzes c, process_entities pe, molecular_entities me
							WHERE " + RequiredIds + @" AND c.process_id = pe.process_id )
					ORDER BY me.name");

            return ServerMolecularEntity.LoadMultiple(command);
        }

        /// <summary>
        /// Find all molecular entities associated with all organisms
        /// </summary>
        /// <returns>A list of ServerMolecularEntity objects containing molecules, rnas, ...</returns>
        public static ServerMolecularEntity[] AllMolecularEntitiesForAllOrganisms()
        {
            string RequiredIds = ServerOrganismGroup.RequireIdInList(ServerOrganismGroup.AllOrganismGroups(), "c", "organism_group_id") + " OR ";
            RequiredIds = RequiredIds.Substring(0, RequiredIds.Length - " OR ".Length);
            // This may not be 100% correct...
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
					FROM molecular_entities
					WHERE id IN (
						SELECT DISTINCT gene_product_id
							FROM catalyzes
							WHERE organism_group_id IN (SELECT id FROM organism_groups)
						UNION
						SELECT DISTINCT entity_id FROM process_entities)
					ORDER BY name");

            //				@"SELECT me.*
            //					FROM molecular_entities me
            //					WHERE me.id IN (
            //						SELECT pe.entity_id
            //							FROM catalyzes c, process_entities pe, molecular_entities me
            //							WHERE " + RequiredIds + @" AND c.process_id = pe.process_id )
            //					ORDER BY me.name");

            return ServerMolecularEntity.LoadMultiple(command);
        }

        #endregion

        #region Additional Queries

        /// <summary>
        /// Returns all pathways that involve processes that are related to the entity 
        ///	through the process_entities table
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public static ServerPathway[] GetAllPathwaysForEntity(Guid entityId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT p.*
					FROM pathways p
					WHERE p.id IN ( SELECT pp.pathway_id
									FROM pathway_processes pp
									INNER JOIN process_entities pe ON pp.process_id = pe.process_id
									WHERE pe.entity_id = @entity_id );",
                "@entity_id", SqlDbType.UniqueIdentifier, entityId);

            ArrayList results = new ArrayList();
            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
            {
                foreach (DataSet d in ds)
                {
                    results.Add(new ServerPathway(new DBRow(d)));
                }
            }
            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }


        /// <summary>
        /// returns all the pathways, processes, and roles for the given molecular entity.
        /// Should work for gene products as well, I gave them the role of 'enzyme'
        /// </summary>
        /// <param name="molecularEntityId"></param>
        /// <returns></returns>
        public static EntityRoleProcessAndPathway[] GetAllRolesProcessesAndPathwaysForEntity(Guid molecularEntityId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT pp.pathway_id AS pathwayId, pe.process_id AS processId, per.name AS role, pat.[name] AS pathwayName 
                    FROM pathway_processes pp, process_entities pe, pathways pat, process_entity_roles per
                    WHERE pp.process_id = pe.process_id AND pp.pathway_id = pat.[id] AND pe.entity_id = @entity_id AND pe.role_id = per.role_id
                UNION SELECT DISTINCT pp.pathway_id AS pathwayId, gpp.process_id AS processId, 'enzyme' AS role, pat.[name] AS pathwayName 
                    FROM pathway_processes pp, catalyzes gpp, pathways pat 
                    WHERE pp.process_id = gpp.process_id AND pp.pathway_id = pat.[id] AND gpp.gene_product_id = @entity_id                     
                UNION SELECT DISTINCT '" + Utilities.Util.NullGuid + @"' AS pathwayId, pe.process_id AS processId, per.name AS role, 'N/A' AS pathwayName 
                    FROM process_entities pe, process_entity_roles per
                    WHERE pe.entity_id = @entity_id AND pe.role_id = per.role_id
                    AND pe.process_id NOT IN
                        (SELECT process_id from pathway_processes) 
                UNION SELECT DISTINCT '" + Utilities.Util.NullGuid + @"' AS pathwayId, gpp.process_id AS processId, 'enzyme' AS role, 'N/A' AS pathwayName 
                    FROM catalyzes gpp
                    WHERE gpp.gene_product_id = @entity_id 
                    AND gpp.process_id NOT IN
                        (SELECT process_id from pathway_processes)
                    ORDER BY pathwayName;",
                "@entity_id", SqlDbType.UniqueIdentifier, molecularEntityId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new EntityRoleProcessAndPathway(new DBRow(d)));
            }

            return (EntityRoleProcessAndPathway[])results.ToArray(typeof(EntityRoleProcessAndPathway));
        }

        /// <summary>
        /// Returns all molecular entities whose name contains the given substring
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] FindMolecularEntities(string substring, SearchMethod searchMethod)
        {
            return FindMolecularEntities(substring, searchMethod, -1);
        }

        /// <summary>
        /// Returns all molecular entities whose name contains the given substring
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="searchMethod"></param>
        /// <param name="topk"></param>
        /// <returns></returns>
        public static ServerMolecularEntity[] FindMolecularEntities(string substring, SearchMethod searchMethod, int topk)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string topkClause = "";
            if (topk > 0)
                topkClause = " TOP " + topk + " ";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT " + topkClause + " * FROM " + __TableName + " WHERE [name] " +
                (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") +
                " @substring and id not in (select local_id from external_database_links) ORDER BY [name];",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));
            }

            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }

        /// <summary>
        /// Paged search function.
        /// </summary>
        /// <param name="substring">
        /// The substring we're searching for.
        /// </param>
        /// <param name="searchMethod">
        /// The search method to use.
        /// </param>
        /// <param name="startRecord">
        /// The first record requested in the returned set.
        /// </param>
        /// <param name="maxRecords">
        /// The maximum number of records to return.
        /// </param>
        /// <param name="section">
        /// The sub-type of Molecular Entity in which to search.
        /// </param>
        /// <returns>
        /// A page of molecular entities matching the search criteria.
        /// </returns>
        public static ServerMolecularEntity[] FindMolecularEntities(string substring, SearchMethod searchMethod, int startRecord, int maxRecords, string section, bool filterBySpecies)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string whichType = "";
            string geneFilter = "molecular_entities me";
            switch (section)
            {
                case "BrowserBasicMolecules": whichType = "basic_molecules"; break;
                case "BrowserProteins": whichType = "proteins"; break;
                case "BrowserGenes": whichType = "genes"; break;
            }

            string mappingFilter = "";
            if (filterBySpecies)
            {
                mappingFilter = @" AND [id] IN
                          (SELECT molecularEntityId FROM MapSpeciesMolecularEntities)";
                whichType = "basic_molecules";
            }
            int bigNum = startRecord + maxRecords;

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT *
					FROM (
						SELECT TOP " + maxRecords.ToString() + @" *
							FROM (
								SELECT TOP " + bigNum.ToString() + @" me.*
									FROM " + geneFilter + @"
									WHERE name " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
										AND me.type_id = @type" + mappingFilter + @"
									ORDER BY name )	" + __TableName + @"
							ORDER BY name DESC ) " + __TableName + @"
					WHERE   id IN (
						            SELECT DISTINCT me.id
							        FROM " + __TableName + @" me ) and
                            id not in (select local_id from external_database_links)
					ORDER BY name",
                "@substring", SqlDbType.VarChar, substring,
                "@type", SqlDbType.TinyInt, MolecularEntityTypeManager.GetTypeId(whichType));

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();

            foreach (DataSet d in ds)
                results.Add(ServerMolecularEntity.LoadDerived(new DBRow(d)));

            return (ServerMolecularEntity[])results.ToArray(typeof(ServerMolecularEntity));
        }


        /// <summary>
        /// The count of molecular entites that would respond to the provided search conditions.
        /// </summary>
        /// <param name="substring">
        /// The substring we're searching for.
        /// </param>
        /// <param name="searchMethod">
        /// The serach method to use.
        /// </param>
        /// <param name="section">
        /// The sub-section of MolecularEntities to search in.
        /// </param>
        /// <returns>
        /// The count of items that respond to the search conditions.
        /// </returns>
        public static int CountFindMolecularEntities(string substring, SearchMethod searchMethod, string section, bool filterBySpecies)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            string whichType = "";
            switch (section)
            {
                case "BrowserBasicMolecules": whichType = "basic_molecules"; break;
                case "BrowserProteins": whichType = "proteins"; break;
                case "BrowserGenes": whichType = "genes"; break;
            }

            string mappingFilter = "";
            if (filterBySpecies)
            {
                mappingFilter = @" AND [id] IN
                          (SELECT molecularEntityId FROM MapSpeciesMolecularEntities);";
                whichType = "basic_molecules";
            }
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT COUNT(*)
					FROM " + __TableName + @"
					WHERE name " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=")
                        + @" @substring AND type_id = @type AND id IN (
						SELECT DISTINCT me.id
							FROM " + __TableName + @" me ) and id not in (select local_id from external_database_links) " + mappingFilter,
                "@substring", SqlDbType.VarChar, substring,
                "@type", SqlDbType.Int, MolecularEntityTypeManager.GetTypeId(whichType));

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }


        #endregion

        #endregion

    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerMolecularEntityclass.cs,v 1.1 2009/07/05 22:17:06 rishi Exp $
	$Log: ServerMolecularEntityclass.cs,v $
	Revision 1.1  2009/07/05 22:17:06  rishi
	*** empty log message ***
	
	Revision 1.5  2009/05/27 14:35:49  ali
	*** empty log message ***
	
	Revision 1.4  2009/05/19 13:57:19  ali
	*** empty log message ***
	
	Revision 1.3  2009/05/19 13:22:25  ali
	*** empty log message ***
	
	Revision 1.2  2008/06/17 15:38:46  akaraca
	graph drawing coloring information is now sent to the client
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.18  2008/05/09 18:33:25  divya
	*** empty log message ***
	
	Revision 1.17  2008/05/01 17:53:29  divya
	*** empty log message ***
	
	Revision 1.16  2008/05/01 02:20:24  divya
	*** empty log message ***
	
	Revision 1.15  2008/04/03 17:11:48  ali
	*** empty log message ***
	
	Revision 1.14  2008/03/07 19:53:13  brendan
	AQI refactoring
	
	Revision 1.13  2007/09/16 07:06:56  chirag
	dataset bug fixes
	
	Revision 1.12  2007/06/01 16:05:29  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.11  2007/05/18 19:26:41  brendan
	Allow objects to be created with specified GUID's by setting the ID in the Soap object
	
	Revision 1.10  2007/04/09 17:14:31  ali
	*** empty log message ***
	
	Revision 1.9  2007/04/06 18:09:25  chirag
	work on autocomplete
	
	Revision 1.8  2007/02/07 00:02:17  brendan
	*** empty log message ***
	
	Revision 1.7  2006/10/25 06:12:39  gokhan
	*** empty log message ***
	
	Revision 1.6  2006/10/25 01:13:45  pathwaysdeploy
	*** empty log message ***
	
	Revision 1.5  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.4  2006/10/03 22:35:14  gokhan
	*** empty log message ***
	
	Revision 1.3  2006/10/03 17:47:44  brendan
	*** empty log message ***
	
	Revision 1.2  2006/09/07 18:07:39  ali
	enzymes, and common molecules are eliminated from the molecular entity drop down box in neighborhood and path query interfaces.
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.56  2006/07/12 15:39:22  greg
	 - JavaScript paging
	Clicking between pages in the browser is now JavaScript-based and super-duper fast.  Searching works correctly, paging works correctly, and clicking on links in the main content page maintains results correctly, too.  Coolness.  This is the way the game was meant to be played.
	
	 - Improved JavaScript for older stuff
	Older panel-collapsing JavaScript has been updated to remove some bulk and to improve efficiency.  For example, the GO viewer window can no longer hang when switching GO terms (though it's still possible to reference regions that don't exist... though that's not really too much of a problem).
	
	 - CSS reorganization
	The main CSS file was getting really yucky, so I straightened a lot of it up so it's a lot easier to find what you're looking for.  At least, it is for me, anyway.
	
	Revision 1.55  2006/07/09 15:22:09  greg
	 - Browser highlighting issues
	Apparently I hadn't fixed the highlighting issues like I thought I had, but it seems to be working fine now.
	
	 - Stuff autoloading that probably shouldn't
	All the ME data for processes (and perhaps other things) were automatically loading, even for items that weren't open.  I didn't really notice this until I started testing the browser with the Kegg data; since Kegg has a zillion more entries than our own database, the performance hit could only be realized when trying to load up extraneous information for hundreds of items at once.  Woof.
	
	 - 0x80040111 (NS_ERROR_NOT_AVAILABLE) error
	Apparently, certain kinds of browsing patterns may interrupt Ajax requests in a way Firefox doesn't like, thus causing this error to be thrown.  I resolved it with a try/catch block, but I'll keep my oeyes open for any additional fishiness.
	
	 - Built-in queries updates
	All built-in queries now include a link to jump to the graph visualization, which is useful when there are hundreds of results and it's not immediately clear that there is a graph to see.  Item pre-selection is fixed for several queries, but not all of them; Brandon and I are pumping those out as quickly as we can.
	
	Revision 1.54  2006/07/07 19:28:04  greg
	The bulk of this update focuses on integrating Ajax browsing into the content browser bar on the left.  It currently only works from the pathways dropdown option, but the framework is now in place for the other lists to function in the same manner.
	
	Revision 1.53  2006/06/20 17:55:22  greg
	This should take care of some more of those process queries... they just need to be thoroughly tested now.
	
	Revision 1.52  2006/06/19 20:59:18  greg
	All of the built-in queries appear to return the correct results in all cases now, but we'll still want some more thorough testing done eventually.  The GO pathway viewer tools should now work correctly, and menu items on the content bar on the left are now displayed in bold when they're what's on the screen (though there are still a few cases where this doesn't work 100% correctly; I'm trying to figure out how to address that).
	
	Revision 1.51  2006/06/16 15:19:25  greg
	Just some tune-ups... nothing major.
	
	Revision 1.50  2006/06/16 10:23:35  greg
	This update handles another handful of appearance-related issues, particularly those concerning IE, and also addresses some issues with built-in queries.  At this time, however, the queries still need to be more thoroughly tested.
	
	Revision 1.49  2006/06/15 23:06:56  ali
	*** empty log message ***
	
	Revision 1.48  2006/06/15 00:36:38  greg
	The GO pathway viewer should be almost completely operational now; any issues should be relatively minor and easily fixable.  Substantial XHTML/CSS updates were done to make more pages compatable with both IE and non-IE browsers, and some spelling/grammar updates were made.  There are still some known issues with content displaying funkily in IE that will be addressed soon.
	
	Revision 1.47  2006/06/02 21:50:44  brandon
	The two Molecular Entity queries are done and work correctly, although I couldn't test the graph drawing on my machine.  Added some queries to ServerMolecularEntity, ServerOrganismGroup, and MolecularEntityQueries
	
	Revision 1.46  2006/06/02 16:10:15  greg
	All built-in queries except the bottom three on the list (on the main website) should work correctly now.  However, there is still an issue communicating with the Java viewer; it appears as if these new queries are not passing the viewer the correct organism information.  This causes the graph to start off with no organism selected, but the links are still all correct.
	
	ServerObjects have also been updated to accomodate for the new queries.
	
	The HyperGraph class has also been moved over from the old service.  It seems like it may be a bit clunky (as with all the old query code), but it should work properly at least.  In the future perhaps we can streamline the code a bit to make it more efficient/cleaner.
	
	Revision 1.45  2006/05/23 18:29:32  greg
	Many old SQL queries were updated/optimized, and a bug that causes the system to crash when trying to navigate after viewing the details of an object through the Java applet is also fixed.  This required some semi-substantial modifications to LinkHelper.cs and SearchPagination.ascx.cs to allow for a slightly different method of dealing with query parameters.
	
	Revision 1.44  2006/05/18 19:01:47  greg
	 - Pathway menu expansion
	When going through the pathways menu in the control bar, the menu that should be open isn't.  Now it is.
	
	 - SQL injection stuff
	More queries were rewritten to prevent SQL injection, and some were also rewritten to be a little more aesthetically pleasing.
	
	 - Finding SQL bugs
	SQL bugs that have arisen as a result of schema changes are still being checked for.  Any that I find are being sent to Ali for rewriting and reintegration.
	
	Revision 1.43  2006/05/17 16:26:04  greg
	 - Search pagination errors
	Trying to access pages that are out-of-bounds results in SQL exceptions.  Additionally, the pagination function seemed to generate more pages than it should have at times, specifically when viewing the last page.  These are both resolved, which also seems to fix the "xx" bug.
	
	 - Potential SQL injection issue
	Search terms were not checked for SQL injection attacks.  Many SQL queries in the different server classes were rewritten to use parameters and thus eliminate the potential issue of SQL injection.
	
	 - Interface enhancement
	The search-type dropdown box now reverts to the last-selected option when the page reloads; prior to that it always went to the top.
	
	 - User input validation
	Most (if not all) user input was not being validated on entry, meaning it was possible to perform cross-site scripting and all that kind of nasty stuff.  Input is now stripped of HTML tags.  Note that validateRequest is now turned off, so all user input has to be validated in this way.
	
	Revision 1.42  2006/05/11 21:18:33  brendan
	Fixed numerous bugs, basic browsing starting to work again
	
	Revision 1.41  2006/05/11 15:47:20  brendan
	Removed remaining SearchMethod magic strings; refactored OrgMeta back into ServerOrganismGroup, fixed many PathwaysLib warnings.
	
	Revision 1.40  2006/05/10 02:40:17  ali
	Server Objects have been modified to make them compliant with the recent schema changes concerning the addition of several entity_type tables into the database.
	
	Revision 1.39  2006/04/12 20:24:43  brian
	*** empty log message ***
	
	Revision 1.38.8.2  2006/04/10 22:17:25  brian
	Aside from a strange duplicate entry bug, all pre-query dropdowns appear to be working now
	
	Revision 1.38.8.1  2006/04/07 19:29:27  brian
	Fixed several "pre-queries".  Not too happy w/ some of the solutions, but I'll come up w/ something better later
	
	Revision 1.38  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.37  2005/10/31 20:27:45  fatih
	*** empty log message ***
	
	Revision 1.36  2005/10/31 06:10:17  fatih
	*** empty log message ***
	
	Revision 1.35  2005/10/31 00:39:36  fatih
	*** empty log message ***
	
	Revision 1.34  2005/10/28 21:13:21  michael
	blah
	
	Revision 1.33  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.32  2005/10/21 21:43:16  michael
	*** empty log message ***
	
	Revision 1.31  2005/10/20 18:36:36  michael
	*** empty log message ***
	
	Revision 1.30  2005/10/13 21:27:44  michael
	*** empty log message ***
	
	Revision 1.29  2005/08/19 21:33:42  brandon
	cleaned up some files, added some comments
	
	Revision 1.28  2005/08/04 01:29:59  michael
	Debugging search and pagination
	
	Revision 1.27  2005/08/03 19:49:17  brandon
	added a paged find function to server molecular entity
	
	Revision 1.26  2005/08/03 05:31:17  michael
	Working on searh and results/display pagination.
	
	Revision 1.25  2005/08/01 16:32:31  brandon
	added "ORDER BY name" clause to the All... and Find... functions in the server objects
	
	Revision 1.24  2005/07/22 16:42:35  brandon
	modified the query for getting exclusive molecular entities for pathways to include gene products
	
	Revision 1.23  2005/07/21 18:45:50  brandon
	can get Pathways, roles, and processes for Gene products now
	
	Revision 1.22  2005/07/21 16:35:15  brandon
	Fixed the GetExclusiveEntitiesForPathways (i think that's what i called it) query.  It's really long.
	
	Revision 1.21  2005/07/20 18:02:19  brandon
	added function to ServerPathway: GetConnectedPathways ( ), which returns an array of ConnectedPathwayAndCommonProcesses objects.  This new object has three properties:
	ServerPathway ConnectedPathway- (to be listed as a connected pathway)
	ServerProcess[] SharedProcesses - (shared by two pathways)
	ServerMolecularEntity[] SharedExclusiveMolecules - (molecules shared
	by two pathways but are not included in any process in SharedProcesses)
	
	Revision 1.20  2005/07/20 04:05:34  brandon
	Added the class EntityRoleProcessAndPathway which is used by ServerMolecularEntity.
	Fixed a bug in GetAllGenes() in ServerGeneProduct.cs
	Created ConnectedPathways.cs to help with pathway links, but it doesn't work yet.
	
	Revision 1.19  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.18  2005/07/18 20:31:13  brandon
	changed GetAllPathways in ServerMolecularEntity.cs to work for all types of molecular entity.  Added a static method to ServerGene to get all pathways for gene products that the gene encodes
	
	Revision 1.17  2005/07/18 19:18:12  brandon
	Added another test file, Brendan fixed his Protein table fix thing, and added a query to geneProducts to override the GetAllPathways in ServerMolecularEntity
	
	Revision 1.16  2005/07/18 17:23:39  brandon
	Added queries:
	1. get all pathways that a given molecular entity is involved in
	2. get all pathways that a given pathway is linked to, as well as the molecular entity they have in common
	
	Revision 1.15  2005/07/15 22:28:00  brendan
	Fix to automatically generate missing protein table entries from entries in molecular entities.
	
	Revision 1.14  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.13  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
	Revision 1.12  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.11  2005/07/11 21:01:52  brendan
	Inheritance now working for Gene/MolecularEntity and the ServerObjectInheritance tests completes successfully.
	
	Revision 1.10  2005/07/11 16:54:39  brandon
	Added ServerProcessEntity and Soap...  for the process_entities relation.  Added funtion GetAllProcesses in ServerMolecularEntity, but GetAllEntities won't work, maybe because the ServerMolecularEntity constructor is protected.  Haven't done any testing yet.
	
	Revision 1.9  2005/07/08 21:55:07  brendan
	Debugging MolecularEntity/EntityNames/Gene inheritance.  Inheritance test not passing yet.
	
	Revision 1.8  2005/07/07 23:30:51  brendan
	Work in progress on entity names.  MolecularEntityName virtually complete, but not tested.
	
	Revision 1.7  2005/07/01 20:47:39  brendan
	Work on inheritance & the object wrapper.
	
	Revision 1.6  2005/06/29 22:06:10  brendan
	Working on adding support for inheritance between MolecularEntity and Gene.
	
	Revision 1.5  2005/06/29 16:44:53  brandon
	Added Insert, Update, and Delete support to these files if they didn't already have it
	
	Revision 1.4  2005/06/28 23:20:25  brendan
	Fixed Gene, MolecularEntity, Organism, and Process to reflect recent wrapper object design changes.
	
	Revision 1.3  2005/06/27 15:44:22  brandon
	revised ServerOrganism.cs to the new format, not sure when to use 'organism_notes' vs. 'notes'
	
	Revision 1.2  2005/06/24 21:57:32  brendan
	Checkin of work on database object design.
	
	Revision 1.1  2005/06/23 21:54:42  brandon
	two new files, see if inheritance works
	

------------------------------------------------------------------------*/
#endregion
