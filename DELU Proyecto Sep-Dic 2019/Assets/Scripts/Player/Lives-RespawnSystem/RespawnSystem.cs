using UnityEngine;

/// <summary>
/// Sistema de respawneo de las naves
/// </summary>
/// <remarks> Sirve para todas las naves </remarks>
[RequireComponent(typeof(HealthManager), typeof(LivesSystem))]
public class RespawnSystem : MonoBehaviour
{
    /// <summary>
    /// Punto de respawn de la nave.
    /// </summary>
    [SerializeField]
    private Transform respawnPoint = null;

    private HealthManager hm = null;
    private LivesSystem lm = null;

    private void Awake()
    {
        //if (respawnPoint == null) Debug.LogError("No hay punto de respawn! Agrega uno!", gameObject);
        hm = GetComponent<HealthManager>();
        lm = GetComponent<LivesSystem>();
    }

    private void Start()
    {
        //lm.onLiveLoss.AddListener((a) => RespawnShip());
    }

    /// <summary>
    /// Respawnea la nave y ... hmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm sera que este manda una senal mejor y ya?
    /// </summary>
    public void RespawnShip()
    {
        //if (lm.LivesDepleted()) return;
        transform.position = respawnPoint.position;
        hm.Revive();
    }
}
