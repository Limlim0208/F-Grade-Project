using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EscapingButton : MonoBehaviour, IPointerClickHandler
{
    [Header("도망 설정")]
    [SerializeField] private float escapeDistance = 150f;  // 이 거리 안에 들어오면 도망
    [SerializeField] private float escapeSpeed = 300f;     // 도망가는 속도

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 originalPosition; 
    private bool isCaught = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        enabled = false;
    }

    public void StartEscaping()
    {
        enabled = true;
    }

    // 버튼 잡았을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        isCaught = true;
        enabled = false;
        rectTransform.anchoredPosition = originalPosition; // 원위치로 복귀
    }

    void Update()
    {
        Vector2 mousePos;
        // 마우스 위치를 캔버스 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out mousePos
        );

        Vector2 buttonPos = rectTransform.anchoredPosition;
        float distance = Vector2.Distance(mousePos, buttonPos);

        if (distance < escapeDistance)
        {
            // 마우스 반대 방향으로 도망
            Vector2 escapeDir = (buttonPos - mousePos).normalized;
            Vector2 newPos = buttonPos + escapeDir * escapeSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = ClampToCanvas(newPos);
        }
    }

    // 캔버스 영역 안으로 위치 제한
    private Vector2 ClampToCanvas(Vector2 pos)
    {
        // (260506 수정)월드 좌표 기준으로 버튼 경계 지정
        Vector3 worldPos = rectTransform.parent.TransformPoint(new Vector3(pos.x, pos.y, 0));
        Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(null, worldPos),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out canvasLocalPos
        );

        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 buttonHalf = rectTransform.sizeDelta * 0.5f;

        canvasLocalPos.x = Mathf.Clamp(canvasLocalPos.x, -canvasSize.x * 0.5f + buttonHalf.x, canvasSize.x * 0.5f - buttonHalf.x);
        canvasLocalPos.y = Mathf.Clamp(canvasLocalPos.y, -canvasSize.y * 0.5f + buttonHalf.y, canvasSize.y * 0.5f - buttonHalf.y);

        // 다시 MainPanel 로컬 좌표로 변환 후 반환
        Vector3 clampedWorld = canvasRect.TransformPoint(new Vector3(canvasLocalPos.x, canvasLocalPos.y, 0));
        Vector2 result = rectTransform.parent.InverseTransformPoint(clampedWorld);
        return result;
    }
}