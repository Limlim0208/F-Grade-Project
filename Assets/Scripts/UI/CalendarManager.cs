using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    [Header("Month Selector (달력 상단)")]
    [SerializeField] private Transform monthButtonContainer;
    [SerializeField] private GameObject monthButtonPrefab;
    [SerializeField] private ToggleGroup monthToggleGroup;

    [Header("Month Panels (index 12개 미리 배치, 이미지 + 이벤트 등 각각 다르게 구성)")]
    [SerializeField] private GameObject[] monthPanels = new GameObject[12];

    [Header("Month Labels (버튼 텍스트)")]
    [SerializeField]
    private string[] monthLabels =
    {
        "1월", "2월", "3월", "4월", "5월", "6월",
        "7월", "8월", "9월", "10월", "11월", "12월"
    };

    // 스테이지 번호(CurrentStage) -> 보여줄 달 인덱스(0=1월) 매핑
    // 예: 스테이지0 -> 1월, 스테이지1 -> 2월, 스테이지2 -> 4월
    [Header("Stage -> Month 매핑")]
    [SerializeField]
    private int[] stageToMonthIndex = { 0, 1, 3 };

    void Start()
    {
        GenerateMonthButtons();
    }

    private int GetInitialMonthIndex()
    {
        int currentStage = StageProgressManager.GetInstance().CurrentStage;

        if (stageToMonthIndex == null || stageToMonthIndex.Length == 0)
            return 0;

        int clampedStage = Mathf.Clamp(currentStage, 0, stageToMonthIndex.Length - 1);
        return stageToMonthIndex[clampedStage];
    }

    private void GenerateMonthButtons()
    {
        int initialMonth = GetInitialMonthIndex();

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

            if (index == initialMonth)
                toggle.isOn = true;
        }
    }

    // 선택된 달 패널만 켜고 나머지는 끄기
    public void SelectMonth(int monthIndex)
    {
        for (int index = 0; index < monthPanels.Length; index++)
        {
            if (monthPanels[index] != null)
                monthPanels[index].SetActive(index == monthIndex);
        }
    }
}
