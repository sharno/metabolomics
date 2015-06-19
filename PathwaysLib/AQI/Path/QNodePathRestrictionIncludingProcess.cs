using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_restriction_including_process")]
    public class QNodePathRestrictionIncludingProcess : QNode
    {
        private QNodePathRestrictionIncludingProcess()
        { }

        public QNodePathRestrictionIncludingProcess(QNode parent)
            : base("path_restriction_including_process", "Process", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "In Pathway",
                 new QInput[] { new QInputPathPathwayName() },
                 new string[] { "", " (optional if there is a single pathway restriction)" },
                 new QConnector[] { });

            AddField("process_name", "Name",
                     new QInput[] { new QInputPathProcessName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}