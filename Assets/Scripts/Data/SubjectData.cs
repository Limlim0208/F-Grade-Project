using System;
using System.Collections.Generic;
using UnityEngine;

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

    private List<float> applicantArrivalTimes; // 신청자별 도착 시각 (오름차순)

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

    /// 신청 오픈 시점(예: AssignRandomRequiredSubjects 호출 시)에 한 번 생성
    public void GenerateApplicantArrivals()
    {
        CurrentApplicants = 0;
        applicantArrivalTimes = new List<float>();

        if (Capacity <= 0)
            return;

        // 마지막 1자리는 반드시 마감 시각에 도착 → 마감 시점에 정원 100% 보장
        for (int i = 0; i < Capacity - 1; i++)
        {
            applicantArrivalTimes.Add(UnityEngine.Random.Range(0f, DeadlineOffsetSeconds));
        }
        applicantArrivalTimes.Add(DeadlineOffsetSeconds);
        applicantArrivalTimes.Sort();
    }

    /// 매 틱 호출. 값이 바뀌었으면 true 반환(불필요한 UI 갱신 방지용).
    public bool UpdateApplicants(float elapsedSeconds)
    {
        int previous = CurrentApplicants;

        if (applicantArrivalTimes == null || applicantArrivalTimes.Count == 0)
            return false;

        if (elapsedSeconds >= DeadlineOffsetSeconds)
        {
            CurrentApplicants = Capacity; // 마감 이후엔 무조건 정원 = 정원
        }
        else
        {
            int count = 0;
            foreach (float t in applicantArrivalTimes)
            {
                if (t <= elapsedSeconds) count++;
                else break; // 정렬돼 있으므로 이후는 다 더 큼
            }
            CurrentApplicants = Mathf.Min(count, Capacity - 1); // 마감 전엔 정원 미만 보장
        }

        return CurrentApplicants != previous;
    }

}