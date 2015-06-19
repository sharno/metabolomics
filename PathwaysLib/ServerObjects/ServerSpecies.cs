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

    public class ServerSpecies : ServerSbase, IGraphSpecies
    {

        #region Constructor, Destructor, ToString
        private ServerSpecies()
        {
        }

        /// <summary>
        /// Constructor for server Reaction wrapper with fields initiallized
        /// </summary>

        //public ServerSpecies(Guid modelId, string sbmlId, string name, Guid speciesTypeId, Guid compartmentId,
        //    double initialAmount, double initialConcentration, Guid substanceUnitsId, bool hasOnlySubstanceUnits,
        //    bool boundaryCondition, int charge, bool constant)
        //{
        //    // not yet in DB, so create empty row
        //    __SpeciesRow = new DBRow(__TableName);

        //    this.ID = DBWrapper.NewID(); // generate a new ID
        //    this.ModelId = modelId;
        //    this.SbmlId = sbmlId;
        //    this.Name = name;
        //    this.SpeciesTypeId = speciesTypeId;
        //    this.CompartmentId = compartmentId;
        //    this.InitialAmount = initialAmount;
        //    this.InitialConcentration = initialConcentration;
        //    this.SubstanceUnitsId = substanceUnitsId;
        //    this.HasOnlySubstanceUnits = hasOnlySubstanceUnits;
        //    this.BoundaryCondition = boundaryCondition;
        //    this.Charge = charge;
        //    this.Constant = constant;
        //}


        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapReaction object.
        /// </remarks>
        public ServerSpecies(SoapSpecies data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __SpeciesRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __SpeciesRow = LoadRow(data.ID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

        }

        /// <summary>
        /// Constructor for server Species wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerSpecies object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerSpecies(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __SpeciesRow = data;

        }

        public ServerSpecies(DBRow speciesRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __SpeciesRow = speciesRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerSpecies()
        {
        }
        #endregion


        #region Member Variables
        public static readonly string UnspecifiedSpecies = "00000000-0000-0000-0000-000000000000";
        private static readonly string __TableName = "Species";
        protected DBRow __SpeciesRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __SpeciesRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __SpeciesRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __SpeciesRow.GetGuid("modelId");
            }
            set
            {
                __SpeciesRow.SetGuid("modelId", value);
            }
        }
        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __SpeciesRow.GetString("sbmlId");
            }
            set
            {
                __SpeciesRow.SetString("sbmlId", value);
            }
        }

        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __SpeciesRow.GetString("name");
            }
            set
            {
                __SpeciesRow.SetString("name", value);
            }
        }

        /// <summary>
        /// Get/set the speciesTypeId.
        /// </summary>
        public Guid SpeciesTypeId
        {
            get
            {
                return __SpeciesRow.GetGuid("speciesTypeId");
            }
            set
            {
                __SpeciesRow.SetGuid("speciesTypeId", value);
            }
        }

        /// <summary>
        /// Get/set the compartmentId.
        /// </summary>
        public Guid CompartmentId
        {
            get
            {
                return __SpeciesRow.GetGuid("compartmentId");
            }
            set
            {
                __SpeciesRow.SetGuid("compartmentId", value);
            }
        }

        /// <summary>
        /// Get/set the initialAmount.
        /// </summary>
        public double InitialAmount
        {
            get
            {
                return __SpeciesRow.GetDouble("initialAmount");
            }
            set
            {
                __SpeciesRow.SetDouble("initialAmount", value);
            }
        }

        /// <summary>
        /// Get/set the initialConcentration.
        /// </summary>
        public double InitialConcentration
        {
            get
            {
                return __SpeciesRow.GetDouble("initialConcentration");
            }
            set
            {
                __SpeciesRow.SetDouble("initialConcentration", value);
            }
        }

        /// <summary>
        /// Get/set the substanceUnitsId.
        /// </summary>
        public Guid SubstanceUnitsId
        {
            get
            {
                return __SpeciesRow.GetGuid("substanceUnitsId");
            }
            set
            {
                __SpeciesRow.SetGuid("substanceUnitsId", value);
            }
        }

        /// <summary>
        /// Get/set the hasOnlySubstanceUnits.
        /// </summary>
        public bool HasOnlySubstanceUnits
        {
            get
            {
                return __SpeciesRow.GetBool("hasOnlySubstanceUnits");
            }
            set
            {
                __SpeciesRow.SetBool("hasOnlySubstanceUnits", value);
            }
        }

        /// <summary>
        /// Get/set the boundaryCondition.
        /// </summary>
        public bool BoundaryCondition
        {
            get
            {
                return __SpeciesRow.GetBool("boundaryCondition");
            }
            set
            {
                __SpeciesRow.SetBool("boundaryCondition", value);
            }
        }

        /// <summary>
        /// Get/set the charge.
        /// </summary>
        public int Charge
        {
            get
            {
                return __SpeciesRow.GetInt("charge");
            }
            set
            {
                __SpeciesRow.SetInt("charge", value);
            }
        }

        /// <summary>
        /// Get/set the constant.
        /// </summary>
        public bool Constant
        {
            get
            {
                return __SpeciesRow.GetBool("constant");
            }
            set
            {
                __SpeciesRow.SetBool("constant", value);
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
            __SpeciesRow.UpdateDatabase();
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
            SoapSpecies retval = (derived == null) ?
                retval = new SoapSpecies() : retval = (SoapSpecies)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.SpeciesTypeId = this.SpeciesTypeId;
            retval.CompartmentId = this.CompartmentId;
            retval.InitialAmount = this.InitialAmount;
            retval.InitialConcentration = this.InitialConcentration;
            retval.SubstanceUnitsId = this.SubstanceUnitsId;
            retval.HasOnlySubstanceUnits = this.HasOnlySubstanceUnits;
            retval.BoundaryCondition = this.BoundaryCondition;
            retval.Charge = this.Charge;
            retval.Constant = this.Constant;
            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerSpecies
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the Species relation.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapSpecies c = o as SoapSpecies;

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
            this.SpeciesTypeId = c.SpeciesTypeId;
            this.CompartmentId = c.CompartmentId;
            this.InitialAmount = c.InitialAmount;
            this.InitialConcentration = c.InitialConcentration;
            this.SubstanceUnitsId = c.SubstanceUnitsId;
            this.HasOnlySubstanceUnits = c.HasOnlySubstanceUnits;
            this.BoundaryCondition = c.BoundaryCondition;
            this.Charge = c.Charge;
            this.Constant = c.Constant;
        }
        #endregion

        public ServerReactionSpecies[] getReactionSpecies()
        {

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT spe.*
				FROM ReactionSpecies spe
				WHERE spe.[SpeciesId] = @id 
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

        public ServerReaction[] getAllReactions()
        {
            ArrayList results = new ArrayList();
            SqlCommand command = DBWrapper.BuildCommand(
            @"SELECT re.*
				FROM Reaction re, ReactionSpecies spe
				WHERE spe.[SpeciesId] = @id 
                AND re.[ID]=spe.[ReactionId]
				ORDER BY spe.[name];",
            "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));

            }


            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
 
        }

        public ServerReactionSpecies[] getAllReactionSpecies()
        {
            ArrayList results = new ArrayList();
            SqlCommand command = DBWrapper.BuildCommand(
            @"SELECT spe.* from
			    ReactionSpecies spe
				WHERE spe.[SpeciesId] = @id                
				ORDER BY spe.[name];",
            "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));

            }


            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));

        }

        public ServerReaction[] getAllReactions(string rolename)
        {
            int roleid = ReactionSpeciesRoleManager.GetRoleId(rolename);  
            ArrayList results = new ArrayList();
            SqlCommand command = DBWrapper.BuildCommand(
            @"SELECT re.*
				FROM Reaction re, ReactionSpecies spe
				WHERE spe.[SpeciesId] = @id 
                AND spe.[roleid] ="+ roleid +"AND re.[ID]=spe.[ReactionId]ORDER BY spe.[name];",
            "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));

            }


            return (ServerReaction[])results.ToArray(typeof(ServerReaction));

        }


        public static int CountFindSpecies(string substring, SearchMethod searchMethod)
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



        /// <summary>
        /// Searches all species and gets the corresponding number of records 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string[]</returns>
        public static string[] FindSpeciesNames(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
        {
            int bigNum = startRecord + maxRecords;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT * FROM ( SELECT TOP " + maxRecords.ToString() + @" *
									FROM ( SELECT TOP " + bigNum.ToString() + @" *
											FROM ( Select MAX([name]) [name] from " + __TableName + @"
                                                   WHERE [name] " + (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + @"
                                                   @substring
											GROUP by [name] ) top_start " + @"
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


        public static string[] FindSpeciesNames(int startRecord, int maxRecords)
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


        public static ServerSpecies[] FindSpecies(string substring, SearchMethod searchMethod, int startRecord, int maxRecords)
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
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ServerSpecies[] AllSpeciesByNameOnly(string name)
        {
            string queryString = @"SELECT c.* 
                                   FROM " + __TableName + " c WHERE c.name = @name ";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@name", SqlDbType.VarChar, name);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }


        public ServerReactionSpecies[] GetRoles(ServerReaction re)
        {
            string commandText = @"SELECT * from ReactionSpecies where reactionId = '"+ re.ID.ToString()
            + "' and speciesId = '"+ this.ID.ToString()+"'" ;
            SqlCommand command = DBWrapper.BuildCommand(commandText);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));
            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));
        }

        public ServerBasicMolecule[] GetAllMolecules()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                       @"select m.*
                from  mapspeciesmolecularentities mp,Basic_molecules m
                where mp.speciesId = @mid and mp.molecularentityId = m.Id	                
                ", "@mid", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerBasicMolecule(new DBRow(d)));
            }

            return (ServerBasicMolecule[])results.ToArray(typeof(ServerBasicMolecule));
        }
        public ServerModel[] GetAllModels()
        {
            ArrayList results = new ArrayList();
            results.Add(ServerModel.Load(this.ModelId));
            return (ServerModel[])results.ToArray(typeof(ServerModel));
        }
        public ServerCompartment[] GetAllCompartments()
        {
            ArrayList results = new ArrayList();
            results.Add(ServerCompartment.Load(this.CompartmentId));
            return (ServerCompartment[])results.ToArray(typeof(ServerCompartment));
        }
        public bool isReactant()
        {
           ServerReactionSpecies[] srs = this.getReactionSpecies();
           bool ret= false;
           foreach (ServerReactionSpecies sr in srs)
           {
               if (sr.RoleId == 1)
                   ret = true;
           }
           return ret;
        }

        public bool isProduct()
        {
            ServerReactionSpecies[] srs = this.getReactionSpecies();
            bool ret = false;
            foreach (ServerReactionSpecies sr in srs)
            {
                if (sr.RoleId == 2)
                    ret = true;
            }
            return ret;
        }

        public bool isModifier()
        {
            ServerReactionSpecies[] srs = this.getReactionSpecies();
            bool ret = false;
            foreach (ServerReactionSpecies sr in srs)
            {
                if (sr.RoleId == 3)
                    ret = true;
            }
            return ret;
        }
        public ServerEventAssignment[] GetEventAssignments()
        {
            string commandtext = @"select EA.* from EventAssignment EA, Event E
            Where E.modelId ='"+ this.ModelId +"' and EA.eventId = E.id and EA.variable like '"+ this.SbmlId +"'";

            SqlCommand command = DBWrapper.BuildCommand(commandtext);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerEventAssignment(new DBRow(d)));
            }

            return (ServerEventAssignment[])results.ToArray(typeof(ServerEventAssignment));
        }
        public ServerRule[] GetRules()
        {
            string commandtext = @"select * from [Rule] where modelID ='"+this.ModelId +"' and variable like '"+ this.SbmlId+"';";
            SqlCommand command = DBWrapper.BuildCommand(commandtext);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerRule(new DBRow(d)));
            }

            return (ServerRule[])results.ToArray(typeof(ServerRule));
        }
        public ServerInitialAssignment[] GetInitialAssignments()
        {
            string commandtext = @"select * from InitialAssignment where modelID ='" + this.ModelId + "' and symbol like '" + this.SbmlId + "';";
            SqlCommand command = DBWrapper.BuildCommand(commandtext);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerInitialAssignment(new DBRow(d)));
            }

            return (ServerInitialAssignment[])results.ToArray(typeof(ServerInitialAssignment));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
    

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __SpeciesRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name, speciesTypeId, compartmentId, initialAmount, initialConcentration, substanceUnitsId, hasOnlySubstanceUnits, boundaryCondition, charge, constant) VALUES (@id, @modelId, @sbmlId, @name, @speciesTypeId, @compartmentId, @initialAmount, @initialConcentration, @substanceUnitsId, @hasOnlySubstanceUnits, @boundaryCondition, @charge, @constant);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@speciesTypeId", SqlDbType.UniqueIdentifier, SpeciesTypeId,
                "@compartmentId", SqlDbType.UniqueIdentifier, CompartmentId,
                "@initialAmount", SqlDbType.Float, InitialAmount,
                "@initialConcentration", SqlDbType.Float, InitialConcentration,
                "@substanceUnitsId", SqlDbType.UniqueIdentifier, SubstanceUnitsId,
                "@hasOnlySubstanceUnits", SqlDbType.Bit, HasOnlySubstanceUnits,
                "@boundaryCondition", SqlDbType.Bit, BoundaryCondition,
                "@charge", SqlDbType.Int, Charge,
                "@constant", SqlDbType.Bit, Constant);

            __SpeciesRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            //todo:

            __SpeciesRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId , sbmlId = @sbmlId , name = @name , speciesTypeId = @speciesTypeId , compartmentId = @compartmentId , initialAmount = @initialAmount , initialConcentration = @initialConcentration , substanceUnitsId = @substanceUnitsId , hasOnlySubstanceUnits = @hasOnlySubstanceUnits ,  boundaryCondition = @boundaryCondition , charge = @charge , constant = @constant where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@speciesTypeId", SqlDbType.UniqueIdentifier, SpeciesTypeId,
                "@compartmentId", SqlDbType.UniqueIdentifier, CompartmentId,
                "@initialAmount", SqlDbType.Float, InitialAmount,
                "@initialConcentration", SqlDbType.Float, InitialConcentration,
                "@substanceUnitsId", SqlDbType.UniqueIdentifier, SubstanceUnitsId,
                "@hasOnlySubstanceUnits", SqlDbType.Bit, HasOnlySubstanceUnits,
                "@boundaryCondition", SqlDbType.Bit, BoundaryCondition,
                "@charge", SqlDbType.Int, Charge,
                "@constant", SqlDbType.Bit, Constant,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __SpeciesRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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

        public static ServerSpecies[] AllSpeciesByName(string name)
        {
            string queryString = @"SELECT s.* 
                                   FROM Species s
                                   WHERE s.name = @name or s.sbmlId = @name";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@name", SqlDbType.VarChar, name);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));

        }


        public static ServerSpecies[] AllSpecies()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }

        public static ServerSpecies[] GetAllSpeciesGivenCompartment(Guid compartmentId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT sp.*
					FROM Species sp
					WHERE sp.compartmentId = @compartmentId
					ORDER BY sp.name",
                "@compartmentId", SqlDbType.UniqueIdentifier, compartmentId);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerSpecies[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }
 
        /// Get all species for a model

        public static ServerSpecies[] GetAllSpeciesForModel(Guid model_id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT sp.*
					FROM Species sp
					WHERE sp.modelId = @model_id
					ORDER BY sp.name",
                "@model_id", SqlDbType.UniqueIdentifier, model_id);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerSpecies[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
        }

        public static List<Guid> GetAllSpeciesIDForModel(Guid model_id)
        {
            List<Guid> returnList = new List<Guid>();

            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT sp.*
					FROM Species sp
					WHERE sp.modelId = @model_id
					ORDER BY sp.name",
                "@model_id", SqlDbType.UniqueIdentifier, model_id);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return null;

            foreach (DataSet d in ds)
            {
                //Guid pwId = (r["pathwayId"] is DBNull) ? Guid.Empty : (Guid)r["pathwayId"];
                
                //Guid graphNodeId = (Guid)r["graphNodeId"];
                returnList.Add(new ServerSpecies(new DBRow(d)).ID);
            }
            return returnList;
        }

        public static bool Exists(Guid id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, id);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }
        public static string getQualifier(ServerSpecies sm, ServerMolecularEntity sp)
        {

            string queryString = @" select r.*
                                    from  MapSpeciesMolecularEntities r
                                    where	r.SpeciesId = @speciesid
                                    and r.MolecularEntityId = '" + sp.ID.ToString() + "'";

            System.Data.SqlClient.SqlCommand command = DBWrapper.BuildCommand(queryString, "@speciesid", System.Data.SqlDbType.UniqueIdentifier, sm.ID);
            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerMapSpeciesMolecularEntities smp;
            string qualifier = string.Empty;
            // this does not make sense 
            foreach (DataSet ds in dsArray)
            {
                smp = new ServerMapSpeciesMolecularEntities(new DBRow(ds));
                qualifier = AnnotationQualifierManager.GetQualifierName(smp.QualifierId);
            }


            return qualifier;
        }

        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerSpecies Load(Guid id)
        {
            return new ServerSpecies(LoadRow(id));
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


        public static ServerSpecies[] LoadAllForCompartment(Guid compid)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE compartmentId = @id;",
                 "@id", SqlDbType.UniqueIdentifier, compid);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpecies(new DBRow(d)));
            }

            return (ServerSpecies[])results.ToArray(typeof(ServerSpecies));
            //return new DBRow(ds);

            
            //return new ServerSpecies(LoadRow(compid));
        }

        #endregion
   
        public static String GetSpeciesMappings(Guid spid,Guid pwid)
        {
            string mapTable = "MapSpeciesMolecularEntities";
            string query = "SELECT molecularEntityId FROM " + mapTable + " WHERE speciesId='" + spid.ToString() + "'";

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
            return null;
        }

        /// <summary>
        /// A search function for SBML Parser
        /// modelId + sbmlId is used as a super key needed for frozen layout
        /// 03/05/2011 Murat Kurtcephe
        /// </summary>
        /// <param name="sbmlFileName"></param>
        /// <returns></returns>
        public static ServerSpecies[] FindSpeciesByIds(Guid modelId, string sbmlId)
        {
            string query = "SELECT * FROM " + __TableName + " WHERE sbmlId='" + sbmlId
                + "' Collate SQL_Latin1_General_CP1_CS_AS and modelId ='" + modelId.ToString() + "'";
            // Collate is needed since only when sbmlID's are case sensitive it can uniquely identifies elements with model id
            SqlCommand command = new SqlCommand(query);

            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerSpecies[] existingSpecies = new ServerSpecies[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingSpecies[index] = new ServerSpecies(new DBRow(ds));
                index++;
            }
            return existingSpecies;
        }
        /// <summary>
        /// A search function for SBML Parser
        /// modelId + sbmlId is used as a super key needed for frozen layout
        /// 03/05/2011 Murat Kurtcephe
        /// </summary>
        /// <param name="sbmlFileName"></param>
        /// <returns></returns>
        public static ServerSpecies[] FindSpeciesById(Guid modelId, Guid Id)
        {
            string query = "SELECT * FROM " + __TableName + " WHERE Id='" + Id.ToString()
                + "' Collate SQL_Latin1_General_CP1_CS_AS and modelId ='" + modelId.ToString() + "'";
            // Collate is needed since only when sbmlID's are case sensitive it can uniquely identifies elements with model id
            SqlCommand command = new SqlCommand(query);

            DataSet[] dsArray = new DataSet[0];
            DBWrapper.LoadMultiple(out dsArray, ref command);
            ArrayList results = new ArrayList();
            ServerSpecies[] existingSpecies = new ServerSpecies[dsArray.Length]; // If the model is not in the database null will return
            int index = 0;
            foreach (DataSet ds in dsArray)
            {
                existingSpecies[index] = new ServerSpecies(new DBRow(ds));
                index++;
            }
            return existingSpecies;
        }
    }// End class



} // End namespace



