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

    public class ServerModelMetadata : ServerSbase
    {

        #region Constructor, Destructor, ToString
        public ServerModelMetadata()
        {
        }

        public ServerModelMetadata(Guid ID, String ModelName, DateTime ModificationDate, DateTime CreationDate, String Notes, int PublicationId)
        {
            __UnitRow = new DBRow(__TableName);
            this.ID = Guid.NewGuid();
            this.ModelName = ModelName;
            this.ModificationDate = ModificationDate;
            this.CreationDate = CreationDate;
            this.Notes = Notes;
            this.PublicationId = PublicationId;
           

        }

        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerModelMetadata(SoapModelMetadata data)
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
        public ServerModelMetadata(DBRow data)
        {
            // setup object
            __UnitRow = data;

        }

        public ServerModelMetadata(DBRow unitRow, DBRow sbaseRow)
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
        ~ServerModelMetadata()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "ModelMetadata";
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
        public String ModelName
        {
            get
            {
                return __UnitRow.GetString("ModelName");
            }
            set
            {
                __UnitRow.SetString("ModelName", value);
            }
        }
        public int PublicationId
        {
            get
            {
                return __UnitRow.GetInt("PublicationId");
            }
            set
            {
                __UnitRow.SetInt("PublicationId", value);
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return __UnitRow.GetDateTime("CreationDate");
            }
            set
            {
                __UnitRow.SetDateTime("CreationDate", value);
            }
        }
        public DateTime ModificationDate
        {
            get
            {
                return __UnitRow.GetDateTime("ModificationDate");
            }
            set
            {
                __UnitRow.SetDateTime("ModificationDate", value);
            }
        }

        public String Notes
        {
            get
            {
                return __UnitRow.GetString("Notes");
            }
            set
            {
                __UnitRow.SetString("Notes", value);
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
            SoapModelMetadata retval = (derived == null) ?
                retval = new SoapModelMetadata() : retval = (SoapModelMetadata)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelName = this.ModelName;
            retval.PublicationId = this.PublicationId;
            retval.CreationDate = this.CreationDate;
            retval.ModificationDate = this.ModificationDate;
            retval.Notes = this.Notes;

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
            SoapModelMetadata c = o as SoapModelMetadata;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;


            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelName = c.ModelName;
            this.PublicationId = c.PublicationId;
            this.CreationDate = c.CreationDate;
            this.ModificationDate = c.ModificationDate;
            this.Notes = c.Notes;
        }

        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __UnitRow.UpdateDatabase();
        }

        public static ServerModelMetadata GetModelMetadataFromModelName(string modelName)
        {
            string queryString = @" SELECT * 
                                    FROM  ModelMetadata
                                    WHERE ModelName = @ModelName";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@ModelName", SqlDbType.VarChar, modelName);
            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command, true);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return new ServerModelMetadata(new DBRow(ds));
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
                "INSERT INTO " + __TableName + " (id, ModelName, PublicationId, CreationDate, ModificationDate, Notes) VALUES (@id, @ModelName, @PublicationId, @CreationDate, @ModificationDate, @Notes);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@ModelName", SqlDbType.VarChar, ModelName,
                "@PublicationId", SqlDbType.Int, PublicationId,
                "@CreationDate", SqlDbType.DateTime, CreationDate,
                "@ModificationDate", SqlDbType.DateTime, ModificationDate,
                "@Notes", SqlDbType.VarChar, Notes);

            __UnitRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET ModelName = @ModelName AND PublicationId = @PublicationId AND CreationDate = @CreationDate AND ModificationDate = @ModificationDate AND Notes = @Notes where id = @id ;",
                "@ModelName", SqlDbType.VarChar, ModelName,
                "@PublicationId", SqlDbType.Int, PublicationId,
                "@CreationDate", SqlDbType.DateTime, CreationDate,
                "@ModificationDate", SqlDbType.DateTime, ModificationDate,
                "@Notes", SqlDbType.VarChar, Notes, "@id", SqlDbType.UniqueIdentifier, ID);

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
        public static ServerModelMetadata[] AllModelMetadata()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModelMetadata(new DBRow(d)));
            }

            return (ServerModelMetadata[])results.ToArray(typeof(ServerModelMetadata));
        }

        public void AddModelMetaData(Guid ID, String ModelName, DateTime CreationDate, DateTime ModificationDate, String Notes, int PublicationId)
        {
            //(bse)
            // check if the process already belongs to the pathway
            //
            if (!Exists(ID))
            {
                if (PublicationId != -1)
                {
                    DBWrapper.Instance.ExecuteNonQuery(
                        "INSERT INTO " + __TableName + " ( ID, ModelName, CreationDate, ModificationDate, Notes, PublicationId) VALUES ( @ID, @ModelName, @CreationDate, @ModificationDate, @Notes, @PublicationId);",
                        "@ID", SqlDbType.UniqueIdentifier, ID,
                        "@ModelName", SqlDbType.VarChar, ModelName,
                        "@CreationDate", SqlDbType.DateTime, CreationDate,
                        "@ModificationDate", SqlDbType.DateTime, ModificationDate,
                        "@Notes", SqlDbType.VarChar, Notes,
                        "@PublicationId", SqlDbType.Int, PublicationId);
                }
                else
                {
                    DBWrapper.Instance.ExecuteNonQuery(
                        "INSERT INTO " + __TableName + " ( ID, ModelName, CreationDate, ModificationDate, Notes) VALUES ( @ID, @ModelName, @CreationDate, @ModificationDate, @Notes);",
                        "@ID", SqlDbType.UniqueIdentifier, ID,
                        "@ModelName", SqlDbType.VarChar, ModelName,
                        "@CreationDate", SqlDbType.DateTime, CreationDate,
                        "@ModificationDate", SqlDbType.DateTime, ModificationDate,
                        "@Notes", SqlDbType.VarChar, Notes);
                }
            }

        }

        public  bool Exists(Guid ID)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE ID = @ID",
                "@ID", SqlDbType.UniqueIdentifier, ID
                );

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }

        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerModelMetadata Load(Guid id)
        {
            return new ServerModelMetadata(LoadRow(id));
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
                @"DELETE FROM " + __TableName);

            DBWrapper.Instance.ExecuteNonQuery(ref command);
        }


        #endregion

    }// End class

} // End namespace

