using Unity.Entities;
using Unity.Mathematics;
using System;

/// <summary>
/// Struct To Tag a Bullet for Rotation
/// </summary>
[Serializable]
public struct BulletRotation  : IComponentData
{
    /// <summary>
    /// Angular Speed of Bullet Rotation
    /// </summary>
   public float radiansPerSecond;
}
