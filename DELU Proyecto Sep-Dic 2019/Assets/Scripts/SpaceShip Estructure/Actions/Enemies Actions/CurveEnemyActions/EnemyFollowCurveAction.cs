using UnityEngine;

/// <summary>
/// Accion con la cual un enemigo sigue una curva
/// </summary>
[CreateAssetMenu(fileName = "A_EnemyFollowCurve", menuName = "Actions/Enemies/SimpleCurveEnemy/SimpleFollowCurve", order = 0)]
public class EnemyFollowCurveAction : Action
{
    public override void doAction(ActionManager manager)
    {
        EnemyShipManager cm = manager as EnemyShipManager;
        (cm.movementSyst as CurveEnemyMovement).MoveInCurve();
    }
}
