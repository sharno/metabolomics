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

    public class ServerSpeciesType : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerSpeciesType()
        {
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerSpeciesType(SoapSpeciesType data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __SpeciesTypeRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __SpeciesTypeRow = LoadRow(data.ID);
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
        public ServerSpeciesType(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __SpeciesTypeRow = data;

        }

        public ServerSpeciesType(DBRow speciesTypeRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __SpeciesTypeRow = speciesTypeRow;
        }
        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerSpeciesType()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "SpeciesType";
        protected DBRow __SpeciesTypeRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __SpeciesTypeRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __SpeciesTypeRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __SpeciesTypeRow.GetGuid("modelId");
            }
            set
            {
                __SpeciesTypeRow.SetGuid("modelId", value);
            }
        }

        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __SpeciesTypeRow.GetString("sbmlId");
            }
            set
            {
                __SpeciesTypeRow.SetString("sbmlId", value);
            }
        }


        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __SpeciesTypeRow.GetString("name");
            }
            set
            {
                __SpeciesTypeRow.SetString("name", value);
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
            SoapSpeciesType retval = (derived == null) ?
                retval = new SoapSpeciesType() : retval = (SoapSpeciesType)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;


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
            SoapSpeciesType c = o as SoapSpeciesType;


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

        }

        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __SpeciesTypeRow.UpdateDatabase();
        }

        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __SpeciesTypeRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name) VALUES (@id, @modelId, @sbmlId, @name);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name);

            __SpeciesTypeRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __SpeciesTypeRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND sbmlId = @sbmlId AND name = @name where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __SpeciesTypeRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerSpeciesType[] AllFunctionDefinitions()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerSpeciesType(new DBRow(d)));
            }

            return (ServerSpeciesType[])results.ToArray(typeof(ServerSpeciesType));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerSpeciesType Load(Guid id)
        {
            return new ServerSpeciesType(LoadRow(id));
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


