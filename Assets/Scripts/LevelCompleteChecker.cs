using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteChecker : MonoBehaviour
{
    [SerializeField] private MapaCounter mapaCounter;

    private bool finished;

    private void Update()
    {
        if (finished || mapaCounter == null) return;
        if (mapaCounter.objetosRecogidos < mapaCounter.objetosTotales) return;

        finished = true;
        CompleteLevel();
    }

    private void CompleteLevel()
    {
        string difficulty = GameManager.Instance != null
            ? GameManager.Instance.SelectedDifficulty.ToString()
            : "Facil";

        PlayerPrefs.SetInt("Level_" + difficulty, 1);

        bool allComplete =
            PlayerPrefs.GetInt("Level_Facil", 0) == 1 &&
            PlayerPrefs.GetInt("Level_Dificil", 0) == 1;

        PlayerPrefs.SetInt("LastResult", allComplete ? 2 : 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameEnd");
    }
}