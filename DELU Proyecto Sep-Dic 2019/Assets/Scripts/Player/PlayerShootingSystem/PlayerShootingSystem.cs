using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletSpawner))]
public class PlayerShootingSystem : MonoBehaviour
{
    /// <summary>
    /// A que equipo pertenece esta bala
    /// </summary>
    [SerializeField]
    private BulletTeam bulletTeamMask = null;
    /// <summary>
    /// A que equipo le hara dano la bala
    /// </summary>
    [SerializeField]
    private BulletTeam bulletCollMask = null;

    /// <summary>
    /// Indica si la nave puede disparar
    /// </summary>
    [SerializeField]
    private bool bReloading = false;
    /// <summary>
    /// Numero de balas de la nave por disparo
    /// </summary>
    /// <remarks>Funciona mejor con numeros impares para siempre tener uno en medio</remarks>
    [SerializeField]
    private int iShotNumber = 1;
    /// <summary>
    /// Cadencia de disparo de la nave
    /// </summary>
    [SerializeField]
    private float fFireRate = 0.15f;
    /// <summary>
    /// Rapidez de la bala
    /// </summary>
    [SerializeField]
    private float fBulletSpeed = 10f;

    /// <summary>
    /// Posicion de spawneo de la bala
    /// </summary>
    public Transform spawnPos = null;
    /// <summary>
    /// Script de spawn de balas
    /// </summary>
    private BulletSpawner bulletShooter = null;

    private void Awake()
    {
        bulletShooter = GetComponent<BulletSpawner>();
        if (spawnPos == null)
        {
            Debug.LogError("No hay transformada de origen de disparo en la nave!", gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !bReloading)
        {
            Shoot();
        }
    }

    /// <summary>
    /// Dispara la cantidad de balas especificadas eniShotNumber en la transformada 
    /// spawnPos
    /// </summary>
    public void Shoot()
    {
        // FailSafe check
        if (bReloading) return;
        bulletShooter.ShootBullet((Vector2)spawnPos.position, spawnPos.up * fBulletSpeed, 0, bulletTeamMask, bulletCollMask);
        for (int i = 0; i < (iShotNumber - 1)/2; i++)
        {
            float sep = (180 / iShotNumber) * (i + 1);
            Vector3 dir1 = (Vector2)spawnPos.right * Mathf.Cos(sep * Mathf.Deg2Rad) + (Vector2)spawnPos.up * Mathf.Sin(sep * Mathf.Deg2Rad);
            Vector3 dir2 = -(Vector2)spawnPos.right * Mathf.Cos(sep * Mathf.Deg2Rad) + (Vector2)spawnPos.up * Mathf.Sin(sep * Mathf.Deg2Rad);
            bulletShooter.ShootBullet((Vector2)spawnPos.position, dir1 * fBulletSpeed, 0, bulletTeamMask, bulletCollMask);
            bulletShooter.ShootBullet((Vector2)spawnPos.position, dir2 * fBulletSpeed, 0, bulletTeamMask, bulletCollMask);
        }
        StartCoroutine(FireRate());
    }

    /// <summary>
    /// Corutina que desactiva y activa el disparo dependiendo delfFireRate
    /// </summary>
    public IEnumerator FireRate()
    {
        bReloading = true;
        yield return new WaitForSeconds(fFireRate);
        bReloading = false;
    }
}
