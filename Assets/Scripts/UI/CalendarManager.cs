using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    [Header("Month Selector (동적 생성)")]
    [SerializeField] private Transform monthButtonContainer;
    [SerializeField] private GameObject monthButtonPrefab;
    [SerializeField] private ToggleGroup monthToggleGroup;

    [Header("Month Panels (씬에 12개 미리 배치, 이미지 + 이벤트 바 전부 손으로 구성)")]
    [SerializeField] private GameObject[] monthPanels = new GameObject[12];

    [Header("Month Labels (버튼 텍스트)")]
    [SerializeField]
    private string[] monthLabels =
    {
        "1월", "2월", "3월", "4월", "5월", "6월",
        "7월", "8월", "9월", "10월", "11월", "12월"
    };

    void Start()
    {
        GenerateMonthButtons();
        SelectMonth(0);
    }

    private void GenerateMonthButtons()
    {
        for (int index = 0; index < monthLabels.Length; index++)
        {
            GameObject buttonObj = Instantiate(monthButtonPrefab, monthButtonContainer);

            Text label = buttonObj.GetComponentInChildren<Text>();
            if (label != null)
                label.text = monthLabels[index];

            Toggle toggle = buttonObj.GetComponent<Toggle>();
            if (toggle == null)
            {
                Debug.LogError("[CalendarManager] monthButtonPrefab에 Toggle 컴포넌트가 없습니다.");
                continue;
            }

            toggle.group = monthToggleGroup;

            int capturedIndex = index;
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) SelectMonth(capturedIndex);
            });

            if (index == 0)
                toggle.isOn = true;
        }
    }

    // 선택된 월 패널만 켜고 나머지는 전부 끔
    public void SelectMonth(int monthIndex)
    {
        for (int index = 0; index < monthPanels.Length; index++)
        {
            if (monthPanels[index] != null)
                monthPanels[index].SetActive(index == monthIndex);
        }
    }
}