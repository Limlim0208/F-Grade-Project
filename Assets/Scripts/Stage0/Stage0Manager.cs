using UnityEngine;
using UnityEngine.UI;

public class Stage00Manager : MonoBehaviour
{
    [Header("입력 필드")]
    [SerializeField] private InputField nameInput;        // 이름
    [SerializeField] private InputField birthInput;       // 생년월일
    [SerializeField] private InputField examNumberInput;  // 수험번호

    [Header("버튼")]
    [SerializeField] private Button examNumberButton;  // 수험번호 조회 버튼
    [SerializeField] private Button searchButton;      // 조회하기 버튼

    [Header("팝업")]
    [SerializeField] private GameObject warningPopup;  // 인적사항 미입력 경고 팝업
    [SerializeField] private Button warningConfirmButton; // 팝업 확인 버튼

    void Start()
    {
        examNumberButton.onClick.AddListener(OnClickExamNumberButton);
        searchButton.onClick.AddListener(OnClickSearchButton);
        warningConfirmButton.onClick.AddListener(() => warningPopup.SetActive(false));

        examNumberInput.interactable = false; // 수험번호 직접 입력 불가
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
            return;
        }

        // 여기서 다음 로직 시작
        Debug.Log("조회 시작! 수험번호: " + examNumberInput.text);
        // ex) GameManager.GetInstance().StartGame();
    }
}