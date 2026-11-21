using UnityEngine;
using UnityEngine.UI;

public class Stage1GameClearPanelCaller : MonoBehaviour
{
    [Header("클리어 UI 패널 (Inspector에서 연결)")]
    [SerializeField] private GameObject clearPanel;

    [Header("패널 내부 확인 버튼 (Inspector에서 연결)")]
    [SerializeField] private Button confirmButton;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;

        if (clearPanel != null)
            clearPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        else
            Debug.LogWarning("[GameClearPanelCaller] confirmButton이 Inspector에 연결되어 있지 않음");
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        if (current == GameManager.GameState.StageClear)
        {
            if (clearPanel != null)
                clearPanel.SetActive(true);
            else
                Debug.LogWarning("[GameClearPanelCaller] clearPanel이 Inspector에 연결되어 있지 않음");
        }
    }

    private void OnConfirmButtonClicked()
    {
        if (clearPanel != null)
            //clearPanel.SetActive(false);

        StageProgressManager.GetInstance().LoadNextStage();
    }
}