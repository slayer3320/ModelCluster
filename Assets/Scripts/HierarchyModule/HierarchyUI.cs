using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Dummiesman;
using SFB;

public class HierarchyUI : MonoBehaviour
{
    public static HierarchyUI Instance;
    void Awake()
    {
        Instance = this;
    }

    public Transform targetRoot;
    public GameObject itemPrefab;
    public RectTransform contentPanel;
    public Color highlightColor = Color.red;
    public Sprite expandSprite;
    public Sprite collapseSprite;
    public ScrollRect scrollRect;


    private bool isFirstBuild = true;
    void Start()
    {
        BuildHierarchy(targetRoot, contentPanel);
        isFirstBuild = false;
        // mergeButton.onClick.AddListener(MergeSelectedObjects);
        // submergeButton.onClick.AddListener(SubmergeSelectedObjects);
        // exportButton.onClick.AddListener(() =>
        // {
        //     string objPath = StandaloneFileBrowser.SaveFilePanel("Save File", Application.dataPath, targetRoot.name, "obj");

        //     RuntimeOBJExporter.instance.ExportGameObjectsToOBJ(Main.current.GetAllChildrenObjects(Main.current.gameObject), Main.current.gameObject, objPath);
        // });

    }

    private int mergeIdx = 1;

    public void MergeSelectedObjects()
    {
        if (selectedItems.Count > 1)
        {
            GameObject mergedObject = new GameObject("MergedObject" + mergeIdx++);
            mergedObject.transform.SetParent(targetRoot);
            mergedObject.transform.SetSiblingIndex(0);
            foreach (var item in selectedItems)
            {
                GameObject obj = uiItems.FirstOrDefault(x => x.Value == item.gameObject.transform.parent.gameObject).Key;
                if (obj != null && obj.layer != LayerMask.NameToLayer("NoShow"))
                {
                    obj.transform.SetParent(mergedObject.transform);
                }

                UnHighlightObject(obj);
            }
            selectedItems.Clear();
        }

        ReBuildHierarchy();
    }

    public void SubmergeSelectedObjects()
    {
        if (selectedItems.Count > 0)
        {
            GameObject obj = uiItems.FirstOrDefault(x => x.Value == selectedItems[^1].gameObject.transform.parent.gameObject).Key;
            if (obj.GetComponent<MeshFilter>() != null)
            {
                return;
            }
            if (obj != null && obj.layer != LayerMask.NameToLayer("NoShow"))
            {
                Transform parent = obj.transform.parent;
                if (parent == null) return;
                List<Transform> children = new List<Transform>();
                foreach (Transform child in obj.transform)
                {
                    //Debug.Log(child.gameObject.name);
                    UnHighlightObject(child.gameObject);
                    children.Add(child);
                }

                children.ForEach(x => x.gameObject.transform.SetParent(parent));
            }
            UnHighlightObject(obj);

            DestroyImmediate(obj);
            selectedItems.Clear();
        }

        ReBuildHierarchy();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // 检查点击的对象是否在 uiItems 映射中
                if (uiItems.TryGetValue(clickedObject, out GameObject uiItem))
                {
                    // 获取 UI 控件并切换 toggle
                    HierarchyItem item = uiItem.transform.GetChild(0).GetComponent<HierarchyItem>();

                    // 切换 Toggle 状态（会自动触发 toggle.onValueChanged 中的高亮逻辑）
                    item.toggle.isOn = !item.toggle.isOn;

                    // 滚动到这个 UI 项目
                    ScrollToItem(uiItem);
                }
            }
        }

    }

    public Dictionary<GameObject, GameObject> uiItems = new Dictionary<GameObject, GameObject>();
    private List<HierarchyItem> selectedItems = new List<HierarchyItem>();
    void BuildHierarchy(Transform root, Transform uiParent)
    {
        //if (root.gameObject.layer == LayerMask.NameToLayer("NoShow")) return;
        // 如果是 targetRoot 下的第一个子物体，重命名为 "UnmergedObject"
        if (root.parent == targetRoot && root.GetSiblingIndex() == 0 && isFirstBuild)
        {
            root.name = "UnmergedObjects";
        }
        var uiItem = Instantiate(itemPrefab, uiParent);
        uiItems.Add(root.gameObject, uiItem);

        HierarchyItem itemScript = uiItem.transform.GetChild(0).GetComponent<HierarchyItem>();
        itemScript.expandButton.onClick.AddListener(() => itemScript.showChildren = !itemScript.showChildren);
        itemScript.toggle.onValueChanged.AddListener(isOn =>
        {


            if (isOn)
            {
                selectedItems.Add(itemScript);
                HighlightObject(root.gameObject);
            }
            else
            {
                selectedItems.Remove(itemScript);
                UnHighlightObject(root.gameObject);
            }
        });
        itemScript.text.text = root.name;
        itemScript.boundObject = root.gameObject;
        foreach (Transform child in root)
        {
            BuildHierarchy(child, itemScript.verticalLayoutGroup);
        }

        // root.GetComponentsInChildren<Transform>(true).Where(x => x.gameObject != root.gameObject).ToList().ForEach(x =>
        // {
        //     BuildHierarchy(x, itemScript.verticalLayoutGroup);
        // });
    }

    public void ReBuildHierarchy()
    {
        uiItems.Clear();
        selectedItems.Clear();

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        BuildHierarchy(targetRoot, contentPanel);
    }

    private void HighlightObject(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("Outline");
    }

    private void UnHighlightObject(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("Default");
    }


    public void ScrollToItem(GameObject uiItem)
    {
        if (uiItem == null || scrollRect == null) return;

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // 获取 UI 项在 Content 中的 index（排第几个）
        int index = uiItem.transform.GetSiblingIndex();

        float itemHeight = 50f;
        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        // 目标中心点的偏移量（让目标项尽量显示在中间）
        float targetY = index * itemHeight + itemHeight / 2f - viewportHeight / 2f;

        // 将 targetY 映射到 normalizedPosition（0=底部，1=顶部）
        float normalized = Mathf.Clamp01(targetY / (contentHeight - viewportHeight));

        scrollRect.verticalNormalizedPosition = 1f - normalized;
    }

}
