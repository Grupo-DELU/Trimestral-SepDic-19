using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accion con la cual un enemigo sigue una curva
/// </summary>
[CreateAssetMenu(fileName = "A_EnemyFollowCurve", menuName = "Actions/Enemies/SimpleCurveEnemy/SimpleFollowCurve", order = 0)]
public class EnemyFollowCurveAction : Action
{
    public override void doAction(ActionManager manager)
    {
        CurveEnemyManager cm = manager as CurveEnemyManager;
        cm.MoveWithVel(GetVelocity(cm));
    }

    private Vector2 GetDirection(CurveEnemyManager manager)
    {
        Vector2 targ = (manager.Target - (Vector2)manager.transform.position);
        //chequear esto en realidad
        float dist = targ.magnitude;
        //treshold modificar
        if (dist < 0.5f)
        {
            manager.NextPoint();
            return targ;
        }
        return (manager.Target - (Vector2)manager.transform.position).normalized;
    }

    private Vector2 GetVelocity(CurveEnemyManager manager)
    {
        return GetDirection(manager) * manager.fSpeed * manager.fMaxSpeed;
    }

}
