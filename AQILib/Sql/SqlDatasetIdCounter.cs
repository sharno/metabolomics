using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib.Sql
{
    public class SqlDatasetIdCounter
    {
        private int _nodeId;
        private Dictionary<int, int> _typeId;

        public int CurrentNodeId()
        {
            return _nodeId;
        }

        public int CurrentTypeId(int type)
        {
            return _typeId.ContainsKey(type) ? _typeId[type] : -1;
        }

        public int NextNodeId()
        {
            return ++_nodeId;
        }

        public int NextTypeId(int type)
        {
            return _typeId.ContainsKey(type) ? ++_typeId[type] : (_typeId[type] = 1);
        }

        public SqlDatasetIdCounter()
        {
            _nodeId = 0;
            _typeId = new Dictionary<int, int>();
        }
    }
}