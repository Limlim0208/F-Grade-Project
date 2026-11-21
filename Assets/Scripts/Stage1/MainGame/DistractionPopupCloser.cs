using UnityEngine;
using UnityEngine.UI;

public class DistractionPopupCloser : MonoBehaviour
{
    [Header("닫기 버튼들 (Inspector에서 연결)")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton; // X 버튼

    void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ClosePopup);
        else
            Debug.LogWarning($"[DistractionPopupCloser] {name}: confirmButton 미연결");

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
        else
            Debug.LogWarning($"[DistractionPopupCloser] {name}: closeButton(X) 미연결");
    }

    private void ClosePopup()
    {
        Destroy(gameObject);
    }
}