using System.Collections;
using UnityEngine;
using TMPro;
using Dummiesman;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Transform modelRoot;
    public ModelPreprocessor ModelPreprocessor;
    public GameObject loadingPanel;  // 进度条外层面板
    public Slider progressSlider;    // 进度条

    private GameObject model;

    private Coroutine loadingAnimation;
    public RectTransform loadingBlock; // 小方块（滑块）
    public float blockSpeed = 200f;    // 滑块移动速度
    private bool isLoading = false;


    public void OnClickOpen()
    {
        string[] paths = SFB.StandaloneFileBrowser.OpenFilePanel("Open OBJ File", "", "obj", false);
        if (paths.Length > 0)
        {
            StartCoroutine(LoadOBJWithProgress(paths[0]));
        }
    }

    private IEnumerator LoadOBJWithProgress(string filePath)
    {
        // 启动加载状态
        isLoading = true;

        // 显示加载UI并重置进度
        loadingPanel.SetActive(true);
        progressSlider.value = 0;

        // 启动滑块动画协程
        loadingAnimation = StartCoroutine(LoadingAnimation());

        // 模拟加载时间（可替换为真实解析加载）
        long fileSizeInBytes = new System.IO.FileInfo(filePath).Length;
        float estimatedDuration = Mathf.Clamp(fileSizeInBytes / (2048f * 1024f), 1f, 1.5f); // 1~1.5秒之间

        yield return new WaitForSeconds(estimatedDuration);

        // 清理旧模型
        if (model != null)
            Destroy(model);
        foreach (Transform child in modelRoot)
            Destroy(child.gameObject);

        // 加载新模型
        model = new OBJLoaderWithMaterials().Load(filePath);
        model.transform.SetParent(modelRoot, false);

        // 转URP材质
        ConvertToURPMaterial(model);

        // 隐藏加载面板
        loadingPanel.SetActive(false);
        yield return new WaitForEndOfFrame();
    
        Debug.Log($"modelRoot 子对象数: {modelRoot.childCount}");
        ModelPreprocessor.InitImportModel();
        if (HierarchyUI.Instance != null)
        {
            HierarchyUI.Instance.ReBuildHierarchy();
        }
        else
        {
            Debug.LogWarning("HierarchyUI.Instance is null! Make sure HierarchyUI is in the scene.");
        }

    }
    private IEnumerator LoadingAnimation()
    {
        // 滑块在进度条上的左右摆动动画
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

   private void ConvertToURPMaterial(GameObject modelRoot)
{
    Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
    if (urpShader == null)
    {
        Debug.LogError("URP Shader not found!");
        return;
    }

    foreach (var renderer in modelRoot.GetComponentsInChildren<Renderer>())
    {
        Material[] oldMats = renderer.materials;
        Material[] newMats = new Material[oldMats.Length];

        for (int i = 0; i < oldMats.Length; i++)
        {
            Material oldMat = oldMats[i];
            Material newMat = new Material(urpShader);

            if (oldMat.HasProperty("_MainTex"))
                newMat.SetTexture("_BaseMap", oldMat.mainTexture);
            if (oldMat.HasProperty("_Color"))
                newMat.SetColor("_BaseColor", oldMat.color);
            if (oldMat.HasProperty("_BumpMap"))
                newMat.SetTexture("_BumpMap", oldMat.GetTexture("_BumpMap"));
            if (oldMat.HasProperty("_EmissionMap"))
                newMat.SetTexture("_EmissionMap", oldMat.GetTexture("_EmissionMap"));

            // 双面显示
            newMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            newMats[i] = newMat;
        }

        renderer.materials = newMats;
    }
}
    private Bounds GetBound(GameObject gameObj)
    {
        Bounds bound = new Bounds(gameObj.transform.position, Vector3.zero);
        var rList = gameObj.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rList)
        {
            bound.Encapsulate(r.bounds);
        }
        return bound;
    }


}
