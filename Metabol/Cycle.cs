using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol
{
    class Cycle
    {
        public Cycle() 
        {
            inCycleReactions = new Dictionary<Guid, HyperGraph.Edge>();
            outOfCycleReactions = new Dictionary<Guid, HyperGraph.Edge>(); 
        }

        public HyperGraph graph = new HyperGraph();
        public Dictionary<Guid, HyperGraph.Edge> inCycleReactions { get; protected set; }
        public Dictionary<Guid, HyperGraph.Edge> outOfCycleReactions { get; protected set; }

        public void AddInCycleReaction(HyperGraph.Edge inCycleReaction)
        {
            inCycleReactions[inCycleReaction.Id] = inCycleReaction;
        }

        public void AddOutOfCycleReaction(HyperGraph.Edge outOfCycleReaction)
        {
            outOfCycleReactions[outOfCycleReaction.Id] = outOfCycleReaction;
        }
    }
}
