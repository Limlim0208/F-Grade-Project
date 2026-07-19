using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

/// <summary>
/// 2026-07-17 임유미 추가
/// GameManager의 상태(GameState) 변화에 맞춰, 대상 Image 컴포넌트의 스프라이트를
/// 인스펙터에 등록된 상태별 스프라이트로 교체하는 범용 스위처.
/// 챗봇 아이콘처럼 "레이아웃은 그대로 두고 이미지 리소스 하나만 바뀌는" 요소에 사용.
/// 챗봇 외에도 스프라이트 교체가 필요한 모든 UI 오브젝트에 재사용 가능.
/// </summary>

public class StateSpriteSwitcher : MonoBehaviour
{
    [System.Serializable]
    private struct StateSprite
    {
        public GameManager.GameState state;
        public Sprite sprite;
    }

    [SerializeField] private Image chatbotImage;
    [SerializeField] private StateSprite[] stateSprites;

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
        foreach (var entry in stateSprites)
        {
            if (entry.state == current)
            {
                chatbotImage.sprite = entry.sprite;
                chatbotImage.SetNativeSize(); // 추가
                return;
            }
        }
        // 매핑에 없는 상태면 기본 스프라이트 유지 (Playing 등)
    }
}