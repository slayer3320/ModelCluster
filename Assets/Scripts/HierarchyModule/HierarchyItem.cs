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

    public Button editnameButton;
    public Toggle toggle;
    public TextMeshProUGUI text;
    public TMP_InputField nameInput;
    public GameObject boundObject;
    public Transform verticalLayoutGroup;
    public int childCount = 0;
    public int actualChildCount = 0;
    public bool showChildren = true;

    public Color normalColor_on;
    public Color normalColor_off;
    public Color noChildColor;



    void Awake()
    {
        editnameButton.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(false);
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                //修改normal color为灰色
                toggle.GetComponent<Image>().color = normalColor_on;
                editnameButton.gameObject.SetActive(true);

                // verticalLayoutGroup.GetComponentsInChildren<HierarchyItem>(true).ToList().ForEach(item =>
                // {
                //     item.toggle.isOn = true;
                // });
            }
            else
            {
                //修改normal color为白色
                toggle.GetComponent<Image>().color = normalColor_off;
                editnameButton.gameObject.SetActive(false);
                // verticalLayoutGroup.GetComponentsInChildren<HierarchyItem>(true).ToList().ForEach(item =>
                // {
                //     item.toggle.isOn = false;
                // });
            }
        });

        // 点击编辑按钮，显示输入框并赋值当前名字
        editnameButton.onClick.AddListener(() =>
        {
            nameInput.text = text.text;
            nameInput.gameObject.SetActive(true);
            nameInput.Select(); // 聚焦
        });

        // 当用户按下 Enter 或失去焦点时提交
        nameInput.onEndEdit.AddListener(newName =>
        {
            if (!string.IsNullOrWhiteSpace(newName))
            {
                text.text = newName;
                gameObject.name = newName; // 更新 root.name
                if (boundObject != null)
                {
                    boundObject.name = newName; // 👈 更新对应 GameObject 的名称
                }

            }
            nameInput.gameObject.SetActive(false);
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
