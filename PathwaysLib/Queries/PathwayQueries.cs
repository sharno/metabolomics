using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using PathwaysLib.ServerObjects;
using PathwaysLib.Utilities;

namespace PathwaysLib.Queries
{
	/// <summary>
	/// Contains utility functions related to pathways used by the
	/// built-in queries and other services to help build up information
	/// for tabular queries.
	/// Transferred by Greg Strnad from the old pathways service.
	/// </summary>
	public class PathwayQueries
	{
		private PathwayQueries() {}

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
		/// Finds all pathways related to the given pathway.
		/// </summary>
		/// <param name="pathway_id">The pathway in question.</param>
		/// <returns>A filled DataSet object.</returns>
		public static DataSet[] GetAllRelatedPathways(string pathway_id, string sql_Pathway_ID_List)
		{
			SqlCommand command = DBWrapper.BuildCommand(
				@"SELECT DISTINCT pm.pathway_id, pa.name, pm.entity_id, me.name
					FROM pathways pa, molecular_entities me, (
						SELECT pathway_id_1 AS pathway_id, entity_id
							FROM pathway_links
							WHERE pathway_id_2 = @pathway_id
						 ) pm
					WHERE pa.id  IN " + sql_Pathway_ID_List + @"AND pa.id = pm.pathway_id AND me.id = pm.entity_id",
				"@pathway_id", SqlDbType.UniqueIdentifier, new Guid(pathway_id));

			DataSet[] ds = new DataSet[0];
			DBWrapper.LoadMultiple( out ds, ref command );

			return ds;
		}

		/// <summary>
		///	Creates a table to store all expanded processes' information.
		/// </summary>
		/// <returns>An instance of DataTable.</returns>
		private static DataTable NewPathwayTable()

		{
			DataTable PathwayTable = new DataTable("Pathway");
			// Add three column objects to the table.
			DataColumn Original_Pathway_Id_Column = new DataColumn();
			Original_Pathway_Id_Column.DataType = System.Type.GetType("System.String");
			Original_Pathway_Id_Column.ColumnName = "Original_Pathway_id";
			PathwayTable.Columns.Add(Original_Pathway_Id_Column);

			DataColumn Original_Pathway_Name_Column = new DataColumn();
			Original_Pathway_Name_Column.DataType = System.Type.GetType("System.String");
			Original_Pathway_Name_Column.ColumnName = "Original_Pathway_Name";
			PathwayTable.Columns.Add(Original_Pathway_Name_Column);

			DataColumn Molecular_Entity_Id_Column = new DataColumn();
			Molecular_Entity_Id_Column.DataType = System.Type.GetType("System.String");
			Molecular_Entity_Id_Column.ColumnName = "Molecular_Entity_Id";
			PathwayTable.Columns.Add(Molecular_Entity_Id_Column);
			
			DataColumn Molecular_Entity_Name_Column = new DataColumn();
			Molecular_Entity_Name_Column.DataType = System.Type.GetType("System.String");
			Molecular_Entity_Name_Column.ColumnName = "Molecular_Entity_Name";
			PathwayTable.Columns.Add(Molecular_Entity_Name_Column);

			DataColumn Step_Column = new DataColumn();
			Step_Column.DataType = System.Type.GetType("System.String");
			Step_Column.ColumnName = "Step";
			PathwayTable.Columns.Add(Step_Column);
			
			DataColumn Next_Pathway_Id_Column = new DataColumn();
			Next_Pathway_Id_Column.DataType = System.Type.GetType("System.String");
			Next_Pathway_Id_Column.ColumnName = "Next_Pathway_id";
			PathwayTable.Columns.Add(Next_Pathway_Id_Column);

			DataColumn Next_Pathway_Name_Column = new DataColumn();
			Next_Pathway_Name_Column.DataType = System.Type.GetType("System.String");
			Next_Pathway_Name_Column.ColumnName = "Next_Pathway_Name";
			PathwayTable.Columns.Add(Next_Pathway_Name_Column);

			return PathwayTable;
		}

