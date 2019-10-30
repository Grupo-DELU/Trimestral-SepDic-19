using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public void CreateCurve(Vector2[] curves, bool isClosed)
    {
        this.points = curves;
        this.isClosed = isClosed;
    }
}
