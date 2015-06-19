using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_pwlinks")]
    public class QNodePathPWLinks : QNode
    {
        private QNodePathPWLinks()
        { }

        public QNodePathPWLinks(QNode parent)
            : base("path_pwlinks", "Pathway Network Paths", parent, new PathPWLinksQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance, "Pathways", "Molecules", "pw", "me"), new QNodeBasicRenderer(false), new QFieldBasicRenderer())
        {
            AddRelationship("path_graph_restriction_organism", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_from_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_from_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_to_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_to_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_including_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });

            //AddField("common_molecules", "Common Molecules",
            //         new QInput[] { new QInputPathNeighborhoodGraphRestrictionCommonMolecules() },
            //         new string[] { "", " Include common molecules?" },
            //         new QConnector[] { });

            AddField("search_direction", "Search Direction",
                     new QInput[] { new QInputPathSearchDirection("search_direction", "Undirected") },
                     new string[] { "", "" },
                     new QConnector[] { });

            AddLink("graph_restrictions", 0, "search_direction", "Restrict network",
                    new string[] { "path_graph_restriction_organism" },
                    new string[] { "Organism" },
                    new string[] { "Restrict the metabolic network graph based on a particular organism" },
                    new string[] { "to a certain ", "" });

            AddLink("from", 1, "graph_restrictions", "From",
                    new string[] { "path_from_pathway", "path_from_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Perform the path query from a pathway", "Perform the path query from a molecule" },
                    new string[] { "a ", " or a ", "" });

            AddLink("to", 0, "from", "To",
                    new string[] { "path_to_pathway", "path_to_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Perform a path query to a pathway", "Perform a path query to a molecule" },
                    new string[] { "a ", " or a ", "" });

            AddField("path_restrictions_length", "Total Path Length",
                     new QInput[] { new QInputRestrictionLength("_min"), new QInputRestrictionLength("_max") },
                     new string[] { "at least ", " and at most ", "" },
                     new QConnector[] { });

            AddLink("path_restrictions_including", 0, "path_restrictions_length", "Including",
                    new string[] { "path_restriction_including_pathway", "path_restriction_including_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Specify that a particular pathway must occur somewhere in the path", "Specify that a particular molecule must occur somewhere in the path" },
                    new string[] { "a ", " or a ", "" });

            AddLink("path_restrictions_not_including", 0, "path_restrictions_including", "Not Including",
                    new string[] { "path_restriction_not_including_pathway", "path_restriction_not_including_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Specify that a particular pathway must not occur anywhere in the path", "Specify that a particular molecule must not occur anywhere in the path" },
                    new string[] { "a ", " or a ", "" });
        }
    }
}