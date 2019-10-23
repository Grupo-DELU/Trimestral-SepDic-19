using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class EnemyNode : Node
{
    [Input] public Nothing input;
    public EnemyType enemyType;
    public int quantity;
    public List<float> delay;
    public SpawningPositions[] spawningPos; 
    public ScriptableObject bases;
}
