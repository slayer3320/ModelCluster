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

    public Transform targetRoot;
    public GameObject itemPrefab; 
    public RectTransform contentPanel; 
    public Color highlightColor = Color.red;
    public Sprite expandSprite;
    public Sprite collapseSprite;

    public Button mergeButton;

    void Start()
    {
        BuildHierarchy(targetRoot, contentPanel);
        
        mergeButton.onClick.AddListener(() =>
        {
            if (targetRoot.childCount > 1)
            {
                GameObject mergedObject = new GameObject("MergedObject");
                mergedObject.transform.SetParent(targetRoot);
                foreach (Transform child in targetRoot)
                {
                    if (child.gameObject.layer != LayerMask.NameToLayer("NoShow"))
                    {
                        child.SetParent(mergedObject.transform);
                    }
                }
            }
        });
    }

    void Update()
    {
        
    }

    private Dictionary<GameObject, GameObject> uiItems = new Dictionary<GameObject, GameObject>();
    private List<HierarchyItem> selectedItems = new List<HierarchyItem>();
    void BuildHierarchy(Transform root, Transform uiParent)
    {
        if (root.gameObject.layer == LayerMask.NameToLayer("NoShow")) return;

        var uiItem = Instantiate(itemPrefab, uiParent);
        uiItems.Add(root.gameObject, uiItem);

        HierarchyItem itemScript = uiItem.transform.GetChild(0).GetComponent<HierarchyItem>();
        itemScript.expandButton.onClick.AddListener(() => itemScript.showChildren = !itemScript.showChildren);
        itemScript.button.onClick.AddListener(() =>
        {
            if (selectedItems.Contains(itemScript))
            {
                selectedItems.Remove(itemScript);
            }
            else
            {
                selectedItems.Add(itemScript);
            }
            HighlightObject(root.gameObject);
        });
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
