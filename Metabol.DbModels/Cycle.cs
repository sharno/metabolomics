namespace Metabol.Util
{
    using System;
    using System.Collections.Generic;

    class Cycle
    {
        public Cycle()
        {
            this.id = Guid.NewGuid();
            this.inCycleReactions = new Dictionary<Guid, HyperGraph.Edge>();
            this.outOfCycleReactions = new Dictionary<Guid, HyperGraph.Edge>();
        }

        public Guid id { get; protected set; }
        public HyperGraph graph = new HyperGraph();
        public Dictionary<Guid, HyperGraph.Edge> inCycleReactions { get; set; }
        public Dictionary<Guid, HyperGraph.Edge> outOfCycleReactions { get; set; }

        public void AddInCycleReaction(HyperGraph.Edge inCycleReaction)
        {
            this.inCycleReactions[inCycleReaction.Id] = inCycleReaction;
        }

        public void AddOutOfCycleReaction(HyperGraph.Edge outOfCycleReaction)
        {
            this.outOfCycleReactions[outOfCycleReaction.Id] = outOfCycleReaction;
        }
    }
}
