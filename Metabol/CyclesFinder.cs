using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol
{
    class CyclesFinder
    {
        static Guid startingNode = Guid.NewGuid();

        public static void Test()
        {
            var a = startingNode;
            // var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var c = Guid.NewGuid();
            var d = Guid.NewGuid();
            var e = Guid.NewGuid();

            var b1 = Guid.NewGuid();
            var b2 = Guid.NewGuid();
            var b3 = Guid.NewGuid();

            var v1 = Guid.NewGuid();
            var v2 = Guid.NewGuid();
            var v3 = Guid.NewGuid();

            var graph = new HyperGraph();
            graph.AddNode(a, "A");
            graph.AddNode(b, "B");
            graph.AddNode(c, "C");
            graph.AddNode(d, "D");
            graph.AddNode(e, "E");

            graph.AddProduct(b1, "b1", a, "A");
            graph.AddProduct(v1, "v1", b, "B");
            graph.AddProduct(v2, "v2", c, "C");
            graph.AddProduct(v2, "v2", e, "E");
            graph.AddProduct(v3, "v3", d, "D");

            graph.AddReactant(v1, "v1", a, "A");
            graph.AddReactant(v1, "v1", e, "E");

            graph.AddReactant(v2, "v2", b, "B");

            graph.AddReactant(v3, "v3", a, "A");
            graph.AddReactant(v3, "v3", e, "E");

            graph.AddReactant(b3, "b3", d, "D");
            graph.AddReactant(b2, "b2", c, "C");
        }

        public void FindCycles(HyperGraph graph)
        {
            graph.GetNode(startingNode);
        }
    }
}
