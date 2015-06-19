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
#endregion

namespace PathwaysLib.ServerObjects
{

    public class ServerReaction : ServerSbase, IGraphReaction
    {

        #region Constructor, Destructor, ToString
        private ServerReaction()
        {
        }

        /// <summary>
        /// Constructor for server Reaction wrapper with fields initiallized
        /// </summary>

        //public ServerReaction(Guid modelId, string sbmlId, string name, bool reversible, bool fast, Guid kineticLawId)
        //{
        //    // not yet in DB, so create empty row
        //    __ReactionRow = new DBRow(__TableName);

        //    this.ID = DBWrapper.NewID(); // generate a new ID
        //    this.ModelId = modelId;
        //    this.SbmlId = sbmlId;
        //    this.Name = name;
        //    this.Reversible = reversible;
        //    this.Fast = fast;
        //    this.KineticLawId = kineticLawId;
        //}


        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapReaction object.
        /// </remarks>
        public ServerReaction(SoapReaction data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __ReactionRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __ReactionRow = LoadRow(data.ID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

        }

        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerReaction(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __ReactionRow = data;

        }

        public ServerReaction(DBRow reactionRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __ReactionRow = reactionRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerReaction()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "Reaction";
        protected DBRow __ReactionRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __ReactionRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __ReactionRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __ReactionRow.GetGuid("modelId");
            }
            set
            {
                __ReactionRow.SetGuid("modelId", value);
            }
        }
        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __ReactionRow.GetString("sbmlId");
            }
            set
            {
                __ReactionRow.SetString("sbmlId", value);
            }
        }


        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __ReactionRow.GetString("name");
            }
            set
            {
                __ReactionRow.SetString("name", value);
            }
        }

        /// <summary>
        /// Get/set the reversible.
        /// </summary>
        public bool Reversible
        {
            get
            {
                return __ReactionRow.GetBool("reversible");
            }
            set
            {
                __ReactionRow.SetBool("reversible", value);
            }
        }

        /// <summary>
        /// Get/set the fast.
        /// </summary>
        public bool Fast
        {
            get
            {
                return __ReactionRow.GetBool("fast");
            }
            set
            {
                __ReactionRow.SetBool("fast", value);
            }
        }


        /// <summary>
        /// Get/set the kineticLawId.
        /// </summary>
        public Guid KineticLawId
        {
            get
            {
                return __ReactionRow.GetGuid("kineticLawId");
            }
            set
            {
                __ReactionRow.SetGuid("kineticLawId", value);
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __ReactionRow.UpdateDatabase();
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
            SoapReaction retval = (derived == null) ?
                retval = new SoapReaction() : retval = (SoapReaction)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.Reversible = this.Reversible;
            retval.Fast = this.Fast;
            retval.KineticLawId = this.KineticLawId;

            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerModel
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the Model relation.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapReaction c = o as SoapReaction;
            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelId = c.ModelId;
            this.SbmlId = c.SbmlId;
            this.Name = c.Name;
            this.Reversible = c.Reversible;
            this.Fast = c.Fast;
            this.KineticLawId = c.KineticLawId;
        }

        /// <summary>
        /// Gets all species for the reaction
        /// </summary>
        public ServerReactionSpecies[] GetAllSpecies()
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT spe.*
				FROM ReactionSpecies spe
				WHERE spe.[reactionId] = @id 
				ORDER BY spe.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));

            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));

        }

        /// <summary>
        /// Gets all species for the reaction
        /// </summary>
        public ServerReactionSpecies[] GetAllSpeciesIdWithName()
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT spe.Id, spe.ReactionId, spe.SpeciesId, spe.RoleId, spe.stoichiometry, s.Name
				FROM ReactionSpecies spe, Species s
				WHERE spe.speciesId = s.ID AND spe.[reactionId] = @id 
				ORDER BY spe.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));

            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));

        }


        /// <summary>
        /// Gets all reactants for the reaction
        /// </summary>
        public ServerSpecies[] GetAllReactants()
        {
            ServerReactionSpecies[] all = GetAllSpecies();
            ArrayList results = new ArrayList();
            foreach (ServerReactionSpecies srs in all)
            {
                if (srs.RoleId == 1)
                {
                    ServerSpecies sr = ServerSpecies.Load(new Guid(srs.SpeciesId.ToString()));
                    results.Add(sr);
                }

            }
            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
 
        }

        /// <summary>
        /// Gets all products for the reaction
        /// </summary>
        public ServerSpecies[] GetAllProducts()
        {
            ServerReactionSpecies[] all = GetAllSpecies();
            ArrayList results = new ArrayList();
            foreach (ServerReactionSpecies srs in all)
            {
                if (srs.RoleId == 2)
                {
                    ServerSpecies sr = ServerSpecies.Load(new Guid(srs.SpeciesId.ToString()));
                    results.Add(sr);
                }

            }
            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));

        }

        /// <summary>
        /// Gets all modifiers for the reaction
        /// </summary>
        public ServerSpecies[] GetAllModifiers()
        {
            ServerReactionSpecies[] all = GetAllSpecies();
            ArrayList results = new ArrayList();
            foreach (ServerReactionSpecies srs in all)
            {
                if (srs.RoleId == 3)
                {
                    ServerSpecies sr = ServerSpecies.Load(new Guid(srs.SpeciesId.ToString()));
                    results.Add(sr);
                }

            }
            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));

        }
        /// <summary>
        /// Gets all compartments to which the reaction's species belong
        /// </summary>
        public ServerCompartment[] GetAllCompartments()
        {
            ServerReactionSpecies[] all = GetAllSpecies();
            ArrayList results = new ArrayList();
            foreach (ServerReactionSpecies srs in all)
            {
                bool flag = true;
                ServerSpecies spe = ServerSpecies.Load(srs.SpeciesId);
                ServerCompartment scomp = ServerCompartment.Load(spe.CompartmentId);
                foreach (ServerCompartment rcomp in results)
                {
                    if (rcomp.ID == scomp.ID)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    results.Add(scomp);
            }
            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));           
        }

        public static int CountFindReactions(string substring, SearchMethod searchMethod)
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

        public static ServerReaction[] FindReactions(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)  
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM " + __TableName + @"
											WHERE [name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @" @substring
											ORDER BY [name] ) " + __TableName + @"
									ORDER BY [name] DESC ) " + __TableName + @"
				ORDER BY [name]",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));
            }

            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }
        public ServerProcess[] GetAllProcesses()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                      @"select p.*
                from  mapreactionsprocessentities mp,Processes p
                where mp.reactionId = @mid	and mp.processId = p.Id	                
                ", "@mid", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerProcess(new DBRow(d)));
            }

            return (ServerProcess[])results.ToArray(typeof(ServerProcess));
        }

        public ServerModel[] GetAllModels()
        {
            ArrayList results = new ArrayList();
            results.Add(ServerModel.Load(this.ModelId));
            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        public string GetRole(ServerSpecies sr)
        {
            string role = "unknown";
            ServerReactionSpecies[] reactionspecies = this.GetAllSpecies();
            foreach (ServerReactionSpecies reactionspe in reactionspecies)
            {
                if (reactionspe.SpeciesId == sr.ID)
                {
                    role = ServerReactionSpeciesRole.Load(reactionspe.RoleId).Role;
                    break;
                }
            }
            return role;
        }
        public ServerParameter[] GetAllParameters()
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT par.*
				FROM Parameter par
				WHERE par.[reactionId] = @id 
				ORDER BY par.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerParameter(new DBRow(d)));

            }

            return (ServerParameter[])results.ToArray(typeof(ServerParameter));

        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __ReactionRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name, reversible, fast, kineticLawId) VALUES (@id, @modelId, @sbmlId, @name, @reversible, @fast, @kineticLawId);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@reversible", SqlDbType.Bit, Reversible,
                "@fast", SqlDbType.Bit, Fast,
                "@kineticLawId", SqlDbType.UniqueIdentifier, KineticLawId);

            __ReactionRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ReactionRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId , sbmlId = @sbmlId , name = @name , reversible = @reversible , fast = @fast , kineticLawId = @kineticLawId where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@reversible", SqlDbType.Bit, Reversible,
                "@fast", SqlDbType.Bit, Fast,
                "@kineticLawId", SqlDbType.UniqueIdentifier, KineticLawId,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ReactionRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Reactions from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapReaction objects ready to be sent via SOAP.
        /// </returns>
        public static ServerReaction[] AllModels()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

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
        /// Returns a single ServerReaction object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerReaction Load(Guid id)
        {
            return new ServerReaction(LoadRow(id));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                 "@id", SqlDbType.UniqueIdentifier, id);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        public static ServerReaction LoadFromBaseRow(DBRow sbaseRow)
        {
            return new ServerReaction(LoadRow(sbaseRow.GetGuid("id")), sbaseRow);
        }

        public static ServerReaction[] GetAllReactionsForModel(Guid model_id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT r.*
					FROM Reaction r
					WHERE r.modelId = @model_id
					ORDER BY r.name",
                "@model_id", SqlDbType.UniqueIdentifier, model_id);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerReaction[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));
            }

            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }

        public static List<Guid> GetAllReactionsIDsForModel(Guid model_id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT r.*
					FROM Reaction r
					WHERE r.modelId = @model_id
					ORDER BY r.name",
                "@model_id", SqlDbType.UniqueIdentifier, model_id);
            List<Guid> returnList = new List<Guid>();
            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return null;

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                //results.Add(new ServerReaction(new DBRow(d)));
                returnList.Add(new ServerReaction(new DBRow(d)).ID);
            }

            return returnList;// (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }

        /// <summary>
        /// Return all reactions' ids related with modelid from compartment table.
        /// By Xinjian Qi   03/07/2009
        /// </summary>
        /// <param name="modelid"></param>
        /// <returns></returns>
        public static ArrayList GetReactionIDsByModel(Guid modelid)
        {                        
            string query = "SELECT id FROM " + __TableName + " WHERE modelid='"+ modelid.ToString()+"'";

            //SqlCommand command = DBWrapper.BuildCommand(query, "@modelid", SqlDbType.UniqueIdentifier, modelid);
            SqlCommand command = new SqlCommand(query);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            
            //int i = 0;
            foreach (DataSet d in ds)
            {
                results.Add(d.Tables[0].Rows[0]["id"]);
            }
            return results;
        }

        /// <summary>
        /// Returns true if the given compartment exists
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

        public static string getQualifier(ServerReaction sm, ServerProcess sp)
        {

            string queryString = @" select r.*
                                    from  mapReactionsProcessentities r
                                    where	r.reactionId = @modelid
                                    and r.processId = '" + sp.ID.ToString() + "'";

            System.Data.SqlClient.SqlCommand command = DBWrapper.BuildCommand(queryString, "@modelid", System.Data.SqlDbType.UniqueIdentifier, sm.ID);
            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerMapReactionsProcessEntities smp;
            string qualifier = string.Empty;
            // this does not make sense 
            foreach (DataSet ds in dsArray)
            {
                smp = new ServerMapReactionsProcessEntities(new DBRow(ds));
                qualifier = AnnotationQualifierManager.GetQualifierName(smp.QualifierId);
            }


            return qualifier;
        }

        /// <summary>
        /// A search function for SBML Parser
        /// modelId + sbmlId is used as a super key needed for frozen layout
        /// 03/05/2011 Murat Kurtcephe
        /// </summary>
        /// <param name="sbmlFileName"></param>
        /// <returns></returns>
        public static ServerReaction[] FindReactionsByIds(Guid modelId, string sbmlId)
        {
            string query = "SELECT * FROM " + __TableName + " WHERE sbmlId='" + sbmlId
                 + "' Collate SQL_Latin1_General_CP1_CS_AS and modelId ='" + modelId.ToString()+"'";
            // Collate is needed since only when sbmlID's are case sensitive it can uniquely identifies elements with model id
            SqlCommand command = new SqlCommand(query);

            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerReaction[] existingReactions = new ServerReaction[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingReactions[index] = new ServerReaction(new DBRow(ds));
                index++;
            }
            return existingReactions;
        }

        /// <summary>
        /// A search function for SBML Parser
        /// modelId + sbmlId is used as a super key needed for frozen layout
        /// 03/05/2011 Murat Kurtcephe
        /// </summary>
        /// <param name="sbmlFileName"></param>
        /// <returns></returns>
        public static ServerReaction[] FindReactionsByName(Guid modelId, string name)
        {
            string query = "SELECT * FROM " + __TableName + " WHERE name='" + name
                 + "' Collate SQL_Latin1_General_CP1_CS_AS and modelId ='" + modelId + "'";
            // Collate is needed since only when sbmlID's are case sensitive it can uniquely identifies elements with model id
            SqlCommand command = new SqlCommand(query);

            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerReaction[] existingReactions = new ServerReaction[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingReactions[index] = new ServerReaction(new DBRow(ds));
                index++;
            }
            return existingReactions;
        }

        /// <summary>
        /// A search function for SBML Parser
        /// modelId + sbmlId is used as a super key needed for frozen layout
        /// 03/05/2011 Murat Kurtcephe
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ServerReaction[] FindReactionsByIds(Guid modelId, Guid id)
        {
            string query = "SELECT * FROM " + __TableName + " WHERE Id='" + id
                 + "' Collate SQL_Latin1_General_CP1_CS_AS and modelId ='" + modelId.ToString() + "'";
            // Collate is needed since only when sbmlID's are case sensitive it can uniquely identifies elements with model id
            SqlCommand command = new SqlCommand(query);

            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            
            ServerReaction[] existingReactions = new ServerReaction[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingReactions[index] = new ServerReaction(new DBRow(ds));
                index++;
            }
            return existingReactions;
        }

        #endregion

    }// End class



} // End namespace

