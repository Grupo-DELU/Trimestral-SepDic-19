using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasterBulletsPD : PowerDown
{
    public override void ApplyPowerDown()
    {
        PlayerFinder.Player.GetComponent<ShipShootingSystem>().SetFireRateModifier(-3f);
    }

    public override void DeApplyPowerDown()
    {
        PlayerFinder.Player.GetComponent<ShipShootingSystem>().SetFireRateModifier(0);
    }
}
