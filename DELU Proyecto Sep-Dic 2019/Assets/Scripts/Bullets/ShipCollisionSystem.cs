using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using static Unity.Mathematics.math;
using System;

/// <summary>
/// Struct To Mark Ship Collisions
/// </summary>
[Serializable]
public struct ShipCollision : IComponentData {
    /// <summary>
    /// Collision Mask to check for collisions
    /// </summary>
    public int collisionMask;
}

/// <summary>
/// Which ships collision mask it belongs
/// Ex. Bullets should have these
/// </summary>
[Serializable]
public struct ShipCollisionMask : IComponentData {
    /// <summary>
    /// Team of the Collider (Collision Mask) to set collisions
    /// </summary>
    public int belongsTo;

    /// <summary>
    /// Which Teams cause destruction of this entity
    /// </summary>
    public int collidesWith;
}

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ShipCollisionSystem : JobComponentSystem {

    private BuildPhysicsWorld physicsWorldSystem;

    private StepPhysicsWorld stepPhysicsWorldSystem;

    /// <summary>
    /// Command Buffer to Destroy Bullets outside of game zone
    /// </summary>
    EndSimulationEntityCommandBufferSystem m_Barrier;

    protected override void OnCreate() {
        physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        m_Barrier = World
            .DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {

        ComponentDataFromEntity<ShipCollisionMask> shipCollisionMaskFromEntity = GetComponentDataFromEntity<ShipCollisionMask>(true);
        ComponentDataFromEntity<ShipCollision> shipCollisionFromEntity = GetComponentDataFromEntity<ShipCollision>();

        var commandBuffer = m_Barrier.CreateCommandBuffer();

        var jobHandle = new TriggerEventJob() {
            CollisionMaskGroup = GetComponentDataFromEntity<ShipCollisionMask>(true),
            CollisionGroup = GetComponentDataFromEntity<ShipCollision>(),
            CommandBuffer = commandBuffer
        }.Schedule(stepPhysicsWorldSystem.Simulation,
            ref physicsWorldSystem.PhysicsWorld, inputDependencies);

        m_Barrier.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    [BurstCompile]
    private struct TriggerEventJob : ITriggerEventsJob {
        [ReadOnly] public ComponentDataFromEntity<ShipCollisionMask> CollisionMaskGroup;
        public ComponentDataFromEntity<ShipCollision> CollisionGroup;
        public EntityCommandBuffer CommandBuffer;

        public void Execute(TriggerEvent triggerEvent) {
            Entity entityA = triggerEvent.Entities.EntityA;
            Entity entityB = triggerEvent.Entities.EntityB;

            bool bodyAHasCollisionMask = CollisionMaskGroup.Exists(entityA);
            bool bodyBHasCollisionMask = CollisionMaskGroup.Exists(entityB);

            bool bodyAHasCollision = CollisionGroup.Exists(entityA);
            bool bodyBHasCollision = CollisionGroup.Exists(entityB);

            if (bodyAHasCollision && bodyBHasCollisionMask) {
                ApplyCollision(entityB, entityA);
            }

            if (bodyAHasCollisionMask && bodyBHasCollision) {
                ApplyCollision(entityA, entityB);
            }
        }

        /// <summary>
        /// Apply Collision from Entity A to B
        /// </summary>
        /// <param name="entityA">Starting Entity</param>
        /// <param name="entityB">Receiving Entity</param>
        public void ApplyCollision(Entity entityA, Entity entityB) {
            ShipCollisionMask collisionStartMaskComponent = CollisionMaskGroup[entityA]; // Bullet

            ShipCollisionMask collisionEndMaskComponent = CollisionMaskGroup[entityB]; // Ship

            // Test if we care about this collision for Ship
            if ((collisionStartMaskComponent.belongsTo & collisionEndMaskComponent.collidesWith) != 0) {
                // Ship Collision
                ShipCollision collisionEndComponent = CollisionGroup[entityB]; // Ship
                collisionEndComponent.collisionMask |= collisionStartMaskComponent.belongsTo;
                CollisionGroup[entityB] = collisionEndComponent;
            }

            // Test if we care about this collision for Bullet (Destroy it)
            if ((collisionEndMaskComponent.belongsTo & collisionStartMaskComponent.collidesWith) != 0) {
                // Bullet Collision
                // TODO see if it is better to just add component
                CommandBuffer.DestroyEntity(entityA);
            }
        }
    }
}