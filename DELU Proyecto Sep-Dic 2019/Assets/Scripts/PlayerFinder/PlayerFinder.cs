using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    public static PlayerFinder Finder { get; private set; }

    public static GameObject Player
    {
        get { return Finder.player; }
    }

    private GameObject player;

    void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }    
    }
}
