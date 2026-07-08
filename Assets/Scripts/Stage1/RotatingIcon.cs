using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotatingIcon : MonoBehaviour, IPointerClickHandler
{
    public float rotationSpeed;
    public bool isFastest;

    [Header("°øÀü")]
    public float orbitRadius;
    public float orbitSpeed;
    public float orbitAngle;
    public RectTransform spawnArea;

    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();

        Vector2 center = Vector2.zero;
        orbitRadius = Vector2.Distance(rt.anchoredPosition, center);

        Vector2 dir = rt.anchoredPosition - center;
        orbitAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        orbitAngle += orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * orbitRadius;
        float y = Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * orbitRadius;
        rt.anchoredPosition = new Vector2(x, y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MinigameManager.Instance.OnIconClicked(isFastest);
    }
}