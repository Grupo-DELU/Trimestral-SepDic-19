using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ShipCollider : MonoBehaviour {

    /// <summary>
    /// Entity Manager
    /// </summary>
    private EntityManager _manager = null;

    /// <summary>
    /// Entity Associated with ship
    /// </summary>
    private Entity _entity;

    private Translation translation;

    private Rotation rotation;

    /// <summary>
    /// Register this Ship Collider
    /// </summary>
    /// <param name="entity">Associated Entity</param>
    /// <param name="manager">Entity manager</param>
    public void Register (Entity entity, EntityManager manager) {
        _manager = manager;
        _entity = entity;
        translation = new Translation { Value = transform.position };
        rotation = new Rotation { Value = transform.rotation };
        _manager.SetComponentData (_entity, translation);
        _manager.SetComponentData (_entity, rotation);
    }

    private void Update () {
        if (_manager != null) {
            translation.Value = transform.position;
            rotation.Value = transform.rotation;
            _manager.SetComponentData (_entity, translation);
            _manager.SetComponentData (_entity, rotation);
        }
    }

    private void OnDestroy () {
        if (_manager != null && _manager.Exists (_entity)) {
            _manager.DestroyEntity (_entity);
        }
    }
}