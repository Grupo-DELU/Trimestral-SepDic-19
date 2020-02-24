using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemyManager : EnemyShipManager
{
    /// <summary>
    /// Indica si la nave se esta moviendo
    /// </summary>
    [SerializeField]
    private bool bIsMoving = true;

    /// <summary>
    /// Indica si la nave puede updatear la posicion del jugador
    /// </summary>
    [SerializeField]
    private bool bSeek = true;
    //private bool bCharging = false;
    public bool IsLaunched { get { return bLaunched; } }
    private bool bLaunched = false;

    public float LaunchSpeed { get { return fLaunchSpeed; } }
    [SerializeField]
    private float fLaunchSpeed = 10f;

    /// <summary>
    /// Tiempo de carga final
    /// </summary>
    [SerializeField]
    private float fChargeTime = 2;

    /// <summary>
    /// Segundos para que updatee la posicion del jugador
    /// </summary>
    [SerializeField]
    private int timeToUpate = 10;
    /// <summary>
    /// Contador de tiempo para updatear
    /// </summary>
    private float updateCounter = 0;
    /// <summary>
    /// Distancia a la que dejara de actualizar la posicion del jugador
    /// </summary>
    [SerializeField]
    private float updateDist = 5f;

    /// <summary>
    /// Ultima direccion conocida hacia el jugador
    /// </summary>
    private Vector2 lastPlayerDir = Vector2.zero;
    /// <summary>
    /// Transform del jugador
    /// </summary>
    private Transform playerT;

    private Coroutine chargeRoutine = null;

    override protected void Awake()
    {
        base.Awake();
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        lastPlayerDir = (Vector2)playerT.position - (Vector2)transform.position;
        bSeek = true;
        bIsMoving = true;
        //bCharging = false;
        bLaunched = false;
    }

    override protected void Update()
    {
        //Si ya pasaron los frames de update posicion jugador
        //updatea la posicion
        float distSqr = (transform.position.y - playerT.position.y);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, lastPlayerDir), 3f);
        if (bSeek)
        {
            if (updateCounter >= timeToUpate)
            {
                lastPlayerDir = (Vector2)playerT.position - (Vector2)transform.position;
                //Debug.Log("Nueva posicion: " + target);
                updateCounter = 0;
            }
            if (distSqr < updateDist)
            {
                //bSeek = false;
                //executeAction("ChargeKamikaze");
            }
            updateCounter += Time.deltaTime;
        }
  
        if (bIsMoving) executeAction("KamikazeMove");
    }


    /// <summary>
    /// Empieza la corutina de carga de la nave
    /// </summary>
    public void StartCharge()
    {
        if (chargeRoutine != null) StopCoroutine(chargeRoutine);
        chargeRoutine = StartCoroutine(Charge());
    }

    /// <summary>
    /// Carga la nave unos segundos antes de actualziar de nuevo 
    /// la posicion del jugador e ir hacia el
    /// </summary>
    /// <returns></returns>
    private IEnumerator Charge()
    {
        //bCharging = true;
        bIsMoving = false;
        bLaunched = false;
        yield return new WaitForSeconds(fChargeTime);      
        //bCharging = false;
        bIsMoving = true;
        bLaunched = true;
        lastPlayerDir = (Vector2)playerT.position - (Vector2)transform.position;

        //executeAction("LaunchKamikaze");
    }

    public Vector2 GetLastPlayerDir()
    {
        return lastPlayerDir;
    }
}
