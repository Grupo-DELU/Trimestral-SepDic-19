using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Bullet Spawner System
/// </summary>
public class BulletSpawner : MonoBehaviour {
    /// <summary>
    /// Bullet Prefab to use
    /// </summary>
    [Tooltip ("Bullet Prefab to use")]
    [SerializeField]
    private GameObject bulletPrefab = null;

    /// <summary>
    /// Bullet Prefab representation in ECS
    /// </summary>
    private Entity bulletPrefabEntity;

    /// <summary>
    /// World ECS Manager
    /// </summary>
    private EntityManager worldEntityManager;

#if UNITY_EDITOR
    [Header ("Debug")]
    /// <summary>
    /// Stress Test Amount to Spawn per Press
    /// </summary>
    [Tooltip ("Stress Test Amount to Spawn per Press")]
    [SerializeField]
    private int testSpawn = 100;
#endif // UNITY_EDITOR

    private BlobAssetStore blobAssetStore;

    private void Start () {
        blobAssetStore = new BlobAssetStore();
        // Get ECS representation
        var settings 
            = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        bulletPrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy (bulletPrefab, settings);
        // Get Current ECS manager
        worldEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    /// <summary>
    /// Shoot a Bullet at a position with a velocity
    /// </summary>
    /// <param name="position">Position to shoot the bullet</param>
    /// <param name="velocity">Bullet's Velocity</param>
    /// <param name="angularSpeed">Bullet's Angular Speed in Radians</param>
    public void ShootBullet (Vector2 position, Vector2 velocity, float angularSpeed) {
        // Create new bullet instance
        Entity newBullet = worldEntityManager.Instantiate (bulletPrefabEntity);

        // Convert to 3D
        Vector3 bulletVelocity = new Vector3 (velocity.x, velocity.y, 0.0f);

        // Add the Position component
        float3 bulletPos = new float3 (position.x, position.y, 0.0f);
        worldEntityManager.SetComponentData (newBullet, new Translation { Value = bulletPos });

        // Add the rotation component
        quaternion bulletRot = Quaternion.LookRotation (Vector3.forward, bulletVelocity);
        worldEntityManager.SetComponentData (newBullet, new Rotation { Value = bulletRot });

        // Add the Bullet Movement
        worldEntityManager.AddComponentData (newBullet, new BulletMovement { speed = velocity.magnitude });
        
        // Set up the Bullet Physics movement
        worldEntityManager.SetComponentData (newBullet,
            new PhysicsVelocity { Linear = bulletVelocity, Angular = new Vector3 (0.0f, 0.0f, angularSpeed) }
        );
    }

    private void Update () {
#if UNITY_EDITOR
        if (Input.GetButtonDown ("Jump")) {
            for (int i = 0; i < testSpawn; i++) {
                Vector2 pos;
                pos.x = UnityEngine.Random.Range (-4.0f, 4.0f);
                pos.y = UnityEngine.Random.Range (-4.0f, 4.0f);
                Vector2 vel;
                vel.x = UnityEngine.Random.Range (-1.0f, 1.0f);
                vel.y = UnityEngine.Random.Range (-1.0f, 1.0f);
                vel.Normalize ();
                vel *= UnityEngine.Random.Range (10.0f, 20.0f);
                ShootBullet (pos, vel, UnityEngine.Random.Range (0.0f, 0.5f) * Mathf.PI);
            }
        }
#endif // UNITY_EDITOR
    }

    private void OnDestroy() {
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }
}