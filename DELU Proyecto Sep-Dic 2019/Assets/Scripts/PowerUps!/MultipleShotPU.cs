using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleShotPU : PowerUp
{
    /// <summary>
    /// Numero de disparos a aumentar a la nave
    /// </summary>
    [SerializeField]
    [Range(2, 10)]
    private int numberOfShots = 3;

    /// <summary>
    /// Cantidad de disparos antes de tomar el PowerUp!
    /// </summary>
    private int previousShots = 0;

    private ShipShootingSystem ss = null;

    public override void ApplyPowerUp(GameObject toApply)
    {
        ss = toApply.GetComponent<ShipShootingSystem>();
        previousShots = ss.ShotNumber;
        ss.SetShotNumber(ss.ShotNumber + numberOfShots);
    }

    public override void DeApplyPowerUp(GameObject toDeApply)
    {
        ss.SetShotNumber(previousShots);
    }
}
