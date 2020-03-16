using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumerador que indica las posibles posiciones de spawn de los enemigos 
/// </summary>
public enum SpawningPositions { left, upper_left, up, upper_right, right}
public enum EnemyTypes { curve, kamikaze}

public class SpawningSystem : MonoBehaviour
{
    public static SpawningSystem Manager { get; private set; }

    public ObjectPooler shipPool = null;

    /// <summary>
    /// Puntos de spawn
    /// </summary>
    /// <remarks> Van en el orden del enumerador, 0 es left, 1 es upper_left...</remarks>
    [SerializeField]
    private Transform[] tSpawnPoints = null;
    /// <summary>
    /// Radio de spawneo en el punto de spawn
    /// </summary>
    [SerializeField]
    private float fSpawnRadius = 1;


    private void Awake()
    {
        #region Singleton
        if (Manager != null && Manager != this)
        {
            Debug.LogError("Hay dos sistemas de spawning! Intentando eliminar/indicar el duplicado...", Manager.gameObject);
            Destroy(Manager.gameObject);
        }
        Manager = this;
        #endregion

        if (shipPool == null) Debug.LogError("No hay pool de naves, esto va a explotar!", gameObject);
    }


    /// <summary>
    /// Spawnea un enemigo de un nodo de enmigos
    /// </summary>
    /// <param name="node">Node de enemigos</param>
    /// <returns>Enemigo spawneado</returns>
    public GameObject SpawnEnemyFromNode(EnemyNode node)
    {
        SpawningPositions[] spawners = node.spawningPos;
        int pos = Random.Range(1, 100) % spawners.Length;
        Vector2 spawnPos = CalculateSpawnPoint(spawners[pos]);

        GameObject spawnedEnemy = SpawnEnemyOfType(node.enemyType, node.bases, spawnPos);

        return spawnedEnemy;
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


    /// <summary>
    /// Spawnea/Setea stats de un enemigo de un tipo
    /// </summary>
    /// <param name="type">Tipo del enemigo</param>
    /// <param name="shipStats">Stats del enemigo</param>
    /// <returns>Nave enemiga spawneada</returns>
    /// <remarks>
    /// El ScriptableObject debe de ser del tipo adecuado a la nave!
    /// Chequea los tipos en: <see cref="NaveBaseSO"/>
    /// </remarks>
    public GameObject SpawnEnemyOfType(EnemyTypes type, ScriptableObject shipStats, Vector2 position)
    {
        GameObject spawned = null;
        switch (type)
        {
            case EnemyTypes.curve:
                spawned = InitializeCurveEnemy(shipStats as CurveShipBaseSO, position);
                break;
            case EnemyTypes.kamikaze:
                spawned = InitializeKamikaze(shipStats as KamikazeBaseSO, position);
                break;
        }
        return spawned;
    }


    /// <summary>
    /// Inicializa los stats generales/comunes de una nave
    /// </summary>
    /// <param name="ship">Nave a inicializar</param>
    /// <param name="baseStats">Stats de la nave a inicializar</param>
    public void InitializeEnemyGeneralStats(GameObject ship, NaveBaseSO baseStats, Vector2 position)
    {
        ship.transform.position = position;
        ShipMovement sm = ship.GetComponent<ShipMovement>();
        sm.SetSpeed(baseStats.movementSpeed);

        ShipShootingSystem ss = ship.GetComponent<ShipShootingSystem>();
        if (baseStats.bulletNum <= 0) ss.SetSystemOnOff(false);
        else
        {
            ss.SetSystemOnOff(true);
            ss.SetShotNumber(baseStats.bulletNum);
            ss.SetBulletSpeed(baseStats.bulletSpeed);
            ss.SetFireRate(baseStats.reloadime);
        }
    }


    /// <summary>
    /// Inicializa un enemigo de curva
    /// </summary>
    /// <param name="baseStats">Stats base del enemigo curva</param>
    /// <returns>Enemigo curva inicializado</returns>
    public GameObject InitializeCurveEnemy(CurveShipBaseSO baseStats, Vector2 position)
    {
        GameObject curveEnemy = shipPool.GetFromPool("Curve");
        curveEnemy.transform.position = position;
        curveEnemy.GetComponent<CurveEnemyMovement>().SetCurve(baseStats.curve);
        InitializeEnemyGeneralStats(curveEnemy, baseStats, position);

        return curveEnemy;
    }


    /// <summary>
    /// Inicializa un enemigo kamikaze
    /// </summary>
    /// <param name="baseStats">Stats base del enemigo kamikaze</param>
    /// <returns>Enemigo kamikaze inicializado</returns>
    public GameObject InitializeKamikaze(KamikazeBaseSO baseStats, Vector2 position)
    {
        GameObject kamikaze = shipPool.GetFromPool("Kamikaze");
        kamikaze.GetComponent<KamikazeEnemyManager>().ResetKamikaze();

        InitializeEnemyGeneralStats(kamikaze, baseStats, position);

        return kamikaze;
    }


    /// <summary>
    /// Agrega el enemigo al sistema de score para que de puntos cuando muera
    /// </summary>
    /// <param name="enemy">Enemigo a agregar</param>
    /// <param name="stats">Stats del enemigo</param>
    public void ProcessEnemyToScore(GameObject enemy, NaveBaseSO stats)
    {
        // No hay score (puede ser partida sin score)
        if (ScoreSystem.Manager == null) return;
        enemy.GetComponent<HealthManager>().onDepletedLife.AddListener((a,b) => ScoreSystem.Manager.AddScore(stats.points));
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (tSpawnPoints.Length <= 0) return;
        Gizmos.color = Color.red;
        foreach (Transform point in tSpawnPoints)
        {
            Gizmos.DrawWireSphere(point.position, fSpawnRadius);
        }
    }
#endif
}
