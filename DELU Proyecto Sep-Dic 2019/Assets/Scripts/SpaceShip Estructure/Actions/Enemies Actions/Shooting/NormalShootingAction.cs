using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accion con la cual un enemigo dispara
/// </summary>
[CreateAssetMenu(fileName = "A_EnemyShoot", menuName = "Actions/Enemies/EnemyShoot", order = 0)]
public class NormalShootingAction : Action
{
    public override void doAction(ActionManager manager)
    {
        EnemyShipManager em = manager as EnemyShipManager;
        em.shootingSyst.Shoot();
    }
}
