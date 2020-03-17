using UnityEngine;
using Utils.SpatialTrees.QuadTrees;

public class CurveScriptObject : ScriptableObject
{
    /// <summary>
    /// Puntos de la curva
    /// </summary>
    public Vector2[] points;
    /// <summary>
    /// La curva es cerrada?
    /// </summary>
    public bool isClosed;
    [System.NonSerialized]
    public DataQuadTree<int> qTree;

    public void CreateCurve(Vector2[] curves, bool isClosed, DataQuadTree<int> qTree)
    {
        this.points = curves;
        this.isClosed = isClosed;
        this.qTree = qTree;
    }
}
