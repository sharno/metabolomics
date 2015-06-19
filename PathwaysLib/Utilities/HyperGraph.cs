using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using PathwaysLib.ServerObjects;

namespace PathwaysLib.Utilities
{
	/// <summary>
	///	The HyperGraph class is used to represent and store a pathway.
	///	In this class, we use an extended adjacency-list to store pathway data,
	///	use DFS to compute paths between two given molecular entities, and use
	///	BFS to compute neighbors of a given molecular entity.
	/// </summary>
	/// <remarks>
	/// This was imported from the old pathways service and likely will not be too
	/// useful for anything but other old functions that were migrated over to the
	/// current system.  Nothing has been really rewritten except the parts that would
	/// otherwise crash and burn.
	/// 
	/// Here be dragons.
	/// </remarks>
	public class HyperGraph
	{
		/// <summary>
		///		A variant to store an edge that includes product_id, product_name, process_id, Process_name;
		/// </summary>
		string[] NewEdge; 
		/// <summary>
		///		A variant to store an extended ajacency-list. The key field is used to store substrate_id, and the value field is used to store an edge-list according to the substrate_id. 																	   Each edge in edge-list includes a product id, it's name, a process id, and it's name, and it means the substrate can reach the product in the process.
		/// </summary>
		private SortedList NodesList;
		/// <summary>
		///		A variant to store nodes' state information, and the node can be a substrate or product. The key field is Entity_id(substrate_id or product_id), and the value field can be true or false:True for "visited", False for "unvisited"
		/// </summary>
		private SortedList NodeVisited = new SortedList();
		/// <summary>
		///		A variant to store processes' state information. The key field is process_id, and the value field can be true or false:True for "visited", False for "unvisited"
		/// </summary>
		private SortedList ProcessVisited = new SortedList();
		/// <summary>
		///		Database connection
		/// </summary>
		SqlConnection objConn;

		private string __unspecified = "00000000-0000-0000-0000-000000000000";

		private Hashtable EdgeVisited = new Hashtable();

			
		/// <summary>
		///		The function is used to initilize state information for each node and process.
		/// </summary>
		/// <param name="MyRow">The record includes the following information: substrate_id,substrate_name,product_id,product_name,process_id,process_name</param>
		/// <remarks>
		///		We need initilize each substrate and product "unvisited" and each process "unvisited"
		/// </remarks>
		private void Maintain(ArrayList MyRow)
		{
			if(!NodeVisited.ContainsKey(MyRow[0].ToString().ToUpper()))
				NodeVisited.Add(MyRow[0].ToString().ToUpper(),false);

			if(!NodeVisited.ContainsKey(MyRow[2].ToString().ToUpper()))
				NodeVisited.Add(MyRow[2].ToString().ToUpper(),false);

			if(!ProcessVisited.ContainsKey(MyRow[4].ToString().ToUpper()))
				ProcessVisited.Add(MyRow[4].ToString().ToUpper(),false);
		}


		private void VisitEdge(string [] edge, string id)
		{
			string name=id+edge[0].ToUpper()+edge[2].ToUpper();
			EdgeVisited.Add(name,1);
				
		}
		private bool IsEdgeVisited(string [] edge, string id)
		{
			string name=id+edge[0].ToUpper()+edge[2].ToUpper();
			return EdgeVisited.ContainsKey(name);
		}


		/// <summary>
		///		Set state information for a given node with a given state(true, false)	
		/// </summary>
		/// <param name="Node_Id">The id of a given node: substrate or product</param>
		/// <param name="Visited">True or False</param>
		//
		private void VisitNode(string Node_Id,bool Visited)
		{
			NodeVisited.SetByIndex(NodeVisited.IndexOfKey(Node_Id),Visited);
		}

		/// <summary>
		/// check whether the node is visited or not
		/// </summary>
		/// <param name="Node_Id">he id of a given node: substrate or product</param>
		/// <returns>True("visited") or False("univisted")</returns>
		private bool IsNodeVisited(string Node_Id)
		{
			return (bool)NodeVisited.GetByIndex(NodeVisited.IndexOfKey(Node_Id));
		}

		/// <summary>
		///		Set state information for a given process with a given state.
		/// </summary>
		/// <param name="Process_Id">The id of a given process</param>
		/// <param name="Visited">True("visited") or False("univisted"</param>
		private void VisitProcess(string Process_Id,bool Visited)
		{
			ProcessVisited.SetByIndex(ProcessVisited.IndexOfKey(Process_Id),Visited);
		}

