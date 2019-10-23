using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HPEvents : UnityEvent<int, int> { }
public class HealthManager : MonoBehaviour
{
    public int ActualHP
    {
        get { return iHP; }
    }
    [SerializeField] private int iHP;
    [SerializeField] private int iMaxHP = 10;
    [SerializeField] private bool bInmortal = false;
    [SerializeField] private bool bDebug = false;

    public HPEvents onLifeChange = new HPEvents();
    public HPEvents onLifeLoss = new HPEvents();
    public HPEvents onLifeGain = new HPEvents();
    public HPEvents onDepletedLife = new HPEvents();
    public HPEvents onFullLife = new HPEvents();
    public HPEvents onActivateInm = new HPEvents();
    public HPEvents onDeactivateInm = new HPEvents();

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

    public void Revive()
    {
        iHP = iMaxHP;
    }

    public void ActivateInmortality(float time)
    {
        if (cInmortalityRoutine != null) StopCoroutine(cInmortalityRoutine);
        cInmortalityRoutine = StartCoroutine(TimerInmortality(time));
    }

    public void CancelInmortality()
    {
        StopCoroutine(cInmortalityRoutine);
    }
    
    private IEnumerator TimerInmortality(float time)
    {
        bInmortal = true;
        onActivateInm.Invoke(iHP, iHP);
        yield return new WaitForSeconds(time);
        bInmortal = false;
        onDeactivateInm.Invoke(iHP, iHP);
    }
}
