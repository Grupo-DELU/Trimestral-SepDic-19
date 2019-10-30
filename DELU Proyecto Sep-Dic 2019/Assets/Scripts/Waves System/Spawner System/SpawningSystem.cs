using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawningPositions { left, upper_left, up, upper_right, right}
public class SpawningSystem : MonoBehaviour
{
    public static SpawningSystem Manager { get; private set; }
    public Transform[] whereToSpawn;

    private void Awake()
    {
        #region Singleton
        if (Manager != null && Manager != this)
        {
            Debug.LogError("Hay dos sistemas de spawning!");
        }
        Manager = this;
        #endregion
    }
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

    public GameObject SpawnEnemy() { return null; }

    void SpawnHere(Transform pos) { }
}
