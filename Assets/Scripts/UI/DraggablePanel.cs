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
            parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        dragOffset = (Vector2)panelRect.localPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        Vector2 targetPos = localPoint + dragOffset;

        Rect pr = parentRect.rect;
        Vector2 size = panelRect.rect.size;
        Vector2 pivot = panelRect.pivot;

        // 상단: 패널 윗변이 부모 윗변을 넘지 않게
        float maxY = pr.yMax - size.y * (1f - pivot.y);
        targetPos.y = Mathf.Min(targetPos.y, maxY);

        // 좌우: 패널이 minVisibleWidth 이상 화면에 걸치게
        float minX = pr.xMin + minVisibleWidth - size.x * (1f - pivot.x);
        float maxX = pr.xMax - minVisibleWidth + size.x * pivot.x;
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

        // 하단: selfRect 하단이 부모 하단 아래로 내려가지 않게
        float deltaY = targetPos.y - panelRect.localPosition.y;

        Vector3[] corners = new Vector3[4];
        selfRect.GetWorldCorners(corners);
        float selfBottomY = parentRect.InverseTransformPoint(corners[0]).y;

        float projected = selfBottomY + deltaY;
        if (projected < pr.yMin)
            targetPos.y += pr.yMin - projected;

        panelRect.localPosition = new Vector3(targetPos.x, targetPos.y, panelRect.localPosition.z);
    }
}