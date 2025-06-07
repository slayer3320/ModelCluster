using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PressDetection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityAction OnPressDown;
    public UnityAction OnPressUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPressDown?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPressUp?.Invoke();
    }
}

