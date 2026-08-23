using UnityEngine;

public class NicknameEditPopupCaller : MonoBehaviour
{
    public void ShowPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("이름 변경")
            .SetInput(placeholder: "변경할 이름을 입력해주세요.", maxLength: 8)
            .SetButtons(Enums.PopupButtonType.Confirm, Enums.PopupButtonType.Close)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        string input = PopupManager.Instance.GetCurrentInputText();
                        if (SettingsManager.Instance.TrySetNickname(input, out string error))
                            PopupManager.Instance.CloseCurrentActivePopup();
                        // 실패 시 팝업 유지 (에러 처리는 아래 참고)
                        break;

                    case Enums.PopupButtonType.Close:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}