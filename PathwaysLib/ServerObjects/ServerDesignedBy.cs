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

	public class ServerDesignedBy : ServerObject
	{

		#region Constructor, Destructor, ToString
        public ServerDesignedBy()
		{
		}

		/// <summary>
		/// Constructor for serverDesignedBy wrapper with fields initiallized
		/// </summary>
		/// <param name="geneProductId"></param>
		/// <param name="organismGroupId"></param>
		/// <param name="processId"></param>
		/// <param name="ecNumber"></param>
		public ServerDesignedBy ( Guid Id, Guid ModelMetadataId, Guid Authorid )
		{
			
			__DBRow = new DBRow( __TableName );

            this.Id = Id; 
            this.ModelMetadataId = ModelMetadataId;
            this.AuthorId = AuthorId;
			
		}

        public ServerDesignedBy(DBRow data)
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

		/// <summary>
        /// Destructor for the serverDesignedBy class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
        ~ServerDesignedBy()
		{
		}
		#endregion


		#region Member Variables
        private static readonly string __TableName = "DesignedBy";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the ID.
		/// </summary>
		public Guid Id
		{
			get
			{
				return __DBRow.GetGuid("Id");
			}
			set
			{
				__DBRow.SetGuid("Id", value);
			}
		}

		/// <summary>
		/// Get/set the process ModelMetadataId.
		/// </summary>
        public Guid ModelMetadataId
		{
			get
			{
                return __DBRow.GetGuid("ModelMetadataId");
			}
			set
			{
                __DBRow.SetGuid("ModelMetadataId", value);
			}
		}
		
		/// <summary>
		/// Get/set the Author.
		/// </summary>
        public Guid AuthorId
		{
			get
			{
				return __DBRow.GetGuid("AuthorId");
			}
			set
			{
                __DBRow.SetGuid("AuthorId", value);
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
		public override SoapObject PrepareForSoap ( SoapObject derived )
		{
            SoapDesignedBy retval = (derived == null) ?
                retval = new SoapDesignedBy() : retval = (SoapDesignedBy)derived;

            retval.Id = this.Id;
            retval.ModelMetadataId = this.ModelMetadataId;
            retval.AuthorId = this.AuthorId;

			retval.Status = ObjectStatus.NoChanges;

			return retval;
		}

		/// <summary>
		/// Consumes a SoapObject object and updates the ServerCatalyze
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the Catalyze relation.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
            SoapDesignedBy c = o as SoapDesignedBy;

			this.Id = c.Id;
			this.ModelMetadataId = c.ModelMetadataId;
			this.AuthorId = c.AuthorId;
		}
		#region THIS IS ALL THAT's LEFT TO CHECK OUT

//		#region Catalyzes Relation
//		/// <summary>
//		/// Adds a gene product to the catalyzing relation
//		/// </summary>
//		/// <param name="gene_product_id"></param>
//		/// <param name="ec_number"></param>
//		public void AddGeneProduct(Guid gene_product_id, string ec_number)
//		{
//			ServerCatalyze.AddGeneProductToProcess( gene_product_id, this.ID, ec_number );
//		}
//
//		/// <summary>
//		/// Remove a gene product from the process
//		/// </summary>
//		/// <param name="gene_product_id"></param>
//		/// <param name="ec_number"></param>
//		public void RemoveGeneProduct(Guid gene_product_id, string ec_number)
//		{
//			ServerCatalyze.RemoveGeneProductFromProcess( gene_product_id, this.ID, ec_number);
//		}
//
//		/// <summary>
//		/// Get all gene products
//		/// </summary>
//		/// <returns>
//		/// Returns all of the gene products involved in the process
//		/// </returns>
//		public ServerGeneProduct[] GetAllGeneProducts( )
//		{
//			ServerCatalyze.GetAllGeneProductsForProcess( this.ID );
//		}
//
//		#endregion
//
		#endregion

		#region ADO.NET SqlCommands

		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (BE) rewrote using BuildCommand()

			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
                "INSERT INTO " + __TableName + " (Id, ModelMetadataId, AuthorId) VALUES (@Id, @ModelMetadataId, @AuthorId);",
                "@Id", SqlDbType.UniqueIdentifier, Id,
                "@ModelMetadataId", SqlDbType.UniqueIdentifier, ModelMetadataId,	
                "@AuthorId", SqlDbType.UniqueIdentifier, AuthorId);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE Id = @Id AND ModelMetadataId = @ModelMetadataId AND AuthorId = @AuthorId;",
                "@Id", SqlDbType.UniqueIdentifier, Id,
                "@ModelMetadataId", SqlDbType.UniqueIdentifier, ModelMetadataId,
                "@AuthorId", SqlDbType.UniqueIdentifier, AuthorId);

			//	Don't know how you could use update
			// (sfa) This is an update statement that doesn't do anything.


			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
                "DELETE FROM " + __TableName + " WHERE Id = @Id AND ModelMetadataId = @ModelMetadataId AND AuthorId = @AuthorId;",
                "@Id", SqlDbType.UniqueIdentifier, Id,
                "@ModelMetadataId", SqlDbType.UniqueIdentifier, ModelMetadataId,
                "@AuthorId", SqlDbType.UniqueIdentifier, AuthorId);
		}
		#endregion

		#endregion


		#region Static Methods
		/// <summary>
		/// Return all model - author rows from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapCatalyze objects ready to be sent via SOAP.
		/// </returns>
        public static ServerDesignedBy[] AllDesignedBy()
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
                results.Add(new ServerDesignedBy(new DBRow(d)));
			}

            return (ServerDesignedBy[])results.ToArray(typeof(ServerDesignedBy));
		}


		/// <summary>
		/// Check if a row already exists
		/// </summary>
		/// <param name="org_id"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
		/// <returns></returns>
        public static bool Exists(Guid ModelMetadataId, Guid AuthorId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE ModelMetadataId = @ModelMetadataId AND AuthorId = @AuthorId",
                "@ModelMetadataId", SqlDbType.UniqueIdentifier, ModelMetadataId,
                "@AuthorId", SqlDbType.UniqueIdentifier, AuthorId);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Create a new author - model row
		/// </summary>
		/// <param name="orgGroupId"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
        public void AddDesignedBy(Guid Id, Guid ModelMetadataId, Guid AuthorId)
		{
			//(bse)
			// check if the process already belongs to the pathway
			//
            if (!Exists( ModelMetadataId, AuthorId))
			{
				DBWrapper.Instance.ExecuteNonQuery(
                    "INSERT INTO " + __TableName + " ( Id, ModelMetadataId, AuthorId) VALUES ( @Id, @ModelMetadataId, @AuthorId );",
                    "@Id", SqlDbType.UniqueIdentifier, Id,
                    "@ModelMetadataId", SqlDbType.UniqueIdentifier, ModelMetadataId,
                    "@AuthorId", SqlDbType.UniqueIdentifier, AuthorId);
			}
			
		}		

		/// <summary>
		/// Removes the selected relation from the table.
		/// </summary>
		/// <param name="orgGroupId"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
        public void RemoveDesignedBy(Guid Id, Guid ModelMetadataId, Guid AuthorId)
		{
			DBWrapper.Instance.ExecuteNonQuery(
                "DELETE FROM " + __TableName + " WHERE Id = @Id AND ModelMetadataId = @ModelMetadataId AND AuthorId = @AuthorId",
                    "@Id", SqlDbType.UniqueIdentifier, Id,
                    "@ModelMetadataId", SqlDbType.UniqueIdentifier, ModelMetadataId,
                    "@AuthorId", SqlDbType.UniqueIdentifier, AuthorId);
		}

		#endregion


    } // End class

} // End namespace
