﻿using System.Collections;
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
    protected Dictionary<string, Action> actions = new Dictionary<string, Action>();

    // Dado a que Unity no puede serializar diccionarios (aunque podemos hacer una hash table sencilla)
    // nos toca usar un array para meter las acciones por inspector y luego al diccionario
    [SerializeField]
    private Action[] soActions;

    // En clase base para que todos los hijos lo ejecuten
    protected virtual void Start()
    {
        loadActions(soActions);
    }

    /// <summary>
    /// executeAction
    /// 
    /// Metodo encargado de ejecutar las acciones solicitadas
    /// por el cliente. 
    /// </summary>
    /// <param name="tag">Identificador de la accion</param>
    public void executeAction(string tag)
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
            #if UNITY_EDITOR
            Debug.Log(act.tag);
            if (act.tag == "default") Debug.LogWarning("There's an action without tag");
            #endif
            actions[act.tag] = act;
        }
    }

}
    
