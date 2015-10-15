using Metabol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CyclesCacher
{
    class Cycle
    {
        public Cycle()
        {
            id = Guid.NewGuid();
            inCycleReactions = new Dictionary<Guid, HyperGraph.Edge>();
            outOfCycleReactions = new Dictionary<Guid, HyperGraph.Edge>();
        }

        public Guid id { get; protected set; }
        public HyperGraph graph = new HyperGraph();
        public Dictionary<Guid, HyperGraph.Edge> inCycleReactions { get; set; }
        public Dictionary<Guid, HyperGraph.Edge> outOfCycleReactions { get; set; }

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
