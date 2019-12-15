using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemyManager : ActionManager
{
    /// <summary>
    /// Indica si la nave se esta moviendo
    /// </summary>
    [SerializeField]
    private bool bIsMoving = true;

    private Transform playerT;
    private Vector2 target;

    [SerializeField]
    private int timeToUpate = 10;
    private float updateCounter = 0;

    public float fMaxSpeed = 10;
    [Range(-1f, 1f)]
    public float fSpeed = 1;

    public Vector2 velocity = Vector2.zero;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Start()
    {
        Debug.Log("Hola");
        base.Start();
    }

    private void OnEnable()
    {
        target = playerT.position;
    }

    private void Update()
    {
        //Si ya pasaron los frames de update posicion jugador
        //updatea la posicion
        if (updateCounter >= timeToUpate)
        {
            target = playerT.position;
            Debug.Log("Nueva posicion: " + target);
            updateCounter = 0;
        }
        updateCounter += Time.deltaTime;
    }
}
