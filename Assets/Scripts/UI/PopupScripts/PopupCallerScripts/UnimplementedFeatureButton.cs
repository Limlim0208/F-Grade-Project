using UnityEngine;
using UnityEngine.UI;

public class UnimplementedFeatureButton : MonoBehaviour
{
    [SerializeField] private Button button;

    void Awake()
    {
        button.onClick.AddListener(ShowUnimplementedPopup);
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