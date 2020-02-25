using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// WaveManager se encarga del spawneo de las naves de una wave
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Manager { get; private set; }
    /// <summary>
    /// Contador de enemigos
    /// </summary>
    private int iEnemyCount;
    /// <summary>
    /// Se llama cuando no quedan mas enemigos en la wave
    /// </summary>
    public WavesEvents onNoMoreEnemies = new WavesEvents();

#if UNITY_EDITOR
    public bool debug = true;
#endif

    private void Awake()
    {
        #region Singleton
        if (Manager != null && Manager != this)
        {
            Debug.LogError("Hay dos sistemas de que manejan el desarollo de la wave!");
        }
        Manager = this;
        #endregion
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (debug && Input.GetKeyDown(KeyCode.D))
        {
            ReduceEnemyCount();
            Debug.Log("Ahora hay: " + iEnemyCount + " enemigos.");
        }
    }
#endif

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
    public IEnumerator SpawnEnemies(EnemyNode enemies)
    {
        Debug.Log("Spawneando enemigos!");
        AddNodeCount(enemies);
        for(int i = 0; i < enemies.quantity; i++)
        {
            yield return new WaitForSeconds(enemies.delay[i]);
            GameObject enemy = SpawningSystem.Manager.SpawnEnemyFromNode(enemies);
            //enemy.GetComponent<HealthManager>().onDepletedLife.AddListener(ReduceEnemyCount);
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

    void ReduceEnemyCount()
    {
        iEnemyCount = iEnemyCount - 1;
        if (iEnemyCount <= 0)
        {
            onNoMoreEnemies.Invoke();
        }
    }
}
