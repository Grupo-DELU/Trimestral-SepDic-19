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
    // Diccionario de aciones del objeto que usa como llaves su tag de accion.
    protected Dictionary<ActionTags, Action> actions = new Dictionary<ActionTags, Action>();

    //En clase base para que todos los hijos lo ejecuten
    protected virtual void Start()
    {
        loadActions(this.GetComponents<Action>());
    }

    /// <summary>
    /// executeAction
    /// 
    /// Metodo encargado de ejecutar las acciones solicitadas
    /// por el cliente. 
    /// </summary>
    /// <param name="tag">Identificador de la accion</param>
    public void executeAction(ActionTags tag)
    {
        actions[tag].doAction(this);
    }

    /// <summary>
    /// loadActions
    /// 
    /// Metodo encargado de cargar todas las acciones a usar 
    /// por el cliente.
    /// </summary>
    public void loadActions(Action[] actions_to_load)
    {
        foreach (Action act in actions_to_load)
        {
            Debug.Log(act.tag);
            actions[act.tag] = act;
        }
    }

}
    
