using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Bullet Movement ECS System
/// </summary>
public class BulletMovementSystem : JobComponentSystem {
    /// <summary>
    /// Query Group saved for speedup
    /// </summary>
    EntityQuery m_Group;

    /// <summary>
    /// Command Buffer to Destroy Bullets outside of game zone
    /// </summary>
    EntityCommandBufferSystem m_Barrier;

    /// <summary>
    /// What is considered the front of the bullet (in local coordinates)
    /// </summary>
    private float3 front = new float3(0.0f, 1.0f, 0.0f);

    protected override void OnCreate () {
        // Cached access to a set of ComponentData based on a specific query
        m_Group = GetEntityQuery (
            typeof (Translation),
            ComponentType.ReadOnly<Rotation> (),
            ComponentType.ReadOnly<BulletMovement> ()
        );

        // Barrier for destroy commands executions
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem> ();
    }

    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    /// <summary>
    /// Job To move all the bullets
    /// </summary>
    [BurstCompile]
    struct BulletMovementJob : IJobChunk {
        /// <summary>
        /// Delta Time for movement
        /// </summary>
        [ReadOnly]
        public float DeltaTime;

        /// <summary>
        /// World 2D Limits xy for lowes and zw for highest point in AABB
        /// </summary>
        [ReadOnly]
        public float4 WorldLimits;

        /// <summary>
        /// Entities associated with bullet movement
        /// </summary>
        [ReadOnly]
        public ArchetypeChunkEntityType EntityType;

        /// <summary>
        /// Array of Translation components of Bullets
        /// </summary>
        public ArchetypeChunkComponentType<Translation> TranslationType;

        /// <summary>
        /// Array of Rotation Components of Bullets
        /// </summary>
        [ReadOnly]
        public ArchetypeChunkComponentType<Rotation> RotationType;

        /// <summary>
        /// Array of BulletMovement Components of Bullets
        /// </summary>
        [ReadOnly]
        public ArchetypeChunkComponentType<BulletMovement> BulletMovementType;

        /// <summary>
        /// Command Buffer to destroy entities
        /// </summary>
        [WriteOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;

        /// <summary>
        /// What is considered the front of the bullet (in local coordinates)
        /// </summary>
        [ReadOnly]
        public float3 Front;

        public void Execute (ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
            var chuckEntities = chunk.GetNativeArray (EntityType);
            var chunkTranslation = chunk.GetNativeArray (TranslationType);
            var chunkRotation = chunk.GetNativeArray (RotationType);
            var chunkBulletMovement = chunk.GetNativeArray (BulletMovementType);
            for (var i = 0; i < chunk.Count; i++) {
                var translation = chunkTranslation[i];
                var rotation = chunkRotation[i];
                var bulletMovement = chunkBulletMovement[i];

                // Move bullet with its velocity
                chunkTranslation[i] = new Translation {
                    Value = translation.Value + 
                        math.mul(rotation.Value, Front) * bulletMovement.speed * DeltaTime
                };

                // If out of bounds delete
                if (chunkTranslation[i].Value.x < WorldLimits.x ||
                    chunkTranslation[i].Value.y < WorldLimits.y ||
                    chunkTranslation[i].Value.x > WorldLimits.w ||
                    chunkTranslation[i].Value.y > WorldLimits.z
                ) {
                    CommandBuffer.DestroyEntity (chunkIndex, chuckEntities[i]);
                }
            }
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate (JobHandle inputDependencies) {
        // Explicitly declare:
        // - Read-Write access to Rotation
        // - Read-Only access to RotationSpeed_IJobChunk
        var entityType = GetArchetypeChunkEntityType ();
        var translationType = GetArchetypeChunkComponentType<Translation> ();
        var rotationType = GetArchetypeChunkComponentType<Rotation> (true);
        var bulletMovementType = GetArchetypeChunkComponentType<BulletMovement> (true);

        // Create and Schedule
        var job = new BulletMovementJob () {
                EntityType = entityType,
                TranslationType = translationType,
                RotationType = rotationType,
                BulletMovementType = bulletMovementType,
                DeltaTime = Time.deltaTime,
                WorldLimits = new float4 (-10.0f, -10.0f, 10.0f, 10.0f),
                CommandBuffer = m_Barrier.CreateCommandBuffer ().ToConcurrent (),
                Front = front
        }.Schedule (m_Group, inputDependencies);

        // Execute Barrier after job
        m_Barrier.AddJobHandleForProducer (job);

        return job;
    }
}