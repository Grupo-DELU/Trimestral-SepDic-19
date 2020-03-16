using UnityEngine;

/// <summary>
/// Funciones relacionadas a calculos de puntos/vectores/curvas relacionados con
/// interpolaciones entre varios puntos.
/// </summary>
namespace Bezier
{
    public class BezierInt
    {
        /// <summary>
        /// Interpolacion lineal entre dos puntos
        /// </summary>
        /// <param name="a">Punto a</param>
        /// <param name="b">Punto b</param>
        /// <param name="t">Parametro t</param>
        /// <returns>Punto de recta entre a y b</returns>
        public static Vector3 LinearInterpolation(Vector3 a, Vector3 b, float t)
        {
            Vector3 linearInterpolation = a + (b - a) * t;
            return linearInterpolation;
        }

        /// <summary>
        /// Interpolacion cuadratica entre tres puntos
        /// </summary>
        /// <param name="a">Punto a</param>
        /// <param name="b">Punto b</param>
        /// <param name="c">Punto c</param>
        /// <param name="t">Parametro t</param>
        /// <returns>Punto de la int cuadratica de a,b y c</returns>
        public static Vector3 QuadraticInt(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 linearAB = LinearInterpolation(a, b, t);
            Vector3 linearBC = LinearInterpolation(b, c, t);
            Vector3 final = LinearInterpolation(linearAB, linearBC, t);
            return final;
        }

        /// <summary>
        /// Bezier cubico
        /// </summary>
        /// <param name="p0">Punto p0 anchor</param>
        /// <param name="p1">Tangente de p0</param>
        /// <param name="p2">Tangente de p3</param>
        /// <param name="p3">Punto p3 anchor</param>
        /// <param name="t">Parametro t</param>
        /// <returns>Punto de la interpolacion cubica entre p0,p1,p2 y p3</returns>
        public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 quadABC = QuadraticInt(p0, p1, p2, t);
            Vector3 quadBCD = QuadraticInt(p1, p2, p3, t);
            Vector3 final = LinearInterpolation(quadABC, quadBCD, t);
            return final;
        }

        /// <summary>
        /// Tangente de un punto en bezier cubico
        /// </summary>
        /// <param name="p0">Punto p0 anchor</param>
        /// <param name="p1">Tangente de p0</param>
        /// <param name="p2">Tangente de p3</param>
        /// <param name="p3">Punto p3 anchor</param>
        /// <param name="t">Parametro t</param>
        /// <returns>Tangente en un punto de bezier cubico</returns>
        public static Vector3 CubicBezierTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            //Vector3 d = QuadraticInt(p0, p1, p2, t);
            //Vector3 e = QuadraticInt(p1, p2, p3, t);
            //return (e - d).normalized;
            //Formula desarrollada
            float omt = 1f - t;
            float omt2 = omt * omt;
            float t2 = t * t;
            Vector3 tangent = 3 * omt2 * (p1 - p0) +
                              6 * t * omt * (p2 - p1) +
                              3 * (p3 - p2) * t2;
            return tangent.normalized;
        }
    }
}