using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabol;

namespace CyclesCacher.SimpleCycle
{
    class CyclesFinder
    {
        public static List<List<Guid>> Find(Dictionary<Guid, HashSet<Guid>> graph)
        {
            List<List<Guid>> cycles = new List<List<Guid>>();
            Stack<Guid> pointStack = new Stack<Guid>();
            Dictionary<Guid, bool> marked = new Dictionary<Guid, bool>();
            Stack<Guid> markedStack = new Stack<Guid>();



            foreach (var v in graph.Keys)
            {
                marked[v] = false;
            }

            foreach (var v in graph.Keys)
            {
                Backtrack(v, v, graph, pointStack, marked, markedStack, cycles);
                while (markedStack.Count != 0)
                {
                    Guid u = markedStack.Pop();
                    marked[u] = false;
                }
            }

            return cycles;
        }

        public static bool Backtrack(Guid s, Guid v, Dictionary<Guid, HashSet<Guid>> graph, Stack<Guid> pointStack, Dictionary<Guid, bool> marked, Stack<Guid> markedStack, List<List<Guid>> cycles)
        {
            bool f = false;
            pointStack.Push(v);
            marked[v] = true;
            markedStack.Push(v);

            foreach (var w in graph[v])
            {
                //                List<HyperGraph.Edge> consumers = product.Value.Consumers.ToList();
                //                for (int i = consumers.Count-1; i >= 0; i--)
                if (w.CompareTo(s) < 0)
                {
//                    consumers.RemoveAt(i);
                }
                else
                if (w == s)
                {
                    // TODO remove DB recording
//                                            CyclesCacher.Program.recordToDatabase(pointStack.ToList());
                    cycles.Add(pointStack.ToList());
                    f = true;
                }
                else if (!marked[w])
                {
                    f = (Backtrack(s, w, graph, pointStack, marked, markedStack, cycles) || f);
                }
                //                product.Value.Consumers = new HashSet<HyperGraph.Edge>(consumers);
            }

            if (f)
            {
                while (markedStack.Peek() != v)
                {
                    Guid u = markedStack.Pop();
                    marked[u] = false;
                }
                markedStack.Pop();
                marked[v] = false;
            }
            pointStack.Pop();
            return f;
        }



        //point_stack = list()
        //marked = dict()
        //marked_stack = list()

        //def backtrack(v):
        //    f = False
        //    point_stack.append(v)
        //    marked[v] = True
        //    marked_stack.append(v)
        //    for w in A[v]:
        //        if w<s:
        //            A[w] = 0
        //        elif w==s:
        //            print_point_stack()
        //            f = True
        //        elif not marked[w]:
        //            f = backtrack(w) or f
        //    if f:
        //        while marked_stack[-1] != v:
        //            u = marked_stack.pop()
        //            marked[u] = False
        //        marked_stack.pop()
        //        marked[v] = False
        //    point_stack.pop()
        //    return f

        //for i in range(len(A)):
        //    marked[i] = False

        //for s in range(len(A)):
        //    backtrack(s)
        //    while marked_stack:
        //        u = marked_stack.pop()
        //        marked[u] = False
    }
}
