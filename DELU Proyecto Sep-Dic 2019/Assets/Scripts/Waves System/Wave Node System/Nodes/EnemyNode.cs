using System.Collections.Generic;
using UnityEngine;
using XNode;

public class EnemyNode : Node
{
    [Input] public Nothing input;
    public int quantity;
    public List<float> delay;
    public SpawningPositions[] spawningPos;
    public EnemyTypes enemyType;
    /// <summary>
    /// Enemy Ship base stats
    /// </summary>
    /// <remarks>
    /// If the enemy is a Kamikaze, it must be a KamikazeShipBase. Same must occur
    /// if the enemy is a CurveEnemy, it must be a CurveShipBase with a curve.
    /// </remarks>
    public ScriptableObject bases;
}
