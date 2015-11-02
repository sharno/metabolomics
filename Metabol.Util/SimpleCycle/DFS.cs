using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabol.Util.SimpleCycle
{
    class DFS
    {
        public static void DetectAndCollapseCycles(Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph, HyperGraph hypergraph)
        {
            Dictionary<Guid, bool> marked = new Dictionary<Guid, bool>();
            Stack<Guid> stack = new Stack<Guid>();
            List<Guid> cycle = new List<Guid>();
            bool findingCycles = true;
            int count = 0;

            while (findingCycles)
            {
                Console.WriteLine("finding cycle " + count);
                count ++;
                findingCycles = false;
                marked.Clear();
                stack.Clear();
                cycle.Clear();
                foreach (var v in graph.Keys)
                {
                    if (Search(graph, v, marked, stack, cycle))
                    {
                        findingCycles = true;
                        Guid cycleId = Program.CollapseCycle(graph, cycle, hypergraph);
                        Program.recordToDatabase(cycleId, cycle, graph, hypergraph);
                        break;
                    }
                }
            }
        }

        public static bool Search(Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph, Guid v, Dictionary<Guid, bool> marked, Stack<Guid> stack, List<Guid> cycle)
        {
            marked[v] = true;
            stack.Push(v);
            foreach (var w in graph[v].Item2)
            {
                // detect a cycle
                if (marked.ContainsKey(w))
                {
                    Guid c = stack.Pop();
                    cycle.Add(c);
                    while (w != c)
                    {
                        c = stack.Pop();
                        cycle.Add(c);
                    }
                    return true;
                }

                if (Search(graph, w, marked, stack, cycle))
                {
                    return true;
                }
            }
            stack.Pop();
            marked.Remove(v);
            return false;
        }
    }
}
