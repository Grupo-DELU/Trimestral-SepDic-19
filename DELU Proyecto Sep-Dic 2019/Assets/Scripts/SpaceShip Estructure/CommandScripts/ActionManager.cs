using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class ActionManager
/// 
/// Clase que define la estructura de un manejador de
/// acciones para el patronde diseño command. Su tarea
/// es obtener el total de acciones de un objeto y 
/// realizar la ejecucion segun el identificador dado
/// </summary>
public class ActionManager : MonoBehaviour
{
    // Arreglo de aciones del objeto.
    protected Action[] actions;

    /// <summary>
    /// executeAction
    /// 
    /// Metodo encargado de ejecutar las acciones solicitadas
    /// por el cliente. 
    /// </summary>
    /// <param name="tag">Identificador de la accion</param>
    public void executeAction(ActionTags tag){
        foreach(Action action in actions){
            if (action.tag.Equals(tag)){
                action.doAction();
            }
        }
    }

}
    
