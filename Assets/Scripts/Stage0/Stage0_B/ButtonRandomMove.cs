using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonRandomMove : MonoBehaviour, IPointerClickHandler
{
    [Header("연결 오브젝트")]
    [SerializeField] private RectTransform chatbotButton;

    [Header("설정")]
    [SerializeField] private int randomMoveCount = 3;
    [SerializeField] private int clicksToDisappear = 5;

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 originalPosition;
    private CanvasGroup chatbotCanvasGroup;

    private int clearClickCount = 0;
    private int chatbotClickCount = 0;
    private bool isHiding = false;
    private bool isRevealed = false;
    private bool isActive = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        chatbotCanvasGroup = chatbotButton.gameObject.GetComponent<CanvasGroup>();
        if (chatbotCanvasGroup == null)
            chatbotCanvasGroup = chatbotButton.gameObject.AddComponent<CanvasGroup>();

        chatbotButton.GetComponent<Button>().onClick.AddListener(OnChatbotClicked);
    }

    public void StartSequence()
    {
        isActive = true;
        clearClickCount = 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive && !isHiding && !isRevealed)
        {
            clearClickCount++;

            if (clearClickCount < randomMoveCount)
            {
                Vector2 canvasSize = canvasRect.rect.size;
                Vector2 randomPos = new Vector2(
                    Random.Range(-canvasSize.x * 0.4f, canvasSize.x * 0.4f),
                    Random.Range(-canvasSize.y * 0.4f, canvasSize.y * 0.4f)
                );
                rectTransform.anchoredPosition = randomPos;
            }
            else if (clearClickCount >= randomMoveCount)
            {
                rectTransform.position = chatbotButton.position;
                isHiding = true;
                isActive = false;
            }
            return;
        }

        if (isRevealed)
        {
            rectTransform.anchoredPosition = originalPosition;
            isRevealed = false;
            chatbotClickCount = 0;

            chatbotButton.gameObject.SetActive(true);
            chatbotCanvasGroup.alpha = 1f;
        }
    }

    private void OnChatbotClicked()
    {
        if (!isHiding) return;

        chatbotClickCount++;
        float alpha = 1f - ((float)chatbotClickCount / clicksToDisappear);
        chatbotCanvasGroup.alpha = Mathf.Clamp01(alpha);

        if (chatbotClickCount >= clicksToDisappear)
        {
            chatbotButton.gameObject.SetActive(false);
            isHiding = false;
            isRevealed = true;
        }
    }
}