using PathwaysLib.ServerObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using AQILib;
using AQILib.Sql;
using AQILib.Gui;

namespace PathwaysLib.AQI
{
    /// <summary>
    /// A query node for organisms
    /// </summary>
    [QNodeType("organism")]
    public class QNodeOrganism : QNode
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private QNodeOrganism() { }

        public QNodeOrganism(QNode parent)
            : base("organism", "Organism", parent, new SqlDatasetQuerier(PathwaysAQIUtil.Instance), new SqlDatasetRenderer(PathwaysAQIUtil.Instance), new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            SqlDatasetQuerier q = (SqlDatasetQuerier) Querier;
            //QNodeBasicRenderer r = (QNodeBasicRenderer) RendererNode;

            q.AddSqlTable("OG", "organism_groups OG{0}");
            q.AddSqlTable("OGgrp", "organism_groups OGgrp{0}");

            q.AddSqlTableCondition("OG", "OG{0}.is_organism = '1'");
            q.AddSqlTableCondition("OGgrp", "OGgrp{0}.is_organism = '0'");

            q.AddSqlJoinCondition("OG", "OGgrp", "OG{0}.nodeLabel LIKE OGgrp{0}.nodeLabel + '%'");

            //r.AddLinkSortOrder = 4;
            //r.AddRelationship("", "Organism", "Search for an organism");

            AddRelationship("pathway", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("pathway", "OG", "organism_groups OG{3}");
            q.AddSqlRelationshipTable("pathway", "C", "catalyzes C{3}_{4}");
            q.AddSqlRelationshipTable("pathway", "PP", "pathway_processes PP{3}_{4}");
            q.AddSqlRelationshipTable("pathway", "PW", "pathways PW{4}");
            q.AddSqlRelationshipJoinCondition("pathway", "OG", "C", "OG{3}.id = C{3}_{4}.organism_group_id");
            q.AddSqlRelationshipJoinCondition("pathway", "C", "PP", "C{3}_{4}.process_id = PP{3}_{4}.process_id");
            q.AddSqlRelationshipJoinCondition("pathway", "PP", "PW", "PP{3}_{4}.pathway_id = PW{4}.id");
            //r.AddRelationship("pathway", "Organism", "An organism in which a process in this pathway takes place");

            AddRelationship("process", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("process", "OG", "organism_groups OG{3}");
            q.AddSqlRelationshipTable("process", "C", "catalyzes C{3}_{4}");
            q.AddSqlRelationshipTable("process", "PR", "processes PR{4}");
            q.AddSqlRelationshipJoinCondition("process", "OG", "C", "OG{3}.id = C{3}_{4}.organism_group_id");
            q.AddSqlRelationshipJoinCondition("process", "C", "PR", "C{3}_{4}.process_id = PR{4}.id");
            //r.AddRelationship("process", "Organism", "An organism in which this process takes place");

            AddRelationship("molecule", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("molecule", "OG", "organism_groups OG{3}");
            q.AddSqlRelationshipTable("molecule", "C", "catalyzes C{3}_{4}");
            q.AddSqlRelationshipTable("molecule", "PE", "process_entities PE{4}");
            q.AddSqlRelationshipTable("molecule", "ME", "molecular_entities ME{4}");
            q.AddSqlRelationshipJoinCondition("molecule", "OG", "C", "OG{3}.id = C{3}_{4}.organism_group_id");
            q.AddSqlRelationshipJoinCondition("molecule", "C", "PE", "C{3}_{4}.process_id = PE{4}.process_id");
            q.AddSqlRelationshipJoinCondition("molecule", "PE", "ME", "PE{4}.entity_id = ME{4}.id");
            //r.AddRelationship("molecule", "Organism", "An organism in which a process that involves this molecular entity takes place");

            AddField("name", "Name", true,
                     new QInput[] { new QInputOrganismName() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("name", "isnull(OG{0}.scientific_name + isnull(nullif(' [' + OG{0}.common_name + ']', ' []'), ''), OG{0}.common_name) AS [Organism {1} Name], OG{0}.id AS [IDorg{1}_{0}]",
                                new string[] { "OG" });
            q.AddSqlFieldCondition("name",
                                   delegate(Dictionary<string, string> data)
                                   {
                                       if(data["name"].Contains(" [") && data["name"].Contains("]"))
                                       {
                                           string dataScientific = data["name"].Substring(0, data["name"].LastIndexOf(" ["));
                                           //string dataCommon = data["name"].Substring(data["name"].LastIndexOf(" [")).TrimStart(' ', '[').TrimEnd(']');
                                           //return "(OG{0}.common_name = " + DBWrapper.PreprocessSqlArgValue(dataCommon) + " OR OG{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(dataScientific) + ")";
                                           return "OG{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(dataScientific);
                                       }
                                       else
                                       {
                                           return "(OG{0}.common_name = " + DBWrapper.PreprocessSqlArgValue(data["name"]) + " OR OG{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(data["name"]) + ")";
                                       }
                                   },
                                   new string[] { "OG" });

            AddField("group", "In Organism Group", false,
                     new QInput[] { new QInputOrganismInGroup() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("group", "isnull(OGgrp{0}.scientific_name + isnull(nullif(' [' + OGgrp{0}.common_name + ']', ' []'), ''), OGgrp{0}.common_name) AS [Organism {1} Group], OGgrp{0}.id AS [IDogp{1}_{0}]",
                                new string[] { "OGgrp", "OG" });
            q.AddSqlFieldCondition("group",
                                   delegate(Dictionary<string, string> data)
                                   {
                                       if(data["group"].Contains(" [") && data["group"].Contains("]"))
                                       {
                                           string dataScientific = data["group"].Substring(0, data["group"].LastIndexOf(" ["));
                                           //string dataCommon = data["group"].Substring(data["group"].LastIndexOf(" [")).TrimStart(' ', '[').TrimEnd(']');
                                           //return "(OGgrp{0}.common_name = " + DBWrapper.PreprocessSqlArgValue(dataCommon) + " OR OGgrp{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(dataScientific) + ")";
                                           return "OGgrp{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(dataScientific);
                                       }
                                       else
                                       {
                                           return "(OGgrp{0}.common_name = " + DBWrapper.PreprocessSqlArgValue(data["group"]) + " OR OGgrp{0}.scientific_name = " + DBWrapper.PreprocessSqlArgValue(data["group"]) + ")";
                                       }
                                   },
                                   new string[] { "OGgrp", "OG" });

            AddLink("AddHas", 0, "group", "Has",
                    new string[] { "pathway", "process", "molecule" },
                    new string[] { "Pathway", "Process", "Molecular Entity" },
                    new string[] { "A pathway that contains a process that takes place in this organism", "A process that takes place in this organism", "A molecular entity involved in a process that takes place in this organism" },
                    new string[] { "a ", ", a ", ", or a ", "" });
        }
    }
}