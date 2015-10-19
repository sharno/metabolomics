using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol;

namespace CyclesCacher.SimpleCycle
{
    class Vertex
    {
        public Guid Id;
        public HashSet<Guid> Next;
    }
}
