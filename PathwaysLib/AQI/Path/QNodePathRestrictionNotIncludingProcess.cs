using AQILib;
using AQILib.Gui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    [QNodeType("path_restriction_not_including_process")]
    public class QNodePathRestrictionNotIncludingProcess : QNode
    {
        private QNodePathRestrictionNotIncludingProcess()
        { }

        public QNodePathRestrictionNotIncludingProcess(QNode parent)
            : base("path_restriction_not_including_process", "Process", parent, null, null /* new PathQuerier(PathwaysAQIUtil.Instance), new PathRenderer(PathwaysAQIUtil.Instance)*/, new QNodeBasicRenderer(), new QFieldBasicRenderer())
        {
            AddField("process_name", "Name",
                     new QInput[] { new QInputPathProcessName() },
                     new string[] { "", "" },
                     new QConnector[] { });
        }
    }
}