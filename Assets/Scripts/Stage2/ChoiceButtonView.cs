using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 선택지 버튼 프리팹에 붙이는 컴포넌트
public class ChoiceButtonView : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;

    public void Setup(string text, Action onClick)
    {
        if (label != null)
            label.text = text;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}
