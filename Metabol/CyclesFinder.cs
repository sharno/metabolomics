using System;
using System.Collections.Generic;
using System.Linq;

namespace Metabol
{
    class CyclesFinder
    {
        

        public static void Test()
        {
            var a = Guid.NewGuid();
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

            CyclesFinder cyclesFinder = new CyclesFinder();
            Dictionary<Guid, Dictionary<Guid, Vertex>> stronglyConnectedComponents = cyclesFinder.FindCycles(graph);
            cyclesFinder.CollapseCycles(graph, stronglyConnectedComponents);

            foreach (var stronglyConnectedComponent in stronglyConnectedComponents)
            {
                Console.WriteLine("component");
                foreach (var vertex in stronglyConnectedComponent.Value)
                {
                    Console.WriteLine(vertex.Value.Value.Label);
                }
            }

            Console.WriteLine("after collapse");
            foreach (var node in graph.Edges[v2].Reactants)
            {
                Console.WriteLine(node.Value.Label);
            }
            Console.ReadLine();

            Console.WriteLine("saving ");
            Util.SaveAsDgs(graph.Nodes[a], graph, "C://b/");
        }

        public void CollapseCycles(HyperGraph graph, Dictionary<Guid, Dictionary<Guid, Vertex>> stronglyConnectedComponents)
        {
            Dictionary<Guid, Cycle> cycles = new Dictionary<Guid, Cycle>();
            int index = 0;
            foreach (var stronglyConnectedComponent in stronglyConnectedComponents)
            {
                index ++;
                Guid cycleId = Guid.NewGuid();
                string cycleLabel = "_cycle_" + index + "_";

                Cycle cycle = new Cycle();
                cycles[cycleId] = cycle;

                bool inCycle = false;

                // store cycle data in a separate data structure
                foreach (var metabolite in stronglyConnectedComponent.Value)
                {
                    Console.WriteLine("metabol " + metabolite.Value.Value.Label);
                    cycle.graph.AddNode(metabolite.Key, metabolite.Value.Value.Label);

                    foreach (var consumer in metabolite.Value.Value.Consumers)
                    {
                        Console.WriteLine("consumer " + consumer.Label);
                        foreach (var product in consumer.Products)
                        {
                            if (metabolite.Value.Dependencies.ContainsKey(product.Key))
                            {
                                inCycle = true;
                                cycle.graph.AddReactant(consumer.Id, consumer.Label, metabolite.Key, metabolite.Value.Value.Label);
                                cycle.graph.AddProduct(consumer.Id, consumer.Label, product.Key, product.Value.Label);
                                Console.WriteLine("adding " + consumer.Label);
                            }
                        }

                        Dictionary<Guid, HyperGraph.Node> outsideProducedMetabolites = consumer.Products.Where(e => !stronglyConnectedComponent.Value.ContainsKey(e.Key)).ToDictionary(e => e.Key, e => e.Value);
                        Dictionary<Guid, HyperGraph.Node> outsideConsumedMetabolites = consumer.Reactants.Where(e => !stronglyConnectedComponent.Value.ContainsKey(e.Key)).ToDictionary(e => e.Key, e => e.Value);
                        Console.WriteLine(outsideProducedMetabolites.Count());
                        Console.WriteLine(outsideConsumedMetabolites.Count());
                        if (outsideProducedMetabolites.Any())
                        {
                            Console.WriteLine("outside");
                            cycle.AddOutOfCycleReaction(consumer);
                        }
                        else if (outsideConsumedMetabolites.Any())
                        {
                            cycle.AddInCycleReaction(consumer);
                        }
                    }
                }

                // collapse cycle in original graph
                graph.AddNode(cycleId, cycleLabel);
                foreach (var inCycleReaction in cycle.inCycleReactions)
                {
                    graph.AddProduct(inCycleReaction.Key, inCycleReaction.Value.Label, cycleId, cycleLabel);

                    Console.WriteLine("adding product to reaction " + inCycleReaction.Value.Label);
                }
                foreach (var outOfCycleReaction in cycle.outOfCycleReactions)
                {
                    graph.AddReactant(outOfCycleReaction.Key, outOfCycleReaction.Value.Label, cycleId, cycleLabel);

                    Console.WriteLine("adding reactant to reaction " + outOfCycleReaction.Value.Label);
                }
            }
        }

