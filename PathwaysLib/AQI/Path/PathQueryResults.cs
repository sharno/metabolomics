using AQILib;
using PathQueryLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// A container to wrap the results from a path-based query.
    /// Holds both the path query engine's results and an error string if an error occured.
    /// </summary>
    public class PathQueryResults : AQILib.IQueryResults
    {
        private PathQueryLib.IQueryResults _pathResults;
        private string _errorString;

        public PathQueryLib.IQueryResults PathResults
        {
            get { return _pathResults; }
        }

        public string ErrorString
        {
            get { return _errorString; }
        }

        public bool IsErrorResult
        {
            get { return _errorString != null; }
        }

        private PathQueryResults()
        { }

        public PathQueryResults(PathQueryLib.IQueryResults pathResults)
        {
            _pathResults = pathResults;
            _errorString = null;
        }

        public PathQueryResults(string errorString)
        {
            _pathResults = null;
            _errorString = errorString;
        }
    }
}