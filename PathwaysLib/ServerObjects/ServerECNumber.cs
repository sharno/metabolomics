#region Using Declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using PathwaysLib.SoapObjects;
using PathwaysLib.ServerObjects;
using PathwaysLib.Exceptions;
using PathwaysLib.Utilities;
#endregion

namespace PathwaysLib.ServerObjects
{	
	
	#region Document Comments
	/// <sourcefile>
	///		<project>Pathways</project>
	///		<filepath>PathwaysLib/Server/ServerECNumber.cs</filepath>
	///		<creation>2005/06/30</creation>
	///		<author>	
	///			<name>Brandon Evans</name>
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
	///				<name>Suleyman Fatih Akgul</name>
	///				<initials>sfa</initials>
	///				<email>fatih@case.edu</email>
	///			</contributor>contributor>
	///			<contributor>
	///				<name>Marc Reynolds</name>
	///				<initials>mrr</initials>
	///				<email>marc.reynolds@case.edu</email>
	///			</contributor>contributor>
	///		</contributors>
	///		<cvs>
	///			<cvs_author>$Author: ali $</cvs_author>
	///			<cvs_date>$Date: 2009/05/01 06:34:34 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/ServerObjects/ServerECNumber.cs,v 1.3 2009/05/01 06:34:34 ali Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.3 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Encapsulates database access related to basic molecules.
	/// </summary>
	#endregion
	public class ServerECNumber : ServerObject
	{

		#region Constructor, Destructor, ToString
		private ServerECNumber ( )
		{
		}

		/// <summary>
		/// Constructor for server ec number wrapper with fields initiallized
		/// </summary>
		/// <param name="ecNumber"></param>
		/// <param name="name"></param>
		/// <param name="nodeCode"></param>
		public ServerECNumber ( string ecNumber, string name, string nodeCode )
		{
			// not yet in DB, so create empty row
			__DBRow = new DBRow( __TableName );

			this.ECNumber = ecNumber;
			this.Name = name;
			this.NodeCode = nodeCode;
		}

		/// <summary>
		/// Constructor for server ec number wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerECNumber object from a
		/// SoapECNumber object.
		/// </remarks>
		/// <param name="data">
		/// A SoapECNumber object from which to construct the
		/// ServerECNumber object.
		/// </param>
		public ServerECNumber ( SoapECNumber data )
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
						__DBRow = LoadRow( data.ECNumber );
					break;
				default:
					throw new DataModelException("Cannot create ServerObject from invalid SoapObject.");
			}

