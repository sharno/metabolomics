using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    public class QueryFailureResult : IQueryResults
    {
        public string Message;

        public QueryFailureResult(string message)
        {
            this.Message = message;
        }
    }
}
