using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;

public class ShipCollider : MonoBehaviour
{

    /// <summary>
    /// Entity Manager
    /// </summary>
    private EntityManager _manager = null;

    /// <summary>
    /// Entity Associated with ship
    /// </summary>
    private Entity _entity;

    /// <summary>
    /// Current Translation
    /// </summary>
    private Translation translation;

    /// <summary>
    /// Current Rotation
    /// </summary>
    private Rotation rotation;

    /// <summary>
    /// Current Collision if any
    /// </summary>
    private ShipCollision collision;

    /// <summary>
    /// Collides With Collision Mask
    /// </summary>
    [Tooltip("Collides With Collision Mask")]
    [SerializeField]
    private BulletTeam collidesWith = new BulletTeam();

    /// <summary>
    /// Belongs To Collision Mask
    /// </summary>
    [Tooltip("Belongs To Collision Mask")]
    [SerializeField]
    private BulletTeam belongsTo = new BulletTeam();

    /// <summary>
    /// Unity Event to send two ints 
    /// </summary>
    [System.Serializable]
    public class IntEvent : UnityEvent<int, int>
    {

    }

    /// <summary>
    /// On Collision Event (Bullet Collision Mask, Collider Collision Mask)
    /// </summary>
    [Tooltip("On Collision Event (Bullet Collision Mask, Collider Collision Mask)")]
    [SerializeField]
    public IntEvent onCollision;

    /// <summary>
    /// Ship Event for other systems to use
    /// </summary>
    [System.Serializable]
    public class ShipEvent : UnityEvent<ShipCollider>
    {

    }

    /// <summary>
    /// Event Called when the setup is complete
    /// </summary>
    [Tooltip("Event Called when the setup is complete")]
    [SerializeField]
    public ShipEvent onSetupComplete;

    /// <summary>
    /// Event Called just before the ship collider is destroyed
    /// </summary>
    [Tooltip("Event Called just before the ship collider is destroyed")]
    [SerializeField]
    public ShipEvent onSetupDestroy;

    /// <summary>
    /// Register this Ship Collider
    /// </summary>
    /// <param name="entity">Associated Entity</param>
    /// <param name="manager">Entity manager</param>
    public void Register(Entity entity, EntityManager manager)
    {
        _manager = manager;
        _entity = entity;
        translation = new Translation { Value = transform.position };
        rotation = new Rotation { Value = transform.rotation };
        _manager.SetComponentData(_entity, translation);
        _manager.SetComponentData(_entity, rotation);
        _manager.AddComponentData(_entity, new ShipCollision { collisionMask = 0 });
        _manager.AddComponentData(_entity, new ShipCollisionMask { belongsTo = belongsTo.value, collidesWith = collidesWith.value });
        onSetupComplete.Invoke(this);
    }

    private void Update()
    {
        if (_manager != null)
        {
            translation.Value = transform.position;
            rotation.Value = transform.rotation;
            _manager.SetComponentData(_entity, translation);
            _manager.SetComponentData(_entity, rotation);
            collision = _manager.GetComponentData<ShipCollision>(_entity);
            if (collision.collisionMask != 0)
            {

                // Inform Event
                onCollision.Invoke(collision.collisionMask, collidesWith.value);
                collision.collisionMask = 0;
                _manager.SetComponentData(_entity, collision);
            }
        }
    }

    private void OnDestroy()
    {
        onSetupDestroy.Invoke(this);
        if (!_manager.IsCreated)
        {
            _manager = null;
        }

        if (_manager != null && _manager.Exists(_entity))
        {
            _manager.DestroyEntity(_entity);
        }
        _manager = null;
    }
}