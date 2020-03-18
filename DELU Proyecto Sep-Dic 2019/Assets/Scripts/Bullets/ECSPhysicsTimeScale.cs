// From https://forum.unity.com/threads/how-to-do-frame-rate-independent-physics-in-dots.844252/

using Unity.Entities;
using UnityEngine;
using Unity.Physics.Systems;



[UpdateBefore(typeof(BuildPhysicsWorld))]
public class PrePhysicsSetDeltaTimeSystem : ComponentSystem
{
    
    /// <summary>
    /// If the physics system is using directly deltaTime rather fixedDeltaTime
    /// </summary>
    public bool IsRealTimeStep = false;

    /// <summary>
    /// Time Scale to use
    /// </summary>
    public float TimeScale = 1.0f;

    /// <summary>
    /// Previous Delta Time Restore Variable
    /// </summary>
    internal float PreviousDeltaTime { get; private set; } = UnityEngine.Time.fixedDeltaTime;

    protected override void OnUpdate()
    {
        PreviousDeltaTime = Time.fixedDeltaTime;

        if (IsRealTimeStep)
        {
            UnityEngine.Time.fixedDeltaTime = UnityEngine.Time.deltaTime * TimeScale;
        }
        else
        {
            UnityEngine.Time.fixedDeltaTime *= TimeScale;
        }
    }
}

[UpdateAfter(typeof(ExportPhysicsWorld))]
public class PostPhysicsResetDeltaTimeSystem : ComponentSystem
{
    public PrePhysicsSetDeltaTimeSystem preSystem;

    protected override void OnCreate()
    {
        preSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PrePhysicsSetDeltaTimeSystem>();
    }

    protected override void OnUpdate()
    {
        UnityEngine.Time.fixedDeltaTime = preSystem.PreviousDeltaTime;
    }
}