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

    public List<Transform> points;

    private void Start()
    {
        foreach (Transform t in points) t.gameObject.SetActive(false);    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            List<Vector2> qp = new List<Vector2>();
            foreach (Transform point in points) qp.Add(point.position);
            quadTree = new QuadTree(3, 1, 1);
            quadTree.BuildQuadTree(cornerTL, cornerBL, qp);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            double start = Time.time;
            best = quadTree.GetNearestPoint(pointToLook, Vector2.one * int.MaxValue, quadTree.root);
            Debug.Log(best);
            Debug.Log("Tiempo: " + (Time.time - start).ToString("f10"));
        }
    }

    

    private void OnDrawGizmos()
    {
        // Primero dibujamos los puntos
        if (quadTree == null) return;
        if (quadTree.root == null) return;

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
