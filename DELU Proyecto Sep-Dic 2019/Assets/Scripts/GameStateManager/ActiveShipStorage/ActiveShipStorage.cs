using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveShipStorage : MonoBehaviour
{
    public static ActiveShipStorage Storage { get; private set; }

    /// <summary>
    /// Naves enemigas activas en escena
    /// </summary>
    [SerializeField]
    private List<EnemyShipManager> enemyShips = new List<EnemyShipManager>();

    private void Awake()
    {
        #region Singleton
        if (Storage != null && Storage != this)
        {
            Debug.LogError("Hay dos almacenadores de naves enemigas! Borrando uno...", gameObject);
            Destroy(gameObject);
        }
        Storage = this;
        #endregion
    }

    private void Start()
    {
        if (enemyShips == null) enemyShips = new List<EnemyShipManager>();
    }

    public void AddShip(EnemyShipManager toAdd)
    {
        if (enemyShips.Contains(toAdd)) Debug.LogWarning("Hay una nave duplicada en el almacenador de naves");
        enemyShips.Add(toAdd);
        toAdd.GetComponent<HealthManager>().onDepletedLife.AddListener((a, b) => enemyShips.Remove(toAdd));
    }
}
