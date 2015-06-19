using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A base link type to link parents to children
    /// </summary>
    public class QLink
    {
        private string _linkTypeId;
        private string _displayAfterId;
        private int _maxCardinality;

        private string _title;
        private List<string> _linkedNodeTypes;
        private List<string> _linkTexts;
        private List<string> _linkMouseOverTexts;
        private List<string> _descriptions;

        public string LinkTypeId
        {
            get { return _linkTypeId; }
        }

        public string DisplayAfterId
        {
            get { return _displayAfterId; }
        }

        public int MaxCardinality
        {
            get { return _maxCardinality; }
        }

        public string Title
        {
            get { return _title; }
        }

        public List<string> LinkedNodeTypes
        {
            get { return _linkedNodeTypes; }
        }

        public List<string> LinkTexts
        {
            get { return _linkTexts; }
        }

        public List<string> LinkMouseOverTexts
        {
            get { return _linkMouseOverTexts; }
        }

        public List<string> Descriptions
        {
            get { return _descriptions; }
        }

        private QLink()
        { }

        public QLink(string linkTypeId, string displayAfterId, int maxCardinality, string title, List<string> linkedNodeTypes, List<string> linkTexts, List<string> linkMouseOverTexts, List<string> descriptions)
        {
            _linkTypeId = linkTypeId;
            _displayAfterId = displayAfterId;
            _maxCardinality = maxCardinality;

            _title = title;
            _linkedNodeTypes = linkedNodeTypes;
            _linkTexts = linkTexts;
            _linkMouseOverTexts = linkMouseOverTexts;
            _descriptions = descriptions;
        }

        public int Cardinality(QNode parentNode)
        {
            int retVal = 0;

            foreach(QNode child in parentNode.Children)
                if(_linkedNodeTypes.Contains(child.NodeTypeName))
                    retVal++;

            return retVal;
        }
    }
}