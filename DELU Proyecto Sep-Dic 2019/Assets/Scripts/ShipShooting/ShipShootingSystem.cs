using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BulletSpawner))]
public class ShipShootingSystem : MonoBehaviour
{
    /// <summary>
    /// Indica si el sistema de disparo esta activo
    /// </summary>
    [SerializeField]
    private bool bIsActive = true;
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
    protected bool bReloading = false;
    /// <summary>
    /// Numero de balas de la nave por disparo
    /// </summary>
    /// <remarks>Funciona mejor con numeros impares para siempre tener uno en medio</remarks>
    [SerializeField]
    private int iShotNumber = 1;
    public int ShotNumber { get { return iShotNumber; } }
    /// <summary>
    /// Cadencia de disparo de la nave
    /// </summary>
    [SerializeField]
    private float fFireRate = 0.15f;
    /// <summary>
    /// Modificador de velocidad de disparo
    /// </summary>
    [Range(0f, 0.99f)]
    private float fFRModifier = 0;
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
            Debug.LogError("No hay transformada de origen de disparo en la nave! Desactivando...", gameObject);
            bIsActive = false;
        }
    }


    /// <summary>
    /// Dispara la cantidad de balas especificadas eniShotNumber en la transformada 
    /// spawnPos
    /// </summary>
    public void Shoot()
    {
        if (bReloading || !bIsActive) return;
        float sep = 180f / (float)(iShotNumber + 1);
        //bulletShooter.ShootBullet((Vector2)spawnPos.position, spawnPos.up * fBulletSpeed, 0, bulletTeamMask, bulletCollMask);
        for (int i = 1; i <= iShotNumber; i++)
        {
            Vector3 dir1 = (Vector2)spawnPos.right * Mathf.Cos(sep * i * Mathf.Deg2Rad) + (Vector2)spawnPos.up * Mathf.Sin(sep * i * Mathf.Deg2Rad);
            bulletShooter.ShootBullet((Vector2)spawnPos.position, dir1 * fBulletSpeed, 0, bulletTeamMask, bulletCollMask);
        }
        StartCoroutine(FireRate());
    }


    /// <summary>
    /// Corutina que desactiva y activa el disparo dependiendo delfFireRate
    /// </summary>
    public IEnumerator FireRate()
    {
        bReloading = true;
        yield return new WaitForSeconds(fFireRate - fFireRate * fFRModifier);
        bReloading = false;
    }


    public void SetSystemOnOff(bool toSet)
    {
        bIsActive = true;
    }


    public void SetFireRate(float newRate)
    {
        fFireRate = newRate;
    }

    public void SetFireRateModifier(float newModifier)
    {
        fFRModifier = newModifier;
    }


    public void SetShotNumber(int newShots)
    {
        iShotNumber = newShots;
    }


    public void SetBulletSpeed(float newSpeed)
    {
        fBulletSpeed = newSpeed;
    }
}
