using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonthToggleView : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image background;
    [SerializeField] private Text label;

    [Header("선택 시")]
    [SerializeField] private Color selectedBackgroundColor = new Color32(0x1A, 0x3D, 0xD1, 0xFF);
    [SerializeField] private Color selectedTextColor = Color.white;

    [Header("선택 안됐을 때 (기본)")]
    [SerializeField] private Color normalBackgroundColor = Color.white;
    [SerializeField] private Color normalTextColor = Color.black;

    void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        background.color = isOn ? selectedBackgroundColor : normalBackgroundColor;
        label.color = isOn ? selectedTextColor : normalTextColor;
    }

    void OnEnable()
    {
        // 초기 상태 즉시 반영 (씬 시작 시 색상 안 맞는 문제 방지)
        OnToggleChanged(toggle.isOn);
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}