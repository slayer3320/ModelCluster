using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    public static CameraControl instance;
    
    [Header("Camera Settings")]
    public float rotationSpeed = 5f;
    public float panSpeed = 5f;
    public float distance = 10f;
    public float minDistance = 2f;
    public float maxDistance = 20f;
    public float zoomSpeed = 5f;

    private float currentX = 0f;
    private float currentY = 30f;
    public Vector3 lookAtPoint = Vector3.zero;
    public GameObject target;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        lookAtPoint = CalculateModelCenter(target);
    }

    public void ChangeTarget(GameObject target)
    {
        this.target = target;
        CalculateModelCenter(target);
    }
    
    Vector3 CalculateModelCenter(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in the hierarchy.");
            return root.transform.position;
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds.center;
    }
    
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButton(0))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, -85f, 85f);
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = rotation * dir + lookAtPoint;

        transform.LookAt(lookAtPoint);
    }
}
