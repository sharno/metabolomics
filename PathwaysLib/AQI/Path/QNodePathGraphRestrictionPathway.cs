using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_graph_restriction_pathway")]
    public class QNodePathGraphRestrictionPathway : QNode
    {
        private QNodePathGraphRestrictionPathway()
        { }

        public QNodePathGraphRestrictionPathway(QNode parent)
            : base("path_graph_restriction_pathway", "Pathway", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "Name",
                     new QInput[] { new QInputPathPathwayName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}