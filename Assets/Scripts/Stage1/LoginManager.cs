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

    // [Header("알림 팝업")]
    // public GameObject alertPanel;
    // public TMP_Text alertText;

    [Header("연출")]
    public GameObject inputPopup;
    public Image overlayImage;
    public GameObject checkboxPopup;
    public GameObject minigamePopup;

    [Header("체크박스")]
    public Toggle robotCheckbox;
    public Image checkboxBackground;

    private const string CORRECT_PASSWORD = "0318";

    void Start()
    {
        studentIdField.contentType = TMP_InputField.ContentType.IntegerNumber;
        studentIdField.onValidateInput += ValidateNumberOnly;

        passwordField.contentType = TMP_InputField.ContentType.Password;

        loginButton.onClick.AddListener(OnLoginClicked);

        // alertPanel.SetActive(false);
        overlayImage.gameObject.SetActive(false);
        checkboxPopup.SetActive(false);
        minigamePopup.SetActive(false);
    }

    char ValidateNumberOnly(string text, int charIndex, char addedChar)
    {
        if (char.IsDigit(addedChar)) return addedChar;
        return '\0';
    }

    void OnLoginClicked()
    {
        string studentId = studentIdField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(studentId) || password != CORRECT_PASSWORD)
        {
            StartCoroutine(ShowAlertAndReturn());
            return;
        }

        OnLoginSuccess();
    }

    IEnumerator ShowAlertAndReturn()
    {
        // alertText.text = "올바른 비밀번호를 입력하세요.";
        // alertPanel.SetActive(true);
        // yield return new WaitForSeconds(2f);
        // alertPanel.SetActive(false);

        yield return null;
        SceneChanger.Instance.LoadScene("StageSelectScene");
    }

    void OnLoginSuccess()
    {
        inputPopup.SetActive(false);
        overlayImage.gameObject.SetActive(true);
        checkboxPopup.SetActive(true);

        robotCheckbox.isOn = false;
        checkboxBackground.enabled = true;
        robotCheckbox.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        checkboxBackground.enabled = !isOn;

        if (isOn)
        {
            StartCoroutine(ShowMinigameAfterDelay());
        }
    }

    IEnumerator ShowMinigameAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        checkboxPopup.SetActive(false);
        minigamePopup.SetActive(true);
    }
}