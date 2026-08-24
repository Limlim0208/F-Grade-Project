using TMPro;
using UnityEngine;

// 날짜 구분자 프리팹에 붙이는 컴포넌트
public class DateDividerView : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text deadlineText; // "제출 마감일까지 D-N" 표시 (선택)

    public void SetDate(string label)
    {
        if (dateText != null)
            dateText.text = label;
    }

    public void SetDeadline(int daysRemaining)
    {
        if (deadlineText != null)
            deadlineText.text = $"제출 마감일까지 D-{daysRemaining}";
    }
}
