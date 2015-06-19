using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// An exception thrown by the path querier if an error is encountered
    /// </summary>
    public class PathQuerierException : Exception
    {
        public PathQuerierException(string message) : base(message) { }
        public PathQuerierException(string message, params object[] args) : base(string.Format(message, args)) { }
    }
}