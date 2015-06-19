using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An attribute assigned to implemented QNode classes; used in the reflection code
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class QNodeTypeAttribute : Attribute
    {
        private string _nodeTypeName;

        public string NodeTypeName
        {
            get { return _nodeTypeName; }
        }

        private QNodeTypeAttribute()
        { }

        public QNodeTypeAttribute(string nodeTypeName)
        {
            _nodeTypeName = nodeTypeName;
        }

        public override string ToString()
        {
            return _nodeTypeName;
        }
    }
}