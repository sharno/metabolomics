using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_to_process")]
    public class QNodePathToProcess : QNode
    {
        private QNodePathToProcess()
        { }

        public QNodePathToProcess(QNode parent)
            : base("path_to_process", "Process", parent, null, null, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddRelationship("path_restriction_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_including_process", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_molecule", new QRelationship.RelationshipConstraintDelegate[] { });
            AddRelationship("path_restriction_not_including_process", new QRelationship.RelationshipConstraintDelegate[] { });

            AddField("process_name", "Name",
                     new QInput[] { new QInputPathProcessName() },
                     new string[] { "", "" },
                     new QConnector[] { });

            AddField("pathway_name", "In Pathway",
                      new QInput[] { new QInputPathPathwayName() },
                      new string[] { "", " (optional if there is a single pathway restriction)" },
                      new QConnector[] { });

            AddField("path_restrictions_length", "Path Length",
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