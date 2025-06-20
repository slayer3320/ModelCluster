using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PositionWindow : MonoBehaviour
{
    public static PositionWindow Instance;
    private Vector3 initialPosition;  // 记录打开窗口时的初始位置
    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;
    public Button resetButton;
    private GameObject target;
    public TMP_Text objectNameText;
    public TMP_Text textX;
    public TMP_Text textY;
    public TMP_Text textZ;
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        sliderX.onValueChanged.AddListener(OnSliderChanged);
        sliderY.onValueChanged.AddListener(OnSliderChanged);
        sliderZ.onValueChanged.AddListener(OnSliderChanged);

        resetButton.onClick.AddListener(ResetPosition);
    }

    public void Open(GameObject obj)
    {
        target = obj;

        objectNameText.text = "已选中:" + obj.name;

        initialPosition = obj.transform.position; // 记录初始位置

        sliderX.minValue = -1000;
        sliderX.maxValue = 1000;
        sliderX.value = initialPosition.x;

        sliderY.minValue = -1000;
        sliderY.maxValue = 1000;
        sliderY.value = initialPosition.y;

        sliderZ.minValue = -1000;
        sliderZ.maxValue = 1000;
        sliderZ.value = initialPosition.z;

        gameObject.SetActive(true);
    }



    void OnSliderChanged(float _)
    {
        if (target == null) return;
        target.transform.position = new Vector3(sliderX.value, sliderY.value, sliderZ.value);

        textX.text = "X坐标:" + sliderX.value.ToString("F2");
        textY.text = "Y坐标:" + sliderY.value.ToString("F2");
        textZ.text = "Z坐标:" + sliderZ.value.ToString("F2");

    }

    void ResetPosition()
    {
        if (target == null) return;

        target.transform.position = initialPosition;

        // 同步滑动条位置
        sliderX.value = initialPosition.x;
        sliderY.value = initialPosition.y;
        sliderZ.value = initialPosition.z;

    }

}
