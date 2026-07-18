using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2026-07-17 임유미 추가
/// 스테이지 클리어/게임오버 시 결과 패널(성공/실패)을 전환하는 전용 매니저.
/// GameStateUISwitcher로 대체 가능한 범용 케이스와 달리, 결과 화면에 특화된
/// 추가 로직(예: 팝업 연출, 결과별 개별 처리)이 필요할 때 사용하기 위해 보류 중.
/// 현재는 미사용 상태이며 필요 시 활성화하여 사용.
/// </summary>
public class StageResultUIManager : MonoBehaviour
{
    [Header("결과 패널")]
    [SerializeField] private GameObject stageClearPanel;
    [SerializeField] private GameObject gameOverPanel;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        switch (current)
        {
            case GameManager.GameState.StageClear:
                ShowOnly(stageClearPanel);
                break;

            case GameManager.GameState.GameOver:
                ShowOnly(gameOverPanel);
                break;

            case GameManager.GameState.Playing:
                // Playing으로 돌아왔다는 건 재시작 등으로 결과 화면을 벗어난 것 → 둘 다 닫기
                HideAll();
                break;
        }
    }

    // 결과 패널 중 하나만 켜고 나머지는 확실히 꺼줌 (동시에 두 개가 떠 있는 상황 방지)
    void ShowOnly(GameObject panelToShow)
    {
        stageClearPanel.SetActive(panelToShow == stageClearPanel);
        gameOverPanel.SetActive(panelToShow == gameOverPanel);
    }

    void HideAll()
    {
        stageClearPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }
}