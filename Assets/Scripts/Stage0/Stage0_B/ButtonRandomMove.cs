using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonRandomMove : MonoBehaviour, IPointerClickHandler
{
    [Header("숨을 위치 오브젝트")]
    [SerializeField] private RectTransform logicB;
    [SerializeField] private Vector2 hideOffset = Vector2.zero;
    [Header("설정")]
    [SerializeField] private int randomMoveCount = 3;
    [SerializeField] private int clicksToDisappear = 5;
    [SerializeField] private float timePenalty = 10f; // 감점될 시간

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 originalPosition;
    private CanvasGroup logicBCanvasGroup;

    private int clearClickCount = 0;
    private int logicBClickCount = 0;
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

        logicBCanvasGroup = logicB.gameObject.GetComponent<CanvasGroup>();
        if (logicBCanvasGroup == null)
            logicBCanvasGroup = logicB.gameObject.AddComponent<CanvasGroup>();

        logicB.GetComponentInChildren<Button>().onClick.AddListener(OnLogicBClicked);
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
        // 랜덤 이동 단계가 아니면 무시
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
                transform.SetAsLastSibling(); // 랜덤 좌표로 이동하는 동안 ClearButton을 최상단으로 올리기 위함

                Vector2 canvasSize = canvasRect.rect.size;
                Vector2 randomCanvasPos = new Vector2(
                    Random.Range(-canvasSize.x * 0.4f, canvasSize.x * 0.4f),
                    Random.Range(-canvasSize.y * 0.4f, canvasSize.y * 0.4f)
                );

                // Canvas 좌표 변환
                Vector3 worldPos = canvasRect.TransformPoint(new Vector3(randomCanvasPos.x, randomCanvasPos.y, 0));
                Vector2 localPos = rectTransform.parent.InverseTransformPoint(worldPos);
                rectTransform.anchoredPosition = localPos;
            }
            else if (clearClickCount >= randomMoveCount)
            {
                transform.SetSiblingIndex(originalSiblingIndex); // 원래 우선순위로 복구
                rectTransform.position = logicB.position + (Vector3)hideOffset; // LogicB 오브젝트 뒤에 숨는 위치로 이동
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
                // 두 번째 클릭 - 스테이지 클리어 처리
                GameManager.GetInstance().OnStageClear();
            }
        }
    }

    private void OnLogicBClicked()
    {
        if (!isHiding) return;

        logicBClickCount++;
        float alpha = 1f - ((float)logicBClickCount / clicksToDisappear);
        logicBCanvasGroup.alpha = Mathf.Clamp01(alpha);

        if (logicBClickCount >= clicksToDisappear)
        {
            logicB.gameObject.SetActive(false);
            isHiding = false;
            isRevealed = true;
        }
    }
}
