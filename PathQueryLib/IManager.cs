using System;
using System.Collections.Generic;
using System.Text;

namespace PathQueryLib
{
    /// <summary>
    /// An interface for the manager that runs the graph service.
    /// The instance is hosted in the GraphService while this interface is also used by any client of GraphService to work with the object over remoting.
    /// </summary>
    public interface IManager
    {
        void RegisterGraph(string name, IGraph graph);
        void RegisterQuery(string name, IQuery query);
        bool ContainsGraph(string name);
        bool ContainsQuery(string name);
        void UnregisterGraph(string name);
        void UnregisterQuery(string name);
        IQueryResults Execute(string queryName, string graphName, EdgeType edgeType, IQueryParameters parameters);
    }
}