using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes de audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX globales")]
    [SerializeField] private AudioClip clickSound;

    [Header("Volumen por defecto")]
    [Range(0f, 1f)][SerializeField] private float defaultMusicVolume = 0.5f;
    [Range(0f, 1f)][SerializeField] private float defaultSfxVolume = 1f;

    [Header("Transiciones")]
    [SerializeField] private float fadeDuration = 0.5f;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";

    private Coroutine fadeRoutine;
    private float currentTrackVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSfxVolume);

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayMusic(AudioClip clip, float volumeScale = 1f, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        currentTrackVolume = Mathf.Clamp01(volumeScale);
        musicSource.loop = loop;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeToClip(clip));
    }

    public void StopMusic()
    {
        if (musicSource == null) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeToClip(null));
    }

    private IEnumerator FadeToClip(AudioClip clip)
    {
        float targetVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume) * currentTrackVolume;
        float startVolume = musicSource.volume;

        if (musicSource.isPlaying && fadeDuration > 0f)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                yield return null;
            }
        }

        musicSource.Stop();

        if (clip == null)
        {
            musicSource.volume = targetVolume;
            fadeRoutine = null;
            yield break;
        }

        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        if (fadeDuration > 0f)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
                yield return null;
            }
        }

        musicSource.volume = targetVolume;
        fadeRoutine = null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayClick()
    {
        PlaySFX(clickSound);
    }

    public float MusicVolume => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
    public float SfxVolume => PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSfxVolume);

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        PlayerPrefs.Save();

        if (musicSource != null && fadeRoutine == null)
            musicSource.volume = value * currentTrackVolume;
    }

    public void SetSfxVolume(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();

        if (sfxSource != null)
            sfxSource.volume = value;
    }
}