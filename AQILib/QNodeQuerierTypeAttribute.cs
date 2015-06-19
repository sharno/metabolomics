using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An attribute for a querier
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class QNodeQuerierTypeAttribute : Attribute
    {
        private string _target;

        public string Target
        {
            get { return _target; }
        }

        private QNodeQuerierTypeAttribute()
        { }

        public QNodeQuerierTypeAttribute(string target)
        {
            _target = target;
        }

        public override string ToString()
        {
            return _target;
        }
    }
}