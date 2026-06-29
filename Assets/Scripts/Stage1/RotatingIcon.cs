using UnityEngine;
using UnityEngine.EventSystems;

public class RotatingIcon : MonoBehaviour, IPointerClickHandler
{
    public float rotationSpeed;
    public bool isFastest;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MinigameManager.Instance.OnIconClicked(isFastest);
    }
}