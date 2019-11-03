﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreEvents : UnityEvent<int> { }
public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Manager { get; private set; }

    /// <summary>
    /// Score actual del jugador.
    /// </summary>
    private int iScore = 0;
    /// <summary>
    /// High Score del jugador.
    /// </summary>
    private int iHighScore;
    /// <summary>
    /// Se llama cuando el score cambia
    /// </summary>
    public ScoreEvents onScoreChange = new ScoreEvents();

    private void Awake()
    {
        #region Singleton
        if (Manager != null && Manager != this)
        {
            Debug.LogError("Manager de Score duplicado!" + this.name, this);
            return; //Asi no hace nada el ultimo en iniciar.
        }
        Manager = this;
        #endregion
    }

    /// <summary>
    /// Suma/Agrega score
    /// </summary>
    /// <param name="toAdd">Score a agregar</param>
    /// <param name="modifier">Modificador de score</param>
    private void AddScore(int toAdd, float modifier = 1f)
    {
        iScore += Mathf.FloorToInt(toAdd * modifier);
        onScoreChange.Invoke(iScore);
    }
}
