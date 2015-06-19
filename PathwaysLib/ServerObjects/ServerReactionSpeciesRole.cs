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

    public class ServerReactionSpeciesRole : ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerReactionSpeciesRole()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        public ServerReactionSpeciesRole(string role)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            this.ID = DBWrapper.NewIntID("ReactionSpeciesRole"); // generate a new ID
            this.Role = role;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerReactionSpeciesRole(SoapReactionSpeciesRole data)
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
                    __DBRow = LoadRow(data.ID);
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
        public ServerReactionSpeciesRole(DBRow data)
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
        ~ServerReactionSpeciesRole()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "ReactionSpeciesRole";
        protected DBRow __ReactionSpeciesRoleRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public short ID
        {
            get
            {
                return __DBRow.GetShort("id");
            }
            set
            {
                __DBRow.SetShort("id", value);
            }
        }

        /// <summary>
        /// Get/set the role.
        /// </summary>
        public string Role
        {
            get
            {
                return __DBRow.GetString("role");
            }
            set
            {
                __DBRow.SetString("role", value);
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
            SoapReactionSpeciesRole retval = (derived == null) ?
                retval = new SoapReactionSpeciesRole() : retval = (SoapReactionSpeciesRole)derived;


            retval.ID = this.ID;
            retval.Role = this.Role;
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
            SoapReactionSpeciesRole c = o as SoapReactionSpeciesRole;

            if (o.Status == ObjectStatus.Insert && c.ID == 0)
                c.ID = DBWrapper.NewIntID("ReactionSpeciesRole"); // generate a new ID

            this.ID = c.ID;
            this.Role = c.Role;

        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, role) VALUES (@id, @role);",
                "@id", SqlDbType.TinyInt, ID,
                "@role", SqlDbType.VarChar, Role);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET role = @role where id = @id ;",
                "@role", SqlDbType.VarChar, Role,
                "@id", SqlDbType.TinyInt, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.TinyInt, ID);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all Compartments from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapCompartment objects ready to be sent via SOAP.
        /// </returns>
        public static ServerReactionSpeciesRole[] AllReactionSpeciesRoles()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpeciesRole(new DBRow(d)));
            }

            return (ServerReactionSpeciesRole[])results.ToArray(typeof(ServerReactionSpeciesRole));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerReactionSpeciesRole Load(short id)
        {
            return new ServerReactionSpeciesRole(LoadRow(id));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(short id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                 "@id", SqlDbType.TinyInt, id);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }


        #endregion

    }// End class

} // End namespace



