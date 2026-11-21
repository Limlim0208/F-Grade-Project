using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisteredSubjectRow : MonoBehaviour
{
    [Header("취소 버튼 (Inspector에서 직접 연결)")]
    [SerializeField] private Button cancelButton;

    private TMP_Text noText;
    private TMP_Text gradeText;
    private TMP_Text subjectNumberText;
    private TMP_Text classNumberText;
    private TMP_Text classNameText;
    private TMP_Text classTypeText;
    private TMP_Text creditText;
    private TMP_Text professorText;
    private TMP_Text timeText;
    private TMP_Text departmentText;
    private TMP_Text numberOfPeopleText;

    private int subjectIndex;
    private SubjectData boundSubject;

    void Awake()
    {
        noText = FindTextByName("No-text");
        gradeText = FindTextByName("grade-text");
        subjectNumberText = FindTextByName("subject-number-text");
        classNumberText = FindTextByName("class-number-text");
        classNameText = FindTextByName("class-name-text");
        classTypeText = FindTextByName("class-type-text");
        creditText = FindTextByName("credit-text");
        professorText = FindTextByName("professor-text");
        timeText = FindTextByName("time-text");
        departmentText = FindTextByName("department-text");
        numberOfPeopleText = FindTextByName("number-of-people-text");

        if (cancelButton == null)
            Debug.LogWarning($"[RegisteredSubjectRow] {name}: cancelButton이 Inspector에 연결되어 있지 않음");
    }

    private TMP_Text FindTextByName(string childName)
    {
        Transform found = FindChildByName(transform, childName);
        if (found == null)
        {
            Debug.LogWarning($"[RegisteredSubjectRow] {name}: '{childName}' 오브젝트를 찾지 못함");
            return null;
        }

        TMP_Text text = found.GetComponent<TMP_Text>();
        if (text == null)
            Debug.LogWarning($"[RegisteredSubjectRow] '{childName}'에 TMP_Text 컴포넌트가 없음");

        return text;
    }

    private Transform FindChildByName(Transform parent, string targetName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName)
                return child;

            Transform result = FindChildByName(child, targetName);
            if (result != null)
                return result;
        }
        return null;
    }

    public void Bind(SubjectData subject)
    {
        boundSubject = subject;
        subjectIndex = subject.SubjectIndex;

        if (noText != null) noText.text = subject.No.ToString();
        if (gradeText != null) gradeText.text = subject.Grade;
        if (subjectNumberText != null) subjectNumberText.text = subject.SubjectCode;
        if (classNumberText != null) classNumberText.text = subject.ClassNumber;
        if (classNameText != null) classNameText.text = subject.SubjectName;
        if (classTypeText != null) classTypeText.text = subject.ClassType;
        if (creditText != null) creditText.text = subject.Credit.ToString();
        if (professorText != null) professorText.text = subject.Professor;
        if (timeText != null) timeText.text = subject.ClassTime;
        if (departmentText != null) departmentText.text = subject.Department;
        if (numberOfPeopleText != null) numberOfPeopleText.text = subject.ApplicantDisplayText;

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
    }

    public void RefreshApplicantText()
    {
        if (boundSubject != null && numberOfPeopleText != null)
            numberOfPeopleText.text = boundSubject.ApplicantDisplayText;
    }

    private void OnCancelButtonClicked()
    {
        SubjectManager.GetInstance().RemoveRegisteredSubject(subjectIndex);
    }
}