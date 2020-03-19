using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IMPORTANTE: NECESITA EL MISMO SETUPS DE LAS NAVES CON LAS COLISIONES Y ESO PARA PODER 
// FUNCIONAR
[RequireComponent(typeof(SpriteRenderer), typeof(ShipCollider))]
public abstract class PowerDown : MonoBehaviour
{
    private SpriteRenderer rndr = null;
    private ShipCollider scs = null;

    /// <summary>
    /// Duracion del PowerDown
    /// </summary>
    [SerializeField]
    protected float duration = 3;

    /// <summary>
    /// Velocidad con la que se mueve el PowerDown
    /// </summary>
    [SerializeField]
    private float speed = 10;

    /// <summary>
    /// Indica si el PowerDown puede moverse
    /// </summary>
    [SerializeField]
    private bool bCanMove = true;


    private void Awake()
    {
        rndr = GetComponent<SpriteRenderer>();
        scs = GetComponent<ShipCollider>();
    }

    private void Start()
    {
        scs.onCollision.AddListener((a, b) => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        scs.SetECSEnabled(false);
    }

    private void OnEnable()
    {
        scs.SetECSEnabled(true);
        // Volvemos a activar el sprite
        rndr.enabled = true;
        bCanMove = true;
    }

    private void Update()
    {
        if (!bCanMove) return;
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        float ratio = Screen.width / Screen.height;
        float halfWidth = ratio * Camera.main.orthographicSize; // EPA, REVISAR ESTO
        if (transform.position.x >=  Camera.main.transform.position.x + halfWidth)
        {
            ApplyPowerDown();
            BeTaken();
        }
    }


    /// <summary>
    /// Se llama cuando el PowerDown es tomado por algo y aplica sus efectos
    /// a este algo.
    /// </summary>
    /// <param name="toApply"></param>
    public abstract void ApplyPowerDown();
    /// <summary>
    /// Desaplica un PowerDown a quien sea que lo haya tomado.
    /// </summary>
    /// <remarks>Cuando llamarse es manejado por el mismo PowerDown</remarks>
    /// <param name="toDeApply"></param>
    public abstract void DeApplyPowerDown();


    public void BeTaken()
    {
        bCanMove = false;
        rndr.enabled = false;
        scs.SetECSEnabled(false);
        StartCoroutine(DeApplyRoutine());
    }


    private IEnumerator DeApplyRoutine()
    {
        yield return new WaitForSeconds(duration);
        DeApplyPowerDown();
        gameObject.SetActive(false);
        // Volver a activar render y cosas asi
        // No se setea True en caso de que justo haya alguien parado en el PowerDown sin renderizar y lo tome
        //canBeTaken = true; 
    }
}
