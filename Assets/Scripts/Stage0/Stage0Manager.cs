using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage00Manager : MonoBehaviour
{
    [Header("입력 필드")]
    [SerializeField] private TMP_InputField nameInput;          //이름
    [SerializeField] private TMP_InputField birthInput;         //생년월일
    [SerializeField] private TMP_InputField examNumberInput;    //번호

    [Header("버튼")]
    [SerializeField] private Button examNumberButton;  // 수험번호 조회 버튼

    [Header("팝업")]
    [SerializeField] private GameObject warningPopup;  // 필수정보 미입력 경고 팝업
    [SerializeField] private Button warningConfirmButton1; // 확인 버튼 1
    [SerializeField] private Button warningConfirmButton2;  // 확인 버튼 2

    [SerializeField] private ExamSearchCompletePopupCaller examSearchCompletePopupCaller;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Start()
    {
        GameManager.GetInstance().StartGame(); // 2026-07-19 임유미 추가: 스테이지 시작 시 게임 상태 초기화
        BGMManager.GetInstance().PlayBGM("stage0_bgm");

        examNumberButton.onClick.AddListener(OnClickExamNumberButton);
        warningConfirmButton1.onClick.AddListener(() => warningPopup.SetActive(false));
        warningConfirmButton2.onClick.AddListener(() => warningPopup.SetActive(false));
        examNumberInput.interactable = false; // 수험번호 직접 입력 불가
    }

    void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        if (current == GameManager.GameState.StageClear)
        {
            BGMManager.GetInstance().PlayBGM("clear_bgm");
        }
        else if (current == GameManager.GameState.GameOver)
        {
            BGMManager.GetInstance().PlayBGM("fail_bgm");
        }
    }

    // 수험번호 조회 버튼
    private void OnClickExamNumberButton()
    {
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(birthInput.text))
        {
            warningPopup.SetActive(true);
            SFXManager.GetInstance().PlaySFX("error");
            return;
        }

        examSearchCompletePopupCaller.ShowPopup(); // 팝업 확인 클릭 시 수험번호 생성 + 로직 시작 (ExamSearchCompletePopupCaller에서 처리)
    }
}
