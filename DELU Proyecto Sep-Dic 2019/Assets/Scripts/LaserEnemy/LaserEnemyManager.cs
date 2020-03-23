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
    /// Numero de spawners
    /// </summary>
    public int number_of_spawners = 1;

    /// <summary>
    /// Numero que divide la pantalla verticalmente
    /// </summary>
    public int screen_height_divider = 3;

    /// <summary>
    /// Desplazamiento del area de ataque hacai arriba
    /// </summary>
    public int pixel_height_offset = 200;

    /// <summary>
    /// Prefab del enemigo laser a utilizar
    /// </summary>
    public GameObject laser_enemy_prefab;

    /// <summary>
    /// Ancho de la pantalla
    /// </summary>
    private int rect_width;

    /// <summary>
    /// Alto de la pantalla
    /// </summary>
    private int rect_height;

    /// <summary>
    /// Arreglo de enemigos laser
    /// </summary>
    private GameObject[] laser_enemies;

    private void Awake() {
        Camera camera = Camera.main;
        if (camera != null) {
            rect_height = camera.pixelHeight;
            rect_width = camera.pixelWidth;
        }

        if (rect_width < 1 || rect_height < 1)
            Debug.LogError("Tamaño de camara invalido" + rect_height + "--" + rect_width);
        if (max_enemies_in_screeen < 1)
            Debug.LogError("No hay enemigos laser asignados");
    }

    private void Start(){
        setSpawnPoints();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Return)){
            for (int i = 0; i < number_of_spawners; i++){
                LaserEnemy script = laser_enemies[i].GetComponent<LaserEnemy>();
                script.moveToAttackPoint();
            }
        }
    }

    /// <summary>
    /// Establece los puntos de aparicion de los enemigos laser basado en las
    /// dimensiones de la pantalla y la cantidad de enemigos disponibles.
    /// </summary>
    void setSpawnPoints(){
        laser_enemies = new GameObject[number_of_spawners];

        int segment = Mathf.RoundToInt(rect_width / number_of_spawners);
        int mid_point = Mathf.RoundToInt(segment / 2);

        for (int i = 0; i < number_of_spawners; i++){
            laser_enemies[i] = (GameObject)(Instantiate(laser_enemy_prefab));
            LaserEnemy script = laser_enemies[i].GetComponent<LaserEnemy>();

            Vector3 pos = new Vector3(mid_point + segment * i, rect_height, 0f);
            Vector3 new_pos = Camera.main.ScreenToWorldPoint(pos);

            // Inicializacion de la instancia del enemigo laser
            script.setSpawnPoint(
                new_pos, 
                rect_height / 2 + pixel_height_offset,
                rect_height / screen_height_divider + pixel_height_offset
            );
        }
    }

    /// <summary>
    /// Dibuja gizmos en el editor
    /// </summary>
    void OnDrawGizmos() {
        Vector2 from = new Vector2(0f, rect_height / 2 + pixel_height_offset);
        Vector2 to = 
            new Vector2(
                rect_width, 
                rect_height / 2 + pixel_height_offset
            );

        float range = rect_height / screen_height_divider + pixel_height_offset;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            Camera.main.ScreenToWorldPoint(from), 
            Camera.main.ScreenToWorldPoint(to)
        );

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            Camera.main.ScreenToWorldPoint(from + Vector2.up * range / 2), 
            Camera.main.ScreenToWorldPoint(to + Vector2.up * range / 2)
        );

        Gizmos.DrawLine(
            Camera.main.ScreenToWorldPoint(from + Vector2.down * range / 2), 
            Camera.main.ScreenToWorldPoint(to + Vector2.down * range / 2)
        );
    }
}
