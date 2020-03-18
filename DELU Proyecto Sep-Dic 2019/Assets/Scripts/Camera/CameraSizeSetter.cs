using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSizeSetter : MonoBehaviour
{
    public float x = 10;
    public float y = 10;

    public float left = -1;
    public float right = 1;
    public float bottom = -1;
    public float top = 1;
    public float near = -1;
    public float far = 1;
    // Use this for initialization

    public bool enableTest = true;
    public float test = 2;
    public bool enableMatrix = false;

    void Update()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height; // Que tanto X hay por cada Y actualmente
        float targetRatio = x / y; // Que tanto X quiero que haya por Y

        if (screenRatio >= targetRatio) // Si hay mas de lo que quiero, me asegura que alcance si seteo la Y
        {
            Camera.main.orthographicSize = y / 2;
        }
        else // Si no, calculamos la diferencia
        {
            float differenceInSize = targetRatio / screenRatio; // Que tanto de lo que quiero es lo que tengo
            Camera.main.orthographicSize = (y / 2) * differenceInSize;
        }
        // Compensando el viewport rect. Funciona pero no estoy seguro porque la idea es que compense en X, no Y, pero igual lo arregla.
        // Se que PIERDE la MITAD de las X, deberia aumentarle es ESAS X pero creo que depende tammbien del targetRatio! Y si hay mas Y que X?
        // Tiene realmente prioridad Y porque siempre va a cumplir las X pero hmmmm
        if (enableTest) Camera.main.orthographicSize = Camera.main.orthographicSize * test; 
        if (enableMatrix) Camera.main.projectionMatrix = Matrix4x4.Ortho(left, right, bottom, top, near, far);
    }
}