using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeVisualizer : MonoBehaviour
{
    public QuadTree quadTree;

    public Vector2 cornerTL = -Vector2.right + Vector2.up;
    public Vector2 cornerBL = Vector2.right - Vector2.up;

    public List<Transform> points;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            List<Vector2> qp = new List<Vector2>();
            foreach (Transform point in points) qp.Add(point.position);
            quadTree = new QuadTree(1, 3, 1);
            quadTree.BuildQuadTree(cornerTL, cornerBL, qp);
        }
    }

    

    private void OnDrawGizmos()
    {
        // Primero dibujamos los puntos
        if (quadTree == null) return;
        if (quadTree.root == null) return;

        RecursiveDraw(quadTree.root);

        void RecursiveDraw(Quadrant root)
        {
            float width = Mathf.Abs(root.cornerTL.x - root.cornerBR.x);

            Gizmos.color = Color.red;
            if (quadTree.root != root) foreach (Vector2 point in root.pointsInside) Gizmos.DrawSphere(point, 0.2f);

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
