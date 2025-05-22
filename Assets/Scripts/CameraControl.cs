using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotationSpeed = 5f;
    public float distance = 10f;
    public float minDistance = 2f;
    public float maxDistance = 20f;
    public float zoomSpeed = 5f;

    private float currentX = 0f;
    private float currentY = 30f;

    void Start()
    {
        UpdateCameraPosition();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, 5f, 85f);
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = rotation * dir;
        
        transform.LookAt(Vector3.zero);
    }
}
