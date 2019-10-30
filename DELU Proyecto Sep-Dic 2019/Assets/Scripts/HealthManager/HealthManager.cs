using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private int iHP;
    /// <summary>
    /// Vida maxima
    /// </summary>
    [SerializeField] private int iMaxHP = 10;
    /// <summary>
    /// Estado de invencibilidad activado
    /// </summary>
    [SerializeField] private bool bInmortal = false;
    [SerializeField] private bool bDebug = false;

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

    /// <summary>
    /// Corutina de inmortalidad
    /// </summary>
    private Coroutine cInmortalityRoutine;

    private void Start()
    {
        iHP = iMaxHP;
    }

    private void Update()
    {
        if (CompareTag("Player") && bDebug && Input.GetKeyDown(KeyCode.T))
        {
            RemoveLife(100000000);
        }
    }
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
        else
        {
            iHP = iHP + toAdd;
        }
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
        else
        {
            iHP = iHP - toRemove;
        }
        onLifeLoss.Invoke(oldHP, iHP);
        if (oldHP != iHP) onLifeChange.Invoke(oldHP, iHP);
    }

    /// <summary>
    /// Por los momentos solo le sube la vida, deberia de agregar un evento
    /// </summary>
    public void Revive()
    {
        iHP = iMaxHP;
        onRevive.Invoke(0, iMaxHP);
    }

    /// <summary>
    /// Llama la corutina de inmortalidad
    /// </summary>
    /// <param name="time">Duracion inmortalidad</param>
    public void ActivateInmortality(float time)
    {
        if (cInmortalityRoutine != null) StopCoroutine(cInmortalityRoutine);
        cInmortalityRoutine = StartCoroutine(TimerInmortality(time));
    }

    /// <summary>
    /// Cancela la inmortalidad
    /// </summary>
    public void CancelInmortality()
    {
        if (cInmortalityRoutine == null) return;
        StopCoroutine(cInmortalityRoutine);
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