		/// <summary>
		/// check whether the process is visited or not
		/// </summary>
		/// <param name="Process_Id">The id of a given process</param>
		/// <returns>True("visited") or False("univisted")</returns>
		private bool IsProcessVisited(string Process_Id)
		{
			return (bool) ProcessVisited.GetByIndex(ProcessVisited.IndexOfKey(Process_Id));
		}
			
		/// <summary>
		///		Given the id of an substrate id, get the edgelist for it from the extended adjancency-list.
		/// </summary>
		/// <param name="Node_id">The id of a substrate</param>
		/// <returns>An arraylist that includes an edge list, and each egde includes a product_id, product name, process_id and process name</returns>
		private ArrayList GetEdgeList(string Node_id)
		{
			ArrayList EdgeList = new ArrayList();
			if (NodesList.ContainsKey(Node_id)) EdgeList = (ArrayList)NodesList.GetByIndex(NodesList.IndexOfKey(Node_id));

			return EdgeList;
		}

			
		private bool ContainEdge(ArrayList EdgeList, string Proc_id, string Prod_id)
		{
			bool result = false;
			foreach(string[] edge in EdgeList)
			{
				if(edge[2]== Proc_id && Prod_id == edge[0])
				{
					result = true;
					break;
				}
			}
			return result;
		}
			

		/// <summary>
		///		Add a new substrate id to the NodesList
		/// </summary>
		/// <param name="Substrate_ID">The record includes the following information: substrate_id,substrate_name,product_id,product_name,process_id,process_name</param>
		/// <returns>An empty edge list of this substrate.</returns>
		private ArrayList AddNode(string Substrate_ID)
		{
			ArrayList EdgeList = new ArrayList();
			NodesList.Add(Substrate_ID,EdgeList);
			return EdgeList;
		}
			
		/// <summary>
		///		If the edge is reversible, we exchange subtrates and products and do it like procedure AddEdge do.
		/// </summary>
		/// <param name="MyRow">The record includes the following information: substrate_id,substrate_name,product_id,product_name,process_id,process_name</param>			
		private void AddReversibleEdge(ArrayList MyRow)
		{
			ArrayList EdgeList;
			//substrate_id, substrate_name, product_id, product_name, process_id, process_name, reversible
			//I can new a sortedlist that stores the pairs of substrate_id and substrate_name.
			if (!NodesList.ContainsKey(MyRow[2].ToString().ToUpper()))//product_id
			{
				EdgeList = AddNode(MyRow[2].ToString().ToUpper());
			}
			else
			{
				EdgeList = (ArrayList)GetEdgeList(MyRow[2].ToString().ToUpper());//substrate_id
			}

			if(!ContainEdge(EdgeList,MyRow[4].ToString().ToUpper(),MyRow[0].ToString().ToUpper()))
			{
				NewEdge = new string[4] {MyRow[0].ToString().ToUpper(),MyRow[1].ToString(),MyRow[4].ToString().ToUpper(),MyRow[5].ToString()}; //product_id,product_Name,process_id,process_name
				EdgeList.Add(NewEdge);
			}
		}

		/// <summary>
		///		Add an edge to an edgelist
		/// </summary>
		/// <param name="MyRow">The record includes the following information: substrate_id,substrate_name,product_id,product_name,process_id,process_name</param>
		/// <remarks>
		///		If substrate_id is not in the extended adjacency list, we add it and new a edgelist for it; otherwise, we get the edgelist form the extended adjacency list.
		///		catch{}finally, construct a new edge from the parameter MyRow and add it into the edgelist.
		/// </remarks>
		private void AddEdge(ArrayList MyRow)
		{
			ArrayList EdgeList;

			//I can new a sortedlist that stores the pairs of substrate_id and substrate_name.
			if (!NodesList.ContainsKey(MyRow[0].ToString().ToUpper()))//subsrate_id
			{
				EdgeList = AddNode(MyRow[0].ToString().ToUpper());
			}
			else
			{
				EdgeList = (ArrayList)GetEdgeList(MyRow[0].ToString().ToUpper());//substrate_id
			}
				
			if(!ContainEdge(EdgeList,MyRow[4].ToString().ToUpper(),MyRow[2].ToString().ToUpper()))
			{
				NewEdge = new string[4] {MyRow[2].ToString().ToUpper(),MyRow[3].ToString(),MyRow[4].ToString().ToUpper(),MyRow[5].ToString()}; //product_id,product_Name,process_id,process_name
				EdgeList.Add(NewEdge);
			}
		}

