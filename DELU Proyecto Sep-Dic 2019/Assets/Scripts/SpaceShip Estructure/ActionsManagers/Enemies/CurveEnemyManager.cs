using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveEnemyManager : ActionManager
{
    /// <summary>
    /// Curva/patron a seguir
    /// </summary>
    [SerializeField]
    private CurveScriptObject path;

    /// <summary>
    /// Indica si la nave se esta moviendo
    /// </summary>
    [SerializeField]
    private bool bIsMoving = true;

    /// <summary>
    /// Posicion del punto a moverse
    /// </summary>
    public Vector2 Target
    {
        get { return vTargetPos; }
    }

    /// <summary>
    /// Posicion del punto a moverse
    /// </summary>
    private Vector2 vTargetPos;
    /// <summary>
    /// Indice del punto a moverse en path
    /// </summary>
    private int iTargetIndex = 0;

    protected override void Start()
    {
        base.Start();
        vTargetPos = path.points[0];    
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsMoving) executeAction(ActionTags.EnemyCurveMove);
    }

    /// <summary>
    /// Setea/calcula el proximo punto target del ennemigo
    /// </summary>
    private void NextPoint()
    {
        iTargetIndex = (iTargetIndex + 1) % path.points.Length;
        vTargetPos = path.points[iTargetIndex];
    }
}
