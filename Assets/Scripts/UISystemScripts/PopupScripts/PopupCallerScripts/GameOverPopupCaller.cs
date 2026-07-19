using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopupCaller : MonoBehaviour
{
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
                    // 사용할 버튼 타입만 남기고 지우기

                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 확인 버튼을 눌렀을 때 작동할 코드 추가

                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}