using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol.Util.DB2;

namespace Metabol.Util.SimpleCycle
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
                //Core.SaveAsDgsTest(hypergraph.Nodes.First().Value, hypergraph, "C:\\Users\\sharno\\Desktop\\" + count);
                Console.WriteLine("finding cycle " + count);
                count++;
                findingCycles = false;
                marked.Clear();
                stack.Clear();
                cycle = new List<HyperGraph.Entity>();
                foreach (var v in hypergraph.Nodes.Values)
                {
                    if (Search(hypergraph, v, new HyperGraph.Edge(Guid.NewGuid(), 0), marked, stack, cycle))
                    {
                        findingCycles = true;
                        var cycleReaction = Program.CollapseCycle(cycle, hypergraph);
                        cycles.Add(cycleReaction, cycle);
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
                entitiesToExpore.UnionWith(v.Previous.Values.Where(e => ((HyperGraph.Edge)e).IsReversible && !e.Equals(entityComingFrom)));
            }
            // cycle has to come before Edge because it's inheriting from edge
            else if (v is HyperGraph.Cycle)
            {
                // next here is only for metabolites
                entitiesToExpore.UnionWith(v.Next.Values.Where(e => !e.Equals(entityComingFrom)));

                // for next reactions
                foreach (var interfaceReaction in ((HyperGraph.Cycle)v).InterfaceReactions.Values)
                {
                    if (interfaceReaction.IsReversible)
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
                    else
                    {
                        entitiesToExpore.UnionWith(interfaceReaction.Next.Values.Where(e => !e.Equals(entityComingFrom)));
                    }
                }
            }
            else if (v is HyperGraph.Edge)
            {
                if (((HyperGraph.Edge)v).IsReversible)
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
                else
                {
                    entitiesToExpore.UnionWith(v.Next.Values.Where(e => !e.Equals(entityComingFrom)));
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
    }
}
