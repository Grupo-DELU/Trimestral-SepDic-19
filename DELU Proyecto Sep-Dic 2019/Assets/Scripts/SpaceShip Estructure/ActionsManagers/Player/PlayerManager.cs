using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class PlayerManager
/// 
/// Clase que hereda la estructura de un manejador del
/// manejador de acciones del patron de diseño command.
/// Su objetivo es ejecutar las acciones dadas en el
/// inspector.
/// </summary>
/// 
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : ActionManager {

    /*
    Este script se basa en una plantilla (Action Manager).
    De esta manera, los objetos (player y enemigos) pueden
    insteractura con sus acciones de la misma manera.
    */
    public float fMaxSpeed = 10;
    [Range(-1f, 1f)]
    public float fSpeed = 1;

    public Vector2 velocity = Vector2.zero;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        /*
        Bajo las condiciones requeridas se ejecuta el metodo
        executeActions con el tag asociado a la accion que se
        quiere correr.

        Por ejemplo:
         */
        executeAction(ActionTags.PlayerMovement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            executeAction(ActionTags.PlayerAttack);
        }
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