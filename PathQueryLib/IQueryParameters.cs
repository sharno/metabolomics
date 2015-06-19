using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// Interface used to pass query parameters between the engine and the interface
    /// </summary>
    public interface IQueryParameters
    {
        /// <summary>
        /// Used by the query to determine if the query is unsatisfiable by just looking at the parameters without running the query
        /// </summary>
        void VerifySatisfiability(IGraph graph);
    }
}