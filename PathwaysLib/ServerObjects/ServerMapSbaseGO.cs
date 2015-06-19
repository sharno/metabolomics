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

    public class ServerMapSbaseGO : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerMapSbaseGO()
        {
        }

        /// <summary>
        /// Constructor for ServerMapSbaseGO with fields initiallized
        /// </summary>

        public ServerMapSbaseGO(Guid sbaseId, string goId, short qualifierId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewIntID(); // generate a new ID
            this.SbaseId = sbaseId;            
            this.QualifierId = qualifierId;
            this.GOId = goId;
        }
        
        /// <summary>
        /// Constructor for ServerMapSbaseGO.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerParameter object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerMapSbaseGO(DBRow data)
        {
            // setup object
            __DBRow = data;

        }

        /// <summary>
        /// Constructor for server DataSource wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerDataSource object from a
        /// SoapDataSource object.
        /// </remarks>
        public ServerMapSbaseGO(SoapMapSbaseGO data)
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
                    __DBRow = LoadRow(data.SbaseId, data.GoId);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

        }
        /// <summary>
        /// Destructor for the ServerMapSbaseGO class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerMapSbaseGO()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "MapSbaseGO";        
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid SbaseId
        {
            get
            {
                return __DBRow.GetGuid("sbaseId");
            }
            set
            {
                __DBRow.SetGuid("sbaseId", value);
            }
        }

        /// <summary>
        /// Get/set the qualifierId.
        /// </summary>
        public short QualifierId
        {
            get
            {
                return __DBRow.GetShort("qualifierId");
            }
            set
            {
                __DBRow.SetShort("qualifierId", value);
            }
        }

        /// <summary>
        /// Get/set the organismGroupId.
        /// </summary>
        public string GOId
        {
            get
            {
                return __DBRow.GetString("goId");
            }
            set
            {
                __DBRow.SetString("goId", value);
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
            SoapMapSbaseGO retval = (derived == null) ?
                retval = new SoapMapSbaseGO() : retval = (SoapMapSbaseGO)derived;

            retval.SbaseId = this.SbaseId;
            retval.GoId = this.GOId;
            retval.QualifierId = this.QualifierId;

            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerDataSource
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the DataSource relation.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapMapSbaseGO c = o as SoapMapSbaseGO;
            this.SbaseId = SbaseId;
            this.GOId = c.GoId;
            this.QualifierId = c.QualifierId;
        }

        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {


            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (sbaseId, goId, qualifierId) VALUES (@sbaseId, @goId, @qualifierId);",
                "@sbaseId", SqlDbType.UniqueIdentifier, SbaseId,
                "@goId", SqlDbType.VarChar, GOId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);


            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE sbaseId = @sbaseId AND goId = @goId AND qualifierId = @qualifierId;",
                "@sbaseId", SqlDbType.UniqueIdentifier, SbaseId,
                "@goId", SqlDbType.VarChar, GOId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
           "UPDATE " + __TableName + " SET sbaseID = @sbaseID AND goId = @goId AND qualifierId = @qualifierId;",
            "@sbaseID", SqlDbType.UniqueIdentifier, SbaseId,
           "@goId", SqlDbType.VarChar, GOId,
           "@qualifierId", SqlDbType.SmallInt, QualifierId);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE  sbaseId = @sbaseId AND goId = @goId AND qualifierId = @qualifierId;",
                "@sbaseId", SqlDbType.UniqueIdentifier, SbaseId,
                "@goId", SqlDbType.VarChar, GOId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);
        }

        #endregion


        #region Static Methods   
     
        public static ServerMapSbaseGO[] FindMappingBySbaseId(Guid sbaseId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE sbaseId = @sbaseId;",
                    "@sbaseId", SqlDbType.UniqueIdentifier, sbaseId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapSbaseGO(new DBRow(d)));
            }

            return (ServerMapSbaseGO[])results.ToArray(typeof(ServerMapSbaseGO));
        }

        public static ServerMapSbaseGO[] FindMappingByGOId(string goId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
             "SELECT * FROM " + __TableName + " WHERE goId = @goId;",
                 "@goId", SqlDbType.VarChar, goId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapSbaseGO(new DBRow(d)));
            }

            return (ServerMapSbaseGO[])results.ToArray(typeof(ServerMapSbaseGO));

        }

        /// <summary>
        /// Return all catalyzing relations from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapDataSource objects ready to be sent via SOAP.
        /// </returns>
        public static ServerMapSbaseGO[] AllMapSbaseGO()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapSbaseGO(new DBRow(d)));
            }

            return (ServerMapSbaseGO[])results.ToArray(typeof(ServerMapSbaseGO));
        }

        /// <summary>
        /// Returns true if there exists a data source with the given name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exists(Guid sbaseId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE sbaseId = @sbaseId;",
                    "@sbaseId", SqlDbType.UniqueIdentifier, sbaseId);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }

        public static bool Exists(string goId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE goId = @goId;",
                    "@goId", SqlDbType.VarChar, goId);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }

        /// <summary>
        /// Returns a single ServerDataSource object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerMapSbaseGO Load(Guid sbaseId, string goId)
        {
            return new ServerMapSbaseGO(LoadRow(sbaseId, goId));
        }

        public static ServerMapSbaseGO LoadBySbaseId(Guid sbaseId)
        {
            if (!Exists(sbaseId))
                return null;
            return new ServerMapSbaseGO(LoadRow(sbaseId));
        }

        public static ServerMapSbaseGO LoadByGOId(string goId)
        {
            if (!Exists(goId))
                return null;
            return new ServerMapSbaseGO(LoadRow(goId));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid sbaseId, string goId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE sbaseId = @sbaseId AND goId = @goId;",
                "@sbaseID", SqlDbType.UniqueIdentifier, sbaseId,
                "@goId", SqlDbType.VarChar, goId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(string goId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE goId = @goId ;",
                "@goId ", SqlDbType.VarChar, goId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        /// <summary>
        /// Return the dataset for an object with the given id.
        /// </summary>
        private static DBRow LoadRow(Guid sbaseId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE sbaseId = @sbaseId;",
                 "@sbaseID", SqlDbType.UniqueIdentifier, sbaseId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        /// <summary>
        /// Returns a single ServerMapSbaseGO object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerMapSbaseGO Load(Guid sbaseId, string goId, short qualifierId)
        {
            return new ServerMapSbaseGO(LoadRow(sbaseId, goId, qualifierId));

        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid sbaseId, string goId, short qualifierId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE  sbaseId = @sbaseId AND goId = @goId AND qualifierId = @qualifierId;",
                "@sbaseId", SqlDbType.UniqueIdentifier, sbaseId,
                "@goId", SqlDbType.VarChar, goId,
                "@qualifierId", SqlDbType.SmallInt, qualifierId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class

} // End namespace



