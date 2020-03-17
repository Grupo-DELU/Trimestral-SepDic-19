using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

/// <summary>
/// Bullet Movement ECS System
/// </summary>
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BulletMovementSystem : JobComponentSystem
{
    /// <summary>
    /// What is considered the front of the bullet (in local coordinates)
    /// </summary>
    private float3 front = new float3(0.0f, 1.0f, 0.0f);

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var localFront = front;

        var jobHandle = Entities.
        WithName("BulletMovementSystem").
        WithBurst(FloatMode.Default, FloatPrecision.Standard, true).
        ForEach(
            (ref PhysicsVelocity velocity, in Rotation rotation, in BulletMovement bulletMovement) =>
            {
                velocity.Linear = math.mul(rotation.Value, localFront) * bulletMovement.speed;
                velocity.Linear.z = 0;

                float3 angVel = velocity.Angular;
                velocity.Angular.x = 0;
                velocity.Angular.y = 0;
            }
        ).Schedule(inputDependencies);

        return jobHandle;
    }
}