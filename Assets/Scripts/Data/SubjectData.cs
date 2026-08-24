using System;

[Serializable]
public class SubjectData
{
    // === UI 정보 ===
    public int No;
    public string Grade;
    public string SubjectCode;
    public string ClassNumber;
    public string SubjectName;
    public string ClassType;
    public int Credit;
    public string Professor;
    public string ClassTime;
    public string Department;
    public int Capacity;              // 정원 (nnn)
    public int CurrentApplicants;     // 현재 신청 인원 (0 → 정원까지 변화)

    // 표시용 "0/36" 형태 텍스트
    public string ApplicantDisplayText => $"{CurrentApplicants}/{Capacity}";

    // === 시스템 정보 ===
    public int SubjectIndex;
    public float DeadlineOffsetSeconds; // 서버시각 09:00:00 기준 몇 초 후 마감되는지

    public SubjectData(int no, string grade, string subjectCode, string classNumber, string subjectName,
        string classType, int credit, string professor, string classTime, string department,
        int capacity, int subjectIndex, float deadlineOffsetSeconds)
    {
        No = no;
        Grade = grade;
        SubjectCode = subjectCode;
        ClassNumber = classNumber;
        SubjectName = subjectName;
        ClassType = classType;
        Credit = credit;
        Professor = professor;
        ClassTime = classTime;
        Department = department;
        Capacity = capacity;
        CurrentApplicants = 0; // 시작은 항상 0
        SubjectIndex = subjectIndex;
        DeadlineOffsetSeconds = deadlineOffsetSeconds;
    }
}