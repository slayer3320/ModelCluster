using UnityEngine;
using System.Collections.Generic;

public class ModelPreprocessor : MonoBehaviour
{
    public float targetMaxSize = 200f;
    public GameObject rootObject;

    void Start()
    {
        InitImportModel();
    }
    public void InitImportModel()
    {
        if (rootObject == null)
        {
            Debug.LogWarning("请在 Inspector 中指定 rootObject");
            return;
        }

        PreprocessModel(rootObject.transform);
    }

    void PreprocessModel(Transform modelRoot)
    {
        // Step 1: 重命名所有子对象
        GameObject[] children = GetAllChildrenObjects(modelRoot.gameObject);
        Debug.Log($"处理了 {children.Length} 个子对象（已重命名）");

        // Step 2: 缩放模型到目标尺寸
        Bounds bounds = GetCombinedBounds(modelRoot);
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = targetMaxSize / maxSize;
        modelRoot.localScale *= scaleFactor;

        // Step 3: 重新计算 bounds 和居中
        bounds = GetCombinedBounds(modelRoot);
        Vector3 centerOffset = bounds.center - modelRoot.position;

        foreach (Transform child in modelRoot)
        {
            child.position -= centerOffset;
        }

        Debug.Log($"[ModelPreprocessor] {modelRoot.name} 缩放系数：{scaleFactor:F4}，模型中心已对齐 pivot。");
    }

    GameObject[] GetAllChildrenObjects(GameObject parent)
    {
        List<GameObject> allChildren = new List<GameObject>();
        GetChildrenRecursive(parent.transform, allChildren);
        return allChildren.ToArray();
    }

    void GetChildrenRecursive(Transform parent, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            ProcessGameObject(child.gameObject);
            list.Add(child.gameObject);
            GetChildrenRecursive(child, list);
        }
    }

    void ProcessGameObject(GameObject obj)
    {
        string original = obj.name;
        string[] parts = obj.name.Split('_');
        if (parts.Length > 0)
        {
            obj.name = parts[0];
            Debug.Log($"重命名：{original} → {obj.name}");
        }
    }

    Bounds GetCombinedBounds(Transform root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(root.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }
}
