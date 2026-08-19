using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stage00Manager : MonoBehaviour
{
    [Header("�Է� �ʵ�")]
    [SerializeField] private TMP_InputField nameInput;          //�̸�
    [SerializeField] private TMP_InputField birthInput;         //�������
    [SerializeField] private TMP_InputField examNumberInput;    //��ȣ

    [Header("��ư")]
    [SerializeField] private Button examNumberButton;  // �����ȣ ��ȸ ��ư

    [Header("�˾�")]
    [SerializeField] private GameObject warningPopup;  // �������� ���Է� ��� �˾�
    [SerializeField] private Button warningConfirmButton1; // Ȯ�� ��ư 1
    [SerializeField] private Button warningConfirmButton2;  // Ȯ�� ��ư 2

    [SerializeField] private ExamSearchCompletePopupCaller examSearchCompletePopupCaller;

    void Start()
    {
        GameManager.GetInstance().StartGame(); // 2026-07-19 ������ �߰�: �������� ���� �� ���� ���� �ʱ�ȭ
        examNumberButton.onClick.AddListener(OnClickExamNumberButton);
        warningConfirmButton1.onClick.AddListener(() => warningPopup.SetActive(false));
        warningConfirmButton2.onClick.AddListener(() => warningPopup.SetActive(false));

        examNumberInput.interactable = false; // �����ȣ ���� �Է� �Ұ�
    }

    // �����ȣ ��ȸ ��ư
    private void OnClickExamNumberButton()
    {
        if (string.IsNullOrEmpty(nameInput.text) || string.IsNullOrEmpty(birthInput.text))
        {
            warningPopup.SetActive(true);
            return;
        }

        examSearchCompletePopupCaller.ShowPopup(); // �˾� Ȯ�� Ŭ�� �� ���ȣ ���� + ���� ���� (ExamSearchCompletePopupCaller���� ó��)
    }
}