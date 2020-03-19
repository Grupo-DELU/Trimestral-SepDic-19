using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]//, typeof(Rigidbody2D))]
public abstract class PowerUp : MonoBehaviour
{
    // NOTA: El RigidBody2D es solo para moverlo hacia abajo con fisicas y no a punta de translate,
    // dado a que si se mueve con translate peude afectar el perfomance fisico!
    private Collider2D coll2d = null;
    private SpriteRenderer rndr = null;

    /// <summary>
    /// Duracion del PowerUp
    /// </summary>
    [SerializeField]
    protected float duration = 3;

    /// <summary>
    /// Velocidad con la que se mueve el PowerUp
    /// </summary>
    [SerializeField]
    private float speed = 10;

    /// <summary>
    /// Indica si se puede tomar el PowerUp
    /// </summary>
    [SerializeField]
    private bool canBeTaken = true;

    /// <summary>
    /// Quien tomo el powerUp
    /// </summary>
    private GameObject takenBy = null;


    private void Awake()
    {
        coll2d = GetComponent<Collider2D>();
        coll2d.isTrigger = true;

        rndr = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        // Puede volver a ser tomado cuando se desactiva y vuelve a activar
        canBeTaken = true;
        // Volvemos a activar el sprite
        rndr.enabled = true;
    }

    private void Update()
    {
        if (canBeTaken) transform.Translate(Vector2.down * speed * Time.deltaTime);
    }


    /// <summary>
    /// Se llama cuando el PowerUp es tomado por algo y aplica sus efectos
    /// a este algo.
    /// </summary>
    /// <param name="toApply"></param>
    public abstract void ApplyPowerUp(GameObject toApply);
    /// <summary>
    /// Desaplica un PowerUp a quien sea que lo haya tomado.
    /// </summary>
    /// <remarks>Cuando llamarse es manejado por el mismo PowerUp</remarks>
    /// <param name="toDeApply"></param>
    public abstract void DeApplyPowerUp(GameObject toDeApply);


    public void BeTaken()
    {
        canBeTaken = false;
        rndr.enabled = false;
        StartCoroutine(DeApplyRoutine());
    }

   
    private IEnumerator DeApplyRoutine()
    {
        yield return new WaitForSeconds(duration);
        DeApplyPowerUp(takenBy);
        gameObject.SetActive(false);
        // Volver a activar render y cosas asi
        // No se setea True en caso de que justo haya alguien parado en el PowerUp sin renderizar y lo tome
        //canBeTaken = true; 
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeTaken) return;
        // En teoria puede ser para todas las naves! Seria interesante explorar PowerUps que haya que destruir
        // para evitar que el enemigo los tome
        if (collision.CompareTag("Player"))
        {
            takenBy = collision.gameObject;
            ApplyPowerUp(collision.gameObject);
            BeTaken();
        }
    }
}
