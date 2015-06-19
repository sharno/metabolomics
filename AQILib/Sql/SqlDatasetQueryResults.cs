using AQILib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AQILib.Sql
{
    public class SqlDatasetQueryResults : IQueryResults
    {
        private DataTable _dt;
        private bool _hasOutputs;
        private int _resultsCount;
        private string _sqlQuery;
        private string _sqlQueryNicelyFormatted;

        public DataTable Dt
        {
            get { return _dt; }
            set { _dt = value; }
        }

        public bool HasOutputs
        {
            get { return _hasOutputs; }
            set { _hasOutputs = value; }
        }

        public int ResultsCount
        {
            get { return _resultsCount; }
            set { _resultsCount = value; }
        }

        public string SqlQuery
        {
            get { return _sqlQuery; }
            set { _sqlQuery = value; }
        }

        public string SqlQueryNicelyFormatted
        {
            get { return _sqlQueryNicelyFormatted; }
            set { _sqlQueryNicelyFormatted = value; }
        }

        public SqlDatasetQueryResults()
        { }

        public SqlDatasetQueryResults(DataTable dt, bool hasOutputs, int resultsCount, string sqlQuery, string sqlQueryNicelyFormatted)
        {
            _dt = dt;
            _hasOutputs = hasOutputs;
            _resultsCount = resultsCount;
            _sqlQuery = sqlQuery;
            _sqlQueryNicelyFormatted = sqlQueryNicelyFormatted;
        }
    }
}