		/// <summary>
		///	Expands a pathway in a given step.
		/// </summary>
		///<param name="pathway_id">The ID of a pathway.</param>
		///<param name="pathway_name">The name of the pathway.</param>
		///<param name="steps">How many steps will be expanded.</param>
		///<returns>An instance of DataSet including all expanded pathways.</returns>
		public static DataSet[] ExpandPathwaysInGivenSteps(string pathwayListStr, string pathway_id, string pathway_name, int steps)
		{
            string sql_Pathway_ID_List = CreatePathway_ID_List(pathwayListStr);

			ArrayList dsPathwayPairs = new ArrayList();
			DataTable dtPathway = NewPathwayTable();
			ArrayList PathwayPairs = new ArrayList();
			string[] Pair; //store the new expanded process.
			DataSet[] PairSet;
			int i = 1;
			bool flag;
			ArrayList ExtendedPathways = new ArrayList();

            /// *********
			//For a given pathway, we get all pathways connected to it.
            PairSet = GetAllRelatedPathways(pathway_id, sql_Pathway_ID_List); // returns immediate neighbors
			//For each pathway, we check if it = not
			foreach(DataSet dsItem in PairSet)
			{	
				DataRow MyRow = dsItem.Tables[0].Rows[0];
                ///////////////////// source_pwid, source_pwname, mol_id, mol_name, step, next_pwid, next_pwname
				Pair = new String[7] {pathway_id, pathway_name, MyRow[2].ToString(), MyRow[3].ToString(),
										 Convert.ToString(i), MyRow[0].ToString(), MyRow[1].ToString()};
				PathwayPairs.Add(Pair);
				DataRow NewRow = dtPathway.NewRow();
                NewRow[0] = pathway_id; // source_pwid
                NewRow[1] = pathway_name; // source_pwname
                NewRow[2] = MyRow[2].ToString(); // mol_id
                NewRow[3] = MyRow[3].ToString(); // mol_name
                NewRow[4] = Convert.ToString(i); // step
                NewRow[5] = MyRow[0].ToString(); // next_pwid
                NewRow[6] = MyRow[1].ToString(); // next_pwname
				dtPathway.Rows.Add(NewRow);
			}
			ExtendedPathways.Add(pathway_id);
		
			//For each new expanded process, we find the related processes and try to expand it.
			while(i < steps) 
			{
				i++;
				ArrayList temp_pairs = (ArrayList)PathwayPairs.Clone();
				foreach(string[] MyPair in temp_pairs)
				{
					if ((MyPair[4] == Convert.ToString(i-1)) && (ExtendedPathways.IndexOf(MyPair[5]) < 0)) //check if the process is a new expaned process or not.
					{
						ExtendedPathways.Add(MyPair[5]);

                        PairSet = GetAllRelatedPathways(MyPair[5], sql_Pathway_ID_List); //Get all processes connected to it.
						//For each pathway, we try to expand it
						foreach(DataSet dsItem in PairSet)
						{	
							DataRow MyRow = dsItem.Tables[0].Rows[0];
							flag = false;
							foreach(string[] EachRecord in PathwayPairs)
							{
								if(((MyPair[5] == EachRecord[0]) && (MyRow[0].ToString() == EachRecord[5]) &&
									(MyRow[2].ToString() == EachRecord[2]))||((MyPair[5] == EachRecord[5]) &&
									(MyRow[0].ToString() == EachRecord[0]) && (MyRow[2].ToString() == EachRecord[2])))
								{
									flag = true;
									break;
								}
							}
							if(!flag)
							{
								Pair = new String[7] {MyPair[5], MyPair[6], MyRow[2].ToString(),
														 MyRow[3].ToString(), Convert.ToString(i),
														 MyRow[0].ToString(), MyRow[1].ToString()};
								PathwayPairs.Add(Pair);
								DataRow NewRow = dtPathway.NewRow();
								NewRow[0] = MyPair[5];
								NewRow[1] = MyPair[6];
								NewRow[2] = MyRow[2].ToString();
								NewRow[3] = MyRow[3].ToString();
								NewRow[4] = Convert.ToString(i);
								NewRow[5] = MyRow[0].ToString();
								NewRow[6] = MyRow[1].ToString();
								dtPathway.Rows.Add(NewRow);
							}
						}
					}
				}
			}

			// Convert into a DataSet[], which is used in PW3
			foreach( DataRow drTemp in dtPathway.Rows )
			{
				DataSet dsTemp = new DataSet();
				DataTable dtTemp = dtPathway.Clone();
				dsTemp.Tables.Add(dtTemp);
				dsTemp.Tables[0].ImportRow(drTemp);
				dsPathwayPairs.Add(dsTemp);
			}

			return (DataSet[])dsPathwayPairs.ToArray(typeof(DataSet));
		}

