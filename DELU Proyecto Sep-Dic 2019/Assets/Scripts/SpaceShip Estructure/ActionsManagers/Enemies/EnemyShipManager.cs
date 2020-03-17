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
        if (!bIsActive) return;
        executeAction("Shoot");
        executeAction("Move");
        //Debug.Log("hola, estoy vacio por dentro :) (por ahora xd)");
    }

    /// <summary>
    /// Settea la AI como activa o inactiva
    /// </summary>
    /// <param name="status">Nuevo status de la AI</param>
    public void SetAIStatus(bool status)
    {
        bIsActive = status;
    }
}
