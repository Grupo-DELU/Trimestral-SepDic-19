using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementEvents : UnityEvent { };

// Para la inmunidad del roll
[RequireComponent(typeof(HealthManager), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Indica si el sistema de movimiento esta activo
    /// </summary>
    [Tooltip("Indica si el sistema esta activo.")]
    [SerializeField]
    private bool bIsActive = true;

    /// <summary>
    /// Indica si el jugador SE PUEDE mover
    /// </summary>
    [Tooltip("Indica si el jugador SE PUEDE mover")]
    [SerializeField]
    private bool bCanMove = true;

    /// <summary>
    /// Indica si el jugador se esta moviendo
    /// </summary>
    [SerializeField]
    private bool bIsMoving = false;

    /// <summary>
    /// Velocidad de movimiento del jugador
    /// </summary>
    [SerializeField]
    private float fMovementSpeed = 10.0f;

    /// <summary>
    /// Direccion de movimiento
    /// </summary>
    private Vector2 vMoveDir = Vector2.zero;
    /// <summary>
    /// PlayerMovement input
    /// </summary>
    private Vector2 vInput = Vector2.zero;

    /// <summary>
    /// Inidica si el jugador esta haciendo un roll
    /// </summary>
    [SerializeField]
    private bool bIsRolling = false;
    /// <summary>
    /// Indica si el jugador puede hacer un roll
    /// </summary>
    [SerializeField]
    private bool bCanRoll = true;

    /// <summary>
    /// Tiempo entre rolls
    /// </summary>
    [Tooltip("Indica COOL DOWN entre rolls")]
    [SerializeField]
    private float fRollCD = 5.0f;

    /// <summary>
    /// Tiempo de cada roll
    /// </summary>
    [Tooltip("Indica duracion del roll")]
    [SerializeField]
    private float fRollTime = 1.5f;

    private Rigidbody2D rb2d;

    private Coroutine cRollRoutine = null;
    private Coroutine cRollCDRoutine = null;

    /// <summary>
    /// LLamado cuando el jugador empieza un roll
    /// </summary>
    public PlayerMovementEvents onPlayerRoll = new PlayerMovementEvents();
    /// <summary>
    /// Llamado cuando el jugador deja de rollear
    /// </summary>
    public PlayerMovementEvents onPlayerStopRoll = new PlayerMovementEvents();
    /// <summary>
    /// Llamado cuando el jugador puede volver a rollear (para indicadores de roll activado)
    /// </summary>
    public PlayerMovementEvents onCanRollAgain = new PlayerMovementEvents();

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

    }


    // Update is called once per frame
    void Update()
    {
        if (!bIsActive) return;
        if (bCanMove)
        {
            if (!bIsMoving)
            {
                vMoveDir.x = Input.GetAxisRaw("Horizontal");
                vMoveDir.y = Input.GetAxisRaw("Vertical");
                bIsMoving = true;
            }
            if (!bIsRolling && bCanRoll)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartRoll();
                    onPlayerRoll.Invoke();
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (bIsMoving)
        {
            rb2d.MovePosition((Vector2)transform.position + vMoveDir.normalized * fMovementSpeed * Time.fixedDeltaTime);
            bIsMoving = false;
        }
    }

    /// <summary>
    /// Empieza la rutina de un Roll
    /// </summary>
    private void StartRoll()
    {
        if (cRollRoutine != null) StopCoroutine(cRollRoutine);
        cRollRoutine = StartCoroutine(Roll());
    }

    /// <summary>
    /// Hace que el jugador haga un roll que lo hace invencible durante un tiempo
    /// </summary>
    private IEnumerator Roll()
    {
        bIsRolling = true;
        yield return new WaitForSeconds(fRollTime);
        bIsRolling = false;

        onPlayerStopRoll.Invoke();
        StartRollCD();
    }


    /// <summary>
    /// Empieza la rutina del CD de un roll
    /// </summary>
    private void StartRollCD()
    {
        if (cRollCDRoutine != null) StopCoroutine(cRollCDRoutine);
        cRollCDRoutine = StartCoroutine(RollCD());
    }

    /// <summary>
    /// Activa el Cool Down entre rolls
    /// </summary>
    private IEnumerator RollCD()
    {
        bCanRoll = false;
        yield return new WaitForSeconds(fRollCD);
        bCanRoll = true;
        onCanRollAgain.Invoke();
    }
}
