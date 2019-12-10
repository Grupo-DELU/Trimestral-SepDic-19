using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    public static PlayerFinder Finder { get; private set; }
    
    /// <summary>
    /// Jugador
    /// </summary>
    public static GameObject Player
    {
        get { return Finder.player; }
    }

    /// <summary>
    /// Jugador
    /// </summary>
    private GameObject player;

    void Awake()
    {
        #region Singleton
        if (Finder != null && Finder != this)
        {
            Debug.LogWarning("Multiples finder del jugador");
        }
        Finder = this;
        #endregion

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }    
    }
}
