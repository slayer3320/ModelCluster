using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class HierarchyUI : MonoBehaviour
{
    public static HierarchyUI Instance;
    void Awake()
    {
        Instance = this;
    }

    public Transform targetRoot; // 要显示的根物体
    public GameObject itemPrefab; // 每个层级项的预制体
    public RectTransform contentPanel; // ScrollView的内容区域
    public Color highlightColor = Color.red;
    public Sprite expandSprite;
    public Sprite collapseSprite;



    void Start()
    {
        BuildHierarchy(targetRoot, contentPanel);
    }

    void Update()
    {
        // foreach (var x in uiItems.Values)
        // {
        //     Destroy(x);
        // }
        // uiItems.Clear();
        // BuildHierarchy(targetRoot, contentPanel);
    }

    private Dictionary<GameObject, GameObject> uiItems = new Dictionary<GameObject, GameObject>();
    void BuildHierarchy(Transform root, Transform uiParent)
    {
        if (root.gameObject.layer == LayerMask.NameToLayer("NoShow")) return;

        var uiItem = Instantiate(itemPrefab, uiParent);
        uiItems.Add(root.gameObject, uiItem);

        HierarchyItem itemScript = uiItem.transform.GetChild(0).GetComponent<HierarchyItem>();
        itemScript.expandButton.onClick.AddListener(() => itemScript.showChildren = !itemScript.showChildren);
        itemScript.button.onClick.AddListener(() => HighlightObject(root.gameObject));
        itemScript.text.text = root.name;
        
        root.GetComponentsInChildren<Transform>(true).Where(x => x.gameObject != root.gameObject).ToList().ForEach(x =>
        {
            BuildHierarchy(x, itemScript.verticalLayoutGroup);
        });
    }

    private Outline outline;
    public void HighlightObject(GameObject obj)
    {
        if(outline != null) Destroy(outline);
        outline = obj.gameObject.GetComponent<Outline>() ?? obj.gameObject.AddComponent<Outline>();
        outline.OutlineColor = highlightColor;
        outline.OutlineWidth = 1.68f;
        outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
    }

}
