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

    [SerializeField]
    private bool bSeek = true;

    private Transform playerT;
    private Vector2 target;

    [SerializeField]
    private int timeToUpate = 10;
    private float updateCounter = 0;
    /// <summary>
    /// Distancia a la que dejara de seguir al jugador
    /// </summary>
    [SerializeField]
    private float updateDist = 5f;

    public float fMaxSpeed = 10;
    [Range(-1f, 1f)]
    public float fSpeed = 1;

    public Vector2 velocity = Vector2.zero;
    public Vector2 dir = Vector2.zero;
    private Rigidbody2D rb2d;


    private void Awake()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        Debug.Log("Hola");
        base.Start();
    }

    private void OnEnable()
    {
        target = playerT.position;
        dir = (Vector2)playerT.position - (Vector2)transform.position;
        bSeek = true;
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
                target = playerT.position;
                dir = (Vector2)playerT.position - (Vector2)transform.position;
                //Debug.Log("Nueva posicion: " + target);
                updateCounter = 0;
            }

            if (distSqr < updateDist * updateDist)
            {
                bSeek = false;
            }
        }
        executeAction("KamikazeMove");
        updateCounter += Time.deltaTime;
    }

    public void MoveWithVel(Vector2 velocity)
    {
        this.velocity = velocity;
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
