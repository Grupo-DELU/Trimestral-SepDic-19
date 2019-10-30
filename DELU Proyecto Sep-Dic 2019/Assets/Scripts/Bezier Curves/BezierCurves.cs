using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurves : MonoBehaviour
{
    //public Vector2 a = Vector2.zero;
    //public Vector2 b = Vector2.one;
    //public Vector2 h1 = Vector2.up;
    //public Vector2 h2 = Vector2.up + Vector2.right * 2;
    public Curve curve;

    public Vector2 LinearInterpolation(Vector2 a, Vector2 b, float t)
    {
        Vector2 linearInterpolation = a + (b - a) * t;
        return linearInterpolation;
    }

    public Vector2 QuadraticInt(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 linearAB = LinearInterpolation(a, b, t);
        Vector2 linearBC = LinearInterpolation(b, c, t);
        Vector2 final = LinearInterpolation(linearAB, linearBC, t);
        return final;
    }

    public Vector2 CubicBezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        //a anchor 1
        //d anchor 2
        //b control 1
        //c control 2
        Vector2 quadABC = QuadraticInt(a, b, c, t);
        Vector2 quadBCD = QuadraticInt(b, c, d, t);
        Vector2 final = LinearInterpolation(quadABC, quadBCD, t);
        return final;
    }
}
