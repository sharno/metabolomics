using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_from_process")]
    public class QNodePathFromProcess : QNode
    {
        private QNodePathFromProcess()
        { }

        public QNodePathFromProcess(QNode parent)
            : base("path_from_process", "Process", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(false), new QFieldBasicRenderer())
        {
            AddField("pathway_name", "Starting in Pathway",
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