using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quadrant
{
    // Puntos dentro del cuadrante
    public List<Vector2> pointsInside;

    // Esquinas delimitadoras del cuadrante
    public Vector2 cornerTL;
    public Vector2 cornerBR;

    // Padre/supercuadrante del cuadrante
    public Quadrant parent;

    // Hijos/subcuadrantes del cuadrante
    public Quadrant childTL;
    public Quadrant childTR;
    public Quadrant childBL;
    public Quadrant childBR;

    /// <summary>
    /// Area minima que tiene que tener un nodo/cuadrado del quad tree para subdividirse
    /// </summary>
    public float minArea;

    /// <summary>
    /// Cantidad maxima de puntos que puede tener un nodo/cuadrado del quad tree antes de subdividirse
    /// </summary>
    public int maxPoints;

    public void BuildQuadTree(List<Vector2> points)
    {
        SubdivideQuadrant(this, true);
    }


    public void SubdivideQuadrant(Quadrant parent, bool toTree = true)
    {
        // Primero chequeamos si en el cuadrante la cantidad de puntos cumple el threshold de puntos y el threshold de area
        if (parent.pointsInside.Count < maxPoints) return;

        // Ahora se chequea si los subcuadrantes van a tener el area minima
        float width = (Mathf.Abs(parent.cornerTL.x - parent.cornerBR.x) / 2);
        if (width * width < minArea) return;

        //Debug.Log("Subdividiendo cuadrante...");

        // Hacemos la subdivision de la esquina superior-izquierda
        parent.childTL = new Quadrant(parent, parent.cornerTL, parent.cornerTL + Vector2.right * width - Vector2.up * width, parent.pointsInside, maxPoints, minArea);

        // Hacemos la subdivision de la esquina superior-derecha
        parent.childTR = new Quadrant(parent, parent.cornerTL + Vector2.right * width, parent.cornerBR + Vector2.up * width, parent.pointsInside, maxPoints, minArea);

        // Hacemos la subdivision de la esquina inferior-izquierda
        parent.childBL = new Quadrant(parent, parent.cornerTL - Vector2.up * width, parent.cornerBR - Vector2.right * width, parent.pointsInside, maxPoints, minArea);

        // Hacemos la subdivision de la esquina inferior-derecha
        parent.childBR = new Quadrant(parent, parent.cornerTL - Vector2.up * width + Vector2.right * width, parent.cornerBR, parent.pointsInside, maxPoints, minArea);

        if (toTree)
        {
            SubdivideQuadrant(parent.childTL);
            SubdivideQuadrant(parent.childTR);
            SubdivideQuadrant(parent.childBL);
            SubdivideQuadrant(parent.childBR);
        }
    }


    /// <summary>
    /// Busca el menor cuadrante en el que se encuentra un punto arbitrario 
    /// </summary>
    /// <param name="point">Punto arbitrario a buscar cuadrante</param>
    /// <param name="root">Arbol a hacer busqueda</param>
    /// <returns>Menor cuadrante en el que se encuentra el punto</returns>
    public Quadrant GetPointQuadrant(Vector2 point, Quadrant root)
    {
        if (root.IsInside(point))
        {
            // Chequea si es un cuadrante hoja
            // Solo es necesario chequear un hijo ya que todos los cuadrantes que
            // no son minimos deberian de tener 4 hijos no nulos
            if (root.childBL == null) return root;
            else
            {
                // Revisa en que cuadrante de los hijos esta y sigue la recursion
                if (root.childBL.IsInside(point)) return GetPointQuadrant(point, root.childBL);
                if (root.childBR.IsInside(point)) return GetPointQuadrant(point, root.childBR);
                if (root.childTL.IsInside(point)) return GetPointQuadrant(point, root.childTL);
                if (root.childTR.IsInside(point)) return GetPointQuadrant(point, root.childTR);
                else return null;
            }
        }
        // Solo pasa cuando el punto de por si esta afuera del rango del area del quadtree
        else return null;
    }


    /// <summary>
    /// Busca el punto mas cercano del quadtree a un punto arbitrario
    /// </summary>
    /// <param name="point">Punto arbitrario</param>
    /// <param name="best">Mejor punto encontrado hasta el momento</param>
    /// <param name="root">Quadrante/subarbol a buscar mejor punto</param>
    /// <returns>Punto mas cercano del quadtree al punto arbitrario</returns>
    /// <remarks>El punto arbitrario debe de estar dentro del area del quadtree</remarks>
    public Vector2 GetNearestPoint(Vector2 point, Vector2 best, Quadrant root)
    {
        if (root == null) return best;

        //if (!root.IsWorthChecking(point, best)) return best;

        if (root.pointsInside.Count <= maxPoints || Mathf.Pow(Mathf.Abs(root.cornerTL.x - root.cornerBR.x)/2, 2) < minArea)
        {
            float min = Vector2.SqrMagnitude(point - best);
            foreach (Vector2 rPoint in root.pointsInside)
            {
                float dist = Vector2.SqrMagnitude(rPoint - point);
                if (dist < min)
                {
                    best = rPoint;
                    min = dist;
                }
            }
        }

        // Buscar como encontrar el hijo mas probable
        // Explora los hijos, si no tiene simplemente se va a regresar
        best = GetNearestPoint(point, best, root.childBL);
        best = GetNearestPoint(point, best, root.childBR);
        best = GetNearestPoint(point, best, root.childTL);
        best = GetNearestPoint(point, best, root.childTR);
        return best;
    }

    public void CalculatePointsInside(List<Vector2> points)
    {
        pointsInside.Clear();
        foreach (Vector2 point in points)
        {
            if (point.x >= cornerTL.x && point.x < cornerBR.x &&
                point.y < cornerTL.y && point.y >= cornerBR.y) pointsInside.Add(point);
        }
    }
    
    
    /// <summary>
    /// Indica si un punto esta dentro del cuadrante
    /// </summary>
    /// <param name="point">Punto a chequear si esta dentro</param>
    /// <returns>Si el punto esta dentro</returns>
    public bool IsInside(Vector2 point)
    {
        return point.x >= cornerTL.x && point.x < cornerBR.x &&
               point.y < cornerTL.y && point.y >= cornerBR.y;
    }


    /// <summary>
    /// Retorna el punto del cuadrante mas cercano a un punto arbitrario.
    /// </summary>
    /// <param name="point">Punto a buscar mas cercano</param>
    /// <returns>Punto mas cercano del cuadrante</returns>
    public Vector2 GetNearestInQuad(Vector2 point)
    {
        if (pointsInside.Count == 0) Debug.LogError("No hay puntos en el cuadrante!");
        Vector2 min = pointsInside[0];
        float sqrMag = Vector2.SqrMagnitude(min - point);
        for (int i = 1; i < pointsInside.Count; i++)
        {
            float sqrMagT = Vector2.SqrMagnitude(point - pointsInside[i]);
            if (sqrMag > sqrMagT)
            {
                min = pointsInside[i];
                sqrMag = sqrMagT;
            }
        }
        return min;
    }


    /// <summary>
    /// Chequea si vale la pena explorar un cuadrante en el proceso de
    /// busqueda de punto mas cercano
    /// </summary>
    /// <param name="point">Punto a buscar punto mas cercano</param>
    /// <param name="min">Distancia del punto mas cercano al punto</param>
    /// <returns>Si vale la pena chequear un cuadrante</returns>
    public bool IsWorthChecking(Vector2 point, Vector2 min)
    {
        if (pointsInside.Count == 0) return false;
        if (IsInside(point)) return true;
        Vector2 best;
        float minMag = Vector2.SqrMagnitude(point - min);
        // Evaluamos la dist con el mejor punto horizontal posible
        // (solo si el punto esta arriba/abajo del cuadrante
        if (point.x < cornerBR.x && point.x > cornerTL.x)
        {
            // Si esta arriba del cuadrante
            if (point.y > cornerTL.y)
            {
                best = Vector2.up * cornerTL.y + Vector2.right * point.x;
            }
            // Si esta debajo (o en la raya)
            else
            {
                best = Vector2.right * point.x + Vector2.up * cornerBR.y;
            }
            // Solo vale la pena chequear si la distancia desde el mejor punto posible es menor a la del minimo
        }
        // Similar a la prueba anterior pero verticalmente
        // y el cuadrante esta a la derecha/izquierdaa
        else if (point.y < cornerTL.y && point.y > cornerBR.y)
        {
            
            // Si esta a la derecha del cuadrante
            if (point.x > cornerBR.x)
            {
                best = Vector2.up * point.y + Vector2.right * cornerBR.x;
            }
            // Si esta a la izquierda (o en la raya)
            else
            {
                best = Vector2.up * point.y + Vector2.right * cornerTL.x;
            }
            // Solo vale la pena chequear si la distancia desde el mejor punto posible es menor a la del minimo
            
        }
        // En otro caso, el cuadrante esta diagonal al punto
        // y es necesario ver si es diagonal superior/inferior/izquierda/derecha
        else
        {
            // Si esta arriba del cuadrante
            if (point.y > cornerTL.y)
            {
                // Si esta a la derecha del cuadrante
                if (point.x > cornerBR.x)
                {
                    best = Vector2.up * cornerTL.y + Vector2.right * cornerBR.x;
                }
                // Si esta a la izquierda (o en la raya)
                else
                {
                    best = cornerTL;
                }
            }
            // Si esta debajo del cuadrante
            else
            {
                // Si esta a la derecha del cuadrante
                if (point.x > cornerBR.x)
                {
                    best = cornerBR;
                }
                // Si esta a la izquierda (o en la raya)
                else
                {
                    best = Vector2.up * cornerBR + Vector2.right * cornerTL.x;
                }
            }
        }
        return Vector2.SqrMagnitude(point - best) <= minMag;
    }


    public Quadrant(int maxPoints, float minArea)
    {
        this.maxPoints = maxPoints;
        this.minArea = minArea;
        pointsInside = new List<Vector2>();
        parent = null;
        childTL = null;
        childTR = null;
        childBL = null;
        childBR = null;
    }


    public Quadrant(Quadrant parent, Vector2 cornerTL, Vector2 cornerBR, List<Vector2> pointsInside, int maxPoints, float minArea)
    {
        this.maxPoints = maxPoints;
        this.minArea = minArea;

        this.parent = parent;
        this.cornerTL = cornerTL;
        this.cornerBR = cornerBR;

        this.pointsInside = new List<Vector2>();
        CalculatePointsInside(pointsInside);

        childTL = null;
        childTR = null;
        childBL = null;
        childBR = null;
    }
}