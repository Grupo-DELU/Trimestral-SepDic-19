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
    /// Command Buffer to Destroy Bullets outside of game zone
    /// </summary>
    EndSimulationEntityCommandBufferSystem m_Barrier;

    /// <summary>
    /// What is considered the front of the bullet (in local coordinates)
    /// </summary>
    private float3 front = new float3 (0.0f, 1.0f, 0.0f);

    /// <summary>
    /// World Limits
    /// </summary>
    private float4 worldLimits = new float4 (-10.0f, -10.0f, 10.0f, 10.0f);

    protected override void OnCreate () {
        // Barrier for destroy commands executions
        m_Barrier = World
            .DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem> ();
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate (JobHandle inputDependencies) {

        var commandBuffer = m_Barrier.CreateCommandBuffer ().ToConcurrent ();

        var localWorldLimits = worldLimits;

        /*

        var destroyJobHandle = Entities.
        WithName ("BulletDestroySystem").
        WithBurst (FloatMode.Default, FloatPrecision.Standard, true).
        WithReadOnly (localWorldLimits).
        ForEach (
            (Entity entity, int entityInQueryIndex, in Translation translation) => {
                // If out of bounds delete
                if (translation.Value.x < localWorldLimits.x ||
                    translation.Value.y < localWorldLimits.y ||
                    translation.Value.x > localWorldLimits.w ||
                    translation.Value.y > localWorldLimits.z
                ) {
                    commandBuffer.DestroyEntity (entityInQueryIndex, entity);
                }
            }
        ).Schedule (inputDependencies);

        // Execute Barrier after job
        m_Barrier.AddJobHandleForProducer (destroyJobHandle);

        */

        var localFront = front;

        var jobHandle = Entities.
        WithName ("BulletMovementSystem").
        WithBurst (FloatMode.Default, FloatPrecision.Standard, true).
        WithReadOnly (localFront).
        ForEach (
            (ref PhysicsVelocity velocity, in Rotation rotation, in BulletMovement bulletMovement) => {
                velocity.Linear = math.mul (rotation.Value, localFront) * bulletMovement.speed;
                velocity.Linear.z = 0;

                float3 angVel = velocity.Angular;
                velocity.Angular.x = 0;
                velocity.Angular.y = 0;
            }
        ).Schedule (inputDependencies);

        return jobHandle;
    }
}