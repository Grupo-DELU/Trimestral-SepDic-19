using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    /// <summary>
    /// Iconos de vida
    /// </summary>
    public List<Image> hpIcons;

    void Start()
    {
        if (PlayerFinder.Player.TryGetComponent(out HealthManager hm))
        {
            hm.onLifeChange.AddListener(UpdateLives);
            if (hpIcons.Count != hm.MaxHP)
            {
                Debug.LogError("No hay suficientes iconos de vida");
            }
        }
        else
        {
            Debug.LogError("Player no tiene health manager");
        }

        if (hpIcons.Count <= 0)
        {
            Debug.LogError("No hay iconos de vida en la lista");
        }

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
            for (int i = oldHP - 1; i < newHP; i++)
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
