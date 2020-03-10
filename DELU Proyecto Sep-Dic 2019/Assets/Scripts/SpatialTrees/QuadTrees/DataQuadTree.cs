using System.Collections.Generic;
using UnityEngine;
using Utils.Collections;
using System;

namespace Utils.SpatialTrees.QuadTrees
{

    /// <summary>
    /// DataQuadtree Node
    /// </summary>
    /// <typeparam name="T">Data type stored with points</typeparam>
    public class DataQuadTreeNode<T>
    {

        /// <summary>
        /// DataQuadtree Node Data
        /// </summary>
        public class DataQuadTreeNodeData
        {
            /// <summary>
            /// Point where is this data
            /// </summary>
            public Vector2 Point { get; private set; }

            /// <summary>
            /// Associated Data
            /// </summary>
            public T Data { get; set; }

            /// <summary>
            /// Node where this data lives in
            /// </summary>
            public DataQuadTreeNode<T> Node { get; internal set; }

            /// <summary>
            /// Create a Data Quadtree Node Data
            /// </summary>
            /// <param name="point">Point where is this data</param>
            /// <param name="data">Associated Data</param>
            /// <param name="node">Node where this data lives in</param>
            internal DataQuadTreeNodeData(in Vector2 point, T data, DataQuadTreeNode<T> node)
            {
                Point = point;
                Data = data;
                Node = node;
            }
        }

        /// <summary>
        /// Data Points inside this node
        /// </summary>
        /// <value></value>
        public List<DataQuadTreeNodeData> DataPoints { get; set; }

        /// <summary>
        /// Minimum Bound of this node
        /// </summary>
        public Vector2 MinBound { get; private set; }

        /// <summary>
        /// Maximum Bound of this node
        /// </summary>
        public Vector2 MaxBound { get; private set; }

        /// <summary>
        /// Center point of this node
        /// </summary>
        public Vector2 Center { get; private set; }

        /// <summary>
        /// Parent of this Node, if any
        /// </summary>
        public DataQuadTreeNode<T> Parent { get; private set; }

        /// <summary>
        /// Top Left Quadrant Node
        /// </summary>
        public DataQuadTreeNode<T> TopLeftNode { get; set; }

        /// <summary>
        /// Top Right Quadrant Node
        /// </summary>
        public DataQuadTreeNode<T> TopRightNode { get; set; }

        /// <summary>
        /// Bottom Left Quadrant Node
        /// </summary>
        public DataQuadTreeNode<T> BottomLeftNode { get; set; }

        /// <summary>
        /// Bottom Right Quadrant Node
        /// </summary>
        public DataQuadTreeNode<T> BottomRightNode { get; set; }

        /// <summary>
        /// Tree in which this node resides
        /// </summary>
        public DataQuadTree<T> Tree { get; private set; }

        /// <summary>
        /// Create a DataQuadTree node
        /// </summary>
        /// <param name="tree">Tree in which this node resides</param>
        /// <param name="minBound">Minimum Bound</param>
        /// <param name="maxBound">Maximum Bound</param>
        public DataQuadTreeNode(DataQuadTree<T> tree, DataQuadTreeNode<T> parent, in Vector2 minBound, in Vector2 maxBound)
        {
            MinBound = minBound;
            MaxBound = maxBound;
            Center = (MinBound / 2.0f) + (MaxBound / 2.0f); // for precision
            Parent = parent;
            Tree = tree;
            DataPoints = new List<DataQuadTreeNodeData>(Tree.MaxNodeSize);
            TopLeftNode = null;
            TopRightNode = null;
            BottomLeftNode = null;
            BottomRightNode = null;
        }

        /// <summary>
        /// Get Closest Point inside this node
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>Closest Point inside this node</returns>
        public Vector2 GetClosestPointInside(in Vector2 point)
        {
            return new Vector2(
                Mathf.Clamp(point.x, MinBound.x, MaxBound.x),
                Mathf.Clamp(point.y, MinBound.y, MaxBound.y)
            );
        }

