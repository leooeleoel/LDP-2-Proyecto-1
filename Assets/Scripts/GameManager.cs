using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Difficulty { Facil, Dificil }

    public static GameManager Instance { get; private set; }

    public Difficulty SelectedDifficulty { get; private set; }
    public int FragmentsCollected { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        SelectedDifficulty = difficulty;
        FragmentsCollected = 0;
    }

    public void AddFragment(int amount)
    {
        FragmentsCollected += amount;
    }

    public string GetSceneName()
    {
        return SelectedDifficulty == Difficulty.Dificil ? "GameH" : "GameF";
    }
}