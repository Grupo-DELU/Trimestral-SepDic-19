using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Cuerpo rigido del jugador
    /// </summary>
    private Rigidbody2D rb2d;

    /// <summary>
    /// Velocidad de movimiento
    /// </summary>
    [SerializeField]
    private float fSpeed = 10;

    [SerializeField]
    private bool bIsMoving = true;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Calcula la direccion del movimiento 
    /// </summary>
    /// <returns>Direccion de movimiento del jugador</returns>
    private Vector2 GetDirection()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    /// <summary>
    /// Mueve al jugador en la direccion deseada
    /// </summary>
    public void Move()
    {
        if (!bIsMoving) return;
        rb2d.MovePosition((Vector2)transform.position + (GetDirection() * fSpeed * Time.fixedDeltaTime));
    }
}
