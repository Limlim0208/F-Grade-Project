using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopupCaller : MonoBehaviour
{
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
        if (current == GameManager.GameState.GameOver)
        {
            ShowPopup();
        }
    }
    public void ShowPopup()
    {

        var info = new PopupInfo.Builder()
            .SetTitle("GameOver")
            .SetContent("사용자님은 합격자 명단에 없습니다.\n안타깝네요...")

            // 사용할 버튼 타입만 남기고 지우기
            .SetButtons(
                Enums.PopupButtonType.Confirm
            )
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        StageManager.GetInstance().OnStageFailed(); // 확인 버튼 클릭 시 스테이지 실패 로직 실행
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}