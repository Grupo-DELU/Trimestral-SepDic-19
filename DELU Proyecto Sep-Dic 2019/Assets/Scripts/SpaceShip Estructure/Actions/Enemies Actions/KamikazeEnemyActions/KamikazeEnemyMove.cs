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
        km.movementSyst.Move(km.GetLastPlayerDir().normalized);
    }

}
