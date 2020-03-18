using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoPU : PowerUp
{
    public override void ApplyPowerUp(GameObject toApply)
    {
        Time.timeScale = 0.5f;
    }

    public override void DeApplyPowerUp(GameObject toDeApply)
    {
        Time.timeScale = 1f;
    }
}
