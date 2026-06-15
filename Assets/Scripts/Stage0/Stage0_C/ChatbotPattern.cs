using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatbotPattern : MonoBehaviour
{
    [Header("챗봇")]
    [SerializeField] private RectTransform chatbot;
    [SerializeField] private GameObject speechBubble;

    [Header("말풍선 콘텐츠")]
    [SerializeField] private GameObject buttonContent;
    [SerializeField] private GameObject scrollContent;
    [SerializeField] private GameObject inputContent;

    [Header("말풍선 이동 설정")]
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float moveSpeed = 1f;

    [Header("클리어 버튼")]
    [SerializeField] private GameObject clearButton;

    private Vector2 originalPosition;
    private ChatbotManager chatbotManager;

    // 시퀀스 활성화 여부
    private bool isActive = false;

    void Start()
    {
        speechBubble.SetActive(false);
        buttonContent.SetActive(false);
        scrollContent.SetActive(false);

        originalPosition = chatbot.anchoredPosition;

        chatbotManager = GetComponent<ChatbotManager>();
    }

    // 2026-05-06 수정: LogicTrigger에서 호출
    public void StartSequence()
    {
        isActive = true;
        StartCoroutine(ChatbotSequence());
    }

    // 패턴 시작 안 했을 때 챗봇 아이콘 클릭 반응 막기
    public void OnChatbotButtonClicked()
    {
        if (!isActive) return;
        chatbotManager.ShowDialogue(0);
    }

    //  챗봇 이동 → 말풍선 표시  시퀀스 코루틴
    private IEnumerator ChatbotSequence()
    {
        speechBubble.SetActive(false);
        yield return StartCoroutine(MoveChatbot());

        if (isActive)
        {
            // 2026-06-15 디버깅 중(임유미 수정)
            chatbotManager.ShowDialogue(1);
        }
    }

    IEnumerator MoveChatbot()
    {
        while (Vector2.Distance(chatbot.anchoredPosition, targetPosition) > 0.5f)
        {
            chatbot.anchoredPosition = Vector2.MoveTowards(
                chatbot.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        chatbot.anchoredPosition = targetPosition;
    }

    // 말풍선 표시 함수
    public void ShowSpeechBubble()
    {
        speechBubble.SetActive(true);
        buttonContent.SetActive(true);
        scrollContent.SetActive(false);
        inputContent.SetActive(false);
        StartCoroutine(RebuildAll());
    }

    // ChatbotManager에서 contentType == "scroll"일 때 호출
    public void ShowScrollView()
    {
        buttonContent.SetActive(false);
        scrollContent.SetActive(true);
        inputContent.SetActive(false);
        StartCoroutine(RebuildAll());
    }

    // ChatbotManager에서 contentType == "input"일 때 호출
    public void ShowInputView()
    {
        buttonContent.SetActive(true);
        scrollContent.SetActive(false);
        inputContent.SetActive(true);
        StartCoroutine(RebuildAll());
    }

    // 챗봇 리셋 로직(아직 사용 X)
    //public void ResetChatbot()
    //{
    //    // 리셋 시 시퀀스 비활성화
    //    isActive = false;

    //    speechBubble.SetActive(false);
    //    buttonContent.SetActive(false);
    //    scrollContent.SetActive(false);
    //    inputContent.SetActive(false);
    //    chatbot.anchoredPosition = originalPosition;
    //}

    // UI 리빌드 함수
    IEnumerator RebuildAll()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(speechBubble.GetComponent<RectTransform>());
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(speechBubble.GetComponent<RectTransform>());
    }

    // 챗봇 삭제 버튼 클릭 시 패턴 클리어
    public void ClearPattern()
    {
        Destroy(chatbot.gameObject);

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

}