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
    /// A query node for processes
    /// </summary>
    [QNodeType("process")]
    public class QNodeProcess : QNode
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private QNodeProcess() { }

        public QNodeProcess(QNode parent)
            : base("process", "Process", parent, new SqlDatasetQuerier(PathwaysAQIUtil.Instance), new SqlDatasetRenderer(PathwaysAQIUtil.Instance), new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            SqlDatasetQuerier q = (SqlDatasetQuerier) Querier;
            //QNodeBasicRenderer r = (QNodeBasicRenderer) RendererNode;

            q.AddSqlTable("PR", "processes PR{0}");

            //r.AddLinkSortOrder = 2;
            //r.AddRelationship("", "Process", "Search for a process");

            AddRelationship("pathway", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("pathway", "PR", "processes PR{3}");
            q.AddSqlRelationshipTable("pathway", "PP", "pathway_processes PP{3}_{4}");
            q.AddSqlRelationshipTable("pathway", "PW", "pathways PW{4}");
            q.AddSqlRelationshipJoinCondition("pathway", "PR", "PP", "PR{3}.id = PP{3}_{4}.process_id");
            q.AddSqlRelationshipJoinCondition("pathway", "PP", "PW", "PP{3}_{4}.pathway_id = PW{4}.id");
            //r.AddRelationship("pathway", "Process", "A process that takes place in this pathway");

            AddRelationship("molecule", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("molecule", "PR", "processes PR{3}");
            q.AddSqlRelationshipTable("molecule", "PE", "process_entities PE{4}");
            q.AddSqlRelationshipTable("molecule", "ME", "molecular_entities ME{4}");
            q.AddSqlRelationshipJoinCondition("molecule", "PR", "PE", "PR{3}.id = PE{4}.process_id");
            q.AddSqlRelationshipJoinCondition("molecule", "PE", "ME", "PE{4}.entity_id = ME{4}.id");
            //r.AddRelationship("molecule", "Process", "A process that involves this molecular entity");

            AddRelationship("organism", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("organism", "PR", "processes PR{3}");
            q.AddSqlRelationshipTable("organism", "C", "catalyzes C{3}_{4}");
            q.AddSqlRelationshipTable("organism", "OG", "organism_groups OG{4}");
            q.AddSqlRelationshipJoinCondition("organism", "PR", "C", "PR{3}.id = C{3}_{4}.process_id");
            q.AddSqlRelationshipJoinCondition("organism", "C", "OG", "C{3}_{4}.organism_group_id = OG{4}.id");
            //r.AddRelationship("organism", "Process", "A process that takes place in this organism");

            AddField("name", "Name", true,
                     new QInput[] { new QInputProcessName() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("name", "PR{0}.name AS [Process {1} Name], PR{0}.generic_process_id AS [IDpc {1}_{0}]",
                                new string[] { "PR" });
            q.AddSqlFieldCondition("name", "name", "PR{0}.name = {2}",
                                   new string[] { "PR" });

            AddField("reversible", "Reversible", false,
                     new QInput[] { new QInputProcessReversible() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("reversible", "PR{0}.reversible AS [Process {1} Reversible]",
                                new string[] { "PR" });
            q.AddSqlFieldCondition("reversible", "reversible", "PR{0}.reversible = {2}",
                                   new string[] { "PR" });

            AddLink("AddContained", 0, "reversible", "Contained in",
                    new string[] { "pathway" },
                    new string[] { "Pathway" },
                    new string[] { "A pathway in which this process takes place" },
                    new string[] { "a ", "" });

            AddLink("AddContains", 0, "AddContained", "Contains",
                    new string[] { "molecule" },
                    new string[] { "Molecular Entity" },
                    new string[] { "A molecular entity involved in this process" },
                    new string[] { "a ", "" });

            AddLink("AddIn", 0, "AddContains", "In",
                    new string[] { "organism" },
                    new string[] { "Organism" },
                    new string[] { "An organism in which this process takes place" },
                    new string[] { "a ", "" });
        }
    }
}