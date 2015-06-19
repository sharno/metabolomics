using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An implementation of the 'or' connector used in the PathCase traditional AQI queries
    /// </summary>
    public class QConnectorOr : QConnector
    {
        public QConnectorOr()
            : base("or")
        { }
    }
}