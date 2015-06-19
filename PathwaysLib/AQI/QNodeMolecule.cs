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
    /// A query node for molecular entities
    /// </summary>
    [QNodeType("molecule")]
    public class QNodeMolecule : QNode
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private QNodeMolecule() { }

        public QNodeMolecule(QNode parent)
            : base("molecule", "Molecular Entity", parent, new SqlDatasetQuerier(PathwaysAQIUtil.Instance), new SqlDatasetRenderer(PathwaysAQIUtil.Instance), new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            SqlDatasetQuerier q = (SqlDatasetQuerier) Querier;
         
            // Add tables used in queries
            q.AddSqlTable("ENL", "entity_name_lookups ENL{0}");
            q.AddSqlTable("ME", "molecular_entities ME{0}");
            q.AddSqlTable("MET", "molecular_entity_types MET{0}");
            q.AddSqlTable("NT", "name_types NT{0}");
            q.AddSqlTable("PE", "process_entities PE{0}");
            q.AddSqlTable("PER", "process_entity_roles PER{0}");

            // Joins between tables used by this node (minimal set, NOT all combinations possible!)
            // The SQL engine can create any combinations it needs from a minimal set
            q.AddSqlJoinCondition("ME", "PE", "ME{0}.id = PE{0}.entity_id");
            q.AddSqlJoinCondition("ME", "ENL", "ME{0}.id = ENL{0}.entity_id");
            q.AddSqlJoinCondition("ENL", "NT", "ENL{0}.name_type_id = NT{0}.name_type_id");
            q.AddSqlJoinCondition("PE", "PER", "PE{0}.role_id = PER{0}.role_id");
            q.AddSqlJoinCondition("ME", "MET", "ME{0}.type_id = MET{0}.type_id");

            // Relationship with the Pathway Node
            AddRelationship("pathway", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("pathway", "ME", "molecular_entities ME{3}");
            q.AddSqlRelationshipTable("pathway", "PE", "process_entities PE{3}");
            q.AddSqlRelationshipTable("pathway", "PP", "pathway_processes PP{3}_{4}");
            q.AddSqlRelationshipTable("pathway", "PW", "pathways PW{4}");
            q.AddSqlRelationshipJoinCondition("pathway", "ME", "PE", "ME{3}.id = PE{3}.entity_id");
            q.AddSqlRelationshipJoinCondition("pathway", "PE", "PP", "PE{3}.process_id = PP{3}_{4}.process_id");
            q.AddSqlRelationshipJoinCondition("pathway", "PP", "PW", "PP{3}_{4}.pathway_id = PW{4}.id");

            // Relationship with the Process Node
            AddRelationship("process", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("process", "ME", "molecular_entities ME{3}");
            q.AddSqlRelationshipTable("process", "PE", "process_entities PE{3}");
            q.AddSqlRelationshipTable("process", "PR", "processes PR{4}");
            q.AddSqlRelationshipJoinCondition("process", "ME", "PE", "ME{3}.id = PE{3}.entity_id");
            q.AddSqlRelationshipJoinCondition("process", "PE", "PR", "PE{3}.process_id = PR{4}.id");

            // Relationship with the Organism Node
            AddRelationship("organism", new QRelationship.RelationshipConstraintDelegate[] { new QRelationship.RelationshipConstraintDelegate(QRelationship.ConstraintNotDuplicatedInPathToRoot) });
            q.AddSqlRelationshipTable("organism", "ME", "molecular_entities ME{3}");
            q.AddSqlRelationshipTable("organism", "PE", "process_entities PE{3}");
            q.AddSqlRelationshipTable("organism", "C", "catalyzes C{3}_{4}");
            q.AddSqlRelationshipTable("organism", "OG", "organism_groups OG{4}");
            q.AddSqlRelationshipJoinCondition("organism", "ME", "PE", "ME{3}.id = PE{3}.entity_id");
            q.AddSqlRelationshipJoinCondition("organism", "PE", "C", "PE{3}.process_id = C{3}_{4}.process_id");
            q.AddSqlRelationshipJoinCondition("organism", "C", "OG", "C{3}_{4}.organism_group_id = OG{4}.id");

            // Add fields
            AddField("name", "Name", true,
                     new QInput[] { new QInputMoleculeName() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("name", "ME{0}.name AS [Molecule {1} Name], ME{0}.id AS [IDme {1}_{0}]",
                                new string[] { "ME" });
            q.AddSqlFieldCondition("name", "name", "ME{0}.name = {2}",
                                   new string[] { "ME" });

            AddField("nameType", "Name type", false,
                     new QInput[] { new QInputMoleculeNameType() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("nameType", "NT{0}.name AS [Molecule {1} Name Type]",
                                new string[] { "NT", "ME" });
            q.AddSqlFieldCondition("nameType", "nameType", "NT{0}.name = {2}",
                                   new string[] { "NT", "ME" });
            
            AddField("type", "Molecule type", false,
                     new QInput[] { new QInputMoleculeType() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("type", "MET{0}.name AS [Molecule {1} Type]",
                                new string[] { "MET", "ME" });
            q.AddSqlFieldCondition("type", "type", "MET{0}.name = {2}",
                                   new string[] { "MET", "ME" });
            
            AddField("role", "Role", false,
                     new QInput[] { new QInputMoleculeRole() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("role", "PER{0}.name AS [Molecule {1} Role]",
                                new string[] { "PER", "ME" });
            q.AddSqlFieldCondition("role", "role", "PER{0}.name = {2}",
                                   new string[] { "PER", "ME" });

            AddField("amount", "Amount", false,
                     new QInput[] { new QInputMoleculeAmount() },
                     new string[] { "", "" },
                     new QConnector[] { new QConnectorOr() });
            q.AddSqlFieldSelect("amount", "PE{0}.quantity AS [Molecule {1} Quantity]",
                                new string[] { "PE", "ME" });
            q.AddSqlFieldCondition("amount", "amount", "PE{0}.quantity = {2}",
                                   new string[] { "PE", "ME" });

            // Add links to add child nodes
            AddLink("AddContained", 0, "amount", "Contained in",
                    new string[] { "pathway", "process" },
                    new string[] { "Pathway", "Process" },
                    new string[] { "A pathway that contains a process that involves this molecular entity", "A process that involves this molecular entity" },
                    new string[] { "a ", " and/or a ", "" });

            AddLink("AddIn", 0, "AddContained", "In",
                    new string[] { "organism" },
                    new string[] { "Organism" },
                    new string[] { "An organism in which a process that involves this molecular entity takes place" },
                    new string[] { "a ", "" });
        }
    }
}