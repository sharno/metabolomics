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

	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/ServerObjects/ServerCatalyze.cs</filepath>
	///		<creation>2005/06/16</creation>
	///		<author>
	///			<name>Brandon S. Evans</name>
	///			<initials>bse</initials>
	///			<email>brandon.evans@case.edu</email>
	///		</author>
	///		<contributors>
	///			<contributor>
	///				<name>Michael F. Starke</name>
	///				<initials>mfs</initials>
	///				<email>michael.starke@case.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Brendan Elliott</name>
	///				<initials>BE</initials>
	///				<email>bxe7@cwru.edu</email>
	///			</contributor>
	///			<contributor>
	///				<name>Marc Reynolds</name>
	///				<initials>mrr</initials>
	///				<email>marc.reynolds@case.edu</email>
	///			</contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: ann $</cvs_author>
	///			<cvs_date>$Date: 2009/05/14 14:28:17 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerCatalyze.cs,v 1.2 2009/05/14 14:28:17 ann Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.2 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to biological processes.
	/// </summary>
	#endregion
	public class ServerCatalyze : ServerObject, IGraphCatalyze
	{

		#region Constructor, Destructor, ToString
		private ServerCatalyze()
		{
		}

		/// <summary>
		/// Constructor for server catalyze wrapper with fields initiallized
		/// </summary>
		/// <param name="geneProductId"></param>
		/// <param name="organismGroupId"></param>
		/// <param name="processId"></param>
		/// <param name="ecNumber"></param>
		public ServerCatalyze ( Guid geneProductId, Guid organismGroupId, Guid processId, string ecNumber )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.GeneProductID = geneProductId; // generate a new ID
			this.OrganismGroupID = organismGroupId;
			this.ProcessID = processId;
			this.ECNumber = ecNumber;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="organismGroupId"></param>
		/// <param name="processId"></param>
		public ServerCatalyze ( Guid organismGroupId, Guid processId )
		{
			__DBRow = new DBRow( __TableName );

			this.OrganismGroupID = organismGroupId;
			this.ProcessID = processId;
			this.ECNumber = String.Empty;
			this.GeneProductID = Guid.Empty;
		}

		/// <summary>
		/// Constructor for server catalyze wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerCatalyze object from a
		/// SoapCatalyze object.
		/// </remarks>
		/// <param name="data">
		/// A SoapCatalyze object from which to construct the
		/// ServerCatalyze object.
		/// </param>
		public ServerCatalyze ( SoapCatalyze data )
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
					__DBRow = LoadRow( data.GeneProductID, data.ProcessID, data.OrganismGroupID, data.ECNumber );
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
		public ServerCatalyze ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

		/// <summary>
		/// Destructor for the ServerCatalyze class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerCatalyze()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "catalyzes";
		#endregion


		#region Properties
		/// <summary>
		/// Get/set the gene product ID.
		/// </summary>
		public Guid GeneProductID
		{
			get
			{
				return __DBRow.GetGuid("gene_product_id");
			}
			set
			{
				__DBRow.SetGuid("gene_product_id", value);
			}
		}

		/// <summary>
		/// Get/set the process ID.
		/// </summary>
		public Guid ProcessID
		{
			get
			{
				return __DBRow.GetGuid("process_id");
			}
			set
			{
				__DBRow.SetGuid("process_id", value);
			}
		}
		
		/// <summary>
		/// Get/set the EC number.
		/// </summary>
		public string ECNumber
		{
			get
			{
				return __DBRow.GetString("ec_number");
			}
			set
			{
				__DBRow.SetString("ec_number", value);
			}
		}

		/// <summary>
		/// Get/set the organism group id.
		/// </summary>
		public Guid OrganismGroupID
		{
			get{return __DBRow.GetGuid( "organism_group_id");}
			set{__DBRow.SetGuid( "organism_group_id", value );}
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
			SoapCatalyze retval = (derived == null) ? 
				retval = new SoapCatalyze() : retval = (SoapCatalyze)derived;

			retval.GeneProductID = this.GeneProductID;
			retval.ProcessID = this.ProcessID;
			retval.ECNumber = this.ECNumber;
			retval.OrganismGroupID = this.OrganismGroupID;

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
			SoapCatalyze c = o as SoapCatalyze;

			this.GeneProductID = c.GeneProductID;
			this.ProcessID = c.ProcessID;
			this.ECNumber = c.ECNumber;
			this.OrganismGroupID = c.OrganismGroupID;
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
				"INSERT INTO " + __TableName + " (gene_product_id, process_id, ec_number, organism_group_id) VALUES (@gene_product_id, @process_id, @ec_number, @org_id);",
				"@gene_product_id", SqlDbType.UniqueIdentifier, GeneProductID,
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@org_id", SqlDbType.UniqueIdentifier, OrganismGroupID);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE gene_product_id = @gene_product_id AND process_id = @process_id AND ec_number = @ec_number AND organism_group_name = @org_id;",
				"@gene_product_id", SqlDbType.UniqueIdentifier, GeneProductID,
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@org_id", SqlDbType.UniqueIdentifier, OrganismGroupID);

			//	Don't know how you could use update
			// (sfa) This is an update statement that doesn't do anything.


			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE gene_product_id = @gene_product_id AND process_id = @process_id AND ec_number = @ec_number AND organism_group_id = @org_id;",
				"@gene_product_id", SqlDbType.UniqueIdentifier, GeneProductID,
				"@process_id", SqlDbType.UniqueIdentifier, ProcessID,
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@org_id", SqlDbType.UniqueIdentifier, OrganismGroupID);
		}
		#endregion

		#endregion


		#region Static Methods
		/// <summary>
		/// Return all catalyzing relations from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapCatalyze objects ready to be sent via SOAP.
		/// </returns>
		public static ServerCatalyze[] AllGeneProductAndProcesses ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerCatalyze( new DBRow( d ) ) );
			}

			return ( ServerCatalyze[] ) results.ToArray( typeof( ServerCatalyze ) );
		}

        public static ServerCatalyze[] AllOrganisms()
        {
            SqlCommand command = new SqlCommand("SELECT DISTINCT organism_group_id FROM " + __TableName + ";");

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCatalyze(new DBRow(d)));
            }

            return (ServerCatalyze[])results.ToArray(typeof(ServerCatalyze));
        }

        /// <summary>
        /// Gets all of the gene products involved in the process within a specific organism
        /// </summary>
        /// <param name="process_id"></param>
        /// <returns>
        /// Returns all of the gene products involved in the process within a specific organism
        /// </returns>
        public static ServerGeneProduct[] GetAllGeneProductsForProcessInOrganism(Guid process_id, Guid orgId)
        {
            SqlCommand command = new SqlCommand(@"SELECT gp.* FROM gene_products gp INNER JOIN "
                                                + __TableName +
                                                @" gpap ON gp.[id] = gpap.gene_product_id 
                                                WHERE gpap.process_id = @process_id
                                                AND gpap.organism_group_id = @orgId;");

            SqlParameter ident = new SqlParameter("@process_id", SqlDbType.UniqueIdentifier);
            ident.SourceVersion = DataRowVersion.Original;
            ident.Value = process_id;
            command.Parameters.Add(ident);

            SqlParameter orgIdent = new SqlParameter("@orgId", SqlDbType.UniqueIdentifier);
            orgIdent.SourceVersion = DataRowVersion.Original;
            orgIdent.Value = orgId;
            command.Parameters.Add(orgIdent);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(ServerGeneProduct.LoadFromRow(new DBRow(d)));
            }

            return (ServerGeneProduct[])results.ToArray(typeof(ServerGeneProduct));
        }

		/// <summary>
		/// Returns a single ServerCatalyze object
		/// </summary>
		/// <param name="org_group_id"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
		/// <returns>
		/// Object ready to be sent via SOAP.
		/// </returns>
		public static ServerCatalyze Load ( Guid gene_product_id, Guid process_id, Guid org_group_id, string ec_number )
		{
			return new ServerCatalyze( LoadRow ( gene_product_id, process_id, org_group_id, ec_number ) );
		}

		/// <summary>
		/// Return the dataset for an object with the given parameters.
		/// </summary>
		/// <param name="orgGroupId"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( Guid gene_product_id, Guid process_id, Guid orgGroupId, string ec_number )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE gene_product_id = @gene_product_id AND process_id = @process_id AND ec_number = @ec_number AND organism_group_id = @org_id;",
				"@gene_product_id", SqlDbType.UniqueIdentifier, gene_product_id,
				"@process_id", SqlDbType.UniqueIdentifier, process_id,
				"@ec_number", SqlDbType.VarChar, ec_number,
				"@org_id", SqlDbType.UniqueIdentifier, orgGroupId);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}

		/// <summary>
		/// Returns the entry in the gene_product_and_processes table with
		/// the given ec number.  If there is no entry NULL is returned
		/// </summary>
		/// <param name="ecNum">
		/// the ec number
		/// </param>
		/// <returns></returns>
		public static ServerCatalyze LoadFromECNumber ( string ecNum)
		{
			if (ecNum == null || ecNum.Trim() == "")
			{
				throw new DataModelException("To load catalyze relation elements with a NULL E.C. number, use LoadAllWithUnknownECNumber().");
			}
            SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number;",
				"@ec_number", SqlDbType.VarChar, ecNum);

			// mrr - allow this to return null
			DataSet ds;
			int count = DBWrapper.LoadSingle( out ds, ref command, true);
			if(count == 0) return null;
			if(count > 1)
				throw new PathwaysLib.Exceptions.DataModelException("EC Number {0} returned more than one product<->process relationship", ecNum);
			return new ServerCatalyze( new DBRow(ds) );
		}

		/// <summary>
		/// Should return all entries with a null ec number, I don't know if it will
		/// </summary>
		/// <returns></returns>
		public static ServerCatalyze[] LoadAllWithUnknownECNumber()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number IS NULL;" );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerCatalyze( new DBRow( d ) ) );
			}

			return ( ServerCatalyze[] ) results.ToArray( typeof( ServerCatalyze ) );
		}

		/// <summary>
		/// Check if a catalyzing relation already exists
		/// </summary>
		/// <param name="org_id"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
		/// <returns></returns>
		public static bool Exists( Guid gene_product_id, Guid process_id, Guid org_id, string ec_number )
		{
			SqlCommand command = DBWrapper.BuildCommand(
                "SELECT * FROM " + __TableName + " WHERE gene_product_id = @gene_product_id AND process_id = @process_id AND ec_number = @ec_number AND organism_group_id = @org_id;",
				"@gene_product_id", SqlDbType.UniqueIdentifier, gene_product_id,
				"@process_id", SqlDbType.UniqueIdentifier, process_id,
				"@ec_number", SqlDbType.VarChar, ec_number,
				"@org_id", SqlDbType.UniqueIdentifier, org_id);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Create a new catalyzing relation
		/// </summary>
		/// <param name="orgGroupId"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
		public static void AddGeneProductToProcess(Guid gene_product_id, Guid process_id, Guid orgGroupId, String ec_number)
		{
			//(bse)
			// check if the process already belongs to the pathway
			//
			if ( !Exists(gene_product_id, process_id, orgGroupId, ec_number ) )
			{
				DBWrapper.Instance.ExecuteNonQuery(
                    "INSERT INTO " + __TableName + " ( gene_product_id, process_id, ec_number, organism_group_id) VALUES ( @gene_product_id, @process_id, @ec_number, @org_id );",
					"@gene_product_id", SqlDbType.UniqueIdentifier, gene_product_id,
					"@process_id", SqlDbType.UniqueIdentifier, process_id,
					"@ec_number", SqlDbType.VarChar, ec_number,
					"@org_id", SqlDbType.UniqueIdentifier, orgGroupId);
			}
			else 
			{
				//do nothing, the relation already exists
			}
		}		

		/// <summary>
		/// Removes the selected relation from the table.
		/// </summary>
		/// <param name="orgGroupId"></param>
		/// <param name="gene_product_id"></param>
		/// <param name="process_id"></param>
		/// <param name="ec_number"></param>
		public static void RemoveGeneProductFromProcess ( Guid gene_product_id, Guid process_id, Guid orgGroupId, string ec_number )
		{
			DBWrapper.Instance.ExecuteNonQuery(				
				"DELETE FROM " + __TableName + " WHERE gene_product_id = @gene_product_id AND process_id = @process_id AND ec_number = @ec_number AND organism_group_id = @org_id;",
				"@gene_product_id", SqlDbType.UniqueIdentifier, gene_product_id,
				"@process_id", SqlDbType.UniqueIdentifier, process_id,
				"@ec_number", SqlDbType.VarChar, ec_number,
				"@org_id", SqlDbType.UniqueIdentifier, orgGroupId);
		}

		/// <summary>
		/// This will be used by ServerGeneProduct
		/// </summary>
		/// <param name="gene_product_id"></param>
		/// <returns>
		/// Returns all of the processes involving the given gene product
		/// </returns>
		public static ServerProcess[] GetAllProcessesForGeneProduct( Guid gene_product_id )
		{
			SqlCommand command = new SqlCommand( "SELECT p.* FROM processes p INNER JOIN " + __TableName + " gpap ON p.[id] = gpap.process_id WHERE gpap.gene_product_id = @gene_product_id);" );
			SqlParameter ident = new SqlParameter( "@gene_product_id", SqlDbType.UniqueIdentifier );
			ident.SourceVersion = DataRowVersion.Original;
			ident.Value = gene_product_id;
			command.Parameters.Add( ident );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerProcess( new DBRow( d ) ) );
			}

			return ( ServerProcess[] ) results.ToArray( typeof( ServerProcess ) );
		}

		/// <summary>
		/// This will be used by ServerProcess
		/// </summary>
		/// <param name="process_id"></param>
		/// <returns>
		/// Returns all of the gene products involved in the process
		/// </returns>
		public static ServerGeneProduct[] GetAllGeneProductsForProcess( Guid process_id )
		{
			SqlCommand command = new SqlCommand( "SELECT gp.* FROM gene_products gp INNER JOIN " + __TableName + " gpap ON gp.[id] = gpap.gene_product_id WHERE gpap.process_id = @process_id;" );
			SqlParameter ident = new SqlParameter( "@process_id", SqlDbType.UniqueIdentifier );
			ident.SourceVersion = DataRowVersion.Original;
			ident.Value = process_id;
			command.Parameters.Add( ident );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( ServerGeneProduct.LoadFromRow( new DBRow( d ) ) );
			}

			return ( ServerGeneProduct[] ) results.ToArray( typeof( ServerGeneProduct ) );
		}

        /// <summary>
        /// This will be used by ServerGenericProcess
        /// </summary>
        /// <param name="genericProcessId"></param>
        /// <returns>
        /// Returns all of the gene products involved in the generic process
        /// </returns>
        public static ServerGeneProduct[] GetAllGeneProductsForGenericProcess( Guid genericProcessId )
        {
            SqlCommand command = new SqlCommand( "SELECT gp.* FROM gene_products gp INNER JOIN " + __TableName + " gpap ON gp.[id] = gpap.gene_product_id INNER JOIN processes p ON p.id = gpap.process_id WHERE p.generic_process_id = @generic_process_id;" );
            SqlParameter ident = new SqlParameter( "@generic_process_id", SqlDbType.UniqueIdentifier );
            ident.SourceVersion = DataRowVersion.Original;
            ident.Value = genericProcessId;
            command.Parameters.Add( ident );

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple( out ds, ref command );

            ArrayList results = new ArrayList();
            foreach ( DataSet d in ds )
            {
                results.Add( ServerGeneProduct.LoadFromRow( new DBRow( d ) ) );
            }

            return ( ServerGeneProduct[] ) results.ToArray( typeof( ServerGeneProduct ) );
        }

        public static List<ServerGeneProduct> GetAllGeneProductsForPathway(Guid pathwayId)
        {
            List<ServerGeneProduct> results = new List<ServerGeneProduct>();
            List<ServerMolecularEntity> meList = ServerMolecularEntity.SelectMolecularEntities(
                "catalyzes c, pathway_processes pp",
                "m.id = c.gene_product_id AND c.process_id = pp.process_id AND pp.pathway_id = @pathwayId",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);

            foreach (ServerMolecularEntity me in meList)
            {
                results.Add((ServerGeneProduct)me);
            }

            return results;
        }

