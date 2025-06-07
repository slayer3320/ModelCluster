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
    public Toggle toggle;
    public Text text;
    public Transform verticalLayoutGroup;

    public int childCount = 0;
    public int actualChildCount = 0;
    public bool showChildren = true;

    void Awake()
    {
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                //修改normal color为灰色
                toggle.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                
                verticalLayoutGroup.GetComponentsInChildren<HierarchyItem>(true).ToList().ForEach(item =>
                {
                    item.toggle.isOn = true;
                });
            }
            else
            {
                //修改normal color为白色
                toggle.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                
                verticalLayoutGroup.GetComponentsInChildren<HierarchyItem>(true).ToList().ForEach(item =>
                {
                    item.toggle.isOn = false;
                });
            }
        });
    }

    void Update()
    {
        Expand();
        ChangeToggleSprite();
    }

    public void Expand()
    {
        childCount = showChildren ? GetComponentsInChildren<HierarchyItem>(true).Where(x => x.gameObject != gameObject).Count() : 0;
        actualChildCount = GetComponentsInChildren<HierarchyItem>(true).Where(x => x.gameObject != gameObject).Count();
        verticalLayoutGroup.gameObject.SetActive(showChildren);
        parentLayoutElement.preferredHeight = 20 + 25 * childCount;
        rectTransform.localPosition = new Vector3
        (
            rectTransform.localPosition.x,
            (parentLayoutElement.preferredHeight - 20) / 2,
            rectTransform.localPosition.z
        );
    }

    public void ChangeToggleSprite()
    {
        if (actualChildCount == 0)
        {
            expandButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        else
        {
            if (showChildren)
            {
                expandButton.GetComponent<Image>().sprite = HierarchyUI.Instance.expandSprite;
                expandButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                expandButton.GetComponent<Image>().sprite = HierarchyUI.Instance.collapseSprite;
                expandButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        
    }

}
