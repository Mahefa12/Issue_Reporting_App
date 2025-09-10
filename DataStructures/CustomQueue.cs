using System;
using System.Collections;

namespace IssuesReportingApp.DataStructures
{
    public class CustomQueue<T> : IEnumerable<T>
    {
        private QueueNode<T>? _front;
        private QueueNode<T>? _rear;
        private int _count;

        private class QueueNode<TNode>
        {
            public TNode Data { get; set; }
            public QueueNode<TNode>? Next { get; set; }

            public QueueNode(TNode data)
            {
                Data = data;
                Next = null;
            }
        }

        public CustomQueue()
        {
            _front = null;
            _rear = null;
            _count = 0;
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Enqueue(T item)
        {
            QueueNode<T> newNode = new QueueNode<T>(item);
            
            if (_rear == null)
            {
                _front = newNode;
                _rear = newNode;
            }
            else
            {
                _rear.Next = newNode;
                _rear = newNode;
            }
            _count++;
        }

        public T Dequeue()
        {
            if (_front == null)
                throw new InvalidOperationException("Queue is empty");

            T data = _front.Data;
            _front = _front.Next;
            
            if (_front == null)
                _rear = null;
                
            _count--;
            return data;
        }

        public T Peek()
        {
            if (_front == null)
                throw new InvalidOperationException("Queue is empty");

            return _front.Data;
        }

        public bool TryDequeue(out T result)
        {
            if (_front == null)
            {
                result = default(T)!;
                return false;
            }

            result = _front.Data;
            _front = _front.Next;
            
            if (_front == null)
                _rear = null;
                
            _count--;
            return true;
        }

        public bool TryPeek(out T result)
        {
            if (_front == null)
            {
                result = default(T)!;
                return false;
            }

            result = _front.Data;
            return true;
        }

        public void Clear()
        {
            _front = null;
            _rear = null;
            _count = 0;
        }

        public bool Contains(T item)
        {
            QueueNode<T>? current = _front;
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
            QueueNode<T>? current = _front;
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
            QueueNode<T>? current = _front;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            return list;
        }

        // Method to get a random item (for encouraging messages)
        public T GetRandomItem(Random random)
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            int targetIndex = random.Next(_count);
            int currentIndex = 0;
            QueueNode<T>? current = _front;
            
            while (current != null && currentIndex < targetIndex)
            {
                current = current.Next;
                currentIndex++;
            }
            
            return current!.Data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            QueueNode<T>? current = _front;
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