using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingController : ShipShootingSystem
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !bReloading)
        {
            Shoot();
        }
    }
}
