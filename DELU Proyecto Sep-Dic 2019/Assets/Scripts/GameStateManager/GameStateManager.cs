using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum GameStates { Starting, Playing, Paused, LevelOver, GameOver }
public class GameStateEvents : UnityEvent { }

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Manager { get; private set; }

    /// <summary>
    /// Estado actual del juego
    /// </summary>
    private GameStates state = GameStates.Starting;

    /// <summary>
    /// Delay antes de iniciar el juego
    /// </summary>
    private float fStartDelay = 4f;

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

    private void Awake()
    {
        #region Singleton
        if (Manager != null && Manager != this)
        {
            Debug.LogWarning("Manager de estado de juego duplicado! Intentado destruir uno", gameObject);
            Debug.LogWarning("Manager de estado de juego destruido", Manager.gameObject);
        }
        Manager = this;
        #endregion
    }

    private void Start()
    {
        StartCoroutine(StartGame());

        if (LevelWavesManager.Manager == null) Debug.LogError("No hay Manager de waves para el GameStateManager!", gameObject);
        else LevelWavesManager.Manager.onAllWavesCompleted.AddListener(FinishLevel);
    }

    private void Update()
    {
        if (state == GameStates.Playing && Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        else if (state == GameStates.Playing && Input.GetKeyDown(KeyCode.Escape)) ResumeGame();

    }

    /// <summary>
    /// Setea el estado del juego como en pausa y manda la senal asociada a pausar
    /// </summary>
    public void PauseGame()
    {
        state = GameStates.Paused;
        onPause.Invoke();
    }

    /// <summary>
    /// Setea el estado del juego como en juego y manda la senal asociada a continuar
    /// </summary>
    public void ResumeGame()
    {
        state = GameStates.Playing;
        onResume.Invoke();
    }

    /// <summary>
    /// Setea el estado del juego como nivel completado y manda la senal asociada a fin de nivel
    /// </summary>
    public void FinishLevel()
    {
        state = GameStates.LevelOver;
        onLevelOver.Invoke();
    }

    /// <summary>
    /// Setea el estado del juego como perdido y manda la senal asociada a gameover
    /// </summary>
    public void GameOver()
    {
        state = GameStates.GameOver;
        onGameOver.Invoke();
    }

    /// <summary>
    /// Empieza el sistema de waves tras un delay y manda la senal asociada a inicio de juego
    /// </summary>
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(fStartDelay);
        state = GameStates.Playing;
        onGameStart.Invoke();
        // Empieza el sistema de waves (quizas deberia estar como listener del manager de juegos!)
        LevelWavesManager.Manager.StartLevelWaves();
    }

    /// <summary>
    /// Devuelve el estado actual del juego
    /// </summary>
    /// <returns>Estado actual del juego en forma de enum</returns>
    public GameStates GetCurrentState()
    {
        return state;
    }
}
