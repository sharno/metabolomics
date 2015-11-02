using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.Util.SimpleCycle
{
    class Cycle : Edge
    {
        public HashSet<Guid> MetabolitesConsumed;
        public HashSet<Guid> MetabolitesProduced;
    }
}
