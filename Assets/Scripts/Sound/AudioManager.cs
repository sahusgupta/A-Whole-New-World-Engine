using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string savePath;
    void Start()
    {
        // Initialize sliders with current values
        if (masterSlider != null) masterSlider.value = source.volume;
        if (musicSlider != null && source != null) musicSlider.value = source.volume;
        if (sfxSlider != null && source != null) sfxSlider.value = source.volume;

        // Add listeners
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        source.volume = volume;
        Debug.Log("Master volume set to: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (source != null)
        {
            source.volume = volume * masterSlider.value;
            Debug.Log("Music volume set to: " + volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (source != null)
        {
            source.volume = volume;
            Debug.Log("SFX volume set to: " + volume);
        }
    }

    public void OnDestroy()
    {
        List<string> attrs = new List<string>
        {
            $"SFXVolume:{source.volume}",
            $"MusicVol: {source.volume}",
            $"MasterVol: {masterSlider.value}"
        };
    }
}