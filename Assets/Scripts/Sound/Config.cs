using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    [SerializeField] private GameObject manager;
    private AudioSource audioS;
    private Slider volumeSlider;

    void Start()
    {
        if (manager == null)
            manager = GameObject.Find("Manager");

        audioS = manager.GetComponent<AudioSource>();
        volumeSlider = GetComponent<Slider>();

        // Force-enable interaction
        volumeSlider.interactable = true;

        // Print debug information
        Debug.Log("Slider interactable: " + volumeSlider.interactable);
        Debug.Log("Initial audio volume: " + audioS.volume);
    }

    // This function should be connected in the Inspector
    public void SetVolume()
    {
        if (audioS != null && volumeSlider != null)
        {
            float newVolume = volumeSlider.value;
            audioS.volume = newVolume;
            Debug.Log("SetVolume called. New volume: " + newVolume);
        }
        else
        {
            Debug.LogError("Audio source or slider reference is null");
        }
    }

    void Update()
    {
        // Debug testing with keyboard controls
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            volumeSlider.value += 0.1f;
            Debug.Log("Forced value up: " + volumeSlider.value);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            volumeSlider.value -= 0.1f;
            Debug.Log("Forced value down: " + volumeSlider.value);
        }
    }
}