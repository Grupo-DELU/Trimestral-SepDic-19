using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accion que mueve un enemigo kamikaze dependiendo de su estado
/// </summary>
[CreateAssetMenu(fileName = "A_KamikazeMove", menuName = "Actions/Enemies/Kamikaze/KamikazeMove", order = 0)]
public class KamikazeEnemyMove : Action
{
    public override void doAction(ActionManager manager)
    {
        KamikazeEnemyManager km = manager as KamikazeEnemyManager;
        km.MoveWithVel(GetVelocity(km));
    }

    public Vector2 GetDirection(KamikazeEnemyManager manager)
    {
        return manager.lastPlayerDir.normalized;
    }

    public Vector2 GetVelocity(KamikazeEnemyManager manager)
    {
        if (!manager.IsLaunched)
        {
            return GetDirection(manager) * manager.fMaxSpeed * manager.fSpeed * Time.fixedDeltaTime;
        }
        return GetDirection(manager) * manager.LaunchSpeed * manager.fSpeed * Time.fixedDeltaTime;
    }
}
