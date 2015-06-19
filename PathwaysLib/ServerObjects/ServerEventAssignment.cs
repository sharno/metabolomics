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

    public class ServerEventAssignment : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerEventAssignment()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerEventAssignment(SoapEventAssignment data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __EventAssignmentRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __EventAssignmentRow = LoadRow(data.ID);
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
        public ServerEventAssignment(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __EventAssignmentRow = data;

        }

        public ServerEventAssignment(DBRow eventAssignmentRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __EventAssignmentRow = eventAssignmentRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerEventAssignment()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "EventAssignment";
        protected DBRow __EventAssignmentRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __EventAssignmentRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __EventAssignmentRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the eventId.
        /// </summary>
        public Guid EventId
        {
            get
            {
                return __EventAssignmentRow.GetGuid("eventId");
            }
            set
            {
                __EventAssignmentRow.SetGuid("eventId", value);
            }
        }

        /// <summary>
        /// Get/set the variable.
        /// </summary>
        public string Variable
        {
            get
            {
                return __EventAssignmentRow.GetString("variable");
            }
            set
            {
                __EventAssignmentRow.SetString("variable", value);
            }
        }

        /// <summary>
        /// Get/set the math.
        /// </summary>
        public string Math
        {
            get
            {
                return __EventAssignmentRow.GetString("math");
            }
            set
            {
                __EventAssignmentRow.SetString("math", value);
            }
        }

        #endregion


        #region Methods
        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __EventAssignmentRow.UpdateDatabase();
        }
        /// <summary>
        /// Returns a representation of this object suitable for being
        /// sent to a client via SOAP.
        /// </summary>
        /// <returns>
        /// A SoapObject object capable of being passed via SOAP.
        /// </returns>
        public override SoapObject PrepareForSoap(SoapObject derived)
        {
            SoapEventAssignment retval = (derived == null) ?
                retval = new SoapEventAssignment() : retval = (SoapEventAssignment)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.EventId = this.EventId;

            retval.Variable = this.Variable;
            retval.Math = this.Math;

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
            SoapEventAssignment c = o as SoapEventAssignment;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.EventId = c.EventId;
            this.Variable = c.Variable;
            this.Math = c.Math;
 

        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __EventAssignmentRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, eventId, variable, math) VALUES (@id, @eventId, @variable, @math);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@eventId", SqlDbType.UniqueIdentifier, EventId,
                "@variable", SqlDbType.VarChar, Variable,
                "@math", SqlDbType.Xml, Math);

            __EventAssignmentRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __EventAssignmentRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND kind = @kind AND exponent = @exponent AND scale = @scale AND multiplier = @multiplier where id = @id ;",
                "@eventId", SqlDbType.UniqueIdentifier, EventId,
                "@variable", SqlDbType.VarChar, Variable,
                "@math", SqlDbType.Xml, Math,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __EventAssignmentRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerEventAssignment[] AllRules()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerEventAssignment(new DBRow(d)));
            }

            return (ServerEventAssignment[])results.ToArray(typeof(ServerEventAssignment));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerEventAssignment Load(Guid id)
        {
            return new ServerEventAssignment(LoadRow(id));
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


