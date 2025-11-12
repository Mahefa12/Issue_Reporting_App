using System;
using System.Collections.Generic;

namespace IssuesReportingApp.DataStructures
{
    public static class MinimumSpanningTree
    {
        // Computes a spanning tree over an unweighted, undirected graph.
        // With uniform weights, any spanning tree is a minimum spanning tree.
        public static List<(T u, T v)> Compute<T>(IEnumerable<T> vertices, Func<T, IEnumerable<T>> neighbors)
        {
            var result = new List<(T u, T v)>();
            var visited = new HashSet<T>();
            var queue = new Queue<T>();

            using var enumerator = vertices.GetEnumerator();
            if (!enumerator.MoveNext()) return result;
            var start = enumerator.Current;
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                foreach (var v in neighbors(u))
                {
                    if (visited.Add(v))
                    {
                        result.Add((u, v));
                        queue.Enqueue(v);
                    }
                }
            }

            return result;
        }
    }
}