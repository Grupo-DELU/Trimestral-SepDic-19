using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    /// <summary>
    /// Posicion inicial del enemigo laser
    /// </summary>
    private Vector3 initial_position;

     /// <summary>
    /// Numero que divide la pantalla verticalmente
    /// </summary>
    public int screen_height_divider;

    /// <summary>
    /// Numero que estable el tamaño del area de ataque
    /// </summary>
    public int attack_area_range;

    /// <summary>
    /// Punto fijado para el ataque
    /// </summary>
    private Vector3 target;

    /// <summary>
    /// Estado incial luego de su creacion
    /// </summary>
    /// <param name="pos">Posicion inicial del enemigo laser</param>
    /// <param name="line">Altura del area de ataque</param>
    /// <param name="area">Rango del area de ataque </param>
    public void setSpawnPoint(Vector3 pos, int line, int area){
        // Inicializacion de variables
        screen_height_divider = line;
        attack_area_range = area;
        initial_position = pos;

        // Estableciendo estado inicial del enemigo laser
        transform.position = initial_position;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Busca y se ubica en la posicion desde donde iniciara el ataque
    /// </summary>
    public void moveToAttackPoint(){
        int range = screen_height_divider + attack_area_range / 2;
        Vector3 attack_point = new Vector3(0f, Random.Range(-range, range), 0f);

        target = Camera.main.ScreenToWorldPoint(attack_point);
        target += Vector3.right * transform.position.x;
        transform.position = Vector3.MoveTowards(transform.position, target, 0.1f);
    }

    /// <summary>
    /// Dibuja gizmos en el editor
    /// </summary>
    void OnDrawGizmos(){
        Gizmos.color = Color.white;

        Gizmos.DrawLine(transform.position, target);
        Gizmos.DrawSphere(target, 0.1f);
    }

}
