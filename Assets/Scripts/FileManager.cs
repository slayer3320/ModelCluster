using System.Collections;
using UnityEngine;
using TMPro;
using Dummiesman;
using UnityEngine.UI;
using SFB;
using System.IO;

public class FileManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Transform modelRoot;
    public ModelPreprocessor ModelPreprocessor;
    public GameObject loadingPanel;
    public GameObject OpenPanel;
    public Slider progressSlider;
    public RectTransform loadingBlock;

    public float blockSpeed = 200f;

    private GameObject model;
    private Coroutine loadingAnimation;
    private bool isLoading = false;

    public void OnClickOpen()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open OBJ File", "", "obj", false);
        if (paths.Length > 0)
        {
            StartCoroutine(LoadOBJWithProgress(paths[0]));
        }
    }

    public void OnClickExport()
    {
        string objPath = StandaloneFileBrowser.SaveFilePanel("Save File", "", modelRoot.name, "obj");
        if (!string.IsNullOrEmpty(objPath))
        {
            StartCoroutine(ExportWithLoading(objPath));
        }
    }

    // ----------- 通用加载控制 -----------
    IEnumerator ShowLoading()
    {
        isLoading = true;
        OpenPanel.SetActive(false);
        loadingPanel.SetActive(true);
        progressSlider.value = 0;
        loadingAnimation = StartCoroutine(LoadingAnimation());
        yield return null;
    }

    IEnumerator HideLoading()
    {
        isLoading = false;
        if (loadingAnimation != null)
            StopCoroutine(loadingAnimation);

        yield return new WaitForSeconds(0.2f); // 保留0.2秒美观
        loadingPanel.SetActive(false);
    }

    // ----------- 导入逻辑 -----------
    private IEnumerator LoadOBJWithProgress(string filePath)
    {
        yield return ShowLoading();

        long fileSize = new FileInfo(filePath).Length;
        float estimatedDuration = Mathf.Clamp(fileSize / (2048f * 1024f), 1f, 1.5f);
        yield return new WaitForSeconds(estimatedDuration);

        if (model != null)
            Destroy(model);
        foreach (Transform child in modelRoot)
            Destroy(child.gameObject);

        model = new OBJLoader().Load(filePath);
        model.name = "UnmergedObjects";
        model.transform.SetParent(modelRoot, false);

        yield return HideLoading();

        Debug.Log($"modelRoot 子对象数: {modelRoot.childCount}");

        ModelPreprocessor.InitImportModel();

        if (HierarchyUI.Instance != null)
            HierarchyUI.Instance.ReBuildHierarchy();
        else
            Debug.LogWarning("HierarchyUI.Instance is null! Check scene.");
    }

    // ----------- 导出逻辑 -----------
    private IEnumerator ExportWithLoading(string path)
    {
        yield return ShowLoading();

        yield return new WaitForSeconds(1f); // 导出动画展示时间

        RuntimeOBJExporter.instance.ExportGameObjectsToOBJ(
            Main.current.GetAllChildrenObjects(Main.current.gameObject),
            Main.current.gameObject,
            path
        );

        yield return HideLoading();
        Debug.Log($"导出完成: {path}");
    }

    // ----------- 动画控制逻辑 -----------
    private IEnumerator LoadingAnimation()
    {
        loadingBlock.anchoredPosition = new Vector2(0, loadingBlock.anchoredPosition.y);

        float sliderWidth = progressSlider.GetComponent<RectTransform>().rect.width - 100;
        float leftBound = -sliderWidth / 2f;
        float rightBound = sliderWidth / 2f;
        bool movingRight = true;

        while (isLoading)
        {
            float currentX = loadingBlock.anchoredPosition.x;

            if (movingRight)
            {
                currentX += blockSpeed * Time.deltaTime;
                if (currentX >= rightBound)
                {
                    currentX = rightBound;
                    movingRight = false;
                }
            }
            else
            {
                currentX -= blockSpeed * Time.deltaTime;
                if (currentX <= leftBound)
                {
                    currentX = leftBound;
                    movingRight = true;
                }
            }

            loadingBlock.anchoredPosition = new Vector2(currentX, loadingBlock.anchoredPosition.y);
            yield return null;
        }
    }
}
