using UnityEngine;
using UnityEngine.UI;

public class Stage1GameOverPanelCaller : MonoBehaviour
{
    [Header("게임오버 UI 패널 (Inspector에서 연결)")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("패널 내부 확인 버튼 (Inspector에서 연결)")]
    [SerializeField] private Button confirmButton;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        else
            Debug.LogWarning("[GameOverPanelCaller] confirmButton이 Inspector에 연결되어 있지 않음");
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        if (current == GameManager.GameState.GameOver)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
            else
                Debug.LogWarning("[GameOverPanelCaller] gameOverPanel이 Inspector에 연결되어 있지 않음");
        }
    }

    private void OnConfirmButtonClicked()
    {
        if (gameOverPanel != null)
            //gameOverPanel.SetActive(false);

        StageProgressManager.GetInstance().OnStageFailed();
    }
}