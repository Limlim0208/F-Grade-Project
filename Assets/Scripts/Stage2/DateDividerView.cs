using TMPro;
using UnityEngine;

// 날짜 구분자 프리팹에 붙이는 컴포넌트
public class DateDividerView : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;

    public void SetDate(string label)
    {
        if (dateText != null)
            dateText.text = label;
    }
}
