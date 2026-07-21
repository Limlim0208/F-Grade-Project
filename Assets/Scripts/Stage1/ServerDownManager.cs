using UnityEngine;
using UnityEngine.UI;

public class ServerDownManager : MonoBehaviour
{
    public Button refreshButton;
    public GameObject shutdown;   // 수정

    void Start()
    {
        shutdown.SetActive(false);   // 수정
        refreshButton.onClick.AddListener(OnRefreshClicked);
    }

    void OnRefreshClicked()
    {
        shutdown.SetActive(false);
    }
}