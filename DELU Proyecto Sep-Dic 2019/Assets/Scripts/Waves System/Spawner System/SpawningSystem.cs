using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawningPositions { left, upper_left, up, upper_right, right}
public class SpawningSystem : MonoBehaviour
{
    public Transform[] whereToSpawn;

    void SpawnEnemyNode(EnemyNode node)
    {
        SpawningPositions[] spawners = node.spawningPos;
        int quantity = node.quantity;
        for (int i = 0; i < quantity; i++)
        {
            int pos = Random.Range(0, 100)%spawners.Length;
            SpawnHere(whereToSpawn[(int)spawners[pos]]);
        }
    }

    void SpawnHere(Transform pos) { }
}
