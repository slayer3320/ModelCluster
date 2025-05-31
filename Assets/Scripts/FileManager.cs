using System.Collections;
using UnityEngine;
using TMPro;
using Dummiesman;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Transform modelRoot;

    public GameObject loadingPanel;  // 进度条外层面板
    public Slider progressSlider;    // 进度条

    private GameObject model;

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
        // 显示加载UI
        loadingPanel.SetActive(true);
        progressSlider.value = 0;

        // 模拟加载时间（按文件大小估算）
        long fileSizeInBytes = new System.IO.FileInfo(filePath).Length;
        float estimatedDuration = Mathf.Clamp(fileSizeInBytes / (2048f * 1024f), 1f, 1f); // 固定为1秒

        float timer = 0f;
        while (timer < estimatedDuration)
        {
            timer += Time.deltaTime;
            progressSlider.value = Mathf.Clamp01(timer / estimatedDuration) * 100f;
            yield return null;
        }

        // 清理旧模型
        if (model != null)
        {
            Destroy(model);
        }
        foreach (Transform child in modelRoot)
        {
            Destroy(child.gameObject);
        }

        // 加载带材质的模型
        model = new OBJLoaderWithMaterials().Load(filePath);
        model.transform.SetParent(modelRoot, false);

        // 转换为URP材质
        ConvertToURPMaterial(model);

        // 收尾工作
        loadingPanel.SetActive(false);
        FitOnScreen();
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
            foreach (var oldMat in renderer.materials)
            {
                Material newMat = new Material(urpShader);

                if (oldMat.HasProperty("_MainTex"))
                    newMat.SetTexture("_BaseMap", oldMat.mainTexture);
                if (oldMat.HasProperty("_Color"))
                    newMat.SetColor("_BaseColor", oldMat.color);

                // 关闭剔除，双面渲染
                newMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

                renderer.material = newMat;
            }
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

    public void FitOnScreen()
    {
        if (model == null) return;

        Bounds bound = GetBound(model);
        Vector3 boundSize = bound.size;
        float diagonal = Mathf.Sqrt(boundSize.x * boundSize.x + boundSize.y * boundSize.y + boundSize.z * boundSize.z);
        Camera.main.orthographicSize = diagonal / 2.0f;
        Camera.main.transform.position = bound.center + new Vector3(0, 0, -diagonal);
        Camera.main.transform.LookAt(bound.center);
    }
}
