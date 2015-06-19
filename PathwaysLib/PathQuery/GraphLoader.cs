using PathQueryLib;
using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;

namespace PathwaysLib.PathQuery
{
    /// <summary>
    /// A helper class that loads the graphs, both Metabolic Network and Pathway Links, into memory
    /// </summary>
    public class GraphLoader
    {
        private static GraphLoader _instance = null;

        /// <summary>
        /// The single instance of this loader.
        /// </summary>
        public static GraphLoader Instance
        {
            get { return _instance == null ? _instance = new GraphLoader() : _instance; }
        }

        /// <summary>
        /// This constructor should only be called once, via the Instance property.
        /// </summary>
        private GraphLoader()
        { }

        /// <summary>
        /// Load the metabolic network graph. This call does not force the graph to reload and will not load the graph again if it is found in the store. Also, it will ignore common molecules and contains no pathway or organism restrictions.
        /// </summary>
        public void LoadMetabolicNetworkGraph(IManager manager, out string graphName)
        {
            LoadMetabolicNetworkGraph(manager, false, true, new List<Guid>(), new List<Guid>(), out graphName);
        }

        /// <summary>
        /// Load the metabolic network graph. This call has a flag which could force the graph to reload, even if it is found in the store.
        /// </summary>
        /// <param name="forceReload">If true, the graph will reload no matter what. If false, the graph will not be loaded again if it is found in the store.</param>
        public void LoadMetabolicNetworkGraph(IManager manager, bool forceReload, bool ignoreCommonMolecules, List<Guid> pathwayRestrictionsIds, /*List<string> pathwayRestrictionsNames,*/ List<Guid> organismRestrictionsIds, /*List<string> organismRestrictionsNames,*/ out string graphName)
        {
            graphName = MetabolicNetworkGraphName(ignoreCommonMolecules, pathwayRestrictionsIds, organismRestrictionsIds);

            if(!forceReload && manager.ContainsGraph(graphName))
                return;

            IGraph graph;
            LoadMetabolicNetworkGraph(ignoreCommonMolecules, pathwayRestrictionsIds, organismRestrictionsIds, out graph);

            manager.RegisterGraph(graphName, graph);

            return;
        }

        /// <summary>
        /// Give the name of a metabolic network graph based on its attributes.
        /// </summary>
        /// <param name="ignoreCommonMolecules"></param>
        /// <param name="pathwayRestrictionsIds"></param>
        /// <param name="pathwayRestrictionsNames"></param>
        /// <param name="organismRestrictionsIds"></param>
        /// <param name="organismRestrictionsNames"></param>
        /// <returns></returns>
        public string MetabolicNetworkGraphName(bool ignoreCommonMolecules, List<Guid> pathwayRestrictionsIds, /*List<string> pathwayRestrictionsNames,*/ List<Guid> organismRestrictionsIds /*, List<string> organismRestrictionsNames*/)
        {
            List<string> restrictionNames = new List<string>();
            
            // err chk list counts btwn id and names
            //if(pathwayRestrictionsIds.Count != pathwayRestrictionsNames.Count)
            //    throw new ArgumentOutOfRangeException("pathwayRestrictionsNames", "Pathway Restriction ID list and Name list are not the same length.");
            //if(organismRestrictionsIds.Count != organismRestrictionsNames.Count)
            //    throw new ArgumentOutOfRangeException("organismRestrictionNames", "Organism Restriction ID list and Name list are not the same length.");

            for (int i = 0; i < pathwayRestrictionsIds.Count; i++)
                restrictionNames.Add(String.Format("pw_{0}", pathwayRestrictionsIds[i].ToString())); //, pathwayRestrictionsNames[i]));
            for(int i = 0; i < organismRestrictionsIds.Count; i++)
                restrictionNames.Add(String.Format("org_{0}", organismRestrictionsIds[i].ToString())); //, organismRestrictionsNames[i]));

            restrictionNames.Sort();

            if(ConfigurationManager.AppSettings.Get("AQIGraphName") == null)
                throw new Exception("Graph Name Config Setting Not Set");

            return String.Format("PathCase_{0}_Metabolic_Network_Graph_{1}_{2}_AdjList", ConfigurationManager.AppSettings.Get("AQIGraphName"), String.Join("", restrictionNames.ToArray()), ignoreCommonMolecules ? "WithoutCommonMolecules" : "WithCommonMolecules");
        }

