using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Utils.SpatialTrees.QuadTrees;
using TestQuadTree = Utils.SpatialTrees.QuadTrees.DataQuadTree<int>;

namespace Tests
{
    public class QuadTreesTestSuite
    {
        public class DistanceToPoint
        {
            public int Pos { get; set; }

            public float SqrDistance { get; set; }

            public static int CompareTo(DistanceToPoint lhs, DistanceToPoint rhs)
            {
                return lhs.SqrDistance.CompareTo(rhs.SqrDistance);
            }
        }


        public void SetUpData(
            out Vector2[] points, out DistanceToPoint[] sortedPoints, in int numberOfTestPoints,
            in Vector2 minBound, in Vector2 maxBound, in Vector2 target
        )
        {
            sortedPoints = new DistanceToPoint[numberOfTestPoints];
            points = new Vector2[numberOfTestPoints];
            for (int i = 0; i < numberOfTestPoints; i++)
            {
                points[i] = new Vector2(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.y, maxBound.y));
                sortedPoints[i] = new DistanceToPoint();
                sortedPoints[i].Pos = i;
                sortedPoints[i].SqrDistance = (points[i] - target).sqrMagnitude;
            }
            System.Array.Sort(sortedPoints, DistanceToPoint.CompareTo);
        }

        // Timing Test for Insertion
        [Test]
        public void QuadTreesInsertionTime()
        {
            Vector2[] points;
            DistanceToPoint[] sortedPoints;
            int numberOfTestPoints = 10000;
            Vector2 minBound = new Vector2(-1000, -1000);
            Vector2 maxBound = new Vector2(1000, 1000);
            Vector2 target = Vector2.zero;
            SetUpData(out points, out sortedPoints, numberOfTestPoints, minBound, maxBound, target);

            TestQuadTree testQuadTree = new TestQuadTree(minBound, maxBound);
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < numberOfTestPoints; i++)
            {
                testQuadTree.Insert(points[i], i);
            }
            watch.Stop();
            Debug.Log($"Data Quadtree Insertion Took {watch.ElapsedMilliseconds} ms for {numberOfTestPoints} points ({(double)watch.ElapsedMilliseconds / numberOfTestPoints} ms per point)");
        }

        // Timing Test for Insertion with Min Area
        [Test]
        public void QuadTreesInsertionTimeMinArea()
        {
            Vector2[] points;
            DistanceToPoint[] sortedPoints;
            int numberOfTestPoints = 10000;
            Vector2 minBound = new Vector2(-1000, -1000);
            Vector2 maxBound = new Vector2(1000, 1000);
            Vector2 target = Vector2.zero;
            float minArea = 1.0f;
            SetUpData(out points, out sortedPoints, numberOfTestPoints, minBound, maxBound, target);

            TestQuadTree testQuadTree = new TestQuadTree(minBound, maxBound, minNodeArea: minArea);
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < numberOfTestPoints; i++)
            {
                testQuadTree.Insert(points[i], i);
            }
            watch.Stop();
            Debug.Log($"Data Quadtree Insertion with Min Area of {minArea} Took {watch.ElapsedMilliseconds} ms for {numberOfTestPoints} points ({(double)watch.ElapsedMilliseconds / numberOfTestPoints} ms per point)");
        }

        // Timing Test for Insertion
        [Test]
        public void QuadTreesInsertionTimePedro()
        {
            Vector2[] points;
            DistanceToPoint[] sortedPoints;
            int numberOfTestPoints = 10000;
            Vector2 minBound = new Vector2(-1000, -1000);
            Vector2 maxBound = new Vector2(1000, 1000);
            Vector2 target = Vector2.zero;
            SetUpData(out points, out sortedPoints, numberOfTestPoints, minBound, maxBound, target);

            Quadrant testQuadTree = new Quadrant(null, minBound, maxBound, 8, 0);
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < numberOfTestPoints; i++)
            {
                testQuadTree.InsertPoint(points[i]);
            }
            watch.Stop();
            Debug.Log($"Pedro Quadtree Insertion Took {watch.ElapsedMilliseconds} ms for {numberOfTestPoints} points ({(double)watch.ElapsedMilliseconds / numberOfTestPoints} ms per point)");
        }

        private Vector2[] mPoints;
        private DistanceToPoint[] mSortedPoints;
        private const int kNumberOfTestPoints = 1000000;
        private static readonly Vector2 kMinBound = new Vector2(-1000, -1000);
        private static readonly Vector2 kMaxBound = new Vector2(1000, 1000);
        private static readonly Vector2 kTarget = Vector2.zero;
        private static readonly int kMaxNodeSize = 8;
        public const float kMinNodeAreaPercent = 0.01f;
        private TestQuadTree mTestQuadTree;
        private Quadrant pQuadrant;
        private readonly double kNanoSecondsPerTick = (1000L * 1000L * 1000L) / (double)System.Diagnostics.Stopwatch.Frequency;
        private readonly double kMiliSecondsPerTick = (1000L) / (double)System.Diagnostics.Stopwatch.Frequency;

        [SetUp]
        public void SetUp()
        {
            Vector2 cornerTL = Vector2.right * kMinBound.x + Vector2.up * kMaxBound.y;
            Vector2 cornetBR = Vector2.right * kMaxBound.x + Vector2.up * kMinBound.y;
            SetUpData(out mPoints, out mSortedPoints, kNumberOfTestPoints, kMinBound, kMaxBound, kTarget);
            mTestQuadTree = new TestQuadTree(kMinBound, kMaxBound);
            pQuadrant = new Quadrant(null, cornerTL, cornetBR, 8, 0);
            mTestQuadTree = new TestQuadTree(kMinBound, kMaxBound, kMaxNodeSize, (kMaxBound - kMinBound).magnitude * kMinNodeAreaPercent);
            for (int i = 0; i < kNumberOfTestPoints; i++)
            {
                mTestQuadTree.Insert(mPoints[i], i);
                pQuadrant.InsertPoint(mPoints[i]);
            }
        }

        public void ShowTreeInfo()
        {
            Debug.Log($"Data Quadtree of Bounds min={kMinBound} max={kMaxBound} maxNodeSize={kMaxNodeSize} minNodeArea={(kMaxBound - kMinBound).magnitude * kMinNodeAreaPercent} ({kMinNodeAreaPercent * 100.0f}% of Area)");
        }

        // Timing Test for Nearest Neighbor
        [Test]
        public void QuadTreesNearestNeighbor()
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            TestQuadTree.DistanceToDataPoint data = mTestQuadTree.NearestNeighbor(kTarget);
            watch.Stop();

            Assert.IsTrue(
                data.SqrClosestDistance <= mSortedPoints[0].SqrDistance,
                $"Point is further than the closest distance: {mSortedPoints[0].SqrDistance} (Closest) < {data.SqrClosestDistance} (Obtained)"
                );
            ShowTreeInfo();
            Debug.Log($"Data Quadtree Nearest Neighbor Took {watch.ElapsedTicks} Ticks ({watch.ElapsedTicks * kNanoSecondsPerTick} ns | {watch.ElapsedTicks * kMiliSecondsPerTick} ms) for a Tree with {kNumberOfTestPoints} points");
        }

        // Timing Test for K Nearest Neighbor
        [Test]
        public void QuadTreesKNearestNeighbor()
        {
            int k = Mathf.Min(10, kNumberOfTestPoints);
            TestQuadTree.DistanceToDataPoint[] storage = new TestQuadTree.DistanceToDataPoint[k];

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            int found = mTestQuadTree.KNearestNeighbor(kTarget, k, storage);
            watch.Stop();

            Assert.IsTrue(
                found == k,
                $"K Nearest Neighbor failed to find {k} neighbors even though the tree has {kNumberOfTestPoints} points"
            );
            for (int i = 0; i < found; i++)
            {
                Assert.IsTrue(
                storage[i].SqrClosestDistance <= mSortedPoints[k].SqrDistance,
                $"Point is further than the kth-closest distance: {mSortedPoints[k].SqrDistance} (kth-closest) < {storage[i].SqrClosestDistance} (Obtained)"
                );
            }
            ShowTreeInfo();
            Debug.Log($"Data Quadtree K={k} Nearest Neighbor Took {watch.ElapsedTicks} Ticks ({watch.ElapsedTicks * kNanoSecondsPerTick} ns | {watch.ElapsedTicks * kMiliSecondsPerTick} ms) for a Tree with {kNumberOfTestPoints} points");
        }

        [Test]
        public void QuadTreesNearestNeighborPedro()
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            Vector2 res = pQuadrant.GetNearestPoint(kTarget, Vector2.one * int.MaxValue, pQuadrant);
            watch.Stop();
            float sqrDist = (res - kTarget).sqrMagnitude;
            Assert.IsTrue(
                sqrDist <= mSortedPoints[0].SqrDistance,
                $"Point is further than the closest distance: {mSortedPoints[0].SqrDistance} (Closest) < {sqrDist} (Obtained)"
                );
            Debug.Log($"Data Quadtree Nearest Neighbor Pedro Took {watch.ElapsedTicks} Ticks ({watch.ElapsedTicks * kNanoSecondsPerTick} ns | {watch.ElapsedTicks * kMiliSecondsPerTick} ms) for a Tree with {kNumberOfTestPoints} points");
        }

        // Timing Test for K Nearest Neighbor Using 10% of the points
        [Test]
        public void QuadTreesKNearestNeighbor10Percent()
        {
            int k = (int)(kNumberOfTestPoints / 10.0f);
            TestQuadTree.DistanceToDataPoint[] storage = new TestQuadTree.DistanceToDataPoint[k];

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            int found = mTestQuadTree.KNearestNeighbor(kTarget, k, storage);
            watch.Stop();

            Assert.IsTrue(
                found == k,
                $"K Nearest Neighbor failed to find {k} neighbors even though the tree has {kNumberOfTestPoints} points"
            );
            for (int i = 0; i < found; i++)
            {
                Assert.IsTrue(
                storage[i].SqrClosestDistance <= mSortedPoints[k].SqrDistance,
                $"Point is further than the kth-closest distance: {mSortedPoints[k].SqrDistance} (kth-closest) < {storage[i].SqrClosestDistance} (Obtained)"
                );
            }
            ShowTreeInfo();
            Debug.Log($"Data Quadtree K={k} Nearest Neighbor Took {watch.ElapsedTicks} Ticks ({watch.ElapsedMilliseconds} ms) for a Tree with {kNumberOfTestPoints} points");
        }
    }
}
