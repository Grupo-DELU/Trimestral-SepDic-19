using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurvePreviewer : MonoBehaviour
{
    public CurveScriptObject curve;

    private void OnDrawGizmos()
    {
        if (curve == null) return;
        foreach(Vector2 point in curve.points)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.05f);
        }
    }
}
