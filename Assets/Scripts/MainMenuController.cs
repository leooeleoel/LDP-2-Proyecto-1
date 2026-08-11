using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayFacil() => StartGame(GameManager.Difficulty.Facil, "GameF");
    public void PlayDificil() => StartGame(GameManager.Difficulty.Dificil, "GameH");

    private void StartGame(GameManager.Difficulty difficulty, string sceneName)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetDifficulty(difficulty);

        SceneManager.LoadScene(sceneName);
    }
}