using UnityEngine;

/// <summary>
/// Class PlayerMovementAction
/// 
/// Clase que hereda la estructura de una accion para
/// el patronde diseño command. Se encarga de ejecutar
/// el movimiento del jugador.
/// </summary>
[CreateAssetMenu(fileName = "A_PlayerMovement", menuName = "Actions/Player/PlayerMovement", order = 0)]
public class PlayerMovementAction : Action
{
    /*
    Aca se pueden implemetar los metodos necesarios para el
    funcionamiento de esta accion. Al final, se debera colocar
    en doActions la logica que sera ejecutada de la accion.
     */
    override public void doAction(ActionManager manager)
    {
        PlayerManager pm = manager as PlayerManager;
        pm.MoveWithVel(GetVelocity(pm));
        if (GetVelocity(pm) != Vector2.zero) Debug.Log("Velocidad: " + GetVelocity(manager));
    }

    private Vector2 GetDirection()
    {
        Debug.Log(Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical"));
        return Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical");
    }

    private Vector2 GetVelocity(ActionManager manager)
    {
        PlayerManager pm = manager as PlayerManager;
        return GetDirection() * pm.fSpeed * pm.fMaxSpeed * Time.fixedDeltaTime;
    }
}
