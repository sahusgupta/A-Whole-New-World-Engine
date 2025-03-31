using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class IOManager : MonoBehaviour
{
    [SerializeField] private GameObject gameMaster;
    [SerializeField] private AudioSource musicAudioSource; 
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider brightnessSlider;

    private Button[] returns;
    private string settingsFilePath;

    private Dictionary<string, float> gameSettings = new Dictionary<string, float>()
    {
        { "MasterVolume", 0.75f },
        { "MusicVolume", 0.75f },
        { "SFXVolume", 0.75f },
        { "Brightness", 1.0f }
    };

    void Awake()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, "gameSettings.txt");
        LoadSettings();
        ApplySettings();
    }

    void Start()
    {
        returns = GameObject.FindObjectsOfType<Button>(true);
        foreach (Button b in returns)
        {
            if (b.gameObject.name == "Return")
            {
                b.onClick.AddListener(OnReturnButtonPressed);
            }
        }

        SaveSettings();
    }

    public void OnReturnButtonPressed()
    {
        UpdateSettingsFromUI();
        SaveSettings();
        ApplySettings();

        Debug.Log("SAVING DATA");
    }

    private void UpdateSettingsFromUI()
    {
        if (masterVolumeSlider != null)
            gameSettings["MasterVolume"] = masterVolumeSlider.value;

        if (musicVolumeSlider != null)
            gameSettings["MusicVolume"] = musicVolumeSlider.value;

        if (sfxVolumeSlider != null)
            gameSettings["SFXVolume"] = sfxVolumeSlider.value;

        if (brightnessSlider != null)
            gameSettings["Brightness"] = brightnessSlider.value;
    }

    // Method to save settings to file
    private void SaveSettings()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(settingsFilePath))
            {
                foreach (var setting in gameSettings)
                {
                    writer.WriteLine($"{setting.Key}={setting.Value}");
                }
            }
            Debug.Log("Settings saved to: " + settingsFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving settings: " + e.Message);
        }
    }

    // Method to load settings from file
    private void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            try
            {
                using (StreamReader reader = new StreamReader(settingsFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            float value;
                            if (float.TryParse(parts[1], out value) && gameSettings.ContainsKey(key))
                            {
                                gameSettings[key] = value;
                            }
                        }
                    }
                }
                Debug.Log("Settings loaded from: " + settingsFilePath);

                // Update UI sliders with loaded values
                UpdateUIFromSettings();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading settings: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Settings file not found. Using default settings.");
        }
    }

    // Method to update UI from loaded settings
    private void UpdateUIFromSettings()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = gameSettings["MasterVolume"];

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = gameSettings["MusicVolume"];

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = gameSettings["SFXVolume"];

        if (brightnessSlider != null)
            brightnessSlider.value = gameSettings["Brightness"];
    }

    // Method to apply settings to game
    private void ApplySettings()
    {
        // Apply master volume
        float masterVolume = gameSettings["MasterVolume"];
        float musicVolume = gameSettings["MusicVolume"];
        float sfxVolume = gameSettings["SFXVolume"];

        // Set volumes directly to audio sources

        if (musicAudioSource != null)
            musicAudioSource.volume = musicVolume * masterVolume;

        if (sfxAudioSource != null)
            sfxAudioSource.volume = sfxVolume * masterVolume;

        // Apply brightness
        float brightness = gameSettings["Brightness"];
        Screen.brightness = brightness;
    }
}