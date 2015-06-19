using System;
using System.Collections;
using System.Text;

using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using System.Collections.Generic;
using PathwaysLib.GraphObjects;

namespace PathwaysLib.ServerObjects
{
    public class ServerCompartmentClass : ServerSbase
    {
        #region Constructor, Destructor, ToString
        private ServerCompartmentClass()
        {
        }

        /// <summary>
        /// Constructor for server Reaction wrapper with fields initiallized
        /// </summary>

        //public ServerCompartment(Guid modelId, string sbmlId, string name, Guid compartmentTypeId, int spatialDimensions, double size, Guid unitsId, Guid outside, bool constant)
        //{
        //    // not yet in DB, so create empty row
        //    __DBRow = new DBRow(__TableName);

        //    this.ID = DBWrapper.NewID(); // generate a new ID
        //    this.ModelId = modelId;
        //    this.SbmlId = sbmlId;
        //    this.Name = name;
        //    this.CompartmentTypeId = compartmentTypeId;
        //    this.SpatialDimensions = spatialDimensions;
        //    this.Size = size;
        //    this.UnitsId = unitsId;
        //    this.Outside = outside;
        //    this.Constant = constant;
        //}


        /// <summary>
        /// Constructor for server Reaction wrapper.
        /// </summary>
        /// <remarks>
        /// This constructor creates a ServerReaction object from a
        /// SoapReaction object.
        /// </remarks>
        public ServerCompartmentClass(SoapCompartment data)
            : base((SoapSbase)data)
        {
            switch (data.Status)
            {
                case ObjectStatus.Insert:
                    // not yet in DB, so create empty row
                    __CompartmentClassRow = new DBRow(__TableName);
                    break;
                case ObjectStatus.ReadOnly:
                case ObjectStatus.Update:
                case ObjectStatus.NoChanges:
                    // need to load existing row first so update works properly
                    __CompartmentClassRow = LoadRow(data.ID);
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
        public ServerCompartmentClass(DBRow data)
            : base(ServerSbase.LoadRow(data.GetGuid("id")))
        {
            // setup object
            __CompartmentClassRow = data;

        }

        public ServerCompartmentClass(DBRow compartmentClassRow, DBRow sbaseRow)
            : base(sbaseRow)
        {
            // setup object
            __CompartmentClassRow = compartmentClassRow;
        }

        /// <summary>
        /// Destructor for the ServerModel class.
        /// </summary>
        /// <remarks>
        /// Call base constructor to ensure proper updates.
        /// </remarks>
        ~ServerCompartmentClass()
        {
        }
        #endregion

        #region Member Variables
        public static readonly string UnspecifiedCompartmentClass = "00000000-0000-0000-0000-000000000000";
        private static readonly string __TableName = "CompartmentClass";
        protected DBRow __CompartmentClassRow;
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the ID.
        /// </summary>
        public override Guid ID
        {
            get
            {
                return __CompartmentClassRow.GetGuid("id");
            }
            set
            {
                base.ID = value; //  update base class ID as well
                __CompartmentClassRow.SetGuid("id", value);
            }
        }
        /// <summary>
        /// Get/set the name.
        /// </summary>
        public string name
        {
            get
            {
                return __CompartmentClassRow.GetString("name");
            }
            set
            {
                __CompartmentClassRow.SetString("name", value);
            }
        }

        /// <summary>
        /// Get/set the ParentId.
        /// </summary>
        public Guid parentId
        {
            get
            {
                return __CompartmentClassRow.GetGuid("parentId");
            }
            set
            {
                __CompartmentClassRow.SetGuid("parentId", value);
            }
        }
   

  
        #endregion

        #region ADO.NET SqlCommands

        /// <summary>
        /// Required function for setting up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            base.SetSqlCommandParameters();

            __CompartmentClassRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (id, name, parentId) VALUES (@id,@name, @parentId);",
                "@id", SqlDbType.UniqueIdentifier, ID,             
                "@name", SqlDbType.VarChar, name,
                "@parentId", SqlDbType.UniqueIdentifier,parentId );

            __CompartmentClassRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);

