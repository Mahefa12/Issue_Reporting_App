using System;
using System.Collections.Generic;

namespace IssuesReportingApp.DataStructures
{
    // Directed graph with adjacency list and BFS traversal
    public class Graph<T>
    {
        private readonly Dictionary<T, List<T>> _adj = new();

        public void AddVertex(T v)
        {
            if (!_adj.ContainsKey(v)) _adj[v] = new List<T>();
        }

        public void AddEdge(T from, T to)
        {
            AddVertex(from);
            AddVertex(to);
            _adj[from].Add(to);
        }

        public IEnumerable<T> Neighbors(T v)
        {
            return _adj.TryGetValue(v, out var list) ? list : Array.Empty<T>();
        }

        public List<T> BfsPath(T start, T target)
        {
            var visited = new HashSet<T>();
            var parent = new Dictionary<T, T>();
            var q = new Queue<T>();
            q.Enqueue(start);
            visited.Add(start);

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                if (EqualityComparer<T>.Default.Equals(u, target))
                    return Reconstruct(parent, start, target);
                foreach (var w in Neighbors(u))
                {
                    if (!visited.Contains(w))
                    {
                        visited.Add(w);
                        parent[w] = u;
                        q.Enqueue(w);
                    }
                }
            }
            return new List<T>();
        }

        private List<T> Reconstruct(Dictionary<T, T> parent, T start, T target)
        {
            var path = new List<T>();
            var curr = target;
            while (!EqualityComparer<T>.Default.Equals(curr, start))
            {
                path.Add(curr);
                if (!parent.TryGetValue(curr, out var p)) break;
                curr = p;
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}