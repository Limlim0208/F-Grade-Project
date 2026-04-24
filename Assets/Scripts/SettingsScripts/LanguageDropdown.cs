using UnityEngine;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] private TooltipBubble tooltipBubble;

    public void OnValueChanged(int index)
    {
        tooltipBubble.Show(); // 드롭다운 선택 시 말풍선 표시 후 자동 페이드아웃
    }
}