using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// An exception that is triggered when a query limit is reached
    /// </summary>
    public class QueryReachedLimitException : Exception
    {
        public QueryReachedLimitException() : base() { }
    }
}