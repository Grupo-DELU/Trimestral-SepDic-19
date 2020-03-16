using UnityEngine;

[RequireComponent(typeof(HealthManager), typeof(LivesSystem), typeof(RespawnSystem))]
public class ShipDeathSystem : MonoBehaviour
{
    private LivesSystem ls = null;
    private RespawnSystem rs = null;
    private HealthManager hm = null;

    private void Awake()
    {
        ls = GetComponent<LivesSystem>();
        rs = GetComponent<RespawnSystem>();
        hm = GetComponent<HealthManager>();
    }

    private void Start()
    {
        hm.onDepletedLife.AddListener((a, b) => KillShip());
    }


    protected virtual void KillShip()
    {
        if (!ls.LivesDepleted())
        {
            rs.RespawnShip();
        }
        else
        {
            // has una animacion primero
            Destroy(gameObject);
        }
    }
}
