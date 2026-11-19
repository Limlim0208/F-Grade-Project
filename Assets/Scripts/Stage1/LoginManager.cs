using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("입력 필드")]
    public TMP_InputField studentIdField;
    public TMP_InputField passwordField;

    [Header("버튼")]
    public Button loginButton;

    [Header("팝업")]
    public GameObject inputPopup;
    public Image overlayImage;
    public GameObject checkboxPopup;
    public GameObject minigamePopup;

    [Header("체크박스")]
    public Toggle robotCheckbox;
    public Image checkboxBackground;

    private const string CORRECT_PASSWORD = "0318";

    private void Start()
    {
        studentIdField.contentType = TMP_InputField.ContentType.IntegerNumber;
        studentIdField.onValidateInput += ValidateNumberOnly;

        passwordField.contentType = TMP_InputField.ContentType.Password;

        loginButton.onClick.AddListener(OnLoginClicked);

        BGMManager.GetInstance().PlayBGM("stage1_bgm");

        overlayImage.gameObject.SetActive(false);
        checkboxPopup.SetActive(false);
        minigamePopup.SetActive(false);
    }

    private char ValidateNumberOnly(string text, int charIndex, char addedChar)
    {
        if (char.IsDigit(addedChar)) return addedChar;
        return '\0';
    }

    private void OnLoginClicked()
    {
        string studentId = studentIdField.text.Trim();
        string password = passwordField.text.Trim();

        // 1. 학번 미입력
        if (string.IsNullOrEmpty(studentId))
        {
            ShowErrorPopup(
                "로그인 실패",
                "학번을 입력해주세요.",
                () => studentIdField.ActivateInputField()
            );
            return;
        }

        // 2. 비밀번호 불일치
        if (password != CORRECT_PASSWORD)
        {
            ShowErrorPopup(
                "로그인 실패",
                "비밀번호가 틀렸습니다.",
                () =>
                {
                    passwordField.text = string.Empty;
                    passwordField.ActivateInputField();
                }
            );
            return;
        }

        OnLoginSuccess();
    }

    private void ShowErrorPopup(string title, string content, System.Action onConfirm)
    {
        SFXManager.GetInstance().PlaySFX("error");

        var info = new PopupInfo.Builder()
            .SetTitle(title)
            .SetContent(content)
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) =>
            {
                switch (type)
                {
                    case Enums.PopupButtonType.Confirm:
                        PopupManager.Instance.CloseCurrentActivePopup();
                        onConfirm?.Invoke();
                        break;
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }

    private void OnLoginSuccess()
    {
        inputPopup.SetActive(false);
        overlayImage.gameObject.SetActive(true);
        StartCoroutine(ShowCheckboxAfterDelay());
    }

    private IEnumerator ShowCheckboxAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        checkboxPopup.SetActive(true);
        robotCheckbox.isOn = false;
        checkboxBackground.enabled = true;
        robotCheckbox.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        checkboxBackground.enabled = !isOn;

        if (isOn)
        {
            StartCoroutine(ShowMinigameAfterDelay());
        }
    }

    private IEnumerator ShowMinigameAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        checkboxPopup.SetActive(false);
        minigamePopup.SetActive(true);
    }
}