using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicaDeEstaEscena;
    [Range(0f, 1f)][SerializeField] private float volumen = 1f;

    private void Start()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlayMusic(musicaDeEstaEscena, volumen);
    }
}