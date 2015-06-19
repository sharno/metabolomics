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

    public abstract class ServerSbase : ServerObject
    {

        #region Constructor, Destructor, ToString
        protected ServerSbase()
        {
        }

        /// <summary>
        /// Constructor for server Parameter wrapper with fields initiallized
        /// </summary>

        protected ServerSbase(string metaId, string sboTerm, string notes, string annotation)
        {
            // not yet in DB, so create empty row
            __DBRow = new DBRow(__TableName);

            this.ID = DBWrapper.NewID(); // generate a new ID

            this.MetaId = metaId;
            this.SboTerm = sboTerm;
            this.Notes = notes;
            this.Annotation = annotation;
        }


        /// <summary>
        /// Constructor for server Parameter wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapParameter object.
        /// </remarks>
        protected ServerSbase(SoapSbase data)
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
            //if (data.Status != ObjectStatus.ReadOnly)
            //    UpdateFromSoap(data);

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
        protected ServerSbase(DBRow data)
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
        ~ServerSbase()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "Sbase";
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public virtual Guid ID
        {
            get
            {
                return __DBRow.GetGuid("id");
            }
            set
            {
                __DBRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the metaId.
        /// </summary>
        public string MetaId
        {
            get
            {
                return __DBRow.GetString("metaId");
            }
            set
            {
                __DBRow.SetString("metaId", value);
            }
        }

        /// <summary>
        /// Get/set the sboTerm.
        /// </summary>
        public string SboTerm
        {
            get
            {
                return __DBRow.GetString("sboTerm");
            }
            set
            {
                __DBRow.SetString("sboTerm", value);
            }
        }

        /// <summary>
        /// Get/set the notes.
        /// </summary>
        public string Notes
        {
            get
            {
                return __DBRow.GetString("notes");
            }
            set
            {
                __DBRow.SetString("notes", value);
            }
        }

        /// <summary>
        /// Get/set the annotation.
        /// </summary>
        public string Annotation
        {
            get
            {
                return __DBRow.GetString("annotation");
            }
            set
            {
                __DBRow.SetString("annotation", value);
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
         
            // this is an abstract class, so this must be created by the derived class
            SoapSbase retval = (SoapSbase)derived;


            retval.ID = this.ID;
            retval.MetaId = this.MetaId;
            retval.SboTerm = this.SboTerm;
            retval.Notes = this.Notes;
            retval.Annotation = this.Annotation;


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
            SoapSbase c = o as SoapSbase;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.MetaId = c.MetaId;
            this.SboTerm = c.SboTerm;
            this.Notes = c.Notes;
            this.Annotation = c.Annotation;
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, metaId, sboTerm, notes, annotation) VALUES (@id, @metaId, @sboTerm, @notes, @annotation);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@metaId", SqlDbType.VarChar, MetaId,
                "@sboTerm", SqlDbType.VarChar, SboTerm,
                "@notes", SqlDbType.NText, Notes,
                "@annotation", SqlDbType.NText, Annotation);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET metaId = @metaId , sboTerm = @sboTerm , notes = @notes , annotation = @annotation where id = @id ;",
                "@metaId", SqlDbType.VarChar, MetaId,
                "@sboTerm", SqlDbType.VarChar, SboTerm,
                "@notes", SqlDbType.NText, Notes,
                "@annotation", SqlDbType.NText, Annotation,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
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
        //public static ServerSbase[] AllSbases()
        //{
        //    SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

        //    DataSet[] ds = new DataSet[0];
        //    DBWrapper.LoadMultiple(out ds, ref command);

        //    ArrayList results = new ArrayList();
        //    foreach (DataSet d in ds)
        //    {
        //        results.Add(new ServerSbase(new DBRow(d)));
        //    }

        //    return (ServerSbase[])results.ToArray(typeof(ServerSbase));
        //}


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        //public static ServerSbase Load(Guid id)
        //{
        //    return new ServerSbase(LoadRow(id));
        //}

        /// <summary>
        /// Return the dataset for an object with the given parameters.
        /// </summary>
        protected static DBRow LoadRow(Guid id)
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


