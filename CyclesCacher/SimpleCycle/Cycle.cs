//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Metabol;
//
//namespace CyclesCacher.SimpleCycle
//{
//    internal class Cycle
//    {
//        public Cycle()
//        {
//            id = Guid.NewGuid();
//        }
//
//        public Guid id { get; protected set; }
//        public Dictionary<Guid, HyperGraph.Edge> reactions = new Dictionary<Guid, HyperGraph.Edge>();
//
//        public void AddReaction(HyperGraph.Edge edge)
//        {
//            reactions.Add(edge.Id, edge);
//        }
//    }
//}
