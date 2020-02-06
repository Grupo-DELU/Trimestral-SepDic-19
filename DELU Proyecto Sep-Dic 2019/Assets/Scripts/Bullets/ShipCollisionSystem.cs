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
    /// Collision Mask to set collisions
    /// </summary>
    public int collisionMask;
}

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class ShipCollisionSystem : JobComponentSystem {

    private BuildPhysicsWorld physicsWorldSystem;

    private StepPhysicsWorld stepPhysicsWorldSystem;

    protected override void OnCreate() {
        physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {

        ComponentDataFromEntity<ShipCollisionMask> shipCollisionMaskFromEntity = GetComponentDataFromEntity<ShipCollisionMask>(true);
        ComponentDataFromEntity<ShipCollision> shipCollisionFromEntity = GetComponentDataFromEntity<ShipCollision>();

        var jobHandle = new TriggerEventJob() {
            CollisionMaskGroup = GetComponentDataFromEntity<ShipCollisionMask>(true),
                CollisionGroup = GetComponentDataFromEntity<ShipCollision>(),
        }.Schedule(stepPhysicsWorldSystem.Simulation,
            ref physicsWorldSystem.PhysicsWorld, inputDependencies);

        return jobHandle;
    }

    [BurstCompile]
    private struct TriggerEventJob : ITriggerEventsJob {
        [ReadOnly] public ComponentDataFromEntity<ShipCollisionMask> CollisionMaskGroup;
        public ComponentDataFromEntity<ShipCollision> CollisionGroup;

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
            ShipCollisionMask collisionMaskComponent = CollisionMaskGroup[entityA];
            ShipCollision collisionComponent = CollisionGroup[entityB];
            collisionComponent.collisionMask |= collisionMaskComponent.collisionMask;
            CollisionGroup[entityB] = collisionComponent;
        }
    }
}