        public static Dictionary<string, SignificanceTestResultItem> FindSignificantlyEnrichedPathwaysWithGenes(string pathwayList, Dictionary<string, string> Genes, string orgId)
        {
            string geneIdList = "";
            foreach (string geid in Genes.Keys)
                geneIdList += "'" + geid + "', ";

            geneIdList = geneIdList.Substring(0, geneIdList.Length - 2);
            geneIdList = "(" + geneIdList + ")";

            string sql_id_set = MolecularEntityQueries.CreatePathway_ID_List(pathwayList);

            string query = @"SELECT DISTINCT pw.id, pw.name, ge.gene_id
                             FROM pathways pw, pathway_processes pp, catalyzes c, gene_encodings ge, genes g
                             WHERE pw.id = pp.pathway_id
                             AND pp.process_id = c.process_id
                             AND c.gene_product_id = ge.gene_product_id  
                             AND g.id = ge.gene_id
                             AND g.organism_group_id='" + orgId + @"'                           
                             AND pw.id in
                             (
                                 SELECT pw.id
                                 FROM pathways pw, pathway_processes pp, catalyzes c, gene_encodings ge
                                 WHERE pw.id = pp.pathway_id
                                 AND pp.process_id = c.process_id
                                 AND c.gene_product_id = ge.gene_product_id
                                 AND ge.gene_id IN " + geneIdList + @"
                                 AND pw.id IN " + sql_id_set + @"
                             )
                             ORDER BY pw.id DESC";


            DataSet ds;
            SqlCommand command = new SqlCommand(query);
            DBWrapper.Instance.ExecuteQuery(out ds, ref command);

            Dictionary<string, SignificanceTestResultItem> results = new Dictionary<string, SignificanceTestResultItem>();
            DataRow dr;
            string name, id, gid;
            Guid geneId;
            SignificanceTestResultItem strItem;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = (DataRow)ds.Tables[0].Rows[i];

                name = dr["name"].ToString();
                id = dr["id"].ToString();
                gid = dr["gene_id"].ToString();

                if (!results.TryGetValue(id, out strItem))
                {
                    strItem = new SignificanceTestResultItem(name, id);
                    results.Add(id, strItem);
                }

                if (Genes.ContainsKey(gid))
                    strItem.AssociatedInputItemIds.Add(gid);
                strItem.SampleSize++;
            }

            int NumGenes = ServerGene.GetNumOfGenes(orgId);
            foreach (SignificanceTestResultItem item in results.Values)
            {
                if (Genes.Count < item.AssociatedInputItemIds.Count || NumGenes < item.SampleSize || Genes.Count > NumGenes || item.AssociatedInputItemIds.Count > item.SampleSize)
                    throw new Exception(item.ItemID + " # " + item.ItemName + " : Calling HyperGD with " + Genes.Count + ", " + item.AssociatedInputItemIds.Count + ", " + NumGenes + ", " + item.SampleSize);
                item.pValue = HypergeometricDistribution.Evaluate(Genes.Count, item.AssociatedInputItemIds.Count, NumGenes, item.SampleSize);
            }
            return results;
        }
	}
}
