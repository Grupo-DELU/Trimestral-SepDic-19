using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils.SpatialTrees.QuadTrees;

public class ShipCurveInitializer : MonoBehaviour
{
    public List<CurveScriptObject> curvesSO = null;
    public Vector2 maxBound = Vector2.one * 50;
    public Vector2 minBound = Vector2.one * -50;

    public UnityEvent onCurvesRdy = new UnityEvent();

    private void Awake()
    {
        foreach (CurveScriptObject curve in curvesSO)
        {
            DataQuadTree<int> qt = new DataQuadTree<int>(minBound, maxBound);
            for (int i = 0; i < curve.points.Length; i++) qt.Insert(curve.points[i], i);
            curve.qTree = qt;
        }
    }
}
