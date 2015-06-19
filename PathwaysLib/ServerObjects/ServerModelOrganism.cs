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

    public class ServerModelOrganism : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerModelOrganism()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerModelOrganism(Guid modelId, Guid orgId, int taxoId, int qId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewIntID(); // generate a new ID
            this.ModelId = modelId;
            this.OrganismGroupId = orgId;
            this.NCBITaxonomyId = taxoId;
            this.QualifierId = qId;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerModelOrganism(SoapModelOrganism data)
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
                    __DBRow = LoadRow(data.ModelId, data.OrganismGroupId, data.NCBITaxonomyId, data.QualifierId);
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
        public ServerModelOrganism(DBRow data)
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
        ~ServerModelOrganism()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "ModelOrganism";
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

        /// <summary>
        /// Get/set the qualifierId.
        /// </summary>
        public int NCBITaxonomyId
        {
            get
            {
                return __DBRow.GetInt("NCBITaxonomyId");
            }
            set
            {
                __DBRow.SetInt("NCBITaxonomyId", value);
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
            SoapModelOrganism retval = (derived == null) ?
                retval = new SoapModelOrganism() : retval = (SoapModelOrganism)derived;

            retval.ModelId = this.ModelId;           
            retval.OrganismGroupId = this.OrganismGroupId;
            retval.NCBITaxonomyId = this.NCBITaxonomyId;
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
            SoapModelOrganism c = o as SoapModelOrganism;

            //if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
            //    c.ID = DBWrapper.NewShortID(); // generate a new ID

            this.ModelId = c.ModelId;            
            this.OrganismGroupId = c.OrganismGroupId;
            this.NCBITaxonomyId = c.NCBITaxonomyId;
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
                "INSERT INTO " + __TableName + " (modelId, organismGroupId, ncbiTaxonomyId, qualifierId) VALUES (@modelId, @organismGroupId, @taxoId, @qualifierId);",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, OrganismGroupId,
                "@taxoId", SqlDbType.Int, NCBITaxonomyId,
                "@qualifierId", SqlDbType.Int, QualifierId);


            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE modelId = @modelId AND organismGroupId = @organismGroupId AND NCBITaxonomyId = @taxoId AND qualifierId = @qualifierId;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, OrganismGroupId,
                "@taxoId", SqlDbType.Int, NCBITaxonomyId,
                "@qualifierId", SqlDbType.Int, QualifierId);

            //__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
            //    "UPDATE " + __TableName + " SET type = @type where id = @id ;",
            //    "@type", SqlDbType.VarChar, Type,
            //    "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE  modelId = @modelId AND organismGroupId = @organismGroupId AND NCBITaxonomyId = @taxoId AND qualifierId = @qualifierId;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, OrganismGroupId,
                "@taxoId", SqlDbType.Int, NCBITaxonomyId,
                "@qualifierId", SqlDbType.Int, QualifierId);
        }

        #endregion


        #region Static Methods
        

        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerModelOrganism Load(Guid modelId, Guid pathwayId, Guid organismGroupId, int taxodId, int qId)
        {
            return new ServerModelOrganism(LoadRow(modelId, organismGroupId, taxodId, qId));

        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid modelId, Guid organismGroupId, int taxoId, int qId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE  modelId = @modelId AND organismGroupId = @organismGroupId AND NCBITaxonomyId = @taxoId  AND qualifierId = @qualifierId;",
                "@modelId", SqlDbType.UniqueIdentifier, modelId,
                "@organismGroupId", SqlDbType.UniqueIdentifier, organismGroupId,
                "@taxoId", SqlDbType.Int, taxoId,
                "@qualifierId", SqlDbType.Int,  qId);


            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        public static ServerModelOrganism[] FindModelOrganisms(Guid modelId)
        {
            SqlCommand command = DBWrapper.BuildCommand("SELECT * FROM " + __TableName + " where modelId = @modelId;", "@modelId", SqlDbType.UniqueIdentifier, modelId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerModelOrganism(new DBRow(d)));
            }

            return (ServerModelOrganism[])results.ToArray(typeof(ServerModelOrganism));
        }



        #endregion

    }// End class

} // End namespace



