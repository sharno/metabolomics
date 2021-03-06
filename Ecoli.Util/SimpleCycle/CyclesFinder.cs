﻿namespace Metabol.Util.SimpleCycle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class CyclesFinder
    {
        public static bool Find(Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph)
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
                return Backtrack(v, v, graph, pointStack, marked, markedStack, cycles);
                while (markedStack.Count != 0)
                {
                    Guid u = markedStack.Pop();
                    marked[u] = false;
                }
            }

//            return cycles;
            return false;
        }

        public static bool Backtrack(Guid s, Guid v, Dictionary<Guid, Tuple<HashSet<Guid>, HashSet<Guid>, bool>> graph, Stack<Guid> pointStack, Dictionary<Guid, bool> marked, Stack<Guid> markedStack, List<List<Guid>> cycles)
        {
            bool f = false;
            pointStack.Push(v);
            marked[v] = true;
            markedStack.Push(v);

            foreach (var w in graph[v].Item2.ToList())
            {
//                if (! graph.ContainsKey(w)) continue;
//                var w = graph[v].Item2
                if (w.CompareTo(s) < 0)
                {
                    //                    consumers.RemoveAt(i);
                }
                else
                if (w == s)
                {
                    // TODO remove DB recording
                    Program.recordToDatabase(Program.CollapseCycle(graph, pointStack.ToList()), pointStack.ToList());
//                    cycles.Add(pointStack.ToList());

                    f = true;
                }
                else if (!marked[w])
                {
                    f = (Backtrack(s, w, graph, pointStack, marked, markedStack, cycles) || f);
                }
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
