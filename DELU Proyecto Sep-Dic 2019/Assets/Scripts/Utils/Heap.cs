using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Collections
{
    /// <summary>
    /// Heap Abstract Class
    /// Based of https://stackoverflow.com/a/13776636
    /// </summary>
    /// <typeparam name="T">Data Type to store</typeparam>
    public abstract class Heap<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initial Capacity for all Heaps
        /// </summary>
        private const int InitialCapacity = 0;

        /// <summary>
        /// Growth factor when the heap is full
        /// </summary>
        private const int GrowthFactor = 2;

        /// <summary>
        /// Minimal Growth that will happen, regardless of GrowthFactor
        /// </summary>
        private const int MinGrow = 1;

        /// <summary>
        /// Heap Array Storage
        /// </summary>
        private T[] _heap = new T[InitialCapacity];

        /// <summary>
        /// If this heap is using external Storage
        /// </summary>
        public bool UsingExternalStorage { get; private set; } = false;

        /// <summary>
        /// Amount of Elements in Heap. As well as its Tail
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Current Capacity of this Heap
        /// </summary>
        public int Capacity { get; private set; } = InitialCapacity;

        /// <summary>
        /// Comparer to use for order
        /// </summary>
        protected Comparer<T> Comparer { get; private set; }

        /// <summary>
        /// If the Left Operand Dominates the Right Operand under the current Comparer
        /// </summary>
        /// <param name="lhs">Left Operand</param>
        /// <param name="rhs">Right Operand</param>
        /// <returns>If the Left Operand Dominates the Right Operand under the current Comparer</returns>
        protected abstract bool Dominates(in T lhs, in T rhs);

        /// <summary>
        /// Create a new a Heap using the Default Comparer for type T
        /// </summary>
        protected Heap() : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Create a new Heap using the provided Comparer for type T
        /// </summary>
        /// <param name="comparer">Comparer to use</param>
        protected Heap(Comparer<T> comparer) : this(Enumerable.Empty<T>(), comparer)
        {
        }

        /// <summary>
        /// Create a new Heap from another collection using the Default Comparer for type T
        /// </summary>
        /// <param name="collection">Collection to extract data</param>
        protected Heap(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Create a new Heap from another collection using the provided Comparer for type T
        /// </summary>
        /// <param name="collection">Collection to extract data</param>
        /// <param name="comparer">Comparer to use</param>
        protected Heap(IEnumerable<T> collection, Comparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            Comparer = comparer;

            foreach (var item in collection)
            {
                if (Count == Capacity)
                {
                    Grow();
                }

                _heap[Count++] = item;
            }

            for (int i = Parent(Count - 1); i >= 0; --i)
            {
                BubbleDown(i);
            }
        }

        /// <summary>
        /// If the Heap is empty
        /// </summary>
        /// <returns>True if the Heap is empty</returns>
        public bool IsEmpty()
        {
            return Count == 0;
        }

        /// <summary>
        /// Add a new item to the Heap
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(in T item)
        {
            if (Count == Capacity)
            {
                Grow();
            }

            _heap[Count++] = item;
            BubbleUp(Count - 1);
        }

        /// <summary>
        /// Fix Node Order againts its parent
        /// </summary>
        /// <param name="i">Index of Node to Fix</param>
        private void BubbleUp(int i)
        {
            if (i == 0 || Dominates(_heap[Parent(i)], _heap[i]))
            {
                return; //correct domination (or root)
            }

            Swap(i, Parent(i));
            BubbleUp(Parent(i));
        }

        /// <summary>
        /// Get the Reference to Dominating Node
        /// </summary>
        /// <returns>Dominating Node</returns>
        public ref T GetTop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }
            return ref _heap[0];
        }

        /// <summary>
        /// Extracs the Dominating Node  and remove it from the heap
        /// </summary>
        /// <returns>Dominating Node</returns>
        public T ExtractDominating()
        {
            if (Count == 0) { throw new InvalidOperationException("Heap is empty"); }
            T ret = _heap[0];
            _heap[0] = default; // Null/Default it
            Count--;
            Swap(Count, 0);
            BubbleDown(0);
            return ret;
        }

        /// <summary>
        /// Fix current node and its children
        /// </summary>
        /// <param name="i"></param>
        private void BubbleDown(int i)
        {
            int dominatingNode = Dominating(i);
            if (dominatingNode == i)
            {
                return;
            }

            Swap(i, dominatingNode);
            BubbleDown(dominatingNode);
        }

        /// <summary>
        /// Get the Dominating node between itself and its children
        /// </summary>
        /// <param name="i">Node to check</param>
        /// <returns>Dominating Node</returns>
        private int Dominating(int i)
        {
            int dominatingNode = i;
            dominatingNode = GetDominating(YoungChild(i), dominatingNode);
            dominatingNode = GetDominating(OldChild(i), dominatingNode);

            return dominatingNode;
        }

        /// <summary>
        /// Determines the dominating node between two nodes
        /// </summary>
        /// <param name="newNode">New Node</param>
        /// <param name="dominatingNode">Node to be expected to be Dominating</param>
        /// <returns>Dominating Node</returns>
        private int GetDominating(int newNode, int dominatingNode)
        {
            if (newNode < Count && !Dominates(_heap[dominatingNode], _heap[newNode]))
            {
                return newNode;
            }
            else
            {
                return dominatingNode;
            }
        }

        /// <summary>
        /// Swap two nodes
        /// </summary>
        /// <param name="i">First Node</param>
        /// <param name="j">Second Node</param>
        private void Swap(int i, int j)
        {
            T tmp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = tmp;
        }

        /// <summary>
        /// Parent of the Current Node
        /// </summary>
        /// <param name="i">Current Node</param>
        /// <returns>Parent of Current Node</returns>
        private static int Parent(int i)
        {
            return (i + 1) / 2 - 1;
        }

        /// <summary>
        /// Get Less Child
        /// </summary>
        /// <param name="i">Current Node</param>
        /// <returns>Less Child</returns>
        private static int YoungChild(int i)
        {
            return (i + 1) * 2 - 1;
        }

        /// <summary>
        /// Get More Child
        /// </summary>
        /// <param name="i">Current Node</param>
        /// <returns>More Child</returns>
        private static int OldChild(int i)
        {
            return YoungChild(i) + 1;
        }

        /// <summary>
        /// Grow Heap for more Capacity
        /// </summary>
        private void Grow()
        {
            if (UsingExternalStorage)
            {
                throw new InvalidOperationException("Ran out of External Storage");
            }
            int newCapacity = Capacity * GrowthFactor + MinGrow;
            var newHeap = new T[newCapacity];
            Array.Copy(_heap, newHeap, Capacity);
            _heap = newHeap;
            Capacity = newCapacity;
        }

        /// <summary>
        /// Set Capacity of the Heap. New Capacity must be higher than current Count, unless forced. If forced, the heap starts from scratch
        /// </summary>
        /// <param name="newCapacity">New Capacity to use</param>
        /// <param name="force">Force the heap to start from scratch. Use this to replace external storage</param>
        public void SetCapacity(int newCapacity, bool force = false)
        {
            if (UsingExternalStorage && !force)
            {
                throw new InvalidOperationException("Trying to Replace External Storage without Force");
            }
            else
            {
                UsingExternalStorage = false;
            }

            if (newCapacity < Count)
            {
                if (!force)
                {
                    throw new InvalidOperationException($"New Capacity {newCapacity} is lower than current Count {Count}");
                }
                else
                {
                    var newHeap = new T[newCapacity];
                    _heap = newHeap;
                    Capacity = newCapacity;
                    Count = 0;
                }
            }
            else
            {
                var newHeap = new T[newCapacity];
                Array.Copy(_heap, newHeap, Capacity);
                _heap = newHeap;
                Capacity = newCapacity;
            }
        }

        /// <summary>
        /// Clears the Heap
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Use External Storage for Heap
        /// </summary>
        /// <param name="newHeap">New Storage to use</param>
        public void UseExternalStorage(in T[] newHeap)
        {
            UsingExternalStorage = true;
            _heap = newHeap;
            Capacity = newHeap.Length;
            Count = 0;
        }

        /// <summary>
        /// Get Enumerator of Heap. It is not ordered
        /// </summary>
        /// <returns>Enumerator of Heap</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _heap.Take(Count).GetEnumerator();
        }

        /// <summary>
        /// Get Enumerator of Heap. It is not ordered
        /// </summary>
        /// <returns>Enumerator of Heap</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Max Heap
    /// </summary>
    /// <typeparam name="T">Data Type to store</typeparam>
    public class MaxHeap<T> : Heap<T>
    {
        /// <summary>
        /// Create a new Max Heap using the Default Comparer for type T
        /// </summary>
        public MaxHeap()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Create a new Max Heap using the provided Comparer for type T
        /// </summary>
        /// <param name="comparer">Comparer to use</param>
        public MaxHeap(Comparer<T> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Create a new Max Heap from another collection using the provided Comparer for type T
        /// </summary>
        /// <param name="collection">Collection to extract data</param>
        /// <param name="comparer">Comparer to use</param>
        public MaxHeap(IEnumerable<T> collection, Comparer<T> comparer)
            : base(collection, comparer)
        {
        }

        /// <summary>
        /// Create a new Max Heap from another collection using the Default Comparer for type T
        /// </summary>
        /// <param name="collection">Collection to extract data</param>
        public MaxHeap(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// If the Left Operand is bigger than the Right Operand under the current Comparer
        /// </summary>
        /// <param name="lhs">Left Operand</param>
        /// <param name="rhs">Right Operand</param>
        /// <returns>If the Left Operand is bigger than the Right Operand under the current Comparer</returns>
        protected override bool Dominates(in T lhs, in T rhs)
        {
            return Comparer.Compare(lhs, rhs) >= 0;
        }
    }

    public class MinHeap<T> : Heap<T>
    {

        /// <summary>
        /// Create a new Min Heap using the Default Comparer for type T
        /// </summary>
        public MinHeap()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Create a new Min Heap using the provided Comparer for type T
        /// </summary>
        /// <param name="comparer">Comparer to use</param>
        public MinHeap(Comparer<T> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Create a new Min Heap from another collection using the Default Comparer for type T
        /// </summary>
        /// <param name="collection">Collection to extract data</param>
        public MinHeap(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Create a new Max Heap from another collection using the provided Comparer for type T
        /// </summary>
        /// <param name="collection">Collection to extract data</param>
        /// <param name="comparer">Comparer to use</param>
        public MinHeap(IEnumerable<T> collection, Comparer<T> comparer)
            : base(collection, comparer)
        {
        }

        /// <summary>
        /// If the Left Operand is less than the Right Operand under the current Comparer
        /// </summary>
        /// <param name="lhs">Left Operand</param>
        /// <param name="rhs">Right Operand</param>
        /// <returns>If the Left Operand is less than the Right Operand under the current Comparer</returns>
        protected override bool Dominates(in T lhs, in T rhs)
        {
            return Comparer.Compare(lhs, rhs) <= 0;
        }
    }
}
