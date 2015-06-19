using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// A wrapper for the connectors between the fields
    /// </summary>
    public abstract class QConnector
    {
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        private QConnector()
        { }

        public QConnector(string name)
        {
            _name = name;
        }
    }
}