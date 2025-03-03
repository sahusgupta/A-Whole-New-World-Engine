using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    [System.Serializable]
    public class CameraPosition
    {
        public string name;
        public Transform transform;
        
    }

    public List<CameraPosition> cameraPositions = new List<CameraPosition>();
    private Dictionary<string, Button> cameraButtons = new Dictionary<string, Button>();
    void Start()
    {

        SetupCameraButtons();
    }

    void SetupCameraButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            MenuButtonIdentifier identifier = button.GetComponent<MenuButtonIdentifier>();

            if (identifier != null && !string.IsNullOrEmpty(identifier.posName))
            {
                cameraButtons[identifier.posName] = button;

                string posName = identifier.posName;
                button.onClick.AddListener(() => MoveCameraToPosition(posName));
            }
        }
    }

    public void MoveCameraToPosition(string positionName)
    {
        // Find the position in our list
        CameraPosition targetPos = cameraPositions.Find(pos => pos.name == positionName);

        if (targetPos != null)
        {
            transform.position = targetPos.transform.position;
            transform.rotation = targetPos.transform.rotation;
        }
        else
        {
            Debug.LogWarning($"Camera position '{positionName}' not found!");
        }
    }
}

