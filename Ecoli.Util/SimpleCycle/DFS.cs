using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecoli.Util.SimpleCycle
{
    class DFS
    {
        public static Dictionary<HyperGraph.Cycle, List<HyperGraph.Entity>> DetectAndCollapseCycles(HyperGraph hypergraph)
        {
            Dictionary<HyperGraph.Cycle, List<HyperGraph.Entity>> cycles = new Dictionary<HyperGraph.Cycle, List<HyperGraph.Entity>>();

            Dictionary<Guid, bool> marked = new Dictionary<Guid, bool>();
            Stack<HyperGraph.Entity> stack = new Stack<HyperGraph.Entity>();
            List<HyperGraph.Entity> cycle = new List<HyperGraph.Entity>();
            bool findingCycles = true;
            int count = 0;

            while (findingCycles)
            {
                //SaveAsDgs(hypergraph.Nodes.Last().Value, hypergraph, "C:\\Users\\sharno\\Desktop\\model2\\" + count);
                Console.WriteLine("finding cycle " + count);
                count++;
                findingCycles = false;
                marked.Clear();
                stack.Clear();
                cycle = new List<HyperGraph.Entity>();
                foreach (var v in hypergraph.Nodes.Values)
                {
                    if (Search(hypergraph, v, new HyperGraph.Edge(Guid.NewGuid(), string.Empty, false, false, 0), marked, stack, cycle))
                    {
                        findingCycles = true;
                        var cycleReaction = CollapseCycle(cycle, hypergraph);
                        // record to DB directly as the cycle reaction object might be changed if it's stored as an object as we did before
                        // todo modify it so that the object we save in memory doesn't get changed until we finish finding all cycles
                        Program.RecordToDatabase(cycleReaction, cycle);
                        //cycles.Add(cycleReaction, cycle);
                        break;
                    }
                }
            }
            return cycles;
        }

        public static bool Search(HyperGraph hypergraph, HyperGraph.Entity v, HyperGraph.Entity entityComingFrom, Dictionary<Guid, bool> marked, Stack<HyperGraph.Entity> stack, List<HyperGraph.Entity> cycle)
        {
            marked[v.Id] = true;
            stack.Push(v);

            // getting valid entities to explore in the hypergraph
            HashSet<HyperGraph.Entity> entitiesToExpore = new HashSet<HyperGraph.Entity>();
            if (v is HyperGraph.Node)
            {
                entitiesToExpore.UnionWith(v.Next.Values.Where(e => !e.Equals(entityComingFrom)));
                entitiesToExpore.UnionWith(v.Previous.Values.Where(e => !e.Equals(entityComingFrom)));
            }
            // cycle has to come before Edge because it's inheriting from edge
            else if (v is HyperGraph.Cycle)
            {
                // next here is only for metabolites
                entitiesToExpore.UnionWith(v.Next.Values.Where(e => !e.Equals(entityComingFrom)));

                // for next reactions
                foreach (var interfaceReaction in ((HyperGraph.Cycle)v).InterfaceReactions.Values)
                {
                    if (interfaceReaction.Previous.ContainsKey(entityComingFrom.Id))
                    {
                        entitiesToExpore.UnionWith(interfaceReaction.Next.Values.Where(e => !e.Equals(entityComingFrom)));
                    }
                    else
                    {
                        entitiesToExpore.UnionWith(interfaceReaction.Previous.Values.Where(e => !e.Equals(entityComingFrom)));
                    }
                }
            }
            else if (v is HyperGraph.Edge)
            {
                if (v.Previous.ContainsKey(entityComingFrom.Id))
                {
                    entitiesToExpore.UnionWith(v.Next.Values.Where(e => !e.Equals(entityComingFrom)));
                }
                else
                {
                    entitiesToExpore.UnionWith(v.Previous.Values.Where(e => !e.Equals(entityComingFrom)));
                }
            }

            foreach (var w in entitiesToExpore)
            {
                // detect a cycle
                if (marked.ContainsKey(w.Id))
                {
                    HyperGraph.Entity c = stack.Pop();
                    cycle.Add(c);
                    while (w.Id != c.Id)
                    {
                        c = stack.Pop();
                        cycle.Add(c);
                    }
                    return true;
                }

                // explore next node and return if a cycle found
                if (Search(hypergraph, w, v, marked, stack, cycle))
                {
                    return true;
                }
            }
            stack.Pop();
            marked.Remove(v.Id);
            return false;
        }


        public static HyperGraph.Cycle CollapseCycle(List<HyperGraph.Entity> cycle, HyperGraph hypergraph)
        {
            // debugging lines
            foreach (var v in cycle)
            {
                Console.WriteLine("Entity " + v.Label + " " + v.Id);
                //                Console.Write("prev:");
                //                foreach (var prev in v.Previous)
                //                {
                //                    Console.Write(prev.Value.Label + "  ");
                //                }
                //                Console.WriteLine();
                //                Console.Write("next:");
                //                foreach (var nxt in v.Next)
                //                {
                //                    Console.Write(nxt.Value.Label + "  ");
                //                }
                //                Console.WriteLine();
                //                Console.WriteLine();
            }
            Console.WriteLine();


            HyperGraph.Cycle cycleReaction = new HyperGraph.Cycle();


            // get all metabolites that are not interface metabolites and put them in cycle
            hypergraph.Nodes.Values.ToList().ForEach(n =>
            {
                if (n.Next.Union(n.Previous).All(nn => cycle.Any(e => e.Id == nn.Key)))
                {
                    cycle.Add(n);
                }
            });
            cycle = cycle.Distinct().ToList();


            // modify hypergraph
            foreach (var v in cycle)
            {
                if (v is HyperGraph.Node)
                {
                    // if this metabolite have any outside connection it should be added with 2 edges as it's consumed and produced inside the cycle
                    if (v.Next.Values.Union(v.Previous.Values).Any(e => !cycle.Contains(e)))
                    {
                        hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)v);
                        hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)v);
                    }
                    else
                    {
                        hypergraph.RemoveNode(v.Id);
                    }
                }
                // cycle should come first because it's a child of edge
                else if (v is HyperGraph.Cycle)
                {
                    // for separate metabolites
                    var outsideReactants = v.Previous.Values.Except(cycle);
                    var outsideProducts = v.Next.Values.Except(cycle);

                    foreach (var outsideProduct in outsideProducts)
                    {
                        hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)outsideProduct);
                    }
                    foreach (var outsideReactant in outsideReactants)
                    {
                        hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)outsideReactant);
                    }


                    // for inside reactions
                    foreach (var reaction in ((HyperGraph.Cycle)v).InterfaceReactions.Values)
                    {
                        if (reaction.Products.Union(reaction.Reactants).Any(m => !cycle.Contains(m.Value)))
                        {
                            foreach (var e in cycle)
                            {
                                reaction.Products.Remove(e.Id);
                                reaction.Reactants.Remove(e.Id);
                            }

                            cycleReaction.InterfaceReactions.Add(reaction.Id, reaction);
                        }
                    }

                    hypergraph.RemoveCycle((HyperGraph.Cycle)v);
                }
                else if (v is HyperGraph.Edge)
                {
                    var outsideReactants = v.Previous.Values.Except(cycle);
                    var outsideProducts = v.Next.Values.Except(cycle);

                    foreach (var outsideProduct in outsideProducts)
                    {
                        hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)outsideProduct);
                    }
                    foreach (var outsideReactant in outsideReactants)
                    {
                        hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)outsideReactant);
                    }

                    if (((HyperGraph.Edge)v).IsReversible)
                    {
                        foreach (var outsideProduct in outsideProducts)
                        {
                            hypergraph.AddReactant(cycleReaction, (HyperGraph.Node)outsideProduct);
                        }
                        foreach (var outsideReactant in outsideReactants)
                        {
                            hypergraph.AddProduct(cycleReaction, (HyperGraph.Node)outsideReactant);
                        }
                    }

                    hypergraph.RemoveReaction((HyperGraph.Edge)v);
                }
            }

            return cycleReaction;
        }




        public static void SaveAsDgs(HyperGraph.Node mi, HyperGraph graph, string dir)
        {
            var file = dir + graph.Step + "graph.dgs";
            var maxLevel = graph.LastLevel;

            var lines = new List<string> { "DGS004", "\"Metabolic Network\" 0 0", "#Nodes" };
            lines.Add(mi.ToDgs(NodeType.Selected));
            lines.AddRange(graph.Nodes.Values
                .Where(n => n.Id != mi.Id)
                .Select(node => new { node, type = node.IsBorder ? NodeType.Border : NodeType.None })
                .Select(@t => @t.node.ToDgs(@t.type)));

            lines.Add("#Hyperedges");
            foreach (var edge in graph.Edges.Values)
            {
                var type = EdgeType.None;
                if (edge.Level == maxLevel)
                    type = EdgeType.New;

                lines.Add(edge.ToDgs(type));
            }

            File.AppendAllLines(file, lines);
        }
    }
}
