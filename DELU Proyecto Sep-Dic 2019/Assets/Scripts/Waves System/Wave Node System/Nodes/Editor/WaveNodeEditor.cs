using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;
[CustomNodeEditor(typeof(WaveNode))]
public class WaveNodeEditor : NodeEditor
{
    public override void OnHeaderGUI()
    {
        GUI.color = Color.white;
        WaveNode node = target as WaveNode;
        WavesGraph graph = node.graph as WavesGraph;
        string title = target.name;
        if (node == graph.initialWave && node.firstWave == true)
        {
            //Hay un bug y hay un nodo que si lo haces el primero, los demas tambien se ponen rojos
            //pero es solo un bug visual lol
            title = "Initial wave";
            GUI.contentColor = Color.red;
        }
        GUILayout.Label(title, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        GUI.color = Color.white;

    }

    public override void OnBodyGUI()
    {
        base.OnBodyGUI();
        WaveNode node = target as WaveNode;
        WavesGraph graph = node.graph as WavesGraph;
        if (node != graph.initialWave)
        {
            if (GUILayout.Button("Set as first wave"))
            {
                graph.initialWave.firstWave = false;
                graph.initialWave = node;
                graph.currentNode = node;
                node.firstWave = true;
            }
        }
    }
}
