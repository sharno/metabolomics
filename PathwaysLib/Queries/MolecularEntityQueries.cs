namespace PathwaysLib.Queries
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Text;
	using PathwaysLib.ServerObjects;
	using PathwaysLib.Exceptions;
	using PathwaysLib.Utilities;


	/// <summary>
	/// Contains utility functions related to molecular entities used by the
	/// built-in queries and other services to help build up information
	/// for tabular queries.
	/// Transferred by Greg Strnad from the old pathways service.
	/// </summary>
	public class MolecularEntityQueries
	{
		private MolecularEntityQueries() {}

        /// <summary>
        ///	create pathway list for the sql statement
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <returns>a pathway list for the sql statement.</returns>
        public static string CreatePathway_ID_List(string pathwayListStr)
        {
            string sql_Pathway_ID_List = "";

            char[] separator = { ',' };
            string[] pathwayList = pathwayListStr.Split(separator);

            if (pathwayList.Length > 0)
            {
                sql_Pathway_ID_List = "(";
                for (int j = 0; j < pathwayList.Length; j++)
                {
                    sql_Pathway_ID_List += "'" + pathwayList[j] + "',";
                }

                sql_Pathway_ID_List = sql_Pathway_ID_List.Substring(0, sql_Pathway_ID_List.Length - 1);
                sql_Pathway_ID_List += ")";
            }

            return sql_Pathway_ID_List;
        }

		/// <summary>
		/// Get common molecule IDs of this ME
		/// </summary>
		/// <returns>A filled DataSet array</returns>
		public static DataSet[] GetCommonMoleculeIDs()
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT id AS entity_id, name
					FROM molecular_entities 
					WHERE id IN ( SELECT id FROM common_molecules )");
	
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}

		/// <summary>
		///	Expand a molecular entity in a given number of steps in a given pathway of a given organism.
		/// </summary>
		/// <param name="organism">The organsim ID</param>
		/// <param name="pathway">The pathway ID</param>
		/// <param name="entity">The molecular entity ID</param>
		/// <param name="steps">How many steps will be expanded.</param>
		/// <param name="entity_list">A list of molecular entities.</param>
		/// <returns>An instance of DataSet including all expanded molecular entities.</returns>
		public static DataSet[] ExpandMolecularEntityInOrganism(string organism, string pathway, string entity, int steps, string[] entity_list)
		{
			ArrayList dsPathList = new ArrayList();
			DataTable dtPathList = new DataTable();
			dtPathList.Columns.Add( "Original_Molecular_Entity_id", typeof( string ) );
			dtPathList.Columns.Add( "Original_Molecular_Entity_Name", typeof( string ) );
			dtPathList.Columns.Add( "Next_Molecular_Entity_id", typeof( string ) );
			dtPathList.Columns.Add( "Next_Molecular_Entity_Name", typeof( string ) );
			dtPathList.Columns.Add( "Process_Id", typeof( string ) );
			dtPathList.Columns.Add( "Process_Name", typeof( string ) );
			dtPathList.Columns.Add( "Step", typeof( string ) );

			HyperGraph Pathway = new HyperGraph(organism, organism, pathway, true, entity_list); //new an instance of class hypergraph to represent the given pathway
			string Molecular_Entity_Name = Pathway.Get_Entity_Name(entity); // get the molecular entity id
			
			ArrayList PathList = Pathway.Expand_An_Entity(entity.ToUpper(), Molecular_Entity_Name, steps);//get all expended nodes

			// All information is stored to the dataset that will return to the caller.
			foreach (string[] Edge in PathList)
			{
				DataRow NewRow = dtPathList.NewRow();
				for(int i = 0;i<=6;i++)
					NewRow[i] = Edge[i];

				dtPathList.Rows.Add(NewRow);
			}

			foreach( DataRow drTemp in dtPathList.Rows )
			{
				DataSet dsTemp = new DataSet();
				DataTable dtTemp = dtPathList.Clone();
				dsTemp.Tables.Add(dtTemp);
				dsTemp.Tables[0].ImportRow(drTemp);
				dsPathList.Add(dsTemp);
			}

			return (DataSet[])dsPathList.ToArray(typeof(DataSet));
		}

        /// <summary>
        /// Expand a molecular entity for a number of steps in the whole network
        /// </summary>
        /// <param name="organism">The organism ID to use.</param>
        /// <param name="entity_id">The molecular entity ID to be expanded</param>
        /// <param name="steps">How many steps to consider.</param>
        /// <returns></returns>
        public static DataTable Expand_An_Entity_Number_of_Steps(string organism, string entity_id, int steps)
        {
            Hashtable visited = new Hashtable();
            //Hashtable current = new Hashtable();
            ArrayList current = new ArrayList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Source Molecule", typeof(string));
            dt.Columns.Add("Source Molecule ID", typeof(string));
            dt.Columns.Add("Source Molecule Role", typeof(string));
            dt.Columns.Add("Process Name", typeof(string));
            dt.Columns.Add("Process ID", typeof(string));
            dt.Columns.Add("Next Molecule", typeof(string));
            dt.Columns.Add("Next Molecule ID", typeof(string));
            dt.Columns.Add("Next Molecule Role", typeof(string));
            dt.Columns.Add("Step", typeof(int));
            DataRow dr;
            int step = 0;
            string commonMols = @"(";
            DataSet[] tmpDt = GetCommonMoleculeIDs();
            foreach (DataSet t in tmpDt)
            {
                commonMols += "'" + t.Tables[0].Rows[0][0].ToString() + "',";
            }
            commonMols = commonMols.Substring(0, commonMols.Length - 1);
            commonMols = commonMols + @")";
            current.Add(new string[2] { entity_id, step.ToString() });
            ArrayList tmpResult;
            for (int i = 0; i < current.Count; i++)
            //foreach (DictionaryEntry de in current.cl) 
            {
                string deKey = ((string[])current[i])[0];
                int deVal = Int32.Parse(((string[])current[i])[1]);
                if (!visited.ContainsKey(deKey) && deVal < steps)
                {
                    //int currStep = deVal;
                    tmpResult = Expand_An_Entity_In_Network(organism, deKey, commonMols);
                    visited.Add(deKey, 1);
                    foreach (string[] arr in tmpResult)
                    {
                        dr = dt.NewRow();
                        dr["Source Molecule"] = arr[0];
                        dr["Source Molecule ID"] = arr[1];
                        dr["Source Molecule Role"] = arr[2];
                        dr["Process Name"] = arr[6];
                        dr["Process ID"] = arr[7];
                        dr["Next Molecule"] = arr[3];
                        dr["Next Molecule ID"] = arr[4];
                        dr["Next Molecule Role"] = arr[5];
                        dr["Step"] = deVal + 1;
                        int newStep = deVal + 1;
                        if (!visited.ContainsKey(arr[4].ToString()))
                        {
                            dt.Rows.Add(dr);
                            current.Add(new string[2] { arr[4], newStep.ToString() });
                        }
                    }

                }

                //current.Remove(de.Key);
            }
            return dt;
        }
       
        /// <summary>
        /// Expand a molecular entity for a number of steps in the whole network
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <param name="organism">The organism ID to use.</param>
        /// <param name="entity_id">The molecular entity ID to be expanded</param>
        /// <param name="steps">How many steps to consider.</param>
        /// <returns></returns>
        public static DataTable Expand_An_Entity_Number_of_Steps(string pathwayListStr, string organism, string entity_id, int steps) 
        {
            Hashtable visited = new Hashtable();
            //Hashtable current = new Hashtable();
            ArrayList current = new ArrayList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Source Molecule", typeof(string));
            dt.Columns.Add("Source Molecule ID", typeof(string));
            dt.Columns.Add("Source Molecule Role", typeof(string));
            dt.Columns.Add("Process Name", typeof(string));
            dt.Columns.Add("Process ID", typeof(string));
            dt.Columns.Add("Next Molecule", typeof(string));
            dt.Columns.Add("Next Molecule ID", typeof(string));
            dt.Columns.Add("Next Molecule Role", typeof(string));
            dt.Columns.Add("Step", typeof(int));
            DataRow dr;
            int step = 0;
            string commonMols = @"(";
            DataSet[] tmpDt = GetCommonMoleculeIDs();
            foreach (DataSet t in tmpDt) {
                commonMols += "'"+t.Tables[0].Rows[0][0].ToString()+"',";
            }
            commonMols = commonMols.Substring(0, commonMols.Length - 1);
            commonMols = commonMols + @")";
            current.Add(new string[2]{entity_id, step.ToString()});
            ArrayList tmpResult;
            for (int i = 0; i < current.Count;i++)
            //foreach (DictionaryEntry de in current.cl) 
            {
                string deKey = ((string[])current[i])[0];
                int deVal = Int32.Parse(((string[])current[i])[1]);
                if (!visited.ContainsKey(deKey) && deVal < steps)
                {
                    //int currStep = deVal;
                   // tmpResult = Expand_An_Entity_In_Network(organism, deKey,commonMols);
                    tmpResult = Expand_An_Entity_In_Network(pathwayListStr, organism, deKey, commonMols);
                    visited.Add(deKey, 1);
                    foreach (string[] arr in tmpResult)
                    {
                        dr = dt.NewRow();
                        dr["Source Molecule"] = arr[0];
                        dr["Source Molecule ID"] = arr[1];
                        dr["Source Molecule Role"] = arr[2];
                        dr["Process Name"] = arr[6];
                        dr["Process ID"] = arr[7];
                        dr["Next Molecule"] = arr[3];
                        dr["Next Molecule ID"] = arr[4];
                        dr["Next Molecule Role"] = arr[5];
                        dr["Step"] = deVal+1;
                        int newStep = deVal + 1;
                        if (!visited.ContainsKey(arr[4].ToString()))
                        {
                            dt.Rows.Add(dr);
                            current.Add(new string[2] { arr[4], newStep.ToString() });
                        }
                    }

                }

                //current.Remove(de.Key);
            }
            return dt;
        }
        /// <summary>
        ///	Returns the 1-neighborhood of a given molecular entity in the entire network
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <param name="organism">The organsim ID</param>
        /// <param name="entity">The molecular entity ID</param>
        /// <param name="commonMoleculeList">The list of common molecules</param>
        /// <returns>An array of record of 8 tuples.</returns>
        public static ArrayList Expand_An_Entity_In_Network(string pathwayListStr, string organism, string entity, string commonMoleculeList)
        {
            SqlCommand command1, command2;
            string sql_Pathway_ID_List = CreatePathway_ID_List(pathwayListStr);
            
            if (organism == ServerOrganism.UnspecifiedOrganism)
            {
                command1 = DBWrapper.BuildCommand(
                 @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.generic_process_id
                        FROM pathway_processes pp, processes p2, 
                             process_entities pe2, process_entity_roles per2, molecular_entities me2,
	                        (SELECT DISTINCT p.generic_process_id AS gid
	                            FROM process_entities pe, process_entity_roles per, processes p
	                            WHERE pe.entity_id= @mol_id AND (per.name = 'product' OR per.name = 'substrate') 
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id
	                         ) as tmp
                        WHERE  pp.pathway_id IN  " + sql_Pathway_ID_List + @" AND p2.id = pp.process_id 
                            AND p2.generic_process_id = tmp.gid AND p2.id = pe2.process_id 
                            AND (per2.name = 'product' OR per2.name = 'substrate') AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id AND  pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList,
                  "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                /*
                command2 = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.generic_process_id
                        FROM pathway_processes pp,processes p2, 
                             process_entities pe2, process_entity_roles per2, molecular_entities me2,
	                        (SELECT DISTINCT p.generic_process_id AS gid
	                            FROM process_entities pe, process_entity_roles per, processes p
	                            WHERE pe.entity_id= @mol_id AND per.name = 'substrate' 
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id
	                         ) as tmp
                        WHERE pp.pathway_id IN  " + sql_Pathway_ID_List + @" AND p2.id = pp.process_id 
                            AND p2.generic_process_id = tmp.gid AND p2.id = pe2.process_id 
                            AND per2.name = 'product' AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id  AND pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList,
                       "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                */
            }
            else
            {
                // Specific organism selected
                ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
                string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");
                //InOrganismIdList = InOrganismIdList.Substring(1, InOrganismIdList.Length - 2);
                command1 = DBWrapper.BuildCommand(
                      @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.id
                        FROM pathway_to_pathway_groups ppg, pathways pw, pathway_processes pp,processes p2, 
                             process_entities pe2, process_entity_roles per2, 
                            molecular_entities me2,common_molecules cm, catalyzes c,
	                        (SELECT DISTINCT p.id AS pid
	                            FROM process_entities pe, process_entity_roles per, processes p 
	                            WHERE pe.entity_id= @mol_id AND (per.name = 'product' OR per.name = 'substrate')
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id 
	                         ) as tmp
                        WHERE  pp.pathway_id IN  " + sql_Pathway_ID_List + @" AND p2.id = pp.process_id 
                            AND p2.id = tmp.pid AND p2.id = pe2.process_id AND (per2.name = 'product' OR per2.name = 'substrate') AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id AND  pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList + " AND c.process_id = tmp.pid AND " + InOrganismIdList, 
                       "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                /*
                command2 = DBWrapper.BuildCommand(
                      @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.id
                        FROM pathway_to_pathway_groups ppg, pathways pw, pathway_processes pp, processes p2, 
                             process_entities pe2, process_entity_roles per2, 
                            molecular_entities me2,common_molecules cm, catalyzes c,
	                        (SELECT DISTINCT p.id AS pid
	                            FROM process_entities pe, process_entity_roles per, processes p 
	                            WHERE pe.entity_id= @mol_id AND per.name = 'substrate' 
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id 
	                         ) as tmp
                        WHERE  pp.pathway_id IN  " + sql_Pathway_ID_List + @" AND p2.id = pp.process_id 
                            AND p2.id = tmp.pid AND p2.id = pe2.process_id AND per2.name = 'product' AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id AND  pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList + " AND c.process_id = tmp.pid AND " + InOrganismIdList,
                       "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                */
            }
            DataSet[] ds1 = new DataSet[0];
            DataSet[] ds2 = new DataSet[0];
            DBWrapper.LoadMultiple(out ds1, ref command1);
            //DBWrapper.LoadMultiple(out ds2, ref command2);

            // Now return the results as an array of 8-tuples
            string[] tuple;
            ArrayList res = new ArrayList();
            foreach (DataSet dstmp in ds1)
            {

                tuple = new string[8] { (ServerMolecularEntity.Load(new Guid(entity))).Name, entity, "substrate", 
                    (string)dstmp.Tables[0].Rows[0][0],  ((Guid)dstmp.Tables[0].Rows[0][1]).ToString().ToUpper(),"product",
                    (string)dstmp.Tables[0].Rows[0][2], ((Guid)dstmp.Tables[0].Rows[0][3]).ToString().ToUpper()};
                res.Add(tuple);
            }
            /*
            foreach (DataSet dstmp in ds2)
            {

                tuple = new string[8] { (ServerMolecularEntity.Load(new Guid(entity))).Name, entity, "product", 
                    (string)dstmp.Tables[0].Rows[0][0], ((Guid)dstmp.Tables[0].Rows[0][1]).ToString().ToUpper(),"substrate",
                    (string)dstmp.Tables[0].Rows[0][2], ((Guid)dstmp.Tables[0].Rows[0][3]).ToString().ToUpper()};
                res.Add(tuple);
            }
            */
            return res;

        }
		
        /// <summary>
        ///	Returns the 1-neighborhood of a given molecular entity in the entire network
        /// </summary>
        /// <param name="organism">The organsim ID</param>
        /// <param name="entity">The molecular entity ID</param>
        /// <param name="commonMoleculeList">The list of common molecules</param>
        /// <returns>An array of record of 8 tuples.</returns>
        public static ArrayList Expand_An_Entity_In_Network(string organism, string entity, string commonMoleculeList)
        {
            SqlCommand command1, command2;
            if (organism == ServerOrganism.UnspecifiedOrganism) 
            {
               command1 = DBWrapper.BuildCommand(
                @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.generic_process_id
                        FROM processes p2, process_entities pe2, process_entity_roles per2, molecular_entities me2,
	                        (SELECT DISTINCT p.generic_process_id AS gid
	                            FROM process_entities pe, process_entity_roles per, processes p
	                            WHERE pe.entity_id= @mol_id AND (per.name = 'product' OR per.name = 'substrate')
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id
	                         ) as tmp
                        WHERE p2.generic_process_id = tmp.gid AND p2.id = pe2.process_id AND (per2.name = 'substrate' OR per2.name = 'product') AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id AND  pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList,
                 "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));

                /*
                command2 = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.generic_process_id
                        FROM processes p2, process_entities pe2, process_entity_roles per2, molecular_entities me2,
	                        (SELECT DISTINCT p.generic_process_id AS gid
	                            FROM process_entities pe, process_entity_roles per, processes p
	                            WHERE pe.entity_id= @mol_id AND per.name = 'substrate' 
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id
	                         ) as tmp
                        WHERE p2.generic_process_id = tmp.gid AND p2.id = pe2.process_id AND per2.name = 'product' AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id  AND pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList,
                     "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                 * */

            }
            else {
                // Specific organism selected
                ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
                string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");
                //InOrganismIdList = InOrganismIdList.Substring(1, InOrganismIdList.Length - 2);
                command1 = DBWrapper.BuildCommand(
                      @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.id
                        FROM processes p2, process_entities pe2, process_entity_roles per2, 
                            molecular_entities me2,common_molecules cm, catalyzes c,
	                        (SELECT DISTINCT p.id AS pid
	                            FROM process_entities pe, process_entity_roles per, processes p 
	                            WHERE pe.entity_id= @mol_id AND (per.name = 'product' OR per.name = 'substrate') 
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id 
	                         ) as tmp
                        WHERE p2.id = tmp.pid AND p2.id = pe2.process_id AND (per2.name = 'product' OR per2.name = 'substrate') AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id AND  pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList +" AND c.process_id = tmp.pid AND " + InOrganismIdList,
                      "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                /*
                command2 = DBWrapper.BuildCommand(
                      @"SELECT DISTINCT me2.name, me2.id, p2.name, p2.id
                        FROM processes p2, process_entities pe2, process_entity_roles per2, 
                            molecular_entities me2,common_molecules cm, catalyzes c,
	                        (SELECT DISTINCT p.id AS pid
	                            FROM process_entities pe, process_entity_roles per, processes p 
	                            WHERE pe.entity_id= @mol_id AND per.name = 'substrate' 
	                            AND per.role_id = pe.role_id AND pe.process_id = p.id 
	                         ) as tmp
                        WHERE p2.id = tmp.pid AND p2.id = pe2.process_id AND per2.name = 'product' AND per2.role_id = pe2.role_id 
                            AND pe2.entity_id <> @mol_id AND  pe2.entity_id = me2.id 
                            AND me2.id NOT IN " + commonMoleculeList + " AND c.process_id = tmp.pid AND " + InOrganismIdList,
                      "@mol_id", SqlDbType.UniqueIdentifier, new Guid(entity));
                 * */

            }
            DataSet[] ds1 = new DataSet[0];
            DataSet[] ds2 = new DataSet[0];
            DBWrapper.LoadMultiple(out ds1, ref command1);
            //DBWrapper.LoadMultiple(out ds2, ref command2);

            // Now return the results as an array of 8-tuples
            string[] tuple; 
            ArrayList res = new ArrayList();
            foreach (DataSet dstmp in ds1) {

                tuple = new string[8] { (ServerMolecularEntity.Load(new Guid(entity))).Name, entity, "substrate", 
                    (string)dstmp.Tables[0].Rows[0][0],  ((Guid)dstmp.Tables[0].Rows[0][1]).ToString().ToUpper(),"product",
                    (string)dstmp.Tables[0].Rows[0][2], ((Guid)dstmp.Tables[0].Rows[0][3]).ToString().ToUpper()};
                res.Add(tuple);
            }
            /*
            foreach (DataSet dstmp in ds2)
            {

                tuple = new string[8] { (ServerMolecularEntity.Load(new Guid(entity))).Name, entity, "product", 
                    (string)dstmp.Tables[0].Rows[0][0], ((Guid)dstmp.Tables[0].Rows[0][1]).ToString().ToUpper(),"substrate",
                    (string)dstmp.Tables[0].Rows[0][2], ((Guid)dstmp.Tables[0].Rows[0][3]).ToString().ToUpper()};
                res.Add(tuple);
            }
            */
            return res;

        }
		/// <summary>
		///		Given two molecular entities A and B, and a pathway P, find alternative ways to go from A to B in P in a given organism but not through the common entities like H2O.
		/// </summary>
		/// <param name="organism">The name of an organism</param>
		/// <param name="pathway">The ID of a pathway</param>
		/// <param name="entityA">The molecular entity as the source</param>
		/// <param name="entityB">The molecular entity as the destination</param>
		/// <param name="entity_list">A list of common entities</param>
		/// <returns>An instance of DataSet including all paths between two entities.</returns>
		public static DataSet[] FindPathsBetweenTwoEntitiesInOrganism(string organism, string pathway, string entityA, string entityB, string[] entity_list)
		{
			int Number = 0;
			
			ArrayList dsPathList = new ArrayList();
			DataTable dtPathList = new DataTable();
			dtPathList.Columns.Add( "Original_Molecular_Entity_id", typeof( string ) );
			dtPathList.Columns.Add( "Original_Molecular_Entity_Name", typeof( string ) );
			dtPathList.Columns.Add( "Next_Molecular_Entity_id", typeof( string ) );
			dtPathList.Columns.Add( "Next_Molecular_Entity_Name", typeof( string ) );
			dtPathList.Columns.Add( "Process_Id", typeof( string ) );
			dtPathList.Columns.Add( "Process_Name", typeof( string ) );
			dtPathList.Columns.Add( "No.", typeof( string ) );

			HyperGraph Pathway = new HyperGraph(organism, organism, pathway, false, entity_list); // new an instance of class hypergraph to represent the given pathway. 

			string Molecular_Entity_Name_A = Pathway.Get_Entity_Name(entityA);

			ArrayList PathList = Pathway.ComputePaths(entityA.ToUpper(), Molecular_Entity_Name_A, entityB.ToUpper()); //Compute paths between two entities

			//store all paths in the data set that will return to the caller.
			foreach (ArrayList Path in PathList)
			{
				Number ++;
				foreach(string[] Edge in Path)
				{
					DataRow NewRow = dtPathList.NewRow();
					for(int j = 0; j < 6; j++)
					{
						if(j == 4) NewRow[j] = Edge[j].Substring(1,Edge[j].Length - 1);
						else NewRow[j] = Edge[j];
					}
					NewRow[6] = Convert.ToString(Number);

					dtPathList.Rows.Add(NewRow);
				}
			}

			foreach( DataRow drTemp in dtPathList.Rows )
			{
				DataSet dsTemp = new DataSet();
				DataTable dtTemp = dtPathList.Clone();
				dsTemp.Tables.Add(dtTemp);
				dsTemp.Tables[0].ImportRow(drTemp);
				dsPathList.Add(dsTemp);
			}

			return (DataSet[])dsPathList.ToArray(typeof(DataSet));
		}

		/// <summary>
		/// Finds enzymes of processes with a given molecule in a specific role
		/// in an organism.
		/// </summary>
		/// <param name="organism">The organism in question.</param>
		/// <param name="role">The role to consider.</param>
		/// <param name="entityId">The id of the molecular entity to consider.</param>
		/// <returns>A filled DataSet[] object.</returns>
		public static DataSet[] EnzymesOfProcessesInvolvingEntityInSpecificUse(string organism, string role, string entityId)
		{
			SqlCommand command;

			if(organism == ServerOrganism.UnspecifiedOrganism)
			{
				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT pw.id as pw_id, pw.name as pw_name, p.id, p.name,
						me.name as enzyme_name, 
						me.id as entity_id, p.generic_process_id as generic_process_id 
						FROM pathways pw, pathway_processes pe,processes p, catalyzes c, molecular_entities me 
						WHERE pw.id = pe.pathway_id and pe.process_id = p.id and c.process_id = p.id 
						and c.gene_product_id = me.id and p.id in 
							(SELECT DISTINCT process_id
							FROM view_process_entities 
							WHERE entity_id = @entityId AND Role = @role ) 
						ORDER BY pw.id, p.generic_process_id",
					"@role", SqlDbType.VarChar, role,
					"@entityId", SqlDbType.UniqueIdentifier, new Guid(entityId));
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT pw.id as pw_id, pw.name as pw_name, p.id, p.name, me.name as enzyme_name, 
						   me.id as entity_id, p.generic_process_id as generic_process_id 
						FROM pathways pw, pathway_processes pe,processes p, catalyzes c, molecular_entities me 
						WHERE pw.id = pe.pathway_id and pe.process_id = p.id and c.process_id = p.id and c.gene_product_id = me.id 
						   and " + InOrganismIdList + @" and p.id in 
							(SELECT DISTINCT process_id 
							FROM view_process_entities 
							WHERE entity_id = @entityId AND role = @role ) 
						ORDER BY pw.id, p.generic_process_id",
					"@role", SqlDbType.VarChar, role,
					"@entityId", SqlDbType.UniqueIdentifier, new Guid(entityId));
			}

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}

		/// <summary>
		/// Finds enzymes of processes with a given molecule in a specific role
		/// in an organism.
		/// </summary>
		/// <param name="organism">The organism in question.</param>
		/// <param name="role">The role to consider.</param>
		/// <param name="entityId">The id of the molecular entity to consider.</param>
		/// <returns>A filled DataSet[] object.</returns>
		public static DataSet[] PathwaysAndProcessesIncludingThisMolecularEntityWithSpecifiedRoleInOrganism(string organism, string entityId, string role)
		{
			SqlCommand command;

			if( organism == ServerOrganism.UnspecifiedOrganism )
			{
				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT pw.id as pw_id, pw.name as pw_name, p.id, p.name, me.name AS enzyme_name,
						me.id AS entity_id, p.generic_process_id as generic_process_id 
						FROM pathways pw, pathway_processes pe, processes p, catalyzes c, molecular_entities me
						WHERE pw.id = pe.pathway_id and pe.process_id = p.id and c.process_id = p.id
						AND c.gene_product_id = me.id AND p.id IN 
							(SELECT DISTINCT process_id 
							FROM view_process_entities 
							WHERE entity_id = @entityId and Role = @role ) 
						order by pw.name, p.name",
					"@role", SqlDbType.VarChar, role,
					"@entityId", SqlDbType.UniqueIdentifier, new Guid(entityId));
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT pw.id as pw_id, pw.name as pw_name, p.id, p.name, me.name AS enzyme_name,
						me.id AS entity_id, p.generic_process_id as generic_process_id 
						FROM pathways pw, pathway_processes pe, processes p, catalyzes c, molecular_entities me
						WHERE pw.id = pe.pathway_id and pe.process_id = p.id and c.process_id = p.id
						AND c.gene_product_id = me.id AND " + InOrganismIdList + @" and p.id IN
							(SELECT DISTINCT process_id 
							from view_process_entities
							where entity_id = @entityId AND Role = @role ) 
						ORDER BY pw.name, p.name",
					"@role", SqlDbType.VarChar, role,
					"@entityId", SqlDbType.UniqueIdentifier, new Guid(entityId));
			}

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}


        /// <summary>
        /// Finds enzymes of processes with a given molecule in a specific role
        /// in an organism.
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <param name="organism">The organism in question.</param>
        /// <param name="role">The role to consider.</param>
        /// <param name="entityId">The id of the molecular entity to consider.</param>
        /// <returns>A filled DataSet[] object.</returns>
        public static DataSet[] PathwaysAndProcessesIncludingThisMolecularEntityWithSpecifiedRoleInOrganism(string pathwayListStr, string organism, string entityId, string role)
        {
            SqlCommand command;
            string sql_Pathway_ID_List = CreatePathway_ID_List(pathwayListStr);

            if (organism == ServerOrganism.UnspecifiedOrganism)
            {
                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT pw.id as pw_id, pw.name as pw_name, p.id, p.name, me.name AS enzyme_name,
						me.id AS entity_id, p.generic_process_id as generic_process_id 
						FROM pathways pw, pathway_processes pp, processes p, catalyzes c, molecular_entities me
						WHERE pw.id IN " + sql_Pathway_ID_List + @" and pw.id = pp.pathway_id and pp.process_id = p.id and c.process_id = p.id
						AND c.gene_product_id = me.id AND p.id IN 
							(SELECT DISTINCT process_id 
							FROM view_process_entities 
							WHERE entity_id = @entityId and Role = @role ) 
						order by pw.name, p.name",
                    "@role", SqlDbType.VarChar, role,
                    "@entityId", SqlDbType.UniqueIdentifier, new Guid(entityId));
            }
            else
            {
                // Specific organism selected
                ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
                string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT pw.id as pw_id, pw.name as pw_name, p.id, p.name, me.name AS enzyme_name,
						me.id AS entity_id, p.generic_process_id as generic_process_id 
						FROM  pathways pw, pathway_processes pp, processes p, catalyzes c, molecular_entities me
						WHERE pw.id IN" + sql_Pathway_ID_List + @" and pw.id = pp.pathway_id and pp.process_id = p.id and c.process_id = p.id
						AND c.gene_product_id = me.id AND " + InOrganismIdList + @" and p.id IN
							(SELECT DISTINCT process_id 
							from view_process_entities
							where entity_id = @entityId AND Role = @role ) 
						ORDER BY pw.name, p.name",
                    "@role", SqlDbType.VarChar, role,
                    "@entityId", SqlDbType.UniqueIdentifier, new Guid(entityId));
            }

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            return ds;
        }
    }
}