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

    public class ServerInitialAssignment : ServerSbase
    {

        #region Constructor, Destructor, ToString
        private ServerInitialAssignment()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerInitialAssignment(SoapInitialAssignment data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __InitialAssignmentRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __InitialAssignmentRow = LoadRow(data.ID);
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
        public ServerInitialAssignment(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __InitialAssignmentRow = data;

        }


        public ServerInitialAssignment(DBRow initialAssignmentRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __InitialAssignmentRow = initialAssignmentRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerInitialAssignment()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "InitialAssignment";
        protected DBRow __InitialAssignmentRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __InitialAssignmentRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __InitialAssignmentRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return __InitialAssignmentRow.GetGuid("modelId");
            }
            set
            {
                __InitialAssignmentRow.SetGuid("modelId", value);
            }
        }

        /// <summary>
        /// Get/set the symbol.
        /// </summary>
        public string Symbol
        {
            get
            {
                return __InitialAssignmentRow.GetString("symbol");
            }
            set
            {
                __InitialAssignmentRow.SetString("symbol", value);
            }
        }


        /// <summary>
        /// Get/set the math.
        /// </summary>
        public string Math
        {
            get
            {
                return __InitialAssignmentRow.GetString("math");
            }
            set
            {
                __InitialAssignmentRow.SetString("math", value);
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
            __InitialAssignmentRow.UpdateDatabase();
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
            SoapInitialAssignment retval = (derived == null) ?
                retval = new SoapInitialAssignment() : retval = (SoapInitialAssignment)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.ModelId = this.ModelId;
            retval.Symbol = this.Symbol;
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
            SoapInitialAssignment c = o as SoapInitialAssignment;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ModelId = c.ModelId;
            this.Symbol = c.Symbol;
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
            __InitialAssignmentRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, modelId, symbol, math) VALUES (@id, @modelId, @symbol, @math);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@symbol", SqlDbType.VarChar, Symbol,
                "@math", SqlDbType.Xml, Math);

            __InitialAssignmentRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __InitialAssignmentRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET modelId = @modelId AND symbol = @symbol AND math = @math where id = @id ;",
                "@modelId", SqlDbType.UniqueIdentifier, ModelId,
                "@symbol", SqlDbType.VarChar, Symbol,
                "@math", SqlDbType.Xml, Math,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __InitialAssignmentRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerInitialAssignment[] AllFunctionDefinitions()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerInitialAssignment(new DBRow(d)));
            }

            return (ServerInitialAssignment[])results.ToArray(typeof(ServerInitialAssignment));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerInitialAssignment Load(Guid id)
        {
            return new ServerInitialAssignment(LoadRow(id));
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


