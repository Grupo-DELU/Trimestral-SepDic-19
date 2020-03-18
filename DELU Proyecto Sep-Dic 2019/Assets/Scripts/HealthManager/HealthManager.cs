using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HPEvents : UnityEvent<int, int> { }
public class HealthManager : MonoBehaviour
{
    /// <summary>
    /// Retorna la vida actual
    /// </summary>
    public int ActualHP
    {
        get { return iHP; }
    }
    /// <summary>
    /// Vida actual
    /// </summary>
    [SerializeField]
    private int iHP;
    public int MaxHP
    {
        get { return iMaxHP; }
    }
    /// <summary>
    /// Vida maxima
    /// </summary>
    [SerializeField]
    private int iMaxHP = 10;
    /// <summary>
    /// Estado de invencibilidad activado
    /// </summary>
    [SerializeField]
    private bool bInmortal = false;

    /// <summary>
    /// Tiempo de proteccion de la nave al revivir
    /// </summary>
    /// <remarks> Si es 0 no tiene</remarks>
    [SerializeField]
    private float fReviveProtTime = 1f;

    /// <summary>
    /// Evento de cambio de vida
    /// </summary>
    public HPEvents onLifeChange = new HPEvents();
    /// <summary>
    /// Evento de perdida de vida
    /// </summary>
    public HPEvents onLifeLoss = new HPEvents();
    /// <summary>
    /// Evento de curacion
    /// </summary>
    public HPEvents onLifeGain = new HPEvents();
    /// <summary>
    /// Evento de vida agotada
    /// </summary>
    public HPEvents onDepletedLife = new HPEvents();
    /// <summary>
    /// Evento de vida recargada full
    /// </summary>
    public HPEvents onFullLife = new HPEvents();
    /// <summary>
    /// Evento de inmortalidad activada
    /// </summary>
    public HPEvents onActivateInm = new HPEvents();
    /// <summary>
    /// Evento de inmortalidad desactivada
    /// </summary>
    public HPEvents onDeactivateInm = new HPEvents();
    /// <summary>
    /// Evento de resucitacion
    /// </summary>
    public HPEvents onRevive = new HPEvents();

#if UNITY_EDITOR
    [SerializeField]
    private bool bDebug = false;
#endif

    /// <summary>
    /// Corutina de inmortalidad
    /// </summary>
    private Coroutine cInmortalityRoutine;

    private void Awake()
    {
        // En caso del jugador, liga la inmortalidad al roll
        // TODO
        // [] El sist de movimiento debe de ser el mismo para todas las naves, la cosa es que pense eso despues asi que arreglar
        // [X] Todas las naves deberian de tener roll asociado a la invencibilidad
        if (TryGetComponent(out PlayerMovement pm))
        {
            pm.onPlayerRoll.AddListener(ActivateInmortality);
            pm.onPlayerStopRoll.AddListener(CancelInmortality);
        }
        else if (TryGetComponent(out ShipMovement sm))
        {
            sm.onRoll.AddListener(ActivateInmortality);
            sm.onStopRoll.AddListener(CancelInmortality);
        }
    }
    private void Start()
    {
        ReplenishLife();
        // esto es para las balas
        if (TryGetComponent(out ShipCollider coll))
        {
            coll.onCollision.AddListener((a, b) => RemoveLife(1));
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (CompareTag("Player") && bDebug)
        {
            if (Input.GetKeyDown(KeyCode.K)) RemoveLife(100000000);
            else if (Input.GetKeyDown(KeyCode.R)) AddLife(1);
            else if (Input.GetKeyDown(KeyCode.T)) RemoveLife(1);
        }
    }
#endif

    /// <summary>
    /// Agrega vida a la nave
    /// </summary>
    /// <param name="toAdd">Vida a agregar</param>
    public void AddLife(int toAdd)
    {
        int oldHP = iHP;
        if (iHP + toAdd >= iMaxHP)
        {
            iHP = iMaxHP;
            onFullLife.Invoke(oldHP, iHP);
        }
        else iHP = iHP + toAdd;
        onLifeGain.Invoke(oldHP, iHP);
        if (oldHP != iHP) onLifeChange.Invoke(oldHP, iHP);
    }

    /// <summary>
    /// Quita vida a la nave
    /// </summary>
    /// <param name="toRemove">Vida a quitar</param>
    public void RemoveLife(int toRemove)
    {
        if (bInmortal) return;
        int oldHP = iHP;
        if (iHP - toRemove <= 0)
        {
            iHP = 0;
            onDepletedLife.Invoke(oldHP, iHP);
        }
        else iHP = iHP - toRemove;
        onLifeLoss.Invoke(oldHP, iHP);
        if (oldHP != iHP) onLifeChange.Invoke(oldHP, iHP);
    }


    /// <summary>
    /// Devuelve toda la vida a la nave
    /// </summary>
    public void ReplenishLife()
    {
        int oldHP = iHP;
        iHP = iMaxHP;
        onLifeChange.Invoke(oldHP, iHP);
    }

    /// <summary>
    /// Por los momentos solo le sube la vida, deberia de agregar un evento
    /// </summary>
    public void Revive()
    {
        iHP = iMaxHP;
        ActivateInmortality(fReviveProtTime);
        onLifeChange.Invoke(0, iHP);
        onRevive.Invoke(0, iMaxHP);
    }

    public void ActivateInmortality()
    {
        bInmortal = true;
        onActivateInm.Invoke(iHP, iHP);
    }

    public void CancelInmortality()
    {
        bInmortal = false;
    }

    /// <summary>
    /// Llama la corutina de inmortalidad
    /// </summary>
    /// <param name="time">Duracion inmortalidad</param>
    public void ActivateInmortality(float time)
    {
        if (time <= 0) return;
        if (cInmortalityRoutine != null) StopCoroutine(cInmortalityRoutine);
        cInmortalityRoutine = StartCoroutine(TimerInmortality(time));
    }

    /// <summary>
    /// Cancela la inmortalidad
    /// </summary>
    public void CancelInmortalityRoutine()
    {
        if (cInmortalityRoutine == null) return;
        StopCoroutine(cInmortalityRoutine);
        bInmortal = false;
    }

    /// <summary>
    /// Timer de la inmortalidad
    /// </summary>
    /// <param name="time">Tiempo a ser inmortal</param>
    /// <returns>Corutina</returns>
    private IEnumerator TimerInmortality(float time)
    {
        bInmortal = true;
        onActivateInm.Invoke(iHP, iHP);
        yield return new WaitForSeconds(time);
        bInmortal = false;
        onDeactivateInm.Invoke(iHP, iHP);
    }
}