        public Dictionary<Guid, Dictionary<Guid, Vertex>> FindCycles(HyperGraph graph)
        {
            Stack<KeyValuePair<Guid, Vertex>> stack = new Stack<KeyValuePair<Guid, Vertex>>();
            Dictionary<Guid, Dictionary<Guid, Vertex>> stronglyConnectedComponents = new Dictionary<Guid, Dictionary<Guid, Vertex>>();
            int index = 0;
            Dictionary<Guid, Vertex> metabolitesDictionary = ConvertAllToVertices(graph);

            foreach (var v in metabolitesDictionary)
            {
                if (v.Value.Index < 0)
                {
                    StrongConnect(v, stronglyConnectedComponents, index, stack);
                }
            }
            // get rid of all single vertices in strongly connected components
            stronglyConnectedComponents = stronglyConnectedComponents.Where(s => s.Value.Count > 1).ToDictionary(s => s.Key, s => s.Value);
            
            return stronglyConnectedComponents;
        }

        private void StrongConnect(KeyValuePair<Guid, Vertex> v, Dictionary<Guid, Dictionary<Guid, Vertex>> stronglyConnectedComponents, int index, Stack<KeyValuePair<Guid, Vertex>> stack)
        {
            v.Value.Index = index;
            v.Value.LowLink = index;
            index++;

            stack.Push(v);

            foreach (var w in v.Value.Dependencies)
            {
                if (w.Value.Index < 0)
                {
                    StrongConnect(w, stronglyConnectedComponents, index, stack);
                    v.Value.LowLink = Math.Min(v.Value.LowLink, w.Value.LowLink);
                }
                else if (stack.Contains(w))
                {
                    v.Value.LowLink = Math.Min(v.Value.LowLink, w.Value.Index);
                }
            }

            if (v.Value.LowLink == v.Value.Index)
            {
                var scc = new Dictionary<Guid, Vertex>();;
                KeyValuePair<Guid, Vertex> w;
                do
                {
                    w = stack.Pop();
                    scc[w.Key] = w.Value;
                } while (v.Key != w.Key);
                stronglyConnectedComponents[Guid.NewGuid()] = scc;
            }
        }

        static public Dictionary<Guid, Vertex> ConvertAllToVertices(HyperGraph graph)
        {
            // to be returned vertices dictionary
            Dictionary<Guid, Vertex> metabolitesDictionary = new Dictionary<Guid, Vertex>();

            // convert all nodes to vertices
            foreach (var node in graph.Nodes)
            {
                Vertex v = new Vertex(node.Value);
                metabolitesDictionary[node.Key] = v;
            }

            // add dependencies for all vertices
            foreach (var metabolite in metabolitesDictionary.Values)
            {
                foreach (var reaction in metabolite.Value.Consumers)
                {
                    foreach (var nextMetabolite in reaction.Products)
                    {
                        metabolite.Dependencies[nextMetabolite.Key] = metabolitesDictionary[nextMetabolite.Key];
                    }
                }
            }

            return metabolitesDictionary;
        }

        public class Vertex
        {
            public Vertex(HyperGraph.Node metabolite)
            {
                this.Index = -1;
                this.Value = metabolite;
                this.Dependencies = new Dictionary<Guid, Vertex>();
            }

            internal int Index { get; set; }

            internal int LowLink { get; set; }

            internal Guid guid { get; set; }

            public HyperGraph.Node Value { get; set; }

            public Dictionary<Guid, Vertex> Dependencies { get; set; }
        }
    }
}
