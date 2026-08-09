using UnityEngine;
using TMPro;

public class LivesCounter : MonoBehaviour
{
    public TextMeshProUGUI textoVidas;
    public PlayerHealth playerHealth;

    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();
        }

        ActualizarVidas();
    }

    void Update()
    {
        ActualizarVidas();
    }

    void ActualizarVidas()
    {
        if (playerHealth == null || textoVidas == null)
            return;

        textoVidas.text = playerHealth.CurrentLives + "";
    }
}