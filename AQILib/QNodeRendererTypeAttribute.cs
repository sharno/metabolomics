using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An attribute for a renderer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QNodeRendererTypeAttribute : Attribute
    {
        private string _target;

        public string Target
        {
            get { return _target; }
        }

        private QNodeRendererTypeAttribute()
        { }

        public QNodeRendererTypeAttribute(string target)
        {
            _target = target;
        }

        public override string ToString()
        {
            return _target;
        }
    }
}