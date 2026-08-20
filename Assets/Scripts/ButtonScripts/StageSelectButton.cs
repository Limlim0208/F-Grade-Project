using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] private int stageId;             // 이 버튼이 해당하는 스테이지 번호
    [SerializeField] private string stageActionText;  // 예: "합격 조회를 하러", "수강신청을 하러"
    [SerializeField] private string stageDisplayName; // 예: "합격 조회", "수강신청", "과제", "중간고사"
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text label;

    [Header("해금됐을 때")]
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Color unlockedTextColor = Color.black;

    [Header("잠겨있을 때")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Color lockedTextColor = Color.gray;

    private bool isAccessible;

    void Start()
    {
        RefreshState();
        button.onClick.AddListener(OnClick);
    }

    private void RefreshState()
    {
        isAccessible = stageId <= StageProgressManager.Instance.CurrentStage;

        buttonImage.sprite = isAccessible ? unlockedSprite : lockedSprite;
        label.color = isAccessible ? unlockedTextColor : lockedTextColor;
    }

    private void OnClick()
    {
        if (isAccessible)
            ShowEnterConfirmPopup();
        else
            ShowLockedPopup();
    }

    private void ShowEnterConfirmPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("스테이지 진입")
            .SetContent($"{stageActionText} 가시겠습니까?")
            .SetButtons(Enums.PopupButtonType.Yes, Enums.PopupButtonType.No)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Yes:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        StageProgressManager.Instance.SetStage(stageId);
                        break;

                    case Enums.PopupButtonType.No:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }

    private void ShowLockedPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("스테이지 진입 불가")
            .SetContent($"아직 {stageDisplayName} 시기가 아닙니다...")
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}