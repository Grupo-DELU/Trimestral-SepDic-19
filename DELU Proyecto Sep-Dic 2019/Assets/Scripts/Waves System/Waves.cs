using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Normal, Shotgun, Laser, Kamikaze};

/// <summary>
/// NADA DE ESTO ES ALGO
/// </summary>
[System.Serializable]
public struct WaveEnemy
{
    public EnemyType eEnemyType;
    public float fSpawnDelay;
}

[System.Serializable]
public class Wave
{
    public List<WaveEnemy> lEnemies;
}
