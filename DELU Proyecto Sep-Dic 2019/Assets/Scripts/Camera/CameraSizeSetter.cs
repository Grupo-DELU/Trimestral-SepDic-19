using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraSizeSetter : MonoBehaviour
{
    public float ySize = 5;
    public float xSize = 5;
    public bool forceY = true;
    public bool forceX = false;
    public bool copiado = false;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (!cam.orthographic)
        {
            cam.orthographic = true;
        }
    }

    private void Update()
    {
        if (forceX && !forceY)
        {
            ForceXSize(xSize);
        }
        else if (forceY && !forceX)
        {
            ForceYSize(ySize);
        }
        else if (!forceX && !forceY)
        {
            Debug.Log("puto");
            ForceRatio(xSize, ySize);
        }
    }

    private void ForceXSize(float size)
    {
        cam.orthographicSize = size * (float)Screen.height / (float)Screen.width * 0.5f;
    }

    private void ForceYSize(float size)
    {
        // Camera height formula is height = size * 2
        cam.orthographicSize = size / 2;
    }

    private void ForceRatio(float x, float y)
    {
        float screenRatio = cam.aspect; // Widht / Height
        float target = x / y;
        Debug.Log("Target:" + target);
        Debug.Log("Screenratio: " + screenRatio);
        // El aspect Ratio actual es mas ancho que el buscado
        if (screenRatio >= target)
        {
            Debug.Log("1");
            cam.orthographicSize = y / 2;
        }
        // El aspect ratio actual es mas chico que el buscado
        else
        {
            Debug.Log("2");
            float differenceInSize = (target / screenRatio) * .5f;
            cam.orthographicSize = (y / 2) * differenceInSize;
        }
        // cam.orthographicSize /= .5f;
    }

}
