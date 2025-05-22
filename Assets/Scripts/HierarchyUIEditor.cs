#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(HierarchyUI))]
public class HierarchyUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HierarchyUI hierarchyUI = (HierarchyUI)target;

        if (GUILayout.Button("Create Default UI"))
        {
            CreateDefaultUI(hierarchyUI);
        }
    }

    void CreateDefaultUI(HierarchyUI hierarchyUI)
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("Hierarchy Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建ScrollView
        GameObject scrollView = new GameObject("Scroll View");
        scrollView.transform.SetParent(canvasObj.transform);
        scrollView.AddComponent<RectTransform>();
        scrollView.AddComponent<Image>();
        scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<Mask>();

        // 创建Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(scrollView.transform);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        contentRect.anchoredPosition = Vector2.zero;
        content.AddComponent<VerticalLayoutGroup>();
        content.AddComponent<ContentSizeFitter>();
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 创建Item预制体
        GameObject itemPrefab = new GameObject("Hierarchy Item");
        itemPrefab.transform.SetParent(canvasObj.transform);
        itemPrefab.AddComponent<RectTransform>();
        itemPrefab.AddComponent<Image>();
        itemPrefab.AddComponent<Button>();

        // 创建文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(itemPrefab.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = "Item";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.black;

        // 设置HierarchyUI引用
        hierarchyUI.targetRoot = null; // 用户需要手动设置
        hierarchyUI.itemPrefab = itemPrefab;
        hierarchyUI.contentPanel = contentRect;

        // 保存预制体
        string prefabPath = "Assets/Prefabs/HierarchyItem.prefab";
        if (!System.IO.Directory.Exists("Assets/Prefabs"))
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
        }
        PrefabUtility.SaveAsPrefabAsset(itemPrefab, prefabPath);
        DestroyImmediate(itemPrefab);

        // 设置HierarchyUI的预制体引用
        hierarchyUI.itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        EditorUtility.SetDirty(hierarchyUI);
    }
}
#endif
