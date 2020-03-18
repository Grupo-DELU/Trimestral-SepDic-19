using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UNUSED
//public class HPIconBar
//{
//    public GameObject panelObject;
//    public List<Image> hpIcons;
//
//    public HPIconBar(GameObject panel)
//    {
//        this.panelObject = panel;
//        for (int i = 0; i < panel.transform.childCount; i++)
//        {
//            Image icon = panel.transform.GetChild(i).GetComponent<Image>();
//            hpIcons.Add(icon);
//            icon.gameObject.SetActive(false);
//        }
//    }
//}

/// <summary>
/// IMPORTANTE. POR LOS MOMENTOS FUNCIONA PARA 4 BARRAS DE 5 ICONOS (20 DE VIDA)
/// </summary>
public class HPUI : MonoBehaviour
{
    /// <summary>
    /// Iconos de vida
    /// </summary>
    public GameObject bigObject;

    public List<Image> hpIcons = new List<Image>();

    void Awake()
    {
        for (int i = 0; i < bigObject.transform.childCount; i++)
        {
            Transform barra = bigObject.transform.GetChild(i);
            for (int j = 0; j < barra.childCount; j++)
            {
                hpIcons.Add(barra.GetChild(j).GetComponent<Image>());
                barra.GetChild(j).gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        try
        {
            PlayerFinder.Player.GetComponent<HealthManager>().onLifeChange.AddListener(UpdateLives);
        }
        catch (System.NullReferenceException)
        {
            Debug.LogWarning("Jugador o HP Manager de el son null para los iconos de vida :(");
            throw;
        }
        UpdateLives(0, PlayerFinder.Player.GetComponent<HealthManager>().MaxHP);
    }

    /// <summary>
    /// Updatea la UI de vida
    /// </summary>
    /// <param name="oldHP">Vida vieja</param>
    /// <param name="newHP">Vida nueva</param>
    private void UpdateLives(int oldHP, int newHP)
    {
        // Desactivar las imagenes y eso
        if (newHP > oldHP)
        {
            // aja y si es 0... xD
            for (int i = oldHP; i < newHP; i++)
            {
                hpIcons[i].gameObject.SetActive(true);
            }
        }
        else if (newHP < oldHP)
        {
            for (int i = oldHP - 1; i > newHP - 1; i--)
            {
                hpIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
