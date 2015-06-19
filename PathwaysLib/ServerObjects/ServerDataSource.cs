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

    public class ServerDataSource: ServerObject
    {

        #region Constructor, Destructor, ToString
        private ServerDataSource()
        {
        }

        /// <summary>
        /// Constructor for server DataSource wrapper with fields initiallized
        /// </summary>
      
        public ServerDataSource(string name, string url) 
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            //this.ID = DBWrapper.NewID(); // generate a new ID
            this.ID = DBWrapper.NewIntID("DataSource");
            this.Name = name; 
            this.Url = url;
        }

        /**********************************************************************
         * code added by mitali
         * we need a overload which takes dataSource name and retrives all the 
         * details for that datasource. 
         ***********************************************************************/
        public ServerDataSource(string name)
        {
            //step1: sqlobject
            //step2: sql statment prepare select * from dbo.DataSource where [name] = @name;
            //step3: populate datatable
            //step4: if record count <> 1 that means there is some data issue, hence raise error. 
            //step5: populate object properties. 

            if (this.__DBRow == null){
                this.__DBRow = new DBRow(__TableName);
            }

            switch (name.ToLower()){
                case "biomodels":
                    this.ID = (short)5;
                    this.Name = "BioModels";
                    this.Url = "http://www.ebi.ac.uk/biomodels-main";
                    break;
                case "cellml":
                    this.ID = (short)6;
                    this.Name = "CellML";
                    this.Url = "http://www.cellML.org";
                    break;
            }
            this.__DBRow.Status = ObjectStatus.NoChanges;
            
        }


        /// <summary>
        /// Constructor for server DataSource wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerDataSource object from a
        /// SoapDataSource object.
        /// </remarks>
        public ServerDataSource(SoapDataSource data)
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
        /// Constructor for server DataSource wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerDataSource object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerDataSource(DBRow data)
        {
            // setup object
            __DBRow = data;

        }

        /// <summary>
        /// Destructor for the ServerDataSource class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerDataSource()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "DataSource";
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the datasource ID.
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
        /// Get/set the datasource name.
        /// </summary>
        public string Name
        {

            get
            {
                return __DBRow.GetString("name");
            }
            set
            {
                __DBRow.SetString("name", value);
            }
        }

        /// <summary>
        /// Get/set the datasource url.
        /// </summary>
        public string Url
        {
            get
            {
                return __DBRow.GetString("url");
            }
            set
            {
                __DBRow.SetString("url", value);
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
            SoapDataSource retval = (derived == null) ?
                retval = new SoapDataSource() : retval = (SoapDataSource)derived;

            retval.ID = this.ID;
            retval.Name = this.Name;
            retval.Url = this.Url;

            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerDataSource
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the DataSource relation.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapDataSource c = o as SoapDataSource;
            
            // Three lines of code below have been commented until 03/29/2011
            if (o.Status == ObjectStatus.Insert && c.ID == 0)
                c.ID = DBWrapper.NewIntID(__TableName); // generate a new ID

            this.ID = c.ID;
            
            //this.ID = -1; What is this? With this code uncommented SBML parser
            // is not working Murat Kurtcephe
            this.Name = c.Name;
            this.Url = c.Url;
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, name, url) VALUES (@id, @name, @url);",
                "@id", SqlDbType.SmallInt, ID,
                "@name", SqlDbType.VarChar, Name,
                "@url", SqlDbType.VarChar, Url);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id AND name = @name AND url = @url;",
                "@id", SqlDbType.SmallInt, ID,
                "@name", SqlDbType.VarChar, Name,
                "@url", SqlDbType.VarChar, Url);


            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET id = @id AND name = @name AND url = @url;",
                "@id", SqlDbType.SmallInt, ID,
                "@name", SqlDbType.VarChar, Name,
                "@url", SqlDbType.VarChar, Url);


            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id AND name = @name AND url = @url;",
                "@id", SqlDbType.SmallInt, ID,
                "@name", SqlDbType.VarChar, Name,
                "@url", SqlDbType.VarChar, Url);
        }

        #endregion


        #region Static Methods
        /// <summary>
        /// Return all catalyzing relations from the system.
        /// </summary>
        /// <returns>
        /// Array of SoapDataSource objects ready to be sent via SOAP.
        /// </returns>
        public static ServerDataSource[] AllDataSources()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerDataSource(new DBRow(d)));
            }

            return (ServerDataSource[])results.ToArray(typeof(ServerDataSource));
        }

        /// <summary>
        /// Returns true if there exists a data source with the given name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exists(string name)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE name = @name;",
                    "@name", SqlDbType.VarChar, name);

            DataSet[] ds;
            if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
                return false;
            return true;
        }


        /// <summary>
        /// Returns a single ServerDataSource object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerDataSource Load(short id, string name, string url)
        {
            return new ServerDataSource(LoadRow(id, name, url));
        }

        public static ServerDataSource LoadById(short id)
        {
            return new ServerDataSource(LoadRow(id));
        }

        public static ServerDataSource LoadByName(string name)
        {
            if (!Exists(name))
                return null;
            return new ServerDataSource(LoadRow(name));
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(short id, string name, string url)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id AND name = @name AND url = @url;",
                "@id", SqlDbType.SmallInt, id,
                "@name", SqlDbType.VarChar, name,
                "@url", SqlDbType.VarChar, url);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        private static DBRow LoadRow(string name)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE name = @name;",
                "@name", SqlDbType.VarChar, name);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        /// <summary>
        /// Return the dataset for an object with the given id.
        /// </summary>
        private static DBRow LoadRow(short id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.SmallInt, id);

            DataSet ds;
            DBWrapper.LoadSingle(out ds, ref command);
            return new DBRow(ds);
        }

        #endregion

    } // End class

} // End namespace
