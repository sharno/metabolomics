using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PathQueryLib
{
    public class StaticManager : IManager
    {
        #region Singleton Pattern

        private StaticManager()
        {
        }

        private static StaticManager instance = new StaticManager();

        public static IManager Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        private ReaderWriterLock rwLock = new ReaderWriterLock();
        private Dictionary<string, IGraph> _graphStore = new Dictionary<string, IGraph>();
        private Dictionary<string,IQuery> _queryStore =new Dictionary<string,IQuery>();

        #region IManager Members

        public void RegisterGraph(string name, IGraph graph)
        {
            rwLock.AcquireWriterLock(-1); // no timeout
            try
            {
                if (!ContainsGraph(name))
                    _graphStore.Add(name, graph);
            }
            finally
            {
                rwLock.ReleaseWriterLock();
            }
        }

        public void RegisterQuery(string name, IQuery query)
        {
            rwLock.AcquireWriterLock(-1); // no timeout
            try
            {
                if (!ContainsQuery(name))
                    _queryStore.Add(name, query);
            }
            finally
            {
                rwLock.ReleaseWriterLock();
            }
        }

        public bool ContainsGraph(string name)
        {
            rwLock.AcquireReaderLock(-1); // no timeout
            try
            {
                return _graphStore.ContainsKey(name);
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
        }

        public bool ContainsQuery(string name)
        {
            rwLock.AcquireReaderLock(-1); // no timeout
            try
            {
                return _queryStore.ContainsKey(name);
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
        }

        public void UnregisterGraph(string name)
        {
            rwLock.AcquireWriterLock(-1); // no timeout
            try
            {
                if (ContainsGraph(name))
                    _graphStore.Remove(name);
            }
            finally
            {
                rwLock.ReleaseWriterLock();
            }
        }

        public void UnregisterQuery(string name)
        {
            rwLock.AcquireWriterLock(-1); // no timeout
            try
            {
                if (ContainsQuery(name))
                    _queryStore.Remove(name);
            }
            finally
            {
                rwLock.ReleaseWriterLock();
            }
        }

        public IQueryResults Execute(string queryName, string graphName, EdgeType edgeType, IQueryParameters parameters)
        {
            rwLock.AcquireReaderLock(-1); // no timeout
            IQueryResults retVal = null;

            try
            {
                if (_queryStore.ContainsKey(queryName))
                {
                    if (_graphStore.ContainsKey(graphName))
                    {
                        // actually execute query
                        retVal = _queryStore[queryName].Execute(_graphStore[graphName], edgeType, parameters);
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }

            return retVal;
        }

        #endregion
    }
}