            __CompartmentClassRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + " SET name = @name AND ParentId = @ParentId where id = @id ;",
                "@name", SqlDbType.VarChar, name,
                "@parentId", SqlDbType.UniqueIdentifier, parentId,       
                "@id", SqlDbType.UniqueIdentifier, ID);

            __CompartmentClassRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @id;",
                "@id", SqlDbType.UniqueIdentifier, ID);
        }

        #endregion



        #region Methods

        public static ServerCompartmentClass Load(Guid id)
        {
            return new ServerCompartmentClass(LoadRow(id));
        }
        

        /// <summary>
        /// Update the base class's data row, then the derived class's row
        /// </summary>
        public override void UpdateDatabase()
        {
            base.UpdateDatabase();
            __CompartmentClassRow.UpdateDatabase();
        }


        /// <summary>
        /// gets all the CompartmentClasses within the CompartmentClass
        /// </summary>
        public  ServerCompartmentClass[] GetImmediateChildCompartmentClass()
        {
            // use recursion....make method get immediate children, then get their immediate childeren until null)
            SqlCommand command = DBWrapper.BuildCommand(
                @" SELECT comp.*
				FROM CompartmentClass Comp
				WHERE comp.[parentid] = @id 
				ORDER BY comp.[name];",
                "@id", SqlDbType.UniqueIdentifier,this.ID );

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartmentClass(new DBRow(d)));

            }

            return (ServerCompartmentClass[])results.ToArray(typeof(ServerCompartmentClass));

        }
        /// <summary>
        /// gets all the names within the CompartmentClass
        /// </summary>
        public string[] GetAllNames()
        {
            // use recursion....make method get immediate children, then get their immediate childeren until null)
            SqlCommand command = DBWrapper.BuildCommand(
                @" select distinct c.[name]
                    from 
                    Compartment c
                    where 
                    c.[compartmentClassId] = @classid",
                "@classid", SqlDbType.UniqueIdentifier,this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add((new DBRow(d).GetString("name")));

            }

            return (string[])results.ToArray(typeof(string));

        }




        /// <summary>
        /// gets all the Root level compartment classes
        /// </summary>
        public static ServerCompartmentClass[] GetRootClass()
        {
            // use recursion....make method get immediate children, then get their immediate childeren until null)
            SqlCommand command = DBWrapper.BuildCommand(
                @" SELECT comp.*
				FROM CompartmentClass Comp
				WHERE comp.[parentid] is null
				ORDER BY comp.[name];"
                );

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartmentClass(new DBRow(d)));

            }

            return (ServerCompartmentClass[])results.ToArray(typeof(ServerCompartmentClass));

        }
        /// <summary>
        /// gets all the CompartmentClasss outside the CompartmentClass
        /// </summary>
        public ServerCompartmentClass[] GetImmediateParentCompartmentClass()
        {
            // use recursion....make method get immediate Parent, then get its immediate parent until null)
            SqlCommand command = DBWrapper.BuildCommand(
                @" SELECT comp.*
				FROM CompartmentClass Comp, CompartmentClass comp1
				WHERE comp1.[ID] = @id 
                AND comp1.[parentid]=comp.[ID]
                ORDER BY comp.[name]",
                "@id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCompartmentClass(new DBRow(d)));
            }

            return (ServerCompartmentClass[])results.ToArray(typeof(ServerCompartmentClass));

        }

        public static int CountFindCompartments(string substring, SearchMethod searchMethod)
        {
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.EndsWith) substring = "%" + substring;
            if (searchMethod == SearchMethod.Contains || searchMethod == SearchMethod.StartsWith) substring += "%";

            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT COUNT(*) FROM " + __TableName + " WHERE [name] " +
                    (searchMethod != SearchMethod.ExactMatch ? "LIKE" : "=") + " @substring;",
                "@substring", SqlDbType.VarChar, substring);

            DataSet[] ds = new DataSet[0];
            return (int)DBWrapper.Instance.ExecuteScalar(ref command);
        }
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
    }
}
