using UnityEngine;
using UnityEngine.UI;

public class EscapingButton : MonoBehaviour
{
    [Header("도망 설정")]
    [SerializeField] private float escapeDistance = 150f;  // 이 거리 안에 들어오면 도망
    [SerializeField] private float escapeSpeed = 300f;     // 도망가는 속도

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
   

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

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
        Vector2 canvasHalf = canvasRect.sizeDelta * 0.5f;
        Vector2 buttonHalf = rectTransform.sizeDelta * 0.5f;

        pos.x = Mathf.Clamp(pos.x, -canvasHalf.x + buttonHalf.x, canvasHalf.x - buttonHalf.x);
        pos.y = Mathf.Clamp(pos.y, -canvasHalf.y + buttonHalf.y, canvasHalf.y - buttonHalf.y);
        return pos;
    }
}