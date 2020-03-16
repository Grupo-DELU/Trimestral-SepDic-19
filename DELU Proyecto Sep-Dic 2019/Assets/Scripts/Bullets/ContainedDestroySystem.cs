using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

/// <summary>
/// Struct To Tag an Entity to be destroyed if outside game zone
/// </summary>
[System.Serializable]
public struct ContaidedDestroyable : IComponentData
{

}

/// <summary>
/// System to destroy entities outside game zone
/// </summary>
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ContainedDestroySystem : JobComponentSystem
{
    /// <summary>
    /// Command Buffer to Destroy Entities outside of game zone
    /// </summary>
    EndSimulationEntityCommandBufferSystem m_Barrier;

    /// <summary>
    /// World Limits
    /// </summary>
    private float4 worldLimits = new float4(-10.0f, -10.0f, 10.0f, 10.0f);

    /// <summary>
    /// World Limits
    /// </summary>
    public float4 WorldLimits { get { return worldLimits; } set { worldLimits = value; } }

    protected override void OnCreate()
    {
        // Barrier for destroy commands executions
        m_Barrier = World
            .DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent();

        var localWorldLimits = worldLimits;

        var destroyJobHandle = Entities.
        WithName("ContainedDestroySystem").
        WithAll<ContaidedDestroyable>().
        WithBurst(FloatMode.Default, FloatPrecision.Standard, true).
        ForEach(
            (Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                // If out of bounds delete
                if (translation.Value.x < localWorldLimits.x ||
                    translation.Value.y < localWorldLimits.y ||
                    translation.Value.x > localWorldLimits.w ||
                    translation.Value.y > localWorldLimits.z
                )
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }
            }
        ).Schedule(inputDependencies);

        // Execute Barrier after job
        m_Barrier.AddJobHandleForProducer(destroyJobHandle);

        return destroyJobHandle;
    }
}