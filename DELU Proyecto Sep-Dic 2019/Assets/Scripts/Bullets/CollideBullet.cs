using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollideBullet : MonoBehaviour {
    private void OnCollisionEnter(Collision other) {
        Debug.Log(other);
    }
}