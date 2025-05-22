using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Collections.Generic;

public class HierarchyUI : MonoBehaviour
{
    public Transform targetRoot; // 要显示的根物体
    public GameObject itemPrefab; // 每个层级项的预制体
    public RectTransform contentPanel; // ScrollView的内容区域
    public Color highlightColor = Color.red;
    public Sprite expandSprite;
    public Sprite collapseSprite;

    private Dictionary<GameObject, GameObject> uiItems = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, bool> expandedStates = new Dictionary<GameObject, bool>();
    private List<GameObject> selectedObjects = new List<GameObject>();
    private GameObject currentHighlight;

    void Start()
    {
        if (targetRoot != null)
        {
            BuildHierarchy(targetRoot);
        }
    }

    void BuildHierarchy(Transform root, int indent = 0, bool parentExpanded = true)
    {
        if (!parentExpanded) return;

        // 创建UI项
        var uiItem = Instantiate(itemPrefab, contentPanel);
        var rect = uiItem.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(indent * 20, -contentPanel.childCount * 30);

        // 设置文本显示
        var text = uiItem.GetComponentInChildren<Text>();
        text.text = root.name;

        // 设置展开/折叠图标
        var expandIcon = uiItem.transform.Find("ExpandIcon")?.GetComponent<Image>();
        bool hasChildren = root.childCount > 0;
        
        if (expandIcon != null && hasChildren)
        {
            bool isExpanded = expandedStates.ContainsKey(root.gameObject) ? 
                expandedStates[root.gameObject] : true;
                
            expandIcon.sprite = isExpanded ? collapseSprite : expandSprite;
            expandIcon.gameObject.SetActive(true);

            // 添加展开/折叠点击事件
            var expandButton = expandIcon.GetComponent<Button>();
            if (expandButton != null)
            {
                expandButton.onClick.AddListener(() => ToggleExpand(root.gameObject));
            }
        }
        else if (expandIcon != null)
        {
            expandIcon.gameObject.SetActive(false);
        }

        // 添加点击事件
        var button = uiItem.GetComponent<Button>();
        button.onClick.AddListener(() => HighlightObject(root.gameObject));

        // 添加拖拽组件
        var dragHandler = uiItem.AddComponent<HierarchyDragHandler>();
        dragHandler.Initialize(this, root.gameObject);

        // 添加合并按钮
        var mergeButton = new GameObject("MergeButton");
        var mergeRect = mergeButton.AddComponent<RectTransform>();
        mergeRect.SetParent(uiItem.transform);
        mergeRect.anchoredPosition = new Vector2(150, 0);
        mergeRect.sizeDelta = new Vector2(60, 20);
        
        var mergeImage = mergeButton.AddComponent<Image>();
        mergeImage.color = new Color(0.2f, 0.6f, 1f);
        
        var mergeText = new GameObject("Text");
        var textRect = mergeText.AddComponent<RectTransform>();
        textRect.SetParent(mergeRect);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        
        var textComp = mergeText.AddComponent<Text>();
        textComp.text = "合并";
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.color = Color.white;
        
        var mergeBtn = mergeButton.AddComponent<Button>();
        mergeBtn.onClick.AddListener(MergeSelectedObjects);
        mergeButton.SetActive(false); // 默认隐藏

        uiItems[root.gameObject] = uiItem;

        // 递归处理子物体
        bool showChildren = hasChildren && 
            expandedStates.ContainsKey(root.gameObject) ? 
            expandedStates[root.gameObject] : true;
            
        foreach (Transform child in root)
        {
            BuildHierarchy(child, indent + 1, showChildren);
        }
    }

    public void HighlightObject(GameObject obj)
    {
        // 按住Ctrl键多选
        bool isMultiSelect = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        
        // 清除之前的高亮
        foreach (var selected in selectedObjects)
        {
            var outlines = selected.GetComponentsInChildren<Outline>();
            foreach (var outline in outlines)
            {
                Destroy(outline);
            }
        }

        // 更新所有合并按钮的显示状态
        foreach (var uiItem in uiItems.Values)
        {
            var mergeBtn = uiItem.transform.Find("MergeButton");
            if (mergeBtn != null)
            {
                mergeBtn.gameObject.SetActive(selectedObjects.Count >= 2);
            }
        }
        
        if (!isMultiSelect)
        {
            selectedObjects.Clear();
        }

        // 添加或移除选中物体
        if (selectedObjects.Contains(obj))
        {
            selectedObjects.Remove(obj);
        }
        else
        {
            selectedObjects.Add(obj);
        }

        // 设置新高亮
        foreach (var selected in selectedObjects)
        {
            var renderers = selected.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                var outline = r.gameObject.GetComponent<Outline>() ?? r.gameObject.AddComponent<Outline>();
                outline.OutlineColor = highlightColor;
                outline.OutlineWidth = 1.68f;
                outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
            }
        }
        
    }

    public void MergeSelectedObjects()
    {
        if (selectedObjects.Count < 2) return;

        // 创建新父物体
        GameObject newParent = new GameObject("Merged Group");
        newParent.transform.SetParent(targetRoot);
        newParent.transform.position = Vector3.zero;

        // 将所有选中物体移动到新父物体下
        foreach (var obj in selectedObjects)
        {
            obj.transform.SetParent(newParent.transform);
        }

        // 重建UI
        RebuildHierarchy();
        selectedObjects.Clear();
    }

    public void ReparentObject(GameObject child, GameObject newParent)
    {
        // 改变父子关系但不改变世界位置
        Vector3 worldPos = child.transform.position;
        child.transform.SetParent(newParent.transform);
        child.transform.position = worldPos;

        // 重建UI
        RebuildHierarchy();
    }

    void ToggleExpand(GameObject obj)
    {
        if (expandedStates.ContainsKey(obj))
        {
            expandedStates[obj] = !expandedStates[obj];
        }
        else
        {
            expandedStates[obj] = false;
        }
        RebuildHierarchy();
    }

    void RebuildHierarchy()
    {
        // 清除现有UI
        foreach (var item in uiItems.Values)
        {
            Destroy(item);
        }
        uiItems.Clear();

        // 重新构建
        if (targetRoot != null)
        {
            BuildHierarchy(targetRoot);
        }
    }
}

public class HierarchyDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HierarchyUI hierarchyUI;
    private GameObject representedObject;
    private GameObject potentialParent;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private Transform startParent;

    public void Initialize(HierarchyUI ui, GameObject obj)
    {
        hierarchyUI = ui;
        representedObject = obj;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
        
        // 检测潜在的父物体
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            var item = result.gameObject.GetComponent<HierarchyDragHandler>();
            if (item != null && item != this)
            {
                potentialParent = item.representedObject;
                return;
            }
        }
        potentialParent = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (potentialParent != null && 
            potentialParent != representedObject &&
            !IsChildOf(representedObject.transform, potentialParent.transform))
        {
            hierarchyUI.ReparentObject(representedObject, potentialParent);
        }
        else
        {
            // 恢复原位
            transform.SetParent(startParent);
            rectTransform.anchoredPosition = startPosition;
        }
    }

    private bool IsChildOf(Transform child, Transform parent)
    {
        while (child != null)
        {
            if (child == parent) return true;
            child = child.parent;
        }
        return false;
    }
}
