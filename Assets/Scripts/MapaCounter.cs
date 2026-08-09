using UnityEngine;
using TMPro;

public class MapaCounter : MonoBehaviour
{
    public TextMeshProUGUI textoContador;

    public int objetosRecogidos = 0;
    public int objetosTotales = 20;

    [Header("Audio")]
    [SerializeField] private AudioClip recogerSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

    }

    void Start()
    {
        ActualizarContador();
    }

    public void RecogerObjeto()
    {
        objetosRecogidos++;

        if (objetosRecogidos > objetosTotales)
        {
            objetosRecogidos = objetosTotales;
        }

        ActualizarContador();

        // Sonido al recoger objeto
        if (recogerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(recogerSound);
            Debug.Log("REPRODUCIENDO SONIDO");

        }
    }

    void ActualizarContador()
    {
        textoContador.text = objetosRecogidos + "/" + objetosTotales;
    }
}