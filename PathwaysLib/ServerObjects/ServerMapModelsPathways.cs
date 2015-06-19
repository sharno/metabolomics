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

    public class ServerMapModelsPathways : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerMapModelsPathways()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerMapModelsPathways(Guid modelId, Guid pathwayId, int qualifierId, Guid organismGroupId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewIntID(); // generate a new ID
            this.ModelId = modelId;            
            this.PathwayId = pathwayId;
            this.QualifierId = qualifierId;
            this.OrganismGroupId = organismGroupId;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerMapModelsPathways(SoapMapModelsPathways data)
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
                    __DBRow = LoadRow(data.ModelId, data.PathwayId, data.QualifierId, data.OrganismGroupId);
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
        public ServerMapModelsPathways(DBRow data)
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
        ~ServerMapModelsPathways()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "MapModelsPathways";
        //protected DBRow __MapModelsPathwaysRow;
        #endregion


        #region Properties

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
        /// Get/set the pathwayId.
        /// </summary>
        public Guid PathwayId
        {
            get
            {
                return __DBRow.GetGuid("pathwayId");
            }
            set
            {
                __DBRow.SetGuid("pathwayId", value);
            }
        }

        /// <summary>
        /// Get/set the qualifierId.
        /// </summary>
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

        /// <summary>
        /// Get/set the organismGroupId.
        /// </summary>
        public Guid OrganismGroupId
        {
            get
            {
                return __DBRow.GetGuid("organismGroupId");
            }
            set
            {
                __DBRow.SetGuid("organismGroupId", value);
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
            SoapMapModelsPathways retval = (derived == null) ?
                retval = new SoapMapModelsPathways() : retval = (SoapMapModelsPathways)derived;

            retval.ModelId = this.ModelId;
            retval.PathwayId = this.PathwayId;
            retval.QualifierId = this.QualifierId;
            retval.OrganismGroupId = this.OrganismGroupId;
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
            SoapMapModelsPathways c = o as SoapMapModelsPathways;

            //if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
            //    c.ID = DBWrapper.NewShortID(); // generate a new ID
           
            this.ModelId= c.ModelId;
            this.PathwayId = c.PathwayId;
            this.QualifierId = c.QualifierId;
            this.OrganismGroupId = c.OrganismGroupId;

        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {


            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (modelId, pathwayId, qualifierId, organismGroupId) VALUES (@modelId, @pathwayId, @qualifierId, @organismGroupId);",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@pathwayId", SqlDbType.UniqueIdentifier, PathwayId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, OrganismGroupId);


            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE modelId = @modelId AND pathwayId = @pathwayId AND qualifierId = @qualifierId AND organismGroupId = @organismGroupId;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@pathwayId", SqlDbType.UniqueIdentifier, PathwayId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, OrganismGroupId);

            //__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
            //    "UPDATE " + __TableName + " SET type = @type where id = @id ;",
            //    "@type", SqlDbType.VarChar, Type,
            //    "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE  modelId = @modelId AND pathwayId = @pathwayId AND qualifierId = @qualifierId AND organismGroupId = @organismGroupId;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@pathwayId", SqlDbType.UniqueIdentifier, PathwayId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, OrganismGroupId);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Compartments from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapCompartment objects ready to be sent via SOAP.
        /// </returns>
        public static ServerMapModelsPathways[] AllMapModelsPathways()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapModelsPathways(new DBRow(d)));
            }

            return (ServerMapModelsPathways[])results.ToArray(typeof(ServerMapModelsPathways));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerMapModelsPathways Load(Guid modelId, Guid pathwayId, int qualifierId, Guid organismGroupId)

        {
            return new ServerMapModelsPathways(LoadRow(modelId, pathwayId, qualifierId, organismGroupId));

        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid modelId, Guid pathwayId, int qualifierId, Guid organismGroupId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE  modelId = @modelId AND pathwayId = @pathwayId AND qualifierId = @qualifierId AND organismGroupId = @organismGroupId;",
                "@modelId", SqlDbType.UniqueIdentifier, modelId,
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId,
                "@qualifierId", SqlDbType.SmallInt, qualifierId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, organismGroupId);


            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class

} // End namespace



