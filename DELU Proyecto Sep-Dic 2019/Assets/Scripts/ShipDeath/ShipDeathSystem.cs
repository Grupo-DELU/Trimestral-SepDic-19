using UnityEngine;

[RequireComponent(typeof(HealthManager))]
public class ShipDeathSystem : MonoBehaviour
{
    protected HealthManager hm = null;
    protected ShipShootingSystem ss = null;
    protected ShipMovement ms = null;

    protected virtual void Awake()
    {
        hm = GetComponent<HealthManager>();
        ss = GetComponent<ShipShootingSystem>();
        ms = GetComponent<ShipMovement>();
    }

    private void Start()
    {
        hm.onDepletedLife.AddListener((a, b) => KillShip());
    }


    protected virtual void KillShip()
    {
        DisableShooting();
        DisableMovement();
        DisableAI();
        gameObject.SetActive(false);
    }


    protected void DisableShooting()
    {
        ss.SetSystemOnOff(false);
    }


    protected void DisableMovement()
    {
        ms.SetSystemOnOff(false);
    }

    // Solo en caso de que sea nave enemiga
    private void DisableAI()
    {
        GetComponent<EnemyShipManager>().SetAIStatus(false);
    }
}
