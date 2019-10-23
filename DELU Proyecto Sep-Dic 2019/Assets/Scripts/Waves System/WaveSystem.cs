using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public class WavesEvents : UnityEvent<WaveNode, WaveNode> { }
public class WaveSystem : MonoBehaviour
{
    [SerializeField] private WavesGraph wavesGraph;
    [SerializeField] private WaveNode currentNode;

    public WavesEvents onFinishWave = new WavesEvents();
    public WavesEvents onNoMoreWaves = new WavesEvents();
    public WavesEvents onAllWavesCompleted = new WavesEvents();

    float timer = 0;

    public void Start()
    {
        wavesGraph.Start();
        currentNode = wavesGraph.currentNode;
    }

    public void Update()
    {

        if (timer < 2)
        {
            timer += Time.deltaTime;
            return;
        }

        List<EnemyNode> enemies = GetWaveEnemyNodes(currentNode);
        foreach(EnemyNode enemy in enemies)
        {
            Debug.Log(enemy.quantity);
        }
        //Debug.Log(currentNode.name);
        //if (currentNode.GetOutputPort("nextWave") == null) Debug.LogError("EL OUTPUT ME DA NULO 2");
        //NextWave();
    }

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

    public WaveNode GetNextWave(WaveNode node)
    {
        if (!NextWaveIsConnected(node)) return null;
        NodePort output = currentNode.GetOutputPort("nextWave");
        return output.Connection.node as WaveNode;
    }

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
