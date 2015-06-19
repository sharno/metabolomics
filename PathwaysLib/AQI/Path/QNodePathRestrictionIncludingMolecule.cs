using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_restriction_including_molecule")]
    public class QNodePathRestrictionIncludingMolecule : QNode
    {
        private QNodePathRestrictionIncludingMolecule()
        { }

        public QNodePathRestrictionIncludingMolecule(QNode parent)
            : base("path_restriction_including_molecule", "Molecule", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "In Pathway",
                 new QInput[] { new QInputPathPathwayName() },
                 new string[] { "", " (optional if there is a single pathway restriction)" },
                 new QConnector[] { });

            AddField("molecule_name", "Name",
                     new QInput[] { new QInputPathMoleculeName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}