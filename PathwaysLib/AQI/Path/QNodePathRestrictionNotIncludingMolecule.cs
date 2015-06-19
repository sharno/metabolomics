using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_restriction_not_including_molecule")]
    public class QNodePathRestrictionNotIncludingMolecule : QNode
    {
        private QNodePathRestrictionNotIncludingMolecule()
        { }

        public QNodePathRestrictionNotIncludingMolecule(QNode parent)
            : base("path_restriction_not_including_molecule", "Molecule", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("molecule_name", "Name",
                     new QInput[] { new QInputPathMoleculeName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}