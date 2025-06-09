using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public UIManager UIManager;
    public GameObject ModelRoot;

    [Header("Camera Settings:FreeViewMode")]
    public float moveSpeed = 100;            // 摄像机移动速度（WASD）
    public float mouseSensitivity = 2f;       // 鼠标灵敏度
    public float minVerticalAngle = -80f;     // 最小垂直角度
    public float maxVerticalAngle = 80f;      // 最大垂直角度

    [Header("Camera Settings:General")]
    public float rotationSpeed = 5f; //鼠标旋转摄像机速度
    public float minDistance = 20f;  // 摄像机最小距离
    public float maxDistance = 800f; // 摄像机最大距离
    public float zoomSpeed = 10f; //缩放速度

    private float currentVerticalAngle = 0f;     // 当前绕X轴的旋转（上下）
    private float currentHorizontalAngle = 0f;   // 当前绕Y轴的旋转（左右）

    private bool isMouseLocked = false;          // 鼠标是否锁定
    private bool isFreeViewMode = false;

    private float currentX = 0f;     // 当前水平旋转角度（绕Y轴）
    private float currentY = 30f;    // 当前垂直旋转角度（绕X轴）
    private float distance = 300f;  //当前摄像机与目标的距离
    public Vector3 lookAtPoint = Vector3.zero;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentVerticalAngle = angles.x;
        currentHorizontalAngle = angles.y;

        LockCursor(false); // 启动时不锁定鼠标
        lookAtPoint = CalculateModelCenter(ModelRoot);


        // 设置纯色背景
        Camera.main.backgroundColor = new Color(0.7f, 0.7f, 0.7f); // 浅灰色
        Camera.main.clearFlags = CameraClearFlags.SolidColor;


    }
    //计算一个 GameObject（及其子对象）的包围盒中心
    Vector3 CalculateModelCenter(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

        // 如果没有渲染器，返回物体的位置作为备选
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in the hierarchy.");
            return root.transform.position;
        }

        // 初始化包围盒为第一个渲染器的范围
        Bounds bounds = renderers[0].bounds;

        // 扩展包围盒，包含所有渲染器
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // 返回包围盒中心
        return bounds.center;
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // 鼠标在 UI 上，不处理摄像机控制
        }
        if (isFreeViewMode && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitFreeViewMode();
        }

        if (isFreeViewMode)
        {
            lookAtPoint = CalculateModelCenter(ModelRoot);

            HandleRotation();
        }
        else
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f)
            {
                // 缩放距离，并限制范围
                distance -= scroll * zoomSpeed;
            }
            // 计算摄像机在当前旋转角度下的位置
            Vector3 dir = new Vector3(0, 0, -distance); // 沿-z方向向后拉
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0); // 构造旋转四元数
            transform.position = rotation * dir + lookAtPoint; // 旋转后的位置再加注视点偏移

            // 让摄像机始终朝向目标中心
            transform.LookAt(lookAtPoint);
        }
        // 始终允许移动
        HandleMovement();
    }
    public void EnterFreeViewMode()
    {
        isFreeViewMode = true;
        LockCursor(true);
            // 重新定位摄像机到模型附近
        lookAtPoint = CalculateModelCenter(ModelRoot); // 更新中心
        Vector3 dir = new Vector3(0, 0, -distance); // 向后拉
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = rotation * dir + lookAtPoint;

        // 设置旋转角度保持一致
        transform.LookAt(lookAtPoint);
        currentVerticalAngle = transform.eulerAngles.x;
        currentHorizontalAngle = transform.eulerAngles.y;
        }

    public void ExitFreeViewMode()
    {
        isFreeViewMode = false;
        LockCursor(false);
        UIManager.HideFreeCameraHelp();
    }
    void HandleRotation()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentHorizontalAngle += mouseX;
        currentVerticalAngle -= mouseY;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

        transform.eulerAngles = new Vector3(currentVerticalAngle, currentHorizontalAngle, 0f);
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            // 获取鼠标水平/垂直移动距离，并按速度缩放
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // 限制仰角角度范围，防止摄像机翻转
            currentY = Mathf.Clamp(currentY, -85f, 85f);
        }
        else
        {
            if (isFreeViewMode)
            {
                float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
                float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
                Vector3 direction = transform.right * moveX + transform.forward * moveZ;
                transform.position += direction;
            }
        }



    }

    // 鼠标锁定/解锁控制
    void LockCursor(bool locked)
    {
        isMouseLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    // 给 UIManager 调用：设置移动速度
    public void SetCameraSpeed(float value)
    {
        moveSpeed = value;
        Debug.Log("Camera moveSpeed set to: " + moveSpeed);
    }
}
