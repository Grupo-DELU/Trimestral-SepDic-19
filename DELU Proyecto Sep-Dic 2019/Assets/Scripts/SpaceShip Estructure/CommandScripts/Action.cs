using UnityEngine;

/// <summary>
/// Abstract Class Action
/// 
/// Clase que define la estructura de una accion para
/// el patronde diseño command. 
/// </summary>
public abstract class Action : MonoBehaviour
{
    //Indetificador de la accion
    public new ActionTags tag;

    /// <summary>
    /// doAction
    /// 
    /// Definicion del metodo que ejecuta la accion.
    /// </summary>
    public abstract void doAction();
}