			// (BE) get potential updates from Soap object, unless it's supposed to be read only
			if (data.Status != ObjectStatus.ReadOnly)
				UpdateFromSoap(data);

		}

		/// <summary>
		/// Constructor for server ec number wrapper.
		/// </summary>
		/// <remarks>
		/// This constructor creates a ServerECNumber object from a
		/// DBRow.
		/// </remarks>
		/// <param name="data">
		/// DBRow to load into the object.
		/// </param>
		public ServerECNumber ( DBRow data )
		{
			// (mfs)
			// setup object
			__DBRow = data;

		}

		/// <summary>
		/// Destructor for the ServerECNumber class.
		/// </summary>
		/// <remarks>
		/// Call base constructor to ensure proper updates.
		/// </remarks>
		~ServerECNumber()
		{
		}
		#endregion


		#region Member Variables
		private static readonly string __TableName = "ec_numbers";

		bool nameDirty = false;
		Hashtable ecNumberNames = null;
		#endregion

		#region Properties
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
		/// Get/set the EC number default name.
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
				AddName(value, "primary name"); // set this as the primary name
				nameDirty = true;
			}
		}

		/// <summary>
		/// Get/set the EC number nodeCode.
		/// </summary>
		public string NodeCode
		{
			get
			{
				return __DBRow.GetString("nodeCode");
			}
			set
			{
				__DBRow.SetString("nodeCode", value);
			}
		}
		#endregion


		#region Methods

        public static ServerECNumber[] LoadRootECNumbers(bool withMapping)
        {            
            //try
            //{
                string query = "";
                if (!withMapping)
                    query = "SELECT * FROM " + __TableName + " where len(nodeCode) = charindex('.', nodeCode);";
                else
                    query = "SELECT * FROM " + __TableName + @" ec where len(nodeCode) = charindex('.', nodeCode)
                            AND EXISTS (SELECT ecNumber 
                                        FROM MapReactionECNumber mec, ec_numbers er
                                        WHERE mec.ecNumber = er.ec_number
                                        AND er.nodeCode like ec.nodeCode + '%');";

                SqlCommand command = DBWrapper.BuildCommand(query);
                DataSet[] ds;
                DBWrapper.LoadMultiple(out ds, ref command);
                ArrayList results = new ArrayList();
                foreach (DataSet d in ds)
                {
                    results.Add(new ServerECNumber(new DBRow(d)));
                }

                return (ServerECNumber[])results.ToArray(typeof(ServerECNumber));
                //return new ServerECNumber[0];
            //}
            //catch (Exception e)
            //{
            //    new Exception(e.Message);
            //    return null;
            //}
        }

        public ServerECNumber[] LoadChildECNumbers(bool withMapping)
        {
            string childrenIds = ServerECNodeCode.Instance.GetChildTerms(this.ECNumber);
            if (childrenIds.Length == 0)
                return new ServerECNumber[0];
            string query = "";

            if (!withMapping)
                query = "SELECT * FROM " + __TableName + " where ec_number IN " + childrenIds + ";";
            else
                query = "SELECT * FROM " + __TableName + " ec where ec_number IN " + childrenIds + @" 
                            AND EXISTS (SELECT ecNumber 
                                        FROM MapReactionECNumber mec, ec_numbers er
                                        WHERE mec.ecNumber = er.ec_number
                                        AND er.nodeCode like ec.nodeCode + '%');";

            SqlCommand command = DBWrapper.BuildCommand(query);
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerECNumber(new DBRow(d)));
            }

            return (ServerECNumber[])results.ToArray(typeof(ServerECNumber));
        }

        public int GetChildCount()
        {
            return ServerECNodeCode.Instance.GetChildCount(ECNumber);
        }

        public int GetMappedReactionCount()
        {           
            string query = "";

            query = @"SELECT count(id) FROM Reaction 
                      WHERE id IN(SELECT reactionId 
                                        FROM MapReactionECNumber mec, ec_numbers er
                                        WHERE mec.ecNumber = er.ec_number
                                        AND er.nodeCode like '" + NodeCode + @"%');";

            SqlCommand command = DBWrapper.BuildCommand(query);
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);
            return int.Parse(ds[0].Tables[0].Rows[0][0].ToString());            
        }

        public ServerReaction[] GetMappedReactions()
        {
            string query = "";

            query = @"SELECT * FROM Reaction 
                      WHERE id IN(SELECT reactionId 
                                        FROM MapReactionECNumber mec
                                        WHERE mec.ecNumber = '" + this.ECNumber + "')";

            SqlCommand command = DBWrapper.BuildCommand(query);
            DataSet[] ds;
            DBWrapper.LoadMultiple(out ds, ref command);
            ArrayList results = new ArrayList();
            foreach (DataSet d in ds)
            {
                results.Add(new ServerReaction(new DBRow(d)));
            }

            return (ServerReaction[])results.ToArray(typeof(ServerReaction));
        }

        /// <summary>
        /// Return all model annotations for this organism.
        /// </summary>
        /// <returns></returns>

        public Dictionary<Guid, BiomodelAnnotation> GetAllMappingAnnotations()
        {
            SqlCommand command = DBWrapper.BuildCommand(
                @"SELECT reactionId, qualifierId
				FROM MapReactionECNumber ms
				WHERE ecNumber = @ec;",
                "@ec", SqlDbType.VarChar, this.ECNumber);

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);
            if (ds.Length == 0)
                return null;

            Dictionary<Guid, BiomodelAnnotation> anns = new Dictionary<Guid, BiomodelAnnotation>();
            BiomodelAnnotation ann = null;
            foreach (DataSet d in ds)
            {
                DataRow row = d.Tables[0].Rows[0];
                Guid sbaseId = new Guid(row["reactionId"].ToString());
                if (anns.ContainsKey(sbaseId))
                    ann = anns[sbaseId];
                else
                {
                    ann = new BiomodelAnnotation(sbaseId);
                    anns.Add(ann.EntityId, ann);
                }
                ann.QualifierIds.AddFirst(int.Parse(row["qualifierId"].ToString()));
            }

            return anns;
        }

		/// <summary>
		/// Gets the GO Terms which annotate this EC Number
		/// </summary>
		/// <returns>The annotating ServerGOTerm(s)</returns>
		/// <remarks>
		/// Added: (mrr) 2006-202-15
		///</remarks>
		public ServerGOTerm[] GetAnnotatingGOTerms()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + ServerGOTerm.__GO_EC_AnnotationTable + " where ec_number = @id;",
				"@id", SqlDbType.VarChar, this.ECNumber);

			DataSet[] dsets;
			DBWrapper.LoadMultiple(out dsets, ref command);

			ServerGOTerm[] goTerms= new ServerGOTerm[dsets.Length];
			//now load each ServerGOTerm
			for(int i=0; i<dsets.Length; i++)
				goTerms[i] = ServerGOTerm.Load((string)dsets[i].Tables[0].Rows[0]["go_id"]);

			return goTerms;
		}

		/// <summary>
		/// Deletes the protein tuple from the ec_numbers table and all the associated tables
		/// </summary>
		public override void Delete()
		{
			// (sfa) Delete from ec_number_name_lookups
			DBWrapper.Instance.ExecuteNonQuery("DELETE FROM ec_number_name_lookups WHERE ec_number = @d_ec_number"
				, "@d_ec_number", SqlDbType.VarChar, this.ECNumber);

			// (sfa) I am not sure if we should delete also from gene_product_and_processes

			base.Delete ();
		}

		/// <summary>
		/// Returns a representation of this object suitable for being
		/// sent to a client via SOAP.
		/// </summary>
		/// <returns>
		/// A SoapObject object capable of being passed via SOAP.
		/// </returns>
		public override SoapObject PrepareForSoap ( SoapObject derived )
		{
			SoapECNumber retval = (derived == null) ? 
				retval = new SoapECNumber() : retval = (SoapECNumber)derived;

			retval.ECNumber   = this.ECNumber;
			retval.Name   = this.Name;
			retval.NodeCode   = this.NodeCode;

			retval.Status = ObjectStatus.NoChanges;

			return retval;
		}

		/// <summary>
		/// Consumes a SoapObject object and updates the object
		/// from it.
		/// </summary>
		/// <param name="o">
		/// The SoapObject object to update from, potentially containing
		/// changes to the object.
		/// </param>
		protected override void UpdateFromSoap ( SoapObject o )
		{
			SoapECNumber e = o as SoapECNumber;

			// ec_numbers has no ID field
//			if (o.Status == ObjectStatus.Insert)
//				e.ID = DBWrapper.NewID(); // generate a new ID

			this.ECNumber = e.ECNumber;
			this.Name = e.Name;
			this.NodeCode = e.NodeCode;
		}

		#region Catalyze relation
		/// <summary>
		/// Get all entries in the gene_products_and_processes relation with this ec_number
		/// </summary>
		/// <remarks>
		/// this should only return one ServerProcessEntity, except for NULL
		/// </remarks>
		/// <returns>
		/// an array of ServerProcessEntity objects
		/// </returns>
		public ServerCatalyze GetAllProcessGeneProducts ( )
		{
			return ServerCatalyze.LoadFromECNumber( this.ECNumber );
		}
		#endregion

		#region Entity name lookup relation ('entity_name_lookups' table)

		private void LoadECNumberNames()
		{
			if (ecNumberNames == null)
			{
				ecNumberNames = new Hashtable();
				ServerECNumberName[] names = ServerECNumberName.AllNames(this.ECNumber);
				if (names.Length > 0)
				{
					foreach(ServerECNumberName name in names)
					{
						ecNumberNames.Add(name.NameId, name);
					}
				}
			}
		}

		/// <summary>
		/// getter for all the names for this EC Number from the ec_number_names table
		/// </summary>
		public ServerECNumberName[] AllNames
		{
			get
			{
				LoadECNumberNames();
				ArrayList results = new ArrayList(ecNumberNames.Values);
				return (ServerECNumberName[])results.ToArray(typeof(ServerECNumberName));
			}
		}

		/// <summary>
		/// Adds a name for an enzyme in a reaction, or changes the type for an existing name
		/// Affects the molecular_entity_names table
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public void AddName(string name, string type)
		{
			LoadECNumberNames();
			Guid nameId = EntityNameManager.AddName(name);
			if (!HasName(name))
			{
				// add the name to this object
				ServerECNumberName n = new ServerECNumberName(new SoapECNumberName(this.ECNumber, name, type));
				ecNumberNames.Add(name, n);
			}
			else
			{
				// modify type of existing name
				((ServerECNumberName)ecNumberNames[nameId]).Type = type;
			}
		}
