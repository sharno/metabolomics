#region Using Declarations
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
using PathwaysLib.GraphObjects;
#endregion

namespace PathwaysLib.SBObjects 
{
    public class ServerProcess : ServerObject
    {
       /*
        #region Constructor, Destructor, ToString
		private ServerProcess()
		{
		}

		/// <summary>
		/// Constructor for server process wrapper with fields initiallized
		/// </summary>
		/// <param name="name"></param>
		/// <param name="reversible"></param>
		/// <param name="location"></param>
		/// <param name="notes"></param>
		/// <param name="genericProcessId"></param>
  
		public ServerProcess( string name, Tribool reversible, string location, string notes, Guid genericProcessId, Tribool fast )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ID = DBWrapper.NewID(); // generate a new ID
			this.Name = name;
			this.Reversible = reversible;
			this.Location = location;
			this.ProcessNotes = notes;
			this.GenericProcessID = genericProcessId;
            this.Fast = fast;
                
		}

		/// <summary>
		/// Constructor for server process wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerProcess object from a
		/// SoapProcess object.
		/// </remarks>
		/// <param name="data">
		/// A SoapProcess object from which to construct the
		/// ServerProcess object.
		/// </param>
		public ServerProcess ( SoapProcess data )
		{
			// (BE) setup database row
			switch(data.Status)
			{
				case ObjectStatus.Insert:
					// not yet in DB, so create empty row
					__DBRow = new DBRow( __TableName );
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

			// (BE) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);

		}

		/// <summary>
		/// Constructor for server process wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerProcess object from a
		/// DataSet.
		/// </remarks>
		/// <param name="data">
		/// DataSet to load into the object.
		/// </param>
		public ServerProcess ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

		/// <summary>
		/// Destructor for the ServerProcess class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerProcess()
		{
		}
		#endregion

        #region Member Variables
        private static readonly string __TableName = "processes";
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the Process ID.
        /// </summary>
        public Guid ID
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
        /// Get/set the Process name.
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
        /// Get/set the Process reversibility.
        /// </summary>
        public Tribool Reversible
        {
            get
            {
                return __DBRow.GetTribool("reversible");
            }
            set
            {
                __DBRow.SetTribool("reversible", value);
            }
        }

        /// <summary>
        /// Get/set the Process Location.
        /// </summary>
        public string Location
        {
            get
            {
                return __DBRow.GetString("location");
            }
            set
            {
                __DBRow.SetString("location", value);
            }
        }

       
        /// <summary>
        /// Get/set the Process type.
        /// </summary>
        public string ProcessNotes
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
        /// Get/set the generic process id.
        /// </summary>
        public Guid GenericProcessID
        {
            get
            {
                return __DBRow.GetGuid("generic_process_id");
            }
            set
            {
                __DBRow.SetGuid("generic_process_id", value);
            }
        }

        public Tribool Fast
        {
            get
            {
                return __DBRow.GetTribool("fast");
            }
            set
            {
                __DBRow.SetTribool("fast", value);
            }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsGenericProcess
        {
            get { return false; }
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
            SoapProcess retval = (derived == null) ?
                retval = new SoapProcess() : retval = (SoapProcess)derived;

            retval.ID = this.ID;
            retval.Name = this.Name;
            retval.Reversible = this.Reversible;
            retval.Location = this.Location;

            retval.ProcessNotes = this.ProcessNotes;
            retval.GenericProcessID = this.GenericProcessID;
            retval.Fast = this.Fast;

            retval.Status = ObjectStatus.NoChanges;

            return retval;
        }

        /// <summary>
        /// Consumes a SoapObject object and updates the ServerProcess
        /// from it.
        /// </summary>
        /// <param name="o">
        /// The SoapObject object to update from, potentially containing
        /// changes to the Process.
        /// </param>
        protected override void UpdateFromSoap(SoapObject o)
        {
            SoapProcess p = o as SoapProcess;

            if (o.Status == ObjectStatus.Insert && p.ID == Guid.Empty)
                p.ID = DBWrapper.NewID(); // generate a new ID

            this.ID = p.ID;
            this.Name = p.Name;
            this.Reversible = p.Reversible;
            this.Location = p.Location;

            this.ProcessNotes = p.ProcessNotes;
            this.GenericProcessID = p.GenericProcessID;
            this.Fast = p.Fast;
        }

        #region Process "belongs to" pathway relation
        /// <summary>
        /// Add this process to the specified pathway
        /// </summary>
        /// <param name="pathway_id"></param>
        /// <param name="notes"></param>
        public void AddToPathway(Guid pathway_id, string notes)
        {
            ServerPathway.AddProcessToPathway(pathway_id, this.ID, notes);
        }

        /// <summary>
        /// Remove this process from the given pathway
        /// </summary>
        /// <param name="pathway_id"></param>
        public void RemoveFromPathway(Guid pathway_id)
        {
            ServerPathway.RemoveProcessFromPathway(pathway_id, this.ID);
        }

        /// <summary>
        /// Return all pathways for this process.
        /// </summary>
        /// <returns></returns>
        public ServerPathway[] GetAllPathways()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                    @"SELECT pat.*
					FROM pathways pat
					WHERE pat.id IN ( SELECT pp.pathway_id
										FROM pathway_processes pp
										WHERE pp.process_id = @process_id )
					ORDER BY pat.[name];",
                "@process_id", SqlDbType.UniqueIdentifier, this.ID);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerPathway(new DBRow(d)));
            }

            return (ServerPathway[])results.ToArray(typeof(ServerPathway));
        }
        #endregion


        #region Additional Queries
        /// <summary>
        /// Returns all the organisms or organism groups for this process
        /// </summary>
        /// <returns></returns>
        public ServerOrganismGroup[] GetAllOrganisms()
        {
            return ServerProcess.GetAllOrganismsForProcess(this.ID);
        }

        /// <summary>
        /// Returns all organism groups for this process
        /// </summary>
        /// <returns></returns>
        public ServerOrganismGroup[] GetAllOrganismGroups()
        {
            return ServerProcess.GetAllOrganismGroupsForProcess(this.ID);
        }

        /// <summary>
        /// Returns all the processes that share this process' generic process id
        /// </summary>
        /// <returns></returns>
        public ServerProcess[] AllProcessesSharingThisGenericProcessId()
        {
            return ServerProcess.GetProcessByGenericProcessId(this.GenericProcessID);
        }

        #endregion

        #region Catalyzes Relation
        /// <summary>
        /// Adds a gene product to the catalyzing relation
        /// </summary>
        /// <param name="gene_product_id"></param>
        /// <param name="orgGroupId"></param>
        /// <param name="ec_number"></param>
        public void AddGeneProduct(Guid gene_product_id, Guid orgGroupId, string ec_number)
        {
            ServerCatalyze.AddGeneProductToProcess(gene_product_id, this.ID, orgGroupId, ec_number);
        }

        /// <summary>
        /// Remove a gene product from the process
        /// </summary>
        /// <param name="gene_product_id"></param>
        /// <param name="orgGroupId"></param>
        /// <param name="ec_number"></param>
        public void RemoveGeneProduct(Guid gene_product_id, Guid orgGroupId, string ec_number)
        {
            ServerCatalyze.RemoveGeneProductFromProcess(gene_product_id, this.ID, orgGroupId, ec_number);
        }

        /// <summary>
        /// Get all gene products
        /// </summary>
        /// <returns>
        /// Returns all of the gene products involved in the process
        /// </returns>
        public ServerGeneProduct[] GetAllGeneProducts()
        {
            return ServerCatalyze.GetAllGeneProductsForProcess(this.ID);
        }

        /// <summary>
        /// Get all gene products in a specific organism
        /// </summary>
        /// <returns>
        /// Returns all of the gene products involved in the process within a specific organism
        /// </returns>
        public ServerGeneProduct[] GetAllGeneProductsInOrganism(Guid orgId)
        {
            return ServerCatalyze.GetAllGeneProductsForProcessInOrganism(this.ID, orgId);
        }


        /// <summary>
        /// Returns the ec number for an enzyme in this process
        /// </summary>
        /// <param name="geneProductId"></param>
        /// <returns></returns>
        public ServerECNumber GetECNumberByGeneProduct(Guid geneProductId)
        {
            return ServerCatalyze.GetECNumberForGeneProductAndProcess(this.ID, geneProductId);
        }

        /// <summary>
        /// Returns the EC Number(s) for this process from 
        /// gene_products_and_processes table
        /// </summary>
        /// <returns></returns>
        public ServerECNumber[] GetECNumbers()
        {
            return ServerCatalyze.GetECNumbersForProcess(this.ID);
        }
        #endregion

        #region Process Entities Relation
        /// <summary>
        /// Returns all Molecular Entities involved in this process
        /// </summary>
        /// <returns></returns>
        public ServerMolecularEntity[] GetAllMolecularEntities()
        {
            return ServerMolecularEntity.GetAllEntitiesInProcess(this.ID);
        }

        /// <summary>
        /// Get all entries in the process_entities (catalyze) relation for this process
        /// </summary>
        /// <returns>
        /// an array of ServerProcessEntity objects
        /// </returns>
        public ServerProcessEntity[] GetAllProcessEntities()
        {
            return ServerProcessEntity.GetAllForProcess(this.ID);
        }

        /// <summary>
        /// Add the given molecular entity to this process with the specified role
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="role"></param>
        /// <param name="quantity"></param>
        /// <param name="notes"></param>
        public void AddMolecularEntity(Guid entityId, string role, int quantity, string notes)
        {
            ServerProcessEntity.AddMolecularEntityToProcess(this.ID, entityId, role, quantity, notes);
        }

        /// <summary>
        /// Remove the given molecular entity from the process
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="role"></param>
        public void RemoveMolecularEntity(Guid entityId, string role)
        {
            ServerProcessEntity.RemoveMolecularEntityFromProcess(this.ID, entityId, role);
        }

        #endregion


        #region ADO.NET SqlCommands
        /// <summary>
        /// Required function for settign up the SqlCommands for ADO.NET.
        /// </summary>
        protected override void SetSqlCommandParameters()
        {
            // (GJS)
            // Rewrote using DBWrapper.BuildCommand

            __DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + @" (id, name, reversible, location, notes, generic_process_id,fast) VALUES
				(@i_id, @i_name, @i_reversible, @i_location, @i_notes, @i_generic_process_id, @fast);",
                "@i_id", SqlDbType.UniqueIdentifier, ID,
                "@i_name", SqlDbType.VarChar, Name,
                "@i_reversible", SqlDbType.Bit, Reversible,
                "@i_location", SqlDbType.VarChar, Location,
                "@i_notes", SqlDbType.Text, ProcessNotes,
                "@i_generic_process_id", SqlDbType.UniqueIdentifier, GenericProcessID,
                "@i_fast",SqlDbType.Bit,Fast);

            __DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE id = @s_id;",
                "@s_id", SqlDbType.UniqueIdentifier, ID);

            __DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
                "UPDATE " + __TableName + @"
				SET [name] = @u_name, reversible = @u_reversible, location = @u_location,
					notes = @u_notes, generic_process_id = @u_generic_process_id, fast = @fast
				WHERE id = @u_id;",
                "@u_name", SqlDbType.VarChar, Name,
                "@u_reversible", SqlDbType.Bit, Reversible,
                "@u_location", SqlDbType.VarChar, Location,
                "@u_notes", SqlDbType.Text, ProcessNotes,
                "@u_generic_process_id", SqlDbType.UniqueIdentifier, ID,
                "@u_fast", SqlDbType.Bit, Fast);

            __DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE id = @d_id;",
                "@d_id", SqlDbType.UniqueIdentifier, ID);
        }
        #endregion
        #endregion

        */
        public override SoapObject PrepareForSoap(SoapObject derivedObject)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateFromSoap(SoapObject o)
        {
            throw new NotImplementedException();
        }

        protected override void SetSqlCommandParameters()
        {
            throw new NotImplementedException();
        }
    }
}
