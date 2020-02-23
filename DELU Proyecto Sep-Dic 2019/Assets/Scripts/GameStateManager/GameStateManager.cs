using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public enum GameStates { Starting, Playing, Paused, LevelOver, GameOver }
public class GameStateEvents : UnityEvent { }

public class GameStateManager : MonoBehaviour
{
    // private GameStates state = GameStates.Starting;

    [SerializeField]
    public GameStateEvents onGameStart = new GameStateEvents();
    [SerializeField]
    public GameStateEvents onPause = new GameStateEvents();
    [SerializeField]
    public GameStateEvents onResume = new GameStateEvents();
    [SerializeField]
    public GameStateEvents onGameOver = new GameStateEvents();
    [SerializeField]
    public GameStateEvents onLevelOver = new GameStateEvents();
}
