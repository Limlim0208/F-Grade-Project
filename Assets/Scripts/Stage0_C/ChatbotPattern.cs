using UnityEngine;
using System.Collections;

public class ChatbotPattern : MonoBehaviour
{
    [Header("Ãªº¿")]
    [SerializeField] private RectTransform chatbotButton;
    [SerializeField] private GameObject speechBubble;

    [Header("¸»Ç³¼± ÄÜÅÙÃ÷")]
    [SerializeField] private GameObject buttonContent;
    [SerializeField] private GameObject scrollContent;

    [Header("¸»Ç³¼± ÀÌµ¿ ¼³Á¤")]
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float moveSpeed = 1f;

    private Vector2 originalPosition;

    void Start()
    {
        speechBubble.SetActive(false);
        buttonContent.SetActive(true);
        scrollContent.SetActive(false);

        originalPosition = chatbotButton.anchoredPosition;

        StartCoroutine(MoveChatbot()); // Ãªº¿ ÀÌµ¿
        ShowSpeechBubble(); // ±âº» ¸»Ç³¼± Ç¥½Ã
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

    // ¸»Ç³¼± Ç¥½Ã ÇÔ¼ö
    public void ShowSpeechBubble()
    {
        speechBubble.SetActive(true);
        buttonContent.SetActive(true);
        scrollContent.SetActive(false);
    }

    // ChatbotManager¿¡¼­ contentType == "scroll"ÀÏ ¶§ È£Ãâ
    public void ShowScrollView()
    {
        buttonContent.SetActive(false);
        scrollContent.SetActive(true);
    }

    public void ResetChatbot()
    {
        speechBubble.SetActive(false);
        buttonContent.SetActive(true);
        scrollContent.SetActive(false);
        chatbotButton.anchoredPosition = originalPosition;
    }
}