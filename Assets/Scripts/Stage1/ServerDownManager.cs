using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ServerDownManager : MonoBehaviour
{
    public Button refreshButton;
    public GameObject shutdown;   // ����
    public GameObject serverWaitingPopup;

    void Start()
    {
        shutdown.SetActive(false);   // ����
        refreshButton.onClick.AddListener(OnRefreshClicked);
    }

    void OnRefreshClicked()
    {
        shutdown.SetActive(false);
        StartCoroutine(ShowServerWaitingAfterDelay());
    }

    IEnumerator ShowServerWaitingAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        serverWaitingPopup.SetActive(true);
    }
}