using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 300f;            // 摄像机移动速度（WASD）
    public float mouseSensitivity = 2f;       // 鼠标灵敏度
    public float minVerticalAngle = -80f;     // 最小垂直角度
    public float maxVerticalAngle = 80f;      // 最大垂直角度

    private float currentVerticalAngle = 0f;     // 当前绕X轴的旋转（上下）
    private float currentHorizontalAngle = 0f;   // 当前绕Y轴的旋转（左右）

    private bool isMouseLocked = true;          // 鼠标是否锁定

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentVerticalAngle = angles.x;
        currentHorizontalAngle = angles.y;

        LockCursor(true); // 启动时锁定鼠标
    }

    void Update()
    {
        // 按下 Esc 解锁鼠标以操作 UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor(false);
        }

        // 鼠标右键点击重新进入视角控制模式
        if (Input.GetMouseButtonDown(1))
        {
            LockCursor(true);
        }

        // 只有在鼠标锁定状态下才旋转视角
        if (isMouseLocked)
        {
            HandleRotation();
        }

        // 始终允许移动
        HandleMovement();
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
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        Vector3 direction = transform.right * moveX + transform.forward * moveZ;
        transform.position += direction;
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
