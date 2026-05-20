using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatbotManager : MonoBehaviour
{
    [Header("챗봇")]
    [SerializeField] private GameObject chatbotButton; // 챗봇 버튼 전체
    [SerializeField] private TextMeshProUGUI speechBubbleText; // 챗봇 대사 텍스트 영역

    [Header("버튼")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;      // ButtonContent
    [SerializeField] private Transform scrollButtonContainer; // ScrollContent 하위 Content

    [Header("텍스트 입력창")]
    [SerializeField] private TMP_InputField inputField;

    [Header("클리어 버튼")]
    [SerializeField] private GameObject clearButton;

    private DialogueDatabase database;
    private ChatbotPattern chatbotPattern;
    private Dictionary<int, DialogueEntry> dialogueMap = new Dictionary<int, DialogueEntry>();

    void Start()
    {
        chatbotPattern = GetComponent<ChatbotPattern>();
        LoadDialogues();
        ShowDialogue(1);
    }

    void LoadDialogues()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("TextScripts/stage0_chatbot_dialogues");
        database = JsonUtility.FromJson<DialogueDatabase>(jsonFile.text);

        foreach (DialogueEntry entry in database.dialogues)
            dialogueMap[entry.id] = entry;
    }

    public void ShowDialogue(int id)
    {
        if (!dialogueMap.TryGetValue(id, out DialogueEntry entry))
        {
            Debug.LogWarning($"[ChatbotManager] id {id} 에 해당하는 대화가 없습니다.");
            return;
        }

        // textPool이 있으면 랜덤 pick, 없으면 text 사용
        if (entry.textPool != null && entry.textPool.Count > 0)
            speechBubbleText.text = entry.textPool[Random.Range(0, entry.textPool.Count)];
        else
            speechBubbleText.text = entry.text;

        SetContentUI(entry);
    }

    // ContentType에 따른 UI 표시
    void SetContentUI(DialogueEntry entry)
    {

        switch (entry.contentType)
        {
            case "scroll":
                chatbotPattern.ShowScrollView();
                GenerateButtons(entry.buttons, scrollButtonContainer);
                break;

            case "input":
                chatbotPattern.ShowInputView();
                if (inputField != null) inputField.text = "";
                GenerateButtons(entry.buttons, buttonContainer);
                break;

            default: // null 또는 "question"
                chatbotPattern.ShowSpeechBubble();
                GenerateButtons(entry.buttons, buttonContainer);
                break;
        }
    }

    // ButtonContent 버튼 생성 + ScrollContent 버튼 생성 함수 병합
    void GenerateButtons(List<ButtonData> buttons, Transform container)
    {
        // 기존 버튼 전부 삭제
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // JSON 버튼 개수만큼 버튼 생성
        foreach (ButtonData btn in buttons)
        {
            GameObject newButton = Instantiate(buttonPrefab, container);

            // 버튼 텍스트 설정
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = btn.label;

            // 버튼 타입에 따라 OnClick 연결
            Button buttonComponent = newButton.GetComponent<Button>();
            string btnType = btn.type; // 클로저 문제 방지용 로컬 변수
            int nextId = btn.nextId;

            switch (btnType)
            {
                case "disabled":
                    // 클릭 불가 버튼 — interactable만 끄고 리스너 없음
                    buttonComponent.interactable = false;
                    break;

                case "fake":
                    // 경유 노드로 이동 (nextId 있으면), 없으면 Q1으로
                    buttonComponent.onClick.AddListener(() =>
                        ShowDialogue(nextId > 0 ? nextId : 1));
                    break;

                case "gameOver":
                    buttonComponent.onClick.AddListener(OnGameOverButtonClicked);
                    break;

                case "patternClear":
                    buttonComponent.onClick.AddListener(OnPatternClearButtonClicked);
                    break;

                default:
                    // type 없음 — nextId로 이동
                    if (nextId >= 0)
                        buttonComponent.onClick.AddListener(() => ShowDialogue(nextId));
                    else
                        Debug.LogWarning($"[ChatbotManager] type 없음 + nextId {nextId} — 이동 불가");
                    break;
            }
        }

        StartCoroutine(RebuildLayoutCoroutine(container));
    }

    // 게임오버 버튼 클릭 시 게임오버
    public void OnGameOverButtonClicked()
    {
        GameManager.GetInstance().OnGameOver();
        Debug.Log("게임 오버!");
    }

    // 챗봇 삭제 버튼 클릭 시 패턴 클리어
    public void OnPatternClearButtonClicked()
    {
        Destroy(chatbotButton);
        Debug.Log("패턴 C 클리어!");

        // ClearButton 클릭 시 Stage선택 창 으로 이동 (민채은 수정)
        if (clearButton != null)
        {
            clearButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            clearButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                SceneChanger.GetInstance().LoadScene("StageSelectScene");
            });
        }
    }

    // 버튼 생성 시 레이아웃 업데이트가 느려 제대로 말풍선이 그려지지 않는 문제를 해결하기 위한 레이아웃 리빌드 함수...
    IEnumerator RebuildLayoutCoroutine(Transform container)
    {
        // 1프레임 대기 후 리빌드 (SetActive 직후 레이아웃 계산 타이밍 문제 해결)
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        // 부모까지 한 번 더 리빌드
        if (container.parent != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(container.parent.GetComponent<RectTransform>());

        yield return null;

        // SpeechBubble 최상위까지 리빌드
        Transform root = container.parent?.parent;
        if (root != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(root.GetComponent<RectTransform>());
    }

}