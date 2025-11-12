using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingApp.DataStructures
{
    public class CustomPriorityQueue<T> where T : IComparable<T>
    {
        private List<T> heap;

        public CustomPriorityQueue()
        {
            heap = new List<T>();
        }

        public int Count => heap.Count;
        public bool IsEmpty => heap.Count == 0;

        public void Enqueue(T item)
        {
            heap.Add(item);
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Priority queue is empty");

            T result = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
                HeapifyDown(0);

            return result;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Priority queue is empty");

            return heap[0];
        }

        public bool TryDequeue(out T item)
        {
            if (IsEmpty)
            {
                item = default(T);
                return false;
            }

            item = Dequeue();
            return true;
        }

        public bool TryPeek(out T item)
        {
            if (IsEmpty)
            {
                item = default(T);
                return false;
            }

            item = heap[0];
            return true;
        }

        public void Clear()
        {
            heap.Clear();
        }

        public T[] ToArray()
        {
            return heap.ToArray();
        }

        public List<T> ToList()
        {
            return new List<T>(heap);
        }

        public bool Contains(T item)
        {
            return heap.Contains(item);
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[index].CompareTo(heap[parentIndex]) >= 0)
                    break;

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild < heap.Count && heap[leftChild].CompareTo(heap[smallest]) < 0)
                    smallest = leftChild;

                if (rightChild < heap.Count && heap[rightChild].CompareTo(heap[smallest]) < 0)
                    smallest = rightChild;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }
    }
}