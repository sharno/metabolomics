using System;
using System.Collections.Generic;
using System.Text;
using PathwaysLib.ServerObjects;
using System.IO;
using System.Data;

namespace PathwaysLib.Utilities
{
    public class MetabolicNetworkLoader
    {
        public static void LoadGraph()
        {
            List<string> pathwaySqlRestrictions = new List<string>();
            List<string> organismSqlRestrictions = new List<string>();

            /*
            for (int i = 0; i < pathwayRestrictionsIds.Count; i++)
                pathwaySqlRestrictions.Add(pathwayRestrictionsIds[i].ToString());
            for (int i = 0; i < organismRestrictionsIds.Count; i++)
                organismSqlRestrictions.Add(organismRestrictionsIds[i].ToString());
            */
            DBWrapper db = DBWrapper.Instance;
            DataSet ds;
            int resultCount = db.ExecuteQuery(out ds, String.Format(@"
                SELECT DISTINCT pr.generic_process_id, pr.name AS Process_Name, pe.entity_id, me.name AS Molecule_Name, pr.reversible, per.name AS Role_Name
                FROM process_entities pe
                     INNER JOIN processes pr
                       ON pe.process_id = pr.id
                     INNER JOIN process_entity_roles per
                       ON pe.role_id = per.role_id
                     INNER JOIN molecular_entities me
                       ON pe.entity_id = me.id
                     {0}
                     {1}
                WHERE per.name IN ('substrate', 'product')
                  {2}
                  {3}
                  {4}",
                //pathwayRestrictionsIds.Count > 0 ? "INNER JOIN pathway_processes pp ON pr.id = pp.process_id INNER JOIN pathways pw ON pp.pathway_id = pw.id" : "",
                //organismRestrictionsIds.Count > 0 ? "INNER JOIN catalyzes c ON pr.id = c.process_id INNER JOIN organism_groups og ON c.organism_group_id = og.id" : "",
                //pathwayRestrictionsIds.Count > 0 ? String.Format("AND pw.id IN ('{0}')", String.Join("', '", pathwaySqlRestrictions.ToArray())) : "",
                //organismRestrictionsIds.Count > 0 ? String.Format("AND og.id IN ('{0}')", String.Join("', '", organismSqlRestrictions.ToArray())) : "",
                //ignoreCommonMolecules ? "AND pe.entity_id NOT IN (SELECT id FROM common_molecules)" : ""));
                "", "", "", "", ""));
            DataTable dt = ds.Tables[0];

            //graph = new GraphAdjacencyList();
            
            if (resultCount > 0)
            {
               /*
                Dictionary<NodeType, Dictionary<Guid, INode>> nodes = new Dictionary<NodeType, Dictionary<Guid, INode>>();
                nodes[NodeType.Edge] = new Dictionary<Guid, INode>();
                nodes[NodeType.Node] = new Dictionary<Guid, INode>();
                */
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> processEntities = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                Dictionary<string, Dictionary<string, string>> processMolecules;
                Dictionary<string, string> subprods;

                Dictionary<string, string> molecules = new Dictionary<string, string>();
                Dictionary<string, Boolean> isReversible = new Dictionary<string,bool>();

                foreach (DataRow r in dt.Rows)
                {
                    // Parse the data out of the table for this row
                    Guid processId = (Guid)r["generic_process_id"];
                    string processName = (string)r["Process_Name"];
                    Guid moleculeId = (Guid)r["entity_id"];
                    string moleculeName = (string)r["Molecule_Name"];
                    bool reversible = (bool)r["reversible"];
                    string role = ((string)r["Role_Name"]).ToLower();

                    if(!molecules.ContainsKey(moleculeId.ToString()))
                        molecules.Add(moleculeId.ToString(), moleculeName);

                    if (!isReversible.ContainsKey(processId.ToString()))
                        isReversible.Add(processId.ToString(), reversible);


                    if (processEntities.TryGetValue(processId.ToString(), out processMolecules))
                    {
                        if (!processMolecules.TryGetValue(role, out subprods))
                        {
                            subprods = new Dictionary<string, string>();
                            processMolecules.Add(role, subprods);
                        }
                        if (!subprods.ContainsKey(moleculeId.ToString()))
                            subprods.Add(moleculeId.ToString(), moleculeName);
                    }
                    else
                    {
                        processMolecules = new Dictionary<string,Dictionary<string,string>>();
                        processEntities.Add(processId.ToString(), processMolecules);
                        subprods = new Dictionary<string, string>();
                        processMolecules.Add(role, subprods);
                        subprods.Add(moleculeId.ToString(), moleculeName);
                    }

                    /*
                    // Create the process node and add it to the graph (if necessary)
                    INode processNode;
                    if (!nodes[NodeType.Edge].ContainsKey(processId))
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
                    if (!nodes[NodeType.Node].ContainsKey(moleculeId))
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
                    if (role.Equals("product"))
                    {
                        graph.AddEdge(new Edge(processNode, moleculeNode, "p"));
                        if (reversible)
                            graph.AddEdge(new Edge(moleculeNode, processNode, "p"));
                    }
                    else if (role.Equals("substrate"))
                    {
                        graph.AddEdge(new Edge(moleculeNode, processNode, "s"));
                        if (reversible)
                            graph.AddEdge(new Edge(processNode, moleculeNode, "s"));
                    }
                     */
                }

                //Dictionary<string, Dictionary<string, List<string>>> processEntities;
                //Dictionary<string, Dictionary<string, string>> processMolecules;
                //Dictionary<string, string> subprods;
                Dictionary<string, string> subs;
                Dictionary<string, string> prods;
                Dictionary<string, string> edges = new Dictionary<string, string>();

                foreach (string pid in processEntities.Keys)
                {
                    processMolecules = processEntities[pid];
                    if (!processMolecules.TryGetValue("substrate", out subs))
                        continue;
                    if (!processMolecules.TryGetValue("product", out prods))
                        continue;
                    //subs = processMolecules["substrate"];
                    //prods = processMolecules["product"];
                    foreach (string source in subs.Keys)
                        foreach (string destination in prods.Keys)
                            if (!source.Equals(destination) && !edges.ContainsKey(source + "#" + destination))
                                edges.Add(source + "#" + destination, source + " " + destination);

                    if (isReversible[pid])
                    {
                        foreach (string source in prods.Keys)
                            foreach (string destination in subs.Keys)
                                if (!source.Equals(destination) && !edges.ContainsKey(source + "#" + destination))
                                    edges.Add(source + "#" + destination, source + " " + destination);
                    }

                }
                Console.WriteLine(molecules.Count + " nodes and " + edges.Count + " edges");
                StreamWriter writer = new StreamWriter("..\\..\\MetabolicNetwork.txt");
                foreach (string id in molecules.Keys)
                    writer.WriteLine("v " + id + " " + molecules[id]);
                foreach (string edge in edges.Values)
                    writer.WriteLine("e " + edge);
                writer.Close();
                Console.Read();
                if (!DBWrapper.IsInstanceNull)
                {
                    DBWrapper.Instance.Close();
                    DBWrapper.Instance = null;
                }
            }
        }
        
    }
}
