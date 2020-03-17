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
        if (GameStateManager.Manager != null)
        {
            GameStateManager.Manager.onPause.AddListener(PauseShips);
            GameStateManager.Manager.onResume.AddListener(ResumeShips);
        }
    }

    public void AddShip(EnemyShipManager toAdd)
    {
        if (enemyShips.Contains(toAdd)) Debug.LogWarning("Hay una nave duplicada en el almacenador de naves");
        enemyShips.Add(toAdd);
        toAdd.GetComponent<HealthManager>().onDepletedLife.AddListener((a, b) => enemyShips.Remove(toAdd));
        if (GameStateManager.Manager != null)
        {
            if (GameStateManager.Manager.GetCurrentState() == GameStates.Paused)
            {
                Debug.Log("Puto estabas spawneando y estoy pausado >:v!!!!!!!!");
                toAdd.SetAIStatus(false);
                toAdd.shootingSyst.SetSystemOnOff(false);
                toAdd.movementSyst.SetSystemOnOff(false);
            }
        }
    }



    public void PauseShips()
    {
        Debug.Log("parando naves...");
        foreach (EnemyShipManager ship in enemyShips)
        {
            ship.SetAIStatus(false);
            ship.shootingSyst.SetSystemOnOff(false);
            ship.movementSyst.SetSystemOnOff(false);
        }
    }

    public void ResumeShips()
    {
        foreach (EnemyShipManager ship in enemyShips)
        {
            ship.SetAIStatus(true);
            ship.shootingSyst.SetSystemOnOff(true);
            ship.movementSyst.SetSystemOnOff(true);
        }
    }
}
