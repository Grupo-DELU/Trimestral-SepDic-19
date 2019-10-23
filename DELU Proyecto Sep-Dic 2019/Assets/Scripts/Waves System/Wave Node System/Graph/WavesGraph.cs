using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "Wave_Graph", menuName = "Wave System/WaveGraph")]
public class WavesGraph : NodeGraph
{
    public WaveNode initialWave;
    public WaveNode currentNode;

    public void Start()
    {
        Debug.Log("Puta");
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
            //Se puede hacer que se conecte al sistema de rondas por un evento y este evento devuelva el nodo anterior y el actual
            //y con la info del anterior agarre el resting time
        }
    }
}
