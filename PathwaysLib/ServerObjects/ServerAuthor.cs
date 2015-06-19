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

    public class ServerAuthor : ServerSbase
    {

        #region Constructor, Destructor, ToString
        public ServerAuthor()
        {
        }
       
        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        public ServerAuthor(SoapAuthor data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __UnitRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __UnitRow = LoadRow(data.ID);
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
        public ServerAuthor(DBRow data)            
        {
            // setup object
            __UnitRow = data;

        }

        public ServerAuthor(DBRow unitRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __UnitRow = unitRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerAuthor()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "Author";
        protected DBRow __UnitRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __UnitRow.GetGuid("id");
            }
            set
            {
                base.ID = value;
                __UnitRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the modelId.
        /// </summary>
        public String Name
        {
            get
            {
                return __UnitRow.GetString("Name");
            }
            set
            {
                __UnitRow.SetString("Name", value);
            }
        }
        public String Surname
        {
            get
            {
                return __UnitRow.GetString("Surname");
            }
            set
            {
                __UnitRow.SetString("Surname", value);
            }
        }

        public String EMail
        {
            get
            {
                return __UnitRow.GetString("EMail");
            }
            set
            {
                __UnitRow.SetString("EMail", value);
            }
        }
        public String OrgName
        {
            get
            {
                return __UnitRow.GetString("OrgName");
            }
            set
            {
                __UnitRow.SetString("OrgName", value);
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
            SoapAuthor retval = (derived == null) ?
                retval = new SoapAuthor() : retval = (SoapAuthor)derived;

            //fill base class properties
            base.PrepareForSoap(retval);

            retval.ID = this.ID;
            retval.Name = this.Name;
            retval.Surname = this.Surname;
            retval.EMail = this.EMail;
            retval.OrgName = this.OrgName;

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
            SoapAuthor c = o as SoapAuthor;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;


            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.Name = c.Name;
            this.Surname = c.Surname;
            this.EMail = c.EMail;
            this.OrgName = c.OrgName;
        }

        /// <summary>
        /// Finds all authors for a specific model.
        /// </summary>
        /// <param name="modelId">Name of the model.</param>
        /// <returns>Array of authors ordered by insertion.</returns>
        public static ServerAuthor[] GetAuthorsFromModelName(string modelName)
        {
            string queryString = @" SELECT a.* 
                                    FROM  Author a INNER JOIN DesignedBy d ON a.Id = d.AuthorId
                                          INNER JOIN ModelMetadata m ON m.Id = d.ModelMetadataId
                                    WHERE m.ModelName = @ModelId";

            SqlCommand command = DBWrapper.BuildCommand(queryString, "@ModelId", SqlDbType.VarChar, modelName);
            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerAuthor(new DBRow(d)));
            }
            return (ServerAuthor[])results.ToArray(typeof(ServerAuthor));
        }


        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __UnitRow.UpdateDatabase();
        }

        #endregion

        #region ADO.NET SqlCommands

        public Guid AddAuthor(Guid _ID, String _firstname, String _surname, String _eMail, String _organization)
        {
            //(bse)
            // check if the process already belongs to the pathway
            //
            Guid id;
            if ((id = Exists(_firstname, _surname, _eMail, _organization)).Equals(new Guid()))
            {
                DBWrapper.Instance.ExecuteNonQuery(
                    "INSERT INTO " + __TableName + " (ID, Name, Surname, EMail, OrgName) VALUES (@ID, @_firstname, @_surname, @_eMail, @_organization );",
                      "@ID", SqlDbType.UniqueIdentifier, _ID,
                      "@_firstname", SqlDbType.VarChar, _firstname,
                      "@_surname", SqlDbType.VarChar, _surname,
                      "@_eMail", SqlDbType.VarChar, _eMail,
                      "@_organization", SqlDbType.VarChar, _organization);

                return _ID;
            }

            return id;
        }

        public Guid Exists(String _firstName, String _secondName, String _eMail, String _organization)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE Name = @_firstname AND Surname = @_surname AND EMail = @_eMail AND OrgName = @_organization",
                "@_firstname", SqlDbType.VarChar, _firstName,
                "@_surname", SqlDbType.VarChar, _secondName,
                "@_eMail", SqlDbType.VarChar, _eMail,
                "@_organization", SqlDbType.VarChar, _organization);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return new Guid();
            return (Guid)ds[0].Tables[0].Rows[0]["ID"];
        }

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __UnitRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, Name, Surname, EMail, OrgName) VALUES (@id, @Name, @Surname, @EMail, @OrgName);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@Name", SqlDbType.VarChar, Name,
                "@Surname", SqlDbType.VarChar, Surname,
                "@EMail", SqlDbType.VarChar, EMail,
                "@OrgName", SqlDbType.VarChar, OrgName);

            __UnitRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET Name = @Name AND Surname = @Surname AND EMail = @EMail AND OrgName = @OrgName where id = @id ;",
                "@Name", SqlDbType.VarChar, Name,
                "@Surname", SqlDbType.VarChar, Surname,
                "@EMail", SqlDbType.VarChar, EMail,
                "@OrgName", SqlDbType.VarChar, OrgName,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __UnitRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        public static ServerAuthor[] AllAuthors()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerAuthor(new DBRow(d)));
            }

            return (ServerAuthor[])results.ToArray(typeof(ServerAuthor));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerAuthor Load(Guid id)
        {
            return new ServerAuthor(LoadRow(id));
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

        /// <summary>
        /// Delelete all entries in unit related tables, but keep base units
        /// </summary>
        public static void DeleteAll()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"DELETE FROM Author");
            
            DBWrapper.Instance.ExecuteNonQuery(ref command);
        }


        #endregion

    }// End class

} // End namespace

