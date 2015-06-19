using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI
{
    // Note: If the "root" node ID here is modified, it will BREAK both the C# and JAVASCRIPT! Thanks :)
    [QNodeType("root")]
    public class QNodeRoot : QNode
    {
        private QNodeRoot()
        { }

        public QNodeRoot(QNode parent)
            // Note: If the "root" node ID here is modified, it will BREAK both the C# and JAVASCRIPT! Thanks :)
            : base("root", "Root", null, new SingleRootQuerier(), new SingleRootRenderer(), new QNodeRootRenderer(), new QFieldBasicRenderer())
        {
            AddRelationship("pathway", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("process", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("organism", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_neighborhood_metabolicnw", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_neighborhood_pwlinks", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_metabolicnw", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_pwlinks", new QRelationship.RelationshipConstraintDelegate[] { });
            
            AddLink("Add", 1, String.Empty, "Search for",
                    new string[] { "pathway",
                                   "process",
                                   "molecule",
                                   "organism",
                                   "path_neighborhood_metabolicnw",
                                   "path_neighborhood_pwlinks",
                                   "path_metabolicnw",
                                   "path_pwlinks" },
                    new string[] { "Pathway",
                                   "Process",
                                   "Molecular Entity",
                                   "Organism",
                                   "Metabolic Network Neighborhood",
                                   "Pathway Network Neighborhood",
                                   "Metabolic Network Paths",
                                   "Pathway Network Paths" },
                    new string[] { "Search for a pathway",
                                   "Search for a process",
                                   "Search for a molecule",
                                   "Search for an organism",
                                   "Run a neighborhood query in the metabolic network graph",
                                   "Run a neighborhood query in the pathway links graph",
                                   "Run a path query in the metabolic network graph",
                                   "Run a path query in the pathway links graph" },
                    new string[] { "a ", ", a ", ", a ", ", or an ", "<br />a ", " or a ", "<br />a ", " or a ", "" });
        }
    }
}