using System;
using System.Collections.Generic;
using System.Text;

namespace AQILib
{
    /// <summary>
    /// An exception thrown if an error occurs in the querier
    /// </summary>
    public class QueryException : Exception
    {
        public QueryException(string message) : base(message) { }
    }
}