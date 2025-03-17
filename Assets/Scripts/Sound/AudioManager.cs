using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // Initialize sliders with current values
        if (masterSlider != null) masterSlider.value = AudioListener.volume;
        if (musicSlider != null && musicSource != null) musicSlider.value = musicSource.volume;
        if (sfxSlider != null && sfxSource != null) sfxSlider.value = sfxSource.volume;

        // Add listeners
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        Debug.Log("Master volume set to: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume * AudioListener.volume;
            Debug.Log("Music volume set to: " + volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            Debug.Log("SFX volume set to: " + volume);
        }
    }
}