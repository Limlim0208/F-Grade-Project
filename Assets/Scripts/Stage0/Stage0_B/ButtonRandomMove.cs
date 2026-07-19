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
    [SerializeField] private float timePenalty = 10f; // 감소할 시간

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
    private bool isReturnedToOrigin = false;
    private int originalSiblingIndex;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        isReturnedToOrigin = false;

        chatbotCanvasGroup = chatbotButton.gameObject.GetComponent<CanvasGroup>();
        if (chatbotCanvasGroup == null)
            chatbotCanvasGroup = chatbotButton.gameObject.AddComponent<CanvasGroup>();

        chatbotButton.GetComponentInChildren<Button>().onClick.AddListener(OnChatbotClicked); // 임유미 수정: Chatbot 오브젝트 구조 변경
        originalSiblingIndex = transform.GetSiblingIndex(); // 우선순위 기억
    }

    public void StartSequence()
    {
        isActive = true;
        clearClickCount = 0;
        isReturnedToOrigin = false;
    }

    void Update()
    {
        // 랜덤 이동 단계일 때만 페널티 적용
        if (!isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            bool clickedOnClear = false;
            foreach (var result in results)
            {
                if (result.gameObject == gameObject)
                {
                    clickedOnClear = true;
                    break;
                }
            }

            if (!clickedOnClear)
                TimerManager.GetInstance().ReduceTime(timePenalty);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive && !isHiding && !isRevealed)
        {
            clearClickCount++;

            if (clearClickCount < randomMoveCount)
            {
                transform.SetAsLastSibling(); // 랜덤 좌표로 이동하는 동안 ClearButton이 최상단으로 올라오게 함

                Vector2 canvasSize = canvasRect.rect.size;
                Vector2 randomCanvasPos = new Vector2(
                    Random.Range(-canvasSize.x * 0.4f, canvasSize.x * 0.4f),
                    Random.Range(-canvasSize.y * 0.4f, canvasSize.y * 0.4f)
                );

                // Canvas 좌표 기준
                Vector3 worldPos = canvasRect.TransformPoint(new Vector3(randomCanvasPos.x, randomCanvasPos.y, 0));
                Vector2 localPos = rectTransform.parent.InverseTransformPoint(worldPos);
                rectTransform.anchoredPosition = localPos;
            }
            else if (clearClickCount >= randomMoveCount)
            {
                transform.SetSiblingIndex(originalSiblingIndex); // 원래 우선순위로 복귀
                rectTransform.position = chatbotButton.position + new Vector3(0, 100f, 0);
                isHiding = true;
                isActive = false;
            }
            return;
        }

        if (isRevealed)
        {
            if (!isReturnedToOrigin)
            {
                // 첫 클릭 - 원위치로 복귀
                rectTransform.anchoredPosition = originalPosition;
                isReturnedToOrigin = true;
            }
            else
            {
                // 2026-07-20 임유미 수정
                // 두 번째 클릭 - 스테이지 클리어 처리
                GameManager.GetInstance().OnStageClear();
            }
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