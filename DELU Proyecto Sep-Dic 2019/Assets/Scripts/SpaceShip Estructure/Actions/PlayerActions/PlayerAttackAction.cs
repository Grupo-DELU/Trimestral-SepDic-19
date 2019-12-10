using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class PlayerAttackAction
/// 
/// Clase que hereda la estructura de una accion para
/// el patronde diseño command. Se encarga de ejecutar
/// un tipo de ataque del jugador.
/// </summary>
public class PlayerAttackAction : Action {

    /*
    Aca se pueden implemetar los metodos necesarios para el
    funcionamiento de esta accion. Al final, se debera colocar
    en doActions la logica que sera ejecutada de la accion.
     */
    override
    public void doAction(ActionManager manager){
        PlayerManager pm = manager as PlayerManager;
        Debug.Log("The player is in attack Action");
    }
}
