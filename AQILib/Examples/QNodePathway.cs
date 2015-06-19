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
    /// A query node for pathways
    /// </summary>
    [QNodeType("pathway")]
    public class QNodePathway : QNode
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private QNodePathway() { }

        public QNodePathway(QNode parent)
            : base("pathway", "Pathway", parent, new SqlDatasetQuerier(PathwaysAQIUtil.Instance), new SqlDatasetRenderer(PathwaysAQIUtil.Instance), new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            SqlDatasetQuerier q = (SqlDatasetQuerier) Querier;
            //QNodeBasicRenderer r = (QNodeBasicRenderer) RendererNode;

            q.AddSqlTable("PW", "pathways PW{0}");

            //r.AddLinkSortOrder = 1;
            //r.AddRelationship("", "Pathway", "Search for a pathway");

            AddRelationship("process", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("process", "PW", "pathways PW{3}");
            q.AddSqlRelationshipTable("process", "PP", "pathway_processes PP{3}_{4}");
            q.AddSqlRelationshipTable("process", "PR", "processes PR{4}");
            q.AddSqlRelationshipJoinCondition("process", "PW", "PP", "PW{3}.id = PP{3}_{4}.pathway_id");
            q.AddSqlRelationshipJoinCondition("process", "PP", "PR", "PP{3}_{4}.process_id = PR{4}.id");
            //r.AddRelationship("process", "Pathway", "A pathway in which this process takes place");

            AddRelationship("molecule", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("molecule", "PW", "pathways PW{3}");
            q.AddSqlRelationshipTable("molecule", "PP", "pathway_processes PP{3}_{4}");
            q.AddSqlRelationshipTable("molecule", "PE", "process_entities PE{4}");
            q.AddSqlRelationshipTable("molecule", "ME", "molecular_entities ME{4}");
            q.AddSqlRelationshipJoinCondition("molecule", "PW", "PP", "PW{3}.id = PP{3}_{4}.pathway_id");
            q.AddSqlRelationshipJoinCondition("molecule", "PP", "PE", "PP{3}_{4}.process_id = PE{4}.process_id");
            q.AddSqlRelationshipJoinCondition("molecule", "PE", "ME", "PE{4}.entity_id = ME{4}.id");
            //r.AddRelationship("molecule", "Pathway", "A pathway that contains a process that involves this molecular entity");

            AddRelationship("organism", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("organism", "PW", "pathways PW{3}");
            q.AddSqlRelationshipTable("organism", "PP", "pathway_processes PP{3}_{4}");
            q.AddSqlRelationshipTable("organism", "C", "catalyzes C{3}_{4}");
            q.AddSqlRelationshipTable("organism", "OG", "organism_groups OG{4}");
            q.AddSqlRelationshipJoinCondition("organism", "PW", "PP", "PW{3}.id = PP{3}_{4}.pathway_id");
            q.AddSqlRelationshipJoinCondition("organism", "PP", "C", "PP{3}_{4}.process_id = C{3}_{4}.process_id");
            q.AddSqlRelationshipJoinCondition("organism", "C", "OG", "C{3}_{4}.organism_group_id = OG{4}.id");
            //r.AddRelationship("organism", "Pathway", "A pathway that contains a process that takes place in this organism");

            AddField("name", "Name", true,
                     new QInput[] { new QInputPathwayName() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("name", "PW{0}.name AS [Pathway {1} Name], PW{0}.id AS [IDpw {1}_{0}]",
                                new string[] { "PW" });
            q.AddSqlFieldCondition("name", "name", "PW{0}.name = {2}",
                                   new string[] { "PW" });

            AddLink("AddContains", 0, "name", "Contains",
                    new string[] { "process", "molecule" },
                    new string[] { "Process", "Molecular Entity" },
                    new string[] { "A process that takes place in this pathway", "A molecular entity that takes place in a process in this pathway" },
                    new string[] { "a ", " or a ", "" });

            AddLink("AddIn", 0, "AddContains", "In",
                    new string[] { "organism" },
                    new string[] { "Organism" },
                    new string[] { "An organism in which a process in this pathway takes place" },
                    new string[] { "an ", "" });
        }
    }
}