//        public static ServerGeneProduct[] SelectGeneProducts(string fromClause, string whereClause, params object[] paramNameTypeValueList)
//        {
//            DataSet[] ds;
//            SqlCommand commandMolecule = DBWrapper.BuildCommand(
//                     @"SELECT m.*
//					FROM molecular_entities m, (
//						SELECT DISTINCT gp.id
//							FROM gene_products pe " + (fromClause != null ? ", " + fromClause : "") + @"
//							" + (whereClause != null ? " WHERE " + whereClause + " " : "") + @" ) AS uniqueGP
//					WHERE uniqueGP.id = m.id;",
//                paramNameTypeValueList);
//            Dictionary<Guid, DBRow> molecules = new Dictionary<Guid, DBRow>();
//            if (DBWrapper.LoadMultiple(out ds, ref commandMolecule) > 0)
//            {
//                foreach (DataSet d in ds)
//                {
//                    DBRow r = new DBRow(d);
//                    molecules.Add(r.GetGuid("id"), r);
//                }
//            }

//            SqlCommand command = DBWrapper.BuildCommand(
//                @"SELECT gp2.*
//					FROM gene_products gp2, (
//						SELECT DISTINCT gp.id
//							FROM gene_products pe " + (fromClause != null ? ", " + fromClause : "") + @"
//							" + (whereClause != null ? " WHERE " + whereClause + " " : "") + @" ) AS uniqueGP
//					WHERE uniqueGP.id = gp2.id;",
//                paramNameTypeValueList);

