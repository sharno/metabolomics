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

    public class ServerMapReactionsProcessEntities : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerMapReactionsProcessEntities()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerMapReactionsProcessEntities(Guid reactionId, Guid processId, int qualifierId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewIntID(); // generate a new ID
            
            this.ReactionId = reactionId;
            this.ProcessId = processId;
            this.QualifierId = qualifierId;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerMapReactionsProcessEntities(SoapMapReactionsProcessEntities data)
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
                    __DBRow = LoadRow(data.ReactionId, data.ProcessId, data.QualifierId);
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
        public ServerMapReactionsProcessEntities(DBRow data)
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
        ~ServerMapReactionsProcessEntities()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "MapReactionsProcessEntities";
        //protected DBRow __MapReactionsProcessEntitiesRow;
        #endregion


        #region Properties       

        /// <summary>
        /// Get/set the reaction.
        /// </summary>
        public Guid ReactionId
        {
            get
            {
                return __DBRow.GetGuid("reactionId");
            }
            set
            {
                __DBRow.SetGuid("reactionId", value);
            }
        }

        /// <summary>
        /// Get/set the processId.
        /// </summary>
        public Guid ProcessId
        {
            get
            {
                return __DBRow.GetGuid("processId");
            }
            set
            {
                __DBRow.SetGuid("processId", value);
            }
        }

        public int QualifierId
        {
            get
            {
                return __DBRow.GetInt("qualifierId");
            }
            set
            {
                __DBRow.SetInt("qualifierId", value);
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
            SoapMapReactionsProcessEntities retval = (derived == null) ?
                retval = new SoapMapReactionsProcessEntities() : retval = (SoapMapReactionsProcessEntities)derived;

            retval.ReactionId = this.ReactionId;
            retval.ProcessId = this.ProcessId;
            retval.QualifierId = this.QualifierId;
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
            SoapMapReactionsProcessEntities c = o as SoapMapReactionsProcessEntities;

            //if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
            //    c.ID = DBWrapper.NewShortID(); // generate a new ID

            this.ReactionId = c.ReactionId;
            this.ProcessId = c.ProcessId;
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
                "INSERT INTO " + __TableName + " (reactionId, processId, qualifierId) VALUES (@reactionId, @processId, @qualifierId);",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@processId", SqlDbType.UniqueIdentifier, ProcessId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE reactionId = @reactionId AND processId = @processId AND qualifierId = @qualifierId;",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@processId", SqlDbType.UniqueIdentifier, ProcessId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            //__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
            //    "UPDATE " + __TableName + " SET type = @type where id = @id ;",
            //    "@type", SqlDbType.VarChar, Type,
            //    "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE reactionId = @reactionId AND processId = @processId AND qualifierId = @qualifierId;",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@processId", SqlDbType.UniqueIdentifier, ProcessId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);
        }

        #endregion


        #region Static Methods

        public static ServerMapReactionsProcessEntities[] AllMapReactionsProcessEntities(Guid processId)
        {
            string query = @"select * from MapReactionsProcessEntities 
                             where processId = @processId";

            SqlCommand command = DBWrapper.BuildCommand(query, "@processId", SqlDbType.UniqueIdentifier, processId);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }

        public static ServerMapReactionsProcessEntities[] GetProcessGivenReaction(Guid reactionId, int qualifierId)
        {
            string query = @"select *
                              from MapReactionsProcessEntities 
                              where reactionId = @reactionId 
                                    And qualifierId = @qualifierId";

            SqlCommand command = DBWrapper.BuildCommand(query, "@reactionId", SqlDbType.UniqueIdentifier, reactionId,
                                                       "@qualifierId", SqlDbType.SmallInt, qualifierId);


            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }

        public static ServerMapReactionsProcessEntities[] GetProcessGivenReaction(Guid reactionId)
        {
            string query = @"select *
                              from MapReactionsProcessEntities 
                              where reactionId = @reactionId";

            SqlCommand command = DBWrapper.BuildCommand(query, "@reactionId", SqlDbType.UniqueIdentifier, reactionId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }
        

        public static ServerMapReactionsProcessEntities[] AllMapReactionsProcessEntities(Guid processId, int qualifierId)
        {
            string query = @"select *
                              from MapReactionsProcessEntities 
                              where processId = @processId 
                                    And qualifierId = @qualifierId";

            SqlCommand command = DBWrapper.BuildCommand(query, "@processId", SqlDbType.UniqueIdentifier, processId,
                                                       "@qualifierId", SqlDbType.SmallInt, qualifierId);
           
         
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }
        /// <summary>
        /// Return all Compartments from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapCompartment objects ready to be sent via SOAP.
        /// </returns>
        public static ServerMapReactionsProcessEntities[] AllMapReactionsProcessEntities()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }

        public static ServerMapReactionsProcessEntities[] AllProcessEntities()
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT DISTINCT processId FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }



        public static ServerMapReactionsProcessEntities[] AllReactions()
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT DISTINCT reactionId FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionsProcessEntities(new DBRow(d)));
            }

            return (ServerMapReactionsProcessEntities[])results.ToArray(typeof(ServerMapReactionsProcessEntities));
        }

        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerMapReactionsProcessEntities Load(Guid reactionId, Guid processId, int qualifierId)
        {
            return new ServerMapReactionsProcessEntities(LoadRow(reactionId, processId, qualifierId));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid reactionId, Guid processId, int qualifierId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE reactionId = @reactionId AND processId = @processId AND qualifierId = @qualifierId;",
                "@reactionId", SqlDbType.UniqueIdentifier, reactionId,
                "@processId", SqlDbType.UniqueIdentifier, processId,
                "@qualifierId", SqlDbType.SmallInt, qualifierId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class

} // End namespace



