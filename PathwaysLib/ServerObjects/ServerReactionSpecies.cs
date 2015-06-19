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

    public class ServerReactionSpecies : ServerSbase, IGraphReactionSpecies
    {

        #region Constructor, Destructor, ToString
        private ServerReactionSpecies()
        {
        }

        /// <summary>
        /// Constructor for server ReactionSpecies wrapper with fields initiallized
        /// </summary>

        //public ServerReactionSpecies(Guid reactionId, Guid speciesId, short roleId,
        //    double stoichiometry, Guid stoichiometryMathId, string sbmlId, string name)
        //{
        //    // not yet in DB, so create empty row
        //    __ReactionSpeciesRow = new DBRow(__TableName);

        //    this.ID = DBWrapper.NewID(); // generate a new ID
        //    this.ReactionId = reactionId;
        //    this.SpeciesId = speciesId;
        //    this.RoleId = roleId;
        //    this.Stoichiometry = stoichiometry;
        //    this.StoichiometryMathId = stoichiometryMathId;
        //    this.SbmlId = sbmlId;
        //    this.Name = name;
        //}


        /// <summary>
        /// Constructor for server ReactionSpecies wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReactionSpecies object from a
        /// SoapReactionSpecies object.
        /// </remarks>
        public ServerReactionSpecies(SoapReactionSpecies data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __ReactionSpeciesRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __ReactionSpeciesRow = LoadRow(data.ID);
                    break;
                default:
                    throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
            }

            // get potential updates from Soap object, unless it's supposed to be read only
            if (data.Status != ObjectStatus.ReadOnly)
                UpdateFromSoap(data);

        }

        /// <summary>
        /// Constructor for server ReactionSpecies wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReactionSpecies object from a
        /// DataSet.
        /// </remarks>
        /// <param name="data">
        /// DataSet to load into the object.
        /// </param>
        public ServerReactionSpecies(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __ReactionSpeciesRow = data;

        }

        public ServerReactionSpecies(DBRow reactionspeciesRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __ReactionSpeciesRow = reactionspeciesRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerReactionSpecies()
        {
        }
        #endregion


        #region Member Variables
        private static readonly string __TableName = "ReactionSpecies";
        protected DBRow __ReactionSpeciesRow;
        #endregion


        #region Properties

        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __ReactionSpeciesRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __ReactionSpeciesRow.SetGuid("id", value);
            }
        }

        /// <summary>
        /// Get/set the reactionId.
        /// </summary>
        public Guid ReactionId
        {
            get
            {
                return __ReactionSpeciesRow.GetGuid("reactionId");
            }
            set
            {
                __ReactionSpeciesRow.SetGuid("reactionId", value);
            }
        }


        /// <summary>
        /// Get/set the speciesId.
        /// </summary>
        public Guid SpeciesId
        {
            get
            {
                return __ReactionSpeciesRow.GetGuid("speciesId");
            }
            set
            {
                __ReactionSpeciesRow.SetGuid("speciesId", value);
            }
        }

        /// <summary>
        /// Get/set the roleId.
        /// </summary>
        public short RoleId
        {
            get
            {
                return __ReactionSpeciesRow.GetShort("roleId");
            }
            set
            {
                __ReactionSpeciesRow.SetShort("roleId", value);
            }
        }

        /// <summary>
        /// Get/set the stoichiometry.
        /// </summary>
        public double Stoichiometry
        {
            get
            {
                return __ReactionSpeciesRow.GetDouble("stoichiometry");
            }
            set
            {
                __ReactionSpeciesRow.SetDouble("stoichiometry", value);
            }
        }

        /// <summary>
        /// Get/set the stoichiometryMathId.
        /// </summary>
        public Guid StoichiometryMathId
        {
            get
            {
                return __ReactionSpeciesRow.GetGuid("stoichiometryMathId");
            }
            set
            {
                __ReactionSpeciesRow.SetGuid("stoichiometryMathId", value);
            }
        }


        /// <summary>
        /// Get/set the sbmlId.
        /// </summary>
        public string SbmlId
        {
            get
            {
                return __ReactionSpeciesRow.GetString("sbmlId");
            }
            set
            {
                __ReactionSpeciesRow.SetString("sbmlId", value);
            }
        }

        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string Name
        {
            get
            {
                return __ReactionSpeciesRow.GetString("name");
            }
            set
            {
                __ReactionSpeciesRow.SetString("name", value);
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
            __ReactionSpeciesRow.UpdateDatabase();
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
            SoapReactionSpecies retval = (derived == null) ?
                retval = new SoapReactionSpecies() : retval = (SoapReactionSpecies)derived;

            //fill base class properties
            base.PrepareForSoap(retval);


            retval.ID = this.ID;
            retval.ReactionId = this.ReactionId;
            retval.SpeciesId = this.SpeciesId;
            retval.RoleId = this.RoleId;
            retval.Stoichiometry = this.Stoichiometry;
            retval.StoichiometryMathId = this.StoichiometryMathId;
            retval.SbmlId = this.SbmlId;
            retval.Name = this.Name;
            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerSpecies
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the Species relation.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapReactionSpecies c = o as SoapReactionSpecies;

            // set base class properties
            base.UpdateFromSoap(o);

            // use the ID set in the base class or the new ID generated by the base class
            this.ID = base.ID;

            if (o.Status == ObjectStatus.Insert && c.ID == Guid.Empty)
                c.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = c.ID;
            this.ReactionId = c.ReactionId;
            this.SpeciesId = c.SpeciesId;
            this.RoleId = c.RoleId;
            this.Stoichiometry = c.Stoichiometry;
            this.StoichiometryMathId = c.StoichiometryMathId;
            this.SbmlId = c.SbmlId;
            this.Name = c.Name;
        }
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __ReactionSpeciesRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, reactionId, speciesId, roleId, stoichiometry, stoichiometryMathId, sbmlId, name ) VALUES (@id, @reactionId, @speciesId, @roleId, @stoichiometry, @stoichiometryMathId, @sbmlId, @name);",
                "@id", SqlDbType.UniqueIdentifier, ID,
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@speciesId", SqlDbType.UniqueIdentifier, SpeciesId,
                "@roleId", SqlDbType.TinyInt, RoleId,
                "@stoichiometry", SqlDbType.Float, Stoichiometry,
                "@stoichiometryMathId", SqlDbType.UniqueIdentifier, StoichiometryMathId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name);

            __ReactionSpeciesRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            //todo:

            __ReactionSpeciesRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET reactionId = @reactionId AND speciesId = @speciesId AND roleId = @roleId AND stoichiometry = @ stoichiometry AND stoichiometryMathId = @stoichiometryMathId AND sbmlId = @sbmlId AND name = @name where id = @id ;",
                "@reactionId", SqlDbType.UniqueIdentifier, ReactionId,
                "@speciesId", SqlDbType.VarChar, SpeciesId,
                "@roleId", SqlDbType.TinyInt, RoleId,
                "@stoichiometry", SqlDbType.Float, Stoichiometry,
                "@stoichiometryMathId", SqlDbType.UniqueIdentifier, StoichiometryMathId,
                "@sbmlId", SqlDbType.VarChar, SbmlId,
                "@name", SqlDbType.VarChar, Name,
                "@id", SqlDbType.UniqueIdentifier, ID);

            __ReactionSpeciesRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);
        }

        #endregion


        #region Static Methods

        public static ServerReactionSpecies[] GetAllReactionsSpeciesForOneSpecies(Guid species_id)
        {
           SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT rs.*
					FROM ReactionSpecies rs
					WHERE rs.speciesId = @species_id",
                "@species_id", SqlDbType.UniqueIdentifier, species_id);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerReactionSpecies[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));
            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));

          }


        public static ServerReactionSpecies[] GetAllReactionsSpeciesForOneReaction(Guid reaction_id)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                 @"SELECT rs.*
					FROM ReactionSpecies rs
					WHERE rs.reactionId = @reaction_id",
                 "@reaction_id", SqlDbType.UniqueIdentifier, reaction_id);

            DataSet[] ds;
            int r = DBWrapper.LoadMultiple(out ds, ref command);
            if (r < 1)
                return new ServerReactionSpecies[0];

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));
            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));

        }


     
        public static ServerReactionSpecies[] AllReactionSpecies()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));
            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));
        }


        /// <summary>
        /// Returns a single ServerCompartment object
        /// </summary>
        /// <returns>
        /// Object ready to be sent via SOAP.
        /// </returns>
        public static ServerReactionSpecies Load(Guid id)
        {
            return new ServerReactionSpecies(LoadRow(id));
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

        public static ServerReactionSpecies[] LoadAllForReaction(Guid reacid)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE reactionId = @id;",
                 "@id", SqlDbType.UniqueIdentifier, reacid);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReactionSpecies(new DBRow(d)));
            }

            return (ServerReactionSpecies[])results.ToArray(typeof(ServerReactionSpecies));
            //return new DBRow(ds);


            //return new ServerSpecies(LoadRow(compid));
        }

        #endregion

    }// End class



} // End namespace



