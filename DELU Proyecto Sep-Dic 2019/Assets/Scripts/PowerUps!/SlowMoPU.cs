using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoPU : PowerUp
{
    public override void ApplyPowerUp(GameObject toApply)
    {
        GameStateManager.Manager.SetTimeScale(0.5f);
        ECSPhysicsManager.Manager.SetTimeScale(0.5f);
    }

    public override void DeApplyPowerUp(GameObject toDeApply)
    {
        GameStateManager.Manager.SetTimeScale(1f);
        ECSPhysicsManager.Manager.SetTimeScale(1f);
    }
}
