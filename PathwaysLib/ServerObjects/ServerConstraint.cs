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

    public class ServerConstraint : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerConstraint()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerConstraint(SoapConstraint data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __ConstraintRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __ConstraintRow = LoadRow(data.ID);
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
        public ServerConstraint(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __ConstraintRow = data;

        }


        public ServerConstraint(DBRow constraintRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __ConstraintRow = constraintRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerConstraint()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "Constraint";
        protected DBRow __ConstraintRow;

        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __ConstraintRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __ConstraintRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __ConstraintRow.GetGuid("modelId");
            }
            set
            {
                __ConstraintRow.SetGuid("modelId", value);
            }
        }




        /// <summary>
        /// Get/set the math.
        /// </summary>
        public string Math
        {
            get
            {
                return __ConstraintRow.GetString("math");
            }
            set
            {
                __ConstraintRow.SetString("math", value);
            }
        }

        /// <summary>
        /// Get/set the message.
        /// </summary>
        public string Message
        {
            get
            {
                return __ConstraintRow.GetString("message");
            }
            set
            {
                __ConstraintRow.SetString("message", value);
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
            SoapConstraint retval = (derived == null) ?
                retval = new SoapConstraint() : retval = (SoapConstraint)derived;

            //fill base class properties
            base.PrepareForSoap(retval);


            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.Math = this.Math;
            retval.Message = this.Message;

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
            SoapConstraint c = o as SoapConstraint;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelId = c.ModelId;
            this.Math = c.Math;
            this.Message = c.Message;
        }

        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __ConstraintRow.UpdateDatabase();
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            base.SetSqlCommandParameters();

            __ConstraintRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, math, message) VALUES (@id, @modelId, @math,  @message);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@math", SqlDbType.Xml, Math,
                "@message", SqlDbType.Xml, Message);

            __ConstraintRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ConstraintRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND math = @math AND message = @message where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@math", SqlDbType.Xml, Math,
                "@message", SqlDbType.Xml, Message,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ConstraintRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerConstraint[] AllConstraints()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerConstraint(new DBRow(d)));
            }

            return (ServerConstraint[])results.ToArray(typeof(ServerConstraint));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerConstraint Load(Guid id)
        {
            return new ServerConstraint(LoadRow(id));
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


