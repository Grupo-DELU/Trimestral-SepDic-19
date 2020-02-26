using System.Collections;
using UnityEngine;

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
        if (debug && Input.GetKeyDown(KeyCode.PageDown))
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
    public IEnumerator SpawnEnemies(EnemyNode enemies)
    {
        Debug.Log("Spawneando enemigos!");
        AddNodeCount(enemies);
        for (int i = 0; i < enemies.quantity; i++)
        {
            yield return new WaitForSeconds(enemies.delay[i]);
            GameObject enemy = SpawningSystem.Manager.SpawnEnemyFromNode(enemies);
            //enemy.GetComponent<HealthManager>().onDepletedLife.AddListener((a, b) => ReduceEnemyCount());
        }
    }


    /// <summary>
    /// Reduce el contador de enemigos y chequea si no quedan mas
    /// </summary>
    void ReduceEnemyCount()
    {
        iEnemyCount = iEnemyCount - 1;
        if (iEnemyCount <= 0)
        {
            onNoMoreEnemies.Invoke();
        }
    }
}
