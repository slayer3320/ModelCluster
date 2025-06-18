using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFB;

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
    public Slider scaleSlider; //层级UI缩放

    //HUI大小
    public RectTransform HUIcontent;
    public float minScale = 0.5f; // 缩放范围
    public float maxScale = 1.0f;

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

    // 模型对象
    public Transform modelRoot;

    // 旋转和缩放速度文本信息
    public TMP_Text rotateSpeedText;
    public TMP_Text zoomSpeedText;
    public TMP_Text cameraSpeedText;
    public TMP_Text HUIscaleText;
    void Start()
    {
        if (ModelController == null)
            Debug.LogError("ModelController not assigned！");
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
        rotateSpeedText.text = "旋转角度:" + ModelController.rotateSpeed.ToString("F2");
        zoomSpeedText.text = "缩放速度:" + ModelController.zoomSpeed.ToString("F2");
        cameraSpeedText.text = "相机移速:" + CameraController.moveSpeed.ToString("F2");
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
        string objPath = StandaloneFileBrowser.SaveFilePanel("Save File", Application.dataPath, modelRoot.name, "obj");
        RuntimeOBJExporter.instance.ExportGameObjectsToOBJ(Main.current.GetAllChildrenObjects(Main.current.gameObject), Main.current.gameObject, objPath);

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
        HideHierarchyPanel();

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
        HideHierarchyPanel();
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