        /// <summary>
        /// If this node has children
        /// </summary>
        /// <returns>If this node has children</returns>
        public bool HasChildren()
        {
            return TopLeftNode != null; // If one exists all exists
        }

        /// <summary>
        /// If this node is empty
        /// </summary>
        /// <returns>If this node is empty</returns>
        public bool IsEmpty()
        {
            return DataPoints == null || DataPoints.Count == 0;
        }

        /// <summary>
        /// If the point is inside the node
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>If the point is inside the node</returns>
        public bool IsInside(in Vector2 point)
        {
            return (MinBound.x <= point.x) && (point.x <= MaxBound.x) &&
                (MinBound.y <= point.y) && (point.y <= MaxBound.y);
        }

        /// <summary>
        /// Inserts a Data inside this node
        /// This function assumes that the data point is inside, must be checked before
        /// </summary>
        /// <param name="data">Data to Insert</param>
        /// <returns>Inserted Data</returns>
        public DataQuadTreeNodeData Insert(DataQuadTreeNodeData data)
        {
            /// If we are a leaf
            if (DataPoints != null)
            {
                // If we still have capacity
                // TODO: Add min area support
                if (DataPoints.Count != Tree.MaxNodeSize)
                {
                    data.Node = this;
                    DataPoints.Add(data);
                    return data;
                }
                // If we are full
                else
                {
                    TopLeftNode = new DataQuadTreeNode<T>(
                        Tree, this, new Vector2(MinBound.x, Center.y), new Vector2(Center.x, MaxBound.y)
                    );
                    TopRightNode = new DataQuadTreeNode<T>(
                        Tree, this, Center, MaxBound
                    );
                    BottomLeftNode = new DataQuadTreeNode<T>(
                        Tree, this, MinBound, Center
                    );
                    BottomRightNode = new DataQuadTreeNode<T>(
                        Tree, this, new Vector2(Center.x, MinBound.y), new Vector2(MaxBound.x, Center.y)
                    );

                    for (int i = 0; i < DataPoints.Count; i++)
                    {
                        InternalChildrenInsert(DataPoints[i]);
                    }
                    DataPoints = null;
                    return InternalChildrenInsert(data);
                }
            }
            // this node was full, we have to insert on children
            {
                return InternalChildrenInsert(data);
            }

        }

        /// <summary>
        /// Inserts a Data inside this node children
        /// This function assumes that the data point is inside, must be checked before
        /// </summary>
        /// <param name="data">Data to Insert</param>
        /// <returns>Inserted Data</returns>
        private DataQuadTreeNodeData InternalChildrenInsert(DataQuadTreeNodeData data)
        {
            // If we are on the top quadrants
            if (Center.y < data.Point.y)
            {
                // If we are on the right quadrants 
                if (Center.x < data.Point.x)
                {
                    return TopRightNode.Insert(data);
                }
                // we are on the left quadrants
                else
                {
                    return TopLeftNode.Insert(data);
                }
            }
            // we are on the bottom quadrants
            else
            {
                // If we are on the right quadrants 
                if (Center.x < data.Point.x)
                {
                    return BottomRightNode.Insert(data);
                }
                // we are on the left quadrants
                else
                {
                    return BottomLeftNode.Insert(data);
                }
            }
        }

        /// <summary>
        /// Checks if all the node children are empty to make this node available again
        /// </summary>
        private void FixForEmptyChildren()
        {
            if (TopRightNode != null &&
                TopRightNode.IsEmpty() &&
                TopLeftNode.IsEmpty() &&
                BottomRightNode.IsEmpty() &&
                BottomLeftNode.IsEmpty())
            {
                DataPoints = new List<DataQuadTreeNodeData>(Tree.MaxNodeSize);
                Parent.FixForEmptyChildren();
            }
        }

