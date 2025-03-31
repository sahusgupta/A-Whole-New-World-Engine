using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source1;
    [SerializeField] private AudioSource source2;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string savePath;
    void Start()
    {


        // Add listeners
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        source1.volume = volume * musicSlider.value;
        source2.volume = volume * sfxSlider.value;
        Debug.Log("Master volume set to: " + volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (source1 != null)
        {
            source1.volume = volume * masterSlider.value;
            Debug.Log("Music volume set to: " + volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (source2 != null)
        {
            source2.volume = volume * masterSlider.value;
            Debug.Log("SFX volume set to: " + volume);
        }
    }

    public void OnDestroy()
    {
        List<string> attrs = new List<string>
        {
            $"SFXVolume:{source2.volume}",
            $"MusicVol: {source1.volume}",
            $"MasterVol: {masterSlider.value}"
        };
    }
}