using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowCurveAction : Action
{
    public override void doAction(ActionManager manager)
    {
        manager = manager as CurveEnemyManager;
        //Deberiamos hacer un movement manager aparte? Que tenga datos como speed movement y eso
        //Me parece que todo en una accion es muy aparatoso
        //Igual que un shoot manager que contenga datos como firerate, tipo de proyectil y eso.
        //Quizas el manager podria tener simplemente una referencia a estos managers?
        Debug.Log("Enemigo moviendose");
    }
}
