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

    public class ServerUnit : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerUnit()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerUnit(SoapUnit data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __UnitRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __UnitRow = LoadRow(data.ID);
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
        public ServerUnit(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __UnitRow = data;

        }

        public ServerUnit(DBRow unitRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __UnitRow = unitRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerUnit()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "Unit";
        protected DBRow __UnitRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __UnitRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __UnitRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __UnitRow.GetGuid("modelId");
            }
            set
            {
                __UnitRow.SetGuid("modelId", value);
            }
        }

        /// <summary>
        /// Get/set the kind.
        /// </summary>
        public Guid Kind
        {
            get
            {
                return __UnitRow.GetGuid("kind");
            }
            set
            {
                __UnitRow.SetGuid("kind", value);
            }
        }

        /// <summary>
        /// Get/set the exponent.
        /// </summary>
        public int Exponent
        {
            get
            {
                return __UnitRow.GetInt("exponent");
            }
            set
            {
                __UnitRow.SetInt("exponent", value);
            }
        }

        /// <summary>
        /// Get/set the scale.
        /// </summary>
        public int Scale
        {
            get
            {
                return __UnitRow.GetInt("scale");
            }
            set
            {
                __UnitRow.SetInt("scale", value);
            }
        }

        /// <summary>
        /// Get/set the multiplier.
        /// </summary>
        public double Multiplier
        {
            get
            {
                return __UnitRow.GetDouble("multiplier");
            }
            set
            {
                __UnitRow.SetDouble("multiplier", value);
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
            SoapUnit retval = (derived == null) ?
                retval = new SoapUnit() : retval = (SoapUnit)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.Kind = this.Kind;
            retval.Exponent = this.Exponent;
            retval.Scale = this.Scale;
            retval.Multiplier = this.Multiplier;

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
            SoapUnit c = o as SoapUnit;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;


            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelId = c.ModelId;
            this.Kind = c.Kind;
            this.Exponent = c.Exponent;
            this.Scale = c.Scale;
            this.Multiplier = c.Multiplier;
        }

        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __UnitRow.UpdateDatabase();
        }

        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __UnitRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, kind, exponent, scale, multiplier) VALUES (@id, @modelId, @kind, @exponent, @scale, @multiplier);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@kind", SqlDbType.UniqueIdentifier, Kind,
                "@exponent", SqlDbType.Int, Exponent,
                "@scale", SqlDbType.Int, Scale,
                "@multiplier", SqlDbType.Float, Multiplier);

            __UnitRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND kind = @kind AND exponent = @exponent AND scale = @scale AND multiplier = @multiplier where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@kind", SqlDbType.UniqueIdentifier, Kind,
                "@exponent", SqlDbType.Int, Exponent,
                "@scale", SqlDbType.Int, Scale,
                "@multiplier", SqlDbType.Float, Multiplier,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerUnit[] AllUnits()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerUnit(new DBRow(d)));
            }

            return (ServerUnit[])results.ToArray(typeof(ServerUnit));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerUnit Load(Guid id)
        {
            return new ServerUnit(LoadRow(id));
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
        /// Delelete all entries in unit related tables, but keep base units
        /// </summary>
        public static void DeleteAll()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"DELETE FROM UnitComposition");
            
            DBWrapper.Instance.ExecuteNonQuery(ref command);

            command = DBWrapper.BuildCommand(
                @"DELETE FROM Unit");

            DBWrapper.Instance.ExecuteNonQuery(ref command);

            command = DBWrapper.BuildCommand(
                @"DELETE FROM UnitDefinition");

            DBWrapper.Instance.ExecuteNonQuery(ref command);
        }


        #endregion

    }// End class

} // End namespace

