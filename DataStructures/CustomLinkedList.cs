using System;
using System.Collections;

namespace IssuesReportingApp.DataStructures
{
    public class CustomLinkedList<T> : IEnumerable<T>
    {
        private LinkedNode<T>? _head;
        private LinkedNode<T>? _tail;
        private int _count;

        private class LinkedNode<TNode>
        {
            public TNode Data { get; set; }
            public LinkedNode<TNode>? Next { get; set; }

            public LinkedNode(TNode data)
            {
                Data = data;
                Next = null;
            }
        }

        public CustomLinkedList()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Add(T item)
        {
            LinkedNode<T> newNode = new LinkedNode<T>(item);
            
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail!.Next = newNode;
                _tail = newNode;
            }
            _count++;
        }

        public void AddFirst(T item)
        {
            LinkedNode<T> newNode = new LinkedNode<T>(item);
            
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head = newNode;
            }
            _count++;
        }

        public bool Remove(T item)
        {
            if (_head == null) return false;

            if (EqualityComparer<T>.Default.Equals(_head.Data, item))
            {
                _head = _head.Next;
                if (_head == null) _tail = null;
                _count--;
                return true;
            }

            LinkedNode<T> current = _head;
            while (current.Next != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Next.Data, item))
                {
                    if (current.Next == _tail) _tail = current;
                    current.Next = current.Next.Next;
                    _count--;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public bool Contains(T item)
        {
            LinkedNode<T>? current = _head;
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
            LinkedNode<T>? current = _head;
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
            LinkedNode<T>? current = _head;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            return list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            LinkedNode<T>? current = _head;
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