using System.Collections.Generic;

namespace IssuesReportingApp.DataStructures
{
    // Wrapper over SortedDictionary, which is backed by a red-black tree.
    public class RedBlackTree<TKey, TValue>
    {
        private readonly SortedDictionary<TKey, TValue> _map = new();

        public void Insert(TKey key, TValue value)
        {
            _map[key] = value;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return _map.TryGetValue(key, out value!);
        }

        public IEnumerable<(TKey key, TValue value)> InOrder()
        {
            foreach (var kv in _map)
            {
                yield return (kv.Key, kv.Value);
            }
        }
    }
}