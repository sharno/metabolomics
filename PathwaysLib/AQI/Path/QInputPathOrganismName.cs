using AQILib;
using AQILib.Gui;
using PathQueryLib;
using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    public class QInputPathOrganismName : QInputAutoComplete
    {
        public QInputPathOrganismName()
            : base("organism_name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathOrganismName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathOrganismName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathOrganismName(QInputPathOrganismName input)
            : base(input)
        { }

        public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        {
            if(Parent.Parent.NodeTypeName == "path_graph_restriction_organism")
            {
                List<string> pathwayNames = new List<string>();

                // find currently selected pathway restrictions
                List<QNode> pwRes = Parent.Parent.Parent.GetChildren("path_graph_restriction_pathway");
                if (pwRes.Count > 0)
                {
                    foreach (QNode pw in pwRes)
                    {
                        if (pw.Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                        {
                            // see if pathway is set in graph restriction -- this will limit our options to those pathway(s)
                            pathwayNames.Add(util.PreprocessSqlArgValue(pw.Fields["pathway_name"].Inputs["pathway_name"][0].Value));
                        }
                    }
                }

                // from a pathway entity
                pwRes = Parent.Parent.Parent.GetChildren("path_from_pathway");
                if (pwRes.Count > 0)
                {
                    foreach (QNode pw in pwRes)
                    {
                        if (pw.Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                        {
                            // see if pathway is set in graph restriction -- this will limit our options to those pathway(s)
                            pathwayNames.Add(util.PreprocessSqlArgValue(pw.Fields["pathway_name"].Inputs["pathway_name"][0].Value));
                        }
                    }
                }

                // check if we should test if the org is in a pathway
                bool orgsWithPathwayOnly = false;
                if (Parent.Parent.Parent.NodeTypeName == "path_neighborhood_pwlinks" || 
                    Parent.Parent.Parent.NodeTypeName == "path_pwlinks")
                {
                    orgsWithPathwayOnly = true;
                }

                Dictionary<string, string> retVal = new Dictionary<string, string>();

                DBWrapper db = DBWrapper.Instance;
                DataSet ds;
                int resultCount = db.ExecuteQuery(out ds, String.Format(@"
                SELECT DISTINCT isnull(og.scientific_name + isnull(nullif(' [' + og.common_name + ']', ' []'), ''), og.common_name) AS [name]
                FROM process_entities pe
                     INNER JOIN processes pr
                       ON pe.process_id = pr.id
                     INNER JOIN process_entity_roles per
                       ON pe.role_id = per.role_id
                     INNER JOIN catalyzes c
                       ON pr.id = c.process_id
                     INNER JOIN organism_groups og
                       ON c.organism_group_id = og.id
                    {1}
                WHERE per.name IN ('substrate', 'product')
                     {0}",
                        pathwayNames.Count > 0 ? " AND (" + Utilities.Util.StringListMerge(pathwayNames, "pw.name = ", "", " OR ") + ")" : "",
                        pathwayNames.Count > 0 || orgsWithPathwayOnly ? @"INNER JOIN pathway_processes pp
                       ON pr.id = pp.process_id
                     INNER JOIN pathways pw
                       ON pp.pathway_id = pw.id" : ""));
                DataTable dt = ds.Tables[0];

                foreach(DataRow r in dt.Rows)
                    if(((string) r["name"]).IndexOf(currentValue, StringComparison.CurrentCultureIgnoreCase) > -1)
                        retVal.Add((string) r["name"], (string) r["name"]);

                return retVal;
            }
            else
            {
                return null;
            }
        }

        public override object Clone()
        {
            return new QInputPathOrganismName(this);
        }
    }
}