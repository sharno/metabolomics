using AQILib;
using PathQueryLib;
using PathwaysLib.PathQuery;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// A querier that wraps the functionality for the metabolic network graph
    /// </summary>
    public abstract class PathQuerierMetabolicNW : PathQuerier
    {
        protected PathQuerierMetabolicNW()
            : base()
        { }

        protected PathQuerierMetabolicNW(IAQIUtil util)
            : base(util)
        { }

        public override Dictionary<string, string> GetInputValues(QNode node, string fieldId, string nodeType, string fieldTypeId, string inputType, string queryString, string treeXmlString, int topkMatches)
        {
            Dictionary<string, string> matches = new Dictionary<string, string>();

            bool useCommonMolecules = false;
            List<string> pwNames = new List<string>();
            List<string> orgNames = new List<string>();

            int childPtr = 0;

            if(node.Fields.ContainsKey("common_molecules")
               && node.Fields["common_molecules"].Inputs.ContainsKey("common_molecules")
               && node.Fields["common_molecules"].Inputs["common_molecules"].Count > 0)
                useCommonMolecules = node.Fields["common_molecules"].Inputs["common_molecules"][0].Value.Equals("true");

            while(childPtr < node.Children.Count
                  && (node.Children[childPtr].NodeTypeName == "path_graph_restriction_pathway"
                      || node.Children[childPtr].NodeTypeName == "path_graph_restriction_organism"))
            {
                if(node.Children[childPtr].NodeTypeName == "path_graph_restriction_pathway")
                {
                    if(node.Children[childPtr].Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                        pwNames.Add(_util.PreprocessSqlArgValue(node.Children[childPtr].Fields["pathway_name"].Inputs["pathway_name"][0].Value));
                }
                else // path_graph_restriction_organism
                {
                    if(node.Children[childPtr].Fields["organism_name"].Inputs["organism_name"].Count > 0)
                    {
                        string name = node.Children[childPtr].Fields["organism_name"].Inputs["organism_name"][0].Value;
                        if(name.Contains(" [") && name.Contains("]"))
                            name = name.Substring(0, name.LastIndexOf(" ["));
                        orgNames.Add(_util.PreprocessSqlArgValue(name));
                    }
                }

                childPtr += 1;
            }

            DataTable dt;
            _util.ExecuteSqlQuery(out dt, String.Format(@"
                SELECT DISTINCT {5}
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
                  {4}
                ORDER BY {5}",
                pwNames.Count > 0 ? "INNER JOIN pathway_processes pp ON pr.id = pp.process_id INNER JOIN pathways pw ON pp.pathway_id = pw.id" : "",
                orgNames.Count > 0 ? "INNER JOIN catalyzes c ON pr.id = c.process_id INNER JOIN organism_groups og ON c.organism_group_id = og.id" : "",
                pwNames.Count > 0 ? String.Format("AND pw.name IN ({0})", String.Join(", ", pwNames.ToArray())) : "",
                orgNames.Count > 0 ? String.Format("AND (og.common_name IN ({0}) OR og.scientific_name IN ({0}))", String.Join(", ", orgNames.ToArray())) : "",
                useCommonMolecules ? "" : "AND pe.entity_id NOT IN (SELECT id FROM common_molecules)",
                nodeType.EndsWith("molecule") ? "me.name" : "pr.name"));

            if(dt != null)
            {
                foreach(DataRow r in dt.Rows)
                {
                    if(((string) r["name"]).IndexOf(queryString, StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        matches.Add((string) r["name"], (string) r["name"]);

                        if(matches.Count >= topkMatches)
                            break;
                    }
                }
            }

            return matches;
        }
    }
}