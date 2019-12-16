using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KamikazeEnemyManager : ActionManager
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
    private bool bCharging = false;
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
    /// Rapidez maxima de la nave
    /// </summary>
    public float fMaxSpeed = 10;
    /// <summary>
    /// Rapidez de la nave
    /// </summary>
    [Range(-1f, 1f)]
    public float fSpeed = 1;

    /// <summary>
    /// Velocidad de la nave
    /// </summary>
    public Vector2 velocity = Vector2.zero;
    /// <summary>
    /// Ultima direccion conocida hacia el jugador
    /// </summary>
    public Vector2 lastPlayerDir = Vector2.zero;
    /// <summary>
    /// Transform del jugador
    /// </summary>
    private Transform playerT;
    private Rigidbody2D rb2d;

    private Coroutine chargeRoutine = null;

    private void Awake()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        lastPlayerDir = (Vector2)playerT.position - (Vector2)transform.position;
        bSeek = true;
        bIsMoving = true;
        bCharging = false;
        bLaunched = false;
    }

    private void Update()
    {
        //Si ya pasaron los frames de update posicion jugador
        //updatea la posicion
        float distSqr = ((Vector2)transform.position - (Vector2)playerT.position).sqrMagnitude;
        if (bSeek)
        {
            if (updateCounter >= timeToUpate)
            {
                lastPlayerDir = (Vector2)playerT.position - (Vector2)transform.position;
                //Debug.Log("Nueva posicion: " + target);
                updateCounter = 0;
            }
            if (distSqr < updateDist * updateDist)
            {
                bSeek = false;
                executeAction("ChargeKamikaze");
            }
            updateCounter += Time.deltaTime;
        }
  
        if (bIsMoving) executeAction("KamikazeMove");
    }

    /// <summary>
    /// Cambia la velocidad de la nave
    /// </summary>
    /// <param name="velocity">Nueva velocidad de la nave</param>
    public void MoveWithVel(Vector2 velocity)
    {
        this.velocity = velocity;
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
        bCharging = true;
        bIsMoving = false;
        bLaunched = false;
        yield return new WaitForSeconds(fChargeTime);      
        bCharging = false;
        bIsMoving = true;
        bLaunched = true;
        lastPlayerDir = (Vector2)playerT.position - (Vector2)transform.position;

        //executeAction("LaunchKamikaze");
    }

    private void FixedUpdate()
    {
        if (velocity != Vector2.zero)
        {
            rb2d.MovePosition((Vector2)transform.position + velocity);
            velocity = Vector2.zero;
        }
    }
}
