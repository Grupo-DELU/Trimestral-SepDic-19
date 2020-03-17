using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    public static PlayerFinder Finder { get; private set; }

    /// <summary>
    /// GameObjectJugador
    /// </summary>
    [SerializeField]
    private GameObject player;
    public static GameObject Player { get { return Finder.player; } }

    private PlayerMovement pm;
    private PlayerShootingController ps;

    void Awake()
    {
        #region Singleton
        if (Finder != null && Finder != this)
        {
            Debug.LogWarning("Multiples finder del jugador");
        }
        Finder = this;
        #endregion

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        pm = player.GetComponent<PlayerMovement>();
        ps = player.GetComponent<PlayerShootingController>();
    }


    /// <summary>
    /// Se encarga de desactivar los sistemas del jugador en caso de pausa
    /// </summary>
    public void PausePlayer()
    {
        pm.SetSystemOnOff(false);
        ps.SetSystemOnOff(false);
    }

    /// <summary>
    /// Se encarga de activar los sistemas del jugador en caso de quitar la pausa
    /// </summary>
    public void ResumePlayer()
    {
        pm.SetSystemOnOff(true);
        ps.SetSystemOnOff(true);
    }
}
