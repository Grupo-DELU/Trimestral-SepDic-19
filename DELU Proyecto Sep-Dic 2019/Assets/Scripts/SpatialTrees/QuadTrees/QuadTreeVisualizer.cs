using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeVisualizer : MonoBehaviour
{
    public QuadTree quadTree;

    public Vector2 cornerTL = -Vector2.right + Vector2.up;
    public Vector2 cornerBL = Vector2.right - Vector2.up;

    public Vector2 pointToLook = Vector2.zero;
    private Vector2 best = Vector2.one * int.MaxValue;

    public float randomPoints = 10000;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            List<Vector2> qp = new List<Vector2>();
            for (int i = 0; i < randomPoints; i++)
            {
                Vector2 rndm = Vector2.right * Random.Range(cornerTL.x, cornerBL.x) + Vector2.up * Random.Range(cornerBL.y, cornerTL.y);
                qp.Add(rndm);
            }
            quadTree = new QuadTree(3, 1, 1);
            quadTree.BuildQuadTree(cornerTL, cornerBL, qp);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            best = quadTree.GetNearestPoint(pointToLook, Vector2.one * int.MaxValue, quadTree.root);
            watch.Stop();
            Debug.Log(watch.ElapsedMilliseconds);
        }
    }

    

    private void OnDrawGizmos()
    {
        // Primero dibujamos los puntos
        if (quadTree == null || quadTree.root == null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(cornerTL, cornerTL + Vector2.right * cornerBL.x);
            Gizmos.DrawLine(cornerTL, cornerTL - Vector2.up * cornerBL.y);
            Gizmos.DrawLine(cornerBL, cornerBL + Vector2.up * cornerTL.y);
            Gizmos.DrawLine(cornerBL, cornerBL - Vector2.right * cornerTL.x);
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointToLook, 0.1f);
        if (best != Vector2.one * int.MaxValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(best, 0.2f);
        }

        RecursiveDraw(quadTree.root);

        void RecursiveDraw(Quadrant root)
        {
            float width = Mathf.Abs(root.cornerTL.x - root.cornerBR.x);

            Gizmos.color = Color.red;
            if (root.childTL == null && root.childTR == null &&
                root.childBL == null & root.childBR == null)
            {
                foreach (Vector2 point in root.pointsInside) Gizmos.DrawSphere(point, 0.1f);
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(root.cornerTL, root.cornerTL + Vector2.right * width);
            Gizmos.DrawLine(root.cornerTL, root.cornerTL - Vector2.up * width);
            Gizmos.DrawLine(root.cornerBR, root.cornerBR + Vector2.up * width);
            Gizmos.DrawLine(root.cornerBR, root.cornerBR - Vector2.right * width);

            if (root.childTL != null) RecursiveDraw(root.childTL);
            
            if (root.childTR != null) RecursiveDraw(root.childTR);
            
            if (root.childBL != null) RecursiveDraw(root.childBL);
            
            if (root.childBR != null) RecursiveDraw(root.childBR);
        }
    }
}