        /// <summary>
        /// Load the metabolic network graph. This call has a flag which could force the graph to reload, even if it is found in the store.
        /// </summary>
        /// <param name="forceReload">If true, the graph will reload no matter what. If false, the graph will not be loaded again if it is found in the store.</param>
        public void LoadMetabolicNetworkGraph(bool ignoreCommonMolecules, List<Guid> pathwayRestrictionsIds, List<Guid> organismRestrictionsIds, out IGraph graph)
        {
            List<string> pathwaySqlRestrictions = new List<string>();
            List<string> organismSqlRestrictions = new List<string>();

            for(int i = 0; i < pathwayRestrictionsIds.Count; i++)
                pathwaySqlRestrictions.Add(pathwayRestrictionsIds[i].ToString());
            for(int i = 0; i < organismRestrictionsIds.Count; i++)
                organismSqlRestrictions.Add(organismRestrictionsIds[i].ToString());

            DBWrapper db = DBWrapper.Instance;
            DataSet ds;
            #region Query on old graph structure (one node for each molecule/process
//            int resultCount = db.ExecuteQuery(out ds, String.Format(@"
//                SELECT DISTINCT pr.generic_process_id, pr.name AS Process_Name, pe.entity_id, me.name AS Molecule_Name, pr.reversible, per.name AS Role_Name
//                FROM process_entities pe
//                     INNER JOIN processes pr
//                       ON pe.process_id = pr.id
//                     INNER JOIN process_entity_roles per
//                       ON pe.role_id = per.role_id
//                     INNER JOIN molecular_entities me
//                       ON pe.entity_id = me.id
//                     {0}
//                     {1}
//                WHERE per.name IN ('substrate', 'product')
//                  {2}
//                  {3}
//                  {4}",
//                pathwayRestrictionsIds.Count > 0 ? "INNER JOIN pathway_processes pp ON pr.id = pp.process_id INNER JOIN pathways pw ON pp.pathway_id = pw.id" : "",
//                organismRestrictionsIds.Count > 0 ? "INNER JOIN catalyzes c ON pr.id = c.process_id INNER JOIN organism_groups og ON c.organism_group_id = og.id" : "",
//                pathwayRestrictionsIds.Count > 0 ? String.Format("AND pw.id IN ('{0}')", String.Join("', '", pathwaySqlRestrictions.ToArray())) : "",
//                organismRestrictionsIds.Count > 0 ? String.Format("AND og.id IN ('{0}')", String.Join("', '", organismSqlRestrictions.ToArray())) : "",
            //                ignoreCommonMolecules ? "AND pe.entity_id NOT IN (SELECT id FROM common_molecules)" : ""));
            #endregion
            int resultCount = db.ExecuteQuery(out ds, String.Format(@"
                SELECT DISTINCT pg.graphNodeId AS process_node_id, pr.name AS Process_Name, eg.graphNodeId AS entity_node_id, me.name AS Molecule_Name, pr.reversible, per.name AS Role_Name
                FROM process_entities pe
                     INNER JOIN processes pr ON pe.process_id = pr.id
                     INNER JOIN process_entity_roles per ON pe.role_id = per.role_id
                     INNER JOIN molecular_entities me ON pe.entity_id = me.id
	                 LEFT OUTER JOIN pathway_processes pp ON pr.id = pp.process_id
	                 INNER JOIN entity_graph_nodes eg ON me.id = eg.entityId AND pp.pathway_id = eg.pathwayId
                     INNER JOIN process_graph_nodes pg ON pr.generic_process_id = pg.genericProcessId AND pp.pathway_id = eg.pathwayId
                     {0}
                WHERE per.name IN ('substrate', 'product')
                  {1}
                  {2}
                  {3}",
                organismRestrictionsIds.Count > 0 ? "INNER JOIN catalyzes c ON pr.id = c.process_id INNER JOIN organism_groups og ON c.organism_group_id = og.id" : "",
                pathwayRestrictionsIds.Count > 0 ? String.Format("AND pp.pathway_id IN ('{0}')", String.Join("', '", pathwaySqlRestrictions.ToArray())) : "",
                organismRestrictionsIds.Count > 0 ? String.Format("AND og.id IN ('{0}')", String.Join("', '", organismSqlRestrictions.ToArray())) : "",
                ignoreCommonMolecules ? "AND pe.entity_id NOT IN (SELECT id FROM common_molecules)" : ""));


            DataTable dt = ds.Tables[0];

            graph = new GraphAdjacencyList();

            if(resultCount > 0)
            {
                Dictionary<NodeType, Dictionary<Guid, INode>> nodes = new Dictionary<NodeType, Dictionary<Guid, INode>>();
                nodes[NodeType.Edge] = new Dictionary<Guid, INode>();
                nodes[NodeType.Node] = new Dictionary<Guid, INode>();

                foreach(DataRow r in dt.Rows)
                {
                    // Parse the data out of the table for this row
                    Guid processId = (Guid)r["process_node_id"];
                    string processName = (string) r["Process_Name"];
                    Guid moleculeId = (Guid)r["entity_node_id"];
                    string moleculeName = (string) r["Molecule_Name"];
                    bool reversible = (bool) r["reversible"];
                    string role = ((string) r["Role_Name"]).ToLower();

                    // Create the process node and add it to the graph (if necessary)
                    INode processNode;
                    if(!nodes[NodeType.Edge].ContainsKey(processId))
                    {
                        processNode = new Node(processId, NodeType.Edge, processName);
                        nodes[NodeType.Edge][processId] = processNode;

                        graph.AddVertex(processNode);
                    }
                    else
                    {
                        processNode = nodes[NodeType.Edge][processId];
                    }

                    // Create the molecule node and add it to the graph (if necessary)
                    INode moleculeNode;
                    if(!nodes[NodeType.Node].ContainsKey(moleculeId))
                    {
                        moleculeNode = new Node(moleculeId, NodeType.Node, moleculeName);
                        nodes[NodeType.Node][moleculeId] = moleculeNode;

                        graph.AddVertex(moleculeNode);
                    }
                    else
                    {
                        moleculeNode = nodes[NodeType.Node][moleculeId];
                    }

                    // Add the edges for the graph
                    // For products, the edge goes from the molecule to the process
                    // For substrates, the edge goes from the process to the molecule

                    // For reversible processes, add edges going in both directions.
                    // Otherwise, add edges going from the process to the molecule for products
                    //   and edges going from the molecule to the process for substrates.
                    if(role.Equals("product"))
                    {
                        graph.AddEdge(new Edge(processNode, moleculeNode, "p"));
                        if(reversible)
                            graph.AddEdge(new Edge(moleculeNode, processNode, "p"));
                    }
                    else if(role.Equals("substrate"))
                    {
                        graph.AddEdge(new Edge(moleculeNode, processNode, "s"));
                        if(reversible)
                            graph.AddEdge(new Edge(processNode, moleculeNode, "s"));
                    }
                }
            }
        }

