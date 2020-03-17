using UnityEngine;

[RequireComponent(typeof(HealthManager))]
public class ShipDeathSystem : MonoBehaviour
{
    private HealthManager hm = null;

    private void Awake()
    {
        hm = GetComponent<HealthManager>();
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


    private void DisableShooting()
    {
        GetComponent<ShipShootingSystem>().SetSystemOnOff(false);
    }


    private void DisableMovement()
    {
        GetComponent<ShipMovement>().SetSystemOnOff(false);
    }


    private void DisableAI()
    {
        GetComponent<EnemyShipManager>().SetAIStatus(false);
    }
}
