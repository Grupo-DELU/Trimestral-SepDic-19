using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawningPositions { left, upper_left, up, upper_right, right}
public class SpawningSystem : MonoBehaviour
{
    public static SpawningSystem Manager { get; private set; }
    public GameObject debugEnemy;
    /// <summary>
    /// Puntos de spawn
    /// </summary>
    /// <remarks>
    /// Van en el orden del enumerador, 0 es left, 1 es upper_left...
    /// </remarks>
    [SerializeField] private Transform[] tSpawnPoints;
    [SerializeField] private float fSpawnRadius;
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

    /// <summary>
    /// Spawnea un enemigo de un nodo de enmigos
    /// </summary>
    /// <param name="node">Node de enemigos</param>
    /// <returns>Enemigo spawneadoo</returns>
    public GameObject SpawnEnemyFromNode(EnemyNode node)
    {
        SpawningPositions[] spawners = node.spawningPos;
        int pos = Random.Range(1, 100) % spawners.Length;
        Vector2 spawnPos = CalculateSpawnPoint(spawners[pos]);
        GameObject debug = Instantiate(debugEnemy);
        debug.transform.position = spawnPos;
        return debug;
    }

    /// <summary>
    /// Calcula el punto de spawn de un enemigo
    /// </summary>
    /// <param name="pos">SpawnPoint a spawnear</param>
    /// <returns></returns>
    public Vector2 CalculateSpawnPoint(SpawningPositions pos)
    {
        return (Vector2)tSpawnPoints[(int)pos].position 
               + Random.insideUnitCircle * fSpawnRadius;
    }

    private void OnDrawGizmos()
    {
        if (tSpawnPoints.Length <= 0) return;
        Gizmos.color = Color.red;
        foreach (Transform point in tSpawnPoints)
        {
            Gizmos.DrawWireSphere(point.position, fSpawnRadius);
        }
    }
}
