using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_from_pathway")]
    public class QNodePathFromPathway : QNode
    {
        private QNodePathFromPathway()
        { }

        public QNodePathFromPathway(QNode parent)
            : base("path_from_pathway", "Pathway", parent, null, null, new QNodeBasicRenderer(false), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "Name",
                     new QInput[] { new QInputPathPathwayName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}