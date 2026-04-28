using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonRandomMove : MonoBehaviour, IPointerClickHandler
{
    [Header("연결 오브젝트")]
    [SerializeField] private RectTransform chatbotButton;
    [SerializeField] private Image chatbotImage;

    [Header("설정")]
    [SerializeField] private int randomMoveCount = 3;
    [SerializeField] private int clicksToDisappear = 5;

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 originalPosition;

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
                // 랜덤 위치로 이동
                Vector2 canvasSize = canvasRect.rect.size;
                Vector2 randomPos = new Vector2(
                    Random.Range(-canvasSize.x * 0.4f, canvasSize.x * 0.4f),
                    Random.Range(-canvasSize.y * 0.4f, canvasSize.y * 0.4f)
                );
                rectTransform.anchoredPosition = randomPos;
            }
            else if (clearClickCount >= randomMoveCount)
            {
                // ChatbotButton 위치로 이동 - 추후 숨는 위치 수정 예정
                rectTransform.position = chatbotButton.position;
                isHiding = true;
                isActive = false;
            }
            return;
        }

        if (isRevealed)
        {
            // 원위치로 복귀
            rectTransform.anchoredPosition = originalPosition;
            isRevealed = false;
            chatbotClickCount = 0;

            // ChatbotButton 복구
            chatbotButton.gameObject.SetActive(true);
            Color c = chatbotImage.color;
            c.a = 1f;
            chatbotImage.color = c;
        }
    }

    private void OnChatbotClicked()
    {
        if (!isHiding) return;

        chatbotClickCount++;
        float alpha = 1f - ((float)chatbotClickCount / clicksToDisappear);
        alpha = Mathf.Clamp01(alpha);

        Color c = chatbotImage.color;
        c.a = alpha;
        chatbotImage.color = c;

        if (chatbotClickCount >= clicksToDisappear)
        {
            chatbotButton.gameObject.SetActive(false);
            isHiding = false;
            isRevealed = true;
        }
    }
}