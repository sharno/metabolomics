using System;
using System.Collections.Generic;

namespace Metabol.Util.SimpleCycle
{
    public class Cycle : Node
    {
        public Cycle()
        {
            Id = Guid.NewGuid();
        }

        public Cycle(Guid cycleId)
        {
            Id = cycleId;
        }

//        public HashSet<Guid> MetabolitesConsumed = new HashSet<Guid>();
//        public HashSet<Guid> MetabolitesProduced = new HashSet<Guid>();
    }
}
