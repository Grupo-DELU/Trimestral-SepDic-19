using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "Wave_Graph", menuName = "Wave System/WaveGraph")]
public class WavesGraph : NodeGraph
{
    public WaveNode initialWave;
    public WaveNode currentNode;

    public void RestartWave()
    {
        currentNode = initialWave;
        //if (initialWave == null) Debug.LogError("LA WAVE INICIAL ES NULA!");
        //if (currentNode == null) Debug.LogError("EL NODO ACTUAL ES NULO!");
    }
    public void NextWave()
    {
        //if (currentNode == null) Debug.LogError("EL NODO ACTUAL ES NULO!");
        //if (currentNode.GetOutputPort("nextWave") == null) Debug.LogError("EL OUTPUT ME DA NULO");
        NodePort otherPort = currentNode.GetOutputPort("nextWave").Connection;
        if (currentNode.GetOutputPort("nextWave").Connection == null) Debug.LogError("EL OUTPUT ME DA NULO");
        if (otherPort.node != null)
        {
            currentNode = otherPort.node as WaveNode;
        }
    }
}
