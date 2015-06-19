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

    public class ServerCompartment : ServerSbase, IGraphCompartment
    {

        #region Constructor, Destructor, ToString
        private ServerCompartment()
        {
        }

        /// <summary>
        /// Constructor for server Reaction wrapper with fields initiallized
        /// </summary>

        //public ServerCompartment(Guid modelId, string sbmlId, string name, Guid compartmentTypeId, int spatialDimensions, double size, Guid unitsId, Guid outside, bool constant)
        //{
        //    // not yet in DB, so create empty row
        //    __DBRow = new DBRow(__TableName);

        //    this.ID = DBWrapper.NewID(); // generate a new ID
        //    this.ModelId = modelId;
        //    this.SbmlId = sbmlId;
        //    this.Name = name;
        //    this.CompartmentTypeId = compartmentTypeId;
        //    this.SpatialDimensions = spatialDimensions;
        //    this.Size = size;
        //    this.UnitsId = unitsId;
        //    this.Outside = outside;
        //    this.Constant = constant;
        //}


        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapReaction object.
        /// </remarks>
        public ServerCompartment(SoapCompartment data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __CompartmentRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __CompartmentRow = LoadRow(data.ID);
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
        public ServerCompartment(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __CompartmentRow = data;

        }

        public ServerCompartment(DBRow compartmentRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __CompartmentRow = compartmentRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerCompartment()
        {
        }
        #endregion


        #region Member Variables
        public static readonly string UnspecifiedCompartment = "00000000-0000-0000-0000-000000000000";
        private static readonly string __TableName = "Compartment";
        protected DBRow __CompartmentRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __CompartmentRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __CompartmentRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __CompartmentRow.GetGuid("modelId");
            }
            set
            {
                __CompartmentRow.SetGuid("modelId", value);
            }
        }
        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __CompartmentRow.GetString("sbmlId");
            }
            set
            {
                __CompartmentRow.SetString("sbmlId", value);
            }
        }

        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __CompartmentRow.GetString("name");
            }
            set
            {
                __CompartmentRow.SetString("name", value);
            }
        }
        public string sbmlID
        {
            get
            {
                return __CompartmentRow.GetString("sbmlID");
            }
            set
            {
                __CompartmentRow.SetString("sbmlID", value);
            }
        }

        /// <summary>
        /// Get/set the compartmentTypeId.
        /// </summary>
        public Guid CompartmentTypeId
        {
            get
            {
                return __CompartmentRow.GetGuid("compartmentTypeId");
            }
            set
            {
                __CompartmentRow.SetGuid("compartmentTypeId", value);
            }
        }

        /// <summary>
        /// Get/set the spatialDimensions.
        /// </summary>
        public int SpatialDimensions
        {
            get
            {
                return __CompartmentRow.GetInt("spatialDimensions");
            }
            set
            {
                __CompartmentRow.SetInt("spatialDimensions", value);
            }
        }

        /// <summary>
        /// Get/set the size.
        /// </summary>
        public double Size
        {
            get
            {
                return __CompartmentRow.GetDouble("size");
            }
            set
            {
                __CompartmentRow.SetDouble("size", value);
            }
        }

        /// <summary>
        /// Get/set the unitsId.
        /// </summary>
        public Guid UnitsId
        {
            get
            {
                return __CompartmentRow.GetGuid("unitsId");
            }
            set
            {
                __CompartmentRow.SetGuid("unitsId", value);
            }
        }

        /// <summary>
        /// Get/set the outside.
        /// </summary>
        public Guid Outside
        {
            get
            {
                return __CompartmentRow.GetGuid("outside");
            }
            set
            {
                __CompartmentRow.SetGuid("outside", value);
            }
        }

        /// <summary>
        /// Get/set the compartmentClassId.
        /// </summary>
        public Guid CompartmentClassId
        {
            get
            {
                return __CompartmentRow.GetGuid("compartmentClassId");
            }
            set
            {
                __CompartmentRow.SetGuid("compartmentClassId", value);
            }
        }

        /// <summary>
        /// Get/set the constant.
        /// </summary>
        public bool Constant
        {
            get
            {
                return __CompartmentRow.GetBool("constant");
            }
            set
            {
                __CompartmentRow.SetBool("constant", value);
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
        public override SoapObject PrepareForSoap(SoapObject derived)
        {
            SoapCompartment retval = (derived == null) ?
                retval = new SoapCompartment() : retval = (SoapCompartment)derived;


            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.CompartmentTypeId = this.CompartmentTypeId;
            retval.SpatialDimensions = this.SpatialDimensions;
            retval.Size = this.Size;
            retval.UnitsId = this.UnitsId;
            retval.Outside = this.Outside;
            retval.CompartmentClassId = this.CompartmentClassId;
            retval.Constant = this.Constant;

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
            SoapCompartment c = o as SoapCompartment;

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
            this.CompartmentTypeId = c.CompartmentTypeId;
            this.SpatialDimensions = c.SpatialDimensions;
            this.Size = c.Size;
            this.UnitsId = c.UnitsId;
            this.Outside = c.Outside;
            this.Constant = c.Constant;
            this.CompartmentClassId = c.CompartmentClassId;
        }

        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __CompartmentRow.UpdateDatabase();
        }

        /// <summary>
        /// gets all the species in the compartment
        /// </summary>
        public ServerSpecies[] GetAllSpecies()
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT spe.*
				FROM Species spe
				WHERE spe.[compartmentId] = @id 
				ORDER BY spe.[name];",
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
        /// Gets all the reactions which contain species in the compartment
        /// </summary>
        public ServerReaction[] GetAllReactions()
        {
            ServerSpecies[] spes = GetAllSpecies();


            ArrayList results = new ArrayList();
            foreach (ServerSpecies spe in spes)
            {
                bool flag = true;
                ServerReactionSpecies[] rspes = spe.getReactionSpecies();
                foreach (ServerReactionSpecies rspe in rspes)
                {
                    foreach (ServerReaction sre in results)
                    {
                        if (rspe.ReactionId == sre.ID)
                            flag = false;
                    }
                    if (flag)
                        results.Add(ServerReaction.Load(rspe.ReactionId));
                }

            }

            return (ServerReaction[])results.ToArray(typeof(ServerReaction));

        }
        /// <summary>
        /// gets all the Compartments within the compartment
        /// </summary>
        public ServerCompartment[] GetImmediateChildCompartments()
        {
            // use recursion....make method get immediate children, then get their immediate childeren until null)
            SqlCommand command = DBWrapper.BuildCommand(
                @" SELECT comp.*
				FROM Compartment Comp
				WHERE comp.[outside] = @id 
				ORDER BY comp.[name];",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));

            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));

        }
        /// <summary>
        /// gets all the Compartments outside the compartment
        /// </summary>
        public ServerCompartment[] GetImmediateParentCompartment()
        {
            // use recursion....make method get immediate Parent, then get its immediate parent until null)
            SqlCommand command = DBWrapper.BuildCommand(
                @" SELECT comp.*
				FROM Compartment Comp, Compartment comp1
				WHERE comp1.[ID] = @id 
                AND comp1.[outside]=comp.[ID]
                ORDER BY comp.[name]",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));

            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));

        }

       

        public ServerReactionSpecies[] GetAllReactionSpecies(Guid reid)
        {
            ArrayList results = new ArrayList();
            ServerSpecies[] compspes = GetAllSpecies();
            ServerReactionSpecies[] reacspes = (ServerReaction.Load(reid)).GetAllSpecies();

            foreach (ServerSpecies cspe in compspes)
            {
                foreach (ServerReactionSpecies rspe in reacspes)
                {
                    if (cspe.ID == rspe.SpeciesId) results.Add(rspe);
                }
            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));
        }

        public ServerGOTerm[] GetAllGOTerms()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                      @"select p.*
                from  mapsbasego mp,go_terms p
                where mp.sbaseid = @mid	and mp.goid = p.Id	                
                ", "@mid", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerGOTerm(new DBRow(d)));
            }

            return (ServerGOTerm[])results.ToArray(typeof(ServerGOTerm));
        }

        public ServerModel[] GetAllModels()
        {
            ArrayList results = new ArrayList();
            results.Add(ServerModel.Load(this.ModelId));
            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }

        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __CompartmentRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name, compartmentTypeId, spatialDimensions, size, unitsId, outside, compartmentClassId, constant) VALUES (@id, @modelId, @sbmlId, @name, @compartmentTypeId, @spatialDimensions, @size, @unitsId, @outside, @compartmentClassId, @constant);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@compartmentTypeId", SqlDbType.UniqueIdentifier, CompartmentTypeId,
                "@spatialDimensions", SqlDbType.TinyInt, SpatialDimensions,
                "@size", SqlDbType.Float, Size,
                "@unitsId", SqlDbType.UniqueIdentifier, UnitsId,
                "@outside", SqlDbType.UniqueIdentifier, Outside,
                "@compartmentClassId", SqlDbType.UniqueIdentifier, CompartmentClassId,
                "@constant", SqlDbType.Bit, Constant);

            __CompartmentRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __CompartmentRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId , sbmlId = @sbmlId , name = @name , compartmentTypeId = @compartmentTypeId , spatialDimensions = @spatialDimensions , size = @size , unitsId = @unitsId , outside = @outside , compartmentClassId = @compartmentClassId , constant = @constant where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@compartmentTypeId", SqlDbType.UniqueIdentifier, CompartmentTypeId,
                "@spatialDimensions", SqlDbType.TinyInt, SpatialDimensions,
                "@size", SqlDbType.Float, Size,
                "@unitsId", SqlDbType.UniqueIdentifier, UnitsId,
                "@outside", SqlDbType.UniqueIdentifier, Outside,
                "@compartmentClassId", SqlDbType.UniqueIdentifier, CompartmentClassId,
                "@constant", SqlDbType.Bit, Constant,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __CompartmentRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Compartments from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapCompartment objects ready to be sent via SOAP.
        /// </returns>
        public static ServerCompartment[] AllCompartments()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));
            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }

        public static ServerCompartment[] AllCompartmentsByModel(Guid modelId)
        {
            string queryString = @"SELECT c.* 
                                   FROM Compartment c
                                   WHERE c.modelId = @modelId";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@modelId", SqlDbType.UniqueIdentifier, modelId);
             
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));
            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }
        /// <summary>
        /// Gets all Compartments with the given name or sbmlid
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ServerCompartment[]</returns>

        public static ServerCompartment[] AllCompartmentsByName(string name)
        {
            string queryString = @"SELECT c.* 
                                   FROM Compartment c
                                   WHERE c.name = @name or c.sbmlId = @name";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@name", SqlDbType.VarChar, name);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));
            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }

        /// <summary>
        /// Searches all compartments and gets the corresponding number of records 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string[]</returns>
        public static string[] FindCompartmentNames(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM ( Select MAX([name]) [name] from " + __TableName + @"
                                                   WHERE [name] "+ (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @"
                                                   @substring
											GROUP by [name] ) top_start " +@"
											ORDER BY [name] DESC ) top_page " + @"
									 )top_needed ORDER BY [name] ",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            { 
                results.Add((new DBRow(d).GetString("name")));
            }

            return (string[])results.ToArray(typeof(string));
        }

        public static int CountFindCompartments(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(distinct [name] ) FROM " + __TableName + " WHERE [name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring;",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        public static string[] FindCompartmentNames(int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;


            string commandText = @" SELECT [name] FROM ( SELECT TOP " + maxRecords.ToString() + @" [name] " +
                                           "FROM ( SELECT TOP " + bigNum.ToString() + @" [name] " +
                                                  "FROM (SELECT distinct [name] FROM " + __TableName + ")"
                                                    + __TableName + @" ORDER BY [name] " +
                                                  @") " + __TableName +
                                                  @" ORDER BY [name] DESC " +
                                                  @") " + __TableName +
                                                  @" ORDER BY [name] ";

            SqlCommand command = DBWrapper.BuildCommand(commandText);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(d.Tables[0].Rows[0]["name"]);
            }
            return (string[])results.ToArray(typeof(string));
        }
        /// <summary>
        /// Gets all Compartments with the given name ,it does not consider the sbmlid
        /// </summary>
        /// <param name="name"></param>
        /// <returns>ServerCompartment[]</returns>

        public static ServerCompartment[] AllCompartmentsByNameOnly(string name)
        {
            string queryString = @"SELECT c.* 
                                   FROM Compartment c
                                   WHERE c.name = @name ";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@name", SqlDbType.VarChar, name);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));
            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }
        /// <summary>
        /// Gets all distinct compartment names
        /// </summary>
        /// <returns>string[]</returns>
        public static string[] GetAllCompartmentNames()
        {
            string queryString = @"SELECT distinct c.name 
                                   FROM Compartment c 
                                   order by c.name";

            SqlCommand command = DBWrapper.BuildCommand(queryString);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(d.Tables[0].Rows[0]["name"]);
            }
            return (string[])results.ToArray(typeof(string));
        }

        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerCompartment Load(Guid id)
        {
            return new ServerCompartment(LoadRow(id));
        }

        public static ServerCompartment LoadFromBaseRow(DBRow sbaseRow)
        {
            return new ServerCompartment(LoadRow(sbaseRow.GetGuid("id")), sbaseRow);
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


        /// <summary>
        /// Return all compartments id related with modelid from compartment table.
        /// By Xinjian Qi   03/07/2009
        /// </summary>
        /// <param name="modelid"></param>
        /// <returns></returns>
        public static ArrayList GetCompartmentIDsByModel(Guid modelid)
        {

            //string query = "SELECT id FROM " + __TableName + " WHERE modelid=@modelid;";
            string query = "SELECT id FROM " + __TableName + " WHERE modelid='"+modelid.ToString()+"'";

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
        public static int CountFindCompartments()
        {

            string commandText = @" Select Count(*)
                                    From " + __TableName + @" m";


            SqlCommand command = DBWrapper.BuildCommand(commandText);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }

        public static ServerCompartment[] FindCompartments(int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;


            string commandText = @" SELECT *
					                FROM ( SELECT TOP " + maxRecords.ToString() + @" *
							               FROM ( SELECT TOP " + bigNum.ToString() + @" *
								                  FROM " + __TableName + @" m " +
                                                  "ORDER BY name " + @"  
                                                ) " + __TableName + @"  
							               ORDER BY name DESC" + @"  
                                         ) " + __TableName + @"
                                    ORDER BY NAME";

            SqlCommand command = DBWrapper.BuildCommand(commandText);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartment(new DBRow(d)));
            }

            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }

        /// <summary>
        /// A search function for SBML Parser
        /// modelId + sbmlId is used as a super key needed for frozen layout
        /// 03/05/2011 Murat Kurtcephe
        /// </summary>
        /// <param name="sbmlFileName"></param>
        /// <returns></returns>
        public static ServerCompartment[] FindCompartmentByIds(Guid modelId, string sbmlId)
        {
            string query = "SELECT * FROM " + __TableName + " WHERE sbmlId='" +sbmlId
                + "' Collate SQL_Latin1_General_CP1_CS_AS and modelId ='" + modelId.ToString() + "'";
            // Collate is needed since only when sbmlID's are case sensitive it can uniquely identifies elements with model id
            SqlCommand command = new SqlCommand(query);

       
            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerCompartment[] existingCompartments = new ServerCompartment[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingCompartments[index] = new ServerCompartment(new DBRow(ds));
                index++;
            }
            return existingCompartments;
        }
      

        #endregion

    }// End class



} // End namespace


