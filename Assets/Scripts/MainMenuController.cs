using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayFacil() => StartGame(GameManager.Difficulty.Facil);
    public void PlayMedio() => StartGame(GameManager.Difficulty.Medio);
    public void PlayDificil() => StartGame(GameManager.Difficulty.Dificil);

    private void StartGame(GameManager.Difficulty difficulty)
    {
        GameManager.Instance.SetDifficulty(difficulty);
        SceneManager.LoadScene("Game");
    }
}