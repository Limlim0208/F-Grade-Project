using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCallerTemplate : MonoBehaviour
{
    public void ShowPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("여기에 제목을 작성하세요.")
            .SetContent("여기에 내용을 작성하세요.")

            // 사용할 버튼 타입만 남기고 지우기
            .SetButtons(
                Enums.PopupButtonType.None,
                Enums.PopupButtonType.Yes,
                Enums.PopupButtonType.No,
                Enums.PopupButtonType.Confirm,
                Enums.PopupButtonType.Close,
                Enums.PopupButtonType.Replay,
                Enums.PopupButtonType.GoHome,
                Enums.PopupButtonType.FinishGame,
                Enums.PopupButtonType.Start
            )
            .SetListener((type) =>
            {
                switch (type)
                {
                    // 사용할 버튼 타입만 남기고 지우기

                    case Enums.PopupButtonType.None:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // None 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.Yes:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 예 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.No:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 아니오 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 확인 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.Close:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 닫기 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.Replay:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 다시하기 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.GoHome:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        SceneChanger.Instance.LoadScene("StartScene");
                        // 메인화면으로 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.FinishGame:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 게임종료 버튼을 눌렀을 때 작동할 코드 추가
                        break;

                    case Enums.PopupButtonType.Start:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        // 시작 버튼을 눌렀을 때 작동할 코드 추가
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}