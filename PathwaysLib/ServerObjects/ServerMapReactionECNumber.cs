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

    public class ServerMapReactionECNumber : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerMapReactionECNumber()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerMapReactionECNumber(Guid reactionId, string ec, int qualifierId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewIntID(); // generate a new ID

            this.ReactionId = reactionId;
            this.ECNumber = ec;
            this.QualifierId = qualifierId;
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
        public ServerMapReactionECNumber(DBRow data)
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
        ~ServerMapReactionECNumber()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "MapReactionECNumber";
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
        /// Get/set the ecNumber.
        /// </summary>
        public string ECNumber
        {
            get
            {
                return __DBRow.GetString("ecNumber");
            }
            set
            {
                __DBRow.SetString("ecNumber", value);
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
            //to be implemented

            return null;
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
            //to be implemented
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (reactionId, ecNumber, qualifierId) VALUES (@reactionId, @ecNumber, @qualifierId);",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@ecNumber", SqlDbType.VarChar, ECNumber,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE reactionId = @reactionId AND ecNumber = @ecNumber AND qualifierId = @qualifierId;",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@ecNumber", SqlDbType.VarChar, ECNumber,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            //__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
            //    "UPDATE " + __TableName + " SET type = @type where id = @id ;",
            //    "@type", SqlDbType.VarChar, Type,
            //    "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE reactionId = @reactionId AND ecNumber = @ecNumber AND qualifierId = @qualifierId;",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@ecNumber", SqlDbType.VarChar, ECNumber,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Compartments from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapCompartment objects ready to be sent via SOAP.
        /// </returns>
        public static ServerMapReactionECNumber[] AllMapReactionECNumbers()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionECNumber(new DBRow(d)));
            }

            return (ServerMapReactionECNumber[])results.ToArray(typeof(ServerMapReactionECNumber));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerMapReactionECNumber Load(Guid reactionId, string ecNumber, int qualifierId)
        {
            return new ServerMapReactionECNumber(LoadRow(reactionId, ecNumber, qualifierId));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid reactionId, string ecNumber, int qualifierId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE reactionId = @reactionId AND ecNumber = @ecNumber AND qualifierId = @qualifierId;",
                "@reactionId", SqlDbType.UniqueIdentifier, reactionId,
                "@ecNumber", SqlDbType.VarChar, ecNumber,
                "@qualifierId", SqlDbType.SmallInt, qualifierId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        public static ServerMapReactionECNumber[] LoadbyReactionId (Guid reactionId)
        {
             SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE reactionId = @reactionId ;",
                "@reactionId", SqlDbType.UniqueIdentifier, reactionId);
                

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionECNumber(new DBRow(d)));
            }

            return (ServerMapReactionECNumber[])results.ToArray(typeof(ServerMapReactionECNumber));
             
           
        }

        public static ServerMapReactionECNumber[] LoadbyECNumber(string ecNumber)
        {
            SqlCommand command = DBWrapper.BuildCommand(
             "SELECT * FROM " + __TableName + " WHERE ecNumber = @ecNumber;",          
             "@ecNumber", SqlDbType.VarChar, ecNumber);
            

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapReactionECNumber(new DBRow(d)));
            }

            return (ServerMapReactionECNumber[])results.ToArray(typeof(ServerMapReactionECNumber));


        }


        #endregion

    }// End class

} // End namespace



