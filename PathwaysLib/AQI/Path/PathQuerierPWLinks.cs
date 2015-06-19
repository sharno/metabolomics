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
    /// A querier that wraps the functionality for the pathway links graph
    /// </summary>
    public abstract class PathQuerierPWLinks : PathQuerier
    {
        protected PathQuerierPWLinks()
            : base()
        { }

        protected PathQuerierPWLinks(IAQIUtil util)
            : base(util)
        { }

        public override Dictionary<string, string> GetInputValues(QNode node, string fieldId, string nodeType, string fieldTypeId, string inputType, string queryString, string treeXmlString, int topkMatches)
        {
            Dictionary<string, string> matches = new Dictionary<string, string>();

            bool useCommonMolecules = false;
            if(node.Fields.ContainsKey("common_molecules")
               && node.Fields["common_molecules"].Inputs.ContainsKey("common_molecules")
               && node.Fields["common_molecules"].Inputs["common_molecules"].Count > 0)
                useCommonMolecules = node.Fields["common_molecules"].Inputs["common_molecules"][0].Value.Equals("true");

            List<string> orgNames = new List<string>();
            int childPtr = 0;
            while(childPtr < node.Children.Count && node.Children[childPtr].NodeTypeName == "path_graph_restriction_organism")
            {
                if(node.Children[childPtr].Fields["organism_name"].Inputs["organism_name"].Count > 0)
                {
                    string name = node.Children[childPtr].Fields["organism_name"].Inputs["organism_name"][0].Value;
                    if(name.Contains(" [") && name.Contains("]"))
                        name = name.Substring(0, name.LastIndexOf(" ["));
                    orgNames.Add(_util.PreprocessSqlArgValue(name));
                }

                childPtr += 1;
            }

            DataTable dt;
            _util.ExecuteSqlQuery(out dt, String.Format(@"
                WITH pwData AS
                (
                    SELECT DISTINCT pp.pathway_id, pw.name AS pathway_name, pe.entity_id, me.name AS entity_name
                    FROM process_entities pe
                         INNER JOIN pathway_processes pp
                           ON pe.process_id = pp.process_id
                         INNER JOIN process_entity_roles per
                           ON pe.role_id = per.role_id
                         INNER JOIN pathways pw
                           ON pp.pathway_id = pw.id
                         INNER JOIN molecular_entities me
                           ON pe.entity_id = me.id
                         {0}
                    WHERE per.name IN ('substrate', 'product')
                      {1}
                      {2}
                )
                SELECT DISTINCT {3}
                FROM (SELECT * FROM pwData) pw1,
                     (SELECT * FROM pwData) pw2
                WHERE pw1.entity_id = pw2.entity_id
                  AND pw1.pathway_id != pw2.pathway_id
                ORDER BY {4}",
                orgNames.Count > 0 ? "INNER JOIN processes pr ON pe.process_id = pr.id INNER JOIN catalyzes c ON pr.id = c.process_id INNER JOIN organism_groups og ON c.organism_group_id = og.id" : "",
                orgNames.Count > 0 ? String.Format("AND (og.common_name IN ({0}) OR og.scientific_name IN ({0}))", String.Join(", ", orgNames.ToArray())) : "",
                useCommonMolecules ? "" : "AND pe.entity_id NOT IN (SELECT id FROM common_molecules)",
                nodeType.EndsWith("pathway") ? "pw1.pathway_name AS name" : "pw1.entity_name AS name",
                nodeType.EndsWith("pathway") ? "pw1.pathway_name" : "pw1.entity_name"));

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