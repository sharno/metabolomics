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

    public class ServerMapSpeciesMolecularEntities : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerMapSpeciesMolecularEntities()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerMapSpeciesMolecularEntities(Guid speciesId, Guid molecularEntityId, int qualifierId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewIntID(); // generate a new ID            
            this.SpeciesId = speciesId;
            this.MolecularEntityId = molecularEntityId;
            this.QualifierId = qualifierId;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerMapSpeciesMolecularEntities(SoapMapSpeciesMolecularEntities data)
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
                    __DBRow = LoadRow(data.SpeciesId, data.MolecularEntityId, data.QualifierId);
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
        public ServerMapSpeciesMolecularEntities(DBRow data)
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
        ~ServerMapSpeciesMolecularEntities()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "MapSpeciesMolecularEntities";
        //protected DBRow __MapSpeciesMolecularEntitiesRow;
        #endregion


        #region Properties
        
        /// <summary>
        /// Get/set the species.
        /// </summary>
        public Guid SpeciesId
        {
            get
            {
                return __DBRow.GetGuid("speciesId");
            }
            set
            {
                __DBRow.SetGuid("speciesId", value);
            }
        }

        /// <summary>
        /// Get/set the molecularEntityId.
        /// </summary>
        public Guid MolecularEntityId
        {
            get
            {
                return __DBRow.GetGuid("molecularEntityId");
            }
            set
            {
                __DBRow.SetGuid("molecularEntityId", value);
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
            SoapMapSpeciesMolecularEntities retval = (derived == null) ?
                retval = new SoapMapSpeciesMolecularEntities() : retval = (SoapMapSpeciesMolecularEntities)derived;

            retval.SpeciesId = this.SpeciesId;
            retval.MolecularEntityId = this.MolecularEntityId;
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
            SoapMapSpeciesMolecularEntities c = o as SoapMapSpeciesMolecularEntities;

            //if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
            //    c.ID = DBWrapper.NewShortID(); // generate a new ID

            this.SpeciesId = c.SpeciesId;
            this.MolecularEntityId = c.MolecularEntityId;
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
                "INSERT INTO " + __TableName + " (speciesId, molecularEntityId, qualifierId) VALUES (@speciesId, @molecularEntityId, @qualifierId);",
                "@speciesId", SqlDbType.UniqueIdentifier, SpeciesId,
                "@molecularEntityId", SqlDbType.UniqueIdentifier, MolecularEntityId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE speciesId = @speciesId AND molecularEntityId = @molecularEntityId AND qualifierId = @qualifierId;",
                "@speciesId", SqlDbType.UniqueIdentifier, SpeciesId,
                "@molecularEntityId", SqlDbType.UniqueIdentifier, MolecularEntityId,
                "@qualifierId", SqlDbType.SmallInt, QualifierId);

            //__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
            //    "UPDATE " + __TableName + " SET type = @type where id = @id ;",
            //    "@type", SqlDbType.VarChar, Type,
            //    "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE speciesId = @species AND molecularEntityId = @molecularEntityId AND qualifierId = @qualifierId;",
                "@speciesId", SqlDbType.UniqueIdentifier, SpeciesId,
                "@molecularEntityId", SqlDbType.UniqueIdentifier, MolecularEntityId,
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
        public static ServerMapSpeciesMolecularEntities[] AllMapSpeciesMolecularEntities()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapSpeciesMolecularEntities(new DBRow(d)));
            }

            return (ServerMapSpeciesMolecularEntities[])results.ToArray(typeof(ServerMapSpeciesMolecularEntities));
        }

        //public static List<Guid> AllMappedMolecularEntitiesBySpecies(String spid)
        //{
        //    List<Guid> returnList = new List<Guid>();

        //    SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName +" where speciesid='"+spid +"';");

        //    DataSet[] ds = new DataSet[0];
        //    DBWrapper.LoadMultiple(out ds, ref command);

        //    //ArrayList results = new ArrayList();
        //   foreach (DataRow r in ds.Tables[0].Rows)
        //    {
        //        //Guid pwId = (r["pathwayId"] is DBNull) ? Guid.Empty : (Guid)r["pathwayId"];
        //        Guid entityId = (Guid)r["molecularEntityId"];
        //        //Guid graphNodeId = (Guid)r["graphNodeId"];
        //        returnList.Add(entityId);
        //    }

        //    return returnList;
        //}


        public static ServerMapSpeciesMolecularEntities[] GetMapSpeciesMolecularEntities(Guid molecularEntityId)
        {

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE molecularEntityId = @molecularEntityId;",
                "@molecularEntityId", SqlDbType.UniqueIdentifier, molecularEntityId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapSpeciesMolecularEntities(new DBRow(d)));
            }

            return (ServerMapSpeciesMolecularEntities[])results.ToArray(typeof(ServerMapSpeciesMolecularEntities));
        }

        public static ServerMapSpeciesMolecularEntities[] GetMapSpeciesMolecularEntities(Guid molecularEntityId, int qualifierId)
        {
            
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE molecularEntityId = @molecularEntityId AND qualifierId = @qualifierId;",
                "@molecularEntityId", SqlDbType.UniqueIdentifier, molecularEntityId,
                "@qualifierId", SqlDbType.SmallInt, qualifierId);
           
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerMapSpeciesMolecularEntities(new DBRow(d)));
            }

            return (ServerMapSpeciesMolecularEntities[])results.ToArray(typeof(ServerMapSpeciesMolecularEntities));
        }

        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerMapSpeciesMolecularEntities Load(Guid speciesId, Guid molecularEntityId, int qualifierId)
        {
            return new ServerMapSpeciesMolecularEntities(LoadRow(speciesId, molecularEntityId, qualifierId));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid speciesId,Guid molecularEntityId, int qualifierId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE speciesId = @speciesId AND molecularEntityId = @molecularEntityId AND qualifierId = @qualifierId;",
                "@speciesId", SqlDbType.UniqueIdentifier, speciesId,
                "@molecularEntityId", SqlDbType.UniqueIdentifier, molecularEntityId,
                "@qualifierId", SqlDbType.SmallInt, qualifierId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class

} // End namespace



