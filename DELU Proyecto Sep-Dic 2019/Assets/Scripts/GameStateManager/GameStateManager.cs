using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum GameStates { Starting, Playing, Paused, LevelOver, GameOver }
public class GameStateEvents : UnityEvent { }

//[RequireComponent(typeof(ShipCurveInitializer))]
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
    [SerializeField]
    private float fStartDelay = 4f;

    /// <summary>
    /// TimeScale anterior
    /// </summary>
    private float prevTimeScale = 1;

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
        //StartCoroutine(StartGameRoutine());
        //GetComponent<ShipCurveInitializer>().onCurvesRdy.AddListener(() => StartCoroutine(StartGameRoutine()));
        // Asociamos el fin del nivel al fin de waves
        StartCoroutine(StartGameRoutine());

        if (LevelWavesManager.Manager == null) Debug.LogError("No hay Manager de waves para el GameStateManager!", gameObject);
        else LevelWavesManager.Manager.onAllWavesCompleted.AddListener(FinishLevel);

        // Asociamos el GameOver al agotamiento de vidas del jugador
        if (PlayerFinder.Player == null) Debug.LogError("No hay jugado para el GameStateManager!");
        else PlayerFinder.Player.GetComponent<LivesSystem>().onLivesDepleted.AddListener((a) => GameOver());
    }


    private void Update()
    {
        if (state == GameStates.Playing && Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        else if (state == GameStates.Paused && Input.GetKeyDown(KeyCode.Escape)) ResumeGame();
    }


    /// <summary>
    /// Setea el estado del juego como en pausa y manda la senal asociada a pausar
    /// </summary>
    public void PauseGame()
    {
        state = GameStates.Paused;
        onPause.Invoke();
        SetTimeScale(0);
    }

    
    /// <summary>
    /// Setea el estado del juego como en juego y manda la senal asociada a continuar
    /// </summary>
    public void ResumeGame()
    {
        state = GameStates.Playing;
        onResume.Invoke();
        SetTimeScale(prevTimeScale);
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
    private IEnumerator StartGameRoutine()
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


    /// <summary>
    /// Setea el timeScale del juego y guarda el anterior en caso de pausa y slowmo 
    /// </summary>
    /// <param name="toSet">Nuevo timeScale</param>
    public void SetTimeScale(float toSet)
    {
        prevTimeScale = Time.timeScale;
        Time.timeScale = toSet;
    }
}
