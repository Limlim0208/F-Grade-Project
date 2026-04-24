using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform panelRect;

    private Vector2 dragOffset;

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
        panelRect.anchoredPosition = localPoint + dragOffset;
    }
}