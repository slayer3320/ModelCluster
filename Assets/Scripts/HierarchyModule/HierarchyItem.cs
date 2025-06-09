using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HierarchyItem : MonoBehaviour
{
    public LayoutElement parentLayoutElement;
    public RectTransform rectTransform;

    public Button expandButton;
    public Toggle toggle;
    public TextMeshProUGUI  text;
    public Transform verticalLayoutGroup;
    public int childCount = 0;
    public int actualChildCount = 0;
    public bool showChildren = true;
    
    public Color normalColor_on;    
    public Color normalColor_off;    
    public Color noChildColor;     



    void Awake()
    {
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                //修改normal color为灰色
                toggle.GetComponent<Image>().color = normalColor_on;
                
                // verticalLayoutGroup.GetComponentsInChildren<HierarchyItem>(true).ToList().ForEach(item =>
                // {
                //     item.toggle.isOn = true;
                // });
            }
            else
            {
                //修改normal color为白色
                toggle.GetComponent<Image>().color = normalColor_off;
                
                // verticalLayoutGroup.GetComponentsInChildren<HierarchyItem>(true).ToList().ForEach(item =>
                // {
                //     item.toggle.isOn = false;
                // });
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
        parentLayoutElement.preferredHeight = 50 + 50 * childCount;
        rectTransform.localPosition = new Vector3
        (
            rectTransform.localPosition.x,
            (parentLayoutElement.preferredHeight - 50) / 2,
            rectTransform.localPosition.z
        );
    }

    public void ChangeToggleSprite()
    {
        if (actualChildCount == 0)
        {
            expandButton.GetComponent<Image>().color = noChildColor;
        }
        else
        {
            expandButton.GetComponent<Image>().color = normalColor_on;
            if (showChildren)
            {
                expandButton.GetComponent<Image>().sprite = HierarchyUI.Instance.expandSprite;

            }
            else
            {
                expandButton.GetComponent<Image>().sprite = HierarchyUI.Instance.collapseSprite;

            }
        }

        
    }

}
