using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurvePreviewer : MonoBehaviour
{
    public CurveScriptObject curve;
    [Range(0.05f, 1f)]
    public float size = 0.1f;

    private void OnDrawGizmos()
    {
        if (curve == null) return;
        foreach(Vector2 point in curve.points)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(point, size);
        }
    }
}
