using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndController : MonoBehaviour
{
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelVictoriaFinal;

    [Header("Musica por estado")]
    [SerializeField] private AudioClip musicaDerrota;
    [SerializeField] private AudioClip musicaVictoria;
    [SerializeField] private AudioClip musicaVictoriaFinal;
    [Range(0f, 1f)][SerializeField] private float volumenMusica = 1f;

    private void Start()
    {
        int result = PlayerPrefs.GetInt("LastResult", 0);

        if (panelDerrota != null) panelDerrota.SetActive(result == 0);
        if (panelVictoria != null) panelVictoria.SetActive(result == 1);
        if (panelVictoriaFinal != null) panelVictoriaFinal.SetActive(result == 2);

        ReproducirMusica(result);
    }

    private void ReproducirMusica(int result)
    {
        if (AudioManager.Instance == null) return;

        AudioClip clip;

        switch (result)
        {
            case 1:
                clip = musicaVictoria;
                break;
            case 2:
                clip = musicaVictoriaFinal;
                break;
            default:
                clip = musicaDerrota;
                break;
        }

        AudioManager.Instance.PlayMusic(clip, volumenMusica, false);
    }

    public void Reintentar()
    {
        string escena = GameManager.Instance != null
            ? GameManager.Instance.GetSceneName()
            : "GameF";

        SceneManager.LoadScene(escena);
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReiniciarProgreso()
    {
        PlayerPrefs.DeleteKey("Level_Facil");
        PlayerPrefs.DeleteKey("Level_Dificil");
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}