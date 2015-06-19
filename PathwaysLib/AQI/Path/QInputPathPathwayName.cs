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
    public class QInputPathPathwayName : QInputAutoComplete
    {
        public QInputPathPathwayName()
            : base("pathway_name", new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathPathwayName(string name)
            : base(name, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        public QInputPathPathwayName(string name, string defaultValue)
            : base(name, defaultValue, new QInputAutoCompleteRenderer(PathwaysAQIUtil.Instance))
        { }

        protected QInputPathPathwayName(QInputPathPathwayName input)
            : base(input)
        { }

        public override Dictionary<string, string> GetValues(string currentValue, IAQIUtil util)
        {
            if(Parent.Parent.NodeTypeName == "path_graph_restriction_pathway" ||
                Parent.Parent.NodeTypeName == "path_from_molecule" ||
                Parent.Parent.NodeTypeName == "path_to_molecule" ||
                Parent.Parent.NodeTypeName == "path_from_process" ||
                Parent.Parent.NodeTypeName == "path_to_process" ||
                Parent.Parent.NodeTypeName == "path_from_pathway" ||
                Parent.Parent.NodeTypeName == "path_to_pathway" ||
                Parent.Parent.NodeTypeName == "path_restriction_including_process" ||
                Parent.Parent.NodeTypeName == "path_restriction_including_molecule")
            {
                Dictionary<string, string> retVal = new Dictionary<string, string>();
                string moleculeName = "";
                string processName = "";
                List<string> organismNames = new List<string>();
                List<string> pathwayNames = new List<string>();

                if (Parent.Parent.NodeTypeName != "path_graph_restriction_pathway")
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

                if (Parent.Parent.NodeTypeName == "path_from_molecule" ||
                    Parent.Parent.NodeTypeName == "path_to_molecule" ||
                    Parent.Parent.NodeTypeName == "path_from_process" ||
                    Parent.Parent.NodeTypeName == "path_to_process")
                {
                    // used in combination with a molecule name field to uniquely identify molecule in metabolic network graph (need molecule + pathway pair!)

                    // see if pathway is set in graph restriction -- this will limit our options to 1 pathway!
                    //List<QNode> pwRes = Parent.Parent.Parent.GetChildren("path_graph_restriction_pathway");
                    //if (pwRes.Count > 0)
                    //{
                    //    foreach (QNode pw in pwRes)
                    //    {
                    //        if (pw.Fields["pathway_name"].Inputs["pathway_name"].Count > 0)
                    //        {
                    //            string pathwayName = pw.Fields["pathway_name"].Inputs["pathway_name"][0].Value;  //util.PreprocessSqlArgValue(pwRes.Fields["pathway_name"].Inputs["pathway_name"][0].Value);

                    //            //BE: assuming value is safe since it is only being returned to the user and not used in SQL statement
                    //            retVal.Add(pathwayName, pathwayName); // forcing this as the only option
                    //        }
                    //    }

                    //    if (retVal.Count > 0)
                    //        return retVal;
                    //}


                    // see if molecule is set
                    if (Parent.Parent.Fields.ContainsKey("molecule_name") &&
                        Parent.Parent.Fields["molecule_name"].Inputs["molecule_name"].Count > 0)
                    {
                        moleculeName = util.PreprocessSqlArgValue(Parent.Parent.Fields["molecule_name"].Inputs["molecule_name"][0].Value);
                    }

                    // see if process is set
                    if (Parent.Parent.Fields.ContainsKey("process_name") &&
                        Parent.Parent.Fields["process_name"].Inputs["process_name"].Count > 0)
                    {
                        processName = util.PreprocessSqlArgValue(Parent.Parent.Fields["process_name"].Inputs["process_name"][0].Value);
                    }

                }

                // see if organism is set
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

                DBWrapper db = DBWrapper.Instance;
                DataSet ds;
                int resultCount = db.ExecuteQuery(out ds, String.Format(@"
                SELECT DISTINCT pw.name
                FROM process_entities pe
                     INNER JOIN processes pr
                       ON pe.process_id = pr.id
                     INNER JOIN process_entity_roles per
                       ON pe.role_id = per.role_id
                     INNER JOIN pathway_processes pp
                       ON pr.id = pp.process_id
                     INNER JOIN pathways pw
                       ON pp.pathway_id = pw.id
                    {2} {4}
                WHERE per.name IN ('substrate', 'product') 
                    {0} {1} {3} {5}", 
                            processName != "" ? " AND pr.name = " + processName : "",
                            moleculeName != "" ? " AND me.name = " + moleculeName : "",
                            moleculeName != "" ? "INNER JOIN molecular_entities me ON pe.entity_id = me.id" : "",
                        organismNames.Count > 0 ? " AND ("+ Utilities.Util.StringListMerge(organismNames,"og.scientific_name = ", "", " OR ")  +")" : "",
                   organismNames.Count > 0 ? @"INNER JOIN catalyzes c 
                       ON c.process_id = pr.id
                     INNER JOIN organism_groups og
                       ON og.id = c.organism_group_id" : "",
                        pathwayNames.Count > 0 ? " AND (" + Utilities.Util.StringListMerge(pathwayNames, "pw.name = ", "", " OR ") + ")" : ""
                                                      ));
                DataTable dt = ds.Tables[0];

                foreach (DataRow r in dt.Rows)
                {
                    if (((string)r["name"]).IndexOf(currentValue, StringComparison.CurrentCultureIgnoreCase) > -1)
                        retVal.Add((string)r["name"], (string)r["name"]);
                }

                return retVal;
            }
            else
            {
                return null;
            }
        }

        public override object Clone()
        {
            return new QInputPathPathwayName(this);
        }
    }
}