//
		// Leaving this for later, not needed now, I don't think
//		private void RemoveOldPrimaryNames(string newName)
//		{
//			LoadECNumberNames();
//			if (ecNumberNames.Count > 0)
//			{
//				foreach(ServerECNumberName name in ecNumberNames.Values)
//				{
//					if (name.Type == "primary name" && name.Name != newName)
//					{
//						name.Type = "other name"; // I don't know about this,
												// ec_number_name_lookups only has 'primary name' for a type
//					}
//				}
//			}
//		}

		/// <summary>
		/// Returns true if this ec number already has the given name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool HasName(string name)
		{
			//return ServerMolecularEntityName.Exists(this.ID, name);
			LoadECNumberNames();
			if (ecNumberNames.Count > 0)
			{
				foreach(ServerECNumberName n in ecNumberNames.Values)
				{
					if (n.Name == name)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// remove a name for an enzyme
		/// </summary>
		/// <param name="name"></param>
		public void DeleteName(string name)
		{
			//ServerMolecularEntityName n = ServerMolecularEntityName.Load(this.ID, name);
			//n.Delete();

			LoadECNumberNames();
			if (ecNumberNames.Count > 0)
			{
				foreach(ServerECNumberName n in ecNumberNames.Values)
				{
					if (n.Name == name)
					{
						n.Delete();
					}
				}
			}

		}

		#endregion

		/// <summary>
		/// Persist the ECNumber to the database.
		/// </summary>
		public override void UpdateDatabase()
		{
			// ensure primary name exists
			EntityNameManager.AddName(Name);

			// save changes in this row
			base.UpdateDatabase();

			// remove old primary names
			if (nameDirty)
			{
				// don't know if we need to (or are able to) remove old primary names
				// RemoveOldPrimaryNames(this.Name);
			}

			// save changes in alternative names
			if (ecNumberNames != null && ecNumberNames.Count > 0)
			{
				foreach(ServerECNumberName name in ecNumberNames.Values)
				{
					if (name.ECNumber == null)
					{
						// initial insert - need to populate entity name ID's
						name.ECNumber = this.ECNumber; 
					}
					name.UpdateDatabase();
				}
			}
		}

		#region ADO.NET SqlCommands


		/// <summary>
		/// Required function for setting up the SqlCommands for ADO.NET.
		/// </summary>
		protected override void SetSqlCommandParameters ( )
		{
			// (BE) rewrote using BuildCommand()

			__DBRow.ADOCommands["insert"] = DBWrapper.BuildCommand(
				"INSERT INTO " + __TableName + " (ec_number, name, nodeCode) VALUES (@ec_number, @name, @nodeCode);",
				"@ec_number", SqlDbType.VarChar, ECNumber,
				"@name", SqlDbType.VarChar, Name,
				"@nodeCode", SqlDbType.VarChar, NodeCode);

			__DBRow.ADOCommands["select"] = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number;",
				"@ec_number", SqlDbType.VarChar, ECNumber);

			__DBRow.ADOCommands["update"] = DBWrapper.BuildCommand(
				"UPDATE " + __TableName + " SET name = @name, nodeCode = @nodeCode WHERE ec_number = @ec_number;",
				"@name", SqlDbType.VarChar, Name,
				"@nodeCode", SqlDbType.VarChar, NodeCode,
				"@ec_number", SqlDbType.VarChar, ECNumber);

			__DBRow.ADOCommands["delete"] = DBWrapper.BuildCommand(
				"DELETE FROM " + __TableName + " WHERE ec_number = @ec_number;",
				"@ec_number", SqlDbType.VarChar, ECNumber);
		}
		#endregion
		#endregion


		#region Static Methods
        /// <summary>
        /// Computes node codes for ec numbers
        /// </summary>
        public static void ComputeNodeCodes()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"update " + __TableName + " set nodeCode = ec_number + '.'");
            DBWrapper.Instance.ExecuteNonQuery(ref command);

            command = DBWrapper.BuildCommand(
				"update " + __TableName + " set nodeCode=substring(ec_number, 0, charIndex('-', ec_number)) where charIndex('-', ec_number) > 0");
            DBWrapper.Instance.ExecuteNonQuery(ref command);

			
		}

        public static void CompleteMissingParents()
        {
            Dictionary<string, string> ecNumbers = new Dictionary<string, string>();
            ServerECNumber[] enums = ServerECNumber.AllECNumbers();
            foreach (ServerECNumber e in enums)
                ecNumbers.Add(e.ECNumber, "");

            Dictionary<string, string> missingECNumbers = new Dictionary<string, string>();

            string parentEC = "", ec;
            int dashIndex;
            foreach (ServerECNumber e in enums)
            {
                parentEC = "";
                ec = e.ECNumber;
                string[] parts = ec.Split('.');
                int z = 0;
                for (; z+1 < parts.Length; z++)
                {
                    if(parts[z+1] == "-")
                        break;
                    parentEC += parts[z] + ".";
                }

                if (parentEC.Length > 0)
                {
                    for (; z < 4; z++)
                        parentEC += "-.";
                    parentEC = parentEC.Substring(0, parentEC.Length - 1);

                    if (!missingECNumbers.ContainsKey(parentEC) && !ecNumbers.ContainsKey(parentEC))
                        missingECNumbers.Add(parentEC, "");
                }
            }

            foreach (string en in missingECNumbers.Keys)
            {
                ServerECNumber newEC = new ServerECNumber(en, en, "");
                newEC.UpdateDatabase();
            }

        }
		/// <summary>
		/// Returns true if the ec_number is in the ec_numbers table,
		/// otherwise returns false
		/// </summary>
		/// <param name="ec_number"></param>
		/// <returns></returns>
		public static bool Exists(string ec_number)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				"SELECT * FROM " + __TableName + " WHERE ec_number = @s_ec_number;",
				"@s_ec_number", SqlDbType.VarChar, ec_number);

			DataSet[] ds;
			if (DBWrapper.LoadMultiple(out ds, ref command) < 1)
				return false;
			return true;
		}

		/// <summary>
		/// Return all basic molecules from the system.
		/// </summary>
		/// <returns>
		/// Array of SoapECNumber objects ready to be sent via SOAP.
		/// </returns>
		public static ServerECNumber[] AllECNumbers ( )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + ";" );
			
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
		/// Return a ec number with given ID.
		/// </summary>
		/// <param name="ec_number">
		/// ec_number is the primary key for ec_numbers.
		/// </param>
		/// <returns>
		/// SoapECNumber object ready to be sent via SOAP.
		/// </returns>
		public static ServerECNumber Load ( string ec_number )
		{
			return new ServerECNumber( LoadRow ( ec_number ) );
		}

		/// <summary>
		/// Return the DBRow for a given ec number
		/// </summary>
		/// <param name="ec_number"></param>
		/// <returns></returns>
		private static DBRow LoadRow ( string ec_number )
		{
			SqlCommand command = new SqlCommand( "SELECT * FROM " + __TableName + " WHERE ec_number = @ec_number;" );
			SqlParameter num = new SqlParameter( "@ec_number", SqlDbType.VarChar );
			num.SourceVersion = DataRowVersion.Original;
			num.Value = ec_number;
			command.Parameters.Add( num );

			DataSet ds = new DataSet();
			DBWrapper.LoadSingle( out ds, ref command );
			return new DBRow(ds);
		}
		#endregion

	} // End class

} // End namespace


