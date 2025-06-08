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

    public Button mergeButton;
    public Button submergeButton;
    public Button importButton;
    public Button exportButton;

    void Start()
    {
        BuildHierarchy(targetRoot, contentPanel);

        mergeButton.onClick.AddListener(MergeSelectedObjects);
        submergeButton.onClick.AddListener(SubmergeSelectedObjects);
        exportButton.onClick.AddListener(() =>
        {
            string objPath = StandaloneFileBrowser.SaveFilePanel("Save File", Application.dataPath, targetRoot.name, "obj");

            RuntimeOBJExporter.instance.ExportGameObjectsToOBJ(Main.current.GetAllChildrenObjects(Main.current.gameObject), Main.current.gameObject, objPath);
        });

    }

    private int mergeIdx = 1;
    public void MergeSelectedObjects()
    {
        if (selectedItems.Count > 1)
        {
            GameObject mergedObject = new GameObject("MergedObject" + mergeIdx++);
            mergedObject.transform.SetParent(targetRoot);
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

    }

    public Dictionary<GameObject, GameObject> uiItems = new Dictionary<GameObject, GameObject>();
    private List<HierarchyItem> selectedItems = new List<HierarchyItem>();
    void BuildHierarchy(Transform root, Transform uiParent)
    {
        //if (root.gameObject.layer == LayerMask.NameToLayer("NoShow")) return;

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

        foreach (Transform child in root)
        {
            BuildHierarchy(child, itemScript.verticalLayoutGroup);
        }

        // root.GetComponentsInChildren<Transform>(true).Where(x => x.gameObject != root.gameObject).ToList().ForEach(x =>
        // {
        //     BuildHierarchy(x, itemScript.verticalLayoutGroup);
        // });
    }

    void ReBuildHierarchy()
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



}