        /// <summary>
        /// Load the pathway links graph. This call has a flag which could force the graph to reload, even if it is found in the store.
        /// </summary>
        /// <param name="forceReload">If true, the graph will reload no matter what. If false, the graph will not be loaded again if it is found in the store.</param>
        public void LoadPathwayLinksGraph(IManager manager, bool forceReload, bool ignoreCommonMolecules, List<Guid> organismRestrictionsIds, out string graphName)
        {
            graphName = PathwayLinksGraphName(ignoreCommonMolecules, organismRestrictionsIds);

            if(!forceReload && manager.ContainsGraph(graphName))
                return;

            IGraph graph;
            LoadPathwayLinksGraph(ignoreCommonMolecules, organismRestrictionsIds, out graph);

            manager.RegisterGraph(graphName, graph);

            return;
        }

        /// <summary>
        /// Give the name of a pathway links graph based on its attributes.
        /// </summary>
        /// <param name="ignoreCommonMolecules"></param>
        /// <param name="organismRestrictionsIds"></param>
        /// <param name="organismRestrictionsNames"></param>
        /// <returns></returns>
        public string PathwayLinksGraphName(bool ignoreCommonMolecules, List<Guid> organismRestrictionsIds)
        {
            List<string> restrictionNames = new List<string>();
            
            // err chk list counts btwn id and names
            //if(organismRestrictionsIds.Count != organismRestrictionsNames.Count)
            //    throw new ArgumentOutOfRangeException("organismRestrictionNames", "Organism Restriction ID list and Name list are not the same length.");

            for (int i = 0; i < organismRestrictionsIds.Count; i++)
                restrictionNames.Add(String.Format("org_{0}", organismRestrictionsIds[i].ToString())); //, organismRestrictionsNames[i]));

            restrictionNames.Sort();

            if(ConfigurationManager.AppSettings.Get("AQIGraphName") == null)
                throw new Exception("Graph Name Config Setting Not Set");

            return String.Format("PathCase_{0}_Pathway_Links_Graph_{1}_{2}_AdjList", ConfigurationManager.AppSettings.Get("AQIGraphName"), String.Join("", restrictionNames.ToArray()), ignoreCommonMolecules ? "WithoutCommonMolecules" : "WithCommonMolecules");
        }

