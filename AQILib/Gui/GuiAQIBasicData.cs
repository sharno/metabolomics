using AQILib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Gui
{
    public class GuiAQIBasicData : IGuiData
    {
        private QNode _rootNode;
        private QNodeIdCounter _idCounter;
        private string _parentNodeID;
        private IAQIUtil _util;

        private GuiAQIBasicData()
        { }

        public GuiAQIBasicData(QNode rootNode, IAQIUtil util)
        {
            _rootNode = rootNode;
            _idCounter = new QNodeIdCounter();
            _parentNodeID = String.Empty;
            _util = util;
        }

        public GuiAQIBasicData(QNode rootNode, QNodeIdCounter idCounter, string parentNodeID, IAQIUtil util)
        {
            _rootNode = rootNode;
            _idCounter = idCounter;
            _parentNodeID = parentNodeID;
            _util = util;
        }

        public QNode RootNode
        {
            get { return _rootNode; }
        }

        public QNodeIdCounter IdCounter
        {
            get { return _idCounter; }
        }

        public string ParentNodeID
        {
            get { return _parentNodeID; }
        }

        public IAQIUtil Util
        {
            get { return _util; }
        }

    }
}