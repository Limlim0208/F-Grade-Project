using UnityEngine;
using UnityEngine.UI;

public class ServerDownManager : MonoBehaviour
{
    public Button refreshButton;
    public GameObject shutdown;

    private void Start()
    {
        shutdown.SetActive(false);
        refreshButton.onClick.AddListener(OnRefreshClicked);
    }

    private void OnRefreshClicked()
    {
        shutdown.SetActive(false);   // 메인게임은 뒤에서 계속 켜져 있으므로 바로 복귀
    }
}