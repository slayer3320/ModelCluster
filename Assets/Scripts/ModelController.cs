using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    public Transform modelRoot;
    public float rotateSpeed;
    public float zoomSpeed;

    public void RotateLeft()
    {
        modelRoot.Rotate(Vector3.up, rotateSpeed);
    }

    public void RotateRight()
    {
        modelRoot.Rotate(Vector3.up, -rotateSpeed);
    }
    public void RotateUp()
    {
        modelRoot.Rotate(Vector3.right, -rotateSpeed);
    }
    public void RotateDown()
    {
        modelRoot.Rotate(Vector3.right, rotateSpeed);
    }

    public void ZoomIn()
    {
        modelRoot.localScale *= 1f + zoomSpeed;

    }

    public void ZoomOut()
    {
        modelRoot.localScale *= 1f - zoomSpeed;

    }
    public void SetRotateSpeed(float value)
    {
        rotateSpeed = value;


    }
    public void SetZoomSpeed(float value)
    {
        zoomSpeed = value;
    }

}
