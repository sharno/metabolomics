using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_metabolicnw")]
    public class QNodePathMetabolicNW : QNode
    {
        private QNodePathMetabolicNW()
        { }

        public QNodePathMetabolicNW(QNode parent)
            : base("path_metabolicnw", "Metabolic Network Paths", parent, new PathMetabolicNWQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance, "Molecules", "Processes", "me", "pc"), new QNodeBasicRenderer(false), new QFieldBasicRenderer())
        {
            AddRelationship("path_graph_restriction_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_graph_restriction_organism", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_from_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_from_process", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_to_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_to_process", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_including_process", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_process", new QRelationship.RelationshipConstraintDelegate[] { });

            AddField("common_molecules", "Common Molecules",
                     new QInput[] { new QInputPathNeighborhoodGraphRestrictionCommonMolecules() },
                     new string[] { "", " Include common molecules?" },
                     new QConnector[] { });

            AddField("search_direction", "Search Direction",
                     new QInput[] { new QInputPathSearchDirection("search_direction", "Undirected") },
                     new string[] { "", "" },
                     new QConnector[] { });

            AddLink("graph_restrictions", 0, "search_direction", "Restrict network",
                    new string[] { "path_graph_restriction_pathway", "path_graph_restriction_organism" },
                    new string[] { "Pathway", "Organism" },
                    new string[] { "Restrict the metabolic network graph based on a particular pathway", "Restrict the metabolic network graph based on a particular organism" },
                    new string[] { "to a certain ", " or ", "" });

            AddLink("from", 1, "graph_restrictions", "From",
                    new string[] { "path_from_molecule", "path_from_process" },
                    new string[] { "Molecular Entity", "Process" },
                    new string[] { "Perform the path query from a molecule", "Perform the path query from a process" },
                    new string[] { "a ", " or a ", "" });

            AddLink("to", 0, "from", "To",
                    new string[] { "path_to_molecule", "path_to_process" },
                    new string[] { "Molecular Entity", "Process" },
                    new string[] { "Perform a path query to a molecule", "Perform a path query to a process" },
                    new string[] { "a ", " or a ", "" });

            AddField("path_restrictions_length", "Total Path Length",
                     new QInput[] { new QInputRestrictionLength("_min"), new QInputRestrictionLength("_max") },
                     new string[] { "at least ", " and at most ", "" },
                     new QConnector[] { });

            AddLink("path_restrictions_including", 0, "path_restrictions_length", "Including",
                    new string[] { "path_restriction_including_molecule", "path_restriction_including_process" },
                    new string[] { "Molecular Entity", "Process" },
                    new string[] { "Specify that a particular molecule must occur somewhere in the path", "Specify that a particular process must occur somewhere in the path" },
                    new string[] { "a ", " or a ", "" });

            AddLink("path_restrictions_not_including", 0, "path_restrictions_including", "Not Including",
                    new string[] { "path_restriction_not_including_molecule", "path_restriction_not_including_process" },
                    new string[] { "Molecular Entity", "Process" },
                    new string[] { "Specify that a particular molecule must not occur anywhere in the path", "Specify that a particular process must not occur anywhere in the path" },
                    new string[] { "a ", " or a ", "" });
        }
    }
}