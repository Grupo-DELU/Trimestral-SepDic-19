using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    /// <summary>
    /// Indica si el dano que realiza la nave le hace instakill
    /// </summary>
    [SerializeField]
    private bool bInstakill = true;

    /// <summary>
    /// Indica el dano que realiza la nave por colision
    /// </summary>
    /// <remarks>
    /// Solo es tomado en cuenta si la nave no hace instakill
    /// </remarks>
    [SerializeField]
    private int iCrashDamage = 1; 


    // LLAMADO CUANDO EL QUE TENGA ESTE SCRIPT ENTRA A UN TRIGGERR
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO
        // [] Evitar que las naves enemigas choquen entre si!

        if (collision.CompareTag("Ship") || collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out HealthManager hm))
            {
                if (bInstakill) hm.RemoveLife(hm.MaxHP);
                else hm.RemoveLife(iCrashDamage);
            }
        }
    }
}
