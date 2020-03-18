using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

public class ECSPhysicsManager : MonoBehaviour
{
    /// <summary>
    /// Step Physics System
    /// </summary>
    private StepPhysicsWorld stepPhysicsWorld = null;

    /// <summary>
    /// TimeScale System
    /// </summary>
    private PrePhysicsSetDeltaTimeSystem prePhysicsSetDeltaTimeSystem = null;

    /// <summary>
    /// If the ECS Physics Updating is Paused
    /// </summary>
    public bool IsPaused { get { return stepPhysicsWorld != null && !stepPhysicsWorld.Enabled; } }

    /// <summary>
    /// Current Manager
    /// </summary>
    public static ECSPhysicsManager Manager { get; private set; } = null;

    private void Awake()
    {
        if (Manager && Manager != this)
        {
            Debug.LogError("ECSPhysicsManager is duplicated", this);
            Debug.DebugBreak();
            Destroy(this);
            return;
        }
        Manager = this;
        stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        prePhysicsSetDeltaTimeSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PrePhysicsSetDeltaTimeSystem>();
    }

    private void Start()
    {
        if (Manager == this)
        {
            if (!GameStateManager.Manager)
            {
                Debug.LogError("Failed to Find GameStateManager", this);
                Debug.DebugBreak();
            }
            // Register to Game State Manager Manager
            GameStateManager.Manager.onResume.AddListener(UnPaused);
            GameStateManager.Manager.onPause.AddListener(Paused);
        }
    }

    private void OnDestroy()
    {
        if (Manager == this)
        {
            // Register to Game State Manager Manager
            GameStateManager.Manager.onResume.RemoveListener(UnPaused);
            GameStateManager.Manager.onPause.AddListener(Paused);
        }
    }

    /// <summary>
    /// Set the ECS Physics Updating in a new State
    /// </summary>
    /// <param name="paused">If the updating is to pause</param>
    public void SetPaused(bool paused)
    {
        if (stepPhysicsWorld != null)
        {
            stepPhysicsWorld.Enabled = !paused;
        }
    }

    /// <summary>
    /// Set the ECS Physics Updating paused
    /// </summary>
    public void Paused()
    {
        SetPaused(true);
    }

    /// <summary>
    /// Set the ECS Physics Updating unpaused
    /// </summary>
    public void UnPaused()
    {
        SetPaused(false);
    }

    /// <summary>
    /// Set TimeScale for ECS Physics, note that 0.0 or 1.0 doesn't pause or unpause
    /// </summary>
    /// <param name="scale"></param>
    public void SetTimeScale(float scale)
    {
        if (stepPhysicsWorld != null)
        {
            prePhysicsSetDeltaTimeSystem.TimeScale = scale;
        }
    }

    /// <summary>
    /// If the physics system is using directly deltaTime rather fixedDeltaTime
    /// </summary>
    /// <param name="useRealTimeStep">True to use directly deltaTime</param>
    public void SetTimeRealTimeStep(bool useRealTimeStep)
    {
        if (stepPhysicsWorld != null)
        {
            prePhysicsSetDeltaTimeSystem.IsRealTimeStep = useRealTimeStep;
        }
    }
}