//            ArrayList results = new ArrayList();
//            if (DBWrapper.LoadMultiple(out ds, ref command) > 0)
//            {
//                foreach (DataSet d in ds)
//                {
//                    DBRow r = new DBRow(d);
//                    results.Add(new ServerGeneProduct(r, molecules[r.GetGuid("id")])); // use bulk-loaded parent class rows
//                }
//            }
//            return (ServerGeneProduct[])results.ToArray(typeof(ServerGeneProduct));
        //        }

        #region Used for graph building

        public static Dictionary<Guid, List<string>> GetECNumbersForGenericProcessesInPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.generic_process_id, c.ec_number
                FROM processes p, catalyzes c, pathway_processes pp
                WHERE p.id = c.process_id AND p.id = pp.process_id AND pp.pathway_id = @pathwayId AND c.ec_number IS NOT NULL",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            Dictionary<Guid, List<string>> results = new Dictionary<Guid,List<string>>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (!results.ContainsKey((Guid)r["generic_process_id"]))
                {
                    results.Add((Guid)r["generic_process_id"], new List<string>());
                }
                results[(Guid)r["generic_process_id"]].Add((string)r["ec_number"]);
            }

            return results;
        }

        public static Dictionary<Guid, List<Guid>> GetGeneProductsForGenericProcessesInPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.generic_process_id, c.gene_product_id
                FROM processes p, catalyzes c, pathway_processes pp
                WHERE p.id = c.process_id AND p.id = pp.process_id AND pp.pathway_id = @pathwayId AND c.gene_product_id IS NOT NULL",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            Dictionary<Guid, List<Guid>> results = new Dictionary<Guid, List<Guid>>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (!results.ContainsKey((Guid)r["generic_process_id"]))
                {
                    results.Add((Guid)r["generic_process_id"], new List<Guid>());
                }
                results[(Guid)r["generic_process_id"]].Add((Guid)r["gene_product_id"]);
            }

            return results;
        }

        public static Dictionary<Guid, List<Guid>> GetGeneProductsForGenericProcesses(Guid genericProcessId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.generic_process_id, c.gene_product_id
                FROM processes p, catalyzes c
                WHERE p.id = c.process_id AND p.generic_process_id = @genericProcessId  AND c.gene_product_id IS NOT NULL",
                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            Dictionary<Guid, List<Guid>> results = new Dictionary<Guid, List<Guid>>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                //Guid genericProcessId = (Guid)r["generic_process_id"];
                if (!results.ContainsKey(genericProcessId))
                {
                    results.Add(genericProcessId, new List<Guid>());
                }
                results[genericProcessId].Add((Guid)r["gene_product_id"]);
            }

            return results;
        }

        public static Dictionary<Guid, List<Guid>> GetOrganismGroupsForGenericProcessesInPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.generic_process_id, c.organism_group_id
                FROM processes p, catalyzes c, pathway_processes pp
                WHERE p.id = c.process_id AND p.id = pp.process_id AND c.organism_group_id IS NOT NULL
                    AND pp.pathway_id = @pathwayId",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            Dictionary<Guid, List<Guid>> results = new Dictionary<Guid, List<Guid>>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (!results.ContainsKey((Guid)r["generic_process_id"]))
                {
                    results.Add((Guid)r["generic_process_id"], new List<Guid>());
                }
                results[(Guid)r["generic_process_id"]].Add((Guid)r["organism_group_id"]);
            }

            return results;
        }

        public static Dictionary<Guid, List<Guid>> GetOrganismGroupsForGenericProcesses(Guid genericProcessId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.generic_process_id, c.organism_group_id
                FROM processes p, catalyzes c
                WHERE p.id = c.process_id AND p.generic_process_id = @genericProcessId AND c.organism_group_id IS NOT NULL",
                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            Dictionary<Guid, List<Guid>> results = new Dictionary<Guid, List<Guid>>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (!results.ContainsKey((Guid)r["generic_process_id"]))
                {
                    results.Add((Guid)r["generic_process_id"], new List<Guid>());
                }
                results[(Guid)r["generic_process_id"]].Add((Guid)r["organism_group_id"]);
            }

            return results;
        }

        public static ServerCatalyze[] LoadAllForGenericProcesses(Guid genericProcessId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT DISTINCT c.* FROM " + __TableName + " c, processes p WHERE p.id = c.process_id AND p.generic_process_id = @genericProcessId;",
                    "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCatalyze(new DBRow(d)));
            }

            return (ServerCatalyze[])results.ToArray(typeof(ServerCatalyze));
        }

        public static ServerCatalyze[] LoadAllInPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT DISTINCT c.* FROM " + __TableName + " c, pathway_processes pp WHERE pp.process_id = c.process_id AND pp.pathway_id = @pathwayId",
                    "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerCatalyze(new DBRow(d)));
            }

            return (ServerCatalyze[])results.ToArray(typeof(ServerCatalyze));
        }


        public static Dictionary<Guid, List<Guid>> GetOrganismGroupsForSpecificProcessesInGenericProcess(Guid genericProcessId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT p.id, c.organism_group_id
                FROM processes p, catalyzes c
                WHERE p.id = c.process_id AND p.generic_process_id = @genericProcessId AND c.organism_group_id IS NOT NULL",
                "@genericProcessId", SqlDbType.UniqueIdentifier, genericProcessId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            Dictionary<Guid, List<Guid>> results = new Dictionary<Guid, List<Guid>>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (!results.ContainsKey((Guid)r["generic_process_id"]))
                {
                    results.Add((Guid)r["generic_process_id"], new List<Guid>());
                }
                results[(Guid)r["generic_process_id"]].Add((Guid)r["organism_group_id"]);
            }

            return results;
        }

        public static List<Guid> GetOrganismGroupsForPathway(Guid pathwayId)
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT DISTINCT pp.pathway_id, c.organism_group_id
                FROM catalyzes c, pathway_processes pp
                WHERE pp.process_id = c.process_id AND pp.pathway_id = @pathwayId AND c.organism_group_id IS NOT NULL",
                "@pathwayId", SqlDbType.UniqueIdentifier, pathwayId);

            DataSet ds;
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);
            List<Guid> results = new List<Guid>();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                results.Add((Guid)r["organism_group_id"]);
            }

            return results;
        }

        #endregion

        #region Ali's (GetECNumberFor...) queries


        /// <summary>
		/// Gets the ECNumber for an enzyme in a given process
		/// </summary>
		/// <param name="processId"></param>
		/// <param name="geneProductId"></param>
		/// <returns></returns>
		public static ServerECNumber GetECNumberForGeneProductAndProcess ( Guid processId, Guid geneProductId )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT ec.* FROM " + __TableName + " gpp INNER JOIN ec_numbers ec ON gpp.ec_number = ec.ec_number WHERE gpp.process_id = @process_id AND gpp.gene_product_id = @gene_product_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processId,
				"@gene_product_id", SqlDbType.UniqueIdentifier, geneProductId );

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command, true );
			if ( ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1 )
			{
				return null;
			}
			return new ServerECNumber ( new DBRow ( ds ) );
		}

        /// <summary>
        /// Gets the EC number for an enzyme and process in an organism
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="geneProductId"></param>
        /// <param name="organismId"></param>
        /// <returns></returns>
		public static ServerECNumber GetECNumberForGeneProductandProcessInOrganism ( Guid processId, Guid geneProductId,  Guid organismId)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT ec.* FROM " + __TableName + " gpp INNER JOIN ec_numbers ec ON gpp.ec_number = ec.ec_number WHERE gpp.process_id = @process_id AND gpp.gene_product_id = @gene_product_id AND gpp.organism_group_id = @organism_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processId,
				"@gene_product_id", SqlDbType.UniqueIdentifier, geneProductId,
				"@organism_id", SqlDbType.UniqueIdentifier, organismId);

			DataSet ds;
			DBWrapper.LoadSingle( out ds, ref command, true );
			if ( ds == null || ds.Tables.Count != 1 || ds.Tables[0].Rows.Count != 1 )
			{
				return null;
			}
			return new ServerECNumber ( new DBRow ( ds ) );
		}

		/// <summary>
		/// Get's the ECNumber(s) for a given process
		/// </summary>
		/// <param name="processId"></param>
		/// <returns></returns>
		public static ServerECNumber[] GetECNumbersForProcess ( Guid processId )
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT ec.* FROM " + __TableName + " gpp INNER JOIN ec_numbers ec ON gpp.ec_number = ec.ec_number WHERE gpp.process_id = @process_id;",
				"@process_id", SqlDbType.UniqueIdentifier, processId );

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			ArrayList results = new ArrayList();
			foreach ( DataSet d in ds )
			{
				results.Add( new ServerECNumber( new DBRow( d ) ) );
			}

			return ( ServerECNumber[] ) results.ToArray( typeof( ServerECNumber ) );
		}

 
        /// <summary>
        /// Get's the ECNumber(s) for a given generic process
        /// </summary>
        /// <param name="genericProcessId"></param>
        /// <returns></returns>
        public static ServerECNumber[] GetECNumbersForGenericProcess ( Guid genericProcessId )
        {
            SqlCommand command = DBWrapper.BuildCommand(
                "SELECT ec2.* FROM ec_numbers ec2, (SELECT DISTINCT ec.ec_number FROM " + __TableName + " gpp INNER JOIN ec_numbers ec ON gpp.ec_number = ec.ec_number INNER JOIN processes p ON p.id = gpp.process_id WHERE p.generic_process_id = @generic_process_id) AS uniqueEC WHERE uniqueEC.ec_number = ec2.ec_number;",
                "@generic_process_id", SqlDbType.UniqueIdentifier, genericProcessId );

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple( out ds, ref command );

            ArrayList results = new ArrayList();
            foreach ( DataSet d in ds )
            {
                results.Add( new ServerECNumber( new DBRow( d ) ) );
            }

            return ( ServerECNumber[] ) results.ToArray( typeof( ServerECNumber ) );
        }

		#endregion

		#endregion


    } // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerCatalyze.cs,v 1.2 2009/05/14 14:28:17 ann Exp $
	$Log: ServerCatalyze.cs,v $
	Revision 1.2  2009/05/14 14:28:17  ann
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.8  2008/04/28 17:34:53  brendan
	fixed bugs in insert/delete/exists/load functions of ServerCatalyze involving org_id; updated unit test for ServerCatalyze
	
	Revision 1.7  2007/02/09 23:16:31  brendan
	Modified new graph data web service to include catalyzes table data directly
	
	Revision 1.6  2007/02/07 23:55:09  brendan
	*** empty log message ***
	
	Revision 1.5  2006/11/26 21:49:25  ali
	*** empty log message ***
	
	Revision 1.4  2006/10/19 21:03:36  brendan
	New graph drawing code ... performs bulk-loading of server objects to reduce the number of queries and filling an object cache.  Also provides an interface for alternative data sources (i.e. XML biopax doc).  Other misc bug fixes.
	
	Revision 1.3  2006/10/06 21:26:34  ali
	*** empty log message ***
	
	Revision 1.2  2006/10/03 17:47:44  brendan
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.23  2006/06/13 01:52:04  ali
	*** empty log message ***
	
	Revision 1.22  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	
	
	Revision 1.21  2006/04/21 17:37:29  michael
	*** empty log message ***
	Revision 1.20.6.1  2006/02/21 22:18:09  marc
	Modified LoadFromECNumber method so that it can return a null ServerCatalyze object if there are no EC catalysts associated with the EC number
	
	Revision 1.20  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.19  2005/11/02 20:35:16  fatih
	Test functions are working for pathwayslib objects
	
	Revision 1.18  2005/10/31 19:25:11  fatih
	*** empty log message ***
	
	Revision 1.17  2005/08/24 22:27:52  michael
	Fixing SQL Connection Overflow bug
	
	Revision 1.16  2005/08/09 00:28:34  michael
	Adding organism selection to browser
	
	Revision 1.15  2005/08/08 20:13:38  michael
	Website content updates
	
	Revision 1.14  2005/08/03 19:18:44  brandon
	*** empty log message ***
	
	Revision 1.13  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.12  2005/07/15 21:02:00  brandon
	added more queries
	
	Revision 1.11  2005/07/15 17:42:47  michael
	Debugging Pathway Details display exception
	
	Revision 1.10  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.9  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.8  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	Revision 1.7  2005/07/11 22:28:57  brendan
	Added inheritance support for GeneProduct and Protein, but they are not fully tested yet.
	
	Revision 1.6  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.5  2005/07/08 21:55:07  brendan
	Debugging MolecularEntity/EntityNames/Gene inheritance.  Inheritance test not passing yet.
	
	Revision 1.4  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.3  2005/07/08 19:32:05  brandon
	fixed ServerCatalyze, sort of,  and uh, this project builds now
	
	Revision 1.2  2005/07/07 19:42:19  brandon
	did more on the catalyzes relation, don't know exactly how to get EC# more involved (?)
	
	Revision 1.1  2005/07/07 15:10:28  brandon
	Added ServerCatalyze.cs (gene_product_and_processes), it's not done yet, and added the GetAllOrganismGroups function to ServerProcess object
	

		
------------------------------------------------------------------------*/
#endregion