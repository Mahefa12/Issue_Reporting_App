using System;
using System.Collections.Generic;

namespace IssuesReportingApp.DataStructures
{
    public class AVLTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private class Node
        {
            public TKey Key;
            public TValue Value;
            public Node? Left;
            public Node? Right;
            public int Height;
            public Node(TKey key, TValue value)
            {
                Key = key; Value = value; Height = 1;
            }
        }

        private Node? _root;

        public void Insert(TKey key, TValue value)
        {
            _root = Insert(_root, key, value);
        }

        private int H(Node? n) => n?.Height ?? 0;
        private int Balance(Node? n) => n == null ? 0 : H(n.Left) - H(n.Right);
        private void Update(Node n) => n.Height = Math.Max(H(n.Left), H(n.Right)) + 1;

        private Node RotateRight(Node y)
        {
            var x = y.Left!;
            var T2 = x.Right;
            x.Right = y;
            y.Left = T2;
            Update(y); Update(x);
            return x;
        }

        private Node RotateLeft(Node x)
        {
            var y = x.Right!;
            var T2 = y.Left;
            y.Left = x;
            x.Right = T2;
            Update(x); Update(y);
            return y;
        }

        private Node Insert(Node? node, TKey key, TValue value)
        {
            if (node == null) return new Node(key, value);
            int cmp = key.CompareTo(node.Key);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else { node.Value = value; return node; }

            Update(node);
            int bal = Balance(node);

            // LL
            if (bal > 1 && key.CompareTo(node.Left!.Key) < 0)
                return RotateRight(node);
            // RR
            if (bal < -1 && key.CompareTo(node.Right!.Key) > 0)
                return RotateLeft(node);
            // LR
            if (bal > 1 && key.CompareTo(node.Left!.Key) > 0)
            {
                node.Left = RotateLeft(node.Left!);
                return RotateRight(node);
            }
            // RL
            if (bal < -1 && key.CompareTo(node.Right!.Key) < 0)
            {
                node.Right = RotateRight(node.Right!);
                return RotateLeft(node);
            }
            return node;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            var curr = _root;
            while (curr != null)
            {
                int cmp = key.CompareTo(curr.Key);
                if (cmp == 0) { value = curr.Value; return true; }
                curr = cmp < 0 ? curr.Left : curr.Right;
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
    }
}