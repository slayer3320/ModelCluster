using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyItem : MonoBehaviour
{
    private LayoutElement parentLayoutElement;
    private RectTransform rectTransform;

    public int childCount = 0;

    void Awake()
    {
        parentLayoutElement = transform.parent.GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();

        childCount = GetComponentsInChildren<HierarchyItem>(true).Where(x => x.gameObject != gameObject).Count();
        parentLayoutElement.preferredHeight = 20 + 25 * childCount;
        rectTransform.position = new Vector3
        (
            rectTransform.position.x,
            (parentLayoutElement.preferredHeight - 20) / 2,
            rectTransform.position.z
        );
    }

    void Update()
    {
        childCount = GetComponentsInChildren<HierarchyItem>().Where(x => x.gameObject != gameObject).Count();
        parentLayoutElement.preferredHeight = 20 + 25 * childCount;
        rectTransform.localPosition = new Vector3
        (
            rectTransform.localPosition.x,
            (parentLayoutElement.preferredHeight - 20) / 2,
            rectTransform.localPosition.z
        );
    }

    
}
