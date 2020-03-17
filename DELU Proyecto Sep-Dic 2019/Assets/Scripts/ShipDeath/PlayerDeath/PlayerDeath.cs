using UnityEngine;

public class PlayerDeath : ShipDeathSystem
{
    private LivesSystem lm = null;
    private RespawnSystem rs = null;

    protected override void Awake()
    {
        base.Awake();
        rs = GetComponent<RespawnSystem>();
        lm = GetComponent<LivesSystem>();
    }

    protected override void KillShip()
    {
        // NOTA: Si vas a llamar animaciones de muerte y cosas asi, puedes hacerlo usando los eventos de muerte
        // como onLifeDepleted del HealthManager o el onLivesDepleted de LivesSystem (para el gameover). Esto puede
        // hacerse por script o arrastrando manualmente y cosas asi!
        lm.LooseLife();
        if (CanRespawn())
        {
            rs.RespawnShip();
        }
        else
        {
            DisableShooting();
            //DisableMovement();
            GetComponent<PlayerMovement>().SetSystemOnOff(false);
            gameObject.SetActive(false);
        }
    }

    private bool CanRespawn()
    {
        return !lm.LivesDepleted();
    }
}