        /// <summary>
        /// Load the pathway links graph. This call has a flag which could force the graph to reload, even if it is found in the store.
        /// </summary>
        /// <param name="forceReload">If true, the graph will reload no matter what. If false, the graph will not be loaded again if it is found in the store.</param>
        public void LoadPathwayLinksGraph(bool ignoreCommonMolecules, List<Guid> organismRestrictionsIds, out IGraph graph)
        {
            //TODO: update this code to load correct linking entities!

            List<string> organismSqlRestrictions = new List<string>();

            for(int i = 0; i < organismRestrictionsIds.Count; i++)
                organismSqlRestrictions.Add(organismRestrictionsIds[i].ToString());

            DBWrapper db = DBWrapper.Instance;
            DataSet ds;
            //BE: replaced Steve's calculation method with use of pathway_links table (precalculated by Ali)
//            int resultCount = db.ExecuteQuery(out ds, String.Format(@"
//                WITH pwData AS
//                (
//                    SELECT DISTINCT pp.pathway_id, pw.name AS pathway_name, pe.entity_id, me.name AS entity_name, per.name AS entity_role
//                    FROM process_entities pe
//                         INNER JOIN pathway_processes pp
//                           ON pe.process_id = pp.process_id
//                         INNER JOIN process_entity_roles per
//                           ON pe.role_id = per.role_id
//                         INNER JOIN pathways pw
//                           ON pp.pathway_id = pw.id
//                         INNER JOIN molecular_entities me
//                           ON pe.entity_id = me.id
//                         {0}
//                    WHERE per.name IN ('substrate', 'product')
//                      {1}
//                      {2}
//                )
//                SELECT DISTINCT pw1.pathway_id AS pw1_id, pw1.pathway_name AS pw1_name, pw2.pathway_id AS pw2_id, pw2.pathway_name AS pw2_name, pw1.entity_id AS mol_id, pw1.entity_name AS mol_name
//                FROM (SELECT * FROM pwData) pw1,
//                     (SELECT * FROM pwData) pw2
//                WHERE pw1.entity_id = pw2.entity_id
//                  AND pw1.pathway_id != pw2.pathway_id
//                  AND pw1.entity_role = 'product'
//                  AND pw2.entity_role = 'substrate'",
//                organismRestrictionsIds.Count > 0 ? "INNER JOIN processes pr ON pe.process_id = pr.id INNER JOIN catalyzes c ON pr.id = c.process_id INNER JOIN organism_groups og ON c.organism_group_id = og.id" : "",
//                organismRestrictionsIds.Count > 0 ? String.Format("AND og.id IN ('{0}')", String.Join("', '", organismSqlRestrictions.ToArray())) : "",
//                ignoreCommonMolecules ? "AND pe.entity_id NOT IN (SELECT id FROM common_molecules)" : ""));

            //BE: common molecules are no longer in pathway links graph
            int resultCount = db.ExecuteQuery(out ds, String.Format(@"
                    SELECT DISTINCT pl.pathway_id_1 AS pw1_id, p1.name AS pw1_name, pl.pathway_id_2 AS pw2_id, p2.name AS pw2_name, pl.entity_id AS mol_id, me.name AS mol_name
                    FROM pathway_links pl
                        INNER JOIN pathways p1 ON p1.id = pl.pathway_id_1
                        INNER JOIN pathways p2 ON p2.id = pl.pathway_id_2
                        INNER JOIN molecular_entities me ON me.id = pl.entity_id
                        {0}
                    {1}",
                          organismRestrictionsIds.Count > 0 ? 
                          @"INNER JOIN pathway_processes pp1 ON pp1.pathway_id = pl.pathway_id_1
                            INNER JOIN catalyzes c1 ON pp1.process_id = c1.process_id 
                            INNER JOIN organism_groups og1 ON c1.organism_group_id = og1.id                            
                            INNER JOIN pathway_processes pp2 ON pp2.pathway_id = pl.pathway_id_2
                            INNER JOIN catalyzes c2 ON pp2.process_id = c2.process_id 
                            INNER JOIN organism_groups og2 ON c2.organism_group_id = og2.id" : "",
                        organismRestrictionsIds.Count > 0 ? 
                            string.Format("WHERE og1.id IN ('{0}') AND og2.id IN ('{0}')", String.Join("', '", organismSqlRestrictions.ToArray())) : ""
                          ));
            
            DataTable dt = ds.Tables[0];

            graph = new GraphAdjacencyList();

            if(resultCount > 0)
            {
                Dictionary<NodeType, Dictionary<Guid, INode>> nodes = new Dictionary<NodeType, Dictionary<Guid, INode>>();
                nodes[NodeType.Edge] = new Dictionary<Guid, INode>();
                nodes[NodeType.Node] = new Dictionary<Guid, INode>();
                Dictionary<INode, Dictionary<INode, List<INode>>> edges = new Dictionary<INode, Dictionary<INode, List<INode>>>(new EqualityComparerINode());

                foreach(DataRow r in dt.Rows)
                {
                    // Parse the data out of the table for this row
                    Guid pathway1Id = (Guid) r["pw1_id"];
                    string pathway1Name = (string) r["pw1_name"];
                    Guid pathway2Id = (Guid) r["pw2_id"];
                    string pathway2Name = (string) r["pw2_name"];
                    Guid moleculeId = GraphNodeManager.GetEntityGraphNodeId(pathway1Id, (Guid) r["mol_id"]); //BE: since it's a linking entity, the entity graph id will be the same regardless of which pathway we use
                    string moleculeName = (string) r["mol_name"];

                    // Create the molecule node and add it to the graph (if necessary)
                    INode moleculeNode;
                    if(!nodes[NodeType.Edge].ContainsKey(moleculeId))
                    {
                        moleculeNode = new Node(moleculeId, NodeType.Edge, moleculeName);
                        nodes[NodeType.Edge][moleculeId] = moleculeNode;

                        graph.AddVertex(moleculeNode);
                    }
                    else
                    {
                        moleculeNode = nodes[NodeType.Edge][moleculeId];
                    }

                    // Create the pathway 1 node and add it to the graph (if necessary)
                    INode pathway1Node;
                    if(!nodes[NodeType.Node].ContainsKey(pathway1Id))
                    {
                        pathway1Node = new Node(pathway1Id, NodeType.Node, pathway1Name);
                        nodes[NodeType.Node][pathway1Id] = pathway1Node;

                        graph.AddVertex(pathway1Node);
                    }
                    else
                    {
                        pathway1Node = nodes[NodeType.Node][pathway1Id];
                    }

                    // Create the pathway 2 node and add it to the graph (if necessary)
                    INode pathway2Node;
                    if(!nodes[NodeType.Node].ContainsKey(pathway2Id))
                    {
                        pathway2Node = new Node(pathway2Id, NodeType.Node, pathway2Name);
                        nodes[NodeType.Node][pathway2Id] = pathway2Node;

                        graph.AddVertex(pathway2Node);
                    }
                    else
                    {
                        pathway2Node = nodes[NodeType.Node][pathway2Id];
                    }

                    // Add edges going from the first pathway (product mol) to the molecule and from the molecule to the second (substrate mol)
                    if(!edges.ContainsKey(pathway1Node))
                        edges[pathway1Node] = new Dictionary<INode, List<INode>>(new EqualityComparerINode());
                    if(!edges[pathway1Node].ContainsKey(pathway2Node))
                        edges[pathway1Node][pathway2Node] = new List<INode>();
                    if(!edges[pathway1Node][pathway2Node].Contains(moleculeNode))
                    {
                        graph.AddEdge(new Edge(pathway1Node, moleculeNode, "s"));
                        graph.AddEdge(new Edge(moleculeNode, pathway2Node, "p"));

                        edges[pathway1Node][pathway2Node].Add(moleculeNode);
                    }
                }
            }
        }
    }
}