using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A counter for the node IDs in a query
    /// </summary>
    public class QNodeIdCounter
    {
        private int _nodeId;
        private Dictionary<string, int> _typeId;

        public int CurrentNodeId()
        {
            return _nodeId;
        }

        [Obsolete("QNodeIdCounter.CurrentTypeId(string type)", true)]
        public int CurrentTypeId(string type)
        {
            return _typeId.ContainsKey(type) ? _typeId[type] : -1;
        }

        public int NextNodeId()
        {
            return ++_nodeId;
        }

        [Obsolete("QNodeIdCounter.NextTypeId(string type)", true)]
        public int NextTypeId(string type)
        {
            return _typeId.ContainsKey(type) ? ++_typeId[type] : (_typeId[type] = 1);
        }

        public QNodeIdCounter()
        {
            _nodeId = 0;
            _typeId = new Dictionary<string, int>();
        }
    }
}