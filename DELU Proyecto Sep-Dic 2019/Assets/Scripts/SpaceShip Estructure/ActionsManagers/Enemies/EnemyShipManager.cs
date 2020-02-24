using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipMovement), typeof(ShipShootingSystem))]
public class EnemyShipManager : ActionManager
{
    /// <summary>
    /// Sistema de movimiento de la nave
    /// </summary>
    [HideInInspector]
    public ShipMovement movementSyst = null;
    /// <summary>
    /// Sistema de disparo de la nave
    /// </summary>
    [HideInInspector]
    public ShipShootingSystem shootingSyst = null;

    /// <summary>
    /// Inicializacion de componentes
    /// </summary>
    /// <remarks>Siempre hacer override en los hijos y base.Awake()!</remarks>
    virtual protected void Awake()
    {
        movementSyst = GetComponent<ShipMovement>();
        shootingSyst = GetComponent<ShipShootingSystem>();
    }

    /// <summary>
    /// Reemplazame si soy un enemigo especial!
    /// </summary>
    virtual protected void Update()
    {
        executeAction("Shoot");
        executeAction("Move");
        //Debug.Log("hola, estoy vacio por dentro :) (por ahora xd)");
    }
}
