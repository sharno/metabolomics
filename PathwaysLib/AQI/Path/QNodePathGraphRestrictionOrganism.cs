using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_graph_restriction_organism")]
    public class QNodePathGraphRestrictionOrganism : QNode
    {
        private QNodePathGraphRestrictionOrganism()
        { }

        public QNodePathGraphRestrictionOrganism(QNode parent)
            : base("path_graph_restriction_organism", "Organism", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("organism_name", "Name",
                     new QInput[] { new QInputPathOrganismName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}