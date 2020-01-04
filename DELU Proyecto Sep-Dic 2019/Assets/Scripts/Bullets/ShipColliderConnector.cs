using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShipColliderConnector : MonoBehaviour, IConvertGameObjectToEntity {
    void IConvertGameObjectToEntity.Convert (Entity entity, EntityManager entityManager, GameObjectConversionSystem gameObjectConversionSystem) {
        if (transform.parent == null) {
            Debug.LogError ("Parent is not assigned", this);
        }

        ShipCollider shipCollider = transform.parent.GetComponent<ShipCollider> ();

        if (shipCollider == null) {
            Debug.LogError ("Parent ShipCollider is not present", this);
        }
        
        shipCollider.Register (entity, entityManager);
    }
}