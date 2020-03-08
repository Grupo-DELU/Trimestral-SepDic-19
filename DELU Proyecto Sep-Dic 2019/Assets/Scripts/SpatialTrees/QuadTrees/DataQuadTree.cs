using System.Collections.Generic;
using UnityEngine;

namespace DELU_Proyecto_Sep_Dic_2019.Assets.Scripts.SpatialTrees.QuadTrees {

    /// <summary>
    /// DataQuadtree Node
    /// </summary>
    /// <typeparam name="T">Data type stored with points</typeparam>
    public class DataQuadTreeNode<T> {

        /// <summary>
        /// DataQuadtree Node Data
        /// </summary>
        public class DataQuadTreeNodeData {
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
            public DataQuadTreeNodeData(Vector2 point, T data, DataQuadTreeNode<T> node) {
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
        public DataQuadTreeNode(DataQuadTree<T> tree, DataQuadTreeNode<T> parent, Vector2 minBound, Vector2 maxBound) {
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
        public Vector2 GetClosestPointInside(Vector2 point) {
            return new Vector2(
                Mathf.Clamp(point.x, MinBound.x, MaxBound.x),
                Mathf.Clamp(point.y, MinBound.y, MaxBound.y)
            );
        }

        /// <summary>
        /// If this node has children
        /// </summary>
        /// <returns>If this node has children</returns>
        public bool HasChildren() {
            return TopLeftNode != null; // If one exists all exists
        }

        /// <summary>
        /// If this node is empty
        /// </summary>
        /// <returns>If this node is empty</returns>
        public bool IsEmpty() {
            return DataPoints == null || DataPoints.Count == 0;
        }

        /// <summary>
        /// If the point is inside the node
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>If the point is inside the node</returns>
        public bool IsInside(Vector2 point) {
            return (MinBound.x <= point.x) && (point.x <= MaxBound.x) &&
                (MinBound.y <= point.y) && (point.y <= MaxBound.y);
        }

        /// <summary>
        /// Inserts a Data inside this node
        /// This function assumes that the data point is inside, must be checked before
        /// </summary>
        /// <param name="data">Data to Insert</param>
        /// <returns>Inserted Data</returns>
        public DataQuadTreeNodeData Insert(DataQuadTreeNodeData data) {
            /// If we are a leaf
            if (DataPoints != null) {
                // If we still have capacity
                // TODO: Add min area support
                if (DataPoints.Count != Tree.MaxNodeSize) {
                    data.Node = this;
                    DataPoints.Add(data);
                    return data;
                }
                // If we are full
                else {
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

                    for (int i = 0; i < DataPoints.Count; i++) {
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
        private DataQuadTreeNodeData InternalChildrenInsert(DataQuadTreeNodeData data) {
            // If we are on the top quadrants
            if (Center.y < data.Point.y) {
                // If we are on the right quadrants 
                if (Center.x < data.Point.x) {
                    return TopRightNode.Insert(data);
                }
                // we are on the left quadrants
                else {
                    return TopLeftNode.Insert(data);
                }
            }
            // we are on the bottom quadrants
            else {
                // If we are on the right quadrants 
                if (Center.x < data.Point.x) {
                    return BottomRightNode.Insert(data);
                }
                // we are on the left quadrants
                else {
                    return BottomLeftNode.Insert(data);
                }
            }
        }

        /// <summary>
        /// Checks if all the node children are empty to make this node available again
        /// </summary>
        private void FixForEmptyChildren() {
            if (TopRightNode != null &&
                TopRightNode.IsEmpty() &&
                TopLeftNode.IsEmpty() &&
                BottomRightNode.IsEmpty() &&
                BottomLeftNode.IsEmpty()) {
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
        public bool DeleteData(DataQuadTreeNodeData data) {
            if (DataPoints != null && DataPoints.Remove(data)) {
                if (IsEmpty() && Parent != null) {
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
        public bool DeleteDataAll(DataQuadTreeNodeData data) {
            if (DataPoints != null && DataPoints.RemoveAll(storedData => storedData == data) != 0) {
                if (IsEmpty() && Parent != null) {
                    Parent.FixForEmptyChildren();
                }
                return true;
            }
            return false;
        }
    }

    public class DataQuadTree<T> {

        public int MaxNodeSize { get; private set; }
    }
}