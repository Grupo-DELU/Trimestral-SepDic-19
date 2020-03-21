using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyManager : MonoBehaviour
{
    /// <summary>
    /// Numero maximo de enemigos laser que posdrian salir a la vez
    /// </summary>
    public int max_enemies_in_screeen = 1;

    /// <summary>
    /// Prefab del enemigo laser a utilizar
    /// </summary>
    public GameObject enemy_laser_prefab;

    /// <summary>
    /// Ancho de la pantalla
    /// </summary>
    private int rect_width;

    /// <summary>
    /// Alto de la pantalla
    /// </summary>
    private int rect_height;

    private GameObject[] laser_enemies;

    private void Awake() {
        Camera camera = GameObject.Find("SceneCamera").GetComponent<Camera>();
        if (camera != null) {
            rect_height = camera.pixelHeight;
            rect_width = camera.pixelWidth;
        }

        if (rect_width < 1 || rect_height < 1)
            Debug.LogError("Tamaño de camara invalido");
        else if (max_enemies_in_screeen < 1)
            Debug.LogError("No hay enemigos laser asignados");
        else 
            Debug.Log(Mathf.RoundToInt(rect_width / max_enemies_in_screeen));
    }

    private void Start(){
        
    }
}
