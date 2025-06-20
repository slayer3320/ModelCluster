using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFB;
using System.Windows.Forms;

public class UIManager : MonoBehaviour
{
    // 引用相机控制脚本
    public CameraController CameraController;
    // 引用层级管理脚本
    // public HierarchyManager HierarchyManager;

    // 滑动条
    public Slider rotateSlider;
    public Slider zoomSlider;
    public Slider cameraSlider;
    public Slider scaleSlider; //层级UI缩放

    //HUI大小
    public RectTransform HUIcontent;
    public float minScale = 0.5f; // 缩放范围
    public float maxScale = 1.0f;

    //子窗口
    public GameObject HelpPanel; //帮助(panel)
    public GameObject OpenPanel;
    public GameObject HierarchyPanel; // 模型层级结构子窗(panel)
    public GameObject ModifyPanel; // 位置调整子窗(panel)
    public GameObject ViewPanel; // 视图子窗(panel)


    //按钮面板
    public GameObject FileOperation; // 文件操作(button)
    public GameObject ViewControl; // 视角控制(button)
    public GameObject MergeHierarchy; //层级合并(button)

    //提示
    public GameObject FreeCameraHelp; // 自由视角提示(text)

    // 模型对象
    public Transform modelRoot;

    // 旋转和缩放速度文本信息
    // public TMP_Text rotateSpeedText;


    public TMP_Text viewSensityText;
    public TMP_Text cameraSpeedText;
    public TMP_Text rotationSpeedText;
    public TMP_Text RangeofDistanceText;
    public TMP_Text zoomSpeedText;

    public TMP_Text HUIscaleText;
    void Start()
    {

        if (CameraController == null)
            Debug.LogError("CameraController not assigned！");

        scaleSlider.minValue = minScale;
        scaleSlider.maxValue = maxScale;
        scaleSlider.value = 1.0f;

        scaleSlider.onValueChanged.AddListener(UpdateScale);
    }
    void Update()
    {
        // 每帧显示当前速度值
        // rotateSpeedText.text = "旋转角度:" + ModelController.rotateSpeed.ToString("F2");
        // zoomSpeedText.text = "缩放速度:" + ModelController.zoomSpeed.ToString("F2");
        //视图设置
        rotationSpeedText.text = "转动视角速度:" + CameraController.rotationSpeed.ToString("F2");
        RangeofDistanceText.text = "相机距离:" + CameraController.minDistance.ToString("F2") + "~" + CameraController.maxDistance.ToString("F2");
        zoomSpeedText.text = "滚轮缩放速度:" + CameraController.zoomSpeed.ToString("F2");


        cameraSpeedText.text = "相机移速:" + CameraController.moveSpeed.ToString("F2");
        viewSensityText.text = "视角灵敏度:" + CameraController.mouseSensitivity.ToString("F2");



    }
    //更新HUI大小文本
    void UpdateScale(float value)
    {
        if (HUIcontent != null)
        {
            HUIcontent.localScale = Vector3.one * value;
            HUIscaleText.text = "缩放比例:" + value.ToString("F2");
        }
    }
    // HierarchyModule的主UI封装接口
    public void OnClickMerge()
    {
        if (HierarchyUI.Instance != null)
        {
            HierarchyUI.Instance.MergeSelectedObjects();
        }
        else
        {
            Debug.LogError("HierarchyUI.Instance is null! Make sure HierarchyUI is in the scene.");
        }
    }
    public void OnClickSubMerge()
    {
        if (HierarchyUI.Instance != null)
        {
            HierarchyUI.Instance.SubmergeSelectedObjects();
        }
        else
        {
            Debug.LogError("HierarchyUI.Instance is null! Make sure HierarchyUI is in the scene.");
        }
    }
    public void OnClickExport()
    {
        string objPath = StandaloneFileBrowser.SaveFilePanel("Save File", "", modelRoot.name, "obj");

        RuntimeOBJExporter.instance.ExportGameObjectsToOBJ(Main.current.GetAllChildrenObjects(Main.current.gameObject), Main.current.gameObject, objPath);

    }


    // 触发显示和隐藏子窗口及组件的方法
    //Open窗口
    public void ShowOpenPanel()
    {
        OpenPanel.SetActive(true);

    }
    public void HideOpenPanel()
    {
        OpenPanel.SetActive(false);
    }

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
        HideMergeHierarchy();
        HideHierarchyPanel();
        HideViewPanel();
        HideHelpPanel();
        HideOpenPanel();

    }
    public void HideViewControl()
    {
        ViewControl.SetActive(false);
        HideViewPanel();
    }
    //文件操作窗口
    public void ShowFileOperation()
    {
        FileOperation.SetActive(true);
        HideViewControl();
        HideMergeHierarchy();
        HideHierarchyPanel();
        HideViewPanel();
        HideHelpPanel();
        HideOpenPanel();
    }
    public void HideFileOperation()
    {
        FileOperation.SetActive(false);
    }
    //层级结构窗口
    public void ShowMergeHierarchy()
    {
        MergeHierarchy.SetActive(true);
        ShowHierarchyPanel();
        HideViewControl();
        HideFileOperation();
        HideViewPanel();
        if (modelRoot == null || modelRoot.childCount == 0)
        {
            if (OpenPanel != null)
            {
                OpenPanel.SetActive(true);  // 显示提示面板
            }
        }
        else
        {
            if (OpenPanel != null)
            {
                OpenPanel.SetActive(false);  // 有内容就隐藏提示面板
            }
        }
    }
    public void HideMergeHierarchy()
    {
        MergeHierarchy.SetActive(false);

    }

    //层级子窗口：模型层级选择
    public void ShowHierarchyPanel()
    {
        HierarchyPanel.SetActive(true);

    }
    public void HideHierarchyPanel()
    {
        HierarchyPanel.SetActive(false);
        HideModifyPanel();
    }
    //模型层级选择子窗口：位置调整窗口
    public void ShowModifyPanel()
    {
        ModifyPanel.SetActive(true);

    }
    public void HideModifyPanel()
    {
        ModifyPanel.SetActive(false);
    }
    //视图子窗口：视图设置
    public void ShowViewPanel()
    {
        ViewPanel.SetActive(true);

    }
    public void HideViewPanel()
    {
        ViewPanel.SetActive(false);
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