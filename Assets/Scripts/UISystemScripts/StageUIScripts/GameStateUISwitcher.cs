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

    // 패널 조작 로직
    [System.Serializable]
    private struct StatePanel
    {
        public GameManager.GameState state;
        public GameObject panel;
    }

    [SerializeField] private StatePanel[] statePanels;


    // 파티클 애니메이션 그룹 조작 로직
    [System.Serializable]
    private struct StateAnimationGroup
    {
        public GameManager.GameState state;
        public GameObject animationRoot; // 파티클들을 담고 있는 부모 오브젝트 (예: "Animation")
    }

    [SerializeField] private StateAnimationGroup[] stateAnimationGroups;

    void Awake()
    {
        Debug.Log($"[UIStateSwitcher] Awake 호출됨: {gameObject.name}");
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        // 같은 패널이 여러 state에 등록된 경우를 대비해, 패널별로 "켜져야 하는지" 여부를 먼저 취합
        var shouldActivateMap = new Dictionary<GameObject, bool>();

        foreach (var entry in statePanels)
        {
            bool matches = entry.state == current;

            if (!shouldActivateMap.ContainsKey(entry.panel))
                shouldActivateMap[entry.panel] = matches;
            else
                shouldActivateMap[entry.panel] |= matches; // 하나라도 true면 true 유지
        }

        foreach (var pair in shouldActivateMap)
        {
            pair.Key.SetActive(pair.Value);
        }

        foreach (var entry in stateAnimationGroups)
        {
            bool shouldPlay = entry.state == current;
            entry.animationRoot.SetActive(shouldPlay);

            if (shouldPlay)
            {
                var particles = entry.animationRoot.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var particle in particles)
                    particle.Play();
            }
        }
    }

}