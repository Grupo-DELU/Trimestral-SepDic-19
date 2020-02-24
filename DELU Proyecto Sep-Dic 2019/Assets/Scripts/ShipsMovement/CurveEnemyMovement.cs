using UnityEngine;

public class CurveEnemyMovement : ShipMovement
{
    /// <summary>
    /// Curva/patron a seguir
    /// </summary>
    [SerializeField]
    private CurveScriptObject path = null;

    /// <summary>
    /// Indice del punto a moverse en path
    /// </summary>
    private int iTargetIndex = 0;
    /// <summary>
    /// A que distancia cambia hacia el siguiente punto
    /// </summary>
    [SerializeField]
    private float fStoppingDist = 0.5f;

    /// <summary>
    /// Punto actual de la trayectoria en Vector2
    /// </summary>
    private Vector2 vTargetPoint = Vector2.zero;


    /// <summary>
    /// Setea/calcula el proximo punto target del ennemigo
    /// </summary>
    public void NextPoint()
    {
        if (path == null)
        {
            Debug.LogError("No hay curva para que la nave se mueva!", gameObject);
        }
        iTargetIndex = (iTargetIndex + 1) % path.points.Length;
        vTargetPoint = path.points[iTargetIndex];
    }


    /// <summary>
    /// Calcula la direccion al punto objetivo de la nave
    /// </summary>
    /// <returns>Direccion al punto objetivo de la nave</returns>
    public Vector2 CalculatePointDir()
    {
        if (path == null)
        {
            Debug.LogError("No hay curva para que la nave se mueva!", gameObject);
        }
        return (vTargetPoint - (Vector2)transform.position).normalized;
    }


    /// <summary>
    /// Mueve la nave en la curva
    /// </summary>
    public void MoveInCurve()
    {
        if (path == null)
        {
            Debug.LogError("No hay curva para que la nave se mueva!", gameObject);
        }
        if (fStoppingDist * fStoppingDist >= Vector2.SqrMagnitude(vTargetPoint - (Vector2)transform.position))
        {
            NextPoint();
        }
        Move(CalculatePointDir());
    }
}
