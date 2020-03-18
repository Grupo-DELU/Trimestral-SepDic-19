using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediKitPU : PowerUp
{
    /// <summary>
    /// Vida de la nave a recuperar
    /// </summary>
    [SerializeField]
    [Range(1,10)]
    private int toHeal = 3;

    public override void ApplyPowerUp(GameObject toApply)
    {
        if (duration != 0) duration = 0;
        toApply.GetComponent<HealthManager>().AddLife(toHeal);
    }

    public override void DeApplyPowerUp(GameObject toDeApply) { }
}
