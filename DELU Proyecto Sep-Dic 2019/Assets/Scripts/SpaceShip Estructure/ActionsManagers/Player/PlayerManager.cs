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
[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : ActionManager {

    /*
    Este script se basa en una plantilla (Action Manager).
    De esta manera, los objetos (player y enemigos) pueden
    insteractura con sus acciones de la misma manera.
    */

    /// <summary>
    /// Manejador de movimiento del jugador
    /// </summary>
    public PlayerMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
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

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            executeAction(ActionTags.PlayerAttack);
        }
    }

}