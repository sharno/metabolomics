using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_from_molecule")]
    public class QNodePathFromMolecule : QNode
    {
        private QNodePathFromMolecule()
        { }

        public QNodePathFromMolecule(QNode parent)
            : base("path_from_molecule", "Molecule", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(false), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "Starting in Pathway",
                     new QInput[] { new QInputPathPathwayName() },
                     new string[] { "", " (optional if there is a single pathway restriction)" },
                     new QConnector[] { });

            AddField("molecule_name", "Molecule Name",
                     new QInput[] { new QInputPathMoleculeName() },
                     new string[] { "", "" },
                     new QConnector[] { });        

        }
    }
}