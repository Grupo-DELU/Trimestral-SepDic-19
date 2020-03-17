using UnityEngine;

/// <summary>
/// Class PlayerAttackAction
/// 
/// Clase que hereda la estructura de una accion para
/// el patronde diseño command. Se encarga de ejecutar
/// un tipo de ataque del jugador.
/// </summary>
[CreateAssetMenu(fileName = "A_Attack", menuName = "Actions/Player/PlayerAttack", order = 1)]
public class PlayerAttackAction : Action
{

    /*
    Aca se pueden implemetar los metodos necesarios para el
    funcionamiento de esta accion. Al final, se debera colocar
    en doActions la logica que sera ejecutada de la accion.
     */
    override
    public void doAction(ActionManager manager)
    {
        PlayerManager pm = manager as PlayerManager;
        //BulletSpawner.Manager.ShootBullet(pm.transform.position, Vector2.up, 0, 15, 15);
        Debug.Log("The player is in attack Action");
    }
}
