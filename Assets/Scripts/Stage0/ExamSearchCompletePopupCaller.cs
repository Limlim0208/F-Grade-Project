using TMPro;
using UnityEngine;

public class ExamSearchCompletePopupCaller : MonoBehaviour
{
    [SerializeField] private LogicTrigger logicTrigger;
    [SerializeField] private TMP_InputField examNumberInput;

    public void ShowPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("")
            .SetContent("수험번호 조회가 완료되었습니다.")
            .SetButtons(
                Enums.PopupButtonType.Confirm
            )
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();

                        // 랜덤 6자리 수험번호 생성 (000000 ~ 999999)
                        int randomNumber = Random.Range(0, 1000000);
                        examNumberInput.text = randomNumber.ToString("D6");

                        logicTrigger.OnClicked();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}
