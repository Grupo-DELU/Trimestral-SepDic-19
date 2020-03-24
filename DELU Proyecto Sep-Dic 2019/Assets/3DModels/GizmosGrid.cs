using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GizmosGrid : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                Gizmos.DrawWireCube(Vector3.right * j + Vector3.up * i, Vector3.one);
                Gizmos.DrawWireCube(Vector3.right * -j + Vector3.up * -i, Vector3.one);
                Gizmos.DrawWireCube(Vector3.right * j + Vector3.up * -i, Vector3.one);
                Gizmos.DrawWireCube(Vector3.right * -j + Vector3.up * i, Vector3.one);
            }
        }
    }
}


