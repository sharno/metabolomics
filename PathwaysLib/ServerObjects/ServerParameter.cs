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

    public class ServerParameter : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerParameter()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        //public ServerParameter(Guid modelId, Guid reactionId, string sbmlId, string name, double pvalue, Guid unitsId, bool constant)
        //{
        //    // not yet in DB, so create empty row
        //    __ParameterRow = new DBRow(__TableName);

        //    this.ID = DBWrapper.NewID(); // generate a new ID
        //    this.ModelId = modelId;
        //    this.ReactionId = reactionId;
        //    this.SbmlId = sbmlId;
        //    this.Name = name;
        //    this.Pvalue = pvalue;
        //    this.UnitsId = unitsId;
        //    this.Constant = constant;
        //}


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerParameter(SoapParameter data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __ParameterRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __ParameterRow = LoadRow(data.ID);
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
        public ServerParameter(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __ParameterRow = data;

        }

        public ServerParameter(DBRow parameterRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __ParameterRow = parameterRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerParameter()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "Parameter";
        protected DBRow __ParameterRow;

        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __ParameterRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __ParameterRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __ParameterRow.GetGuid("modelId");
            }
            set
            {
                __ParameterRow.SetGuid("modelId", value);
            }
        }

        /// <summary>
        /// Get/set the reactionId.
        /// </summary>
        public Guid ReactionId
        {
            get
            {
                return __ParameterRow.GetGuid("reactionId");
            }
            set
            {
                __ParameterRow.SetGuid("reactionId", value);
            }
        }
        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __ParameterRow.GetString("sbmlId");
            }
            set
            {
                __ParameterRow.SetString("sbmlId", value);
            }
        }

        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __ParameterRow.GetString("name");
            }
            set
            {
                __ParameterRow.SetString("name", value);
            }
        }

        

        /// <summary>
        /// Get/set the pvalue.
        /// </summary>
        public double Value
        {
            get
            {
                return __ParameterRow.GetDouble("value");
            }
            set
            {
                __ParameterRow.SetDouble("value", value);
            }
        }

        /// <summary>
        /// Get/set the unitsId.
        /// </summary>
        public Guid UnitsId
        {
            get
            {
                return __ParameterRow.GetGuid("unitsId");
            }
            set
            {
                __ParameterRow.SetGuid("unitsId", value);
            }
        }

        /// <summary>
        /// Get/set the constant.
        /// </summary>
        public bool Constant
        {
            get
            {
                return __ParameterRow.GetBool("constant");
            }
            set
            {
                __ParameterRow.SetBool("constant", value);
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
            __ParameterRow.UpdateDatabase();
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
            SoapParameter retval = (derived == null) ?
                retval = new SoapParameter() : retval = (SoapParameter)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.ReactionId = this.ReactionId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.Value = this.Value;
            retval.UnitsId = this.UnitsId;
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
            SoapParameter c = o as SoapParameter;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelId = c.ModelId;
            this.ReactionId = c.ReactionId;
            this.SbmlId = c.SbmlId;
            this.Name = c.Name;
            this.Value = c.Value;
            this.UnitsId = c.UnitsId;
            this.Constant = c.Constant;
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __ParameterRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, reactionId, sbmlId, name, [value], unitsId, constant) VALUES (@id, @modelId, @reactionId, @sbmlId, @name, @value, @unitsId, @constant);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@value", SqlDbType.Float, Value,
                "@unitsId", SqlDbType.UniqueIdentifier, UnitsId,
                "@constant", SqlDbType.Bit, Constant);

            __ParameterRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ParameterRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND reactionId = @reactionId AND sbmlId = @sbmlId AND name = @name AND [value] = @value AND unitsId = @unitsId AND constant = @constant where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@value", SqlDbType.Float, Value,
                "@unitsId", SqlDbType.UniqueIdentifier, UnitsId,
                "@constant", SqlDbType.Bit, Constant,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ParameterRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerParameter[] AllParameters()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerParameter(new DBRow(d)));
            }

            return (ServerParameter[])results.ToArray(typeof(ServerParameter));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerParameter Load(Guid id)
        {
            return new ServerParameter(LoadRow(id));
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

