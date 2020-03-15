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
        foreach (Vector2 point in points) InsertPoint(point);
    }


    public void CompressTree()
    {
        CompressTreeRecursion(this);
    }


    public void CompressTreeRecursion(Quadrant quad)
    {
        if (quad == null || quad.childTL == null) return;
        CompressTreeRecursion(quad.childTL);
        CompressTreeRecursion(quad.childTR);
        CompressTreeRecursion(quad.childBL);
        CompressTreeRecursion(quad.childBR);

        int c = 0;
        // Chequea si sus hijos son hojas y si estan vacias
        if (quad.childTL.childTL == null && quad.childTL.pointsInside.Count == 0) c += 1;
        if (quad.childTR.childTL == null && quad.childTR.pointsInside.Count == 0) c += 1;
        if (quad.childBL.childTL == null && quad.childBL.pointsInside.Count == 0) c += 1;
        if (quad.childBR.childTL == null && quad.childBR.pointsInside.Count == 0) c += 1;

        if (c >= 3)
        {
            if (quad.pointsInside == null) quad.pointsInside = new List<Vector2>();
            if (quad.childTL.childTL == null)
            {
                quad.pointsInside.AddRange(quad.childTL.pointsInside);
                quad.childTL = null;
            }
            if (quad.childTR.childTL == null)
            { 
                quad.pointsInside.AddRange(quad.childTR.pointsInside);
                quad.childTR = null;
            }
            if (quad.childBL.childTL == null)
            { 
                quad.pointsInside.AddRange(quad.childBL.pointsInside);
                quad.childBL = null;
            }
            if (quad.childBR.childTL == null)
            {
                quad.pointsInside.AddRange(quad.childBR.pointsInside);
                quad.childBR = null;
            }
        }
    }


    public void SubdivideQuadrant(Quadrant parent)
    {
        // Primero chequeamos si en el cuadrante la cantidad de puntos cumple el threshold de puntos y el threshold de area
        //if (parent.pointsInside != null)
        //{
        //    if (parent.pointsInside.Count < maxPoints)
        //    {
        //        Debug.Log("F: " + (parent.childTL != null));
        //        return;
        //    }
        //}
        // Ahora se chequea si los subcuadrantes van a tener el area minima
        float width = (Mathf.Abs(parent.cornerTL.x - parent.cornerBR.x) / 2);
        //if (width * width < minArea)
        //{
        //    Debug.Log("F: " + (parent.childTL != null));
        //    return;
        //}


        // Hacemos la subdivision del cuadrante empezando por la esquina superior izquierda en sentido Z
        parent.childTL = new Quadrant(parent, parent.cornerTL, parent.cornerTL + Vector2.right * width - Vector2.up * width, maxPoints, minArea);
        parent.childTR = new Quadrant(parent, parent.cornerTL + Vector2.right * width, parent.cornerBR + Vector2.up * width, maxPoints, minArea);
        parent.childBL = new Quadrant(parent, parent.cornerTL - Vector2.up * width, parent.cornerBR - Vector2.right * width, maxPoints, minArea);
        parent.childBR = new Quadrant(parent, parent.cornerTL - Vector2.up * width + Vector2.right * width, parent.cornerBR, maxPoints, minArea);
    }


    /// <summary>
    /// Busca el menor cuadrante en el que se encuentra un punto arbitrario 
    /// </summary>
    /// <param name="point">Punto arbitrario a buscar cuadrante</param>
    /// <param name="root">Arbol a hacer busqueda</param>
    /// <returns>Menor cuadrante en el que se encuentra el punto</returns>
    public Quadrant GetPointQuadrant(Vector2 point, Quadrant root)
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


    public void InsertPoint(Vector2 point)
    {
        if (pointsInside != null)
        {
            // O el area es menor al area minima o no podra subdividirse
            if (pointsInside.Count < maxPoints) pointsInside.Add(point); // O mejor ==
            // Si esta full la lista, intenta subdividirte
            else
            {
                SubdivideQuadrant(this);
                // Si no se pudo subdividir el cuadrante, quedate tu punto maldito
                if (childTL == null) pointsInside.Add(point);
                else
                {
                    foreach (Vector2 ppoint in pointsInside) InsertInChild(ppoint);
                    InsertInChild(point);
                    pointsInside = null;
                }
            }
        }
        else
        {
            InsertInChild(point);
        }
    }


    public void InsertInChild(Vector2 point)
    {
        if (childBL.IsInside(point))
        {
            childBL.InsertPoint(point);
            return;
        }
        else if (childBR.IsInside(point))
        {
            childBR.InsertPoint(point);
            return;
        }
        else if (childTL.IsInside(point))
        {
            childTL.InsertPoint(point);
            return;
        }
        else if (childTR.IsInside(point))
        {
            childTR.InsertPoint(point);
            return;
        }
        //Debug.Log("aja f aca maricon");
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

        if (!root.IsWorthChecking(point, best)) return best;

        // Solo chequea esto si es un nodo hoja
        if (root.pointsInside != null)
        {
            //if (root.pointsInside.Count <= maxPoints || Mathf.Pow(Mathf.Abs(root.cornerTL.x - root.cornerBR.x) / 2, 2) < minArea)
            //{
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
            //}
        }
        // aca puede ir un else
        // Buscar como encontrar el hijo mas probable
        // Explora los hijos, si no tiene simplemente se va a regresar
        best = GetNearestPoint(point, best, root.childBL);
        best = GetNearestPoint(point, best, root.childBR);
        best = GetNearestPoint(point, best, root.childTL);
        best = GetNearestPoint(point, best, root.childTR);
        return best;
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
    /// Chequea si vale la pena explorar un cuadrante en el proceso de
    /// busqueda de punto mas cercano
    /// </summary>
    /// <param name="point">Punto a buscar punto mas cercano</param>
    /// <param name="min">Distancia del punto mas cercano al punto</param>
    /// <returns>Si vale la pena chequear un cuadrante</returns>
    public bool IsWorthChecking(Vector2 point, Vector2 min)
    {
        //if (pointsInside == null && childBL == null) return false;
        //if (pointsInside.Count == 0) return false;
        if (IsInside(point)) return true;

        Vector2 best = new Vector2(Mathf.Clamp(point.x, cornerTL.x, cornerBR.x), Mathf.Clamp(point.y, cornerBR.y, cornerTL.y));

        return Vector2.SqrMagnitude(point - best) <= Vector2.SqrMagnitude(point - min);
    }


    public Quadrant(Quadrant parent, Vector2 cornerTL, Vector2 cornerBR, int maxPoints, float minArea)
    {
        this.maxPoints = maxPoints;
        this.minArea = minArea;

        this.parent = parent;
        this.cornerTL = cornerTL;
        this.cornerBR = cornerBR;

        this.pointsInside = new List<Vector2>(maxPoints);

        childTL = null;
        childTR = null;
        childBL = null;
        childBR = null;
    }
}