using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[System.Serializable]
public struct Nothing { }

public class WaveNode : Node
{
    [Output] public Nothing enemies;
    [Output] public Nothing nextWave;
    [Input] public Nothing previusWave;

    public string name;

    [HideInInspector] public bool firstWave;
    [HideInInspector] public bool lastWave;
    public float roundRestingTime;

    //HACER BOTON PARA QUE SETEE LA VAINA COMO FIRSTWAVE Y ESO
    //Y EL BOTON CHEQUEA EL GRAFO Y CANCELA LOS DEMAS FIRSTWAVE
}
