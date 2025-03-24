using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderDebug : MonoBehaviour
{
    void Start()
    {
        Slider slider = GetComponent<Slider>();
        if (slider != null)
        {
            // Extensive logging and debugging
            Debug.Log($"Slider {name} initial setup:");
            Debug.Log($"Interactable: {slider.interactable}");
            Debug.Log($"Initial value: {slider.value}");

            // Add multiple listeners for comprehensive tracking
            slider.onValueChanged.AddListener(OnSliderValueChanged);

            // Add event trigger for more detailed input tracking
            EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

            // Pointer down event
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => {
                Debug.Log($"Pointer Down on {name}");
            });
            trigger.triggers.Add(pointerDown);

            // Pointer up event
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => {
                Debug.Log($"Pointer Up on {name}");
            });
            trigger.triggers.Add(pointerUp);
        }
    }

    void OnSliderValueChanged(float value)
    {
        Debug.Log($"Slider {name} value changed to: {value}");
    }

    void Update()
    {
        // Additional input debugging
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button down detected");
        }
    }
}