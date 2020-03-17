using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Eventos de vidas de las naves
/// </summary>
/// <remarks>Envia consigo la NUEVA CANTIDAD DE VIDA</remarks>
[System.Serializable]
public class LivesEvents : UnityEvent<int> { };

/// <summary>
/// Sistema de vida de las naves
/// </summary>
/// <remarks>
/// Sirve para todas las naves y es mejor acompanarlo con el sistema
/// de respawn. Ademas, pierdes cuando las vidas LLEGUEN A -1, NO 0.
/// </remarks>
[RequireComponent(typeof(HealthManager))]
public class LivesSystem : MonoBehaviour
{
    /// <summary>
    /// Cantidad maxima de vidas de la nave
    /// </summary>
    [SerializeField]
    private int iMaxLives = 3;
    /// <summary>
    /// Vidas iniciales de la nave
    /// </summary>
    [SerializeField]
    private int iInitialLives = 3;

    /// <summary>
    /// Vidas de la nave
    /// </summary>
    [SerializeField]
    private int iLives = 3;

    /// <summary>
    /// Llamado cuando la nave pierde una vida
    /// </summary>
    public LivesEvents onLiveLoss = new LivesEvents();
    /// <summary>
    /// Llamado cuando la nave gana una vida
    /// </summary>
    public LivesEvents onLiveGain = new LivesEvents();
    /// <summary>
    /// Llamado cuando la nave gana una vida
    /// </summary>
    public LivesEvents onLiveChange = new LivesEvents();
    /// <summary>
    /// Llamado cuando la nave agota todas sus vidas
    /// </summary>
    public LivesEvents onLivesDepleted = new LivesEvents();

    private HealthManager hm = null;

    private void Awake()
    {
        hm = GetComponent<HealthManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (iMaxLives <= -1)
        {
            iMaxLives = 3;
            Debug.LogWarning("Settea las vidas maximas tal que iMaxLives > -1", gameObject);
        }
        if (iInitialLives > iMaxLives)
        {
            iInitialLives = iMaxLives;
            iLives = iMaxLives;
            Debug.LogWarning("Settea las vidas de la nave tal que iLives <= iMaxLives", gameObject);
        }
        else
        {
            iLives = iInitialLives;
        }
        //hm.onDepletedLife.AddListener((a, b) => LooseLife()); Puede hacer conflicto con PlayerDeath, se chequea si el player tiene vida y luego se baja la vida seria F
    }

    public void ReplenishLives()
    {
        iLives = iInitialLives;
    }
    /// <summary>
    /// Quita una vida a la nave
    /// </summary>
    public void LooseLife()
    {
        if (iLives <= -1)
        {
            Debug.LogWarning("Ya estas muerto! No deberias perder mas vidas", gameObject);
            return;
        }
        iLives -= 1;
        if (iLives == -1) onLivesDepleted.Invoke(iLives);
        onLiveLoss.Invoke(iLives);
        onLiveChange.Invoke(iLives);
    }

    /// <summary>
    /// Le da una vida a la nave 
    /// </summary>
    public void GainLife()
    {
        if (iLives < iMaxLives)
        {
            iLives += 1;
            onLiveGain.Invoke(iLives);
            onLiveChange.Invoke(iLives);
        }
    }

    /// <summary>
    /// Indica si la nave se quedo sin vidas
    /// </summary>
    /// <returns>true si se quedo sin vidas/false en otro caso</returns>
    public bool LivesDepleted()
    {
        return iLives <= -1;
    }
}
