using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundEarth : MonoBehaviour
{
    public float rotationSpeed = 3f;
    public bool invertX = false;

    bool isDragging;
    Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            float yRotation = (currentMousePosition.x - lastMousePosition.x) * rotationSpeed * Time.deltaTime;
            if (invertX) yRotation = -yRotation;
            transform.Rotate(Vector3.up, yRotation, Space.World);
            lastMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

}
