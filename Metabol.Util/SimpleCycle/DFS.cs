using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.Util.SimpleCycle
{
    class DFS
    {
        public static void DetectAndCollapseCycles(Graph graph, HyperGraph hypergraph)
        {
            Dictionary<Guid, bool> marked = new Dictionary<Guid, bool>();
            Stack<Guid> stack = new Stack<Guid>();
            List<Guid> cycle = new List<Guid>();
            bool findingCycles = true;
            int count = 0;

            while (findingCycles)
            {
                Core.SaveAsDgsTest(hypergraph.Nodes.First().Value, hypergraph, "C:\\Users\\sharno\\Desktop\\" + count);
                Console.WriteLine("finding cycle " + count);
                count ++;
                findingCycles = false;
                marked.Clear();
                stack.Clear();
                cycle.Clear();
                foreach (var v in graph.Nodes.Values)
                {
                    if (Search(graph, v, new Edge(null, null, false), marked, stack, cycle))
                    {
                        findingCycles = true;
                        Guid cycleId = Program.CollapseCycle(graph, cycle, hypergraph);
//                        Program.recordToDatabase(cycleId, cycle, graph, hypergraph);

                        foreach (var reaction in cycle)
                        {
                            hypergraph.RemoveReaction(hypergraph.Edges[reaction]);
                        }
                        break;
                    }
                }
            }
        }

        public static bool Search(Graph graph, Node v, Edge edgeComingFrom, Dictionary<Guid, bool> marked, Stack<Guid> stack, List<Guid> cycle)
        {
            marked[v.Id] = true;
            stack.Push(v.Id);

            // follow all next edges plus previous reversible ones but not the one we came from
            HashSet<Edge> edges = new HashSet<Edge>();
            foreach (var edge in v.Next)
            {
                if (!edge.Equals(edgeComingFrom) 
                    && edge.Destination is Reaction && ((Reaction)edge.Destination).CurrentDirection != Reaction.Backward 
                    && v is Reaction && ((Reaction)v).CurrentDirection != Reaction.Backward)
                {
                    edges.Add(edge);
                }
            }
            foreach (var edge in v.Previous)
            {
                if (!edge.Equals(edgeComingFrom) 
                    && edge.IsReversible
                    && edge.Destination is Reaction && ((Reaction)edge.Destination).CurrentDirection != Reaction.Forward
                    && v is Reaction && ((Reaction)v).CurrentDirection != Reaction.Forward)
                {
                    edges.Add(edge);
                }
            }

            foreach (var w in edges /*v.Next.Union(v.Previous.Where(edge => edge.IsReversible)).Where(edge => !edge.Equals(edgeComingFrom))*/)
            {
                // explore next node but not the same one (take reversibility into account)
                Node nodeToExplore = w.Destination.Equals(v) ? w.Source : w.Destination;
                
                // detect a cycle
                if (marked.ContainsKey(nodeToExplore.Id))
                {
                    Guid c = stack.Pop();
                    cycle.Add(c);
                    while (w.Destination.Id != c)
                    {
                        c = stack.Pop();
                        cycle.Add(c);
                    }
                    return true;
                }

                // assign direction to the to be visited reaction
                if (nodeToExplore is Reaction && ((Reaction)nodeToExplore).IsReversible)
                {
                    ((Reaction) nodeToExplore).CurrentDirection = w.Destination.Equals(nodeToExplore)
                        ? Reaction.Forward
                        : Reaction.Backward;
                }

                // explore next node and return if a cycle found
                if (Search(graph, nodeToExplore, w, marked, stack, cycle))
                {
                    return true;
                }
            }

            // assign direction to the to be visited reaction
            if (v is Reaction && ((Reaction)v).IsReversible)
            {
                ((Reaction)v).CurrentDirection = Reaction.Undecided;
            }

            stack.Pop();
            marked.Remove(v.Id);
            return false;
        }
    }
}
