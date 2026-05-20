using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume;
    [Range(0f, 1f)] public float sfxVolume;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource loopSfxSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [SerializeField] private float fadeInDuration;
    [SerializeField] public float FadeOutDuration;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sfxSlider = GameObject.Find("UICanvas/SettingsMenu/EffectsVolumeSlider")?.GetComponent<Slider>();
        musicSlider = GameObject.Find("UICanvas/SettingsMenu/MusicVolumeSlider")?.GetComponent<Slider>();

        LoadVolumeSettings();

        if (musicSlider != null && sfxSlider != null)
        {
            musicSlider.value = musicVolume;
            sfxSlider.value = sfxVolume;
        }

        SetMusicVolume(musicVolume);
        SetEffectsVolume(sfxVolume);
        SetMasterVolume(masterVolume);
    }


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (loopSfxSource.isPlaying && loopSfxSource.clip == clip) return;

        loopSfxSource.clip = clip;
        loopSfxSource.loop = true;
        loopSfxSource.volume = masterVolume * sfxVolume;
        loopSfxSource.Play();
    }

    public void StopLoopingSFX()
    {
        loopSfxSource.Stop();
    }

    public void PlayMusic(AudioClip clip, bool fadeIn = true)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return; // Don’t restart same track

        StopAllCoroutines();
        musicSource.Stop(); // Stop whatever was playing

        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        if (fadeIn)
            StartCoroutine(FadeInRoutine(fadeInDuration));
        else
            musicSource.volume = masterVolume * musicVolume;
    }


    public void SetMasterVolume(float vol)
    {
        masterVolume = vol;
        musicSource.volume = masterVolume * musicVolume;
        loopSfxSource.volume = masterVolume * sfxVolume;
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        musicSource.volume = masterVolume * musicVolume;
        SaveVolumeSettings();
    }

    public void SetEffectsVolume(float vol)
    {
        sfxVolume = vol;
        loopSfxSource.volume = masterVolume * sfxVolume;
        SaveVolumeSettings();
    }


    public void FadeOutMusic()
    {
        StartCoroutine(FadeOutRoutine(musicSource, FadeOutDuration));
    }

    public void FadeOutSFX()
    {
        StartCoroutine(FadeOutRoutine(sfxSource, FadeOutDuration));
    }

    private IEnumerator FadeInRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            musicSource.volume = t * masterVolume * musicVolume;
            yield return null;
        }

        musicSource.volume = masterVolume * musicVolume;
    }

    private IEnumerator FadeOutRoutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0f)
        {
            source.volume -= startVolume * Time.unscaledDeltaTime / duration;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

}
