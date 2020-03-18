using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLivePU : PowerUp
{
    /// <summary>
    /// Vida de la nave a recuperar
    /// </summary>
    [SerializeField]
    [Range(1, 3)]
    private int extra = 1;

    public override void ApplyPowerUp(GameObject toApply)
    {
        if (duration != 0) duration = 0;
        toApply.GetComponent<LivesSystem>().GainLife(extra);
    }

    public override void DeApplyPowerUp(GameObject toDeApply) { }
}
