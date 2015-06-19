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

    public class ServerEvent : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerEvent()
        {
        }

        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapReaction object.
        /// </remarks>
        public ServerEvent(SoapEvent data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __EventRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __EventRow = LoadRow(data.ID);
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
        public ServerEvent(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __EventRow = data;

        }


        public ServerEvent(DBRow eventRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __EventRow = eventRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerEvent()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "[Event]";
        protected DBRow __EventRow;

        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __EventRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __EventRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __EventRow.GetGuid("modelId");
            }
            set
            {
                __EventRow.SetGuid("modelId", value);
            }
        }
        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __EventRow.GetString("sbmlId");
            }
            set
            {
                __EventRow.SetString("sbmlId", value);
            }
        }


        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __EventRow.GetString("name");
            }
            set
            {
                __EventRow.SetString("name", value);
            }
        }
  
        /// <summary>
        /// Get/set the eventTriggerId.
        /// </summary>
        public Guid EventTriggerId
        {
            get
            {
                return __EventRow.GetGuid("eventTriggerId");
            }
            set
            {
                __EventRow.SetGuid("eventTriggerId", value);
            }
        }


        /// <summary>
        /// Get/set the kineticLawId.
        /// </summary>
        public Guid EventDelayId
        {
            get
            {
                return __EventRow.GetGuid("eventDelayId");
            }
            set
            {
                __EventRow.SetGuid("eventDelayId", value);
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
            SoapEvent retval = (derived == null) ?
                retval = new SoapEvent() : retval = (SoapEvent)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.EventTriggerId = this.EventTriggerId;
            retval.EventDelayId = this.EventDelayId;
      
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
            SoapEvent c = o as SoapEvent;

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
            this.EventTriggerId = c.EventTriggerId;
            this.EventDelayId = c.EventDelayId;
        }
        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __EventRow.UpdateDatabase();
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();
            __EventRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, sbmlId, name, eventTriggerId, eventDelayId) VALUES (@id, @modelId, @sbmlId, @name, @eventTriggerId, @eventDelayId);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@eventTriggerId", SqlDbType.UniqueIdentifier, EventTriggerId,
                "@eventDelayId", SqlDbType.UniqueIdentifier, EventDelayId);

            __EventRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __EventRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND sbmlId = @sbmlId AND name = @name AND eventTriggerId = @eventTriggerId AND eventDelayId = @eventDelayId where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@eventTriggerId", SqlDbType.UniqueIdentifier, EventTriggerId,
                "@eventDelayId", SqlDbType.UniqueIdentifier, EventDelayId,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __EventRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Reactions from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapReaction objects ready to be sent via SOAP.
        /// </returns>
        public static ServerEvent[] AllModels()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerEvent(new DBRow(d)));
            }

            return (ServerEvent[])results.ToArray(typeof(ServerEvent));
        }


        /// <summary>
        /// Returns a single ServerReaction object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerEvent Load(Guid id)
        {
            return new ServerEvent(LoadRow(id));
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

