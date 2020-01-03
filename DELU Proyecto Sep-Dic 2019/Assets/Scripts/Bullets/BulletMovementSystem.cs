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

        var localFront = front;
        var localWorldLimits = worldLimits;

        var jobHandle = Entities
            .WithName ("BulletMovementSystem")
            .WithBurst (FloatMode.Default, FloatPrecision.Standard, true)
            .WithReadOnly (localFront)
            .WithReadOnly (localWorldLimits)
            .ForEach (
                (
                    Entity enitity, int entityInQueryIndex, ref PhysicsVelocity velocity, in Translation translation, in Rotation rotation, in BulletMovement bulletMovement
                ) => {
                    float3 vel = math.mul (rotation.Value, localFront) * bulletMovement.speed;
                    vel.z = 0;

                    float3 angVel = velocity.Angular;
                    angVel.x = 0;
                    angVel.y = 0;

                    velocity = new PhysicsVelocity {
                        Linear = vel,
                        Angular = angVel
                    };

                    // If out of bounds delete
                    if (translation.Value.x < localWorldLimits.x ||
                        translation.Value.y < localWorldLimits.y ||
                        translation.Value.x > localWorldLimits.w ||
                        translation.Value.y > localWorldLimits.z
                    ) {
                        commandBuffer.DestroyEntity (entityInQueryIndex, enitity);
                    }
                }
            ).Schedule (inputDependencies);

        // Execute Barrier after job
        m_Barrier.AddJobHandleForProducer (jobHandle);

        return jobHandle;
    }
}