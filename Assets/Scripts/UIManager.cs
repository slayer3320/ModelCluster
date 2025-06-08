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
    // 引用层级管理脚本
    // public HierarchyManager HierarchyManager;

    // 滑动条
    public Slider rotateSlider;
    public Slider zoomSlider;
    public Slider cameraSlider;

    //子窗口
    public GameObject HelpPanel; //帮助(panel)
    public GameObject HierarchyPanel; // 模型层级结构子窗(panel)
    public GameObject TextView; // 文本查看(panel)

    //按钮面板
    public GameObject FileOperation; // 文件操作(button)
    public GameObject ViewControl; // 视角控制(button)
    public GameObject MergeHierarchy; //层级合并(button)

    //提示
    public GameObject FreeCameraHelp; // 自由视角提示(text)

    //预制体和精灵
    public GameObject itemPrefab;
    public Sprite expandSprite;
    public Sprite collapseSprite;


    //颜色
    public Color highlightColor = Color.red;//模型部件选中高光：红色

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
        HideFreeCameraHelp();

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
        HideMergeHierarchy();

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
        HideMergeHierarchy();
    }
    public void HideFileOperation()
    {
        FileOperation.SetActive(false);
        HideTextView();
    }
    //层级结构窗口
    public void ShowMergeHierarchy()
    {
        MergeHierarchy.SetActive(true);
        ShowHierarchyPanel();
        HideViewControl();
        HideFileOperation();
    }
    public void HideMergeHierarchy()
    {
        MergeHierarchy.SetActive(false);
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
    //层级子窗口：模型层级选择
    public void ShowHierarchyPanel()
    {
        HierarchyPanel.SetActive(true);

    }
    public void HideHierarchyPanel()
    {
        HierarchyPanel.SetActive(false);
    }

    //自由视角提示
    public void ShowFreeCameraHelp()
    {
        FreeCameraHelp.SetActive(true);
        HideHelpPanel();
    }
    public void HideFreeCameraHelp()
    {
        FreeCameraHelp.SetActive(false);
    }



}