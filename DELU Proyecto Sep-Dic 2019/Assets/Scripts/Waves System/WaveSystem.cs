using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public class WavesEvents : UnityEvent<WaveNode, WaveNode> { }
/// <summary>
/// Wave System se encarga de llamar a las waves y navegar entre ellas,
/// no se encarga del spawneo ni de nada de eso. 
/// </summary>
public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Manager { get; private set; }
    /// <summary>
    /// Indica si se esta en descanso de ronda
    /// </summary>
    private bool bIsResting = false;

    /// <summary>
    /// Grafo de las waves
    /// </summary>
    [SerializeField] private WavesGraph wavesGraph;
    /// <summary>
    /// Nodo de la wave actual
    /// </summary>
    [SerializeField] private WaveNode currentNode;

    //esta deberia de ir en el manager de la wave
    public WavesEvents onFinishWave = new WavesEvents();
    public WavesEvents onNoMoreWaves = new WavesEvents();
    public WavesEvents onAllWavesCompleted = new WavesEvents();

    /// <summary>
    /// Corutina de descanso de wave
    /// </summary>
    private Coroutine cWaveRestRoutine;

    private void Awake()
    {
        #region Singleton
        if (Manager != null && Manager != this)
        {
            Debug.LogError("Hay dos sistemas de wave!");
        }
        Manager = this;
        #endregion
    }
    public void Start()
    {
        wavesGraph.Start();
        currentNode = wavesGraph.currentNode;
    }

    public void Update()
    {
    }

    /// <summary>
    /// Accede a la siguiente wave del grafo, si no hay, activa evento de que terminaron
    /// </summary>
    public void NextWave()
    {
        if (currentNode.GetOutputPort("nextWave") == null) Debug.LogError("No hay un puerto de salida nextWave", currentNode);
        if (NextWaveIsConnected(currentNode))
        {
            wavesGraph.NextWave();
            currentNode = wavesGraph.currentNode;
        }
        else
        {
            onAllWavesCompleted.Invoke(currentNode, currentNode);
        }
    }

    /// <summary>
    /// Chequea si hay una wave siguiente
    /// </summary>
    /// <param name="node">Nodo a chequear</param>
    /// <returns>Si hay una wave mas</returns>
    public bool NextWaveIsConnected(WaveNode node)
    {
        if (node.GetOutputPort("nextWave").IsConnected)
        {
            NodePort output = node.GetOutputPort("nextWave");
            if (output.GetConnections().Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Chequea si la wave tiene enemigos ???????????????????????????????????????????????
    /// </summary>
    /// <param name="node">Nodo a chequear</param>
    /// <returns>Si tiene o no enemigos</returns>
    public  bool WaveHaveEnemies(WaveNode node)
    {
        if (node.GetOutputPort("enemies").IsConnected)
        {
            NodePort output = node.GetOutputPort("enemies");
            if (output.GetConnections().Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Retorna la siguiente wave
    /// </summary>
    /// <param name="node">Nodo actual</param>
    /// <returns>La siguiente wave del nodo</returns>
    public WaveNode GetNextWave(WaveNode node)
    {
        if (!NextWaveIsConnected(node)) return null;
        NodePort output = currentNode.GetOutputPort("nextWave");
        return output.Connection.node as WaveNode;
    }

    /// <summary>
    /// Nodos de enemigos de la wave
    /// </summary>
    /// <param name="node">Nodo a revisar</param>
    /// <returns>Los nodos de enemigos si hay</returns>
    public List<EnemyNode> GetWaveEnemyNodes(WaveNode node)
    {
        if (!WaveHaveEnemies(node)) return null;
        List<NodePort> connections = node.GetOutputPort("enemies").GetConnections();
        List<EnemyNode> enemies = new List<EnemyNode>();
        foreach(NodePort port in connections)
        {
            enemies.Add(port.node as EnemyNode);
        }
        return enemies;
    }

    /// <summary>
    /// Inicia el descanso entre rondas
    /// </summary>
    /// <param name="node">Wave a descansar</param>
    public void StartRest(WaveNode node)
    {
        if (cWaveRestRoutine != null) StopCoroutine(cWaveRestRoutine);
        cWaveRestRoutine = StartCoroutine(RoundResting(node.roundRestingTime));
    }

    /// <summary>
    /// Activa el descanso de waves
    /// </summary>
    /// <param name="time">Tiempo a descansar</param>
    /// <returns>Corutina</returns>
    public IEnumerator RoundResting(float time)
    {
        bIsResting = true;
        yield return new WaitForSeconds(time);
        bIsResting = false;
        //Deberia activar la siguiente wave?
    }
}

public struct WaveInfo
{
    public int iQuantity;
    public ScriptableObject baseEnemy;
    public List<float> l_fDelays;

    public WaveInfo(EnemyNode node)
    {
        iQuantity = node.quantity;
        baseEnemy = node.bases;
        l_fDelays = node.delay;
    }
}
