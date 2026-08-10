using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndController : MonoBehaviour
{
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelVictoriaFinal;

    private void Start()
    {
        int result = PlayerPrefs.GetInt("LastResult", 0);

        if (panelDerrota != null) panelDerrota.SetActive(result == 0);
        if (panelVictoria != null) panelVictoria.SetActive(result == 1);
        if (panelVictoriaFinal != null) panelVictoriaFinal.SetActive(result == 2);
    }

    public void Reintentar()
    {
        SceneManager.LoadScene("Game");
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReiniciarProgreso()
    {
        PlayerPrefs.DeleteKey("Level_Facil");
        PlayerPrefs.DeleteKey("Level_Medio");
        PlayerPrefs.DeleteKey("Level_Dificil");
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}