using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bezier;

[System.Serializable]
public class Curve
{
    public List<Vector2> points;
    public bool isClosed;
    public void StartCurve(Vector2 anchorPosition)
    {
        points = new List<Vector2>();
        points.Add(anchorPosition);
        points.Add(anchorPosition + Vector2.up);
        points.Add(anchorPosition + Vector2.right + Vector2.down);
        points.Add(anchorPosition + Vector2.right);       
    }

    public int NumSegments
    {
        get { return ((points.Count - 4) / 3) + 1; }
    }

    public int NumPoints
    {
        get { return points.Count; }
    }
    public void AddSegment(Vector2 mousePos)
    {
        points.Add(2 * points[points.Count - 1] - points[points.Count - 2]);
        points.Add(mousePos + Vector2.one);
        points.Add(mousePos);
    }
    public Vector2[] GetPointsInSegment(int seg)
    {
        return new Vector2[] { points[3 * seg], points[3 * seg + 1], points[3 * seg + 2], points[3 * seg + 3] };
    }

    //agrega caso de closed
    public void MovePoint(int i, Vector2 newPos)
    {
        // Si es un anchor point... Listo
        if (i % 3 == 0)
        {
            Vector2 oldPos = points[i];
            Vector2 delta = (newPos - oldPos);
            
            if (i == 0)
            {
                points[1] += delta;
                // Si la curva es cerrada, ahora el ultimo es control del primero
                // y tambien hay que moverlo con este
                if (isClosed)
                {
                    points[points.Count - 1] += delta;
                }
            }
            else if (i == points.Count - 1)
            {
                points[i - 1] += delta;
            }
            else
            {
                points[i - 1] += delta;
                points[i + 1] += delta;
            }
        }
        // Si es un punto de control
        else
        {
            if (!isClosed)
            {
                if (i - 1 != 0 && i + 1 != points.Count - 1)
                {
                    if (i % 3 == 1)
                    {
                        Vector2 dir = points[i - 1] - points[i];
                        points[i - 2] = points[i - 1] + dir;
                    }
                    else if (i % 3 == 2)
                    {
                        Vector2 dir = points[i + 1] - points[i];
                        points[i + 2] = points[i + 1] + dir;
                    }
                }
            }
            else
            {
                // Si es el ultimo punto
                if (i == points.Count - 1)
                {
                    //Vector2 dir = points[i - 2] - points[i];
                    Vector2 dir = points[0] - points[i];
                    points[1] = points[0] + dir;
                }
                // Si es el penultimo
                else if (i == points.Count - 2)
                {
                    Vector2 dir = points[i - 1] - points[i];
                    points[i - 2] = points[i - 1] + dir;
                }
                // Si es el ante penultimo!
                else if (i == points.Count - 4)
                {
                    Debug.Log(i - 3);
                    Vector2 dir = points[i + 1] - points[i];
                    points[i + 2] = points[i + 1] + dir;
                }
                // Si es el primero
                else if (i == 1)
                {
                    Vector2 dir = points[0] - points[i];
                    points[points.Count - 1] = points[0] + dir;
                }
                // Despues de todos los casos especiales,
                // los puntos intermedios...
                else
                {
                    if (i % 3 == 1)
                    {
                        Vector2 dir = points[i - 1] - points[i];
                        points[i - 2] = points[i - 1] + dir;
                    }
                    else if (i % 3 == 2)
                    {
                        Vector2 dir = points[i + 1] - points[i];
                        points[i + 2] = points[i + 1] + dir;
                    }
                }
            }
        }
        points[i] = newPos;
    }
}

[CustomEditor(typeof(BezierCurves))]
public class BezierCurvesEditor : Editor
{
    BezierCurves creator;
    Curve curve;
    bool removed = false;
    bool added = false;
    /// <summary>
    /// Separacion entre cada punto de la curva
    /// </summary>
    public float separation = 0.1f;
    
    /// <summary>
    /// Precision del calculo de separacion uniforme de puntos en
    /// una curva de bezier.
    /// </summary>
    /// <remarks>
    /// Mientras mas bajo, mas preciso.
    /// </remarks>
    public float prec_step = 0.002f;

