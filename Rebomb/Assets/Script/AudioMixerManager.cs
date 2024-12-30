using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetVolumeEffects(float volume)
    {
        Debug.Log("SetVolumeEffects to " + volume);
        audioMixer.SetFloat("VolumeSoundEffects", Mathf.Log10(volume) * 20);

    }

    public void SetVolumeMusic(float volume)
    {
        Debug.Log("SetVolumeMusic to " + volume);
        audioMixer.SetFloat("VolumeBackgroundMusic", Mathf.Log10(volume) * 20);

    }
}
