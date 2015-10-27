namespace Metabol.Util.SimpleCycle
{
    using System;
    using System.Collections.Generic;

    class Vertex
    {
        public Guid Id;
        public HashSet<Guid> Next;
    }
}