		/// <summary>
		///		In this procedure, we execute the pre-defined sql and get all (substrate, product) pair for each process in a given pathway. 
		///		For each (substrate, product) pair in a process, we get the edge list of the substrate, and contruct a new edge and try to add it the edge list. If the process is reversible, we need exchange the substrate and the product, construct another edge and add it into the edge list of the product.
		///		At the same time, we initialize all substrates, products and processes' state information.
		/// </summary>
		/// <param name="strSQL">A sql statement</param>
		/// <param name="bReversible">This variable represents whether all processes are reversible or not</param>
		/// <param name="Common_Entities_List">Common entities</param>
		private void GenerateHyperGraph(string strSQL, bool bReversible, Array Common_Entities_List)
		{
			objConn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["dbConnectString"]);
			objConn.Open();
			string temp;

			//fill data into table.
			try 
			{
				SqlDataAdapter daPathways = new SqlDataAdapter(strSQL, objConn);
				DataSet dsPathways = new DataSet("Pathway");
				daPathways.Fill(dsPathways,"Pathway");

				NodesList = new SortedList();
			
				foreach (DataRow MyRow in dsPathways.Tables["Pathway"].Rows)
				{
					if((Array.BinarySearch(Common_Entities_List, MyRow[0].ToString().ToUpper()) < 0) && (Array.BinarySearch(Common_Entities_List, MyRow[2].ToString().ToUpper()) < 0))
					{
						ArrayList row = new ArrayList();
						for( int x = 0; x < MyRow.ItemArray.Length; x++ )
						{
							row.Add(MyRow[x].ToString());
						}

						temp = row[4].ToString();
						row[4] = "+" + temp;
						AddEdge( row ); //Try to add edge
						Maintain( row );

						if( ( row[6].ToString() == "1" ) || ( row[6].ToString() == "yes" ) || bReversible ) 
						{
							row[4] = "-" + temp;
							AddReversibleEdge( row ); //If the process is reversible, then try to add a new edge with exchange of substrates and products.
							Maintain( row ); //Initilize the process and molecular entities' state information.
						}
					}
				}
			}
			catch(Exception)
			{
				// Eh?
			}
			finally
			{
				objConn.Close();
			}
		}

		public HyperGraph(string Pathway_ID,string Activator_ID)
		{
			//string strSQL;
		}

