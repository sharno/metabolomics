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

    public class ServerUnitComposition : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerUnitComposition()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerUnitComposition(Guid unitDefinitionId, Guid unitId)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewID(); // generate a new ID
            this.UnitDefinitionId = unitDefinitionId;
            this.UnitId = unitId;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerUnitComposition(SoapUnitComposition data)
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
                    __DBRow = LoadRow(data.UnitDefinitionId);
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
        public ServerUnitComposition(DBRow data)
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
        ~ServerUnitComposition()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "UnitComposition";
        protected DBRow __UnitCompositionRow;
        #endregion


        #region Properties

        ///// <summary>
        ///// Get/set the ID.
        ///// </summary>
        //public Guid ID
        //{
        //    get
        //    {
        //        return __DBRow.GetGuid("id");
        //    }
        //    set
        //    {
        //        __DBRow.SetGuid("id", value);
        //    }
        //}

        /// <summary>
        /// Get/set the unitDefinitionId.
        /// </summary>
        public Guid UnitDefinitionId
        {
            get
            {
                return __DBRow.GetGuid("unitDefinitionId");
            }
            set
            {
                __DBRow.SetGuid("unitDefinitionId", value);
            }
        }

        /// <summary>
        /// Get/set the unitId.
        /// </summary>
        public Guid UnitId
        {
            get
            {
                return __DBRow.GetGuid("unitId");
            }
            set
            {
                __DBRow.SetGuid("unitId", value);
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
            SoapUnitComposition retval = (derived == null) ?
                retval = new SoapUnitComposition() : retval = (SoapUnitComposition)derived;


            //retval.ID = this.ID;
            retval.UnitDefinitionId = this.UnitDefinitionId;
            retval.UnitId = this.UnitId;

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
            SoapUnitComposition c = o as SoapUnitComposition;

            //if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
            //    c.ID = DBWrapper.NewID(); // generate a new ID

           // this.ID = c.ID;
            this.UnitDefinitionId = c.UnitDefinitionId;
            this.UnitId = c.UnitId;
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (unitDefinitionId, unitId) VALUES (@unitDefinitionId, @unitId);",
                "@unitDefinitionId", SqlDbType.UniqueIdentifier, UnitDefinitionId,
                "@unitId", SqlDbType.UniqueIdentifier, UnitId);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE unitDefinitionId = @unitDefinitionId;",
                "@unitDefinitionId", SqlDbType.UniqueIdentifier, UnitDefinitionId);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET unitId = @unitId where unitDefinitionId = @unitDefinitionId ;",
                "@unitId", SqlDbType.UniqueIdentifier, UnitId,
                 "@unitDefinitionId", SqlDbType.UniqueIdentifier, UnitDefinitionId);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE unitDefinitionId = @unitDefinitionId AND unitId = @unitId;",
                "@unitDefinitionId", SqlDbType.UniqueIdentifier, UnitDefinitionId,
                "@unitId", SqlDbType.UniqueIdentifier, UnitId);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Compartments from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapCompartment objects ready to be sent via SOAP.
        /// </returns>
        public static ServerUnitComposition[] AllUnitCompositions()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerUnitComposition(new DBRow(d)));
            }

            return (ServerUnitComposition[])results.ToArray(typeof(ServerUnitComposition));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerUnitComposition Load(Guid unitDefinitionId)
        {
            return new ServerUnitComposition(LoadRow(unitDefinitionId));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(Guid unitDefinitionId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE unitDefinitionId = @unitDefinitionId;",
                 "@unitDefinitionId", SqlDbType.UniqueIdentifier, unitDefinitionId);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class

} // End namespace

