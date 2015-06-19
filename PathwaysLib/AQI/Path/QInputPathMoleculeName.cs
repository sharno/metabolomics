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
    public class QInputPathMoleculeName : QInputAutoComplete
    {
        public QInputPathMoleculeName()
            : base("molecule_name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathMoleculeName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathMoleculeName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathMoleculeName(QInputPathMoleculeName input)
            : base(input)
        { }

        public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        {
            if (Parent.Parent.NodeTypeName == "path_graph_restriction_molecule" ||
                Parent.Parent.NodeTypeName == "path_from_molecule" || 
                Parent.Parent.NodeTypeName == "path_to_molecule" || 
                Parent.Parent.NodeTypeName == "path_restriction_including_molecule")
            {
                List<string> pathwayNames = new List<string>();
                List<string> organismNames = new List<string>();

                // check for pathway restriction
                //QNode pwRes = Parent.Parent.Parent.GetChild("path_graph_restriction_pathway");
                //if (pwRes != null && pwRes.Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                //{
                //    // see if pathway is set in graph restriction -- this will limit our options to 1 pathway!
                //    pathwayName = util.PreprocessSqlArgValue(pwRes.Fields["pathway_name"].Inputs["pathway_name"][0].Value);
                //}
                //else if (Parent.Parent.Fields.ContainsKey("pathway_name") &&
                //    Parent.Parent.Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                //{
                //    // see if pathway is set explicitly in this node
                //    pathwayName = util.PreprocessSqlArgValue(Parent.Parent.Fields["pathway_name"].Inputs["pathway_name"][0].Value);
                //}
                if (Parent.Parent.Fields.ContainsKey("pathway_name") &&
                    Parent.Parent.Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                {
                    // see if pathway is set explicitly in this node
                    pathwayNames.Add(util.PreprocessSqlArgValue(Parent.Parent.Fields["pathway_name"].Inputs["pathway_name"][0].Value));
                }
                else
                {
                    List<QNode> pwRes = Parent.Parent.Root.GetChildren("path_graph_restriction_pathway");
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
                }

                // check for organism restriction
                //QNode orgRes = Parent.Parent.Parent.GetChild("path_graph_restriction_organism");
                //if (orgRes != null && orgRes.Fields["organism_name"].Inputs["organism_name"].Count > 0)
                //{
                //    // see if pathway is set in graph restriction -- this will limit our options to 1 pathway!
                //    //return "OG{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(dataScientific);

                //    organismName = util.PreprocessSqlArgValue(QNodeOrganism.GetScienticName(orgRes.Fields["organism_name"].Inputs["organism_name"][0].Value));
                //}
                List<QNode> orgRes = Parent.Parent.Root.GetChildren("path_graph_restriction_organism");
                if (orgRes.Count > 0)
                {
                    foreach (QNode org in orgRes)
                    {
                        if (org.Fields["organism_name"].Inputs["organism_name"].Count > 0)
                        {
                            // see if pathway is set in graph restriction -- this will limit our options to 1 pathway!
                            organismNames.Add(util.PreprocessSqlArgValue(QNodeOrganism.GetScienticName(org.Fields["organism_name"].Inputs["organism_name"][0].Value)));
                        }
                    }
                }

                // check if we should restrict to molecules in pathway links table
                bool pathwayLinksEntitiesOnly = false;
                if (Parent.Parent.Root.NodeTypeName == "path_neighborhood_pwlinks" ||
                    Parent.Parent.Root.NodeTypeName == "path_pwlinks")
                {
                    pathwayLinksEntitiesOnly = true;
                }

                Dictionary<string, string> retVal = new Dictionary<string, string>();

                DBWrapper db = DBWrapper.Instance;
                DataSet ds;
                int resultCount = db.ExecuteQuery(out ds, String.Format(@"
                SELECT DISTINCT me.name
                FROM process_entities pe
                     INNER JOIN processes pr
                       ON pe.process_id = pr.id
                     INNER JOIN process_entity_roles per
                       ON pe.role_id = per.role_id
                     INNER JOIN molecular_entities me
                       ON pe.entity_id = me.id
                    {1} {3} {4}
                WHERE per.name IN ('substrate', 'product')
                    {0} {2}",
                        pathwayNames.Count > 0 ? " AND (" + Utilities.Util.StringListMerge(pathwayNames, "pw.name = ", "", " OR ") + ")" : "",
                   @"INNER JOIN pathway_processes pp
                       ON pr.id = pp.process_id
                     INNER JOIN pathways pw
                       ON pp.pathway_id = pw.id", //BE: ensure in a pathway
                        organismNames.Count > 0 ? " AND (" + Utilities.Util.StringListMerge(organismNames, "og.scientific_name = ", "", " OR ") + ")" : "",
                   organismNames.Count > 0 ? @"INNER JOIN catalyzes c 
                       ON c.process_id = pr.id
                     INNER JOIN organism_groups og
                       ON og.id = c.organism_group_id" : "",
                   pathwayLinksEntitiesOnly ? @"INNER JOIN pathway_links pl ON me.id = pl.entity_id" : ""));
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
            return new QInputPathMoleculeName(this);
        }
    }
}