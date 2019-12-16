using UnityEngine;

/// <summary>
/// Abstract Class Action
/// 
/// Clase que define la estructura de una accion para
/// el patronde diseño command. 
/// </summary>
public abstract class Action: ScriptableObject
{
    /// <summary>
    /// Identificador de la accion
    /// </summary>
    public string tag = "default";

    /// <summary>
    /// doAction
    /// 
    /// Definicion del metodo que ejecuta la accion.
    /// </summary>
    public abstract void doAction(ActionManager manager);
}
