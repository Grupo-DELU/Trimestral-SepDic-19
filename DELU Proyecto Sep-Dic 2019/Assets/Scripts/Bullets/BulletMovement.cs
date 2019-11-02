using Unity.Entities;
using Unity.Mathematics;
using System;

/// <summary>
/// Struct To Tag a Bullet for Movement
/// </summary>
[Serializable]
public struct BulletMovement  : IComponentData
{
    /// <summary>
    /// Speed at which the bullet is moving
    /// </summary>
   public float speed;
}
