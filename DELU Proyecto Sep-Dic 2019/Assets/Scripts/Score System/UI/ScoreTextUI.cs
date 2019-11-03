using UnityEngine;
using UnityEngine.UI;
public class ScoreTextUI : MonoBehaviour
{
    /// <summary>
    /// Texto donde se muestra el score
    /// </summary>
    public Text scoreText;

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("No hay texto de Score!", this);
            return;
        }
        ScoreSystem.Manager.onScoreChange.AddListener(UpdateScoreText);
        //Empieza con 0 de score naive
        UpdateScoreText(0);
    }

    /// <summary>
    /// Updatea el texto que muestra el score
    /// </summary>
    /// <param name="newScore">Nuevo score a mostrar</param>
    private void UpdateScoreText(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
