using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2026-07-17 임유미 추가
/// GameManager의 상태(GameState) 변화에 맞춰, 인스펙터에 등록된 여러 패널(GameObject) 중
/// 현재 상태와 일치하는 패널만 활성화하고 나머지는 전부 비활성화하는 범용 스위처.
/// 헤더, 메인 패널 등 "상태별로 UI 구조 자체가 통째로 바뀌는" 요소에 사용.
/// 상태-패널 매핑은 코드 수정 없이 인스펙터의 배열에서 추가/변경 가능.
/// </summary>

public class GameStateUISwitcher : MonoBehaviour
{
    [System.Serializable]
    private struct StatePanel
    {
        public GameManager.GameState state;
        public GameObject panel;
    }

    [SerializeField] private StatePanel[] statePanels;

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
        foreach (var entry in statePanels)
        {
            entry.panel.SetActive(entry.state == current);
        }
    }
}