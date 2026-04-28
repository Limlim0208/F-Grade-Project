using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChatbotManager : MonoBehaviour
{
    [Header("챗봇")]
    [SerializeField] private GameObject chatbotButton; // 챗봇 버튼 전체
    [SerializeField] private TextMeshProUGUI speechBubbleText; // 챗봇 대사 텍스트 영역

    [Header("버튼")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;      // ButtonContent
    [SerializeField] private Transform scrollButtonContainer; // ScrollContent 하위 Content

    private DialogueDatabase database;
    private ChatbotPattern chatbotPattern;
    private int currentIndex = 0;

    void Start()
    {
        chatbotPattern = GetComponent<ChatbotPattern>();
        LoadDialogues();
        ShowDialogue(0);
    }

    void LoadDialogues()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("TextScripts/stage0_chatbot_dialogues");
        database = JsonUtility.FromJson<DialogueDatabase>(jsonFile.text);
    }

    public void ShowDialogue(int index)
    {
        if (index >= database.dialogues.Count)
        {
            Debug.Log("더 이상 대사가 없음");
            return;
        }

        currentIndex = index;
        DialogueEntry entry = database.dialogues[index];

        // 텍스트 업데이트
        speechBubbleText.text = entry.text;
        // contentType에 따라 콘텐츠 전환
        if (entry.contentType == "scroll")
        {
            chatbotPattern.ShowScrollView();
            GenerateScrollButtons(entry.buttons);
        }
        else
        {
            GenerateButtons(entry.buttons);
        }
    }
    // ButtonContent 버튼 생성
    void GenerateButtons(List<ButtonData> buttons)
    {
        // 기존 버튼 전부 삭제
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // JSON 버튼 개수만큼 버튼 생성
        foreach (ButtonData btn in buttons)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

            // 버튼 텍스트 설정
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = btn.label;

            // 버튼 타입에 따라 OnClick 연결
            Button buttonComponent = newButton.GetComponent<Button>();
            string btnType = btn.type; // 클로저 문제 방지용 로컬 변수

            buttonComponent.onClick.AddListener(() =>
            {
                if (btnType == "correct")
                    OnCorrectButtonClicked();
                else if (btnType == "wrong")
                    OnWrongButtonClicked();
                else if (btnType == "patternClear")
                    OnPatternClearButtonClicked();
                else if (btnType == "gameOver")
                    OnGameOverButtonClicked();
            });
        }

        RebuildLayout(buttonContainer);
    }
    // ScrollContent 버튼 생성
    void GenerateScrollButtons(List<ButtonData> buttons)
    {
        foreach (Transform child in scrollButtonContainer)
            Destroy(child.gameObject);

        foreach (ButtonData btn in buttons)
        {
            GameObject newButton = Instantiate(buttonPrefab, scrollButtonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = btn.label;

            Button buttonComponent = newButton.GetComponent<Button>();
            string btnType = btn.type;

            buttonComponent.onClick.AddListener(() =>
            {
                if (btnType == "fake")
                    OnFakeButtonClicked();
                else if (btnType == "patternClear")
                    OnPatternClearButtonClicked();
                else if (btnType == "gameOver")
                    OnGameOverButtonClicked();
            });
        }

        RebuildLayout(scrollButtonContainer);
    }

    // 오답버튼 클릭 시 질문 반복
    public void OnWrongButtonClicked()
    {
        if (currentIndex < 3)
            ShowDialogue(currentIndex + 1);
        else
            ShowDialogue(0);

    }

    // 정답버튼 클릭 시 다음 질문 패턴으로 넘어감
    public void OnCorrectButtonClicked()
    {
        if(currentIndex < 3)
            ShowDialogue(4);
        else if (currentIndex >= 4)
            ShowDialogue(currentIndex + 1);
    }

    public void OnFakeButtonClicked()
    {
        // TODO: 페이크 버튼 클릭 시 특정 스크립트 출력 예정
        Debug.Log("페이크 버튼 클릭");
    }


    // 게임오버 버튼 클릭 시 게임오버
    public void OnGameOverButtonClicked()
    {
        GameManager.GetInstance().OnGameOver();
    }

    /*
    TODO: 페이크 버튼 리스트 눌렀을 때 특정 스크립트 출력하도록 코드 추가해야함
     */


    // 챗봇 삭제 버튼 클릭 시 패턴 클리어
    public void OnPatternClearButtonClicked()
    {
        Destroy(chatbotButton);
        Debug.Log("게임 오버!");
        return;
    }

    // 버튼 생성 시 레이아웃 업데이트가 느려 제대로 말풍선이 그려지지 않는 문제를 해결하기 위한 레이아웃 리빌드 함수...
    void RebuildLayout(Transform container)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
    }

}