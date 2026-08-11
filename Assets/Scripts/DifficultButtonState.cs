using UnityEngine;
using TMPro;

public class DifficultButtonState : MonoBehaviour
{
    [SerializeField] private GameManager.Difficulty difficulty;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private bool atenuarSiCompletado = true;
    [SerializeField] private Color colorCompletado = new Color(0.6f, 0.55f, 0.45f, 1f);

    private void Start()
    {
        if (label == null) return;

        bool completado = PlayerPrefs.GetInt("Level_" + difficulty.ToString(), 0) == 1;

        if (!completado) return;

        label.fontStyle |= FontStyles.Strikethrough;

        if (atenuarSiCompletado)
            label.color = colorCompletado;
    }
}