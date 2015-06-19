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

    public class ServerCompartment : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerCompartment()
        {
        }

        /// <summary>
        /// Constructor for server Reaction wrapper with fields initiallized
        /// </summary>

        public ServerCompartment(Guid modelId, string sbmlId, string name, Guid compartmentTypeId, int spatialDimensions, float size, Guid unitsId, Guid outside, bool constant)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            this.ID = DBWrapper.NewID(); // generate a new ID
            this.ModelId = modelId;
            this.SbmlId = sbmlId;
            this.Name = name;
            this.CompartmentTypeId = compartmentTypeId;
            this.SpatialDimensions = spatialDimensions;
            this.Size = size;
            this.UnitsId = unitsId;
            this.Outside = outside;
            this.Constant = constant;
        }


        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapReaction object.
        /// </remarks>
        public ServerCompartment(SoapCompartment data)
        {
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
        {
            // setup object
            __DBRow = data;

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
        private static readonly string __TableName = "Compartment";
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
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
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __DBRow.GetGuid("modelId");
            }
            set
            {
                __DBRow.SetGuid("modelId", value);
            }
        }
        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __DBRow.GetString("sbmlId");
            }
            set
            {
                __DBRow.SetString("sbmlId", value);
            }
        }

        /// <summary>
        /// Get/set the name.
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
            }
        }

        /// <summary>
        /// Get/set the compartmentTypeId.
        /// </summary>
        public Guid CompartmentTypeId
        {
            get
            {
                return __DBRow.GetGuid("compartmentTypeId");
            }
            set
            {
                __DBRow.SetGuid("compartmentTypeId", value);
            }
        }

        /// <summary>
        /// Get/set the spatialDimensions.
        /// </summary>
        public int SpatialDimensions
        {
            get
            {
                return __DBRow.GetInt("spatialDimensions");
            }
            set
            {
                __DBRow.SetInt("spatialDimensions", value);
            }
        }

        /// <summary>
        /// Get/set the size.
        /// </summary>
        public float Size
        {
            get
            {
                return __DBRow.GetFloat("size");
            }
            set
            {
                __DBRow.SetFloat("size", value);
            }
        }

        /// <summary>
        /// Get/set the unitsId.
        /// </summary>
        public Guid UnitsId
        {
            get
            {
                return __DBRow.GetGuid("unitsId");
            }
            set
            {
                __DBRow.SetGuid("unitsId", value);
            }
        }

        /// <summary>
        /// Get/set the outside.
        /// </summary>
        public Guid Outside
        {
            get
            {
                return __DBRow.GetGuid("outside");
            }
            set
            {
                __DBRow.SetGuid("outside", value);
            }
        }

        /// <summary>
        /// Get/set the constant.
        /// </summary>
        public bool Constant
        {
            get
            {
                return __DBRow.GetBool("constant");
            }
            set
            {
                __DBRow.SetBool("constant", value);
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


            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.CompartmentTypeId = this.CompartmentTypeId;
            retval.SpatialDimensions = this.SpatialDimensions;
            retval.Size = this.Size;
            retval.UnitsId = this.UnitsId;
            retval.Outside = this.Outside;
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
            SoapReaction c = o as SoapReaction;

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
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name, compartmentTypeId, spatialDimensions, size, unitsId, outside, constant) VALUES (@id, @modelId, @sbmlId, @name, @compartmentTypeId, @spatialDimensions, @size, @unitsId, @outside, @constant);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@compartmentTypeId", SqlDbType.UniqueIdentifier, CompartmentTypeId,
                "@spatialDimensions", SqlDbType.TinyInt, SpatialDimensions,
                "@size", SqlDbType.Float, Size,
                "@unitsId", SqlDbType.UniqueIdentifier, UnitsId,
                "@outside", SqlDbType.UniqueIdentifier, Outside,
                "@constant", SqlDbType.Bit, Constant);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND sbmlId = @sbmlId AND name = @name AND compartmentTypeId = @compartmentTypeId AND spatialDimensions = @spatialDimensions AND size = @size AND unitsId = @unitsId AND outside = @outside AND constant = @constant where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@compartmentTypeId", SqlDbType.UniqueIdentifier, CompartmentTypeId,
                "@spatialDimensions", SqlDbType.TinyInt, SpatialDimensions,
                "@size", SqlDbType.Float, Size,
                "@unitsId", SqlDbType.UniqueIdentifier, UnitsId,
                "@outside", SqlDbType.UniqueIdentifier, Outside,
                "@constant", SqlDbType.Bit, Constant,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerCompartment[] AllModels()
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

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                 "@id", SqlDbType.UniqueIdentifier, ID);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class



} // End namespace


