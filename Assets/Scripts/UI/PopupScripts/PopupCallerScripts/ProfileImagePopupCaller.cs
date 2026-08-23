using UnityEngine;

public class ProfileImagePopupCaller : MonoBehaviour
{
    public void ShowPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("프로필 사진")
            .SetProfileOptions()
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        int selected = PopupManager.Instance.GetCurrentSelectedProfileImageIndex();
                        Debug.Log($"[Profile] 선택된 인덱스: {selected}"); // 임시 로그
                        if (selected >= 0)
                            SettingsManager.Instance.TrySetProfileImage(selected);
                        PopupManager.Instance.CloseCurrentActivePopup();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}