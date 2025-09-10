using System;
using System.Collections;

namespace IssuesReportingApp.DataStructures
{
    public class CustomStack<T> : IEnumerable<T>
    {
        private StackNode<T>? _top;
        private int _count;

        private class StackNode<TNode>
        {
            public TNode Data { get; set; }
            public StackNode<TNode>? Next { get; set; }

            public StackNode(TNode data)
            {
                Data = data;
                Next = null;
            }
        }

        public CustomStack()
        {
            _top = null;
            _count = 0;
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Push(T item)
        {
            StackNode<T> newNode = new StackNode<T>(item);
            newNode.Next = _top;
            _top = newNode;
            _count++;
        }

        public T Pop()
        {
            if (_top == null)
                throw new InvalidOperationException("Stack is empty");

            T data = _top.Data;
            _top = _top.Next;
            _count--;
            return data;
        }

        public T Peek()
        {
            if (_top == null)
                throw new InvalidOperationException("Stack is empty");

            return _top.Data;
        }

        public bool TryPop(out T result)
        {
            if (_top == null)
            {
                result = default(T)!;
                return false;
            }

            result = _top.Data;
            _top = _top.Next;
            _count--;
            return true;
        }

        public bool TryPeek(out T result)
        {
            if (_top == null)
            {
                result = default(T)!;
                return false;
            }

            result = _top.Data;
            return true;
        }

        public void Clear()
        {
            _top = null;
            _count = 0;
        }

        public bool Contains(T item)
        {
            StackNode<T>? current = _top;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Data, item))
                    return true;
                current = current.Next;
            }
            return false;
        }

        public T[] ToArray()
        {
            T[] array = new T[_count];
            int index = 0;
            StackNode<T>? current = _top;
            while (current != null)
            {
                array[index++] = current.Data;
                current = current.Next;
            }
            return array;
        }

        public List<T> ToList()
        {
            List<T> list = new List<T>();
            StackNode<T>? current = _top;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            return list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            StackNode<T>? current = _top;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}