        /// <summary>
        /// Delete Data from this node.
        /// Remove the first instance of the Data
        /// </summary>
        /// <param name="data">Data to Remove</param>
        /// <returns>if data is successfully removed; otherwise, false</returns>
        public bool DeleteData(DataQuadTreeNodeData data)
        {
            if (DataPoints != null && DataPoints.Remove(data))
            {
                if (IsEmpty() && Parent != null)
                {
                    Parent.FixForEmptyChildren();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete Data from this node.
        /// Remove all the instances of the Data
        /// </summary>
        /// <param name="data">Data to Remove</param>
        /// <returns>if data is successfully removed; otherwise, false</returns>
        public bool DeleteDataAll(DataQuadTreeNodeData data)
        {
            if (DataPoints != null && DataPoints.RemoveAll(storedData => storedData == data) != 0)
            {
                if (IsEmpty() && Parent != null)
                {
                    Parent.FixForEmptyChildren();
                }
                return true;
            }
            return false;
        }

    }

    /// <summary>
    /// Data Quadtree
    /// </summary>
    /// <typeparam name="T">Data Type</typeparam>
    public class DataQuadTree<T>
    {

        /// <summary>
        /// Max Node Data Size
        /// </summary>
        public int MaxNodeSize { get; private set; }

        /// <summary>
        /// Standard Max node Size
        /// </summary>
        const int kMaxNodeSizeStandard = 8;

        /// <summary>
        /// Root of the tree
        /// </summary>
        public DataQuadTreeNode<T> Root { get; private set; } = null;

        /// <summary>
        /// Create a Data Quadtree
        /// </summary>
        /// <param name="minBound">Minimum Bound</param>
        /// <param name="maxBound">Maximum Bound</param>
        /// <param name="maxNodeSize">Max amount of data in a sigle node</param>
        public DataQuadTree(in Vector2 minBound, in Vector2 maxBound, int maxNodeSize = kMaxNodeSizeStandard)
        {
            MaxNodeSize = maxNodeSize;
            Root = new DataQuadTreeNode<T>(this, null, minBound, maxBound);
        }

        /// <summary>
        /// Insert a point and its associated data, if they are in the bounds of the Quadtree
        /// </summary>
        /// <param name="point">Point to insert</param>
        /// <param name="data">Data to insert</param>
        /// <returns>DataNode Created, if any</returns>
        public DataQuadTreeNode<T>.DataQuadTreeNodeData Insert(in Vector2 point, T data)
        {
            if (Root.IsInside(point))
            {
                return Root.Insert(new DataQuadTreeNode<T>.DataQuadTreeNodeData(point, data, null));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Distance to Node
        /// </summary>
        private class DistanceToNode
        {
            /// <summary>
            /// Squared Distance to the closest point inside the Node
            /// </summary>
            public float SqrClosestDistance { get; private set; }

            /// <summary>
            /// Associated node
            /// </summary>
            public DataQuadTreeNode<T> Node { get; private set; }

            /// <summary>
            /// Create a DistanceToNode from a node and a point
            /// </summary>
            /// <param name="node">Node to use</param>
            /// <param name="point">Point to use</param>
            public DistanceToNode(DataQuadTreeNode<T> node, in Vector2 point)
            {
                Node = node;
                SqrClosestDistance = (Node.GetClosestPointInside(point) - point).SqrMagnitude();
            }
        }

        /// <summary>
        /// Closest to node comparator
        /// </summary>
        private class ClosestToNode : Comparer<DistanceToNode>
        {
            public override int Compare(DistanceToNode x, DistanceToNode y)
            {
                return x.SqrClosestDistance.CompareTo(y.SqrClosestDistance);
            }
        }

        // <summary>
        /// Distance to DataPoint
        /// </summary>
        public class DistanceToDataPoint
        {
            /// <summary>
            /// Squared Distance to a point
            /// </summary>
            public float SqrClosestDistance { get; private set; }

            /// <summary>
            /// Associated DataPoint
            /// </summary>
            public DataQuadTreeNode<T>.DataQuadTreeNodeData DataNode { get; private set; }

            /// <summary>
            /// Create a DistanceToDataPoint using a dominating value for distance (i.e MaxFloat)
            /// </summary>
            /// <param name="maxDominatingDistance">Dominating value for distance</param>
            public DistanceToDataPoint(float maxDominatingDistance)
            {
                DataNode = null;
                SqrClosestDistance = maxDominatingDistance;
            }

            /// <summary>
            /// Create a DistanceToDataNode from a data node and a point
            /// </summary>
            /// <param name="dataNode">DataNode to use</param>
            /// <param name="point">Point to use</param>
            public DistanceToDataPoint(DataQuadTreeNode<T>.DataQuadTreeNodeData dataNode, in Vector2 point)
            {
                DataNode = dataNode;
                SqrClosestDistance = (dataNode.Point - point).SqrMagnitude();
            }
        }

        /// <summary>
        /// Closest to data node comparator
        /// </summary>
        public class ClosestToDataPoint : Comparer<DistanceToDataPoint>
        {
            public override int Compare(DistanceToDataPoint x, DistanceToDataPoint y)
            {
                return x.SqrClosestDistance.CompareTo(y.SqrClosestDistance);
            }
        }

        /// <summary>
        /// Initial Capacity of Closest Node Heap, to avoid allocations
        /// </summary>
        private const int kInitialCapacityOfClosestNodeHeap = 512;

        /// <summary>
        /// Maximum Distance to a DataPoint
        /// </summary>
        private static readonly DistanceToDataPoint kMaxDistanceToDataPoint = new DistanceToDataPoint(float.PositiveInfinity);

        /// <summary>
        /// Closest Node Heap to reuse memory
        /// </summary>
        private MinHeap<DistanceToNode> closestNodeHeap = null;

        /// <summary>
        /// Calculates the nearest neighbor to a point
        /// </summary>
        /// <param name="point">Point to Calculate Nearest Neighbor</param>
        /// <returns>The nearest neighbor to a point, if any</returns>
        public DistanceToDataPoint NearestNeighbor(in Vector2 point)
        {
            DistanceToDataPoint nearestDataPoint = kMaxDistanceToDataPoint;

            if (closestNodeHeap == null)
            {
                MinHeap<DistanceToNode> closestNodeHeap = new MinHeap<DistanceToNode>(ClosestToNode.Default)
                {
                    new DistanceToNode(Root, point)
                };
                closestNodeHeap.SetCapacity(kInitialCapacityOfClosestNodeHeap, true);
            }
            else
            {
                closestNodeHeap.Clear();
                closestNodeHeap.Add(new DistanceToNode(Root, point));
            }

            // Temps
            DataQuadTreeNode<T> node;
            List<DataQuadTreeNode<T>.DataQuadTreeNodeData> dataPoints;
            int dataPointIter;
            DistanceToNode possibleNode;
            DistanceToDataPoint possibleDataPoint;

            while (!closestNodeHeap.IsEmpty())
            {
                DistanceToNode currentNode = closestNodeHeap.ExtractDominating();
                if (nearestDataPoint.SqrClosestDistance < currentNode.SqrClosestDistance)
                {
                    // Is further away than current best
                    continue;
                }

                node = currentNode.Node;

                if (node.HasChildren())
                {
                    // Not a leaf
                    possibleNode = new DistanceToNode(node.TopLeftNode, point);
                    if (possibleNode.SqrClosestDistance < nearestDataPoint.SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    possibleNode = new DistanceToNode(node.TopRightNode, point);
                    if (possibleNode.SqrClosestDistance < nearestDataPoint.SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    possibleNode = new DistanceToNode(node.BottomLeftNode, point);
                    if (possibleNode.SqrClosestDistance < nearestDataPoint.SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    possibleNode = new DistanceToNode(node.BottomRightNode, point);
                    if (possibleNode.SqrClosestDistance < nearestDataPoint.SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    continue;
                }
                else
                {
                    // Leaf
                    dataPoints = node.DataPoints;

                    for (dataPointIter = 0; dataPointIter < dataPoints.Count; dataPointIter++)
                    {
                        possibleDataPoint = new DistanceToDataPoint(dataPoints[dataPointIter], point);
                        if (possibleDataPoint.SqrClosestDistance < nearestDataPoint.SqrClosestDistance)
                        {
                            nearestDataPoint = possibleDataPoint;
                        }
                    }
                }

            }
            return nearestDataPoint;
        }

        /// <summary>
        /// Calculates the k nearest neighbors to a point. If the storage is smaller than k, then it'll only use the full storage and no more
        /// </summary>
        /// <param name="point">Point to Calculate Nearest Neighbor</param>
        /// <param name="k">How many neighbors to search for</param>
        /// <param name="storage">Storage to use, to avoid allocations</param>
        /// <returns>Number of neighbors found, may be 0</returns>
        public int KNearestNeighbor(in Vector2 point, int k, in DistanceToDataPoint[] storage)
        {
            if (storage.Length < k)
            {
                k = storage.Length;
            }

            MaxHeap<DistanceToDataPoint> nearestDataPoints = new MaxHeap<DistanceToDataPoint>(ClosestToDataPoint.Default);
            nearestDataPoints.UseExternalStorage(storage);
            nearestDataPoints.Add(kMaxDistanceToDataPoint);

            if (closestNodeHeap == null)
            {
                MinHeap<DistanceToNode> closestNodeHeap = new MinHeap<DistanceToNode>(ClosestToNode.Default)
                {
                    new DistanceToNode(Root, point)
                };
                closestNodeHeap.SetCapacity(kInitialCapacityOfClosestNodeHeap, true);
            }
            else
            {
                closestNodeHeap.Clear();
                closestNodeHeap.Add(new DistanceToNode(Root, point));
            }

            // Temps
            DataQuadTreeNode<T> node;
            List<DataQuadTreeNode<T>.DataQuadTreeNodeData> dataPoints;
            int dataPointIter;
            DistanceToNode possibleNode;
            DistanceToDataPoint possibleDataPoint;

            while (!closestNodeHeap.IsEmpty())
            {
                DistanceToNode currentNode = closestNodeHeap.ExtractDominating();
                if (nearestDataPoints.Count == k && nearestDataPoints.GetTop().SqrClosestDistance < currentNode.SqrClosestDistance)
                {
                    // Is further away than current best
                    continue;
                }

                node = currentNode.Node;

                if (node.HasChildren())
                {
                    // Not a leaf
                    possibleNode = new DistanceToNode(node.TopLeftNode, point);
                    if (nearestDataPoints.Count < k || possibleNode.SqrClosestDistance < nearestDataPoints.GetTop().SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    possibleNode = new DistanceToNode(node.TopRightNode, point);
                    if (nearestDataPoints.Count < k || possibleNode.SqrClosestDistance < nearestDataPoints.GetTop().SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    possibleNode = new DistanceToNode(node.BottomLeftNode, point);
                    if (nearestDataPoints.Count < k || possibleNode.SqrClosestDistance < nearestDataPoints.GetTop().SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    possibleNode = new DistanceToNode(node.BottomRightNode, point);
                    if (nearestDataPoints.Count < k || possibleNode.SqrClosestDistance < nearestDataPoints.GetTop().SqrClosestDistance)
                    {
                        closestNodeHeap.Add(possibleNode);
                    }

                    continue;
                }
                else
                {
                    // Leaf
                    dataPoints = node.DataPoints;

                    for (dataPointIter = 0; dataPointIter < dataPoints.Count; dataPointIter++)
                    {
                        possibleDataPoint = new DistanceToDataPoint(dataPoints[dataPointIter], point);
                        if (nearestDataPoints.Count == k)
                        {
                            // We found k neighbors
                            if (possibleDataPoint.SqrClosestDistance < nearestDataPoints.GetTop().SqrClosestDistance)
                            {
                                nearestDataPoints.ExtractDominating(); // Found someone better than the worst neighbor
                                nearestDataPoints.Add(possibleDataPoint);
                            }
                        }
                        else
                        {
                            // We still need more neighbors
                            nearestDataPoints.Add(possibleDataPoint);
                        }

                    }
                }

            }
            if (nearestDataPoints.GetTop().DataNode == null)
            {
                return nearestDataPoints.Count - 1; // Maximum Dominating DataPoint still inside, meaning less than k neighbors
            }
            else
            {
                return nearestDataPoints.Count;
            }
        }
    }
}