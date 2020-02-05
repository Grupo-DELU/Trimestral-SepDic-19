﻿using System.Collections;
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
    /// Collision Mask
    /// </summary>
    [Tooltip("Collision Mask")]
    [SerializeField]
    private BulletTeam collisionMask;

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
        _manager.AddComponentData (_entity, new ShipCollision { collisionMask = 0 });
    }

    private void Update () {
        if (_manager != null) {
            translation.Value = transform.position;
            rotation.Value = transform.rotation;
            _manager.SetComponentData (_entity, translation);
            _manager.SetComponentData (_entity, rotation);
            collision = _manager.GetComponentData<ShipCollision> (_entity);
            if (collision.collisionMask != 0) {

                for (int i = 0; i < 32; i++) {
                    if ((collision.collisionMask & (1 << i)) != 0) {
                        Debug.Log (i);
                    }
                }
                collision.collisionMask = 0;
                _manager.SetComponentData (_entity, collision);
            }
        }
    }

    private void OnDestroy () {
        if (_manager != null && _manager.Exists (_entity)) {
            _manager.DestroyEntity (_entity);
        }
        _manager = null;
    }
}