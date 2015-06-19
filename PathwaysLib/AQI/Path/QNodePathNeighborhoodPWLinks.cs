using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_neighborhood_pwlinks")]
    public class QNodePathNeighborhoodPWLinks : QNode
    {
        private QNodePathNeighborhoodPWLinks()
        { }

        public QNodePathNeighborhoodPWLinks(QNode parent)
            : base("path_neighborhood_pwlinks", "Pathway Network Neighborhood", parent, new PathNeighborhoodPWLinksQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance, "Pathways", "Molecules", "pw", "me"), new QNodeBasicRenderer(false), new QFieldBasicRenderer())
        {
            AddRelationship("path_graph_restriction_organism", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_from_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_from_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            //AddRelationship("path_restriction_including_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            //AddRelationship("path_restriction_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });

            //BE: not including common molecules for pathway links graph
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
                    new string[] { "Restrict the pathway links graph based on a particular organism" },
                    new string[] { "to a certain ", "" });

            AddLink("from", 1, "graph_restrictions", "From",
                    new string[] { "path_from_pathway", "path_from_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Perform the neighborhood query from a pathway", "Perform the neighborhood query from a molecule" },
                    new string[] { "a ", " or a ", "" });

            AddField("path_restrictions_length", "Length",
                     new QInput[] { new QInputRestrictionLength("_min", "0"), new QInputRestrictionLength("_max", "1") },
                     new string[] { "at least ", " and at most ", "" },
                     new QConnector[] { });

            /*AddLink("path_restrictions_including", 0, "path_restrictions_length", "Including",
                    new string[] { "path_restriction_including_pathway", "path_restriction_including_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Specify that a particular pathway must occur somewhere in the path", "Specify that a particular molecule must occur somewhere in the path" },
                    new string[] { "a ", " or a ", "" });*/

            AddLink("path_restrictions_not_including", 0, "path_restrictions_length" /*"path_restrictions_including"*/, "Not Including",
                    new string[] { "path_restriction_not_including_pathway", "path_restriction_not_including_molecule" },
                    new string[] { "Pathway", "Molecule" },
                    new string[] { "Specify that a particular pathway must not occur anywhere in the path", "Specify that a particular molecule must not occur anywhere in the path" },
                    new string[] { "a ", " or a ", "" });
        }
    }
}