		/// <summary>
		///		Construct a sql statement by using a given pathway ID and a given organism name and call the GenerateHyperGraph function.
		/// </summary>
		/// <param name="Organism_Name">The name of an organism</param>
		/// <param name="Organism_Group">The organism group</param>
		/// <param name="Pathway_ID">The ID of a pathway.</param>
		/// <param name="bReversible">This variable represents whether all processes are reversible or not</param>
		/// <param name="Common_Entities_List">A list of common entities</param>
		public HyperGraph(string Organism_Name,string Organism_Group, string Pathway_ID, bool bReversible, string[] Common_Entities_List)
		{
			string strSQL ="";

			if (Pathway_ID == "")
			{
				Pathway_ID = "*";
			}

			if( Organism_Group == __unspecified )
			{
				strSQL =
					@"SELECT DISTINCT Substrates.entity_id AS Substrate_id, Substrates.name AS Substrate_name,
							pe2.entity_id AS Product_id, me2.name AS Product_name, Substrates.Process_id,
							Substrates.Process_name, Substrates.reversible
						FROM processes pr2, process_entities pe2, molecular_entities me2,
							process_entity_roles per2, (
							SELECT DISTINCT pe.entity_id, me.name, pr.generic_process_id AS Process_id,
									pr.name AS process_name, pr.reversible
								FROM pathway_processes pp, processes pr, process_entities pe,
									molecular_entities me, process_entity_roles per
								WHERE pp.process_id = pr.id
									AND pp.pathway_id = '" + Pathway_ID + @"' AND pr.id = pe.process_id
									AND pe.role_id = per.role_id AND per.name = 'substrate'
									AND pe.entity_id = me.id ) Substrates
						WHERE Substrates.process_id = pr2.generic_process_id AND pr2.id = pe2.process_id
							AND pe2.role_id = per2.role_id AND per2.name = 'product'
							AND me2.id = pe2.entity_id
						ORDER BY Substrate_id, Product_id, Substrates.Process_id";
			}
			else
			{
				// Specific organism
				ServerOrganismGroup org = ServerOrganismGroup.LoadFromID(new Guid(Organism_Group));
				string InOrganismIdList = ServerOrganismGroup.RequireIdInList(org.GetAllChildrenAndParent(), "c", "organism_group_id");

				strSQL =
					@"SELECT DISTINCT Substrates.entity_id AS Substrate_id, Substrates.name AS Substrate_name,
							pe2.entity_id AS Product_id, me2.name AS Product_name, Substrates.Process_id,
							Substrates.Process_name, Substrates.reversible
						FROM process_entities pe2, molecular_entities me2, process_entity_roles per2, (
							SELECT DISTINCT pe.entity_id, me.name, pe.process_id, pr.name AS process_name,
									pr.reversible
								FROM pathway_processes pp, processes pr, process_entities pe,
									molecular_entities me, process_entity_roles per, catalyzes c
								WHERE pp.pathway_id = '" + Pathway_ID + @"' AND pp.process_id = pr.id
									AND c.process_id = pr.id AND " + InOrganismIdList + @"
									AND pr.id = pe.process_id AND pe.role_id = per.role_id
									AND per.name = 'substrate' AND pe.entity_id = me.id ) Substrates
						WHERE Substrates.process_id = pe2.process_id AND pe2.role_id = per2.role_id
							AND per2.name = 'product' AND me2.id = pe2.entity_id
						ORDER BY Substrate_id, Product_id, Substrates.Process_id";				
			}
			
			Array arrCommon_Entities_List = Array.CreateInstance(typeof (string),Common_Entities_List.Length);
			for(int i = 0; i < Common_Entities_List.Length; i ++)
			{
				arrCommon_Entities_List.SetValue(Common_Entities_List[i],i);
			}
			Array.Sort(arrCommon_Entities_List);
			GenerateHyperGraph(strSQL, bReversible, Common_Entities_List);
		}

		/// <summary>
		///		Find a name for a given molecular entity id
		/// </summary>
		/// <param name="Entity_ID">The ID of a given entity</param>
		/// <returns>An string</returns>
		/// <remarks>An string to store the entity name</remarks>
		public string Get_Entity_Name(string Entity_ID)
		{
			if( Entity_ID == __unspecified )
			{
				return "unspecified";
			}
			else
			{
				string sql_string = @"SELECT me.name
									FROM molecular_entities me
									WHERE me.id = '" + Entity_ID + "'";

				SqlDataAdapter daPathways = new SqlDataAdapter(sql_string, objConn);
				DataSet dsPathways = new DataSet("Processes");
				//daPathways.FillSchema(dsPathways,SchemaType.Source, "Pathway");
				daPathways.Fill(dsPathways,"Processes");
				if (dsPathways.Tables["Processes"].Rows.Count > 0)
				{
					return dsPathways.Tables["Processes"].Rows[0][0].ToString();
				}
				else
				{
					return "";
				}
			}
		}

		private void Compute(ArrayList PathList,ArrayList Path,string[] NextEdge,string Molecular_Entity_ID_A,string Entity_Name_A,string Molecular_Entity_ID_B)
		{
			//check whether node or process has been visited or not
			if ((IsNodeVisited(NextEdge[0].ToString()))||(IsProcessVisited(NextEdge[2].ToString())))
				return;
	
			string[] Step = new string[6]{Molecular_Entity_ID_A,Entity_Name_A, NextEdge[0],NextEdge[1],NextEdge[2],NextEdge[3]}; //construct a step
			string[] ReverseStep;
			if (NextEdge[2].Substring(0,1) == "+") //construct reverse step.
			{
				ReverseStep = new String[6]{NextEdge[0],NextEdge[1],Molecular_Entity_ID_A,Entity_Name_A,"-" + NextEdge[2].Substring(1,NextEdge[2].Length - 1),NextEdge[3]};
			}
			else
			{
				ReverseStep = new String[6]{NextEdge[0],NextEdge[1],Molecular_Entity_ID_A,Entity_Name_A,"+" + NextEdge[2].Substring(1,NextEdge[2].Length - 1),NextEdge[3]};
			}

			if(!Path.Contains(ReverseStep)) Path.Add(Step); //Add the edge into path
			else return;

			if((Molecular_Entity_ID_B == "")||(NextEdge[0] == Molecular_Entity_ID_B)) //check whether destination node is found or not
			{
				//If pathlist doesn't include this new path,then add it to list.
				if (!PathList.Contains(Path)) PathList.Add(Path);
			}
			else
			{
				//follow next edge
				VisitNode(NextEdge[0].ToString(),true); //Visit the prodcut 
				VisitProcess(NextEdge[2].ToString(),true); //Visite the process
				foreach(string[] MyEdge in GetEdgeList(NextEdge[0]))
				{
					ArrayList NewPath = (ArrayList)Path.Clone();				
					Compute(PathList,NewPath,MyEdge,NextEdge[0],NextEdge[1],Molecular_Entity_ID_B);
				}
				VisitNode(NextEdge[0].ToString(),false); //unvisit the product
				VisitProcess(NextEdge[2].ToString(),false); //unvisit the process
			}
		}

