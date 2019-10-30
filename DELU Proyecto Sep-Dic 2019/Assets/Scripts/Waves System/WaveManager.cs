using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWaveEvents : UnityEvent { }
public class WaveManager : MonoBehaviour
{
    /// <summary>
    /// Contador de enemigos
    /// </summary>
    private int iEnemyCount;
    public EnemyWaveEvents onNoMoreEnemies = new EnemyWaveEvents();

    /// <summary>
    /// Le agrega al contador la cantidad de enemigos del nodo
    /// </summary>
    /// <param name="enemies">Nodo de enemigos</param>
    public void AddNodeCount(EnemyNode enemies)
    {
        iEnemyCount = iEnemyCount + enemies.quantity;
    }

    /// <summary>
    /// Se encarga de spawnear los enemigos de un nodo y suscribir el reductor
    /// del contador al evento de muerte
    /// </summary>
    /// <param name="enemies">Nodo de enemigos a spawnear</param>
    /// <returns>Corutina</returns>
    private IEnumerator SpawnEnemies(EnemyNode enemies)
    {
        AddNodeCount(enemies);
        for(int i = 0; i < enemies.quantity; i++)
        {
            yield return new WaitForSeconds(enemies.delay[i]);
            GameObject enemy = SpawningSystem.Manager.SpawnEnemy();
            enemy.GetComponent<HealthManager>().onDepletedLife.AddListener(ReduceEnemyCount);
        }
    }

    /// <summary>
    /// Reduce el contador de enemigos y chequea si no hay mas
    /// </summary>
    /// <param name="oldHp">Useless</param>
    /// <param name="newHp">Vida del enemigo muerto</param>
    void ReduceEnemyCount(int oldHp, int newHp)
    {
        if (newHp > 0) return;
        iEnemyCount = iEnemyCount - 1;
        if (iEnemyCount <= 0)
        {
            onNoMoreEnemies.Invoke();
        }
    }
}
