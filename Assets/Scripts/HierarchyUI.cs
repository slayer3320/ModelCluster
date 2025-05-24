using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

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
            BuildHierarchy(targetRoot, contentPanel);
        }
    }

    void BuildHierarchy(Transform root, Transform uiParent)
    {
        if (root.gameObject.layer == LayerMask.NameToLayer("NoShow")) return;

        var uiItem = Instantiate(itemPrefab, uiParent);
        HierarchyItem itemScript = uiItem.transform.GetChild(0).GetComponent<HierarchyItem>();
        //itemScript.expandButton
        //itemScript.button.onClick.AddListener(() => HighlightObject(root.gameObject));
        itemScript.text.text = root.name;
        
        root.GetComponentsInChildren<Transform>(true).Where(x => x.gameObject != root.gameObject).ToList().ForEach(x =>
        {
            BuildHierarchy(x, itemScript.verticalLayoutGroup);
        });
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
        foreach (var item in uiItems.Values)
        {
            Destroy(item);
        }
        uiItems.Clear();

        if (targetRoot != null)
        {
            BuildHierarchy(targetRoot, contentPanel);
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