		/// <summary>
		///		Compute paths between two molecular_entities_Wanhong. We have a document to talk about the algorithm. We use DFS to compute paths, and make sure that in a path, a process can't occur two times.
		/// </summary>
		/// <param name="Molecular_Entity_ID_A">The source node</param>
		/// <param name="Entity_Name_A">Source node name</param>
		/// <param name="Molecular_Entity_ID_B">The destination node</param>
		/// <returns>An arraylist including all paths between two nodes</returns>
		public ArrayList ComputePaths(string Molecular_Entity_ID_A,string Entity_Name_A,string Molecular_Entity_ID_B)
		{
			ArrayList PathList; //Stores all pathes
			ArrayList Path;     //Stores one path between two molecular_entities_Wanhong.

			PathList = new ArrayList();
			//Starting at source entity, we follow each edge in its edgelist to try to find the destination node.			
			foreach(string[] EachEdge in GetEdgeList(Molecular_Entity_ID_A))
			{
				VisitNode(Molecular_Entity_ID_A,true);
				Path = new ArrayList();
				Compute(PathList,Path,EachEdge,Molecular_Entity_ID_A,Entity_Name_A,Molecular_Entity_ID_B); //
			}

			return PathList;
		}

		/// <summary>
		///		The function is to used to expand an entity and get its neighbors in a given step.
		/// </summary>
		/// <param name="Molecular_Entity_ID_A">The id of an entity that will be expanded</param>
		/// <param name="Entity_Name">The name of the entity</param>
		/// <param name="step">The pre-defined maximum steps</param>
		/// <returns>An arraylist including all expended edges.</returns>
		public ArrayList Expand_An_Entity(string Molecular_Entity_ID_A,string Entity_Name,int step)
		{
			ArrayList ProcessPairs = new ArrayList();
			string[] Pair; // It's used to store (substrate product) pair that includes substrate id, substrate name, product id, product name, process id, process name
			int i = 1;
			//VisitNode(Molecular_Entity_ID_A,true); //visited the source node.
			//follow each edge in its edgelist.
			foreach(string[] EachEdge in GetEdgeList(Molecular_Entity_ID_A))
			{															//product_id, product_name,process_id,process_name
				Pair = new String[7] {Molecular_Entity_ID_A,Entity_Name,EachEdge[0],EachEdge[1],EachEdge[2].Substring(1,EachEdge[2].Length - 1),EachEdge[3],Convert.ToString(i)}; 
				ProcessPairs.Add(Pair); // add new pair into pair list.
				VisitNode(EachEdge[0],true); //visit the product
				VisitEdge(EachEdge, Molecular_Entity_ID_A);
			}

			while(i < step) 
			{
				i ++;
				ArrayList temp_pairs = (ArrayList)ProcessPairs.Clone();
				//We continue expand all new molecular entities
				foreach(string[] MyPair in temp_pairs)
				{
					if (MyPair[6] == Convert.ToString(i-1)) //If it's new added, we expand it
					{						
						foreach(string[] EachEdge in GetEdgeList(MyPair[2]))// follow each edge.
						{

							if(!IsEdgeVisited(EachEdge, MyPair[2]))
							{
								Pair = new String[7]{MyPair[2],MyPair[3],EachEdge[0],EachEdge[1],EachEdge[2].Substring(1,EachEdge[2].Length - 1),EachEdge[3],Convert.ToString(i)};
								ProcessPairs.Add(Pair);

								VisitEdge(EachEdge, MyPair[2]);
							}
						}
					}
				}			
			}
			return ProcessPairs;
		}
	}
}
