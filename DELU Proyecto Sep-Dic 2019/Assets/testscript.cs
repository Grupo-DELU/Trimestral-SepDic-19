using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float speed = 1f;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition((Vector2)transform.position + Vector2.up * speed * Time.fixedDeltaTime);
    }
}
