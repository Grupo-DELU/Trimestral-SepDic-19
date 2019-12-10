using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class PlayerMovementAction
/// 
/// Clase que hereda la estructura de una accion para
/// el patronde diseño command. Se encarga de ejecutar
/// el movimiento del jugador.
/// </summary>
public class PlayerMovementAction : Action
{
    /*
    Aca se pueden implemetar los metodos necesarios para el
    funcionamiento de esta accion. Al final, se debera colocar
    en doActions la logica que sera ejecutada de la accion.
     */
    override
    public void doAction(ActionManager manager){
        PlayerManager pm = manager as PlayerManager;
        pm.Movement.Move();
        //Debug.Log("The player is in movevement Action");
    }
}
