namespace Metabol.Util.SimpleCycle
{
    using System;
    using System.Collections.Generic;

    class Edge
    {
        public Guid Id;
        public HashSet<Guid> Next;
        public HashSet<Guid> Previous;
    }
}
