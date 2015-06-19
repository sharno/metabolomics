using AQILib;
using PathQueryLib;
using PathwaysLib.PathQuery;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace PathwaysLib.AQI.Path
{
    /// <summary>
    /// A querier that wraps the functionality for the path-based queries
    /// </summary>
    public abstract class PathQuerier : Querier
    {
        protected IAQIUtil _util;

        protected PathQuerier()
        { }

        protected PathQuerier(IAQIUtil util)
            : base()
        {
            _util = util;
        }
    }
}