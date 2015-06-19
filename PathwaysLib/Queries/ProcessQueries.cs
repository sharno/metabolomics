namespace PathwaysLib.Queries
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Text;
	using PathwaysLib.Exceptions;
	using PathwaysLib.ServerObjects;

	/// <summary>
	/// Contains utility functions related to processes used by the
	/// built-in queries and other services to help build up information
	/// for tabular queries.
	/// Transferred by Greg Strnad from the old pathways service.
	/// </summary>
	public class ProcessQueries
	{
		private ProcessQueries() {}

        /// <summary>
        ///	create pathway list for the sql statement
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <returns>a pathway list for the sql statement.</returns>
        public static  string CreatePathway_ID_List(string pathwayListStr)
        {
            string sql_Pathway_ID_List = "";

            char[] separator = {','};
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
		///	Creates a table to store all expended processes' information.
		/// </summary>
		/// <returns>An instance of DataTable.</returns>
		private static DataTable NewTable()
		{
			DataTable ProcessTable = new DataTable();
			ProcessTable.Columns.Add( "Original_Process_id", typeof( string ) );
			ProcessTable.Columns.Add( "Original_Process_Name", typeof( string ) );
			ProcessTable.Columns.Add( "Molecular_Entity_Id", typeof( string ) );
			ProcessTable.Columns.Add( "Molecular_Entity_Name", typeof( string ) );
			ProcessTable.Columns.Add( "Use_In_Original_Process", typeof( string ) );
			ProcessTable.Columns.Add( "Step", typeof( string ) );
			ProcessTable.Columns.Add( "Next_Process_id", typeof( string ) );
			ProcessTable.Columns.Add( "Next_Process_Name", typeof( string ) );
			ProcessTable.Columns.Add( "Use_In_Next_Process", typeof( string ) );
			return ProcessTable;
		}

        /// <summary>
        /// Expand processes that are a given number of steps away from a group of processes that contain
        /// a specific group of molectules.
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <param name="organism">The organism ID to use.</param>
        /// <param name="pathway">The pathway ID to use; some queries may set this to "".</param>
        /// <param name="process">The process ID to use.</param>
        /// <param name="steps">How many steps to consider.</param>
        /// <param name="entity_list">Molecules to consider.</param>
        /// <returns></returns>
        public static DataSet[] ExpandProcessesInGivenStepsCommonListInOrganism(string pathwayListStr, string organism, string pathway, string process, int steps, string[] entity_list)
        {
            ArrayList dsProcessPairs = new ArrayList();
            DataTable dtProcess = NewTable();
            ArrayList ProcessPairs = new ArrayList();
            string[] Pair; //store the new expanded process.
            DataSet[] PairSet;
            int i = 1;
            bool flag;

            //create common entities list for the sql statement
            string sql_Entity_ID_List = "('" + process + "')";

            if (entity_list.Length > 0)
            {
                sql_Entity_ID_List = "(";
                for (int j = 0; j < entity_list.Length; j++)
                {
                    sql_Entity_ID_List += "'" + entity_list[j] + "',";
                }

                sql_Entity_ID_List = sql_Entity_ID_List.Substring(0, sql_Entity_ID_List.Length - 1);
                sql_Entity_ID_List += ")";
            }


            //For a given process, we get all processes connected to it.
           // PairSet = GetAllRelatedProcessesInOrganism(organism, process, pathway, sql_Entity_ID_List, true);

            // generic_process_id, generic_process_name, mol_id, mol_name, mol_role, generic_process_id, generic_process_name, mol_role
            PairSet = GetAllRelatedProcessesInOrganism(pathwayListStr, organism, process, pathway, sql_Entity_ID_List, true);
            


            //For each process, we check if it = not
            foreach (DataSet dsItem in PairSet)
            {
                DataRow MyRow = dsItem.Tables[0].Rows[0];

                flag = false;
                foreach (string[] EachRecord in ProcessPairs)
                {
                    if ((MyRow[5].ToString().ToUpper() == EachRecord[0].ToUpper() ||
                        MyRow[5].ToString().ToUpper() == EachRecord[6].ToUpper()) && MyRow[2].ToString().ToUpper() == EachRecord[2].ToUpper())
                    {
                        flag = true; // the process has already been expanded
                        break;
                    }
                }
                if (!flag) //If not be expanded, than expand it. 
                {
                    Pair = new String[9] {MyRow[0].ToString(), MyRow[1].ToString(), MyRow[2].ToString(),
											 MyRow[3].ToString(), MyRow[4].ToString(), Convert.ToString(i),
											 MyRow[5].ToString(), MyRow[6].ToString(), MyRow[7].ToString()};  //"0" represents the common direction
                    ProcessPairs.Add(Pair);

                    DataRow NewRow = dtProcess.NewRow();
                    NewRow[0] = MyRow[0].ToString();
                    NewRow[1] = MyRow[1].ToString();
                    NewRow[2] = MyRow[2].ToString();
                    NewRow[3] = MyRow[3].ToString();
                    NewRow[4] = MyRow[4].ToString();
                    NewRow[5] = Convert.ToString(i);
                    NewRow[6] = MyRow[5].ToString();
                    NewRow[7] = MyRow[6].ToString();
                    NewRow[8] = MyRow[7].ToString();
                    dtProcess.Rows.Add(NewRow);
                }
            }

            //For each new expanded process, we find the related processes and try to expand it.
            while (i < steps)
            {
                i++;
                ArrayList temp_pairs = (ArrayList)ProcessPairs.Clone();
                foreach (string[] MyPair in temp_pairs)
                {
                    if (MyPair[5] == Convert.ToString(i - 1)) //check if the process is a new expaned process or not.
                    {
                        PairSet = GetAllRelatedProcessesInOrganism(pathwayListStr, organism, MyPair[6], pathway, sql_Entity_ID_List, false); //Get all processes connected to it.
                        //For each process, we try to expand it
                        foreach (DataSet dsItem in PairSet)
                        {
                            DataRow MyRow = dsItem.Tables[0].Rows[0];
                            flag = false;
                            foreach (string[] EachRecord in ProcessPairs)
                            {

                                if ((MyRow[5].ToString().ToUpper() == EachRecord[0].ToUpper() ||
                                    MyRow[5].ToString().ToUpper() == EachRecord[6].ToUpper()) && MyRow[2].ToString().ToUpper() == EachRecord[2].ToUpper())
                                /*
                                =======
                            if((MyRow[5].ToString().ToUpper() == EachRecord[0].ToUpper() ||
                                MyRow[5].ToString().ToUpper() == EachRecord[6].ToUpper()) && MyRow[2].ToString().ToUpper() == EachRecord[2].ToUpper())
                                >>>>>>> 1.2
                                 * */
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                Pair = new String[9] {MyPair[6], MyPair[7], MyRow[2].ToString().ToUpper(),
														 MyRow[3].ToString().ToUpper(),
														 MyRow[4].ToString().ToUpper(), Convert.ToString(i),
														 MyRow[5].ToString().ToUpper(), MyRow[6].ToString(),
														 MyRow[7].ToString().ToUpper()};
                                ProcessPairs.Add(Pair);
                                DataRow NewRow = dtProcess.NewRow();
                                NewRow[0] = MyPair[6];
                                NewRow[1] = MyPair[7];
                                NewRow[2] = MyRow[2].ToString();
                                NewRow[3] = MyRow[3].ToString();
                                NewRow[4] = MyRow[4].ToString();
                                NewRow[5] = Convert.ToString(i);
                                NewRow[6] = MyRow[5].ToString();
                                NewRow[7] = MyRow[6].ToString();
                                NewRow[8] = MyRow[7].ToString();
                                dtProcess.Rows.Add(NewRow);
                            }
                        }
                    }
                }
            }

            // Convert into a DataSet[], which is used in PW3
            foreach (DataRow drTemp in dtProcess.Rows)
            {
                DataSet dsTemp = new DataSet();
                DataTable dtTemp = dtProcess.Clone();
                dsTemp.Tables.Add(dtTemp);
                dsTemp.Tables[0].ImportRow(drTemp);
                dsProcessPairs.Add(dsTemp);
            }

            return (DataSet[])dsProcessPairs.ToArray(typeof(DataSet));
        }

	

		/// <summary>
		/// Expand processes that are a given number of steps away from a group of processes that contain
		/// a specific group of molectules.
		/// </summary>
		/// <param name="organism">The organism ID to use.</param>
		/// <param name="pathway">The pathway ID to use; some queries may set this to "".</param>
		/// <param name="process">The process ID to use.</param>
		/// <param name="steps">How many steps to consider.</param>
		/// <param name="entity_list">Molecules to consider.</param>
		/// <returns></returns>
		public static DataSet[] ExpandProcessesInGivenStepsCommonListInOrganism(string organism, string pathway, string process, int steps, string[] entity_list)
		{
			ArrayList dsProcessPairs = new ArrayList();
			DataTable dtProcess = NewTable();
			ArrayList ProcessPairs = new ArrayList();
			string[] Pair; //store the new expanded process.
			DataSet[] PairSet;
			int i = 1;
			bool flag;

			//create common entities list for the sql statement
			string sql_Entity_ID_List = "('" + process + "')";

			if(entity_list.Length > 0)
			{
				sql_Entity_ID_List = "(";
				for(int j = 0; j < entity_list.Length; j++)
				{
					sql_Entity_ID_List += "'" + entity_list[j] + "',";
				}

				sql_Entity_ID_List = sql_Entity_ID_List.Substring(0,sql_Entity_ID_List.Length - 1);
				sql_Entity_ID_List += ")";
			} 

			//For a given process, we get all processes connected to it.
			PairSet = GetAllRelatedProcessesInOrganism(organism, process, pathway, sql_Entity_ID_List, true);
			//For each process, we check if it = not
			foreach(DataSet dsItem in PairSet)
			{
				DataRow MyRow = dsItem.Tables[0].Rows[0];

				flag = false;
				foreach(string[] EachRecord in ProcessPairs) 
				{
					if((MyRow[5].ToString().ToUpper() == EachRecord[0].ToUpper() ||
                        MyRow[5].ToString().ToUpper() == EachRecord[6].ToUpper()) && MyRow[2].ToString().ToUpper() == EachRecord[2].ToUpper())
					{
						flag = true; // the process has already been expanded
						break;
					}  
				}
				if(!flag) //If not be expanded, than expand it. 
				{	
					Pair = new String[9] {MyRow[0].ToString(), MyRow[1].ToString(), MyRow[2].ToString(),
											 MyRow[3].ToString(), MyRow[4].ToString(), Convert.ToString(i),
											 MyRow[5].ToString(), MyRow[6].ToString(), MyRow[7].ToString()};  //"0" represents the common direction
					ProcessPairs.Add(Pair);
			
					DataRow NewRow = dtProcess.NewRow();
                    NewRow[0] = MyRow[0].ToString(); // generic_process_id
                    NewRow[1] = MyRow[1].ToString(); // generic_process_name
                    NewRow[2] = MyRow[2].ToString(); // mol_id
                    NewRow[3] = MyRow[3].ToString(); // mol_name
                    NewRow[4] = MyRow[4].ToString(); // use in mol_role
					NewRow[5] = Convert.ToString(i); // steps
                    NewRow[6] = MyRow[5].ToString(); // generic_process_id
                    NewRow[7] = MyRow[6].ToString(); // generic_process_name
                    NewRow[8] = MyRow[7].ToString(); // mol_role
					dtProcess.Rows.Add(NewRow);
				}
			}
		
			//For each new expanded process, we find the related processes and try to expand it.
			while(i < steps) 
			{
				i++;
				ArrayList temp_pairs = (ArrayList)ProcessPairs.Clone();
				foreach(string[] MyPair in temp_pairs)
				{
					if (MyPair[5] == Convert.ToString(i-1)) //check if the process is a new expaned process or not.
					{
						PairSet = GetAllRelatedProcessesInOrganism(organism, MyPair[6], pathway, sql_Entity_ID_List, false); //Get all processes connected to it.
						//For each process, we try to expand it
						foreach(DataSet dsItem in PairSet)
						{
							DataRow MyRow = dsItem.Tables[0].Rows[0];
							flag = false;
							foreach(string[] EachRecord in ProcessPairs)
							{

                                if ((MyRow[5].ToString().ToUpper() == EachRecord[0].ToUpper() ||
									MyRow[5].ToString().ToUpper() == EachRecord[6].ToUpper() ) && MyRow[2].ToString().ToUpper() == EachRecord[2].ToUpper())
                                    /*
                                    =======
								if((MyRow[5].ToString().ToUpper() == EachRecord[0].ToUpper() ||
                                    MyRow[5].ToString().ToUpper() == EachRecord[6].ToUpper()) && MyRow[2].ToString().ToUpper() == EachRecord[2].ToUpper())
                                    >>>>>>> 1.2
                                     * */
								{
									flag = true;
									break;
								}  
							}
							if(!flag)
							{
								Pair = new String[9] {MyPair[6], MyPair[7], MyRow[2].ToString().ToUpper(),
														 MyRow[3].ToString().ToUpper(),
														 MyRow[4].ToString().ToUpper(), Convert.ToString(i),
														 MyRow[5].ToString().ToUpper(), MyRow[6].ToString(),
														 MyRow[7].ToString().ToUpper()};
								ProcessPairs.Add(Pair);
								DataRow NewRow = dtProcess.NewRow();
								NewRow[0] = MyPair[6];
								NewRow[1] = MyPair[7];
								NewRow[2] = MyRow[2].ToString();
								NewRow[3] = MyRow[3].ToString();
								NewRow[4] = MyRow[4].ToString();
								NewRow[5] = Convert.ToString(i);
								NewRow[6] = MyRow[5].ToString();
								NewRow[7] = MyRow[6].ToString();
								NewRow[8] = MyRow[7].ToString();
								dtProcess.Rows.Add(NewRow);
							}
						}
					}
				}			
			}

			// Convert into a DataSet[], which is used in PW3
			foreach( DataRow drTemp in dtProcess.Rows )
			{
				DataSet dsTemp = new DataSet();
				DataTable dtTemp = dtProcess.Clone();
				dsTemp.Tables.Add(dtTemp);
				dsTemp.Tables[0].ImportRow(drTemp);
				dsProcessPairs.Add(dsTemp);
			}

			return (DataSet[])dsProcessPairs.ToArray(typeof(DataSet));
		}

		/// <summary>
		/// Finds all processes that involve exactly one substrate and one
		/// product in a specific organism.
		/// </summary>
		/// <param name="organism">The organism in question.</param>
		/// <returns>A filled DataSet object.</returns>
		public static DataSet[] ProcessesInvolvingOneSubstrateAndProductInOrganism(string organism)
		{
			SqlCommand command;

			if(organism == ServerOrganism.UnspecifiedOrganism)
			{
				// No organism predicate selected
				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT p.generic_process_id AS process_id, p.name AS process_name,
							me2.id AS entity_id, me2.name AS entity_name, per2.name AS role,
							pa.id AS pathway_id, pa.name AS pathway_name
						FROM processes p, process_entities pe2, molecular_entities me2, pathways pa,
							pathway_processes pp, process_entity_roles per2
						WHERE pp.pathway_id = pa.id AND p.id = pp.process_id AND p.id = pe2.process_id
							AND pe2.role_id = per2.role_id AND pe2.entity_id = me2.id
							AND (per2.name = 'product' OR per2.name = 'substrate') AND p.id IN (
							SELECT pe1.process_id
								FROM process_entities pe1, catalyzes c, process_entity_roles per1
								WHERE pe1.role_id = per1.role_id AND c.process_id = pe1.process_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
								GROUP BY pe1.process_id
								HAVING COUNT(*) = 2 )
						ORDER BY pathway_name, process_name, per2.role");
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT p.generic_process_id AS process_id, p.name AS process_name,
							me2.id AS entity_id, me2.name AS entity_name, per2.name AS role,
							pa.id AS pathway_id, pa.name AS pathway_name
						FROM processes p, process_entities pe2, molecular_entities me2, pathways pa,
							pathway_processes pp, process_entity_roles per2
						WHERE pp.pathway_id = pa.id AND p.id = pp.process_id AND p.id = pe2.process_id
							AND pe2.role_id = per2.role_id AND pe2.entity_id = me2.id
							AND (per2.name = 'product' OR per2.name = 'substrate') AND p.id IN (
							SELECT pe1.process_id
								FROM process_entities pe1, catalyzes c, process_entity_roles per1
								WHERE pe1.role_id = per1.role_id AND c.process_id = pe1.process_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
									AND " + InOrganismIdList + @"
								GROUP BY pe1.process_id
								HAVING COUNT(*) = 2 )
						ORDER BY pathway_name, process_name, per2.role");
			}
			
			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}
        /// <summary>
        /// Finds all processes that involve exactly one substrate and one
        /// product in a specific organism.
        /// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
        /// <param name="organism">The organism in question.</param>
        /// <returns>A filled DataSet object.</returns>
        public static DataSet[] ProcessesInvolvingOneSubstrateAndProductInOrganism(string pathwayListStr, string organism)
        {
            SqlCommand command;

            string sql_Pathway_ID_List = CreatePathway_ID_List(pathwayListStr);

            if (organism == ServerOrganism.UnspecifiedOrganism)
            {
                // No organism predicate selected
                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT p.generic_process_id AS process_id, p.name AS process_name,
							me2.id AS entity_id, me2.name AS entity_name, per2.name AS role,
							pw.id AS pathway_id, pw.name AS pathway_name
						FROM pathways pw, pathway_processes pp, processes p, process_entities pe2, molecular_entities me2, 
                       process_entity_roles per2
						WHERE pw.id  IN " + sql_Pathway_ID_List + @"  
                            AND pp.pathway_id = pw.id 
                            AND p.id = pp.process_id 
                            AND p.id = pe2.process_id
							AND pe2.role_id = per2.role_id AND pe2.entity_id = me2.id
							AND (per2.name = 'product' OR per2.name = 'substrate') AND p.id IN (
							SELECT pe1.process_id
								FROM process_entities pe1, catalyzes c, process_entity_roles per1
								WHERE pe1.role_id = per1.role_id AND c.process_id = pe1.process_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
								GROUP BY pe1.process_id
								HAVING COUNT(*) = 2 )
						ORDER BY pathway_name, process_name, per2.role");
            }
            else
            {
                // Specific organism selected
                ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
                string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT p.generic_process_id AS process_id, p.name AS process_name,
							me2.id AS entity_id, me2.name AS entity_name, per2.name AS role,
							pw.id AS pathway_id, pw.name AS pathway_name
						FROM pathways pw, pathway_processes pp, processes p,
                            process_entities pe2, molecular_entities me2, process_entity_roles per2
						WHERE pw.id  IN " + sql_Pathway_ID_List + @" 
                            AND pp.pathway_id = pw.id
                            AND p.id = pp.process_id 
							AND pe2.role_id = per2.role_id AND pe2.entity_id = me2.id
							AND (per2.name = 'product' OR per2.name = 'substrate') AND p.id IN (
							SELECT pe1.process_id
								FROM process_entities pe1, catalyzes c, process_entity_roles per1
								WHERE pe1.role_id = per1.role_id AND c.process_id = pe1.process_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
									AND " + InOrganismIdList + @"
								GROUP BY pe1.process_id
								HAVING COUNT(*) = 2 )
						ORDER BY pathway_name, process_name, per2.role");
            }

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            return ds;
        }



        /// <summary>
        /// Finds processes with a given number of molecules of a specific role
        /// in an organism.
        /// </summary>
        /// <param name="organism">The organism in question.</param>
        /// <param name="role">The role to consider.</param>
        /// <param name="number">How many molecules to consider.</param>
        /// <returns>A filled DataSet[] object.</returns>
        public static DataSet[] ProcessesWithGivenNumberOfMoleculesWithRoleInOrganism(string organism, string role, int number)
        {
            SqlCommand command;

            if (organism == ServerOrganism.UnspecifiedOrganism)
            {
                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT p.generic_process_id AS process_id, p.name AS process_name,
							me.id AS entity_id,	me.name AS entity_name,
							p.generic_process_id AS generic_process_id
						FROM processes p, process_entities pe, molecular_entities me, process_entity_roles per
						WHERE p.id = pe.process_id AND pe.role_id = per.role_id AND per.name = @role
							AND pe.entity_id = me.id AND (
								SELECT COUNT(*)
								FROM (
									SELECT DISTINCT entity_id
										FROM processes pr1, process_entities pe1, process_entity_roles per1
										WHERE pr1.id = pe1.process_id AND pr1.generic_process_id = p.generic_process_id
											AND pe1.role_id = per1.role_id AND per1.name = @role ) AS tmp ) >= @number
						ORDER BY p.generic_process_id",
                    "@role", SqlDbType.VarChar, role,
                    "@number", SqlDbType.TinyInt, number);
            }
            else
            {
                // Specific organism selected
                ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
                string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT p.name AS process_name, p.id AS process_id, 
							p.generic_process_id AS generic_process_id, me.name AS entity_name,
							me.id AS entity_id
						FROM processes p, process_entities pe, molecular_entities me,
							process_entity_roles per, catalyzes c
						WHERE c.process_id = pe.process_id AND p.id IN (
							SELECT DISTINCT pe1.process_id
								FROM process_entities pe1, process_entity_roles per1
								WHERE pe1.role_id = per1.role_id AND per1.name = @role
								GROUP BY pe1.process_id
								HAVING COUNT(*) >= @number )
							AND " + InOrganismIdList + @"
							AND p.id = pe.process_id AND pe.entity_id = me.id
							AND pe.role_id = per.role_id AND per.name = @role
						ORDER BY process_id, entity_id",
                    "@role", SqlDbType.VarChar, role,
                    "@number", SqlDbType.TinyInt, number);
            }

            DataSet[] ds = new DataSet[0];
            DBWrapper.LoadMultiple(out ds, ref command);

            return ds;
        }


		/// <summary>
		/// Finds processes with a given number of molecules of a specific role
		/// in an organism.
		/// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
		/// <param name="organism">The organism in question.</param>
		/// <param name="role">The role to consider.</param>
		/// <param name="number">How many molecules to consider.</param>
		/// <returns>A filled DataSet[] object.</returns>
		public static DataSet[] ProcessesWithGivenNumberOfMoleculesWithRoleInOrganism(string pathwayListStr, string organism, string role, int number)
		{
			SqlCommand command;
            string sql_Pathway_ID_List = CreatePathway_ID_List(pathwayListStr);         

			if(organism == ServerOrganism.UnspecifiedOrganism)
			{
				command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT p.generic_process_id AS process_id, p.name AS process_name,
							me.id AS entity_id,	me.name AS entity_name,
							p.generic_process_id AS generic_process_id
						FROM pathway_processes pp, processes p, process_entities pe, molecular_entities me, process_entity_roles per
						WHERE  pp.pathway_id IN " + sql_Pathway_ID_List + @" AND p.id = pp.process_id 
                            AND p.id = pe.process_id AND pe.role_id = per.role_id AND per.name = @role
							AND pe.entity_id = me.id AND (
								SELECT COUNT(*)
								FROM (
									SELECT DISTINCT entity_id
										FROM processes pr1, process_entities pe1, process_entity_roles per1
										WHERE pr1.id = pe1.process_id AND pr1.generic_process_id = p.generic_process_id
											AND pe1.role_id = per1.role_id AND per1.name = @role ) AS tmp ) >= @number
						ORDER BY p.generic_process_id",
					"@role", SqlDbType.VarChar, role,
					"@number", SqlDbType.TinyInt, number);
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT p.name AS process_name, p.id AS process_id, 
							p.generic_process_id AS generic_process_id, me.name AS entity_name,
							me.id AS entity_id
						FROM pathway_processes pp, processes p, process_entities pe, molecular_entities me,
							process_entity_roles per, catalyzes c
						WHERE pp.pathway_id IN " + sql_Pathway_ID_List + @" AND p.id = pp.process_id 
                            AND c.process_id = pe.process_id AND p.id IN (
							SELECT DISTINCT pe1.process_id
								FROM process_entities pe1, process_entity_roles per1
								WHERE pe1.role_id = per1.role_id AND per1.name = @role
								GROUP BY pe1.process_id
								HAVING COUNT(*) >= @number )
							AND " + InOrganismIdList + @"
							AND p.id = pe.process_id AND pe.entity_id = me.id
							AND pe.role_id = per.role_id AND per.name = @role
						ORDER BY process_id, entity_id",
					"@role", SqlDbType.VarChar, role,
					"@number", SqlDbType.TinyInt, number);
			}

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}

		/// <summary>
		/// Finds processes involving a specific pathway and ME in an organism
		/// </summary>
		/// <param name="organism">The organism in question</param>
		/// <param name="pathway">The pathway in question</param>
		/// <param name="molecule">The molecule in question</param>
		/// <returns>A DataSet array of processes involving the pathway and ME in the organism</returns>
		public static DataSet[] ProcessesInvolvingPathwayAndMolecularEntityInOrganism(string organism, string pathway, string molecule )
		{
			SqlCommand command;

			if(organism == ServerOrganism.UnspecifiedOrganism)
			{
				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT pr.generic_process_id AS process_id, pr.name AS process_name,
							pr.generic_process_id AS generic_process_id, per.name AS role,
							me.id AS entity_id, me.name
						FROM processes pr, pathway_processes pp, process_entities pe, pathways pa,
							molecular_entities me, process_entity_roles per
						WHERE pp.pathway_id = pa.id AND pa.id = @pathway AND pr.id = pp.process_id
							AND pe.role_id = per.role_id AND pr.id = pe.process_id
							AND pe.entity_id = me.id AND me.id = @molecule
						ORDER BY process_id",
					"@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway),
					"@molecule", SqlDbType.UniqueIdentifier, new Guid(molecule));
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT pr.id AS process_id, pr.name AS process_name,
							pr.generic_process_id AS generic_process_id, per.name AS role,
							me.id AS entity_id, me.name
						FROM processes pr, pathway_processes pp, process_entities pe, pathways pa,
							molecular_entities me, process_entity_roles per, catalyzes c
						WHERE pp.pathway_id = pa.id AND pa.id = @pathway AND pe.role_id = per.role_id
							AND " + InOrganismIdList + @"
							AND pr.id = pp.process_id AND pr.id = pe.process_id
							AND pe.entity_id = me.id AND me.id = @molecule AND c.process_id = pe.process_id
						ORDER BY process_id",
					"@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway),
					"@molecule", SqlDbType.UniqueIdentifier, new Guid(molecule));
			}

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}

		/// <summary>
		/// Finds processes sharing activators and inhibitors with processes of a pathway in a
		/// specific organism.
		/// </summary>
		/// <param name="organism"></param>
		/// <param name="pathway"></param>
		/// <param name="process"></param>
		/// <returns></returns>
		public static DataSet[] ProcessesSharingActivatorsAndInhibitorsWithProcessOfPathwayInOrganism(string organism, string pathway, string process)
		{
			SqlCommand command;

			if(organism == ServerOrganism.UnspecifiedOrganism)
			{
				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT per.name AS role
						FROM process_entities pe, process_entity_roles per
						WHERE pe.process_id = @process AND pe.role_id = per.role_id
							AND (per.name = 'inhibitor' OR per.name = 'activator')",
					"@process", SqlDbType.UniqueIdentifier, new Guid(process));
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT per.name AS role
						FROM process_entities pe, process_entity_roles per, catalyzes c
						WHERE pe.process_id = @process AND pe.role_id = per.role_id
							AND (per.name = 'inhibitor' OR per.name = 'activator')
							AND c.process_id = pe.process_id
							AND " + InOrganismIdList,
					"@process", SqlDbType.UniqueIdentifier, new Guid(process));
			}

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			if(ds.Length == 0)
			{
				return ds;
			}
			else
			{
				if(organism == ServerOrganism.UnspecifiedOrganism)
				{
					command = DBWrapper.BuildCommand(
						@"SELECT DISTINCT pr1.id AS process_id, pr1.name AS process_name,
								pr1.generic_process_id AS generic_process_id, per1.name AS role,
								me.name, me.id AS entity_id
							FROM process_entities pe1, processes pr1, molecular_entities me,
								process_entity_roles per1
							WHERE pr1.id IN (
								SELECT pp.process_id
									FROM pathway_processes pp
									WHERE pp.pathway_id = @pathway )
								AND pe1.entity_id IN (
								SELECT entity_id
									FROM process_entities pe2, process_entity_roles per2
									WHERE pe2.process_id = @process AND pe2.role_id = per2.role_id
										AND (per2.name = 'inhibitor' OR per2.name = 'activator') )
								AND pr1.id = pe1.process_id AND pe1.entity_id = me.id
								AND pe1.role_id = per1.role_id
								AND (per1.name = 'inhibitor' OR per1.name = 'activator')
							ORDER BY pr1.name",
						"@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway),
						"@process", SqlDbType.UniqueIdentifier, new Guid(process));
				}
				else
				{
					// Specific organism selected
					ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
					string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

					command = DBWrapper.BuildCommand(
						@"SELECT DISTINCT pr1.id AS process_id, pr1.name AS process_name,
								pr1.generic_process_id AS generic_process_id, per1.name AS role,
								me.name, me.id AS entity_id
							FROM process_entities pe1, processes pr1, molecular_entities me,
								process_entity_roles per1, catalyzes c
							WHERE pr1.id IN (
								SELECT pp.process_id
									FROM pathway_processes pp
									WHERE pp.pathway_id = @pathway )
								AND " + InOrganismIdList + @"
								AND pe1.entity_id IN (
								SELECT entity_id
									FROM process_entities pe2, process_entity_roles per2
									WHERE pe2.process_id = @process AND pe2.role_id = per2.role_id
										AND (per2.name = 'inhibitor' OR per2.name = 'activator') )
								AND pr1.id = pe1.process_id AND pe1.entity_id = me.id
								AND pe1.role_id = per1.role_id
								AND c.process_id = pe1.process_id
								AND (per1.name = 'inhibitor' OR per1.name = 'activator')
							ORDER BY pr1.name",
						"@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway),
						"@process", SqlDbType.UniqueIdentifier, new Guid(process));
				}

				DBWrapper.LoadMultiple( out ds, ref command );
				return ds;
			}
		}



		/// <summary>
		/// Get all related processes in an organism.
		/// </summary>
		/// <param name="organism">The organism ID to consider.</param>
		/// <param name="process">The process ID to consider.</param>
		/// <param name="pathway">The pathway ID to consider; this may be "".</param>
		/// <param name="entity_list">A list of molecular entities to consider.</param>
		/// <param name="first">Whether this is the first run or not.</param>
		/// <returns>A filled DataSet[] object.</returns>
		public static DataSet[] GetAllRelatedProcessesInOrganism(string organism, string process, string pathway, string entity_list, bool first)
		{
			SqlCommand command;
			
			if( organism == ServerOrganism.UnspecifiedOrganism )
			{
				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT op.id, op.name, op.entity_id, op.entity_name, op.role,
							pr.generic_process_id, pr.name, per.name AS role
						FROM pathway_processes pp, processes pr, process_entities pe,
							process_entity_roles per, (
							SELECT DISTINCT pr1.generic_process_id AS id, pr1.name, pe1.entity_id,
									per1.name AS role, me1.name AS entity_name
								FROM processes pr1, process_entities pe1, molecular_entities me1,
									process_entity_roles per1
								WHERE pr1.id = pe1.process_id AND pr1." + (first?"":"generic_process_") + @"id = @process
									AND pe1.role_id = per1.role_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
									AND pe1.entity_id NOT IN " + entity_list + @"
									AND pe1.entity_id = me1.id ) op
						WHERE " +(pathway==""?"":"pp.pathway_id = @pathway AND ") + @"
							pr.id = pe.process_id AND pe.role_id = per.role_id
							AND (per.name = 'product' OR per.name = 'substrate')
							AND pe.entity_id = op.entity_id AND pp.process_id = pe.process_id
							AND pr.generic_process_id <> op.id",
					"@process", SqlDbType.UniqueIdentifier, new Guid(process),
					"@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway==""?ServerOrganism.UnspecifiedOrganism:pathway));
			}
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				command = DBWrapper.BuildCommand(
					@"SELECT DISTINCT op.id, op.name, op.entity_id, op.entity_name, op.role, pr.id,
							pr.name, per.name AS role
						FROM pathway_processes pp, processes pr, process_entities pe,
							process_entity_roles per, catalyzes c, (
							SELECT DISTINCT pr1.id, pr1.name, pe1.entity_id,
									per1.name AS role, me1.name AS entity_name
								FROM processes pr1, process_entities pe1, molecular_entities me1,
									process_entity_roles per1
								WHERE pr1.id = pe1.process_id AND pr1.id = @process
									AND pe1.role_id = per1.role_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
									AND pe1.entity_id NOT IN " + entity_list + @"
									AND pe1.entity_id = me1.id ) op
						WHERE " + (pathway==""?"":"pp.pathway_id = @pathway AND ") + @"c.process_id = pr.id
							AND pr.id = pe.process_id AND pe.role_id = per.role_id
							AND (per.name = 'product' OR per.name = 'substrate')
							AND pe.entity_id = op.entity_id AND pp.process_id = pe.process_id
							AND pr.id <> op.id AND " + InOrganismIdList,
					"@process", SqlDbType.UniqueIdentifier, new Guid(process),
					"@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway==""?ServerOrganism.UnspecifiedOrganism:pathway));
			}

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}
	

		/// <summary>
		/// Get all related processes in an organism.
		/// </summary>
        /// <param name="pathwayListStr">The string consists of the related pathway ID to consider.</param>
		/// <param name="organism">The organism ID to consider.</param>
		/// <param name="process">The process ID to consider.</param>
		/// <param name="pathway">The pathway ID to consider; this may be "".</param>
		/// <param name="entity_list">A list of molecular entities to consider.</param>
		/// <param name="first">Whether this is the first run or not.</param>
		/// <returns>A filled DataSet[] object.</returns>
        public static DataSet[] GetAllRelatedProcessesInOrganism(string pathwayListStr, string organism, string process, string pathway, string entity_list, bool first)
		{
			SqlCommand command;

            string sql_Pathway_ID_List = CreatePathway_ID_List(pathwayListStr);
            string datasetStatement = @" pp.pathway_id  IN " + sql_Pathway_ID_List + @" AND ";

			if( organism == ServerOrganism.UnspecifiedOrganism )
			{
                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT op.id, op.name, op.entity_id, op.entity_name, op.role,
							pr.generic_process_id, pr.name, per.name AS role
						FROM pathway_processes pp, processes pr, process_entities pe,
							process_entity_roles per, (
							SELECT DISTINCT pr1.generic_process_id AS id, pr1.name, pe1.entity_id,
									per1.name AS role, me1.name AS entity_name
								FROM processes pr1, process_entities pe1, molecular_entities me1,
									process_entity_roles per1
								WHERE pr1.id = pe1.process_id AND pr1." + (first ? "" : "generic_process_") + @"id = @process
									AND pe1.role_id = per1.role_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
									AND pe1.entity_id NOT IN " + entity_list + @"
									AND pe1.entity_id = me1.id ) op
						WHERE " + (pathway == "" ? datasetStatement : " pp.pathway_id = @pathway AND ") + @"
							pr.id = pe.process_id AND pe.role_id = per.role_id
							AND (per.name = 'product' OR per.name = 'substrate')
							AND pe.entity_id = op.entity_id AND pp.process_id = pe.process_id
							AND pr.generic_process_id <> op.id",
                    "@process", SqlDbType.UniqueIdentifier, new Guid(process),
                    "@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway == "" ? ServerOrganism.UnspecifiedOrganism : pathway));

            }
			else
			{
				// Specific organism selected
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(organism));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

                command = DBWrapper.BuildCommand(
                    @"SELECT DISTINCT op.id, op.name, op.entity_id, op.entity_name, op.role, pr.id,
							pr.name, per.name AS role
						FROM pathway_processes pp, processes pr, process_entities pe,
							process_entity_roles per, catalyzes c, (
							SELECT DISTINCT pr1.id, pr1.name, pe1.entity_id,
									per1.name AS role, me1.name AS entity_name
								FROM processes pr1, process_entities pe1, molecular_entities me1,
									process_entity_roles per1
								WHERE pr1.id = pe1.process_id AND pr1.id = @process
									AND pe1.role_id = per1.role_id
									AND (per1.name = 'product' OR per1.name = 'substrate')
									AND pe1.entity_id NOT IN " + entity_list + @"
									AND pe1.entity_id = me1.id ) op
						WHERE " + (pathway == "" ? datasetStatement : " pp.pathway_id = @pathway AND ") + @"c.process_id = pr.id
							AND pr.id = pe.process_id AND pe.role_id = per.role_id
							AND (per.name = 'product' OR per.name = 'substrate')
							AND pe.entity_id = op.entity_id AND pp.process_id = pe.process_id
							AND pr.id <> op.id AND " + InOrganismIdList,
                    "@process", SqlDbType.UniqueIdentifier, new Guid(process),
                    "@pathway", SqlDbType.UniqueIdentifier, new Guid(pathway == "" ? ServerOrganism.UnspecifiedOrganism : pathway));

                   }

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}
	}
}