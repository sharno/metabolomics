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

    public class ServerUnitDefinition : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerUnitDefinition()
        {
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerUnitDefinition(SoapUnitDefinition data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __UnitDefinitionRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __UnitDefinitionRow = LoadRow(data.ID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

        }

        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerParameter object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerUnitDefinition(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __UnitDefinitionRow = data;

        }

        public ServerUnitDefinition(DBRow unitDefinitionRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __UnitDefinitionRow = unitDefinitionRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerUnitDefinition()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "UnitDefinition";
        protected DBRow __UnitDefinitionRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __UnitDefinitionRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __UnitDefinitionRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __UnitDefinitionRow.GetGuid("modelId");
            }
            set
            {
                __UnitDefinitionRow.SetGuid("modelId", value);
            }
        }

        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __UnitDefinitionRow.GetString("sbmlId");
            }
            set
            {
                __UnitDefinitionRow.SetString("sbmlId", value);
            }
        }


        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __UnitDefinitionRow.GetString("name");
            }
            set
            {
                __UnitDefinitionRow.SetString("name", value);
            }
        }

       
        /// <summary>
        /// Get/set the isBaseUnit.
        /// </summary>
        public bool IsBaseUnit
        {
            get
            {
                return __UnitDefinitionRow.GetBool("isBaseUnit");
            }
            set
            {
                __UnitDefinitionRow.SetBool("isBaseUnit", value);
            }
        }


        #endregion


        #region Methods
       
        /// <summary>
        /// Adds individual units into a unit definition        
        /// kind: a base unit
        /// </summary>
        public void AddUnit(Guid kind, int exponent, int scale, double multiplier)
        {
            SoapUnit su = new SoapUnit("", "", "", "", this.ModelId, kind, exponent, scale, multiplier);
            ServerUnit srvu = new ServerUnit(su);
            srvu.UpdateDatabase();

            SoapUnitComposition uc = new SoapUnitComposition(this.ID, srvu.ID);
            ServerUnitComposition srvuc = new ServerUnitComposition(uc);
            srvuc.UpdateDatabase();
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
            SoapUnitDefinition retval = (derived == null) ?
                retval = new SoapUnitDefinition() : retval = (SoapUnitDefinition)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.IsBaseUnit = this.IsBaseUnit;
     

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
            SoapUnitDefinition c = o as SoapUnitDefinition;
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
            this.IsBaseUnit = c.IsBaseUnit;
        }

        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __UnitDefinitionRow.UpdateDatabase();
        }


        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();
            __UnitDefinitionRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name, isBaseUnit) VALUES (@id, @modelId, @sbmlId, @name, @isBaseUnit);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@isBaseUnit", SqlDbType.Bit, IsBaseUnit);

            __UnitDefinitionRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitDefinitionRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND kind = @kind AND exponent = @exponent AND scale = @scale AND multiplier = @multiplier where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@isBaseUnit", SqlDbType.Bit, IsBaseUnit,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitDefinitionRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerUnitDefinition[] AllUnits()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerUnitDefinition(new DBRow(d)));
            }

            return (ServerUnitDefinition[])results.ToArray(typeof(ServerUnitDefinition));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerUnitDefinition Load(Guid id)
        {
            return new ServerUnitDefinition(LoadRow(id));
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


        #endregion

    }// End class

} // End namespace

