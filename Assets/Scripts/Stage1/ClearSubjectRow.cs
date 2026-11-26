using TMPro;
using UnityEngine;

public class ClearSubjectRow : MonoBehaviour
{
    [SerializeField] private TMP_Text subjectNameText;

    public void Bind(string subjectName)
    {
        if (subjectNameText != null)
            subjectNameText.text = subjectName;
    }
}