using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeVisualizer : MonoBehaviour
{
    public Quadrant rootQuad = null;

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
            Debug.Log("Buildeando arbol...");
            rootQuad = new Quadrant(null, cornerTL, cornerBL, qp, 3, 16);
            rootQuad.BuildQuadTree(qp);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            best = rootQuad.GetNearestPoint(pointToLook, Vector2.one * int.MaxValue, rootQuad);
            watch.Stop();
            Debug.Log(watch.ElapsedTicks/);
            Debug.Log(best);
            foreach (Vector2 p in rootQuad.pointsInside)
            {
                if (Vector2.SqrMagnitude(p - pointToLook) < Vector2.SqrMagnitude(best - pointToLook))
                {
                    if (p != best)
                    {
                        Debug.Log("MAL");
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            List<float> times = new List<float>();
            for (int j = 0; j < 1000; j++)
            {
                List<Vector2> qp = new List<Vector2>();
                for (int i = 0; i < randomPoints; i++)
                {
                    Vector2 rndm = Vector2.right * Random.Range(cornerTL.x, cornerBL.x) + Vector2.up * Random.Range(cornerBL.y, cornerTL.y);
                    qp.Add(rndm);
                }
                rootQuad = new Quadrant(null, cornerTL, cornerBL, qp, 3, 16);
                rootQuad.BuildQuadTree(qp);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                best = rootQuad.GetNearestPoint(pointToLook, Vector2.one * int.MaxValue, rootQuad);
                watch.Stop();
                times.Add(watch.ElapsedMilliseconds);
            }
            float total = 0;
            foreach (float t in times)
            {
                total += t;
            }
            Debug.Log(total.ToString("f10"));
            Debug.Log(total / 1000);
            Debug.Log(total / randomPoints);
        }
    }

    

    private void OnDrawGizmos()
    {
        if (rootQuad == null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(cornerTL, cornerTL + Vector2.right * cornerBL.x);
            Gizmos.DrawLine(cornerTL, cornerTL - Vector2.up * cornerBL.y);
            Gizmos.DrawLine(cornerBL, cornerBL + Vector2.up * cornerTL.y);
            Gizmos.DrawLine(cornerBL, cornerBL - Vector2.right * cornerTL.x);
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointToLook, 1f);
        if (best != Vector2.one * int.MaxValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(best, 2f);
        }

        RecursiveDraw(rootQuad);

        void RecursiveDraw(Quadrant root)
        {
            float width = Mathf.Abs(root.cornerTL.x - root.cornerBR.x);

            Gizmos.color = Color.red;
            if (root.childTL == null && root.childTR == null &&
                root.childBL == null & root.childBR == null)
            {
                foreach (Vector2 point in root.pointsInside) Gizmos.DrawSphere(point, 0.9f);
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
