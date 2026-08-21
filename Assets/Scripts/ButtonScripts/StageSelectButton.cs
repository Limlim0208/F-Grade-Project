using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SceneChangeButton))]
public class StageSelectButton : MonoBehaviour
{
    [Header("스테이지 정보")]
    [SerializeField] private string stageName;   // 팝업 문구용 이름 (ex. "1스테이지")
    // stageId는 SceneChangeButton 쪽 값을 그대로 사용 (중복 관리 X)

    [Header("버튼 컴포넌트")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;

    [Header("상태별 비주얼")]
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Color unlockedTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.gray;

    private SceneChangeButton sceneChangeButton;
    private bool isUnlocked;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        sceneChangeButton = GetComponent<SceneChangeButton>();

        button.onClick.AddListener(OnClickStageButton);
    }

    private void OnEnable()
    {
        RefreshState();
    }

    public void RefreshState()
    {
        isUnlocked = StageProgressManager.GetInstance().IsStageUnlocked(sceneChangeButton.StageId);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (buttonImage != null)
            buttonImage.sprite = isUnlocked ? unlockedSprite : lockedSprite;

        if (buttonText != null)
            buttonText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
    }

    private void OnClickStageButton()
    {
        if (isUnlocked)
            ShowEnterConfirmPopup();
        else
            ShowLockedPopup();
    }

    private void ShowEnterConfirmPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("스테이지 진입")
            .SetContent($"{stageName}을(를) 하시겠습니까?")
            .SetButtons(Enums.PopupButtonType.Yes, Enums.PopupButtonType.No)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Yes:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        sceneChangeButton.OnClick(); // 실제 이동은 SceneChangeButton에 위임
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
            .SetContent($"아직 {stageName} 시기가 아닙니다..")
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) =>
            {
                if (type == Enums.PopupButtonType.Confirm)
                    PopupManager.Instance.CloseCurrentActivePopup();
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}