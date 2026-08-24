using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform panelRect;       // 실제로 이동시키는 대상 (그림자/여백 포함, 화면 밖으로 나갈 수 있음)
    private float minVisibleWidth = 100f;   // 좌우 최소 노출 폭

    private RectTransform selfRect;   // 이 스크립트가 붙은 오브젝트 (하단 이탈 방지 기준)
    private Vector2 dragOffset;
    private RectTransform parentRect;

    void Start()
    {
        selfRect = transform as RectTransform;
        parentRect = panelRect.parent as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        dragOffset = panelRect.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        Vector2 targetPos = localPoint + dragOffset;

        Vector2 parentSize = parentRect.rect.size;
        Vector2 panelSize = panelRect.rect.size;

        // 상단: panelRect 기준, 화면 위로 이탈 불가 (기존과 동일)
        float maxY = parentSize.y / 2 - panelSize.y / 2;
        targetPos.y = Mathf.Min(targetPos.y, maxY);

        // 좌/우: panelRect 기준, minVisibleWidth만 화면에 걸치면 됨
        float minX = -parentSize.x / 2 + minVisibleWidth - panelSize.x / 2;
        float maxX = parentSize.x / 2 - minVisibleWidth + panelSize.x / 2;
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

        // 하단: selfRect(스크립트 붙은 오브젝트)의 높이만큼만 화면 안에 있으면 됨
        float deltaY = targetPos.y - panelRect.anchoredPosition.y;

        Vector3[] selfCorners = new Vector3[4];
        selfRect.GetWorldCorners(selfCorners); // 0번 = 좌하단(bottom-left)
        float selfBottomLocalY = parentRect.InverseTransformPoint(selfCorners[0]).y;
        float projectedSelfBottomY = selfBottomLocalY + deltaY;

        float parentBottomY = -parentSize.y / 2;
        if (projectedSelfBottomY < parentBottomY)
        {
            targetPos.y += parentBottomY - projectedSelfBottomY; // 부족한 만큼 위로 보정
        }

        panelRect.anchoredPosition = targetPos;
    }
}