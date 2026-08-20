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
    [SerializeField] private Button searchButton;      // 조회하기 버튼

    [Header("팝업")]
    [SerializeField] private GameObject warningPopup;  // 인적사항 미입력 경고 팝업
    [SerializeField] private Button warningConfirmButton1; // 확인 버튼 1
    [SerializeField] private Button warningConfirmButton2;  // 확인 버튼 2
    [SerializeField] private LogicTrigger logicTrigger;

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
        GameManager.GetInstance().StartGame(); // 2026-07-19 임유미 추가: 스테이지 진입 시 게임 상태 초기화
        BGMManager.GetInstance().PlayBGM("stage0_main");

        examNumberButton.onClick.AddListener(OnClickExamNumberButton);
        searchButton.onClick.AddListener(OnClickSearchButton);
        warningConfirmButton1.onClick.AddListener(() => warningPopup.SetActive(false));
        warningConfirmButton2.onClick.AddListener(() => warningPopup.SetActive(false));
        examNumberInput.interactable = false; // 수험번호 직접 입력 불가
    }

    void HandleGameStateChanged(GameManager.GameState previous, GameManager.GameState current)
    {
        if (current == GameManager.GameState.StageClear)
        {
            BGMManager.GetInstance().PlayBGM("stage0_success");
        }
    }

    // 수험번호 조회 버튼
    private void OnClickExamNumberButton()
    {
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(birthInput.text))
        {
            warningPopup.SetActive(true);
            return;
        }
        // 랜덤 6자리 생성 (000000 ~ 999999)
        int randomNumber = Random.Range(0, 1000000);
        examNumberInput.text = randomNumber.ToString("D6"); // 앞자리 0 포함 6자리
    }

    // 조회하기 버튼
    private void OnClickSearchButton()
    {
        if (string.IsNullOrEmpty(nameInput.text) ||
            string.IsNullOrEmpty(birthInput.text) ||
            string.IsNullOrEmpty(examNumberInput.text))
        {
            warningPopup.SetActive(true);
            return; // 검사 실패면 여기서 끝
        }
        logicTrigger.OnClicked(); // 검사 통과 후에만 실행
    }
}