#region Change Log
//----------------------------- END OF SOURCE ----------------------------

/*------------------------------- Change Log -----------------------------
	$Id: ServerECNumber.cs,v 1.3 2009/05/01 06:34:34 ali Exp $
	$Log: ServerECNumber.cs,v $
	Revision 1.3  2009/05/01 06:34:34  ali
	*** empty log message ***
	
	Revision 1.2  2009/04/30 16:18:00  ali
	*** empty log message ***
	
	Revision 1.1  2008/05/16 21:15:53  mustafa
	*** empty log message ***
	
	Revision 1.1  2006/07/31 19:37:43  greg
	Ported from VS7 to VS8.  The system should compile, but there are definitely issues with the site that are still at large.
	
	Revision 1.15  2006/05/11 16:18:39  marc
	Woah, Merge from GeneOntologyFeatures
	
	Revision 1.14.6.1  2006/02/21 22:15:27  marc
	Added GetAnnotatingGOTerms method
	
	Revision 1.14  2005/11/07 17:28:26  brendan
	New generic graph building code, exposed via a new web service call.
	
	Revision 1.13  2005/10/31 19:25:11  fatih
	*** empty log message ***
	
	Revision 1.12  2005/10/26 17:53:57  michael
	Updating doc comments
	
	Revision 1.11  2005/07/19 18:15:36  brandon
	Added a bunch of XML comments, also changed get all processes for pathway function to remove duplicates
	
	Revision 1.10  2005/07/15 14:30:43  brandon
	Added ECNumberName objects and made changes to ServerECNumber for name lookups.
	Also added functions for common molecules in BasicMolecule objects
	
	Revision 1.9  2005/07/13 22:14:12  brandon
	bug fixes, updated ServerBasicMolecule for inheritance, but it's not finished so don't use it yet.
	
	Revision 1.8  2005/07/13 16:41:45  brandon
	Added a bunch of XML comments where they were missing.
	Changed ServerCatalyze so that it loads a single object from a given ec number instead of an array.
	
	Revision 1.7  2005/07/12 22:14:04  brandon
	Bug fixes.      Also added external_database_links objects
	
	Revision 1.6  2005/07/12 17:34:56  brandon
	Finished up (for the most part) the ProcessEntities relation (left out adding and removing stuff).  Created the ExternalDatabase objects.  fixed a bug in SoapRNA.
	
	Revision 1.5  2005/07/11 22:13:57  brandon
	Upated most of the Server and Soap objects to fit the current model.  Still need to add more XML comments in places
	
	Revision 1.4  2005/07/08 20:36:39  brandon
	changed LoadDataSet to LoadRow in all the Server objects
	
	Revision 1.3  2005/07/08 19:32:05  brandon
	fixed ServerCatalyze, sort of,  and uh, this project builds now
	
	Revision 1.2  2005/07/07 15:10:28  brandon
	Added ServerCatalyze.cs (gene_product_and_processes), it's not done yet, and added the GetAllOrganismGroups function to ServerProcess object
	
	Revision 1.1  2005/07/06 20:18:21  brandon
	Added server objects for RNA and EC number.  Done with the relation between Pathway and Process, still working on relation between Process and Organism Group.  Function AddProcessToOrganismGroup still not working, can't figure out why
	

		
------------------------------------------------------------------------*/
#endregion