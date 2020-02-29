using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    /// <summary>
    /// Area minima que tiene que tener un nodo/cuadrado del quad tree
    /// </summary>
    public float minArea;
    /// <summary>
    /// Cantidad maxima de puntos que puede tener un nodo/cuadrado del quad tree
    /// </summary>
    /// <remarks>
    /// Si se supera esta cantidad, el nodo/cuadrado es dividido
    /// </remarks>
    public int maxPoints;

    public Quadrant root;

    public void BuildQuadTree(Vector2 cornerTL, Vector2 cornerBR, List<Vector2> points)
    {
        root = new Quadrant(null, cornerTL, cornerBR, points);
        SubdivideQuadrant(root);
    }

    public void SubdivideQuadrant(Quadrant parent)
    {
        // Primero chequeamos si en el cuadrante la cantidad de puntos cumple el threshold de puntos
        if (parent.pointsInside.Count > maxPoints) return;

        // Ahora se chequea si los subcuadrantes van a tener el area minima
        float width = (Mathf.Abs(parent.cornerTL.x - parent.cornerBR.x) / 4);
        float height = (Mathf.Abs(parent.cornerTL.y - parent.cornerBR.y) / 4);
        // widht/height es lo mismo xd gafedad mia
        if (width * height < minArea) return;
        

        // Hacemos la subdivision de la esquina superior-izquierda
        parent.childTL = new Quadrant(parent, parent.cornerTL, parent.cornerTL + Vector2.right * width - Vector2.up * height, parent.pointsInside);
        SubdivideQuadrant(parent.childTL);

        // Hacemos la subdivision de la esquina superior-derecha
        parent.childTR = new Quadrant(parent, parent.cornerTL + Vector2.right * width, parent.cornerBR + Vector2.up * height, parent.pointsInside);
        SubdivideQuadrant(parent.childTR);

        // Hacemos la subdivision de la esquina inferior-izquierda
        parent.childBL = new Quadrant(parent, parent.cornerTL - Vector2.up * height, parent.cornerBR - Vector2.right * width, parent.pointsInside);
        SubdivideQuadrant(parent.childBL);

        // Hacemos la subdivision de la esquina inferior-derecha
        parent.childBR = new Quadrant(parent, parent.cornerTL - Vector2.up * height + Vector2.right * width, parent.cornerBR, parent.pointsInside);
        SubdivideQuadrant(parent.childBR);
    }

    public QuadTree(int maxPoints, float minArea)
    {
        this.maxPoints = maxPoints;
        this.minArea = minArea;
    }
}

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

    public void CalculatePointsInside(List<Vector2> points)
    {
        pointsInside.Clear();
        foreach (Vector2 point in points)
        {
            if (point.x >= cornerTL.x && point.x < cornerBR.x &&
                point.y < cornerTL.y && point.y >= cornerBR.y) pointsInside.Add(point);
        }
    }

    public Quadrant()
    {
        pointsInside = new List<Vector2>();
        parent = null;
        childTL = null;
        childTR = null;
        childBL = null;
        childBR = null;
    }

    public Quadrant(Quadrant parent, Vector2 cornerTL, Vector2 cornerBR)
    {
        this.parent = parent;
        this.cornerTL = cornerTL;
        this.cornerBR = cornerBR;

        childTL = null;
        childTR = null;
        childBL = null;
        childBR = null;
    }

    public Quadrant(Quadrant parent, Vector2 cornerTL, Vector2 cornerBR, List<Vector2> pointsInside)
    {
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