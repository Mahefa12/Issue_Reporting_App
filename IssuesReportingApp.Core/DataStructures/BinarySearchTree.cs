using System;
using System.Collections.Generic;

namespace IssuesReportingApp.DataStructures
{
    public class BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Node
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public Node? Left { get; set; }
            public Node? Right { get; set; }
            public Node(TKey key, TValue value) { Key = key; Value = value; }
        }

        private Node? _root;

        public void Insert(TKey key, TValue value)
        {
            _root = Insert(_root, key, value);
        }

        private Node Insert(Node? node, TKey key, TValue value)
        {
            if (node == null) return new Node(key, value);
            int cmp = key.CompareTo(node.Key);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = value;
            return node;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            var n = _root;
            while (n != null)
            {
                int cmp = key.CompareTo(n.Key);
                if (cmp == 0) { value = n.Value; return true; }
                n = cmp < 0 ? n.Left : n.Right;
            }
            value = default!;
            return false;
        }

        public IEnumerable<(TKey key, TValue value)> InOrder()
        {
            var stack = new Stack<Node>();
            var curr = _root;
            while (stack.Count > 0 || curr != null)
            {
                if (curr != null)
                {
                    stack.Push(curr);
                    curr = curr.Left;
                }
                else
                {
                    var n = stack.Pop();
                    yield return (n.Key, n.Value);
                    curr = n.Right;
                }
            }
        }

        public IEnumerable<(TKey key, TValue value)> Range(TKey low, TKey high)
        {
            foreach (var (key, value) in InOrder())
            {
                if (key.CompareTo(low) < 0) continue;
                if (key.CompareTo(high) > 0) break;
                yield return (key, value);
            }
        }
    }
}