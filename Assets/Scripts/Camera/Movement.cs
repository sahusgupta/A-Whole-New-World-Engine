using UnityEngine;

public class Movement : MonoBehaviour
{
    public float rotationSpeed = 3f;
    public bool invertX = false;
    public bool invertY = false;
    public float maxUpAngle = 80f;
    public float maxDownAngle = 80f;

    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentYRotation = angles.y;
        currentXRotation = angles.x;
        if (currentXRotation > 180)
            currentXRotation -= 360;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;
            RotateCamera(mouseDelta);
            lastMousePosition = currentMousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void RotateCamera(Vector3 mouseDelta)
    {
        float xRotation = mouseDelta.y * rotationSpeed * Time.deltaTime;
        float yRotation = mouseDelta.x * rotationSpeed * Time.deltaTime;

        if (invertX) yRotation = -yRotation;
        if (invertY) xRotation = -xRotation;

        currentXRotation -= xRotation;
        currentYRotation += yRotation;

        currentXRotation = Mathf.Clamp(currentXRotation, -maxDownAngle, maxUpAngle);

        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
    }
}