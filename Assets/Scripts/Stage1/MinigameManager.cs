using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [Header("참조")]
    public IconSpawner spawner;
    public GameObject resultPanel;
    public GameObject minigamePopup;
    public GameObject verifiedPopup;
    public GameObject shutdown;
    public GameObject overlayImage;

    [Header("연출")]
    public float failRestartDelay = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        StartGame();
    }

    public void StartGame()
    {
        resultPanel.SetActive(false);
        spawner.SpawnIcons();
    }

    public void OnIconClicked(bool isFastest)
    {
        if (isFastest)
        {
            OnSuccess();
        }
        else
        {
            StartCoroutine(OnFail());
        }
    }

    void OnSuccess()
    {
        minigamePopup.SetActive(false);
        StartCoroutine(ShowVerifiedPopup());
    }

    IEnumerator OnFail()
    {
        resultPanel.SetActive(true);
        yield return new WaitForSeconds(failRestartDelay);

        StartGame();
    }

    IEnumerator ShowVerifiedPopup()
    {
        verifiedPopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        verifiedPopup.SetActive(false);
        overlayImage.SetActive(false);   // 추가
        shutdown.SetActive(true);
    }
}