using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform panelRect;

    private Vector2 dragOffset;
    private RectTransform parentRect;

    void Start()
    {
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

        // 화면 밖 이탈 방지
        Vector2 parentSize = parentRect.rect.size;
        Vector2 panelSize = panelRect.rect.size;

        float minX = -parentSize.x / 2 + panelSize.x / 2;
        float maxX = parentSize.x / 2 - panelSize.x / 2;
        float minY = -parentSize.y / 2 + panelSize.y / 2;
        float maxY = parentSize.y / 2 - panelSize.y / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        panelRect.anchoredPosition = targetPos;
    }
}