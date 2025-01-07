using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBackgroundMusic(AudioClip musicClip, float fadeDuration = 0.5f)
    {
        StartCoroutine(FadeInMusic(musicClip, fadeDuration));
    }


    private IEnumerator FadeInMusic(AudioClip musicClip, float duration)
    {
        backgroundMusicSource.clip = musicClip;
        backgroundMusicSource.Play();
        backgroundMusicSource.volume = 0f;

        float targetVolume = 1f;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            backgroundMusicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }
        backgroundMusicSource.volume = targetVolume;
    }

    public void PlaySoundEffect(AudioClip effectClip)
    {
        soundEffectSource.PlayOneShot(effectClip);
    }

    public void SetVolume(string parameterName, float volume)
    {
        audioMixer.SetFloat(parameterName, Mathf.Log10(volume) * 20); // Convert linear to decibel
    }

    public void StopMusic()
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }
}
