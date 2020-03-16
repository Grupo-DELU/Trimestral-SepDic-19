using UnityEngine;

/// <summary>
/// Accion que simplemente empieza la carga de la nave kamikaze
/// </summary>
[CreateAssetMenu(fileName = "A_ChargeKamikaze", menuName = "Actions/Enemies/Kamikaze/KamikazeCharge", order = 1)]
public class KamikazeChargeAction : Action
{
    public override void doAction(ActionManager manager)
    {
        KamikazeEnemyManager km = manager as KamikazeEnemyManager;
        km.StartCharge();
    }
}
