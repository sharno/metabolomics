using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// An interface for the query
    /// </summary>
    public interface IQuery : ICloneable
    {
        /// <summary>
        /// The main method used by the manager to execute the query
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="edgeType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryResults Execute(IGraph graph, EdgeType edgeType, IQueryParameters parameters);

        /// <summary>
        /// The cloning method that satisfies this interface's inheritance of ICloneable
        /// </summary>
        /// <returns></returns>
        //object Clone();
    }
}