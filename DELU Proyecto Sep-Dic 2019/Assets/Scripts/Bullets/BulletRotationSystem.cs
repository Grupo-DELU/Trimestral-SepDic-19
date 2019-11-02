using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Bullet Rotation ECS System
/// </summary>
[UpdateBefore(typeof(BulletMovementSystem))]
public class BulletRotationSystem : JobComponentSystem {
    /// <summary>
    /// Query Group saved for speedup
    /// </summary>
    EntityQuery m_Group;

    /// <summary>
    /// Axis of rotation, for sprite bullets it should be Z
    /// </summary>
    private readonly float3 axisOfRotation = new float3(0.0f, 0.0f, 1.0f);

    protected override void OnCreate () {
        // Cached access to a set of ComponentData based on a specific query
        m_Group = GetEntityQuery (
            typeof (Rotation),
            ComponentType.ReadOnly<BulletRotation> ()
        );
    }

    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    /// <summary>
    /// Job To rotate all the bullets
    /// </summary>
    [BurstCompile]
    struct BulletRotationJob : IJobChunk {
        /// <summary>
        /// Delta Time for movement
        /// </summary>
        public float DeltaTime;

        /// <summary>
        /// Array of Rotation components of Bullets
        /// </summary>
        public ArchetypeChunkComponentType<Rotation> RotationType;

        /// <summary>
        /// Array of BulletRotation Components of Bullets
        /// </summary>
        [ReadOnly]
        public ArchetypeChunkComponentType<BulletRotation> BulletRotationType;

        /// <summary>
        /// Axis of rotation, for sprite bullets it should be Z
        /// </summary>
        [ReadOnly]
        public float3 AxisOfRotation;

        public void Execute (ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
            var chunkRotation = chunk.GetNativeArray (RotationType);
            var chunkBulletRotation = chunk.GetNativeArray (BulletRotationType);
            for (var i = 0; i < chunk.Count; i++) {
                var rotation = chunkRotation[i];
                var bulletRotation = chunkBulletRotation[i];

                // Rotate bullet with its angular speed
                chunkRotation[i] = new Rotation {
                    Value = math.mul(math.normalize(rotation.Value),
                        quaternion.AxisAngle(AxisOfRotation, bulletRotation.radiansPerSecond * DeltaTime))
                };
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate (JobHandle inputDependencies) {
        // Explicitly declare:
        // - Read-Write access to Rotation
        // - Read-Only access to RotationSpeed_IJobChunk
        var rotationType = GetArchetypeChunkComponentType<Rotation> ();
        var bulletRotationType = GetArchetypeChunkComponentType<BulletRotation> (true);

        // Create and Schedule
        var job = new BulletRotationJob () {
                RotationType = rotationType,
                BulletRotationType = bulletRotationType,
                DeltaTime = Time.deltaTime,
                AxisOfRotation = axisOfRotation
        }.Schedule (m_Group, inputDependencies);

        return job;
    }
}