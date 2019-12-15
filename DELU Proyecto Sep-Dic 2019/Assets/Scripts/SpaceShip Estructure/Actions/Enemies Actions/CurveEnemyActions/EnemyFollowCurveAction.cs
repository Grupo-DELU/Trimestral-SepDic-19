using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowCurveAction : Action
{
    public override void doAction(ActionManager manager)
    {
        CurveEnemyManager cm = manager as CurveEnemyManager;
        cm.MoveWithVel(GetVelocity(cm));
        if (GetVelocity(cm) != Vector2.zero) Debug.Log("Velocidad: " + GetVelocity(manager));
        Debug.Log("Enemigo moviendose");
    }

    private Vector2 GetDirection(ActionManager manager)
    {
        CurveEnemyManager cm = manager as CurveEnemyManager;
        Vector2 targ = (cm.Target - (Vector2)cm.transform.position);
        //chequear esto en realidad
        float dist = targ.magnitude;
        //treshold modifical
        if (dist < 0.5f)
        {
            cm.NextPoint();
            return targ;
        }
        //Debug.Log("zorra maldita");
        return (cm.Target - (Vector2)cm.transform.position).normalized;
    }

    private Vector2 GetVelocity(ActionManager manager)
    {
        CurveEnemyManager cm = manager as CurveEnemyManager;
        return GetDirection(cm) * cm.fSpeed * cm.fMaxSpeed;
    }

}
