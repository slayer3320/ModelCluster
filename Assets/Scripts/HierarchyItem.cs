using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyItem : MonoBehaviour
{
    public LayoutElement parentLayoutElement;
    public RectTransform rectTransform;

    public Button expandButton;
    public Button button;
    public Text text;
    public Transform verticalLayoutGroup;

    public int childCount = 0;
    public bool showChildren = false;

    void Awake()
    {
        
    }

    void Update()
    {
        childCount = GetComponentsInChildren<HierarchyItem>(true).Where(x => x.gameObject != gameObject).Count();
        parentLayoutElement.preferredHeight = 20 + 25 * childCount;
        rectTransform.localPosition = new Vector3
        (
            rectTransform.localPosition.x,
            (parentLayoutElement.preferredHeight - 20) / 2,
            rectTransform.localPosition.z
        );
    }

    public void UpdateExpansion()
    {
        childCount = GetComponentsInChildren<HierarchyItem>(true).Where(x => x.gameObject != gameObject).Count();
        parentLayoutElement.preferredHeight = 20 + 25 * childCount;
        rectTransform.position = new Vector3
        (
            rectTransform.position.x,
            (parentLayoutElement.preferredHeight - 20) / 2,
            rectTransform.position.z
        );
    }


}
