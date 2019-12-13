using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Bullet Movement ECS System
/// </summary>
[UpdateAfter (typeof (EndFramePhysicsSystem))]
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
    private float3 front = new float3 (0.0f, 1.0f, 0.0f);

    protected override void OnCreate () {
        m_Group = GetEntityQuery (
            typeof (PhysicsVelocity),
            ComponentType.ReadOnly<Translation> (),
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
        [ReadOnly]
        public ArchetypeChunkComponentType<Translation> TranslationType;

        /// <summary>
        /// Array of Physics Velocities
        /// </summary>
        public ArchetypeChunkComponentType<PhysicsVelocity> VelocityType;

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
            var chunkVelocity = chunk.GetNativeArray (VelocityType);
            var chunkRotation = chunk.GetNativeArray (RotationType);
            var chunkBulletMovement = chunk.GetNativeArray (BulletMovementType);
            for (var i = 0; i < chunk.Count; i++) {
                var translation = chunkTranslation[i];
                var velocity = chunkVelocity[i];
                var rotation = chunkRotation[i];
                var bulletMovement = chunkBulletMovement[i];

                float3 vel = math.mul (rotation.Value, Front) * bulletMovement.speed;
                vel.z = 0;

                float3 angVel = velocity.Angular;
                angVel.x = 0;
                angVel.y = 0;

                chunkVelocity[i] = new PhysicsVelocity {
                    Linear = vel,
                    Angular = angVel
                };

                // If out of bounds delete
                if (translation.Value.x < WorldLimits.x ||
                    translation.Value.y < WorldLimits.y ||
                    translation.Value.x > WorldLimits.w ||
                    translation.Value.y > WorldLimits.z
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
        var translationType = GetArchetypeChunkComponentType<Translation> (true);
        var velocityType = GetArchetypeChunkComponentType<PhysicsVelocity> ();
        var rotationType = GetArchetypeChunkComponentType<Rotation> (true);
        var bulletMovementType = GetArchetypeChunkComponentType<BulletMovement> (true);

        // Create and Schedule
        var job = new BulletMovementJob () {
            EntityType = entityType,
                TranslationType = translationType,
                VelocityType = velocityType,
                RotationType = rotationType,
                BulletMovementType = bulletMovementType,
                WorldLimits = new float4 (-10.0f, -10.0f, 10.0f, 10.0f),
                CommandBuffer = m_Barrier.CreateCommandBuffer ().ToConcurrent (),
                Front = front
        }.Schedule (m_Group, inputDependencies);

        // Execute Barrier after job
        m_Barrier.AddJobHandleForProducer (job);

        return job;
    }
}