    void Draw()
    {
        if (!curve.isClosed)
        {
            if (added && !removed)
            {
                curve.points.RemoveAt(curve.points.Count - 1);
                curve.points.RemoveAt(curve.points.Count - 2);
                added = false;
                removed = true;
            }
            for (int i = 0; i < curve.NumSegments; i++)
            {
                Vector2[] segment = curve.GetPointsInSegment(i);
                Handles.color = Color.black;
                Handles.DrawLine(segment[0], segment[1]);
                Handles.DrawLine(segment[3], segment[2]);
                Handles.DrawBezier(segment[0], segment[3], segment[1], segment[2], Color.green, null, 5);
            }
            for (int i = 0; i < curve.NumPoints; i++)
            {
                Handles.color = Color.black;
                Vector2 newPos = Handles.FreeMoveHandle(curve.points[i], Quaternion.identity, .5f, Vector2.zero, Handles.SphereHandleCap);
                if (newPos != curve.points[i])
                {
                    Undo.RecordObject(creator, "Move Position");
                    curve.MovePoint(i, newPos);
                }
            }
        }
        else if (curve.isClosed)
        {
            for (int i = 0; i < curve.NumSegments; i++)
            {
                Vector2[] segment = curve.GetPointsInSegment(i);
                Handles.color = Color.black;
                Handles.DrawLine(segment[0], segment[1]);
                Handles.DrawLine(segment[3], segment[2]);
                Handles.DrawBezier(segment[0], segment[3], segment[1], segment[2], Color.green, null, 5);
            }
            //if (curve.points[curve.points.Count - 1])
            if (!added)
            {
                curve.points.Add(curve.points[curve.points.Count - 1] + Vector2.up);
                curve.points.Add(curve.points[0] + Vector2.down);
                added = true;
            }
            Handles.DrawBezier(curve.points[curve.points.Count - 3], curve.points[0], curve.points[curve.points.Count - 2], curve.points[curve.points.Count - 1], Color.red, null, 5);
            Handles.DrawLine(curve.points[curve.points.Count - 3], curve.points[curve.points.Count - 2]);
            Handles.DrawLine(curve.points[0], curve.points[curve.points.Count - 1]);
            for (int i = 0; i < curve.NumPoints; i++)
            {
                Handles.color = Color.black;
                Vector2 newPos = Handles.FreeMoveHandle(curve.points[i], Quaternion.identity, .5f, Vector2.zero, Handles.SphereHandleCap);
                if (newPos != curve.points[i])
                {
                    Undo.RecordObject(creator, "Move Position");
                    curve.MovePoint(i, newPos);
                }
            }
        }
    }

    private void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(creator, "New Segment");
            curve.AddSegment(mousePosition);
        }
        else if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.A)
        {
            creator.curve = new Curve();
            curve = creator.curve;
            curve.StartCurve(Vector2.zero);
            removed = false;
            added = false;
            curve.isClosed = false;
        }
    }

    private void OnEnable()
    {
        creator = target as BezierCurves;
        if (creator.curve == null)
        {
            creator.curve = new Curve();
        }
        curve = creator.curve;
        curve.StartCurve(Vector2.zero);
    }

    private void OnSceneGUI()
    {
        Input();
        Draw();
        creator.curve = curve;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Separation");
        separation = Mathf.Abs(EditorGUILayout.FloatField(separation));

        GUILayout.Label("Precision step");
        prec_step = Mathf.Abs(EditorGUILayout.FloatField(prec_step));

        //Boton para crear scriptable object de curva
        if (GUILayout.Button("*honk* *honk*"))
        {
            float actDist = 0;

            List<Vector2> points = new List<Vector2>();
            int segment = 0;
            float t = prec_step;

            Vector2 handler1 = curve.GetPointsInSegment(segment)[1];
            Vector2 handler2 = curve.GetPointsInSegment(segment)[2];
            Vector2 anchor1 = curve.GetPointsInSegment(segment)[0];
            Vector2 anchor2 = curve.GetPointsInSegment(segment)[3];

            Vector2 prevpoint = BezierInt.CubicBezier(anchor1, handler1, handler2, anchor2, 0);
            Vector2 actpoint;
            while (segment < curve.NumSegments) 
            {
                handler1 = curve.GetPointsInSegment(segment)[1];
                handler2 = curve.GetPointsInSegment(segment)[2];
                anchor1 = curve.GetPointsInSegment(segment)[0];
                anchor2 = curve.GetPointsInSegment(segment)[3];

                actpoint = BezierInt.CubicBezier(anchor1, handler1, handler2, anchor2, t);
                float distbet = (actpoint - prevpoint).magnitude;
                actDist += distbet;
                if (actDist >= separation)
                {
                    points.Add(actpoint);
                    actDist = 0;
                }
                if (t + prec_step > 1)
                {
                    segment += 1;
                    //Parte fraccional del nuevo step
                    t = (t + prec_step) - Mathf.Floor(t + prec_step);
                }
                else
                {
                    t += prec_step;
                }
                prevpoint = actpoint;
            }
            //Elimina duplicados adyacentes
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i] == points[i + 1])
                {
                    points.RemoveAt(i + 1);
                    i -= 1;
                }
            }
            
            CurveScriptObject final = ScriptableObject.CreateInstance<CurveScriptObject>();
            final.CreateCurve(points.ToArray(), curve.isClosed);
            AssetDatabase.CreateAsset(final, "Assets/curva.asset");
            AssetDatabase.SaveAssets();
            
        }
    }
}
