using UnityEngine;
using UnityEngine.UI;

public class UnimplementedFeatureToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (!isOn) return;

        ShowUnimplementedPopup();

        // 안내만 띄우고 다시 꺼진 상태로 되돌림
        toggle.SetIsOnWithoutNotify(false);
    }

    private void ShowUnimplementedPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("이런!")
            .SetContent("아직 이 기능은 구현 못 했는데...")
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) => PopupManager.Instance.CloseCurrentActivePopup())
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}