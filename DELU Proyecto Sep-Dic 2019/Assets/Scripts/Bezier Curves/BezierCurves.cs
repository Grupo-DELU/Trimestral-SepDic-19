using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurves : MonoBehaviour
{
    //public Vector2 p0 = Vector2.zero;
    //public Vector2 p1 = Vector2.one;
    //public Vector2 h1 = Vector2.up;
    //public Vector2 h2 = Vector2.up + Vector2.right * 2;
    public Curve curve;

    public Vector2 LinearInterpolation(Vector2 p0, Vector2 p1, float t)
    {
        Vector2 linearInterpolation = p0 + (p1 - p0) * t;
        return linearInterpolation;
    }

    public Vector2 QuadraticInt(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 linearAB = LinearInterpolation(p0, p1, t);
        Vector2 linearBC = LinearInterpolation(p1, p2, t);
        Vector2 final = LinearInterpolation(linearAB, linearBC, t);
        return final;
    }

    public Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        //p0 anchor 1
        //p3 anchor 2
        //p1 control 1
        //p2 control 2
        Vector2 quadABC = QuadraticInt(p0, p1, p2, t);
        Vector2 quadBCD = QuadraticInt(p1, p2, p3, t);
        Vector2 final = LinearInterpolation(quadABC, quadBCD, t);
        return final;
    }

    /// <summary>
    /// Divide una curva de bezier en intervalos
    /// </summary>
    /// <param name="p0">Anchor point 1</param>
    /// <param name="p1">Hanler 1</param>
    /// <param name="p2">Handler 2</param>
    /// <param name="p3">Anchor point 2</param>
    /// <param name="intervals">Intervalos deseados</param>
    /// <param name="precStep">Precision del step usado para calcular los intervalos</param>
    /// <returns>Puntos entre cada intervalo</returns>
    public Vector2[] IntervalBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, int intervals, float precStep = 0.005f)
    {
        Vector2[] points = new Vector2[intervals + 1];
        points[0] = CubicBezier(p0, p1, p2, p3, 0); //Initial point
        points[intervals] = CubicBezier(p0, p1, p2, p3, 1); //Final point
        float lenght = 0;
        Vector2 prev = points[0];
        for (float t = precStep; t < 1; t += precStep)
        {
            lenght += (prev - CubicBezier(p0, p1, p2, p3, t)).magnitude;
            prev = CubicBezier(p0, p1, p2, p3, t);
        }
        float intLenght = lenght / intervals;
        int i = 0;
        float prevLenght = 0;
        lenght = 0;
        prev = points[0];
        float j = precStep;
        while (i < intervals - 1)
        {
            prevLenght = lenght;
            lenght += (prev - CubicBezier(p0, p1, p2, p3, j)).magnitude;
            if (prevLenght <= intLenght && intLenght <= lenght)
            {
                points[1 + i] = CubicBezier(p0, p1, p2, p3, j);
                prevLenght = 0;
                lenght = 0;
                i += 1;
            }
            prev = CubicBezier(p0, p1, p2, p3, j);
            j += precStep;
        }
        return points;
    }
}
