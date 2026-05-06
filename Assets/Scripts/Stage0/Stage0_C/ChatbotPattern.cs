using UnityEngine;
using System.Collections;

public class ChatbotPattern : MonoBehaviour
{
    [Header("챗봇")]
    [SerializeField] private RectTransform chatbotButton;
    [SerializeField] private GameObject speechBubble;

    [Header("말풍선 콘텐츠")]
    [SerializeField] private GameObject buttonContent;
    [SerializeField] private GameObject scrollContent;

    [Header("말풍선 이동 설정")]
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float moveSpeed = 1f;

    private Vector2 originalPosition;

    // 시퀀스 활성화 여부
    private bool isActive = false;

    void Start()
    {
        speechBubble.SetActive(false);
        buttonContent.SetActive(false);
        scrollContent.SetActive(false);

        originalPosition = chatbotButton.anchoredPosition;
    }

    // 2026-05-06 수정: LogicTrigger에서 호출
    public void StartSequence()
    {
        isActive = true;
        StartCoroutine(ChatbotSequence());
    }

    //  챗봇 이동 → 말풍선 표시  시퀀스 코루틴
    private IEnumerator ChatbotSequence()
    {
        speechBubble.SetActive(false);
        yield return StartCoroutine(MoveChatbot());

        if (isActive)
        {
            ShowSpeechBubble();
        }
    }

    IEnumerator MoveChatbot()
    {
        while (Vector2.Distance(chatbotButton.anchoredPosition, targetPosition) > 0.5f)
        {
            chatbotButton.anchoredPosition = Vector2.MoveTowards(
                chatbotButton.anchoredPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        chatbotButton.anchoredPosition = targetPosition;
    }

    // 말풍선 표시 함수
    public void ShowSpeechBubble()
    {
        speechBubble.SetActive(true);
        buttonContent.SetActive(true);
        scrollContent.SetActive(false);
    }

    // ChatbotManager에서 contentType == "scroll"일 때 호출
    public void ShowScrollView()
    {
        buttonContent.SetActive(false);
        scrollContent.SetActive(true);
    }

    public void ResetChatbot()
    {
        // 리셋 시 시퀀스 비활성화
        isActive = false;

        speechBubble.SetActive(false);
        buttonContent.SetActive(false);
        scrollContent.SetActive(false);
        chatbotButton.anchoredPosition = originalPosition;
    }
}