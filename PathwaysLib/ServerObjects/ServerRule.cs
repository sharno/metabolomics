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

    public class ServerRule : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerRule ()
        {
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerRule (SoapRule data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __RuleRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __RuleRow = LoadRow(data.ID);
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
        public ServerRule (DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __RuleRow = data;

        }

        public ServerRule(DBRow ruleRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __RuleRow = ruleRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerRule()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "[Rule]";
        protected DBRow __RuleRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __RuleRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __RuleRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __RuleRow.GetGuid("modelId");
            }
            set
            {
                __RuleRow.SetGuid("modelId", value);
            }
        }

        /// <summary>
        /// Get/set the variable.
        /// </summary>
        public string Variable
        {
            get
            {
                return __RuleRow.GetString("variable");
            }
            set
            {
                __RuleRow.SetString("variable", value);
            }
        }

        /// <summary>
        /// Get/set the math.
        /// </summary>
        public string Math
        {
            get
            {
                return __RuleRow.GetString("math");
            }
            set
            {
                __RuleRow.SetString("math", value);
            }
        }

        /// <summary>
        /// Get/set the ruleTypeId.
        /// </summary>
        public short RuleTypeId
        {
            get
            {
                return __RuleRow.GetShort("ruleTypeId");
            }
            set
            {
                __RuleRow.SetShort("ruleTypeId", value);
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
            SoapRule retval = (derived == null) ?
                retval = new SoapRule() : retval = (SoapRule)derived;

            //fill base class properties
            base.PrepareForSoap(retval);


            retval.ID = this.ID;
            retval.ModelId = this.ModelId;

            retval.Variable = this.Variable;
            retval.Math = this.Math;
            retval.RuleTypeId = this.RuleTypeId;

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
            SoapRule c = o as SoapRule;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelId = c.ModelId;
            this.Variable = c.Variable;
            this.Math = c.Math;
            this.RuleTypeId = c.RuleTypeId;
   
        }

        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __RuleRow.UpdateDatabase();
        }


        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __RuleRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, variable, math, ruleTypeId) VALUES (@id, @modelId, @variable, @math, @ruleTypeId);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@variable", SqlDbType.VarChar, Variable,
                "@math", SqlDbType.VarChar, Math,
                "@ruleTypeId", SqlDbType.TinyInt, RuleTypeId);

            __RuleRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __RuleRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND kind = @kind AND exponent = @exponent AND scale = @scale AND multiplier = @multiplier where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@variable", SqlDbType.VarChar, Variable,
                "@math", SqlDbType.VarChar, Math,
                "@ruleTypeId", SqlDbType.TinyInt, RuleTypeId,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __RuleRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerRule[] AllRules()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerRule(new DBRow(d)));
            }

            return (ServerRule[])results.ToArray(typeof(ServerRule));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerRule Load(Guid id)
        {
            return new ServerRule(LoadRow(id));
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

