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

[UpdateAfter (typeof (EndFramePhysicsSystem))]
public class ShipCollisionSystem : JobComponentSystem {

    private BuildPhysicsWorld physicsWorldSystem;

    private CollisionWorld collisionWorld;

    protected override void OnCreate () {
        physicsWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld> ();
        collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
    }

    /// <summary>
    /// Check if something collided with a physics collider (Gets the closests collision)
    /// This only exists because lambdas cant be unsafe
    /// </summary>
    /// <param name="col">Collider to test</param>
    /// <param name="translation">Its possition</param>
    /// <param name="rotation">Its rotation</param>
    /// <param name="physicsWorld">Where it belongs</param>
    /// <param name="collisionWorld">Where it belongs</param>
    /// <returns>Entity of the collision or null</returns>
    public unsafe static Entity Execute (in PhysicsCollider col, in Translation translation, in Rotation rotation, in PhysicsWorld physicsWorld, CollisionWorld collisionWorld) {
        ColliderCastInput input = new ColliderCastInput () {
            Collider = col.ColliderPtr,
            Orientation = rotation.Value,
            Start = translation.Value,
            End = translation.Value
        };

        ColliderCastHit hit = new ColliderCastHit ();
        bool haveHit = collisionWorld.CastCollider (input, out hit);
        if (haveHit) {
            // see hit.Position
            // see hit.SurfaceNormal
            Entity e = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            return e;
        }
        return Entity.Null;
    }

    protected override JobHandle OnUpdate (JobHandle inputDependencies) {

        // Assign values to the fields on your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     job.deltaTime = UnityEngine.Time.deltaTime;

        var localPhysicsWorld = physicsWorldSystem.PhysicsWorld;
        var localCollisionWorld = collisionWorld;

        ComponentDataFromEntity<ShipCollisionMask> shipCollisionMaskFromEntity = GetComponentDataFromEntity<ShipCollisionMask> (true);

        var jobHandle = Entities.
        WithName ("ShipCollisionSystem").
        WithBurst (FloatMode.Default, FloatPrecision.Standard, true).
        ForEach (
            (ref ShipCollision ship, in PhysicsCollider col, in Translation translation, in Rotation rotation) => {
                Entity collided = Execute (col, translation, rotation, localPhysicsWorld, localCollisionWorld);
                if (collided != Entity.Null) {
                    if (shipCollisionMaskFromEntity.Exists (collided)) {
                        ShipCollisionMask collision = shipCollisionMaskFromEntity[collided];
                        ship.collisionMask |= collision.collisionMask;
                    }
                }
            }
        ).Schedule (inputDependencies);

        return jobHandle;
    }
}