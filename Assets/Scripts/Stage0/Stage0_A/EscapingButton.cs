using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EscapingButton : MonoBehaviour, IPointerClickHandler
{
    [Header("도망 설정")]
    [SerializeField] private float escapeDistance = 500f;   // 이 거리 안에 들어오면 도망
    [SerializeField] private float moveSpeed = 1500f;       // 초기 이동 속도
    [SerializeField] private float damping = 10f;            // 감속 계수 (클수록 빨리 멈춤)

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private Vector2 originalPosition;
    private Vector2 targetPos;          // 목표 랜덤 좌표
    private Vector2 velocity;           // 현재 속도
    private bool isMoving = false;      // 이동 중인지
    private bool isCaught = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        targetPos = originalPosition;
        enabled = false;
    }

    public void StartEscaping()
    {
        transform.SetAsLastSibling();
        isCaught = false;
        enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isCaught)
        {
            // 첫 클릭 - 원위치로 복귀
            isCaught = true;
            velocity = Vector2.zero;
            rectTransform.anchoredPosition = originalPosition;
        }
        else
        {
            // 2026-07-20 임유미 수정
            // 두 번째 클릭 - 스테이지 클리어 처리
            GameManager.GetInstance().OnStageClear();
        }
    }

    void Update()
    {
        if (isCaught) return;

        // 2026-07-20 임유미 추가: 캔버스 렌더링 방식을 오버레이에서 카메라로 바꾸면서 좌표 계산 방식 일부 수정
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out mousePos
        );

        // MainPanel 로컬 좌표로 변환
        Vector2 buttonPos = rectTransform.anchoredPosition;
        Vector3 worldPos = rectTransform.parent.TransformPoint(new Vector3(buttonPos.x, buttonPos.y, 0));
        Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(cam, worldPos), // 2026-07-20 임유미 수정: null → cam
            cam,
            out canvasLocalPos
    );

        float distance = Vector2.Distance(mousePos, canvasLocalPos);

        //// 2026-07-20 임유미 추가: 디버깅
        //// ===== 디버깅 로그 =====
        //Debug.Log(
        //    $"[EscapingButton] " +
        //    $"renderMode: {canvas.renderMode} | " +
        //    $"Camera.main: {(Camera.main != null ? Camera.main.name : "NULL")} | " +
        //    $"Input.mousePosition: {Input.mousePosition} | " +
        //    $"mousePos(local): {mousePos} | " +
        //    $"buttonPos(anchored): {buttonPos} | " +
        //    $"worldPos: {worldPos} | " +
        //    $"canvasLocalPos: {canvasLocalPos} | " +
        //    $"distance: {distance:F1} | " +
        //    $"escapeDistance: {escapeDistance} | " +
        //    $"isMoving: {isMoving}"
        //);
        //// =====================

        if (distance < escapeDistance)
        {
            if (!isMoving) 
            {
                targetPos = GetRandomCanvasPos();
                velocity = (targetPos - buttonPos).normalized * moveSpeed;
                isMoving = true;
            }
        }

        if (isMoving)
        {
            // 감속하면서 목표 위치로 이동
            velocity = Vector2.Lerp(velocity, Vector2.zero, damping * Time.deltaTime);
            Vector2 newPos = buttonPos + velocity * Time.deltaTime;
            newPos = ClampToCanvas(newPos);
            rectTransform.anchoredPosition = newPos;

            // 목표 위치 근처에 도달하면 멈춤
            if (Vector2.Distance(newPos, targetPos) < 10f || velocity.magnitude < 10f)
            {

                isMoving = false;
                velocity = Vector2.zero;
            }
        }
    }

    private Vector2 GetRandomCanvasPos()
    {
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 buttonHalf = rectTransform.sizeDelta * 0.5f;
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out mousePos
        );

        Vector2 randomCanvasPos;
        int maxTry = 20;    // 무한 루프 방지를 위한 횟수 제한

        do
        {
            randomCanvasPos = new Vector2(
                Random.Range(-canvasSize.x * 0.5f + buttonHalf.x, canvasSize.x * 0.5f - buttonHalf.x),
                Random.Range(-canvasSize.y * 0.5f + buttonHalf.y, canvasSize.y * 0.5f - buttonHalf.y)
            );
            maxTry--;
        }
        while (Vector2.Distance(randomCanvasPos, mousePos) < escapeDistance * 2f && maxTry > 0);
        // 마우스에서 escapeDistance*2 이상 떨어진 곳으로만 이동

        Vector3 worldPos = canvasRect.TransformPoint(new Vector3(randomCanvasPos.x, randomCanvasPos.y, 0));
        return rectTransform.parent.InverseTransformPoint(worldPos);
    }

    private Vector2 ClampToCanvas(Vector2 pos)
    {
        // 2026-07-20 임유미 추가
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

        Vector3 worldPos = rectTransform.parent.TransformPoint(new Vector3(pos.x, pos.y, 0));
        Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(cam, worldPos), // 2026-07-20 임유미 수정: null → cam
            cam,
            out canvasLocalPos
    );

        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 buttonHalf = rectTransform.sizeDelta * 0.5f;

        canvasLocalPos.x = Mathf.Clamp(canvasLocalPos.x, -canvasSize.x * 0.5f + buttonHalf.x, canvasSize.x * 0.5f - buttonHalf.x);
        canvasLocalPos.y = Mathf.Clamp(canvasLocalPos.y, -canvasSize.y * 0.5f + buttonHalf.y, canvasSize.y * 0.5f - buttonHalf.y);

        Vector3 clampedWorld = canvasRect.TransformPoint(new Vector3(canvasLocalPos.x, canvasLocalPos.y, 0));
        return rectTransform.parent.InverseTransformPoint(clampedWorld);
    }
}