using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 引用模型控制脚本
    public ModelController ModelController;
    // 引用相机控制脚本
    public CameraController CameraController;
    // 滑动条
    public Slider rotateSlider;
    public Slider zoomSlider;
    public Slider cameraSlider;

    //子窗口
    public GameObject HelpPanel;
    public GameObject TextView;
    public GameObject FileOperation;
    public GameObject ViewControl;

    // 模型对象
    public Transform modelRoot;

    // 旋转和缩放速度文本信息
    public TMP_Text rotateSpeedText;
    public TMP_Text zoomSpeedText;
    public TMP_Text cameraSpeedText;

    void Update()
    {
        // 每帧显示当前速度值
        rotateSpeedText.text = "旋转角度:" + ModelController.rotateSpeed.ToString("F2");
        zoomSpeedText.text = "缩放速度:" + ModelController.zoomSpeed.ToString("F2");
        cameraSpeedText.text = "相机移速:" + CameraController.moveSpeed.ToString("F2");
    }

    // 触发显示和隐藏子窗口及组件的方法

    //帮助窗口
    public void ShowHelpPanel()
    {
        HelpPanel.SetActive(true);
    }
    public void HideHelpPanel()
    {
        HelpPanel.SetActive(false);
    }
    //视图窗口
    public void ShowViewControl()
    {
        ViewControl.SetActive(true);
        HideFileOperation();

    }
    public void HideViewControl()
    {
        ViewControl.SetActive(false);
    }
    //文件操作窗口
    public void ShowFileOperation()
    {
        FileOperation.SetActive(true);
        HideViewControl();
    }

    public void HideFileOperation()
    {
        FileOperation.SetActive(false);
        HideTextView();
    }
    //文件子窗口：obj文本
    public void ShowTextView()
    {
        TextView.SetActive(true);
    }

    public void HideTextView()
    {
        TextView.SetActive(false);
    }

}