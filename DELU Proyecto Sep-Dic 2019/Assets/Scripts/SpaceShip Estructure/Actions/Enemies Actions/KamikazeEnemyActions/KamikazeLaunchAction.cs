using UnityEngine;

/// <summary>
/// Accion que lanza a un enemigo Kamikaze
/// </summary>
[CreateAssetMenu(fileName = "A_KamikazeLaunch", menuName = "Actions/Enemies/Kamikaze/KamikazeLaunch", order = 2)]
public class KamikazeLaunchAction : Action
{
    public override void doAction(ActionManager manager)
    {
        KamikazeEnemyManager km = manager as KamikazeEnemyManager;
        km.movementSyst.SetSpeed(km.GetLaunchSpeed());
    }
}
