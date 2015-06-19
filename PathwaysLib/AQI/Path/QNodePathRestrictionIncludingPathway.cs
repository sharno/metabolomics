using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_restriction_including_pathway")]
    public class QNodePathRestrictionIncludingPathway : QNode
    {
        private QNodePathRestrictionIncludingPathway()
        { }

        public QNodePathRestrictionIncludingPathway(QNode parent)
            : base("path_restriction_including_pathway", "Pathway", parent, null, null, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "Name",
                     new QInput[] { new QInputPathPathwayName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}