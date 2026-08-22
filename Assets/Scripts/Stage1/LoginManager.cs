using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("�Է� �ʵ�")]
    public TMP_InputField studentIdField;
    public TMP_InputField passwordField;

    [Header("��ư")]
    public Button loginButton;

    // [Header("�˸� �˾�")]
    // public GameObject alertPanel;
    // public TMP_Text alertText;

    [Header("����")]
    public GameObject inputPopup;
    public Image overlayImage;
    public GameObject checkboxPopup;
    public GameObject minigamePopup;

    [Header("üũ�ڽ�")]
    public Toggle robotCheckbox;
    public Image checkboxBackground;

    private const string CORRECT_PASSWORD = "0318";

    void Start()
    {
        studentIdField.contentType = TMP_InputField.ContentType.IntegerNumber;
        studentIdField.onValidateInput += ValidateNumberOnly;

        passwordField.contentType = TMP_InputField.ContentType.Password;

        loginButton.onClick.AddListener(OnLoginClicked);

        BGMManager.GetInstance().PlayBGM("stage1_bgm");

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
        // alertText.text = "�ùٸ� ��й�ȣ�� �Է��ϼ���.";
        // alertPanel.SetActive(true);
        // yield return new WaitForSeconds(2f);
        // alertPanel.SetActive(false);

        SFXManager.GetInstance().PlaySFX("error");

        yield return null;
        SceneChanger.Instance.LoadScene("StageSelectScene");
    }

    void OnLoginSuccess()
    {
        inputPopup.SetActive(false);
        overlayImage.gameObject.SetActive(true);
        StartCoroutine(ShowCheckboxAfterDelay());
    }

    IEnumerator ShowCheckboxAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(1f);
        checkboxPopup.SetActive(false);
        minigamePopup.SetActive(true);
    }
}